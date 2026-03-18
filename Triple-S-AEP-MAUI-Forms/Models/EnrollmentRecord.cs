using LiteDB;
using System;

namespace Triple_S_AEP_MAUI_Forms.Models;

public class EnrollmentRecord
{
    [BsonId]
    public ObjectId Id { get; set; } = default!;

    // Beneficiary Information
    public string BeneficiaryFirstName { get; set; } = string.Empty;
    public string BeneficiaryLastName { get; set; } = string.Empty;
    public string BeneficiaryMiddleInitial { get; set; } = string.Empty;
    public DateTime? BeneficiaryDOB { get; set; }
    public string BeneficiaryPhone { get; set; } = string.Empty;
    public string BeneficiaryAltPhone { get; set; } = string.Empty;
    public string BeneficiaryEmail { get; set; } = string.Empty;
    
    // Beneficiary Address
    public string BeneficiaryAddress1 { get; set; } = string.Empty;
    public string BeneficiaryAddress2 { get; set; } = string.Empty;
    public string BeneficiaryCity { get; set; } = string.Empty;
    public string BeneficiaryState { get; set; } = string.Empty;
    public string BeneficiaryZip { get; set; } = string.Empty;

    // Authorized Representative Information
    public string AuthorizedRepFirstName { get; set; } = string.Empty;
    public string AuthorizedRepLastName { get; set; } = string.Empty;
    public string AuthorizedRepMiddleInitial { get; set; } = string.Empty;
    public string AuthorizedRepRelationship { get; set; } = string.Empty;

    // SOA Information
    public string SOANumber { get; set; } = string.Empty;
    public string CampaignNumber { get; set; } = string.Empty;
    public string CampaignName { get; set; } = string.Empty;
    public string ProductType { get; set; } = string.Empty;
    public string InitialContactMethod { get; set; } = string.Empty;
    public bool BeneficiaryWalkedIn { get; set; }
    public bool NewToMedicare { get; set; }
    public string CurrentPlan { get; set; } = string.Empty;
    public ObjectId? LinkedSoaRecordId { get; set; }

    // File Paths
    public string? EnrollmentFormPdfPath { get; set; }
    public string? SoaFormPdfPath { get; set; }
    public string? WorkingAgeSurveyPdfPath { get; set; }

    // DMS Document IDs
    public string? EnrollmentFormDmsDocumentId { get; set; }
    public string? SoaFormDmsDocumentId { get; set; }
    public string? WorkingAgeSurveyDmsDocumentId { get; set; }

    // Upload Status
    public EnrollmentUploadStatus EnrollmentUploadStatus { get; set; } = EnrollmentUploadStatus.Pending;
    public EnrollmentUploadStatus SoaUploadStatus { get; set; } = EnrollmentUploadStatus.Pending;
    public EnrollmentUploadStatus WorkingAgeSurveyUploadStatus { get; set; } = EnrollmentUploadStatus.Pending;

    // Metadata
    public DateTime CreatedDate { get; set; }
    public DateTime? SubmittedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }

    // Status
    public bool IsComplete { get; set; }

    public string DisplayName => $"{BeneficiaryFirstName} {BeneficiaryLastName}";
}

public enum EnrollmentUploadStatus
{
    Pending,
    Uploaded,
    Failed,
    Archived
}
