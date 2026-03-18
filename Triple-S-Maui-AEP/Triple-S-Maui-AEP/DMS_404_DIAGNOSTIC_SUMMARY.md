# DMS 404 Error - Diagnostic Tools Summary

## Problem
Getting **404 (Not Found)** errors when connecting to QA DMS endpoint: `https://soaqa.ssspr.com/Triple-S-AEP-API`

## Solution Provided

### ✅ 1. New Diagnostic Method Added

**File:** `Triple-S-Maui-AEP/Services/DMSService.cs`

**Method:** `TestConnectivityAsync()`

This method:
- ✅ Tests connectivity to QA endpoint
- ✅ Logs all request headers
- ✅ Sends minimal test request
- ✅ Logs full response (status, headers, body)
- ✅ Identifies the specific error (404, 401, SSL, etc.)
- ✅ Provides diagnostic suggestions

### ✅ 2. Comprehensive Documentation

**File 1: QA_ENDPOINT_QUICK_FIX.md**
- Step-by-step testing instructions
- Common issues & quick fixes
- Postman/curl testing examples
- Fiddler network inspection guide
- Checklist for troubleshooting

**File 2: QA_ENDPOINT_404_TROUBLESHOOTING.md**
- Deep dive technical guide
- SSL certificate handling details
- HTTP header validation
- Authentication troubleshooting
- Root cause analysis for each error type

---

## How to Use the Diagnostic Tool

### Step 1: Run the Test

Add this code to test the endpoint:

```csharp
// In DashboardViewModel.cs or DashboardPage.xaml.cs
var dmsService = new DMSService();
var (connected, message, details) = await dmsService.TestConnectivityAsync();

if (connected)
{
    System.Diagnostics.Debug.WriteLine("✅ QA Endpoint is working!");
}
else
{
    System.Diagnostics.Debug.WriteLine($"❌ Error: {message}");
}
```

### Step 2: Check Output

Open **Debug Output Window:**
- Visual Studio → Debug → Windows → Output

### Step 3: Analyze Results

Look for these sections:

**If Successful:**
```
✅ CONNECTIVITY TEST PASSED
   Endpoint is reachable and accepting requests
```

**If 404 Error:**
```
⚠️ CONNECTIVITY TEST FAILED
   Endpoint returned 404: Not Found
💡 Possible causes:
   • Incorrect endpoint path
   • API not running on QA server
```

**If SSL Error:**
```
❌ CONNECTIVITY TEST FAILED
   HTTP Request Error: The SSL connection could not be established
💡 Possible causes:
   • Network unreachable
   • SSL/TLS certificate error
```

---

## What Information You'll Get

The diagnostic output includes:

```
📍 Testing URL: https://soaqa.ssspr.com/Triple-S-AEP-API
📋 Base URL: https://soaqa.ssspr.com
📋 Endpoint: /Triple-S-AEP-API
📋 Method: POST

📬 Headers Being Sent:
   Hyland-Username: [agent-npn]
   Hyland-Password: [agent-password]
   Accept: application/json

✓ Response received!
   Status Code: 404 (NotFound)
   Status Description: Not Found
   Response Body: [full error message]

💡 Suggested Causes & Fixes
```

---

## Common Issues & Quick Fixes

### Issue 1: 404 Not Found
- **Cause:** Wrong API endpoint path
- **Fix:** Ask Triple-S team for exact path
- **Example paths to try:**
  - `/Triple-S-AEP-API`
  - `/api/Triple-S-AEP-API`
  - `/Triple-S-AEP-API/upload`
  - `/api/document/upload`

### Issue 2: 401 Unauthorized
- **Cause:** Wrong authentication headers
- **Fix:** Ask Triple-S team for correct auth format
- **Current headers sent:**
  - `Hyland-Username`
  - `Hyland-Password`

### Issue 3: SSL/TLS Certificate Error
- **Cause:** Certificate validation failure
- **Fix:** 
  - Verify certificate is valid
  - Ask IT if using self-signed cert
  - Check if firewall/proxy blocking

---

## Files Modified & Created

### Modified:
- `Triple-S-Maui-AEP/Services/DMSService.cs`
  - Added `TestConnectivityAsync()` method (~150 lines)
  - No breaking changes to existing code

### Created:
- `Triple-S-Maui-AEP/QA_ENDPOINT_QUICK_FIX.md` - Quick testing guide
- `Triple-S-Maui-AEP/QA_ENDPOINT_404_TROUBLESHOOTING.md` - Deep troubleshooting

---

## Next Steps

1. **Run the diagnostic test**
   ```csharp
   await new DMSService().TestConnectivityAsync();
   ```

2. **Check the output** in Visual Studio Debug window

3. **Determine the error type:**
   - Is it 404? → Ask for correct API path
   - Is it 401? → Ask for correct auth
   - Is it SSL? → Check certificate

4. **Update configuration** once you have the correct details:
   - `DMSService.cs` line 17: Update endpoint
   - `AppSettings.cs` line 11: Update endpoint
   - Header auth if needed

5. **Re-test** with the diagnostic tool

---

## Support Information to Share

When contacting Triple-S support, provide:

```
1. Diagnostic output from TestConnectivityAsync()
2. Status code received (404, 401, 500, etc.)
3. Response body/error message
4. Network inspection (Fiddler results)
5. Certificate details (if SSL error)
```

---

## Technical Details

### Diagnostic Method Implementation

Location: `DMSService.cs` (line 373)

Features:
- ✅ Full HTTP request logging
- ✅ Header inspection
- ✅ Response body capture
- ✅ Certificate validation details
- ✅ Error categorization
- ✅ Suggested solutions

### Error Handling

The method catches:
- `HttpRequestException` - Network/SSL errors
- `TaskCanceledException` - Timeout errors
- Generic `Exception` - Unexpected errors

Each error type gets specific logging and suggestions.

---

## Integration Points

### Where to Use TestConnectivityAsync()

**Option 1: Debug/Testing**
```csharp
// Temporary testing
var (connected, msg, details) = await new DMSService().TestConnectivityAsync();
```

**Option 2: Startup Validation**
```csharp
// In App.xaml.cs or MauiProgram.cs
var dmsService = new DMSService();
var (connected, msg, _) = await dmsService.TestConnectivityAsync();
if (!connected) { /* log warning */ }
```

**Option 3: Dashboard Health Check**
```csharp
// In DashboardPage.xaml.cs
var dmsService = new DMSService();
var (connected, msg, _) = await dmsService.TestConnectivityAsync();
ConnectivityStatusLabel.Text = connected ? "✅ Online" : "❌ Offline";
```

---

## Commit Information

**Commit Hash:** c9c0a0c

**Message:** "Add DMS endpoint diagnostic tools and 404 troubleshooting guides"

**Files Changed:**
- `Services/DMSService.cs` (+150 lines)
- `QA_ENDPOINT_QUICK_FIX.md` (new)
- `QA_ENDPOINT_404_TROUBLESHOOTING.md` (new)

---

## Summary

You now have:
✅ Diagnostic method to identify the exact 404 cause
✅ Comprehensive troubleshooting guides
✅ Examples for Postman/curl testing
✅ Network inspection instructions
✅ Documentation of common issues & fixes

**To diagnose your 404 issue:**
1. Call `TestConnectivityAsync()`
2. Check Debug output
3. Share results with Triple-S team
4. They'll provide correct endpoint/auth details
5. Update config and test again
