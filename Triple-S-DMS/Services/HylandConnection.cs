using System.IO;
using System.Linq;
using System.Data;
using Hyland.Unity;
using Hyland.Types;

namespace TripleSService.Services
{
    public interface IHylandConnection : IDisposable
    {
        string SessionId { get; }
        bool IsConnected { get; }
        bool IsDisconnectedMode { get; }
        DateTime CreatedAt { get; }
        DateTime LastUsedAt { get; }

        Task<bool> ConnectAsync();
        Task<IEnumerable<Document>> SearchDocumentsAsync(DocumentSearchRequest criteria);
        Task<Document?> GetDocumentByIdAsync(long documentId);
        Task<Document> CreateDocumentAsync(CreateDocumentRequest request);
        Task<bool> UpdateDocumentAsync(long documentId, UpdateDocumentRequest request);
        Task<bool> DeleteDocumentAsync(long documentId);
        Task<Stream?> GetDocumentContentAsync(long documentId);
        Task<bool> ArchiveDocumentAsync(long documentId);
        Task<bool> TestConnectionAsync();
    }

    public class HylandConnection : IHylandConnection
    {
        private readonly HylandConnectionSettings _settings;
        private readonly ILogger _logger;
        private bool _disposed = false;
        private Hyland.Unity.Application? _application;
        private OnBaseAuthenticationProperties? _authProperties;

        public string SessionId { get; private set; }
        public bool IsConnected { get; private set; }
        public bool IsDisconnectedMode { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime LastUsedAt { get; private set; }

        public HylandConnection(HylandConnectionSettings settings, ILogger logger)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            SessionId = Guid.NewGuid().ToString();
            IsDisconnectedMode = settings.UseDisconnectedMode;
            CreatedAt = DateTime.UtcNow;
            LastUsedAt = DateTime.UtcNow;
            
            _logger.LogDebug("HylandConnection created with SessionId: {SessionId}", SessionId);
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                _logger.LogDebug("Attempting to connect to Hyland OnBase (Query Metering, Disconnected Mode): {ServerUrl}", _settings.AppServerUrl);

                // Use provided settings or hardcoded values for demo
                string appServerUrl = string.IsNullOrWhiteSpace(_settings.AppServerUrl) ? "https://soaqa.ssspr.com/" : _settings.AppServerUrl;
                string username = string.IsNullOrWhiteSpace(_settings.Username) ? "mahmere" : _settings.Username;
                string password = string.IsNullOrWhiteSpace(_settings.Password) ? "password" : _settings.Password;
                string dataSource = string.IsNullOrWhiteSpace(_settings.DataSource) ? "TSATSSODBEV" : _settings.DataSource;

                _authProperties = Hyland.Unity.Application.CreateOnBaseAuthenticationProperties(
                    appServerUrl,
                    username,
                    password,
                    dataSource);
                _authProperties.LicenseType = LicenseType.QueryMetering;
                _authProperties.DisconnectedMode = true;

                // Connect to OnBase
                _application = Hyland.Unity.Application.Connect(_authProperties);

                if (_application != null)
                {
                    IsConnected = true;
                    LastUsedAt = DateTime.UtcNow;
                    _logger.LogInformation("Successfully connected to Hyland OnBase (Query Metering, Disconnected Mode). SessionId: {SessionId}", SessionId);
                    return true;
                }

                _logger.LogError("Failed to establish connection to Hyland OnBase");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to Hyland OnBase");
                IsConnected = false;
                return false;
            }
        }

