# QA Endpoint Testing - Quick Start

## Issue
Getting **404 errors** when connecting to QA DMS endpoint: `https://soaqa.ssspr.com/Triple-S-AEP-API`

## Quick Fix Steps

### 1. Run Connectivity Diagnostic Test

Add this to `DashboardPage.xaml.cs` or a test method:

```csharp
// Test connectivity to QA endpoint
var dmsService = new DMSService();
var (connected, message, details) = await dmsService.TestConnectivityAsync();

if (connected)
{
    System.Diagnostics.Debug.WriteLine("✅ QA Endpoint is reachable!");
}
else
{
    System.Diagnostics.Debug.WriteLine($"❌ QA Endpoint error: {message}");
    System.Diagnostics.Debug.WriteLine($"Details: {details}");
}
```

### 2. Check Debug Output Window

**Visual Studio → Debug → Windows → Output**

Look for:
- ✅ `CONNECTIVITY TEST PASSED` = Endpoint is working
- ❌ `CONNECTIVITY TEST FAILED` = Something is wrong

### 3. Common Issues & Fixes

#### Issue: 404 Not Found

**Likely Cause:** Wrong API path

**Questions to ask Triple-S team:**
1. What's the correct API endpoint path?
2. Is it `/Triple-S-AEP-API` or `/api/Triple-S-AEP-API`?
3. Is it `/Triple-S-AEP-API/upload`?
4. Does it expect a specific resource path like `/document/upload`?

**Fix:** Update `DMSService.cs` (line 17):
```csharp
// Change FROM:
private const string DMS_UPLOAD_ENDPOINT = "/Triple-S-AEP-API";

// Change TO (ask team for correct path):
private const string DMS_UPLOAD_ENDPOINT = "/api/Triple-S-AEP-API";  // Example
```

#### Issue: SSL Certificate Error

**Error Message:** "The SSL connection could not be established"

**Likely Cause:** QA certificate not trusted

**Fix:** The app already handles this in `DMSService.cs` (lines 32-46)
- For `localhost`: Self-signed certs allowed ✅
- For `soaqa.ssspr.com`: Full cert validation required ✅

**Solution:**
1. Verify certificate is valid: `https://soaqa.ssspr.com`
2. If using self-signed cert, request proper certificate from IT
3. If still failing, check with your IT team about proxy/firewall

#### Issue: 401 Unauthorized

**Error Message:** "401 Unauthorized"

**Likely Cause:** Wrong authentication headers

**Current Headers Sent:**
```
Hyland-Username: {agent NPN}
Hyland-Password: {agent password}
```

**Questions for Triple-S team:**
1. Is header-based auth correct?
2. Should it be `X-Hyland-Username` instead?
3. Should it use `Authorization: Bearer` token?
4. Should it use Basic Auth: `Authorization: Basic`?

**Fix:** If different auth needed, modify `DMSService.cs` (lines 56-58)

---

## Diagnostic Output Explained

When you run `TestConnectivityAsync()`, look for this section:

### Success Response:
```
╔════════════════════════════════════════╗
║  DMS CONNECTIVITY DIAGNOSTIC TEST      ║
╚════════════════════════════════════════╝

📍 Testing URL: https://soaqa.ssspr.com/Triple-S-AEP-API
📋 Base URL: https://soaqa.ssspr.com
📋 Endpoint: /Triple-S-AEP-API
📋 Method: POST

📬 Headers:
   Hyland-Username: mahmer
   Hyland-Password: password
   Accept: application/json

✓ Response received!
   Status Code: 200 (OK)
   Status Description: OK
   Is Success: True

✅ CONNECTIVITY TEST PASSED
   Endpoint is reachable and accepting requests
```

### Failure Response:
```
✓ Response received!
   Status Code: 404 (NotFound)
   Status Description: Not Found
   Is Success: False

⚠️ CONNECTIVITY TEST FAILED
   Endpoint returned 404: Not Found

💡 Possible causes:
   • Incorrect endpoint path
   • API not running on QA server
   • Authentication headers incorrect
   • Firewall blocking request
   • SSL certificate issue
```

---

## Network Inspection

### Option A: Fiddler (Recommended)

1. **Download:** https://www.telerik.com/download/fiddler
2. **Start Fiddler before running your app**
3. **Look for requests to `soaqa.ssspr.com`**
4. **Check:**
   - Request headers (see what's being sent)
   - Request body (see the JSON)
   - Response status (404, 401, 500, etc.)
   - Response body (error message from server)

### Option B: Browser DevTools

If the app uses WebView:
1. Open app in WebView
2. Press **F12**
3. Go to **Network** tab
4. Make the request
5. Click on the request to see headers/response

### Option C: Debug Output Window

**Visual Studio → Debug → Windows → Output**

All `Debug.WriteLine()` output appears here:
- Shows exact URL being called
- Shows headers being sent
- Shows response status and body
- Shows certificate errors

---

## Testing with Postman/curl

### Using Postman:

1. **New → Request**
2. **Method:** POST
3. **URL:** `https://soaqa.ssspr.com/Triple-S-AEP-API`
4. **Headers:**
   ```
   Hyland-Username: mahmer
   Hyland-Password: password
   Content-Type: application/json
   ```
5. **Body (JSON):**
   ```json
   {
     "documentTypeId": 123,
     "fileTypeId": 16,
     "base64Document": "JVBERi0xLjQ=",
     "keywords": [
       {
         "keywordTypeId": 1,
         "value": "TEST"
       }
     ]
   }
   ```
6. **Send**
7. **Check status code and response**

### Using curl:

```bash
curl -X POST https://soaqa.ssspr.com/Triple-S-AEP-API \
  -H "Content-Type: application/json" \
  -H "Hyland-Username: mahmer" \
  -H "Hyland-Password: password" \
  -d '{
    "documentTypeId": 123,
    "fileTypeId": 16,
    "base64Document": "JVBERi0xLjQ=",
    "keywords": [{"keywordTypeId": 1, "value": "TEST"}]
  }' \
  -v
```

The `-v` flag shows headers and response.

---

## Information to Gather

Before contacting Triple-S support, gather:

1. **From TestConnectivityAsync() output:**
   ```
   Status Code: ___
   Response: ___
   ```

2. **From Fiddler/DevTools:**
   ```
   Request URL: https://soaqa.ssspr.com/Triple-S-AEP-API
   Request Headers: [paste here]
   Response Status: ___
   Response Body: [paste first 500 chars]
   ```

3. **Other info:**
   ```
   Agent NPN: ___
   Is agent logged in: ___
   Debug output from Test: [paste entire output]
   ```

---

## Checklist

- [ ] Run TestConnectivityAsync() diagnostic
- [ ] Check Debug Output window for result
- [ ] If 404: Ask team for correct API path
- [ ] If 401: Ask team for correct auth headers
- [ ] If SSL error: Check certificate validity
- [ ] Test with Postman to isolate issue
- [ ] Share diagnostic results with team
- [ ] Update endpoint config once confirmed

---

## Code Locations

**Testing Method:** `Triple-S-Maui-AEP\Services\DMSService.cs` (line 373)
**Configuration:** `Triple-S-Maui-AEP\Configuration\AppSettings.cs` (line 11)
**DMS Service:** `Triple-S-Maui-AEP\Services\DMSService.cs` (lines 15-17)

---

**Need help?** Run the diagnostic test and share the output!
