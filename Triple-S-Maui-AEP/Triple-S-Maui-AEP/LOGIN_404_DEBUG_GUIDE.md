# Login 404 Debugging Guide

## Enhanced Debug Logging Added ✅

I've added comprehensive request/response logging to help diagnose the 404 login error.

---

## How to Debug the 404 Error

### Step 1: Run the Login

1. Open the app
2. Enter NPN and password
3. Click Login button
4. Watch the Debug Output window

### Step 2: Open Debug Output Window

**Visual Studio → Debug → Windows → Output**

You'll see detailed logging like this:

```
╔════════════════════════════════════════════════════════════╗
║     ONBASE USER VERIFICATION START                         ║
╚════════════════════════════════════════════════════════════╝

📍 FULL REQUEST URL: https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/verify-user
📋 Base URL: https://soaqa.ssspr.com/Triple-S-AEP-API
📋 Endpoint: /api/document/verify-user
📋 HTTP Method: GET
⏱️  Timeout: 30 seconds

🔐 Credentials:
   NPN: 12345678
   Password: [REDACTED - 8 chars]

📬 Request Headers:
   Hyland-Username: 12345678
   Hyland-Password: [REDACTED]
   Accept: application/json

⏳ Sending verification request...

📥 RESPONSE RECEIVED (in 245ms)
   Status Code: 404 (NotFound)
   Status Description: Not Found
   Is Success: False

📋 Response Headers:
   Content-Type: application/json
   ...

📄 Response Body (124 bytes):
   {"error": "Endpoint not found"}

❌ Verification failed!
   Error: 404 Not Found - Check endpoint path: https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/verify-user

💡 404 Diagnosis:
   Full URL being called: https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/verify-user
   Possible causes:
   • Incorrect endpoint path (current: /api/document/verify-user)
   • Incorrect base URL (current: https://soaqa.ssspr.com/Triple-S-AEP-API)
   • API server not responding
   • Firewall blocking request
```

---

## Information Logged

### Request Information
- ✅ **Full URL** - The complete URL being called
- ✅ **Base URL** - The server domain
- ✅ **Endpoint** - The API path
- ✅ **HTTP Method** - GET, POST, etc.
- ✅ **Timeout** - Request timeout setting
- ✅ **Headers** - All headers being sent (password masked)
- ✅ **Credentials** - NPN and masked password length

### Response Information
- ✅ **Status Code** - HTTP status (200, 404, 401, etc.)
- ✅ **Response Time** - How long the request took (ms)
- ✅ **Response Headers** - Server response headers
- ✅ **Response Body** - Full server response (up to 500 chars)
- ✅ **Error Analysis** - Diagnosis for common errors

### Error Diagnostics
- ✅ **404 Not Found** - Shows exact URL and possible causes
- ✅ **Network Errors** - DNS, firewall, connectivity issues
- ✅ **Timeouts** - Server not responding
- ✅ **SSL/TLS Errors** - Certificate validation failures

---

## What the 404 Tells You

If you see **404 Not Found**, the problem is one of these:

| Issue | Solution |
|-------|----------|
| **Wrong endpoint path** | The `/api/document/verify-user` path might be wrong |
| **Wrong base URL** | The `https://soaqa.ssspr.com/Triple-S-AEP-API` might need adjustment |
| **API not running** | QA server might be down |
| **Firewall blocking** | Your network might be blocking the connection |

---

## Fixing the 404

### Option 1: Wrong Endpoint Path

If the endpoint path is wrong, update this file:

**File:** `Triple-S-Maui-AEP/Services/OnBaseAuthenticationService.cs` (line 13)

```csharp
// WRONG - Getting 404:
private const string VERIFY_USER_ENDPOINT = "/api/document/verify-user";

// TRY THESE - Ask your Triple-S team which is correct:
private const string VERIFY_USER_ENDPOINT = "/verify-user";
private const string VERIFY_USER_ENDPOINT = "/api/verify-user";
private const string VERIFY_USER_ENDPOINT = "/Triple-S-AEP-API/verify-user";
private const string VERIFY_USER_ENDPOINT = "/api/agent/verify";
```

### Option 2: Wrong Base URL

If the base URL is wrong, update this:

**File:** `Triple-S-Maui-AEP/Services/OnBaseAuthenticationService.cs` (line 12)

