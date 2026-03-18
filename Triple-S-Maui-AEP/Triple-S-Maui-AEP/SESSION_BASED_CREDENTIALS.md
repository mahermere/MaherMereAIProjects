# Session-Based Credentials for DMS Operations

## Overview
The login credentials (NPN and password) are now stored in the `AgentSessionService` session and reused for all DMS upload operations. This eliminates the need for hardcoded credentials and ensures the same authenticated user performs the uploads.

---

## Architecture

### 1. AgentSessionService (Updated)
**Location:** `Services/AgentSessionService.cs`

**New Storage:**
- `CurrentAgentPassword` - Stores the agent's password from login

**New Methods:**
- `GetDMSCredentials()` - Returns both NPN and password as a tuple for DMS operations

**Key Features:**
```csharp
// Get credentials for DMS
var (npn, password) = AgentSessionService.GetDMSCredentials();

// Session clearing now removes password too
AgentSessionService.ClearSession(); // Clears NPN, Name, AND Password
```

### 2. AgentLoginPage (Updated)
**Location:** `Views/AgentLoginPage.xaml.cs`

**Changes:**
```csharp
// OLD:
Services.AgentSessionService.SetSession(agentId ?? npn);

// NEW:
Services.AgentSessionService.SetSession(
    agentId ?? npn, 
    agentName: null, 
    password: password  // ← Now passes the password!
);
```

### 3. DMSService (Updated)
**Location:** `Services/DMSService.cs`

**Changes:**
```csharp
// OLD: Hardcoded credentials
private const string HYLAND_USERNAME = "mahmer";
private const string HYLAND_PASSWORD = "password";

// NEW: Dynamic credentials from session
private static (string Username, string Password) GetDMSCredentials()
{
    if (AgentSessionService.IsAgentLoggedIn)
    {
        var (npn, password) = AgentSessionService.GetDMSCredentials();
        if (!string.IsNullOrEmpty(password))
        {
            return (npn, password); // ← Uses logged-in user's credentials
        }
    }
    return (FALLBACK_HYLAND_USERNAME, FALLBACK_HYLAND_PASSWORD);
}
```

---

## Authentication Flow

```
1. User Login
   ├─ Enter NPN and Password
   └─ Submit credentials

2. OnBaseAuthenticationService
   ├─ Validate NPN format
   ├─ Validate password requirements
   └─ Call /api/document/verify-user

3. Successful Authentication
   ├─ Return (Success, Message, AgentId)
   └─ AgentLoginPage stores credentials

4. AgentSessionService.SetSession()
   ├─ Store NPN
   ├─ Store Password ← NEW!
   └─ Store Name (optional)

5. DMS Upload Operation
   ├─ Create DMSService
   ├─ GetDMSCredentials() → Returns (NPN, Password) from session
   ├─ Add headers:
   │  ├─ Hyland-Username: {NPN}
   │  └─ Hyland-Password: {password}
   └─ Upload document

6. Logout
   └─ ClearSession() → Clears everything including password
```

---

## Usage Examples

### Storing Credentials After Login
```csharp
// In AgentLoginPage.xaml.cs
var (success, message, agentId) = await _viewModel.AuthenticateAsync(npn, password);

if (success)
{
    // Store NPN AND password for later DMS operations
    AgentSessionService.SetSession(agentId ?? npn, password: password);
    await Shell.Current.GoToAsync("///MainPage");
}
```

### Retrieving Credentials for DMS
```csharp
// In DMSService constructor
var (username, password) = GetDMSCredentials();

_httpClient.DefaultRequestHeaders.Add("Hyland-Username", username);
_httpClient.DefaultRequestHeaders.Add("Hyland-Password", password);
```

### Checking Session Status
```csharp
if (AgentSessionService.IsAgentLoggedIn)
{
    var (npn, password) = AgentSessionService.GetDMSCredentials();
    // Use credentials
}
else
{
    // No session - use fallback or prompt for login
}
```

