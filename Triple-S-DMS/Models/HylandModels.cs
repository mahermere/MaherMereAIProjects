using System.Text.Json.Serialization;

namespace TripleSService.Services
{
    // Configuration and Connection Models
    public class HylandConnectionConfiguration
    {
        public string AppServerUrl { get; set; } = "https://localhost/AppServer/Service.asmx";
        public string Username { get; set; } = "MANAGER";
        public string Password { get; set; } = string.Empty;
        public string DataSource { get; set; } = "OnBase";
        public bool UseQueryMetering { get; set; } = true;
        public bool UseDisconnectedMode { get; set; } = true;
        public int MaxQueriesPerHour { get; set; } = 2000;
        public int MinConnections { get; set; } = 2;
        public int MaxConnections { get; set; } = 10;
        public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromSeconds(30);
        public TimeSpan IdleConnectionTimeout { get; set; } = TimeSpan.FromMinutes(30);
    }

    public class HylandConnectionSettings
    {
        public string AppServerUrl { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DataSource { get; set; } = string.Empty;
        public bool UseDisconnectedMode { get; set; }
        public bool UseQueryMetering { get; set; }
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    }

    // Core Document Models
    public class Document
    {
        // Core Hyland properties from Unity API
        public long ID { get; set; } // Hyland uses long IDs
        public string Name { get; internal set; } = string.Empty; // AutoName String in Hyland (read-only for API consumers)
        public string CreatedBy { get; set; } = string.Empty; // OnBase User who created/imported
        public DateTime DateStored { get; set; } // Date/Time document was stored in OnBase
        public DateTime DocumentDate { get; set; } // Document Date from Hyland
        public string DefaultFileType { get; set; } = string.Empty; // FileType from DocumentType config
        public string DocumentType { get; set; } = string.Empty; // DocumentType name
        public DocumentStatus Status { get; set; } = DocumentStatus.Active;
        
        // Additional metadata
        public string[] Keywords { get; set; } = Array.Empty<string>();
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, object> CustomProperties { get; set; } = new();
        
        // File-related properties
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string MimeType { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        
        // API convenience properties
        public DateTime CreatedDate { get; set; } // Settable property for API compatibility
        public DateTime ModifiedDate { get; set; } // For tracking API-level modifications
        
        // Compatibility properties for string-based ID access
        [JsonIgnore]
        public string Id 
        { 
            get => ID.ToString(); 
            set => ID = long.TryParse(value, out var id) ? id : 0; 
        }
    }

    public enum DocumentStatus
    {
        Active,
        Archived,
        Deleted,
        InReview,
        Draft
    }

    public enum DocumentRetrievalOptions
    {
        Default = 0,
        LoadKeywords = 1,
        LoadNotes = 2,
        LoadRevisionsAndVersions = 4
    }

    // Request/Response Models
    public class CreateDocumentRequest
    {
        public string FileName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty; // Must match existing DocumentType in Hyland
        public string[]? Keywords { get; set; }
        public byte[]? Content { get; set; }
        public string MimeType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? DocumentDate { get; set; } // Optional override for document date
        public Dictionary<string, object>? CustomProperties { get; set; }
    }

    public class UpdateDocumentRequest
    {

        public string? Description { get; set; }
        public string[]? Keywords { get; set; }
        public Dictionary<string, object>? CustomProperties { get; set; }
        public DocumentStatus? Status { get; set; }
    }

    public class DocumentSearchRequest
    {
        // Search method selector (determines which search approach to use)
        public DocumentSearchMethod SearchMethod { get; set; } = DocumentSearchMethod.DocumentTypeAndDateRange;
        
        // Method 1: Documents By ID (or Range)
        public long? DocumentId { get; set; }
        public long? DocumentIdFrom { get; set; }
        public long? DocumentIdTo { get; set; }
        
        // Method 2: Documents by Document Type and Date Range
        public string? DocumentType { get; set; }
        public DateTime? DateStoredFrom { get; set; }
        public DateTime? DateStoredTo { get; set; }
        public DateTime? DocumentDateFrom { get; set; }
        public DateTime? DocumentDateTo { get; set; }
        
        // Method 3: Documents by Custom Query, Keywords, and Date Range
        public string? CustomQueryName { get; set; }
        public List<KeywordCriteria>? Keywords { get; set; }
        public string? KeywordRecordTypeName { get; set; }
        
        // Common filters

        public string? CreatedBy { get; set; }
        public DocumentStatus? Status { get; set; }
        public Dictionary<string, object>? CustomProperties { get; set; }
        
        // Query options and limits
        public int MaxResults { get; set; } = 100; // Important for disconnected mode
        public DocumentRetrievalOptions RetrievalOptions { get; set; } = DocumentRetrievalOptions.Default;
        
