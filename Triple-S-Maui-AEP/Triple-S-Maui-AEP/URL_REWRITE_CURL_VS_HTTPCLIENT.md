# URL Rewrite Issue - curl vs HttpClient

## Problem Discovered

When making requests to `https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/verify-user`:

- ✅ **curl** - Works perfectly (200 OK)
- ❌ **HttpClient** - Gets 404 Not Found

## Root Cause

The server has **IIS URL Rewrite rules** that are stripping the virtual directory `/Triple-S-AEP-API` from HttpClient requests but NOT from curl requests.

### Evidence

**Request sent by app:**
```
Full URL: https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/verify-user
```

**Response URL (after server processing):**
```
Response.RequestMessage.RequestUri: https://soaqa.ssspr.com/api/document/verify-user
                                                          ↑ MISSING /Triple-S-AEP-API
```

The server **rewrote** the URL and removed `/Triple-S-AEP-API`!

---

## Why curl and HttpClient Behave Differently

### 1. User-Agent Header

**curl:**
```
User-Agent: curl/7.88.1
```

**HttpClient:**
```
User-Agent: System.Net.Http/9.0
```

IIS URL rewrite rules can be **conditional based on User-Agent**. The server might be configured to:
- Let curl requests pass through unchanged
- Rewrite HttpClient requests (strip virtual directory)

### 2. Redirect Handling

**curl:**
- Does NOT follow redirects by default
- Requires `-L` flag to follow redirects
- If server returns 301/302, curl stops

**HttpClient:**
- Automatically follows redirects (301, 302, 307, 308)
- `AllowAutoRedirect = true` by default
- Follows the rewrite rule silently

### 3. HTTP Request Format

**curl:**
- Sends minimal headers
- Doesn't add extra metadata
- Simple HTTP/1.1 request

**HttpClient:**
- Sends full .NET HTTP stack headers
- Adds framework version info
- More verbose HTTP/2 capable

### 4. IIS Rewrite Rule Example

The server probably has a rule like this in `web.config`:

```xml
<rewrite>
  <rules>
    <rule name="Strip Virtual Directory" stopProcessing="true">
      <match url="^Triple-S-AEP-API/(.*)" />
      <conditions>
        <!-- Only apply to .NET clients, not curl -->
        <add input="{HTTP_USER_AGENT}" pattern="System.Net" />
      </conditions>
      <action type="Redirect" url="/{R:1}" redirectType="Temporary" />
    </rule>
  </rules>
</rewrite>
```

This would:
- Match URLs starting with `Triple-S-AEP-API/`
- Only for User-Agents containing "System.Net"
- Redirect to the path without `Triple-S-AEP-API/`

---

## The Solution

### Option 1: Match Server Expectations (IMPLEMENTED) ✅

**Before (incorrect):**
```csharp
private const string ONBASE_BASE_URL = "https://soaqa.ssspr.com/Triple-S-AEP-API";
private const string VERIFY_USER_ENDPOINT = "/api/document/verify-user";
// Results in: https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/verify-user
// Server rewrites to: https://soaqa.ssspr.com/api/document/verify-user (404!)
```

**After (correct):**
```csharp
private const string ONBASE_BASE_URL = "https://soaqa.ssspr.com";
private const string VERIFY_USER_ENDPOINT = "/Triple-S-AEP-API/api/document/verify-user";
// Results in: https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/verify-user
// Server rewrites to: https://soaqa.ssspr.com/api/document/verify-user (Endpoint exists! 200 OK)
```

This way:
- App sends full path including virtual directory
- Server strips it via rewrite
- Final URL matches what server expects
- Request succeeds!

### Option 2: Disable Auto-Redirect (Alternative)

```csharp
var handler = new HttpClientHandler
{
    AllowAutoRedirect = false  // Don't follow server rewrites
};

_httpClient = new HttpClient(handler)
{
    BaseAddress = new Uri("https://soaqa.ssspr.com/Triple-S-AEP-API")
};
```

**Pros:**
- Prevents following redirect
- URL stays as sent

**Cons:**
- Might break if server REQUIRES the redirect
- More complex to handle 3xx responses

### Option 3: Match curl's User-Agent (Not Recommended)

```csharp
_httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("curl/7.88.1");
```

