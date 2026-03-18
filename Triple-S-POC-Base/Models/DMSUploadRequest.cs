using System.Text.Json.Serialization;

namespace TripleS.SOA.AEP.UI.Models
{
    /// <summary>
    /// Request model for uploading documents to Triple-S DMS
    /// </summary>
    public class DMSUploadRequest
    {
        [JsonPropertyName("documentType")]
        public string DocumentType { get; set; } = string.Empty;

        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = string.Empty;

        [JsonPropertyName("fileContent")]
        public string FileContent { get; set; } = string.Empty; // Base64 encoded

        [JsonPropertyName("metadata")]
        public DMSMetadata Metadata { get; set; } = new DMSMetadata();
    }

    /// <summary>
    /// Metadata for DMS upload
    /// </summary>
    public class DMSMetadata
    {
        [JsonPropertyName("enrollmentNumber")]
        public string? EnrollmentNumber { get; set; }

        [JsonPropertyName("soaNumber")]
        public string? SOANumber { get; set; }

        [JsonPropertyName("beneficiaryFirstName")]
        public string? BeneficiaryFirstName { get; set; }

        [JsonPropertyName("beneficiaryLastName")]
        public string? BeneficiaryLastName { get; set; }

        [JsonPropertyName("medicareNumber")]
        public string? MedicareNumber { get; set; }

        [JsonPropertyName("dateOfBirth")]
        public string? DateOfBirth { get; set; }

        [JsonPropertyName("agentNPN")]
        public string? AgentNPN { get; set; }

        [JsonPropertyName("uploadDate")]
        public string? UploadDate { get; set; }

        [JsonPropertyName("planName")]
        public string? PlanName { get; set; }

        [JsonPropertyName("contractNumber")]
        public string? ContractNumber { get; set; }
    }
}
