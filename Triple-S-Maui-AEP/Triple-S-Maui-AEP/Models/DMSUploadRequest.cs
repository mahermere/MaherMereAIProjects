using System.Text.Json.Serialization;

namespace Triple_S_Maui_AEP.Models
{
    /// <summary>
    /// Request model for uploading documents to Triple-S DMS (Hyland OnBase)
    /// </summary>
    public class DMSUploadRequest
    {
        [JsonPropertyName("DocumentTypeId")]
        public int DocumentTypeId { get; set; }

        [JsonPropertyName("FileTypeId")]
        public int FileTypeId { get; set; }

        [JsonPropertyName("Base64Document")]
        public string Base64Document { get; set; } = string.Empty;

        [JsonPropertyName("Keywords")]
        public List<DMSKeyword> Keywords { get; set; } = new List<DMSKeyword>();
    }

    /// <summary>
    /// Keyword metadata for DMS document
    /// </summary>
    public class DMSKeyword
    {
        [JsonPropertyName("KeywordTypeId")]
        public int KeywordTypeId { get; set; }

        [JsonPropertyName("Value")]
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// Known DocumentTypeIds
    /// </summary>
    public static class DMSDocumentTypes
    {
        public const int SOA = 841;
        public const int Enrollment = 842;
    }

    /// <summary>
    /// Known KeywordTypeIds for SOA (DocumentTypeId 841)
    /// Based on actual Hyland OnBase configuration from API
    /// </summary>
    public static class SOAKeywordTypes
    {
        // Core SOA Fields
        public const int SOA_ID = 1067;                    // TSA-SOA ID - REQUIRED
        public const int ProspectID = 1062;                // TSA-Prospect ID
        
        // Personal Information
        public const int FirstName = 1049;                 // TSA-Name
        public const int MiddleName = 1050;                // TSA-Middle Name
        public const int LastName = 1051;                  // TSA-Last Names
        public const int DOB = 1107;                       // TSA-DOB
        public const int HIC = 1092;                       // TSA-HIC (Medicare Number)
        
        // Contact Information
        public const int MainPhone = 1052;                 // TSA-Main Phone
        public const int Phone2 = 1059;                    // TSA-Phone 2
        public const int Phone3 = 1060;                    // TSA-Phone 3
        public const int Email = 1063;                     // TSA-Email
        
        // Address
        public const int Address1 = 1053;                  // TSA-Address 1
        public const int Address2 = 1054;                  // TSA-Address 2
        public const int City = 1055;                      // TSA-City
        public const int State = 1056;                     // TSA-State
        public const int ZipCode = 1057;                   // TSA-Zip Code
        public const int ZipCode4 = 1058;                  // TSA-Zip Code 4
        public const int Region = 1061;                    // TSA-Region
        
        // Dates & Signature
        public const int SignatureDate = 1082;             // TSA-Signature Date
        public const int PresentationDate = 1070;          // TSA-Presentation Date
        public const int Timestamp = 1333;                 // TSA-Timestamp - SOA ONLY
        public const int SignatureHour = 1334;             // TSA-Signature Hour - SOA ONLY
        public const int SignatureMethod = 1512;           // Signature Method
        public const int SignatureIndicator = 1109;        // SignatureIndicator
        public const int Year = 1093;                      // Year
        
        // Agent/User Information
        public const int Username = 1066;                  // Username
        public const int UploadedBy = 1108;                // UploadedBy
        public const int SPUsername = 1335;                // SP Username - SOA ONLY
        public const int SP_ID = 1080;                     // TSA-SP ID
        public const int SP_Name = 1076;                   // TSA-SP Name
        public const int SP_Region = 1077;                 // TSA-SP Region
        
        // SOA-Specific Fields
        public const int Attestation = 1638;               // SOA-Attestation - SOA ONLY
        public const int Disposition = 1075;               // TSA-Disposition
        public const int SOA_Status = 1071;                // TSA-SOA Status
        public const int Outcome = 1064;                   // TSA-Outcome
        public const int InitialContact = 1500;            // SOA-Initial Contact
        public const int AdditionalNotes = 1510;           // SOA Additional Notes
        public const int RecordingID = 1511;               // Recording ID
        
        // Campaign Fields
        public const int CampaignID = 1079;                // TSA-Campaign ID
        public const int CampaignName = 1078;              // TSA-Campaign Name
        public const int CampaignRegion = 1103;            // TSA-Campaign Region
        
        // Guardian (if applicable)
        public const int GuardianName = 1095;              // TSA-Guardian Name
        public const int GuardianLastNames = 1094;         // TSA-Guardian Last Names
        public const int GuardianPhone = 1097;             // TSA-Guardian Phone
        public const int GuardianRelationship = 1096;      // TSA-Guardian Relationship
    }