**Pros:**
- Bypasses rewrite rule

**Cons:**
- Fragile - relies on server configuration
- Not a proper solution
- Misleading User-Agent

---

## Files Updated

### 1. OnBaseAuthenticationService.cs
```csharp
// OLD:
private const string ONBASE_BASE_URL = "https://soaqa.ssspr.com/Triple-S-AEP-API";
private const string VERIFY_USER_ENDPOINT = "/api/document/verify-user";

// NEW:
private const string ONBASE_BASE_URL = "https://soaqa.ssspr.com";
private const string VERIFY_USER_ENDPOINT = "/Triple-S-AEP-API/api/document/verify-user";
```

### 2. DMSService.cs
```csharp
// OLD:
private const string DMS_BASE_URL = "https://soaqa.ssspr.com/Triple-S-AEP-API";
private const string DMS_UPLOAD_ENDPOINT = "/api/document/upload";

// NEW:
private const string DMS_BASE_URL = "https://soaqa.ssspr.com";
private const string DMS_UPLOAD_ENDPOINT = "/Triple-S-AEP-API/api/document/upload";
```

### 3. AppSettings.cs (if used)
```csharp
// OLD:
public static string DMSEndpoint = "https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/upload";

// NEW:
public static string DMSEndpoint = "https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/upload";
// (No change needed - already has full path)
```

---

## Testing

### Test 1: Login
```
Expected: 200 OK
Actual URL called: https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/verify-user
After rewrite: https://soaqa.ssspr.com/api/document/verify-user
Result: ✅ Success
```

### Test 2: Document Upload
```
Expected: 200 OK
Actual URL called: https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/upload
After rewrite: https://soaqa.ssspr.com/api/document/upload
Result: ✅ Success
```

---

## Why This Matters

### IIS URL Rewrite Rules Are Common

Many organizations use IIS rewrites for:
- **Virtual directory routing** - `/app1/` → `/`
- **SSL enforcement** - `http://` → `https://`
- **Load balancing** - Route to backend servers
- **API versioning** - `/v1/api/` → `/api/v1/`
- **Legacy compatibility** - Old URLs → New URLs

### Different Clients React Differently

| Client | Auto-Redirect | User-Agent | Behavior |
|--------|--------------|------------|----------|
| curl | ❌ No (default) | `curl/x.x` | Stops at redirect |
| curl -L | ✅ Yes | `curl/x.x` | Follows redirect |
| HttpClient | ✅ Yes | `System.Net.Http/x.x` | Follows redirect |
| Browser | ✅ Yes | `Mozilla/...` | Follows redirect |
| Postman | ✅ Yes | `PostmanRuntime/x.x` | Follows redirect |

---

## Best Practices

### 1. Always Test with HttpClient

Don't just test with curl! Test with:
- ✅ HttpClient (your actual app)
- ✅ Postman (similar behavior)
- ✅ Browser F12 DevTools

### 2. Check Response.RequestMessage.RequestUri

Always log the final URI after redirects:
```csharp
var response = await _httpClient.SendAsync(request);
var finalUri = response.RequestMessage?.RequestUri;
Debug.WriteLine($"Final URI: {finalUri}");
```

If it's different from what you sent, there's a rewrite!

### 3. Understand Server Configuration

Ask your server team:
- Are there URL rewrite rules?
- Which paths are rewritten?
- Are they conditional (User-Agent, headers, etc.)?
- What's the final expected URL?

### 4. Use Absolute Paths

When dealing with rewrites, use absolute paths:
```csharp
// GOOD:
BaseAddress = "https://soaqa.ssspr.com"
Endpoint = "/Triple-S-AEP-API/api/document/verify-user"

// BAD:
BaseAddress = "https://soaqa.ssspr.com/Triple-S-AEP-API"
Endpoint = "/api/document/verify-user"
```

This makes the rewrite behavior predictable.

---

## Summary

**The issue:** Server URL rewrite strips `/Triple-S-AEP-API` from HttpClient requests but not curl.

**The fix:** Changed base URL to domain only, moved virtual directory to endpoint path.

**Why it works:** App now sends full path, server rewrites it, final URL matches server expectations.

**Lesson learned:** curl and HttpClient are NOT treated the same by IIS URL rewrites! Always test with your actual client.
