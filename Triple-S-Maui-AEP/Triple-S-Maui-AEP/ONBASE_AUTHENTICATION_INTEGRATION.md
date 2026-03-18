# OnBase Authentication Service Integration

## Overview
The login page now authenticates agents directly against the Hyland OnBase API using the `/api/document/verify-user` endpoint. This matches the authentication pattern used in the DMS upload service.

---

## Architecture

### Components

#### 1. OnBaseAuthenticationService (`Services/OnBaseAuthenticationService.cs`)
**Purpose:** Direct communication with Hyland OnBase API for credential verification

**Key Features:**
- ✅ Sends NPN and password as request headers (matching DMS pattern)
- ✅ Uses self-signed certificate handling for localhost development
- ✅ Includes stub mode for testing without API
- ✅ Comprehensive error handling with specific error messages
- ✅ Request timeout handling (30 seconds)
- ✅ Detailed debug logging for troubleshooting

**Endpoints Used:**
```
GET https://localhost:44304/api/document/verify-user
Headers:
  - Hyland-Username: {NPN}
  - Hyland-Password: {password}
  - Accept: application/json
```

#### 2. AgentLoginViewModel (`ViewModels/AgentLoginViewModel.cs`)
**Purpose:** Orchestrates login flow and validation

**Key Features:**
- ✅ Validates NPN format (8-10 digits)
- ✅ Validates password requirements (minimum 6 characters)
- ✅ Calls OnBaseAuthenticationService
- ✅ Manages IsLoading state for UI feedback
- ✅ Stores error messages for display

#### 3. AgentLoginPage (`Views/AgentLoginPage.xaml.cs`)
**Purpose:** User interface for login

**Already Implemented:**
- ✅ Bilingual support (English/Español)
- ✅ NPN and Password input fields
- ✅ Error message display
- ✅ Navigation to MainPage on success
- ✅ Session management via AgentSessionService

---

## Authentication Flow

```
1. User enters NPN and Password
                ↓
2. OnSignInClicked() validates inputs
                ↓
3. Calls _viewModel.AuthenticateAsync(npn, password)
                ↓
4. ViewModel validates NPN format and password requirements
                ↓
5. Calls OnBaseAuthenticationService.VerifyUserAsync()
                ↓
6. Service sends HTTP GET with headers:
   - Hyland-Username: {NPN}
   - Hyland-Password: {password}
                ↓
7. Receives response (success or error)
                ↓
8. Returns (Success, Message, AgentId)
                ↓
9. If success:
   - Call AgentSessionService.SetSession(agentId)
   - Navigate to MainPage
   
10. If failure:
   - Display error message in ErrorMessageLabel
```

---

## API Integration Details

### Request Format
```http
GET /api/document/verify-user HTTP/1.1
Host: localhost:44304
Hyland-Username: 1234567890
Hyland-Password: password123
Accept: application/json
```

### Response Codes

| Status | Meaning | Error Message |
|--------|---------|---------------|
| 200 OK | Credentials valid | "User verified successfully" |
| 401 Unauthorized | Invalid credentials | "Invalid NPN or password" |
| 403 Forbidden | No DMS access | "User does not have access to DMS" |
| 404 Not Found | User doesn't exist | "User not found in OnBase" |
| Other | Server error | "Verification failed: {status code}" |

### Error Handling

**Client-Side Validation:**
```csharp
// NPN Format
- Required (non-empty)
- 8-10 characters
- All digits

// Password
- Required (non-empty)
- At least 6 characters
```

**Server-Side Response Handling:**
```csharp
401 Unauthorized   → "Invalid NPN or password"
403 Forbidden      → "User does not have access to DMS"
404 Not Found      → "User not found in OnBase"
TimeoutException   → "Request timed out. Please check your connection and try again."
HttpRequestException → "Connection error: {message}"
Other Exception    → "Verification error: {message}"
```

---

## Testing Modes

### Stub Mode (for testing without API)
```csharp
// In AppShell.xaml.cs or startup:
OnBaseAuthenticationService.UseStubMode = true;  // Use mock responses
OnBaseAuthenticationService.UseStubMode = false; // Use actual API
```

**Behavior when stubbed:**
- Any NPN and password combination is accepted
- Returns success immediately with 500ms delay
- Useful for UI testing without DMS access

### Debug Logging

All authentication calls produce detailed debug output:

**Successful Login:**
```
=== ONBASE USER VERIFICATION START ===
Endpoint: GET /api/document/verify-user
Base URL: https://localhost:44304
NPN: 1234567890
Password: [REDACTED]
Sending verification request...
OnBase Response Status: OK
OnBase Response Body: ...
✓ User verification successful!
=== ONBASE USER VERIFICATION SUCCESS ===
```