    /// <summary>
    /// Known KeywordTypeIds for Enrollment (DocumentTypeId 842)
    /// Based on actual Hyland OnBase configuration from API
    /// ONLY includes keywords that exist for Enrollment documents
    /// </summary>
    public static class EnrollmentKeywordTypes
    {
        // Core Enrollment Fields
        public const int EnrollmentID = 1067;              // TSA-SOA ID (reused) - REQUIRED
        public const int ProspectID = 1062;                // TSA-Prospect ID
        
        // Personal Information
        public const int FirstName = 1049;                 // TSA-Name
        public const int MiddleName = 1050;                // TSA-Middle Name
        public const int LastName = 1051;                  // TSA-Last Names
        public const int DOB = 1107;                       // TSA-DOB
        public const int HIC = 1092;                       // TSA-HIC (Medicare Number)
        public const int SSN = 1120;                       // TSA-SSN
        public const int Gender = 1111;                    // TSA-Gender
        public const int Prefix = 1118;                    // TSA-Prefix
        
        // Contact Information
        public const int MainPhone = 1052;                 // TSA-Main Phone
        public const int Phone2 = 1059;                    // TSA-Phone 2
        public const int Email = 1063;                     // TSA-Email
        public const int CellPhone = 1253;                 // TSA-Cell Phone Number
        
        // Address
        public const int Address1 = 1053;                  // TSA-Address 1
        public const int Address2 = 1054;                  // TSA-Address 2
        public const int City = 1055;                      // TSA-City
        public const int State = 1056;                     // TSA-State
        public const int ZipCode = 1057;                   // TSA-Zip Code
        public const int ZipCode4 = 1058;                  // TSA-Zip Code 4
        public const int Region = 1061;                    // TSA-Region
        
        // Mailing Address
        public const int MailingAddress1 = 1113;           // TSA-Mailing Address 1
        public const int MailingAddress2 = 1112;           // TSA-Mailing Address 2
        public const int MailingCity = 1114;               // TSA-Mailing City
        public const int MailingState = 1115;              // TSA-Mailing State
        public const int MailingZipCode = 1116;            // TSA-Mailing Zip Code
        public const int MailingZipCode4 = 1117;           // TSA-Mailing Zip Code 4
        
        // Dates & Signature
        public const int SignatureDate = 1082;             // TSA-Signature Date
        public const int CoverageEffectiveDate = 1123;     // TSA-Coverage Effective Date
        public const int PresentationDate = 1070;          // TSA-Presentation Date
        public const int SignatureMethod = 1512;           // Signature Method
        public const int SignatureIndicator = 1109;        // SignatureIndicator
        public const int Year = 1093;                      // Year
        
        // Agent/User Information (NO SPUsername, NO Timestamp for Enrollment!)
        public const int Username = 1066;                  // Username
        public const int UploadedBy = 1108;                // UploadedBy
        public const int UploadedDate = 1241;              // UploadedDate
        
        // Plan Information
        public const int Plan = 1110;                      // TSA-Plan
        public const int PlanContract = 1121;              // TSA-Plan Contract
        public const int PlanPBP = 1122;                   // TSA-Plan PBP
        public const int PlanIdentifier = 1163;            // TSA-Plan Identifier
        
        // Emergency Contact
        public const int EmergencyContactName = 1128;      // TSA-Emergency Contact Name
        public const int EmergencyContactPhone = 1129;     // TSA-Emergency Contact Phone
        public const int EmergencyContactRelationship = 1136;  // TSA-Emergency Contact Relationship
        
        // Guardian (if applicable)
        public const int Guardian = 1144;                  // TSA-Guardian
        public const int GuardianName = 1095;              // TSA-Guardian Name
        public const int GuardianLastNames = 1094;         // TSA-Guardian Last Names
        public const int GuardianPhone = 1097;             // TSA-Guardian Phone
        public const int GuardianRelationship = 1096;      // TSA-Guardian Relationship
        
        // Medical Information
        public const int ESRD = 1083;                      // TSA-ESRD
        public const int Medicaid = 1084;                  // TSA-Medicaid
        public const int MedicaidNumber = 1127;            // TSA-Medicaid Number
        public const int MedicarePartA = 1137;             // TSA-Medicare Part A
        public const int MedicarePartB = 1138;             // TSA-Medicare Part B
        
        // Language & Format
        public const int Language = 1021;                  // Language
        public const int Format = 1131;                    // TSA-Format
    }
}
