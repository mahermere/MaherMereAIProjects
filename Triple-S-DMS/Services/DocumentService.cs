namespace TripleSService.Services
{
    public interface IDocumentService
    {
        Task<Document?> GetDocumentByIdAsync(string id);
        Task<Document> CreateDocumentAsync(CreateDocumentRequest request);
        Task<Document?> UpdateDocumentAsync(string id, UpdateDocumentRequest request);
        Task<bool> DeleteDocumentAsync(string id);
        Task<IEnumerable<Document>> SearchDocumentsAsync(DocumentSearchRequest searchRequest);
        Task<bool> ArchiveDocumentAsync(string id);
        Task<byte[]?> GetDocumentContentAsync(string id);
        Task<QueryMeteringStatus> GetQueryMeteringStatusAsync();
        void ConfigureQueryMetering(int maxQueriesPerHour, int warningThreshold = 80);
    }

    public class DocumentService : IDocumentService, IDisposable
    {
        private readonly IHylandConnectionFactory _connectionFactory;
        private readonly ILogger<DocumentService> _logger;
        private bool _disposed = false;

        public DocumentService(
            IHylandConnectionFactory connectionFactory,
            ILogger<DocumentService> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Document?> GetDocumentByIdAsync(string id)
        {
            try
            {
                if (!long.TryParse(id, out var documentId))
                {
                    _logger.LogWarning("Invalid document ID format: {DocumentId}", id);
                    return null;
                }
                
                using var connection = await _connectionFactory.CreateDisconnectedConnectionAsync();
                var document = await connection.GetDocumentByIdAsync(documentId);
                
                if (document == null)
                {
                    _logger.LogWarning("Document {DocumentId} not found", id);
                    return null;
                }

                _logger.LogDebug("Retrieved document {DocumentId} from Hyland", id);
                return document;
            }
            catch (QueryLimitExceededException ex)
            {
                _logger.LogWarning(ex, "Query limit exceeded while retrieving document {DocumentId}", id);
                throw new InvalidOperationException("Query limit exceeded. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document {DocumentId} from Hyland", id);
                throw new InvalidOperationException($"Failed to retrieve document {id}", ex);
            }
        }

        public async Task<Document> CreateDocumentAsync(CreateDocumentRequest request)
        {
            try
            {
                using var connection = await _connectionFactory.CreateDisconnectedConnectionAsync();
                var document = await connection.CreateDocumentAsync(request);
                
                _logger.LogInformation("Created document {DocumentId} in Hyland", document.ID);
                return document;
            }
            catch (QueryLimitExceededException ex)
            {
                _logger.LogWarning(ex, "Query limit exceeded while creating document");
                throw new InvalidOperationException("Query limit exceeded. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating document in Hyland");
                throw new InvalidOperationException("Failed to create document", ex);
            }
        }

        public async Task<bool> UpdateDocumentByIdAsync(string id, UpdateDocumentRequest request)
        {
            try
            {
                if (!long.TryParse(id, out var documentId))
                {
                    _logger.LogWarning("Invalid document ID format: {DocumentId}", id);
                    return false;
                }
                
                using var connection = await _connectionFactory.CreateDisconnectedConnectionAsync();
                var success = await connection.UpdateDocumentAsync(documentId, request);
                
                if (success)
                {
                    _logger.LogInformation("Updated document {DocumentId} in Hyland", id);
                }
                else
                {
                    _logger.LogWarning("Failed to update document {DocumentId} in Hyland", id);
                }
                
                return success;
            }
            catch (QueryLimitExceededException ex)
            {
                _logger.LogWarning(ex, "Query limit exceeded while creating document");
                throw new InvalidOperationException("Query limit exceeded. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating document in Hyland");
                throw new InvalidOperationException("Failed to create document", ex);
            }
        }

        public async Task<Document?> UpdateDocumentAsync(string id, UpdateDocumentRequest request)
        {
            try
            {
                if (!long.TryParse(id, out var documentId))
                {
                    _logger.LogWarning("Invalid document ID format: {DocumentId}", id);
                    return null;
                }
                
                using var connection = await _connectionFactory.CreateDisconnectedConnectionAsync();
                
                var success = await connection.UpdateDocumentAsync(documentId, request);
                if (!success)
                {
                    _logger.LogWarning("Failed to update document {DocumentId} in Hyland", id);
                    return null;
                }

                // Retrieve updated document
                var updatedDocument = await connection.GetDocumentByIdAsync(documentId);
                if (updatedDocument == null)
                {
                    _logger.LogWarning("Updated document {DocumentId} not found", id);
                    return null;
                }

                updatedDocument.ModifiedDate = DateTime.UtcNow;
                
                _logger.LogInformation("Updated document {DocumentId} in Hyland", id);
                return updatedDocument;
            }
            catch (QueryLimitExceededException ex)
            {
                _logger.LogWarning(ex, "Query limit exceeded while updating document {DocumentId}", id);
                throw new InvalidOperationException("Query limit exceeded. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating document {DocumentId} in Hyland", id);
                throw new InvalidOperationException($"Failed to update document {id}", ex);
            }
        }

        public async Task<bool> DeleteDocumentAsync(string id)
        {
            try
            {
                if (!long.TryParse(id, out var documentId))
                {
                    _logger.LogWarning("Invalid document ID format: {DocumentId}", id);
                    return false;
                }
                
                using var connection = await _connectionFactory.CreateDisconnectedConnectionAsync();
                var success = await connection.DeleteDocumentAsync(documentId);
                
                if (success)
                {
                    _logger.LogInformation("Deleted document {DocumentId} from Hyland", id);
                }
                else
                {
                    _logger.LogWarning("Failed to delete document {DocumentId} from Hyland", id);
                }
                
                return success;
            }
            catch (QueryLimitExceededException ex)
            {
                _logger.LogWarning(ex, "Query limit exceeded while deleting document {DocumentId}", id);
                throw new InvalidOperationException("Query limit exceeded. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document {DocumentId} from Hyland", id);
                throw new InvalidOperationException($"Failed to delete document {id}", ex);
            }
        }

        public async Task<IEnumerable<Document>> SearchDocumentsAsync(DocumentSearchRequest searchRequest)
        {
            try
            {
                using var connection = await _connectionFactory.CreateDisconnectedConnectionAsync();
                var documents = await connection.SearchDocumentsAsync(searchRequest);
                
                _logger.LogDebug("Found {DocumentCount} documents matching search criteria", documents.Count());
                return documents;
            }
            catch (QueryLimitExceededException ex)
            {
                _logger.LogWarning(ex, "Query limit exceeded while searching documents");
                throw new InvalidOperationException("Query limit exceeded. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching documents in Hyland");
                throw new InvalidOperationException("Failed to search documents", ex);
            }
        }

        public async Task<bool> ArchiveDocumentAsync(string id)
        {
            try
            {
                if (!long.TryParse(id, out var documentId))
                {
                    _logger.LogWarning("Invalid document ID format: {DocumentId}", id);
                    return false;
                }
                
                using var connection = await _connectionFactory.CreateDisconnectedConnectionAsync();
                var success = await connection.ArchiveDocumentAsync(documentId);
                
                if (success)
                {
                    _logger.LogInformation("Archived document {DocumentId} in Hyland", id);
                }
                else
                {
                    _logger.LogWarning("Failed to archive document {DocumentId} in Hyland", id);
                }
                
                return success;
            }
            catch (QueryLimitExceededException ex)
            {
                _logger.LogWarning(ex, "Query limit exceeded while archiving document {DocumentId}", id);
                throw new InvalidOperationException("Query limit exceeded. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error archiving document {DocumentId} in Hyland", id);
                throw new InvalidOperationException($"Failed to archive document {id}", ex);
            }
        }

        public async Task<byte[]?> GetDocumentContentAsync(string id)
        {
            try
            {
                if (!long.TryParse(id, out var documentId))
                {
                    _logger.LogWarning("Invalid document ID format: {DocumentId}", id);
                    return null;
                }
                
                using var connection = await _connectionFactory.CreateDisconnectedConnectionAsync();
                var contentStream = await connection.GetDocumentContentAsync(documentId);
                
                if (contentStream == null)
                {
                    _logger.LogWarning("No content found for document {DocumentId}", id);
                    return null;
                }

                using var memoryStream = new MemoryStream();
                await contentStream.CopyToAsync(memoryStream);
                
                var content = memoryStream.ToArray();
                _logger.LogDebug("Retrieved {ContentSize} bytes of content for document {DocumentId}", 
                    content.Length, id);
                
                return content;
            }
            catch (QueryLimitExceededException ex)
            {
                _logger.LogWarning(ex, "Query limit exceeded while retrieving content for document {DocumentId}", id);
                throw new InvalidOperationException("Query limit exceeded. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving content for document {DocumentId} from Hyland", id);
                throw new InvalidOperationException($"Failed to retrieve content for document {id}", ex);
            }
        }

        public async Task<QueryMeteringStatus> GetQueryMeteringStatusAsync()
        {
            return await _connectionFactory.GetQueryMeteringStatusAsync();
        }

        public void ConfigureQueryMetering(int maxQueriesPerHour, int warningThreshold = 80)
        {
            _connectionFactory.ConfigureQueryMetering(maxQueriesPerHour, warningThreshold);
        }

        public void Dispose()
        {
            if (_disposed) return;
            
            _connectionFactory?.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}