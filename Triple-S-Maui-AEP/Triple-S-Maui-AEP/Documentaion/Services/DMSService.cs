using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TripleS.SOA.AEP.UI.Models;
using TripleS.SOA.AEP.UI.Configuration;

namespace TripleS.SOA.AEP.UI.Services
{
    /// <summary>
    /// Service for uploading documents to Triple-S Document Management System (DMS)
    /// Supports enrollment forms (834) and SOA documents
    /// </summary>
    public class DMSService
    {
        private readonly HttpClient _httpClient;
        
        // Document Type IDs
        public const int ENROLLMENT_FORM_DOCTYPE = 834;
        public const int SOA_FORM_DOCTYPE = 650; // Update if different
        
        // File Type IDs
        public const int PDF_FILETYPE = 16;
        public const int IMAGE_FILETYPE = 2;

        // Keyword Type IDs for Enrollment Form Mapping
        public class EnrollmentKeywords
        {
            public const int FirstName = 1049;      // TSA-Name
            public const int LastName = 1051;       // TSA-Last Names
            public const int MiddleName = 1050;     // TSA-Middle Name
            public const int DOB = 1107;            // TSA-DOB
            public const int Gender = 1111;         // TSA-Gender
            public const int SSN = 1120;            // TSA-SSN
            public const int Email = 1063;          // TSA-Email
            public const int PrimaryPhone = 1052;   // TSA-Main Phone
            public const int CellPhone = 1253;      // TSA-Cell Phone Number
            public const int MedicareNumber = 1137; // TSA-Medicare Part A
            public const int Address1 = 1053;       // TSA-Address 1
            public const int Address2 = 1054;       // TSA-Address 2
            public const int City = 1055;           // TSA-City
            public const int State = 1056;          // TSA-State
            public const int ZipCode = 1057;        // TSA-Zip Code
            public const int MailingAddress1 = 1113; // TSA-Mailing Address 1
            public const int MailingAddress2 = 1112; // TSA-Mailing Address 2
            public const int MailingCity = 1114;    // TSA-Mailing City
            public const int MailingState = 1115;   // TSA-Mailing State
            public const int MailingZip = 1116;     // TSA-Mailing Zip Code
            public const int EmergencyContactName = 1128;      // TSA-Emergency Contact Name
            public const int EmergencyContactPhone = 1129;     // TSA-Emergency Contact Phone
            public const int EmergencyContactRelationship = 1136; // TSA-Emergency Contact Relationship
            public const int Plan = 1110;           // TSA-Plan
            public const int PlanContract = 1121;   // TSA-Plan Contract
            public const int PlanPBP = 1122;        // TSA-Plan PBP
            public const int Language = 1179;       // TSA-Language
            public const int AgentNPN = 1280;       // TSA-NPI (closest match)
            public const int EnrollmentNumber = 1254; // TSA-Member ID
            public const int SignatureDate = 1082;  // TSA-Signature Date
            public const int PrimaryPhoneIsMobile = 1253; // TSA-Cell Phone Number (indicator)
            public const int MedicaidNumber = 1127; // TSA-Medicaid Number
            public const int ContactMethod = 1100;  // TSA-Contacted (or use Document Source 1098)
        }

        // Keyword Type IDs for SOA Form Mapping
        public class SOAKeywords
        {
            public const int FirstName = 1049;      // TSA-Name
            public const int LastName = 1051;       // TSA-Last Names
            public const int DOB = 1107;            // TSA-DOB
            public const int SOANumber = 1067;      // TSA-SOA ID
            public const int MedicareNumber = 1137; // TSA-Medicare Part A
            public const int AgentNPN = 1280;       // TSA-NPI
            public const int SignatureDate = 1082;  // TSA-Signature Date
            public const int Phone = 1052;          // TSA-Main Phone
        }

        public DMSService()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(AppSettings.DMSUploadTimeoutMinutes)
            };
            