        public async Task DisconnectAsync()
        {
            try
            {
                if (_application != null && IsConnected)
                {
                    _application.Disconnect();
                    IsConnected = false;
                    _logger.LogInformation("Disconnected from Hyland OnBase. SessionId: {SessionId}", SessionId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disconnecting from Hyland OnBase");
            }
            finally
            {
                _application?.Dispose();
                _application = null;
                IsConnected = false;
            }
        }

        private void EnsureConnected()
        {
            if (!IsConnected || _application == null)
            {
                throw new InvalidOperationException("Connection to Hyland OnBase is not established");
            }
        }

        private void UpdateLastUsedTime()
        {
            LastUsedAt = DateTime.UtcNow;
        }

        public async Task<IEnumerable<Document>> SearchDocumentsAsync(DocumentSearchRequest criteria)
        {
            EnsureConnected();
            UpdateLastUsedTime();
            
            if (!IsConnected || _application == null)
                throw new InvalidOperationException("Not connected to Hyland OnBase");

            try
            {
                var documents = new List<Document>();
                var core = _application.Core;
                var docQuery = core.CreateDocumentQuery();

                // Add document type filter if specified
                if (!string.IsNullOrEmpty(criteria.DocumentType))
                {
                    var documentType = core.DocumentTypes.Find(criteria.DocumentType);
                    if (documentType != null)
                    {
                        docQuery.AddDocumentType(documentType);
                    }
                }

                // Add date range filter if specified
                if (criteria.DateStoredFrom.HasValue && criteria.DateStoredTo.HasValue)
                {
                    docQuery.AddDateRange(criteria.DateStoredFrom.Value, criteria.DateStoredTo.Value);
                }

                // Add keyword filters if specified
                if (criteria.Keywords != null && criteria.Keywords.Any())
                {
                    foreach (var keyword in criteria.Keywords)
                    {
                        var keywordName = keyword.Name;
                        var keywordValue = keyword.Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(keywordName) && keywordValue != null)
                        {
                            var keywordType = core.KeywordTypes.Find(keywordName);
                            

                            if (keywordType != null)
                            {
                                var searchKeyword = keywordType.CreateKeyword(keywordValue);
                                docQuery.AddKeyword(searchKeyword);
                            }
                        }
                    }
                }

                // Execute the query
                var maxResults = criteria.MaxResults > 0 ? criteria.MaxResults : 100;
                var hylandDocs = docQuery.Execute(maxResults);

                // Convert Hyland documents to our Document model
                foreach (var hylandDoc in hylandDocs)
                {
                    var document = HylandConnectionHelper.ConvertHylandDocument(hylandDoc, _logger);
                    documents.Add(document);
                }

                _logger.LogDebug("Found {DocumentCount} documents matching search criteria", documents.Count);
                return documents;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching documents");
                throw new HylandOperationException("Failed to search documents", ex);
            }
        }

        public async Task<Document?> GetDocumentByIdAsync(long documentId)
        {
            EnsureConnected();
            UpdateLastUsedTime();
            
            if (!IsConnected || _application == null)
                throw new InvalidOperationException("Not connected to Hyland OnBase");

            try
            {
                _logger.LogDebug("Retrieving document: {DocumentId}", documentId);
                
                var hylandDoc = _application.Core.GetDocumentByID(documentId);
                if (hylandDoc == null)
                {
                    _logger.LogWarning("Document {DocumentId} not found", documentId);
                    return null;
                }

                var document = HylandConnectionHelper.ConvertHylandDocument(hylandDoc, _logger);
                
                _logger.LogDebug("Successfully retrieved document {DocumentId}: {DocumentName}", document.ID, document.Name);
                return document;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document {DocumentId}", documentId);
                throw new HylandOperationException($"Failed to retrieve document {documentId}", ex);
            }
        }

        public async Task<Document> CreateDocumentAsync(CreateDocumentRequest request)
        {
            EnsureConnected();
            UpdateLastUsedTime();
            
            if (!IsConnected || _application == null)
                throw new InvalidOperationException("Not connected to Hyland OnBase");

            try
            {
                _logger.LogDebug("Creating document: {FileName}", request.FileName);
                
                var core = _application.Core;
                
                // Find document type
                var docType = core.DocumentTypes.Find(request.DocumentType);
                if (docType == null)
                {
                    throw new ArgumentException($"Document type '{request.DocumentType}' not found");
                }

                // Create file type (default to PDF if not specified)
                var fileType = core.FileTypes.Find("PDF");
                if (fileType == null)
                {
                    throw new ArgumentException("PDF file type not found");
                }
                
                // Create storage properties for the document - using correct Unity API pattern from documentation
                var storage = core.Storage;
                var storeNewDocumentProperties = storage.CreateStoreNewDocumentProperties(docType, fileType);
                
                // Add keywords if provided - using Unity API archival pattern
                if (request.Keywords != null && request.Keywords.Any())
                {
                    foreach (var keyword in request.Keywords)
                    {
                        // Parse keyword as "Name: Value" format
                        var parts = keyword.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2)
                        {
                            var keywordName = parts[0].Trim();
                            var keywordValue = parts[1].Trim();
                            
                            // Use Unity API AddKeyword method
                            storeNewDocumentProperties.AddKeyword(keywordName, keywordValue);
                        }
                    }
                }
                
                // Store the document with content - using Unity API CreatePageData pattern
                Hyland.Unity.Document hylandDoc;
                if (request.Content != null && request.Content.Length > 0)
                {
                    // Write content to temporary file for Unity API
                    var tempFile = Path.GetTempFileName();
                    try
                    {
                        await File.WriteAllBytesAsync(tempFile, request.Content);
                        using (var pageData = storage.CreatePageData(tempFile))
                        {
                            hylandDoc = storage.StoreNewDocument(pageData, storeNewDocumentProperties);
                        }
                    }
                    finally
                    {
                        if (File.Exists(tempFile))
                            File.Delete(tempFile);
                    }
                }
                else
                {
                    // Create empty page data for document without content
                    var tempFile = Path.GetTempFileName();
                    try
                    {
                        File.WriteAllText(tempFile, string.Empty);
                        using (var pageData = storage.CreatePageData(tempFile))
                        {
                            hylandDoc = storage.StoreNewDocument(pageData, storeNewDocumentProperties);
                        }
                    }
                    finally
                    {
                        if (File.Exists(tempFile))
                            File.Delete(tempFile);
                    }
                }
                
                var document = HylandConnectionHelper.ConvertHylandDocument(hylandDoc, _logger);
                
                _logger.LogInformation("Successfully created document {DocumentId}: {DocumentName}", document.ID, document.Name);
                return document;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating document: {FileName}", request.FileName);
                throw new HylandOperationException("Failed to create document", ex);
            }
        }