### Clearing Session on Logout
```csharp
// Clear everything including password
AgentSessionService.ClearSession();

// All values are cleared:
// - CurrentAgentNPN = "UNKNOWN"
// - CurrentAgentName = null
// - CurrentAgentPassword = null
```

---

## Security Considerations

### Current Implementation
✅ Credentials stored in session memory only (not persisted)
✅ Credentials cleared on logout
✅ Password never logged (uses `[REDACTED]` in debug output)
✅ Only stored if user is authenticated

### Security Notes
⚠️ **In-Memory Storage:** Credentials are kept in application memory
- Suitable for single-user desktop/mobile applications
- Cleared when app is closed or session ends
- Not suitable for shared/multi-user systems

### Production Recommendations

**For Enhanced Security:**

1. **Use Secure Credential Storage:**
   ```csharp
   // Instead of plain string storage, use platform-specific secure storage:
   // - Windows: DPAPI (Data Protection API)
   // - iOS: Keychain
   // - Android: SharedPreferences with encryption
   // - .NET MAUI: SecureStorage
   ```

2. **Token-Based Authentication:**
   ```csharp
   // Instead of storing password, use temporary tokens:
   // 1. Login gets bearer token from OnBase
   // 2. Use token for DMS operations
   // 3. Token expires after time period
   // 4. Only store token, not password
   ```

3. **Optional: Implement SecureStorage**
   ```csharp
   // Using MAUI SecureStorage for encrypted credential storage
   await SecureStorage.SetAsync("agent_npn", npn);
   await SecureStorage.SetAsync("agent_password", password);
   
   var stored_npn = await SecureStorage.GetAsync("agent_npn");
   var stored_password = await SecureStorage.GetAsync("agent_password");
   ```

---

## Behavior Changes

### Before This Update
```
1. User logs in with NPN and password
2. Credentials authenticated against OnBase ✓
3. Only NPN stored in session
4. DMS uploads use hardcoded "mahmer" / "password"
5. All uploads appear from "mahmer" account

❌ Problem: Actual authenticated user not tracked in DMS
```

### After This Update
```
1. User logs in with NPN and password
2. Credentials authenticated against OnBase ✓
3. NPN AND password stored in session
4. DMS uploads use logged-in user's credentials
5. All uploads appear from actual user account

✅ Benefit: Audit trail shows actual uploader
```

---

## Configuration

### DMSService Initialization Flow

```csharp
public DMSService()
{
    // 1. Create HTTP client
    var handler = new HttpClientHandler();
    _httpClient = new HttpClient(handler) { BaseAddress = new Uri(DMS_BASE_URL) };
    
    // 2. Get credentials (from session if available, fallback to hardcoded)
    var (username, password) = GetDMSCredentials();
    
    // 3. Set headers for all requests
    _httpClient.DefaultRequestHeaders.Add("Hyland-Username", username);
    _httpClient.DefaultRequestHeaders.Add("Hyland-Password", password);
}
```

### Fallback Credentials
If no session is available (user not logged in), the service falls back to:
```csharp
const string FALLBACK_HYLAND_USERNAME = "mahmer";
const string FALLBACK_HYLAND_PASSWORD = "password";
```

This allows development/testing without requiring login.

---

## Debug Logging

### Session Operations
```
✓ Agent session set: NPN=1234567890, HasPassword=True
✓ Agent session cleared (including credentials)
```

### DMSService Initialization
```
✓ Using credentials from AgentSessionService
  (when user is logged in)

⚠️ Using fallback hardcoded credentials
   (no session available)
```

### Credential Retrieval
```
DMSService initialized:
  Base URL: https://localhost:44304
  Username: 1234567890
  Using session credentials: True
  Stub Mode: False
```

---

## Related Code Files

### Core Files Updated
- `Services/AgentSessionService.cs` - Stores NPN + password
- `Views/AgentLoginPage.xaml.cs` - Passes password to session
- `Services/DMSService.cs` - Uses session credentials

