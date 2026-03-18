using System;
using System.Collections.Generic;

namespace TripleSPOC.Models
{
    // Enum for supported languages
    public enum Language
    {
        English,
        Spanish
    }

    // Base interface for language-aware models
    public interface ILanguageAware
    {
        Language Language { get; set; }
    }

    // Personal Information
    public class PersonalInfo : ILanguageAware
    {
        public Language Language { get; set; }
        public string? FirstName { get; set; } // Required, alpha, 50
        public string? MiddleInitial { get; set; } // Optional, 1 char
        public string? LastName { get; set; } // Required, alpha, 50
        public DateTime DateOfBirth { get; set; } // Required, 18+, not future
        public string? Gender { get; set; } // Enum: Male, Female, Non-Binary, Prefer Not to Answer
        public string? PrimaryPhone { get; set; } // Required, phone format
        public bool PrimaryPhoneIsMobile { get; set; } // Indicates if primary phone is mobile
        public string? SecondaryPhone { get; set; } // Optional
        public bool SecondaryPhoneIsMobile { get; set; } // Indicates if secondary phone is mobile
        public string? Email { get; set; } // Optional, email format
        public string? MedicareNumber { get; set; } // Required, 11-15 chars
        public string? MedicareCardImagePath { get; set; } // Optional, file path
        public string? SSN { get; set; } // Optional, 9 digits
        public string? PreferredContactMethod { get; set; } // Enum: Phone, Email, Mail, In-Person
    }

    // Address Information
    public class AddressInfo : ILanguageAware
    {
        public Language Language { get; set; }
        public string? Street1 { get; set; } // Required
        public string? Street2 { get; set; } // Optional
        public string? City { get; set; } // Required
        public string? State { get; set; } // Required, 2-letter
        public string? County { get; set; } // Optional
        public string? Zip { get; set; } // Required, 5-10
        public bool HasMailingAddress { get; set; }
        public AddressInfo? MailingAddress { get; set; } // Optional
    }

    // Emergency Contact
    public class EmergencyContact : ILanguageAware
    {
        public Language Language { get; set; }
        public string? Name { get; set; } // Required
        public string? Phone { get; set; } // Required
        public string? Relationship { get; set; } // Optional
    }

    // Dependent
    public class Dependent : ILanguageAware
    {
        public Language Language { get; set; }
        public string? Name { get; set; }
        public string? Relationship { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool EnrolledInPlan { get; set; }
    }

    // Plan Selection
    public class PlanSelection : ILanguageAware
    {
        public Language Language { get; set; }
        public string? PlanName { get; set; } // Required
        public string? PlanContractNumber { get; set; } // Required
        public string? PlanId { get; set; } // Required
        public string? PremiumPaymentMethod { get; set; } // Enum
        public string? BankAccountNumber { get; set; } // Conditional
        public string? RoutingNumber { get; set; } // Conditional
        public string? CreditCardNumber { get; set; } // Conditional
        public bool? LongTermCare { get; set; } // Optional
        public string? LTCFacilityName { get; set; } // Conditional
        public string? PCP { get; set; } // Optional
        public string? PCPClinic { get; set; } // Optional
    }

    // Current Coverage
    public class CurrentCoverage : ILanguageAware
    {
        public Language Language { get; set; }
        public bool HasOtherInsurance { get; set; }
        public string? OtherCoverageType { get; set; }
        public string? OtherCoveragePolicyNumber { get; set; }
        public bool CurrentlyEnrolledInMA { get; set; }
        public string? CurrentPlanName { get; set; }
        public string? CurrentPlanContractNumber { get; set; }
        public string? CurrentPlanId { get; set; }
        public DateTime? CoverageStartDate { get; set; }
        public string? ReasonForChange { get; set; }
    }

    // Special Circumstances
    public class SpecialCircumstances : ILanguageAware
    {
        public Language Language { get; set; }
        public bool? SNPEligible { get; set; }
        public string? SNPEligibilityType { get; set; }
        public string? ChronicConditionType { get; set; }
        public bool? MSAPlan { get; set; }
        public decimal? MSADepositAmount { get; set; }
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