**Failed Login:**
```
=== ONBASE USER VERIFICATION START ===
...
OnBase Response Status: Unauthorized
OnBase Response Body: ...
✗ Verification failed: Invalid NPN or password
=== ONBASE USER VERIFICATION FAILED ===
```

---

## Security Considerations

### Current Implementation
✅ Credentials sent via HTTPS request headers (matching DMS pattern)
✅ Password never logged (displayed as `[REDACTED]`)
✅ Self-signed certificate handling for development only

### Production Recommendations
1. **Move hardcoded URLs to configuration:**
   ```csharp
   // Instead of: const string ONBASE_BASE_URL = "https://localhost:44304";
   // Use AppSettings from configuration file
   ```

2. **Use environment-specific configurations:**
   - Development: localhost with self-signed certs
   - Staging/Production: proper SSL certificates

3. **Consider credential caching:**
   - Cache authentication tokens instead of re-authenticating
   - Implement token refresh mechanism

4. **Add rate limiting:**
   - Prevent brute force attacks on login endpoint
   - Implement exponential backoff for failed attempts

---

## Code Examples

### Using OnBaseAuthenticationService Directly

```csharp
var authService = new OnBaseAuthenticationService();
var (success, message, agentId) = await authService.VerifyUserAsync("1234567890", "password");

if (success)
{
    Console.WriteLine($"Agent verified: {agentId}");
}
else
{
    Console.WriteLine($"Verification failed: {message}");
}
```

### Validation Examples

```csharp
// NPN validation
if (OnBaseAuthenticationService.IsValidNPNFormat("1234567890"))
{
    // Valid: 10 digits
}

if (!OnBaseAuthenticationService.IsValidNPNFormat("12345"))
{
    // Invalid: less than 8 digits
}

// Password validation
if (OnBaseAuthenticationService.IsValidPassword("password123"))
{
    // Valid: 6+ characters
}

if (!OnBaseAuthenticationService.IsValidPassword("pass"))
{
    // Invalid: less than 6 characters
}
```

---

## Integration Points

### AgentSessionService
After successful authentication, session is created:

```csharp
Services.AgentSessionService.SetSession(agentId ?? npn);

// Stores:
// - CurrentAgentNPN: Agent's NPN
// - CurrentAgentName: Agent's display name
// - CurrentAgentId: Unique agent identifier
```

### LanguageService
Login page is fully bilingual:
- English (default)
- Spanish (Español)

All UI labels update based on selected language.

### DMS Service
Uses same authentication approach but for document operations:
- Base URL: `https://localhost:44304`
- Headers: `Hyland-Username`, `Hyland-Password`
- Purpose: Uploading documents to OnBase

---

## Troubleshooting

### "Connection error: The server returned an invalid or unrecognized response"
- Check if OnBase API is running
- Verify URL in OnBaseAuthenticationService
- Check firewall/network connectivity

### "Request timed out"
- OnBase API is not responding
- Network is slow (increase timeout if needed)
- API server is overloaded

### "Invalid NPN or password"
- NPN or password is incorrect
- User doesn't exist in OnBase
- User account is locked

### "User does not have access to DMS"
- User exists but has no DMS permissions
- Contact OnBase administrator to grant access

### Debug logging not showing
- Ensure debugging is enabled
- Check Visual Studio Debug Output window
- Build in Debug configuration

---

## Related Files

- `Services/OnBaseAuthenticationService.cs` - Authentication service
- `ViewModels/AgentLoginViewModel.cs` - Login ViewModel
- `Views/AgentLoginPage.xaml.cs` - Login page code-behind
- `Views/AgentLoginPage.xaml` - Login page XAML
- `Services/DMSService.cs` - Document upload (uses same auth pattern)
- `Services/AgentSessionService.cs` - Session management

---

## Version History

**Current:** v1.0 - Initial OnBase API integration
- Direct API authentication against /api/document/verify-user
- NPN and password validation
- Error handling with specific messages
- Stub mode for testing
- Bilingual support

---

## Future Enhancements

1. **Token-Based Authentication:**
   - Cache authentication tokens
   - Implement token refresh
   - Reduce API calls on subsequent navigation

2. **MFA (Multi-Factor Authentication):**
   - Add second factor verification
   - Integrate with OnBase MFA if available

3. **Credential Caching:**
   - Remember last used NPN (with security considerations)
   - Skip authentication for returning users (with timeout)

4. **Advanced Logging:**
   - Log authentication attempts to database
   - Track failed login attempts
   - Implement audit trail

5. **Rate Limiting:**
   - Implement exponential backoff
   - Lock account after N failed attempts
   - Display remaining attempts to user