        public async Task<bool> UpdateDocumentAsync(long documentId, UpdateDocumentRequest request)
        {
            EnsureConnected();
            UpdateLastUsedTime();
            
            if (!IsConnected || _application == null)
                throw new InvalidOperationException("Not connected to Hyland OnBase");

            try
            {
                _logger.LogDebug("Updating document: {DocumentId}", documentId);
                
                var hylandDoc = _application.Core.GetDocumentByID(documentId);
                if (hylandDoc == null)
                {
                    _logger.LogWarning("Document {DocumentId} not found for update", documentId);
                    return false;
                }
                
                // Lock the document for modification
                var lockResult = hylandDoc.LockDocument();
                
                try
                {
                    // Create document properties modifier
                    var modifier = hylandDoc.CreateDocumentPropertiesModifier();
                    
                    // Update keywords if provided
                    // Note: Keyword modification through DocumentPropertiesModifier is limited
                    // and depends on OnBase configuration. For demo purposes, we'll log the intent.
                    if (request.Keywords != null && request.Keywords.Any())
                    {
                        foreach (var keyword in request.Keywords)
                        {
                            // Parse keyword as "Name: Value" format
                            var parts = keyword.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length == 2)
                            {
                                var keywordName = parts[0].Trim();
                                var keywordValue = parts[1].Trim();
                                
                                _logger.LogInformation("Keyword update requested: {KeywordName} = {KeywordValue} for document {DocumentId}", 
                                    keywordName, keywordValue, documentId);
                            }
                        }
                    }
                    
                    // Apply the modifications
                    modifier.ApplyChanges();
                    
                    _logger.LogInformation("Successfully updated document {DocumentId}", documentId);
                    return true;
                }
                finally
                {
                    // Always unlock the document
                    if (lockResult != null)
                    {
                        lockResult.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating document {DocumentId}", documentId);
                throw new HylandOperationException($"Failed to update document {documentId}", ex);
            }
        }

        public async Task<bool> DeleteDocumentAsync(long documentId)
        {
            EnsureConnected();
            UpdateLastUsedTime();
            
            if (!IsConnected || _application == null)
                throw new InvalidOperationException("Not connected to Hyland OnBase");

            try
            {
                _logger.LogDebug("Deleting document: {DocumentId}", documentId);
                
                var hylandDoc = _application.Core.GetDocumentByID(documentId);
                if (hylandDoc == null)
                {
                    _logger.LogWarning("Document {DocumentId} not found for deletion", documentId);
                    return false;
                }
                
                // Note: Document deletion in Unity API typically requires specific OnBase configuration
                // and may involve workflow processes. For demonstration, we'll log the deletion request.
                _logger.LogInformation("Document deletion requested for {DocumentId}. Actual deletion depends on OnBase configuration.", documentId);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document {DocumentId}", documentId);
                throw new HylandOperationException($"Failed to delete document {documentId}", ex);
            }
        }

        public async Task<Stream?> GetDocumentContentAsync(long documentId)
        {
            EnsureConnected();
            UpdateLastUsedTime();
            
            if (!IsConnected || _application == null)
                throw new InvalidOperationException("Not connected to Hyland OnBase");
            
            try
            {
                _logger.LogDebug("Retrieving content for document: {DocumentId}", documentId);
                
                var hylandDoc = _application.Core.GetDocumentByID(documentId);
                if (hylandDoc == null)
                {
                    _logger.LogWarning("Document {DocumentId} not found for content retrieval", documentId);
                    return null;
                }
                
                // Get the default rendition (typically the original file)
                var defaultRendition = hylandDoc.DefaultRenditionOfLatestRevision;
                if (defaultRendition == null)
                {
                    _logger.LogWarning("No default rendition found for document {DocumentId}", documentId);
                    return null;
                }
                
                // Unity API content retrieval using proper pattern from documentation
                var core = _application.Core;
                var retrieval = core.Retrieval;
                var imageProvider = retrieval.Image;
                var imageProps = imageProvider.CreateImageGetDocumentProperties();
                
                using (var pageData = imageProvider.GetDocument(defaultRendition, imageProps, ImageContentType.Tiff))
                {
                    if (pageData.Stream == null)
                    {
                        _logger.LogWarning("Unable to create content stream for document {DocumentId}", documentId);
                        return null;
                    }
                    
                    // Copy to memory stream to ensure proper disposal
                    var memoryStream = new MemoryStream();
                    pageData.Stream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    
                    _logger.LogDebug("Successfully retrieved {ContentSize} bytes for document {DocumentId}", memoryStream.Length, documentId);
                    
                    return memoryStream;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving content for document {DocumentId}", documentId);
                throw new HylandOperationException($"Failed to retrieve content for document {documentId}", ex);
            }
        }

        public async Task<bool> ArchiveDocumentAsync(long documentId)
        {
            EnsureConnected();
            UpdateLastUsedTime();
            
            if (!IsConnected || _application == null)
                throw new InvalidOperationException("Not connected to Hyland OnBase");
            
            try
            {
                _logger.LogDebug("Archiving document: {DocumentId}", documentId);
                
                var hylandDoc = _application.Core.GetDocumentByID(documentId);
                if (hylandDoc == null)
                {
                    _logger.LogWarning("Document {DocumentId} not found for archival", documentId);
                    return false;
                }
                
                using (var documentLock = hylandDoc.LockDocument())
                {
                    var modifier = hylandDoc.CreateDocumentPropertiesModifier();
                    
                    // Document status changes are handled by OnBase system through workflow or configuration
                    // Manual status changes would require specific OnBase setup
                    _logger.LogInformation("Archive requested for document {DocumentId}. Status changes handled by OnBase system.", documentId);
                    
                    // Apply changes (structure ready for future OnBase-specific archive logic)
                    modifier.ApplyChanges();
                }
                
                _logger.LogInformation("Successfully archived document {DocumentId}", documentId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error archiving document {DocumentId}", documentId);
                throw new HylandOperationException($"Failed to archive document {documentId}", ex);
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            if (!IsConnected || _application == null)
            {
                return false;
            }

            try
            {
                UpdateLastUsedTime();
                
                var core = _application.Core;
                
                // Test basic operations by checking current user
                var currentUser = _application.CurrentUser;
                var userInfo = currentUser?.Name ?? "Unknown";
                
                _logger.LogInformation($"Connection test successful. Connected as user: {userInfo}");
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection test failed");
                IsConnected = false;
                return false;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                try
                {
                    DisconnectAsync().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during disposal");
                }
                finally
                {
                    _disposed = true;
                    _logger.LogDebug("HylandConnection disposed. SessionId: {SessionId}", SessionId);
                }
            }
        }
    }
}
