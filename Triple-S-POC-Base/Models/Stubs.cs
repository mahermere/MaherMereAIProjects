namespace TripleS.SOA.AEP.Models
{
    // Stub for missing Models namespace
    // Add actual model classes as needed
}

namespace TripleS.SOA.AEP.Models.Domain
{
    // Stub for missing Domain models
    public class Enrollment {
        public int EnrollmentId { get; set; }
        public string? BeneficiaryId { get; set; }
        public string? PlanId { get; set; }
        public string? Status { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
    public class SignatureOfAuthority {
        public int SOAId { get; set; }
        public string? BeneficiaryName { get; set; }
        public DateTime MeetingDate { get; set; }
        public string? MeetingLocation { get; set; }
        public bool MedicareAdvantageSelected { get; set; }
        public bool ProductInformationProvided { get; set; }
        public bool ComplianceDocumentsProvided { get; set; }
        public byte[]? BeneficiarySignatureData { get; set; }
        public byte[]? AgentSignatureData { get; set; }
        public string? Status { get; set; }
        public DateTime CompletedDate { get; set; }
    }
}

namespace TripleS.SOA.AEP.Models.ViewModels
{
    // Stub for missing ViewModels
}