        // Validation properties
        [JsonIgnore]
        public bool IsValid => SearchMethod switch
        {
            DocumentSearchMethod.DocumentById => DocumentId.HasValue || (DocumentIdFrom.HasValue && DocumentIdTo.HasValue),
            DocumentSearchMethod.DocumentTypeAndDateRange => !string.IsNullOrEmpty(DocumentType) && 
                                                            (DateStoredFrom.HasValue || DocumentDateFrom.HasValue),
            DocumentSearchMethod.CustomQueryWithKeywords => !string.IsNullOrEmpty(CustomQueryName) && 
                                                           Keywords?.Any(k => !string.IsNullOrEmpty(k.Name)) == true,
            _ => false
        };
    }
    
    public enum DocumentSearchMethod
    {
        DocumentById,
        DocumentTypeAndDateRange,
        CustomQueryWithKeywords
    }
    
    public class KeywordSearchCriteria
    {
        public string KeywordTypeName { get; set; } = string.Empty;
        public KeywordDataType DataType { get; set; }
        public KeywordSearchOperator Operator { get; set; } = KeywordSearchOperator.Equal;
        
        // Typed value properties matching Unity API pattern
        public string? AlphaNumericValue { get; set; }
        public decimal? CurrencyValue { get; set; }
        public DateTime? DateTimeValue { get; set; }
        public double? FloatingPointValue { get; set; }
        public decimal? Numeric20Value { get; set; }
        public long? Numeric9Value { get; set; }
        
        // Convenience property for JSON serialization/deserialization
        [JsonIgnore]
        public object? TypedValue
        {
            get => DataType switch
            {
                KeywordDataType.AlphaNumeric => AlphaNumericValue,
                KeywordDataType.Currency or KeywordDataType.SpecificCurrency => CurrencyValue,
                KeywordDataType.Date or KeywordDataType.DateTime => DateTimeValue,
                KeywordDataType.FloatingPoint => FloatingPointValue,
                KeywordDataType.Numeric20 => Numeric20Value,
                KeywordDataType.Numeric9 => Numeric9Value,
                _ => null
            };
            set
            {
                switch (DataType)
                {
                    case KeywordDataType.AlphaNumeric:
                        AlphaNumericValue = value?.ToString();
                        break;
                    case KeywordDataType.Currency:
                    case KeywordDataType.SpecificCurrency:
                        CurrencyValue = value switch
                        {
                            decimal d => d,
                            string s when decimal.TryParse(s, out var d) => d,
                            _ => null
                        };
                        break;
                    case KeywordDataType.Date:
                    case KeywordDataType.DateTime:
                        DateTimeValue = value switch
                        {
                            DateTime dt => dt,
                            string s when DateTime.TryParse(s, out var dt) => dt,
                            _ => null
                        };
                        break;
                    case KeywordDataType.FloatingPoint:
                        FloatingPointValue = value switch
                        {
                            double d => d,
                            float f => f,
                            string s when double.TryParse(s, out var d) => d,
                            _ => null
                        };
                        break;
                    case KeywordDataType.Numeric20:
                        Numeric20Value = value switch
                        {
                            decimal d => d,
                            string s when decimal.TryParse(s, out var d) => d,
                            _ => null
                        };
                        break;
                    case KeywordDataType.Numeric9:
                        Numeric9Value = value switch
                        {
                            long l => l,
                            int i => i,
                            string s when long.TryParse(s, out var l) => l,
                            _ => null
                        };
                        break;
                }
            }
        }
        
        // Validation property
        [JsonIgnore]
        public bool IsValid => !string.IsNullOrEmpty(KeywordTypeName) && TypedValue != null;
    }
    
    // Simplified model for JSON API input
    public class KeywordCriteria
    {
        public string Name { get; set; } = string.Empty;
        public KeywordDataType DataType { get; set; }
        public object? Value { get; set; }
        public KeywordSearchOperator Operator { get; set; } = KeywordSearchOperator.Equal;
        
        // Convert to typed KeywordSearchCriteria
        public KeywordSearchCriteria ToKeywordSearchCriteria()
        {
            var criteria = new KeywordSearchCriteria
            {
                KeywordTypeName = Name,
                DataType = DataType,
                Operator = Operator
            };
            criteria.TypedValue = Value;
            return criteria;
        }
    }
    
    public enum KeywordDataType
    {
        AlphaNumeric,
        Currency,
        SpecificCurrency,
        Date,
        DateTime,
        FloatingPoint,
        Numeric20,
        Numeric9
    }
    
    public enum KeywordSearchOperator
    {
        Equal,
        NotEqual,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Contains,
        StartsWith,
        EndsWith
    }

    public class HylandConnectionException : Exception
    {
        public HylandConnectionException(string message) : base(message) { }
        public HylandConnectionException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class HylandOperationException : Exception
    {
        public HylandOperationException(string message) : base(message) { }
        public HylandOperationException(string message, Exception innerException) : base(message, innerException) { }
    }
}