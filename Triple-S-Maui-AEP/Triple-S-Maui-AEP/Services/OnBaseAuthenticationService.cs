using System.Net.Http.Headers;

namespace Triple_S_Maui_AEP.Services
{
    /// <summary>
    /// Service for authenticating agents against Hyland OnBase API
    /// Uses the same endpoint and authentication headers as DMS operations
    /// </summary>
    public class OnBaseAuthenticationService
    {
        private readonly HttpClient _httpClient;
        
        // CORRECTED: Use base domain only, full path in endpoint
        // The server is rewriting URLs and stripping the virtual directory
        private const string ONBASE_BASE_URL = "https://soaqa.ssspr.com";
        private const string VERIFY_USER_ENDPOINT = "Triple-S-AEP-API/api/document/verify-user";

        /// <summary>
        /// Set to true to use stub/mock responses for testing login without actual API
        /// </summary>
        public static bool UseStubMode { get; set; } = false;

        public OnBaseAuthenticationService()
        {
            // Create HttpClientHandler that allows self-signed certificates for development
            var handler = new HttpClientHandler();
            
            System.Diagnostics.Debug.WriteLine("\nConfiguring HttpClientHandler:");
            System.Diagnostics.Debug.WriteLine($"   AllowAutoRedirect: {handler.AllowAutoRedirect}");
            System.Diagnostics.Debug.WriteLine($"   UseProxy: {handler.UseProxy}");
            System.Diagnostics.Debug.WriteLine($"   Proxy: {handler.Proxy}");
            System.Diagnostics.Debug.WriteLine($"   AutomaticDecompression: {handler.AutomaticDecompression}");

            // Allow self-signed certificates for localhost development
#if DEBUG
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                System.Diagnostics.Debug.WriteLine("\nCertificate Validation Callback");
                System.Diagnostics.Debug.WriteLine($"   Host: {message.RequestUri?.Host}");
                System.Diagnostics.Debug.WriteLine($"   Certificate Subject: {cert?.Subject}");
                System.Diagnostics.Debug.WriteLine($"   Certificate Valid: {cert?.Verify()}");
                System.Diagnostics.Debug.WriteLine($"   SSL Errors: {errors}");
                
                // For development: allow self-signed certificates on localhost
                if (message.RequestUri?.Host == "localhost" || message.RequestUri?.Host == "127.0.0.1")
                {
                    System.Diagnostics.Debug.WriteLine("   ALLOWING: Self-signed certificate for localhost");
                    return true;
                }

                // For QA and Production: validate certificates properly
                var isValid = errors == System.Net.Security.SslPolicyErrors.None;
                System.Diagnostics.Debug.WriteLine($"   Result: {(isValid ? "VALID" : "INVALID")}");
                return isValid;
            };
#endif

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(ONBASE_BASE_URL),
                Timeout = TimeSpan.FromSeconds(30)
            };

