using System.Net.Http.Json;
using System.Net.Http.Headers;
using Triple_S_Maui_AEP.Configuration;
using Triple_S_Maui_AEP.Models;

namespace Triple_S_Maui_AEP.Services
{
    /// <summary>
    /// Service for managing DMS (Document Management System) uploads to Hyland OnBase
    /// Uses credentials from AgentSessionService when available
    /// </summary>
    public class DMSService
    {
        private readonly HttpClient _httpClient;
        
        // CORRECTED: Use base domain only, full path in endpoint
        // The server is rewriting URLs and stripping the virtual directory
        private const string DMS_BASE_URL = "https://soaqa.ssspr.com";
        private const string DMS_UPLOAD_ENDPOINT = "/Triple-S-AEP-API/api/document/upload";
        
        // Fallback credentials - use AgentSessionService credentials when available
        private const string FALLBACK_HYLAND_USERNAME = "";
        private const string FALLBACK_HYLAND_PASSWORD = "";
        
        /// <summary>
        /// Set to true to use stub/mock responses for testing UI without actual API
        /// </summary>
        public static bool UseStubMode { get; set; } = false; // Use actual DMS API

        public DMSService()
        {
            // Create HttpClientHandler that allows self-signed certificates for development
            var handler = new HttpClientHandler();
            
            // Allow self-signed certificates for localhost development
            #if DEBUG
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                // For development: allow self-signed certificates on localhost
                if (message.RequestUri?.Host == "localhost" || message.RequestUri?.Host == "127.0.0.1")
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Accepting self-signed certificate for localhost development");
                    return true;
                }
                
                // For production: validate certificates properly
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
            #endif
            
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(DMS_BASE_URL)
            };
            
            // Get credentials: use session credentials if available, otherwise fall back to hardcoded
            var (username, password) = GetDMSCredentials();
            
