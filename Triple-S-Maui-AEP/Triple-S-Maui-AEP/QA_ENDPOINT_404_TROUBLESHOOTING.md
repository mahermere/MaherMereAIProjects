# QA Endpoint 404 Troubleshooting Guide

## Issue
Getting **404 (Not Found)** errors when trying to connect to: `https://soaqa.ssspr.com/Triple-S-AEP-API`

## Root Causes & Solutions

### 1. ✅ Certificate Validation (LIKELY CAUSE)

The QA endpoint may require proper certificate validation. The current code only allows self-signed certs on `localhost`.

**Solution: Enable certificate validation for QA**

Edit `DMSService.cs` constructor (around line 30-46):

```csharp
var handler = new HttpClientHandler();

// Certificate validation
#if DEBUG
handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
{
    // Localhost: allow self-signed
    if (message.RequestUri?.Host == "localhost" || message.RequestUri?.Host == "127.0.0.1")
    {
        System.Diagnostics.Debug.WriteLine("⚠️ Accepting self-signed certificate for localhost development");
        return true;
    }
    
    // QA Environment: log certificate info for debugging
    if (message.RequestUri?.Host.Contains("soaqa.ssspr.com") == true)
    {
        System.Diagnostics.Debug.WriteLine($"📋 QA Certificate Validation:");
        System.Diagnostics.Debug.WriteLine($"   Host: {message.RequestUri.Host}");
        System.Diagnostics.Debug.WriteLine($"   Certificate: {cert?.Subject}");
        System.Diagnostics.Debug.WriteLine($"   Valid: {cert?.Verify()}");
        System.Diagnostics.Debug.WriteLine($"   Errors: {errors}");
        
        // For QA: validate properly
        return errors == System.Net.Security.SslPolicyErrors.None;
    }
    
    // Default: validate certificates
    return errors == System.Net.Security.SslPolicyErrors.None;
};
#endif
```

---

### 2. ✅ Incorrect API Endpoint Path

The 404 could mean the API path is wrong. Verify the actual endpoint structure.

**Diagnosis Steps:**

1. **Check what the server expects:**
   - Browse to: `https://soaqa.ssspr.com/` (you should get a response)
   - Check if there's a base API path like `/api/` or `/v1/`

2. **Current Configuration:**
   ```
   Base URL: https://soaqa.ssspr.com
   Endpoint: /Triple-S-AEP-API
   Full URL: https://soaqa.ssspr.com/Triple-S-AEP-API
   ```

3. **Possible Correct Paths:**
   ```
   https://soaqa.ssspr.com/api/Triple-S-AEP-API
   https://soaqa.ssspr.com/api/document/upload
   https://soaqa.ssspr.com/Triple-S-AEP-API/upload
   https://soaqa.ssspr.com/Triple-S-AEP-API/document/upload
   ```

**Ask your Triple-S team for the exact endpoint:**
- What's the base URL for QA?
- What's the correct upload endpoint?
- Do you need `/api/` in the path?

---

### 3. ✅ Check HTTP Request Details

Add detailed logging to see exactly what's being sent:

```csharp
// In DMSService.cs - Add before making the request
System.Diagnostics.Debug.WriteLine($"📡 HTTP Request Details:");
System.Diagnostics.Debug.WriteLine($"   Method: {uploadRequest.GetType().Name}");
System.Diagnostics.Debug.WriteLine($"   URL: {_httpClient.BaseAddress}{DMS_UPLOAD_ENDPOINT}");
System.Diagnostics.Debug.WriteLine($"   Headers: {string.Join(", ", _httpClient.DefaultRequestHeaders.Select(h => h.Key))}");
System.Diagnostics.Debug.WriteLine($"   Content-Type: application/json");
```

---

### 4. ✅ Authentication Headers

The 404 might be because auth headers are missing or wrong.

**Current Headers Being Sent:**
```csharp
Hyland-Username: {agent NPN}
Hyland-Password: {agent password}
Content-Type: application/json
```