            // Add Hyland authentication headers
            if (!string.IsNullOrWhiteSpace(AppSettings.HylandUsername) && 
                !string.IsNullOrWhiteSpace(AppSettings.HylandPassword))
            {
                _httpClient.DefaultRequestHeaders.Add("Hyland-Username", AppSettings.HylandUsername);
                _httpClient.DefaultRequestHeaders.Add("Hyland-Password", AppSettings.HylandPassword);
            }
        }

        /// <summary>
        /// Upload a document to Triple-S DMS with keywords
        /// </summary>
        public async Task<DMSUploadResponse> UploadDocumentAsync(
            string filePath, 
            int documentTypeId,
            List<(int KeywordTypeId, string Value)> keywords)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[DMSService] Starting upload for: {filePath}");
                System.Diagnostics.Debug.WriteLine($"[DMSService] Document Type ID: {documentTypeId}");

                // Validate file exists
                if (!File.Exists(filePath))
                {
                    return new DMSUploadResponse
                    {
                        Success = false,
                        Message = $"File not found: {filePath}",
                        ErrorCode = "FILE_NOT_FOUND"
                    };
                }

                // Read file and convert to Base64
                byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
                string base64Content = Convert.ToBase64String(fileBytes);
                
                System.Diagnostics.Debug.WriteLine($"[DMSService] File size: {fileBytes.Length} bytes");
                System.Diagnostics.Debug.WriteLine($"[DMSService] Base64 length: {base64Content.Length} characters");

                // Build keywords array
                var keywordsList = new List<object>();
                foreach (var (keywordTypeId, value) in keywords)
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        keywordsList.Add(new { KeywordTypeId = keywordTypeId, Value = value });
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[DMSService] Total keywords: {keywordsList.Count}");

                // Create request object
                var requestObject = new
                {
                    DocumentTypeId = documentTypeId,
                    FileTypeId = PDF_FILETYPE,
                    Base64Document = base64Content,
                    Keywords = keywordsList
                };

                // Serialize to JSON
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                string jsonContent = JsonSerializer.Serialize(requestObject, jsonOptions);

                System.Diagnostics.Debug.WriteLine($"[DMSService] Request JSON length: {jsonContent.Length} characters");
                System.Diagnostics.Debug.WriteLine($"[DMSService] Request JSON (first 500 chars): {jsonContent.Substring(0, Math.Min(500, jsonContent.Length))}");

                // Create HTTP request
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                System.Diagnostics.Debug.WriteLine($"[DMSService] Sending POST to: {AppSettings.DMSEndpoint}");

                // Send request
                var response = await _httpClient.PostAsync(AppSettings.DMSEndpoint, httpContent);

                System.Diagnostics.Debug.WriteLine($"[DMSService] Response Status Code: {response.StatusCode}");

                // Read response
                string responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[DMSService] Response Content: {responseContent}");

                // Parse response
                if (response.IsSuccessStatusCode)
                {
                    var dmsResponse = JsonSerializer.Deserialize<DMSUploadResponse>(responseContent, jsonOptions);
                    
                    if (dmsResponse != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[DMSService] Upload successful! Document ID: {dmsResponse.DocumentId}");
                        return dmsResponse;
                    }
                    
                    return new DMSUploadResponse
                    {
                        Success = true,
                        Message = "Upload successful",
                        Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[DMSService] Upload failed: {response.StatusCode} - {responseContent}");
                    
                    return new DMSUploadResponse
                    {
                        Success = false,
                        Message = $"Upload failed: {response.StatusCode}",
                        ErrorCode = response.StatusCode.ToString()
                    };
                }
            }
            catch (HttpRequestException httpEx)
            {
                System.Diagnostics.Debug.WriteLine($"[DMSService] HTTP Error: {httpEx.Message}");
                return new DMSUploadResponse
                {
                    Success = false,
                    Message = $"Network error: {httpEx.Message}",
                    ErrorCode = "NETWORK_ERROR"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DMSService] Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[DMSService] Stack Trace: {ex.StackTrace}");
                
                return new DMSUploadResponse
                {
                    Success = false,
                    Message = $"Upload error: {ex.Message}",
                    ErrorCode = "UPLOAD_ERROR"
                };
            }
        }

        /// <summary>
        /// Upload enrollment document with proper keyword mapping
        /// </summary>
        public async Task<DMSUploadResponse> UploadEnrollmentAsync(
            string filePath,
            string enrollmentNumber,
            string firstName,
            string lastName,
            string middleName = null,
            string dateOfBirth = null,
            string gender = null,
            string ssn = null,
            string medicare = null,
            string medicaid = null,
            string primaryPhone = null,
            string secondaryPhone = null,
            bool primaryPhoneIsMobile = false,
            string email = null,
            string address1 = null,
            string address2 = null,
            string city = null,
            string state = null,
            string zipCode = null,
            string mailingAddress1 = null,
            string mailingAddress2 = null,
            string mailingCity = null,
            string mailingState = null,
            string mailingZip = null,
            string emergencyContactName = null,
            string emergencyContactPhone = null,
            string emergencyContactRelationship = null,
            string planName = null,
            string contractNumber = null,
            string planPBP = null,
            string language = null,
            string contactMethod = null)
        {
            var keywords = new List<(int, string)>
            {
                // Primary Information
                (EnrollmentKeywords.EnrollmentNumber, enrollmentNumber),
                (EnrollmentKeywords.FirstName, firstName),
                (EnrollmentKeywords.LastName, lastName),
                (EnrollmentKeywords.MiddleName, middleName),
                
                // Demographics
                (EnrollmentKeywords.DOB, dateOfBirth),
                (EnrollmentKeywords.Gender, gender),
                (EnrollmentKeywords.SSN, ssn),
                
                // Insurance Numbers
                (EnrollmentKeywords.MedicareNumber, medicare),
                (EnrollmentKeywords.MedicaidNumber, medicaid),
                
                // Contact Information
                (EnrollmentKeywords.PrimaryPhone, primaryPhone),
                (EnrollmentKeywords.CellPhone, secondaryPhone),
                (EnrollmentKeywords.Email, email),
                
                // Permanent Address
                (EnrollmentKeywords.Address1, address1),
                (EnrollmentKeywords.Address2, address2),
                (EnrollmentKeywords.City, city),
                (EnrollmentKeywords.State, state),
                (EnrollmentKeywords.ZipCode, zipCode),
                
                // Mailing Address
                (EnrollmentKeywords.MailingAddress1, mailingAddress1),
                (EnrollmentKeywords.MailingAddress2, mailingAddress2),
                (EnrollmentKeywords.MailingCity, mailingCity),
                (EnrollmentKeywords.MailingState, mailingState),
                (EnrollmentKeywords.MailingZip, mailingZip),
                
                // Emergency Contact
                (EnrollmentKeywords.EmergencyContactName, emergencyContactName),
                (EnrollmentKeywords.EmergencyContactPhone, emergencyContactPhone),
                (EnrollmentKeywords.EmergencyContactRelationship, emergencyContactRelationship),
                
                // Plan Information
                (EnrollmentKeywords.Plan, planName),
                (EnrollmentKeywords.PlanContract, contractNumber),
                (EnrollmentKeywords.PlanPBP, planPBP),
                
                // Preferences
                (EnrollmentKeywords.Language, language),
                (EnrollmentKeywords.ContactMethod, contactMethod),
                
                // Metadata
                (EnrollmentKeywords.AgentNPN, TripleSPOC.Services.AgentSessionService.CurrentAgentNPN),
                (EnrollmentKeywords.SignatureDate, DateTime.Now.ToString("yyyy-MM-dd"))
            };

            return await UploadDocumentAsync(filePath, ENROLLMENT_FORM_DOCTYPE, keywords);
        }

        /// <summary>
        /// Upload SOA document with proper keyword mapping
        /// </summary>
        public async Task<DMSUploadResponse> UploadSOAAsync(
            string filePath,
            string soaNumber,
            string firstName,
            string lastName,
            string dateOfBirth = null,
            string medicareNumber = null,
            string phone = null)
        {
            var keywords = new List<(int, string)>
            {
                (SOAKeywords.SOANumber, soaNumber),
                (SOAKeywords.FirstName, firstName),
                (SOAKeywords.LastName, lastName),
                (SOAKeywords.DOB, dateOfBirth),
                (SOAKeywords.MedicareNumber, medicareNumber),
                (SOAKeywords.Phone, phone),
                (SOAKeywords.AgentNPN, TripleSPOC.Services.AgentSessionService.CurrentAgentNPN),
                (SOAKeywords.SignatureDate, DateTime.Now.ToString("yyyy-MM-dd"))
            };

            return await UploadDocumentAsync(filePath, SOA_FORM_DOCTYPE, keywords);
        }
    }
}
