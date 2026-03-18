using System;

namespace Triple_S_Maui_AEP.Models
{
    public interface ILanguageAware
    {
        Language Language { get; set; }
    }

    // Beneficiary Demographics
    public class BeneficiaryDemographics : ILanguageAware
    {
        public Language Language { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? SSN { get; set; }
        public string? MedicareNumber { get; set; }
    }

    // Contact Information
    public class ContactInfo : ILanguageAware
    {
        public Language Language { get; set; }
        public string? Phone { get; set; }
        public bool PhoneIsMobile { get; set; }
        public string? Email { get; set; }
        public string? PreferredContactMethod { get; set; }
    }

    // Address Information
    public class AddressInfo : ILanguageAware
    {
        public Language Language { get; set; }
        public string? StreetAddress { get; set; }
        public string? Apartment { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? County { get; set; }
    }

    // Plan Selection
    public class PlanSelection : ILanguageAware
    {
        public Language Language { get; set; }
        public string? ContractNumber { get; set; }
        public string? PlanName { get; set; }
        public string? PlanType { get; set; }
        public DateTime EffectiveDate { get; set; }
    }

    // Medical History
    public class MedicalHistory : ILanguageAware
    {
        public Language Language { get; set; }
        public bool? ChronicConditions { get; set; }
        public bool? CurrentMedications { get; set; }
        public bool? HospitalizationHistory { get; set; }
        public bool? SurgeryHistory { get; set; }
        public string? MedicalHistoryNotes { get; set; }
    }

    // Health Status
    public class HealthStatus : ILanguageAware
    {
        public Language Language { get; set; }
        public bool? PFFSPlan { get; set; }
        public bool? DualEligible { get; set; }
        public bool? LIS { get; set; }
        public bool? ESRD { get; set; }
        public bool? InstitutionalCare { get; set; }
    }

    // SEP Information
    public class SEPInfo : ILanguageAware
    {
        public Language Language { get; set; }
        public string? SEPReason { get; set; }
        public DateTime? SEPEventDate { get; set; }
        public string? SEPEventDescription { get; set; }
        public string? SEPDocumentationPath { get; set; }
        public string? GoodCauseStatus { get; set; }
        public string? GoodCauseNotes { get; set; }
    }

    // Language and Accessibility
    public class LanguageAccessibility : ILanguageAware
    {
        public Language Language { get; set; }
        public bool SimplifiedLanguage { get; set; }
    }

    // Employer/Union Info
    public class EmployerUnionInfo : ILanguageAware
    {
        public Language Language { get; set; }
        public bool EmployerGroupPlan { get; set; }
        public string? EmployerName { get; set; }
        public string? UnionName { get; set; }
        public string? GroupNumber { get; set; }
    }

    // Electronic Signature
    public class ElectronicSignature : ILanguageAware
    {
        public Language Language { get; set; }
        public string? SignatureImageBase64 { get; set; }
        public DateTime SignatureTimestamp { get; set; }
        public string? PrintedName { get; set; }
        public string? SignerRole { get; set; }
        public string? AuthorizedRepName { get; set; }
        public string? AuthorizedRepPhone { get; set; }
        public string? AuthorizedRepEmail { get; set; }
        public string? AuthorizedRepRelationship { get; set; }
        public string? DeviceInfo { get; set; }
        public string? IPAddress { get; set; }
        public string? GPSCoordinates { get; set; }
    }

    // Agent/Broker Info
    public class AgentBrokerInfo : ILanguageAware
    {
        public Language Language { get; set; }
        public string? NPNNumber { get; set; }
        public string? LicenseNumber { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? AgencyName { get; set; }
        public bool AssistedWithForm { get; set; }
    }

    // Attestations
    public class Attestations : ILanguageAware
    {
        public Language Language { get; set; }
        public bool ConfirmsEnrollmentRequest { get; set; }
        public bool UnderstandsPlanBenefits { get; set; }
        public bool UnderstandsPlanNetwork { get; set; }
        public bool CertifiesInformationAccuracy { get; set; }
        public bool AuthorizesInformationRelease { get; set; }
        public bool AcknowledgesPrivacyNotice { get; set; }
        public bool AcceptsElectronicMaterials { get; set; }
        public bool UnderstandsMarketingGuidelines { get; set; }
        public DateTime AttestationTimestamp { get; set; }
    }

    // Form Metadata
    public class FormMetadata : ILanguageAware
    {
        public Language Language { get; set; }
        public string? FormType { get; set; }
        public string? FormStatus { get; set; }
        public string? FormIdentifier { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string? EnrollmentMechanism { get; set; }
        public DateTime SubmissionTimestamp { get; set; }
        public string? SubmissionLocationType { get; set; }
        public string? OMBControlNumber { get; set; }
        public DateTime OMBExpirationDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string? FormVariant { get; set; }
    }
}
