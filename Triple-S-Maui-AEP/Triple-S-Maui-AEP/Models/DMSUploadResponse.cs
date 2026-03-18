using System.Text.Json.Serialization;

namespace Triple_S_Maui_AEP.Models
{
    /// <summary>
    /// Response model from Triple-S DMS upload endpoint
    /// </summary>
    public class DMSUploadResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("documentId")]
        public string? DocumentId { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("errorCode")]
        public string? ErrorCode { get; set; }

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; }
    }
}
