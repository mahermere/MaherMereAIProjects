using Microsoft.AspNetCore.Mvc;
using TripleSService.Services;

namespace TripleSService.Controllers
{
    [ApiController]
    [Route("api/documents/search")]
    public class DocumentSearchController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentSearchController> _logger;

        public DocumentSearchController(
            IDocumentService documentService,
            ILogger<DocumentSearchController> logger)
        {
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Search documents by ID or ID range using Hyland Unity API patterns
        /// </summary>
        /// <param name="id">Single document ID to retrieve</param>
        /// <param name="idFrom">Starting document ID for range search</param>
        /// <param name="idTo">Ending document ID for range search</param>
        /// <param name="maxResults">Maximum number of results (max 100)</param>
        /// <param name="retrievalOptions">Document retrieval options for performance optimization</param>
        /// <returns>List of documents matching the ID criteria</returns>
        [HttpGet("by-id")]
        [ProducesResponseType(typeof(IEnumerable<Document>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Document>>> SearchByDocumentId(
            [FromQuery] long? id = null,
            [FromQuery] long? idFrom = null,
            [FromQuery] long? idTo = null,
            [FromQuery] int maxResults = 100,
            [FromQuery] DocumentRetrievalOptions retrievalOptions = DocumentRetrievalOptions.Default)
        {
            try
            {
                // Validate input - require either single ID or valid range
                if (!id.HasValue && (!idFrom.HasValue || !idTo.HasValue))
                {
                    return BadRequest(new { error = "Either 'id' or both 'idFrom' and 'idTo' parameters are required" });
                }

                if (idFrom.HasValue && idTo.HasValue && idFrom > idTo)
                {
                    return BadRequest(new { error = "idFrom cannot be greater than idTo" });
                }

                var searchRequest = new DocumentSearchRequest
                {
                    SearchMethod = DocumentSearchMethod.DocumentById,
                    DocumentId = id,
                    DocumentIdFrom = idFrom,
                    DocumentIdTo = idTo,
                    MaxResults = Math.Min(maxResults, 100),
                    RetrievalOptions = retrievalOptions
                };

                _logger.LogDebug("Searching documents by ID: SingleId={Id}, Range={IdFrom}-{IdTo}", 
                    id, idFrom, idTo);

                var documents = await _documentService.SearchDocumentsAsync(searchRequest);
                
                return Ok(documents);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Query limit exceeded"))
            {
                _logger.LogWarning(ex, "Query limit exceeded while searching documents by ID");
                return StatusCode(429, new { error = "Query limit exceeded. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching documents by ID");
                return StatusCode(500, new { error = "An error occurred while searching documents" });
            }
        }

        /// <summary>
        /// Search documents by Document Type and Date Range using Hyland Unity API patterns
        /// </summary>
        /// <param name="documentType">Document type name (required)</param>
        /// <param name="dateStoredFrom">Start date for document storage date range</param>
        /// <param name="dateStoredTo">End date for document storage date range</param>
        /// <param name="documentDateFrom">Start date for document date range</param>
        /// <param name="documentDateTo">End date for document date range</param>
        /// <param name="maxResults">Maximum number of results (max 100)</param>
        /// <param name="retrievalOptions">Document retrieval options for performance optimization</param>
        /// <returns>List of documents matching the criteria</returns>
        [HttpGet("by-type-and-date")]
        [ProducesResponseType(typeof(IEnumerable<Document>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Document>>> SearchByDocumentTypeAndDateRange(
            [FromQuery] string documentType,
            [FromQuery] DateTime? dateStoredFrom = null,
            [FromQuery] DateTime? dateStoredTo = null,
            [FromQuery] DateTime? documentDateFrom = null,
            [FromQuery] DateTime? documentDateTo = null,
            [FromQuery] int maxResults = 100,
            [FromQuery] DocumentRetrievalOptions retrievalOptions = DocumentRetrievalOptions.Default)
        {
            try
            {
                // Validate required parameters
                if (string.IsNullOrWhiteSpace(documentType))
                {
                    return BadRequest(new { error = "documentType parameter is required" });
                }

                // Require at least one date range for performance (Hyland best practice)
                if (!dateStoredFrom.HasValue && !dateStoredTo.HasValue && 
                    !documentDateFrom.HasValue && !documentDateTo.HasValue)
                {
                    return BadRequest(new { error = "At least one date range parameter is required for performance optimization" });
                }

                var searchRequest = new DocumentSearchRequest
                {
                    SearchMethod = DocumentSearchMethod.DocumentTypeAndDateRange,
                    DocumentType = documentType,
                    DateStoredFrom = dateStoredFrom,
                    DateStoredTo = dateStoredTo,
                    DocumentDateFrom = documentDateFrom,
                    DocumentDateTo = documentDateTo,
                    MaxResults = Math.Min(maxResults, 100),
                    RetrievalOptions = retrievalOptions
                };

                _logger.LogDebug("Searching documents by type and date: DocumentType={DocumentType}, " +
                                "DateStored={DateStoredFrom}-{DateStoredTo}, DocumentDate={DocumentDateFrom}-{DocumentDateTo}",
                    documentType, dateStoredFrom, dateStoredTo, documentDateFrom, documentDateTo);

                var documents = await _documentService.SearchDocumentsAsync(searchRequest);
                
                return Ok(documents);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Query limit exceeded"))
            {
                _logger.LogWarning(ex, "Query limit exceeded while searching documents by type and date");
                return StatusCode(429, new { error = "Query limit exceeded. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching documents by type and date range");
                return StatusCode(500, new { error = "An error occurred while searching documents" });
            }
        }

        /// <summary>
        /// Search documents using Custom Queries, Keywords, and Date Range using Hyland Unity API patterns
        /// </summary>
        /// <param name="request">Advanced search request with custom query, keywords, and date filters</param>
        /// <returns>List of documents matching the custom query criteria</returns>
        [HttpPost("by-custom-query")]
        [ProducesResponseType(typeof(IEnumerable<Document>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Document>>> SearchByCustomQueryAndKeywords(
            [FromBody] DocumentSearchRequest request)
        {
            try
            {
                // Validate the request
                if (request == null)
                {
                    return BadRequest(new { error = "Search request is required" });
                }

                // Ensure search method is set correctly
                request.SearchMethod = DocumentSearchMethod.CustomQueryWithKeywords;

                // Validate custom query requirements
                if (string.IsNullOrWhiteSpace(request.CustomQueryName))
                {
                    return BadRequest(new { error = "customQueryName is required for custom query search" });
                }

                if (request.Keywords == null || !request.Keywords.Any())
                {
                    return BadRequest(new { error = "At least one keyword search criteria is required" });
                }

                // Validate keyword criteria
                foreach (var keyword in request.Keywords)
                {
                    if (string.IsNullOrWhiteSpace(keyword.Name))
                    {
                        return BadRequest(new { error = "Keyword Name is required for all keyword criteria" });
                    }
                    
                    if (keyword.Value == null)
                    {
                        return BadRequest(new { error = "Keyword Value is required for all keyword criteria" });
                    }
                    
                    // Validate that the value is compatible with the specified data type
                    var isValidValue = keyword.DataType switch
                    {
                        KeywordDataType.AlphaNumeric => keyword.Value is string,
                        KeywordDataType.Currency or KeywordDataType.SpecificCurrency => 
                            keyword.Value is decimal || (keyword.Value is string s1 && decimal.TryParse(s1, out _)),
                        KeywordDataType.Date or KeywordDataType.DateTime => 
                            keyword.Value is DateTime || (keyword.Value is string s2 && DateTime.TryParse(s2, out _)),
                        KeywordDataType.FloatingPoint => 
                            keyword.Value is double || keyword.Value is float || (keyword.Value is string s3 && double.TryParse(s3, out _)),
                        KeywordDataType.Numeric20 => 
                            keyword.Value is decimal || (keyword.Value is string s4 && decimal.TryParse(s4, out _)),
                        KeywordDataType.Numeric9 => 
                            keyword.Value is long || keyword.Value is int || (keyword.Value is string s5 && long.TryParse(s5, out _)),
                        _ => false
                    };
                    
                    if (!isValidValue)
                    {
                        return BadRequest(new { error = $"Invalid value type for keyword '{keyword.Name}'. Expected {keyword.DataType} compatible value." });
                    }
                }

                // Require date range for performance (Hyland best practice)
                if (!request.DateStoredFrom.HasValue && !request.DateStoredTo.HasValue &&
                    !request.DocumentDateFrom.HasValue && !request.DocumentDateTo.HasValue)
                {
                    return BadRequest(new { error = "At least one date range is required for custom query searches" });
                }

                // Cap results for performance
                request.MaxResults = Math.Min(request.MaxResults, 100);

                if (!request.IsValid)
                {
                    return BadRequest(new { error = "Invalid search request. Please check all required parameters." });
                }

                _logger.LogDebug("Searching documents by custom query: CustomQuery={CustomQuery}, KeywordCount={KeywordCount}",
                    request.CustomQueryName, request.Keywords.Count);

                var documents = await _documentService.SearchDocumentsAsync(request);
                
                return Ok(documents);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Query limit exceeded"))
            {
                _logger.LogWarning(ex, "Query limit exceeded while searching documents by custom query");
                return StatusCode(429, new { error = "Query limit exceeded. Please try again later." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid search parameters for custom query");
                return BadRequest(new { error = $"Invalid search parameters: {ex.Message}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching documents by custom query and keywords");
                return StatusCode(500, new { error = "An error occurred while searching documents" });
            }
        }

        /// <summary>
        /// Get available custom queries for the current user
        /// </summary>
        /// <returns>List of available custom queries</returns>
        [HttpGet("custom-queries")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<string>>> GetAvailableCustomQueries()
        {
            try
            {
                _logger.LogDebug("Retrieving available custom queries");
                
                // This would be implemented to return actual custom queries from Hyland
                // For now, return a placeholder response
                var customQueries = new List<string>
                {
                    "Invoice Search",
                    "Contract Documents", 
                    "Customer Records",
                    "Financial Reports"
                };
                
                return Ok(customQueries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving custom queries");
                return StatusCode(500, new { error = "An error occurred while retrieving custom queries" });
            }
        }

        /// <summary>
        /// Get document status enumeration values and ordinals
        /// </summary>
        /// <returns>Document status enum names and ordinal positions</returns>
        [HttpGet("enums/document-status")]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<object>> GetDocumentStatusEnum()
        {
            var statuses = Enum.GetValues<DocumentStatus>()
                .Select(status => new
                {
                    Name = status.ToString(),
                    Ordinal = (int)status
                })
                .OrderBy(x => x.Ordinal);

            return Ok(statuses);
        }

        /// <summary>
        /// Get search method enumeration values and ordinals
        /// </summary>
        /// <returns>Search method enum names and ordinal positions</returns>
        [HttpGet("enums/search-methods")]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<object>> GetSearchMethodsEnum()
        {
            var methods = Enum.GetValues<DocumentSearchMethod>()
                .Select(method => new
                {
                    Name = method.ToString(),
                    Ordinal = (int)method
                })
                .OrderBy(x => x.Ordinal);

            return Ok(methods);
        }

        /// <summary>
        /// Get retrieval options enumeration values and ordinals
        /// </summary>
        /// <returns>Retrieval options enum names and ordinal positions</returns>
        [HttpGet("enums/retrieval-options")]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<object>> GetRetrievalOptionsEnum()
        {
            var options = Enum.GetValues<DocumentRetrievalOptions>()
                .Select(option => new
                {
                    Name = option.ToString(),
                    Ordinal = (int)option
                })
                .OrderBy(x => x.Ordinal);

            return Ok(options);
        }

        /// <summary>
        /// Get keyword data type enumeration values and ordinals
        /// </summary>
        /// <returns>Keyword data type enum names and ordinal positions</returns>
        [HttpGet("enums/keyword-data-types")]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<object>> GetKeywordDataTypesEnum()
        {
            var dataTypes = Enum.GetValues<KeywordDataType>()
                .Select(dataType => new
                {
                    Name = dataType.ToString(),
                    Ordinal = (int)dataType
                })
                .OrderBy(x => x.Ordinal);

            return Ok(dataTypes);
        }

        /// <summary>
        /// Get search operator enumeration values and ordinals
        /// </summary>
        /// <returns>Search operator enum names and ordinal positions</returns>
        [HttpGet("enums/search-operators")]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<object>> GetSearchOperatorsEnum()
        {
            var operators = Enum.GetValues<KeywordSearchOperator>()
                .Select(op => new
                {
                    Name = op.ToString(),
                    Ordinal = (int)op
                })
                .OrderBy(x => x.Ordinal);

            return Ok(operators);
        }
    }
}