```csharp
// WRONG - Getting 404:
private const string ONBASE_BASE_URL = "https://soaqa.ssspr.com/Triple-S-AEP-API";

// TRY THESE - Ask your Triple-S team which is correct:
private const string ONBASE_BASE_URL = "https://soaqa.ssspr.com";
private const string ONBASE_BASE_URL = "https://soaqa.ssspr.com/api";
```

### Option 3: Test with TestConnectivityAsync()

If you want to test the endpoint separately (without login):

```csharp
// In DashboardViewModel or a test page
var dmsService = new DMSService();
var (connected, message, details) = await dmsService.TestConnectivityAsync();

System.Diagnostics.Debug.WriteLine($"Connected: {connected}");
System.Diagnostics.Debug.WriteLine($"Message: {message}");
System.Diagnostics.Debug.WriteLine($"Details: {details}");
```

---

## Common 404 Scenarios

### Scenario 1: Endpoint Path Wrong

**Debug Output Shows:**
```
Full URL: https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/verify-user
Status Code: 404
```

**Fix:** Ask Triple-S what the correct endpoint should be. Examples:
- `/verify-user` (remove `/api/document/`)
- `/agent/verify` (different path)
- Just `/api/` (different structure)

### Scenario 2: Base URL Double Pathing

**Debug Output Shows:**
```
Full URL: https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/verify-user
         ↑ Notice the double path: /Triple-S-AEP-API/api/...
```

**Fix:** Change base URL to just the domain:
```csharp
// Change FROM:
private const string ONBASE_BASE_URL = "https://soaqa.ssspr.com/Triple-S-AEP-API";

// Change TO:
private const string ONBASE_BASE_URL = "https://soaqa.ssspr.com";

// Then add full path to endpoint:
private const string VERIFY_USER_ENDPOINT = "/Triple-S-AEP-API/api/document/verify-user";
```

### Scenario 3: Server Down

**Debug Output Shows:**
```
Status Code: 504 (Gateway Timeout)
Response: Server not responding
```

**Fix:** 
- Wait for QA server to come back up
- Check with IT if server is running

---

## Next Steps

1. **Try to login**
2. **Open Debug Output window** (Debug → Windows → Output)
3. **Share the output** with your Triple-S team, specifically:
   - The full URL being called
   - The HTTP status code
   - The response body
   - Any error messages

4. **Ask them:**
   - "What's the correct endpoint URL?"
   - "What's the correct base URL?"
   - "What's the correct endpoint path?"

5. **Update the configuration** once you know the correct values:
   - `OnBaseAuthenticationService.cs` lines 12-13

6. **Rebuild and test**

---

## Files Modified

- ✅ `Triple-S-Maui-AEP/Services/OnBaseAuthenticationService.cs`
  - Added comprehensive request/response logging
  - Added 404 diagnostic messages
  - Added network error diagnosis
  - Shows full URL, headers, request body, response body
  - Timing information (how long the request took)

---

## What You'll See in Debug Output

### Successful Login:
```
╔════════════════════════════════════════════════════════════╗
║     ONBASE USER VERIFICATION SUCCESS ✓                      ║
╚════════════════════════════════════════════════════════════╝
```

### Failed with 404:
```
❌ Verification failed!
   Error: 404 Not Found - Check endpoint path: ...

💡 404 Diagnosis:
   Full URL being called: ...
```

### Network Error:
```
❌ HttpRequestException
   Message: The remote name could not be resolved
   
💡 Network Diagnosis:
   Possible causes:
   • Network connectivity issue
   • DNS resolution failed
```

### Timeout:
```
⏱️  Request timeout
   Message: A task was canceled
```

---

## Quick Reference

**Key Lines in Debug Output:**

```
📍 FULL REQUEST URL     → The exact URL being called (main thing to check!)
💡 404 Diagnosis        → Specific help for 404 errors
⏳ Sending...            → Request is being sent
📥 RESPONSE RECEIVED    → Response came back (with timing)
Status Code: XXX        → HTTP status (404, 401, 200, etc.)
```

---

## Support

If you're still getting 404 after checking everything, gather this info:

1. Copy entire Debug Output
2. Note the status code
3. Note the exact URL being called
4. Note if it's a network error or HTTP error
5. Share with Triple-S team

They can then tell you the correct endpoint!
