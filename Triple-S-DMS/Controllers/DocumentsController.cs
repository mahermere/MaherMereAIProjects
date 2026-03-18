using Microsoft.AspNetCore.Mvc;
using TripleSService.Services;

namespace TripleSService.Controllers
{
    [ApiController]
    [Route("api/documents")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(
            IDocumentService documentService,
            ILogger<DocumentsController> logger)
        {
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Search for documents using specific criteria. For security and performance reasons,
        /// retrieving all documents is not allowed in disconnected mode.
        /// </summary>
        /// <param name="documentType">Optional document type filter</param>
        /// <param name="status">Optional document status filter</param>
        /// <param name="maxResults">Maximum number of results to return (capped at 100)</param>
        /// <returns>A list of documents matching the search criteria</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Document>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Document>>> SearchDocuments([FromQuery] string? documentType = null, 
            [FromQuery] DocumentStatus? status = null,
            [FromQuery] int maxResults = 50)
        {
            try
            {
                // Require at least one search criterion for security
                if (string.IsNullOrWhiteSpace(documentType) && status == null)
                {
                    return BadRequest(new { error = "At least one search criterion is required (documentType or status)" });
                }

                var searchRequest = new DocumentSearchRequest
                {
                    DocumentType = documentType,
                    Status = status,
                    MaxResults = Math.Min(maxResults, 100) // Cap at 100 for performance
                };

                _logger.LogDebug("Searching documents with criteria: DocumentType={DocumentType}, Status={Status}", 
                    documentType, status);
                
                var documents = await _documentService.SearchDocumentsAsync(searchRequest);
                
                return Ok(documents);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Query limit exceeded"))
            {
                _logger.LogWarning(ex, "Query limit exceeded while searching documents");
                return StatusCode(429, new { error = "Query limit exceeded. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching documents from Hyland");
                return StatusCode(500, new { error = "An error occurred while searching documents" });
            }
        }

        /// <summary>
        /// Gets a specific document by ID from Hyland
        /// </summary>
        /// <param name="id">The document ID</param>
        /// <returns>The requested document</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Document), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Document>> GetDocument(string id)
        {
            try
            {
                _logger.LogDebug("Retrieving document {DocumentId} from Hyland", id);
                var document = await _documentService.GetDocumentByIdAsync(id);
                
                if (document == null)
                {
                    _logger.LogWarning("Document {DocumentId} not found", id);
                    return NotFound();
                }

                return Ok(document);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Query limit exceeded"))
            {
                _logger.LogWarning(ex, "Query limit exceeded while retrieving document {DocumentId}", id);
                return StatusCode(429, new { error = "Query limit exceeded. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document {DocumentId} from Hyland", id);
                return StatusCode(500, new { error = "An error occurred while retrieving the document" });
            }
        }

        /// <summary>
        /// Creates a new document in Hyland
        /// </summary>
        /// <param name="request">Document creation request</param>
        /// <returns>The created document</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Document), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Document>> CreateDocument([FromBody] CreateDocumentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogDebug("Creating document '{FileName}' in Hyland", request.FileName);
                var document = await _documentService.CreateDocumentAsync(request);
                
                _logger.LogInformation("Created document {DocumentId} in Hyland", document.ID);
                return CreatedAtAction(nameof(GetDocument), new { id = document.ID }, document);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Query limit exceeded"))
            {
                _logger.LogWarning(ex, "Query limit exceeded while creating document");
                return StatusCode(429, new { error = "Query limit exceeded. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating document in Hyland");
                return StatusCode(500, new { error = "An error occurred while creating the document" });
            }
        }

        /// <summary>
        /// Updates an existing document in Hyland
        /// </summary>
        /// <param name="id">The document ID</param>
        /// <param name="request">Document update request</param>
        /// <returns>The updated document</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Document), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Document>> UpdateDocument(string id, [FromBody] UpdateDocumentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogDebug("Updating document {DocumentId} in Hyland", id);
                var document = await _documentService.UpdateDocumentAsync(id, request);
                
                if (document == null)
                {
                    _logger.LogWarning("Document {DocumentId} not found for update", id);
                    return NotFound();
                }

                _logger.LogInformation("Updated document {DocumentId} in Hyland", id);
                return Ok(document);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Query limit exceeded"))
            {
                _logger.LogWarning(ex, "Query limit exceeded while updating document {DocumentId}", id);
                return StatusCode(429, new { error = "Query limit exceeded. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating document {DocumentId} in Hyland", id);
                return StatusCode(500, new { error = "An error occurred while updating the document" });
            }
        }

        /// <summary>
        /// Soft deletes a document in Hyland
        /// </summary>
        /// <param name="id">The document ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDocument(string id)
        {
            try
            {
                _logger.LogDebug("Deleting document {DocumentId} from Hyland", id);
                var success = await _documentService.DeleteDocumentAsync(id);
                
                if (!success)
                {
                    _logger.LogWarning("Document {DocumentId} not found for deletion", id);
                    return NotFound();
                }

                _logger.LogInformation("Deleted document {DocumentId} from Hyland", id);
                return NoContent();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Query limit exceeded"))
            {
                _logger.LogWarning(ex, "Query limit exceeded while deleting document {DocumentId}", id);
                return StatusCode(429, new { error = "Query limit exceeded. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document {DocumentId} from Hyland", id);
                return StatusCode(500, new { error = "An error occurred while deleting the document" });
            }
        }

        /// <summary>
        /// Searches documents in Hyland based on criteria
        /// </summary>
        /// <param name="searchRequest">Search criteria</param>
        /// <returns>A list of matching documents</returns>
        [HttpPost("search")]
        [ProducesResponseType(typeof(IEnumerable<Document>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Document>>> SearchDocuments([FromBody] DocumentSearchRequest searchRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogDebug("Searching documents in Hyland with criteria");
                var documents = await _documentService.SearchDocumentsAsync(searchRequest);
                
                return Ok(documents);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Query limit exceeded"))
            {
                _logger.LogWarning(ex, "Query limit exceeded while searching documents");
                return StatusCode(429, new { error = "Query limit exceeded. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching documents in Hyland");
                return StatusCode(500, new { error = "An error occurred while searching documents" });
            }
        }

        /// <summary>
        /// Archives a document in Hyland
        /// </summary>
        /// <param name="id">The document ID</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/archive")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ArchiveDocument(string id)
        {
            try
            {
                _logger.LogDebug("Archiving document {DocumentId} in Hyland", id);
                var success = await _documentService.ArchiveDocumentAsync(id);
                
                if (!success)
                {
                    _logger.LogWarning("Document {DocumentId} not found for archiving", id);
                    return NotFound();
                }

                _logger.LogInformation("Archived document {DocumentId} in Hyland", id);
                return Ok(new { message = "Document archived successfully" });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Query limit exceeded"))
            {
                _logger.LogWarning(ex, "Query limit exceeded while archiving document {DocumentId}", id);
                return StatusCode(429, new { error = "Query limit exceeded. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error archiving document {DocumentId} in Hyland", id);
                return StatusCode(500, new { error = "An error occurred while archiving the document" });
            }
        }

        /// <summary>
        /// Downloads the content of a document from Hyland
        /// </summary>
        /// <param name="id">The document ID</param>
        /// <returns>The document content as a file</returns>
        [HttpGet("{id}/content")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDocumentContent(string id)
        {
            try
            {
                _logger.LogDebug("Retrieving content for document {DocumentId} from Hyland", id);
                
                var document = await _documentService.GetDocumentByIdAsync(id);
                if (document == null)
                {
                    _logger.LogWarning("Document {DocumentId} not found", id);
                    return NotFound();
                }

                var content = await _documentService.GetDocumentContentAsync(id);
                if (content == null)
                {
                    _logger.LogWarning("Content not found for document {DocumentId}", id);
                    return NotFound();
                }

                _logger.LogDebug("Retrieved {ContentSize} bytes for document {DocumentId}", content.Length, id);
                return File(content, document.ContentType, document.FileName);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Query limit exceeded"))
            {
                _logger.LogWarning(ex, "Query limit exceeded while retrieving content for document {DocumentId}", id);
                return StatusCode(429, new { error = "Query limit exceeded. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving content for document {DocumentId} from Hyland", id);
                return StatusCode(500, new { error = "An error occurred while retrieving document content" });
            }
        }

        /// <summary>
        /// Gets the current query metering status
        /// </summary>
        /// <returns>Query metering status information</returns>
        [HttpGet("query-metering/status")]
        [ProducesResponseType(typeof(QueryMeteringStatus), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QueryMeteringStatus>> GetQueryMeteringStatus()
        {
            try
            {
                _logger.LogDebug("Retrieving query metering status");
                var status = await _documentService.GetQueryMeteringStatusAsync();
                
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving query metering status");
                return StatusCode(500, new { error = "An error occurred while retrieving query metering status" });
            }
        }

        /// <summary>
        /// Configures query metering limits
        /// </summary>
        /// <param name="request">Query metering configuration</param>
        /// <returns>Success status</returns>
        [HttpPost("query-metering/configure")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ConfigureQueryMetering([FromBody] ConfigureQueryMeteringRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (request.MaxQueriesPerHour <= 0)
                {
                    return BadRequest("MaxQueriesPerHour must be greater than 0");
                }

                if (request.WarningThreshold is < 0 or > 100)
                {
                    return BadRequest("WarningThreshold must be between 0 and 100");
                }

                _logger.LogInformation("Configuring query metering: {MaxQueries} queries per hour, {WarningThreshold}% warning threshold", 
                    request.MaxQueriesPerHour, request.WarningThreshold);
                
                _documentService.ConfigureQueryMetering(request.MaxQueriesPerHour, request.WarningThreshold);
                
                return Ok(new { message = "Query metering configured successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error configuring query metering");
                return StatusCode(500, new { error = "An error occurred while configuring query metering" });
            }
        }
    }

    public class ConfigureQueryMeteringRequest
    {
        public int MaxQueriesPerHour { get; set; }
        public int WarningThreshold { get; set; } = 80;
    }
}
        