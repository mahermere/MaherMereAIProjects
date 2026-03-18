# Login 404 Debug - Curl vs App Comparison

## Working Curl Command

Your curl command works perfectly:

```bash
curl -X GET \
  --header 'Accept: application/json' \
  --header 'Hyland-Username: mahmer' \
  --header 'Hyland-Password: password' \
  'https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/verify-user'
```

**Status:** ✅ 200 OK (or appropriate success response)

---

## App Configuration

Current settings in `OnBaseAuthenticationService.cs`:

```csharp
private const string ONBASE_BASE_URL = "https://soaqa.ssspr.com/Triple-S-AEP-API";
private const string VERIFY_USER_ENDPOINT = "/api/document/verify-user";
```

**Combined URL:** `https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/verify-user`  
**Expected:** ✅ Same as curl (CORRECT!)

---

## Why Curl Works But App Doesn't

If the URLs are identical and curl works, the issue is likely one of these:

### 1. ✅ HttpClientHandler Configuration

Check the debug output for:
```
🔧 Configuring HttpClientHandler:
   AllowAutoRedirect: True/False
   UseProxy: True/False
   Proxy: [value]
   AutomaticDecompression: [value]
```

**Curl equivalent:** No proxies, no auto-redirect processing  
**Possible issue:** If `UseProxy: True`, the request might be going through a proxy that blocks it

### 2. ✅ Certificate Validation

Check the debug output for:
```
🔐 Certificate Validation Callback
   Host: soaqa.ssspr.com
   Certificate Subject: [cert info]
   SSL Errors: None
   Result: ✅ VALID
```

**Curl equivalent:** Uses system certificate store  
**Possible issue:** If `SSL Errors: X`, the request is being blocked

### 3. ✅ Request Headers

Check the debug output for:
```
📬 Final Request Headers Before Send:
   Hyland-Username: mahmer
   Hyland-Password: [REDACTED]
   Accept: application/json
```

**Curl equivalent:**
```
--header 'Accept: application/json'
--header 'Hyland-Username: mahmer'
--header 'Hyland-Password: password'
```

**Possible issues:**
- Missing headers
- Extra headers the server doesn't like
- `Accept` header has quality values (`application/json; q=1.0`)

### 4. ✅ URL Construction

Check the debug output for:
```
📍 FULL REQUEST URL: https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/verify-user
📍 Expected (from curl): https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/verify-user
📍 Match: True
```

**If `Match: False`** - The URLs don't match! This is the problem!

### 5. ✅ Request Redirects

Check the debug output for:
```
   Response.RequestMessage.RequestUri: [actual final URI]
```

**Possible issue:** If this is different from the requested URL, the server might be redirecting and the app is following the redirect to a 404 endpoint.

### 6. ✅ HttpClient State

Check the debug output for:
```
📬 HttpClient.DefaultRequestHeaders (merged with request):
   [any headers here]
```

**Curl equivalent:** Only sends explicitly specified headers  
**Possible issue:** Extra default headers that curl doesn't send

---

## Quick Diagnostic Steps

### Step 1: Run Login and Check Debug Output

Look specifically for these sections:

1. **URL Match Check:**
   ```
   📍 Match: True/False  ← This should be TRUE
   ```

2. **Certificate Validation:**
   ```
   🔐 Certificate Validation Callback
      Result: ✅ VALID  ← This should be VALID
   ```

3. **Request Headers:**
   ```
   📬 Final Request Headers Before Send:
      Hyland-Username: mahmer
      Hyland-Password: [REDACTED]
      Accept: application/json
   ```

4. **Response Status:**
   ```
   📥 RESPONSE RECEIVED
      Status Code: 200 (OK) ← Should NOT be 404
   ```

### Step 2: If You See 404

Check these debug lines:

```
❌ Verification failed!
   Error: 404 Not Found

💡 404 Diagnosis:
   Full URL being called: [URL]
   Expected URL: https://soaqa.ssspr.com/Triple-S-AEP-API/api/document/verify-user
   URLs match: [True/False]  ← THIS IS KEY!
```

---

## Most Likely Causes

If curl works but app doesn't:

### Cause 1: URL Mismatch (Most Likely)

**Debug to look for:**
```
URLs match: False
```

**Solution:** Update the endpoint or base URL

### Cause 2: Proxy Issues

**Debug to look for:**
```
UseProxy: True
Proxy: [some proxy URL]
```

**Solution:** Either:
- Bypass the proxy for soaqa.ssspr.com
- Configure the proxy credentials
- Use `handler.UseProxy = false;` (not recommended)

### Cause 3: Certificate Error

**Debug to look for:**
```
🔐 Certificate Validation Callback
   SSL Errors: RemoteCertificateChainErrors
   Result: ❌ INVALID
```

**Solution:**
- Install the certificate on your machine
- Ask IT for the correct certificate
- Verify the certificate is valid

### Cause 4: Extra Headers

**Debug to look for:**
```
📬 HttpClient.DefaultRequestHeaders (merged with request):
   User-Agent: [something]
   Other headers that curl doesn't send
```

**Solution:** Remove extra headers or configure HttpClient properly

### Cause 5: Redirects

**Debug to look for:**
```
Response.RequestMessage.RequestUri: [different from requested URL]
```

**Solution:** Check if server is redirecting and follow the redirect URL

---

## Step-by-Step Debug Guide

### 1. Try to Login

Enter credentials and click login

### 2. Check Debug Output

Copy the entire section from:
```
╔════════════════════════════════════════════════════════════╗
║     ONBASE USER VERIFICATION START                         ║
```

to:

```
╔════════════════════════════════════════════════════════════╗
║     ONBASE USER VERIFICATION FAILED ✗                       ║
╚════════════════════════════════════════════════════════════╝
```

### 3. Compare with Curl

- ✅ **URL**: Should match exactly  
- ✅ **Headers**: Should have Hyland-Username, Hyland-Password, Accept  
- ✅ **No 404 if URL and headers are correct**: Means it's a handler/cert issue

### 4. Share Results

If you're still getting 404, share:

1. **The full debug output** (from above)
2. **These specific lines:**
   - `Full URL being called:`
   - `URLs match: True/False`
   - `SSL Errors:`
   - `Status Code:`

---

## Hypothesis

Given that **curl works**, my hypothesis is:

1. **Most Likely:** A **proxy setting** is interfering with the request (UseProxy: True)
2. **Second:** A **certificate validation error** that curl bypasses but the app respects
3. **Third:** **Extra headers** being sent by HttpClient that the server rejects

Once you run the app and check the debug output, we'll know exactly which it is!

---

## What to Do Next

1. **Rebuild and run the app**
2. **Try to login with any credentials**
3. **Check Debug Output window** (Debug → Windows → Output)
4. **Look for these key lines:**
   - `📍 Full URL being called: `
   - `📍 Match: `
   - `🔐 SSL Errors: `
   - `📥 Status Code: `
5. **Share the output with me**

The enhanced logging will tell us **exactly** what's different between curl and the app! 🔍