            System.Diagnostics.Debug.WriteLine("\nOnBaseAuthenticationService initialized:");
            System.Diagnostics.Debug.WriteLine($"   Base URL: {ONBASE_BASE_URL}");
            System.Diagnostics.Debug.WriteLine($"   Verify User Endpoint: {VERIFY_USER_ENDPOINT}");
            System.Diagnostics.Debug.WriteLine($"   Full URL will be: {ONBASE_BASE_URL}{VERIFY_USER_ENDPOINT}");
            System.Diagnostics.Debug.WriteLine($"   HttpClient Timeout: {_httpClient.Timeout.TotalSeconds} seconds");
        }

        /// <summary>
        /// Verifies agent credentials against OnBase API
        /// </summary>
        /// <param name="npn">Agent NPN (National Producer Number)</param>
        /// <param name="password">Agent password</param>
        /// <returns>Tuple with (Success, Message, AgentIdentifier)</returns>
        public async Task<(bool Success, string Message, string? AgentIdentifier)> VerifyUserAsync(string npn, string password)
        {
            // STUB MODE: Return successful response for testing
            if (UseStubMode)
            {
                System.Diagnostics.Debug.WriteLine($"[STUB MODE] Verifying user: {npn}");
                await Task.Delay(500); // Simulate network delay

                return (
                    true,
                    "User verified successfully (stubbed)",
                    npn
                );
            }

            // REAL MODE: Make actual API call to OnBase
            try
            {
                System.Diagnostics.Debug.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
                System.Diagnostics.Debug.WriteLine("║     ONBASE USER VERIFICATION START                         ║");
                System.Diagnostics.Debug.WriteLine("╚════════════════════════════════════════════════════════════╝\n");
                
                // Calculate full URL
                var fullUrl = $"{ONBASE_BASE_URL}{VERIFY_USER_ENDPOINT}";
                
                System.Diagnostics.Debug.WriteLine($"📍 FULL REQUEST URL: {fullUrl}");
                System.Diagnostics.Debug.WriteLine($"📍 Expected (from curl): https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/verify-user");
                System.Diagnostics.Debug.WriteLine($"📍 Match: {fullUrl == "https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/verify-user"}");
                
                System.Diagnostics.Debug.WriteLine($"\n📋 URL Components:");
                System.Diagnostics.Debug.WriteLine($"   Base URL: '{ONBASE_BASE_URL}'");
                System.Diagnostics.Debug.WriteLine($"   Endpoint: '{VERIFY_USER_ENDPOINT}'");
                System.Diagnostics.Debug.WriteLine($"   HttpClient.BaseAddress: '{_httpClient.BaseAddress}'");
                System.Diagnostics.Debug.WriteLine($"   HTTP Method: GET");
                System.Diagnostics.Debug.WriteLine($"⏱️  Timeout: {_httpClient.Timeout.TotalSeconds} seconds");
                System.Diagnostics.Debug.WriteLine($"\n🔐 Credentials:");
                System.Diagnostics.Debug.WriteLine($"   NPN: {npn}");
                System.Diagnostics.Debug.WriteLine($"   Password: [REDACTED - {password?.Length ?? 0} chars]");

                // Create request with NPN and password in headers (matching DMS pattern)
                var request = new HttpRequestMessage(HttpMethod.Get, VERIFY_USER_ENDPOINT);
                
                System.Diagnostics.Debug.WriteLine($"\n📬 Setting Request Headers:");
                System.Diagnostics.Debug.WriteLine($"   Adding: Hyland-Username = {npn}");
                request.Headers.Add("Hyland-Username", npn);
                
                System.Diagnostics.Debug.WriteLine($"   Adding: Hyland-Password = [REDACTED]");
                request.Headers.Add("Hyland-Password", password);
                
                System.Diagnostics.Debug.WriteLine($"   Adding: Accept = application/json");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                System.Diagnostics.Debug.WriteLine($"\n📬 Final Request Headers Before Send:");
                foreach (var header in request.Headers)
                {
                    var value = header.Key.Contains("Password") ? "[REDACTED]" : string.Join(", ", header.Value);
                    System.Diagnostics.Debug.WriteLine($"   {header.Key}: {value}");
                }
                
                System.Diagnostics.Debug.WriteLine($"\n📬 HttpClient.DefaultRequestHeaders (merged with request):");
                foreach (var header in _httpClient.DefaultRequestHeaders)
                {
                    var value = header.Key.Contains("Password") ? "[REDACTED]" : string.Join(", ", header.Value);
                    System.Diagnostics.Debug.WriteLine($"   {header.Key}: {value}");
                }
                
                System.Diagnostics.Debug.WriteLine($"\n⏳ Calling SendAsync()...");
                System.Diagnostics.Debug.WriteLine($"   Request.RequestUri: {request.RequestUri}");
                System.Diagnostics.Debug.WriteLine($"   Request.Method: {request.Method}");
                
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var response = await _httpClient.SendAsync(request);
                stopwatch.Stop();

                var responseBody = await response.Content.ReadAsStringAsync();
                
                System.Diagnostics.Debug.WriteLine($"\n📥 RESPONSE RECEIVED (in {stopwatch.ElapsedMilliseconds}ms)");
                System.Diagnostics.Debug.WriteLine($"   Status Code: {response.StatusCode} ({(int)response.StatusCode})");
                System.Diagnostics.Debug.WriteLine($"   Status Description: {response.ReasonPhrase}");
                System.Diagnostics.Debug.WriteLine($"   Is Success: {response.IsSuccessStatusCode}");
                System.Diagnostics.Debug.WriteLine($"   Response.RequestMessage.RequestUri: {response.RequestMessage?.RequestUri}");
                
                System.Diagnostics.Debug.WriteLine($"\n📋 Response Headers:");
                foreach (var header in response.Headers)
                {
                    System.Diagnostics.Debug.WriteLine($"   {header.Key}: {string.Join(", ", header.Value)}");
                }
                
                System.Diagnostics.Debug.WriteLine($"\n📄 Response Body ({responseBody?.Length ?? 0} bytes):");
                if (string.IsNullOrWhiteSpace(responseBody))
                {
                    System.Diagnostics.Debug.WriteLine($"   (empty)");
                }
                else if (responseBody.Length > 500)
                {
                    System.Diagnostics.Debug.WriteLine($"   {responseBody.Substring(0, 500)}...");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"   {responseBody}");
                }

                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"\n✅ User verification successful!");
                    System.Diagnostics.Debug.WriteLine("╔════════════════════════════════════════════════════════════╗");
                    System.Diagnostics.Debug.WriteLine("║     ONBASE USER VERIFICATION SUCCESS ✓                      ║");
                    System.Diagnostics.Debug.WriteLine("╚════════════════════════════════════════════════════════════╝\n");

                    return (
                        true,
                        "User verified successfully",
                        npn  // Return NPN as the agent identifier
                    );
                }
                else
                {
                    string errorMessage = response.StatusCode switch
                    {
                        System.Net.HttpStatusCode.NotFound => $"404 Not Found - Check endpoint path: {fullUrl}",
                        System.Net.HttpStatusCode.Unauthorized => "Invalid NPN or password",
                        System.Net.HttpStatusCode.Forbidden => "User does not have access to DMS",
                        _ => $"Verification failed: {response.StatusCode}"
                    };

                    System.Diagnostics.Debug.WriteLine($"\n❌ Verification failed!");
                    System.Diagnostics.Debug.WriteLine($"   Error: {errorMessage}");
                    
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        System.Diagnostics.Debug.WriteLine($"\n💡 404 Diagnosis:");
                        System.Diagnostics.Debug.WriteLine($"   Full URL being called: {fullUrl}");
                        System.Diagnostics.Debug.WriteLine($"   Expected URL: https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/verify-user");
                        System.Diagnostics.Debug.WriteLine($"   URLs match: {fullUrl == "https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/verify-user"}");
                        System.Diagnostics.Debug.WriteLine($"\n   Response RequestUri: {response.RequestMessage?.RequestUri}");
                        System.Diagnostics.Debug.WriteLine($"\n   Note: curl works fine with this URL, so server is accepting the endpoint.");
                        System.Diagnostics.Debug.WriteLine($"   Possible causes:");
                        System.Diagnostics.Debug.WriteLine($"   • Missing or wrong header values");
                        System.Diagnostics.Debug.WriteLine($"   • HttpClientHandler configuration (proxy, etc.)");
                        System.Diagnostics.Debug.WriteLine($"   • Request redirects being handled differently");
                        System.Diagnostics.Debug.WriteLine($"   • Server is blocking the request for some reason");
                    }
                    
                    System.Diagnostics.Debug.WriteLine("╔════════════════════════════════════════════════════════════╗");
                    System.Diagnostics.Debug.WriteLine("║     ONBASE USER VERIFICATION FAILED ✗                       ║");
                    System.Diagnostics.Debug.WriteLine("╚════════════════════════════════════════════════════════════╝\n");

                    return (
                        false,
                        errorMessage,
                        null
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"\n❌ HttpRequestException");
                System.Diagnostics.Debug.WriteLine($"   Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   InnerException: {ex.InnerException?.Message}");
                System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
                
                System.Diagnostics.Debug.WriteLine($"\n💡 Network Diagnosis:");
                System.Diagnostics.Debug.WriteLine($"   Full URL: {ONBASE_BASE_URL}{VERIFY_USER_ENDPOINT}");
                System.Diagnostics.Debug.WriteLine($"   Possible causes:");
                System.Diagnostics.Debug.WriteLine($"   • Network connectivity issue");
                System.Diagnostics.Debug.WriteLine($"   • DNS resolution failed (can't reach soaqa.ssspr.com)");
                System.Diagnostics.Debug.WriteLine($"   • SSL/TLS certificate error");
                System.Diagnostics.Debug.WriteLine($"   • Firewall/proxy blocking");
                
                System.Diagnostics.Debug.WriteLine("╔════════════════════════════════════════════════════════════╗");
                System.Diagnostics.Debug.WriteLine("║     ONBASE USER VERIFICATION EXCEPTION ✗                     ║");
                System.Diagnostics.Debug.WriteLine("╚════════════════════════════════════════════════════════════╝\n");

                return (
                    false,
                    $"Connection error: {ex.Message}",
                    null
                );
            }
            catch (TaskCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine($"\n⏱️  Request timeout");
                System.Diagnostics.Debug.WriteLine($"   Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   Endpoint: {ONBASE_BASE_URL}{VERIFY_USER_ENDPOINT}");
                System.Diagnostics.Debug.WriteLine($"   Possible causes:");
                System.Diagnostics.Debug.WriteLine($"   • Server not responding");
                System.Diagnostics.Debug.WriteLine($"   • Network latency");
                System.Diagnostics.Debug.WriteLine($"   • Firewall/proxy blocking");
                System.Diagnostics.Debug.WriteLine("╔════════════════════════════════════════════════════════════╗");
                System.Diagnostics.Debug.WriteLine("║     ONBASE USER VERIFICATION TIMEOUT ✗                       ║");
                System.Diagnostics.Debug.WriteLine("╚════════════════════════════════════════════════════════════╝\n");

                return (
                    false,
                    "Request timed out. Please check your connection and try again.",
                    null
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"\n❌ Unexpected exception: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"   Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine("╔════════════════════════════════════════════════════════════╗");
                System.Diagnostics.Debug.WriteLine("║     ONBASE USER VERIFICATION EXCEPTION ✗                     ║");
                System.Diagnostics.Debug.WriteLine("╚════════════════════════════════════════════════════════════╝\n");

                return (
                    false,
                    $"Verification error: {ex.Message}",
                    null
                );
            }
        }

        /// <summary>
        /// Validates NPN format (8-10 digits)
        /// </summary>
        public static bool IsValidNPNFormat(string npn)
        {
            if (string.IsNullOrWhiteSpace(npn))
                return false;

            if (npn.Length < 8 || npn.Length > 10)
                return false;

            return npn.All(char.IsDigit);
        }

        /// <summary>
        /// Validates password requirements
        /// </summary>
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            return password.Length >= 6;
        }
    }
}