            // Add authentication headers
            _httpClient.DefaultRequestHeaders.Add("Hyland-Username", username);
            _httpClient.DefaultRequestHeaders.Add("Hyland-Password", password);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            System.Diagnostics.Debug.WriteLine($"DMSService initialized:");
            System.Diagnostics.Debug.WriteLine($"  Base URL: {DMS_BASE_URL}");
            System.Diagnostics.Debug.WriteLine($"  Username: {username}");
            System.Diagnostics.Debug.WriteLine($"  Using session credentials: {AgentSessionService.IsAgentLoggedIn}");
            System.Diagnostics.Debug.WriteLine($"  Stub Mode: {UseStubMode}");
        }

        /// <summary>
        /// Get DMS credentials from session or use fallback hardcoded credentials
        /// </summary>
        private static (string Username, string Password) GetDMSCredentials()
        {
            if (AgentSessionService.IsAgentLoggedIn)
            {
                var (npn, password) = AgentSessionService.GetDMSCredentials();
                
                if (!string.IsNullOrEmpty(password))
                {
                    System.Diagnostics.Debug.WriteLine("✓ Using credentials from AgentSessionService");
                    return (npn, password);
                }
            }

            System.Diagnostics.Debug.WriteLine("⚠️ Using fallback hardcoded credentials (no session available)");
            return (FALLBACK_HYLAND_USERNAME, FALLBACK_HYLAND_PASSWORD);
        }

        public async Task<DMSUploadResponse> UploadDocumentAsync(DMSUploadRequest request)
        {
            // STUB MODE: Return successful response for testing
            if (UseStubMode)
            {
                System.Diagnostics.Debug.WriteLine($"[STUB MODE] Simulating upload for DocumentTypeId: {request.DocumentTypeId}");
                System.Diagnostics.Debug.WriteLine($"[STUB MODE] Keywords count: {request.Keywords.Count}");
                
                // Simulate network delay
                await Task.Delay(1000);
                
                return new DMSUploadResponse
                {
                    Success = true,
                    DocumentId = $"DOC_{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                    Message = "Document uploaded successfully (stubbed)",
                    Timestamp = DateTime.UtcNow.ToString("o")
                };
            }

            // REAL MODE: Make actual API call to Hyland OnBase
            try
            {
                System.Diagnostics.Debug.WriteLine("=== DMS UPLOAD START ===");
                System.Diagnostics.Debug.WriteLine($"Endpoint: POST {DMS_UPLOAD_ENDPOINT}");
                System.Diagnostics.Debug.WriteLine($"Base URL: {DMS_BASE_URL}");
                System.Diagnostics.Debug.WriteLine($"DocumentTypeId: {request.DocumentTypeId}");
                System.Diagnostics.Debug.WriteLine($"FileTypeId: {request.FileTypeId}");
                System.Diagnostics.Debug.WriteLine($"Base64Document length: {request.Base64Document?.Length ?? 0} bytes");
                System.Diagnostics.Debug.WriteLine($"Keywords count: {request.Keywords.Count}");
                
                // Log keywords
                foreach (var kw in request.Keywords)
                {
                    System.Diagnostics.Debug.WriteLine($"  - KeywordTypeId: {kw.KeywordTypeId}, Value: {kw.Value}");
                }
                
                var response = await _httpClient.PostAsJsonAsync(DMS_UPLOAD_ENDPOINT, request);
                
                var responseBody = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"DMS Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"DMS Response Body: {responseBody}");
                
                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"✓ Upload successful! DocumentId: {responseBody}");
                    System.Diagnostics.Debug.WriteLine("=== DMS UPLOAD SUCCESS ===");
                    
                    return new DMSUploadResponse 
                    { 
                        Success = true,
                        DocumentId = responseBody,
                        Message = "Document uploaded successfully",
                        Timestamp = DateTime.UtcNow.ToString("o")
                    };
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"✗ Upload failed with status: {response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine("=== DMS UPLOAD FAILED ===");
                    
                    return new DMSUploadResponse 
                    { 
                        Success = false, 
                        Message = $"DMS upload failed: {response.StatusCode} - {responseBody}",
                        ErrorCode = response.StatusCode.ToString()
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"✗ HttpRequestException: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"  InnerException: {ex.InnerException?.Message}");
                System.Diagnostics.Debug.WriteLine("=== DMS UPLOAD EXCEPTION ===");
                
                return new DMSUploadResponse
                {
                    Success = false,
                    Message = $"DMS connection error: {ex.Message}",
                    ErrorCode = "HTTP_ERROR"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"✗ Unexpected exception: {ex.GetType().Name}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"  Stack: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine("=== DMS UPLOAD EXCEPTION ===");
                
                return new DMSUploadResponse
                {
                    Success = false,
                    Message = $"DMS upload error: {ex.Message}",
                    ErrorCode = "UPLOAD_ERROR"
                };
            }
        }

        /// <summary>
        /// Helper method to create SOA upload request with proper keywords
        /// </summary>
        public static DMSUploadRequest CreateSOAUploadRequest(
            string soaNumber,
            string base64Document,
            string firstName,
            string lastName,
            string? middleName = null,
            string? phone = null,
            string? medicareNumber = null,
            DateTime? dateOfBirth = null,
            DateTime? signatureDate = null,
            string? agentUsername = null,
            string? signatureMethod = null,
            string? attestation = null)
        {
            // Validate SOA ID is within 20-character limit for Hyland DMS
            if (soaNumber.Length > 20)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ WARNING: SOA ID exceeds 20-character limit: '{soaNumber}' ({soaNumber.Length} chars)");
                System.Diagnostics.Debug.WriteLine($"  Truncating to: '{soaNumber.Substring(0, 20)}'");
            }
            
            var soaIdForDMS = soaNumber.Length > 20 ? soaNumber.Substring(0, 20) : soaNumber;
            
            var request = new DMSUploadRequest
            {
                DocumentTypeId = DMSDocumentTypes.SOA,
                FileTypeId = 16, // PDF file type
                Base64Document = base64Document,
                Keywords = new List<DMSKeyword>()
            };

            // Add SOA ID (required) - MUST be ≤ 20 characters for Hyland DMS
            request.Keywords.Add(new DMSKeyword 
            { 
                KeywordTypeId = SOAKeywordTypes.SOA_ID,  // TSA-SOA ID (max 20 chars)
                Value = soaIdForDMS
            });

            // Add beneficiary information
            if (!string.IsNullOrWhiteSpace(firstName))
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = SOAKeywordTypes.FirstName, Value = firstName });

            if (!string.IsNullOrWhiteSpace(lastName))
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = SOAKeywordTypes.LastName, Value = lastName });

            if (!string.IsNullOrWhiteSpace(middleName))
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = SOAKeywordTypes.MiddleName, Value = middleName });

            if (!string.IsNullOrWhiteSpace(phone))
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = SOAKeywordTypes.MainPhone, Value = phone });

            if (!string.IsNullOrWhiteSpace(medicareNumber))
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = SOAKeywordTypes.HIC, Value = medicareNumber });

            // Add dates
            if (dateOfBirth.HasValue)
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = SOAKeywordTypes.DOB, Value = dateOfBirth.Value.ToString("yyyy-MM-dd") });

            if (signatureDate.HasValue)
            {
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = SOAKeywordTypes.SignatureDate, Value = signatureDate.Value.ToString("yyyy-MM-dd") });
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = SOAKeywordTypes.Year, Value = signatureDate.Value.Year.ToString() });
            }

            // Add agent/user information
            if (!string.IsNullOrWhiteSpace(agentUsername))
            {
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = SOAKeywordTypes.UploadedBy, Value = agentUsername });
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = SOAKeywordTypes.Username, Value = agentUsername });
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = SOAKeywordTypes.SPUsername, Value = agentUsername }); // SOA-specific
            }

            // Add signature metadata
            if (!string.IsNullOrWhiteSpace(signatureMethod))
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = SOAKeywordTypes.SignatureMethod, Value = signatureMethod });

            if (!string.IsNullOrWhiteSpace(attestation))
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = SOAKeywordTypes.Attestation, Value = attestation }); // SOA-specific

            // Add SOA-specific timestamp fields
            var now = DateTime.Now;
            request.Keywords.Add(new DMSKeyword 
            { 
                KeywordTypeId = SOAKeywordTypes.Timestamp, 
                Value = now.ToString("yyyy-MM-ddTHH:mm:ss") 
            });

            request.Keywords.Add(new DMSKeyword 
            { 
                KeywordTypeId = SOAKeywordTypes.SignatureHour, 
                Value = now.ToString("HH:mm") 
            });

            return request;
        }

        /// <summary>
        /// Helper method to create Enrollment upload request with proper keywords
        /// Uses ONLY keywords configured for Enrollment DocumentTypeId 842 in Hyland OnBase
        /// </summary>
        public static DMSUploadRequest CreateEnrollmentUploadRequest(
            string enrollmentNumber,
            string base64Document,
            string firstName,
            string lastName,
            string? middleName = null,
            string? phone = null,
            string? medicareNumber = null,
            DateTime? dateOfBirth = null,
            DateTime? signatureDate = null,
            string? agentUsername = null,
            string? signatureMethod = null)
        {
            var request = new DMSUploadRequest
            {
                DocumentTypeId = DMSDocumentTypes.Enrollment,
                FileTypeId = 16, // PDF file type
                Base64Document = base64Document,
                Keywords = new List<DMSKeyword>()
            };

            // Add Enrollment ID (required)
            request.Keywords.Add(new DMSKeyword 
            { 
                KeywordTypeId = EnrollmentKeywordTypes.EnrollmentID, 
                Value = enrollmentNumber 
            });

            // Add beneficiary information
            if (!string.IsNullOrWhiteSpace(firstName))
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = EnrollmentKeywordTypes.FirstName, Value = firstName });

            if (!string.IsNullOrWhiteSpace(lastName))
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = EnrollmentKeywordTypes.LastName, Value = lastName });

            if (!string.IsNullOrWhiteSpace(middleName))
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = EnrollmentKeywordTypes.MiddleName, Value = middleName });

            if (!string.IsNullOrWhiteSpace(phone))
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = EnrollmentKeywordTypes.MainPhone, Value = phone });

            if (!string.IsNullOrWhiteSpace(medicareNumber))
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = EnrollmentKeywordTypes.HIC, Value = medicareNumber });

            // Add dates
            if (dateOfBirth.HasValue)
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = EnrollmentKeywordTypes.DOB, Value = dateOfBirth.Value.ToString("yyyy-MM-dd") });

            if (signatureDate.HasValue)
            {
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = EnrollmentKeywordTypes.SignatureDate, Value = signatureDate.Value.ToString("yyyy-MM-dd") });
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = EnrollmentKeywordTypes.Year, Value = signatureDate.Value.Year.ToString() });
            }

            // Add agent/user information
            // NOTE: Enrollment does NOT support these SOA-only keywords:
            // - Timestamp (1333) - SOA ONLY
            // - SignatureHour (1334) - SOA ONLY  
            // - SPUsername (1335) - SOA ONLY
            if (!string.IsNullOrWhiteSpace(agentUsername))
            {
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = EnrollmentKeywordTypes.UploadedBy, Value = agentUsername });
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = EnrollmentKeywordTypes.Username, Value = agentUsername });
            }

            // Add signature metadata
            if (!string.IsNullOrWhiteSpace(signatureMethod))
                request.Keywords.Add(new DMSKeyword { KeywordTypeId = EnrollmentKeywordTypes.SignatureMethod, Value = signatureMethod });

            // NOTE: UploadedDate (1241) exists but uses DateTime format - adding current timestamp
            request.Keywords.Add(new DMSKeyword 
            { 
                KeywordTypeId = EnrollmentKeywordTypes.UploadedDate, 
                Value = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") 
            });

            return request;
        }

        /// <summary>
        /// Diagnostic method to test connectivity to DMS endpoint
        /// </summary>
        public async Task<(bool Connected, string Message, string Details)> TestConnectivityAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("\n╔════════════════════════════════════════╗");
                System.Diagnostics.Debug.WriteLine("║  DMS CONNECTIVITY DIAGNOSTIC TEST      ║");
                System.Diagnostics.Debug.WriteLine("╚════════════════════════════════════════╝\n");
                
                var fullUrl = $"{DMS_BASE_URL}{DMS_UPLOAD_ENDPOINT}";
                System.Diagnostics.Debug.WriteLine($"📍 Testing URL: {fullUrl}");
                System.Diagnostics.Debug.WriteLine($"📋 Base URL: {DMS_BASE_URL}");
                System.Diagnostics.Debug.WriteLine($"📋 Endpoint: {DMS_UPLOAD_ENDPOINT}");
                System.Diagnostics.Debug.WriteLine($"📋 Method: POST");
                
                // Log headers being sent
                System.Diagnostics.Debug.WriteLine($"\n📬 Headers:");
                foreach (var header in _httpClient.DefaultRequestHeaders)
                {
                    System.Diagnostics.Debug.WriteLine($"   {header.Key}: {string.Join(", ", header.Value)}");
                }
                
                // Create a minimal test request
                var testRequest = new DMSUploadRequest
                {
                    DocumentTypeId = 123,
                    FileTypeId = 16,
                    Base64Document = "JVBERi0xLjQ=", // Minimal valid PDF header base64
                    Keywords = new List<DMSKeyword> 
                    { 
                        new DMSKeyword { KeywordTypeId = 1, Value = "TEST_CONNECTIVITY" }
                    }
                };
                
                System.Diagnostics.Debug.WriteLine($"\n🧪 Sending minimal test request...");
                var response = await _httpClient.PostAsJsonAsync(DMS_UPLOAD_ENDPOINT, testRequest);
                
                System.Diagnostics.Debug.WriteLine($"\n✓ Response received!");
                System.Diagnostics.Debug.WriteLine($"   Status Code: {response.StatusCode} ({(int)response.StatusCode})");
                System.Diagnostics.Debug.WriteLine($"   Status Description: {response.ReasonPhrase}");
                System.Diagnostics.Debug.WriteLine($"   Is Success: {response.IsSuccessStatusCode}");
                
                // Log response headers
                System.Diagnostics.Debug.WriteLine($"\n📥 Response Headers:");
                foreach (var header in response.Headers)
                {
                    System.Diagnostics.Debug.WriteLine($"   {header.Key}: {string.Join(", ", header.Value)}");
                }
                
                // Log response body
                var responseBody = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"\n📄 Response Body:");
                System.Diagnostics.Debug.WriteLine($"   {(string.IsNullOrWhiteSpace(responseBody) ? "(empty)" : responseBody.Substring(0, Math.Min(200, responseBody.Length)))}");
                
                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"\n✅ CONNECTIVITY TEST PASSED");
                    System.Diagnostics.Debug.WriteLine($"   Endpoint is reachable and accepting requests");
                    return (true, "Endpoint is reachable", responseBody);
                }
                else
                {
                    var errorMsg = $"Endpoint returned {response.StatusCode}: {response.ReasonPhrase}";
                    System.Diagnostics.Debug.WriteLine($"\n⚠️ CONNECTIVITY TEST FAILED");
                    System.Diagnostics.Debug.WriteLine($"   {errorMsg}");
                    System.Diagnostics.Debug.WriteLine($"\n💡 Possible causes:");
                    System.Diagnostics.Debug.WriteLine($"   • Incorrect endpoint path");
                    System.Diagnostics.Debug.WriteLine($"   • API not running on QA server");
                    System.Diagnostics.Debug.WriteLine($"   • Authentication headers incorrect");
                    System.Diagnostics.Debug.WriteLine($"   • Firewall blocking request");
                    System.Diagnostics.Debug.WriteLine($"   • SSL certificate issue");
                    
                    return (false, errorMsg, responseBody);
                }
            }
            catch (HttpRequestException ex)
            {
                var msg = $"HTTP Request Error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"\n❌ CONNECTIVITY TEST FAILED");
                System.Diagnostics.Debug.WriteLine($"   {msg}");
                System.Diagnostics.Debug.WriteLine($"\n💡 Possible causes:");
                System.Diagnostics.Debug.WriteLine($"   • Network unreachable");
                System.Diagnostics.Debug.WriteLine($"   • DNS resolution failed");
                System.Diagnostics.Debug.WriteLine($"   • SSL/TLS certificate error");
                System.Diagnostics.Debug.WriteLine($"   • Server not responding");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"\n📋 Inner Error: {ex.InnerException.Message}");
                }
                return (false, msg, ex.InnerException?.Message ?? ex.Message);
            }
            catch (Exception ex)
            {
                var msg = $"Unexpected Error: {ex.GetType().Name}: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"\n❌ CONNECTIVITY TEST FAILED");
                System.Diagnostics.Debug.WriteLine($"   {msg}");
                System.Diagnostics.Debug.WriteLine($"\n📋 Stack Trace:\n{ex.StackTrace}");
                return (false, msg, ex.StackTrace ?? "");
            }
        }
    }
}
