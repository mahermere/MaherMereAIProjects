using System.Data;
using Hyland.Unity;
using Hyland.Types;

namespace TripleSService.Services
{
    public static class HylandConnectionHelper
    {
        public static Document ConvertHylandDocument(Hyland.Unity.Document hylandDoc, ILogger logger)
        {
            try
            {
                var document = new Document
                {
                    ID = hylandDoc.ID,
                    Name = hylandDoc.Name,
                    FileName = hylandDoc.Name, // OnBase uses Name for file identification
                    DocumentType = hylandDoc.DocumentType?.Name ?? "Unknown",
                    DateStored = hylandDoc.DateStored,
                    CreatedDate = hylandDoc.DateStored,
                    ModifiedDate = hylandDoc.DateStored, // OnBase doesn't track separate modification dates
                    CreatedBy = hylandDoc.CreatedBy?.RealName ?? "OnBase User",
                    FileSize = 0, // Would need to access renditions for actual file size
                    MimeType = "application/pdf", // Default, would need to check renditions for actual type
                    Status = DocumentStatus.Active,
                    CustomProperties = new Dictionary<string, object>()
                };

                // Extract keywords from KeywordRecords (Unity API pattern)
                var keywords = new List<string>();
                foreach (Hyland.Unity.KeywordRecord keyRecord in hylandDoc.KeywordRecords)
                {
                    foreach (Hyland.Unity.Keyword keyword in keyRecord.Keywords)
                    {
                        var keywordName = keyword.KeywordType.Name;
                        var keywordValue = GetKeywordValue(keyword);
                        
                        keywords.Add($"{keywordName}: {keywordValue}");
                        
                        // Add to custom properties for API consumers
                        if (!document.CustomProperties.ContainsKey(keywordName))
                        {
                            document.CustomProperties[keywordName] = keywordValue;
                        }
                    }
                }
                document.Keywords = keywords.ToArray();

                // Try to get document date from keywords (common pattern in OnBase)
                document.DocumentDate = GetDocumentDateFromKeywords(hylandDoc) ?? document.DateStored;

                return document;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error converting Hyland document {DocumentId}", hylandDoc?.ID);
                
                // Return basic document info if conversion fails
                return new Document
                {
                    ID = hylandDoc?.ID ?? 0,
                    Name = hylandDoc?.Name ?? "Unknown",
                    FileName = hylandDoc?.Name ?? "Unknown",
                    DocumentType = "Unknown",
                    DateStored = DateTime.UtcNow,
                    DocumentDate = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    Status = DocumentStatus.Active,
                    Keywords = Array.Empty<string>(),
                    CustomProperties = new Dictionary<string, object>()
                };
            }
        }
        
        private static object GetKeywordValue(Hyland.Unity.Keyword keyword)
        {
            // Extract keyword value based on data type (Unity API pattern)
            switch (keyword.KeywordType.DataType)
            {
                case Hyland.Unity.KeywordDataType.AlphaNumeric:
                    return keyword.AlphaNumericValue;
                case Hyland.Unity.KeywordDataType.Currency:
                case Hyland.Unity.KeywordDataType.SpecificCurrency:
                    return keyword.CurrencyValue;
                case Hyland.Unity.KeywordDataType.Date:
                case Hyland.Unity.KeywordDataType.DateTime:
                    return keyword.DateTimeValue;
                case Hyland.Unity.KeywordDataType.FloatingPoint:
                    return keyword.FloatingPointValue;
                case Hyland.Unity.KeywordDataType.Numeric20:
                    return keyword.Numeric20Value;
                case Hyland.Unity.KeywordDataType.Numeric9:
                    return keyword.Numeric9Value;
                default:
                    return keyword.ToString();
            }
        }
        
        private static DateTime? GetDocumentDateFromKeywords(Hyland.Unity.Document hylandDoc)
        {
            try
            {
                // Look for common document date keyword names
                var dateKeywordNames = new[] { "Document Date", "Date", "ProcessDate", "DocumentDate" };
                
                foreach (var keywordName in dateKeywordNames)
                {
                    foreach (Hyland.Unity.KeywordRecord keyRecord in hylandDoc.KeywordRecords)
                    {
                        foreach (Hyland.Unity.Keyword keyword in keyRecord.Keywords)
                        {
                            if (keyword.KeywordType.Name.Equals(keywordName, StringComparison.OrdinalIgnoreCase) &&
                                (keyword.KeywordType.DataType == Hyland.Unity.KeywordDataType.Date ||
                                 keyword.KeywordType.DataType == Hyland.Unity.KeywordDataType.DateTime))
                            {
                                return keyword.DateTimeValue;
                            }
                        }
                    }
                }
                
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}