**Possible Issues:**
- Headers should be `Authorization: Bearer {token}` instead of `Hyland-Username/Password`
- Headers should use `X-` prefix: `X-Hyland-Username`
- API expects Basic Auth: `Authorization: Basic {base64}`

**Check with your team:**
- How does the QA API authenticate?
- Is it header-based, OAuth, or something else?

---

## Quick Diagnostic Test

Add this method to `DMSService.cs` to test connectivity:

```csharp
public async Task<bool> TestConnectivityAsync()
{
    try
    {
        System.Diagnostics.Debug.WriteLine("🧪 Testing QA endpoint connectivity...");
        
        var testUrl = $"{DMS_BASE_URL}{DMS_UPLOAD_ENDPOINT}";
        System.Diagnostics.Debug.WriteLine($"   URL: {testUrl}");
        
        var response = await _httpClient.GetAsync(testUrl);
        
        System.Diagnostics.Debug.WriteLine($"   Status: {response.StatusCode}");
        System.Diagnostics.Debug.WriteLine($"   Success: {response.IsSuccessStatusCode}");
        
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"   Response: {content}");
        }
        
        return response.IsSuccessStatusCode;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"❌ Connectivity test failed: {ex.Message}");
        return false;
    }
}
```

**Usage in DashboardViewModel:**
```csharp
private async Task LoadDashboardData()
{
    try
    {
        IsLoading = true;
        
        // Test connectivity first
        var dmsService = new DMSService();
        var isConnected = await dmsService.TestConnectivityAsync();
        
        if (!isConnected)
        {
            System.Diagnostics.Debug.WriteLine("⚠️ Cannot reach QA endpoint");
            // Handle connectivity issue
        }
        
        // ... rest of method
    }
    // ...
}
```

---

## Network Inspection Tools

To see exactly what's being sent/received:

### Option 1: Browser DevTools (if using WebView)
- F12 → Network tab
- Look for the failing request
- Check Status Code, Headers, Response

### Option 2: Fiddler (recommended)
- Download: https://www.telerik.com/download/fiddler
- All HTTP(S) requests visible
- Can see exact request/response

### Option 3: Debug Output Window
- Visual Studio → Debug → Windows → Output
- All `Debug.WriteLine()` messages appear here
- Set breakpoints to pause execution

---

## Solutions Checklist

- [ ] Verify certificate is valid for `soaqa.ssspr.com`
- [ ] Confirm correct API endpoint path with Triple-S team
- [ ] Check if authentication headers are correct format
- [ ] Test connectivity with `TestConnectivityAsync()`
- [ ] Enable Fiddler/DevTools to inspect actual HTTP traffic
- [ ] Check server-side logs on QA for what's being received
- [ ] Verify firewall/network allows outbound HTTPS

---

## Common 404 Causes

| Cause | Solution |
|-------|----------|
| Wrong endpoint path | Verify exact path with API docs/team |
| Missing base path like `/api/` | Add to endpoint configuration |
| Trailing slash mismatch | Ensure consistent slash usage |
| Wrong HTTP method (POST vs GET) | Check DMS API expects POST |
| Invalid auth headers | Use correct authentication format |
| Server can't find route | Check API routing configuration |
| Firewall blocking request | Check network/proxy settings |

---

## Next Steps

1. **Get clarification from Triple-S team:**
   - Exact QA endpoint URL
   - Expected request format
   - Authentication method
   - Any API documentation

2. **Test with Postman/curl:**
   ```bash
   curl -X POST https://soaqa.ssspr.com/Triple-S-AEP-API \
     -H "Content-Type: application/json" \
     -H "Hyland-Username: mahmer" \
     -H "Hyland-Password: password" \
     -d '{"documentTypeId": 123}'
   ```

3. **Enable detailed logging** and run a test upload

4. **Share results** with Triple-S team:
   - Exact URL being called
   - HTTP status code
   - Response headers/body
   - Certificate info

---

**Questions to ask your team:**
- Is the QA endpoint `soaqa.ssspr.com` correct?
- What's the exact API path (with `/api/` or not)?
- How should the request be authenticated?
- Any IP whitelisting needed?
- Are there any firewall rules to configure?