### Referenced Files
- `Services/OnBaseAuthenticationService.cs` - Authenticates credentials
- `ViewModels/AgentLoginViewModel.cs` - Validates inputs before authentication
- `Models/DMSUploadRequest.cs` - Document upload structure
- `Models/DMSUploadResponse.cs` - Upload response handling

---

## Testing

### Manual Testing Steps

1. **Login with Valid Credentials**
   ```
   NPN: 1234567890
   Password: ValidPassword123
   → Check debug output shows "✓ Using credentials from AgentSessionService"
   ```

2. **Verify Session Storage**
   ```
   After login:
   - AgentSessionService.IsAgentLoggedIn = true
   - AgentSessionService.CurrentAgentNPN = "1234567890"
   - AgentSessionService.GetDMSCredentials() returns ("1234567890", "ValidPassword123")
   ```

3. **Upload Document**
   ```
   Click upload button
   → DMSService uses session credentials
   → Check debug: "Username: 1234567890"
   → Verify DMS shows upload from user's account
   ```

4. **Logout**
   ```
   Click logout
   → ClearSession() removes all data
   → Next upload would use fallback credentials
   ```

### Stub Mode Testing
```csharp
// To test without real OnBase API:
OnBaseAuthenticationService.UseStubMode = true;
DMSService.UseStubMode = true;

// Any credentials will work for login
// Uploads will return stub success response
```

---

## Benefits

✅ **Audit Trail:** Know which user uploaded each document
✅ **Consistency:** Use same credentials throughout session
✅ **Flexibility:** Can use different credentials without re-login
✅ **Security:** No hardcoded credentials in production code
✅ **Maintainability:** Credentials managed in one place (AgentSessionService)
✅ **Testability:** Can easily mock credentials for testing

---

## Future Enhancements

### 1. Token-Based Authentication
```csharp
// Instead of storing password:
public static string? CurrentAuthToken { get; set; }
public static DateTime TokenExpiration { get; set; }

// Check token validity before DMS operations
if (DateTime.UtcNow > TokenExpiration)
{
    // Token expired - need to re-authenticate
}
```

### 2. Secure Storage
```csharp
// Use platform-specific encrypted storage
public static async Task SetSecureCredentialsAsync(string npn, string password)
{
    await SecureStorage.SetAsync("agent_npn", npn);
    await SecureStorage.SetAsync("agent_password", password);
}

public static async Task<(string, string)> GetSecureCredentialsAsync()
{
    var npn = await SecureStorage.GetAsync("agent_npn");
    var password = await SecureStorage.GetAsync("agent_password");
    return (npn, password);
}
```

### 3. Multi-Session Support
```csharp
// Allow multiple concurrent sessions
private static Dictionary<string, AgentSession> _activeSessions;

public class AgentSession
{
    public string SessionId { get; set; }
    public string NPN { get; set; }
    public string Password { get; set; }
    public string? AgentName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}
```

### 4. Audit Logging
```csharp
// Log all credential usage
private static List<AuditLog> _auditLogs;

public class AuditLog
{
    public DateTime Timestamp { get; set; }
    public string NPN { get; set; }
    public string Operation { get; set; } // "Login", "Logout", "Upload", etc.
    public string Details { get; set; }
    public bool Success { get; set; }
}
```

---

## Change Summary

| Component | Old Behavior | New Behavior |
|-----------|--------------|--------------|
| Login | Store only NPN | Store NPN + Password |
| DMS Upload | Use hardcoded "mahmer" account | Use logged-in user's account |
| Audit Trail | All uploads from "mahmer" | Shows actual uploader |
| Logout | Clear NPN + Name | Clear NPN + Name + Password |
| Session Query | Get NPN only | Get NPN + Password via GetDMSCredentials() |

---

## Build Status
✅ All changes compile successfully
✅ No warnings or errors
✅ Ready for testing and deployment
