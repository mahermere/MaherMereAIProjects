# Hyland Authentication Implementation Guide

## Quick Start

### Step 1: Update AppSettings.cs

Replace the API key configuration with Hyland credentials:

```csharp
// Configuration/AppSettings.cs

namespace TripleS.SOA.AEP.UI.Configuration
{
    public static class AppSettings
    {
        // DMS Endpoint
        public static string DMSEndpoint { get; set; } = 
            "https://dms.example.com/api/document/upload";

        // Hyland Authentication (NEW)
        public static string HylandUsername { get; set; } = 
            Environment.GetEnvironmentVariable("HYLAND_USERNAME") ?? "";
        
        public static string HylandPassword { get; set; } = 
            Environment.GetEnvironmentVariable("HYLAND_PASSWORD") ?? "";

        // Other settings...
        public static int DMSUploadTimeoutMinutes { get; set; } = 5;
    }
}
```

### Step 2: Set Environment Variables

#### Windows (Command Prompt)
```cmd
setx HYLAND_USERNAME "your-username"
setx HYLAND_PASSWORD "your-password"
```

#### Windows (PowerShell)
```powershell
[Environment]::SetEnvironmentVariable("HYLAND_USERNAME", "your-username", "User")
[Environment]::SetEnvironmentVariable("HYLAND_PASSWORD", "your-password", "User")
```

#### Linux/macOS
```bash
export HYLAND_USERNAME="your-username"
export HYLAND_PASSWORD="your-password"
```

#### Visual Studio (Local Testing)
1. Right-click project → Properties
2. Debug tab
3. Environment variables section
4. Add:
   - HYLAND_USERNAME = test-user
   - HYLAND_PASSWORD = test-pass

### Step 3: Verify Authentication

The DMSService will automatically add headers:

```csharp
// These headers are added automatically by DMSService constructor
Hyland-Username: your-username
Hyland-Password: your-password
```

### Step 4: Test Upload

```csharp
var dmsService = new DMSService();

var response = await dmsService.UploadEnrollmentAsync(
    filePath: enrollmentPdfPath,
    enrollmentNumber: "ENR-12345678-20250115123456-Smith",
    firstName: "John",
    lastName: "Smith",
    dateOfBirth: "01/15/1950",
    gender: "Male",
    ssn: "123-45-6789",
    medicare: "1234567890A"
    // ... other fields
);

if (response.Success)
{
    Debug.WriteLine($"Upload successful: {response.DocumentId}");
}
else
{
    Debug.WriteLine($"Upload failed: {response.Message}");
}
```

---

## HTTP Request Example

Here's what the actual HTTP request looks like:

```http
POST /api/document/upload HTTP/1.1
Host: dms.example.com
Content-Type: application/json
Hyland-Username: your-username
Hyland-Password: your-password

{
  "DocumentTypeId": 834,
  "FileTypeId": 16,
  "Base64Document": "JVBERi0xLjQK...",
  "Keywords": [
    { "KeywordTypeId": 1254, "Value": "ENR-12345678..." },
    { "KeywordTypeId": 1049, "Value": "John" },
    { "KeywordTypeId": 1051, "Value": "Smith" }
    // ... 29 more keywords
  ]
}
```

---

## Environment-Specific Setup

### Development

```csharp
// AppSettings.cs - Development
public static string HylandUsername { get; set; } = 
    Environment.GetEnvironmentVariable("HYLAND_USERNAME") ?? "dev-user";

public static string HylandPassword { get; set; } = 
    Environment.GetEnvironmentVariable("HYLAND_PASSWORD") ?? "dev-password";
```

### QA

Set environment variables in QA environment:
```cmd
HYLAND_USERNAME=qa-user
HYLAND_PASSWORD=qa-password
```

### Production

Use Azure Key Vault or secure configuration:
```csharp
var keyVaultUrl = "https://keyvault.azure.net/";
var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

var username = await client.GetSecretAsync("HylandUsername");
var password = await client.GetSecretAsync("HylandPassword");

AppSettings.HylandUsername = username.Value.Value;
AppSettings.HylandPassword = password.Value.Value;
```

---

## Debugging Authentication Issues

### Check if Headers are Being Sent

Add this logging to DMSService:

```csharp
public DMSService()
{
    _httpClient = new HttpClient
    {
        Timeout = TimeSpan.FromMinutes(AppSettings.DMSUploadTimeoutMinutes)
    };
    
    // Add Hyland authentication headers
    if (!string.IsNullOrWhiteSpace(AppSettings.HylandUsername) && 
        !string.IsNullOrWhiteSpace(AppSettings.HylandPassword))
    {
        _httpClient.DefaultRequestHeaders.Add("Hyland-Username", AppSettings.HylandUsername);
        _httpClient.DefaultRequestHeaders.Add("Hyland-Password", AppSettings.HylandPassword);
        
        // DEBUG: Verify headers
        Debug.WriteLine($"[DMSService] Hyland authentication configured");
        Debug.WriteLine($"[DMSService] Username: {AppSettings.HylandUsername}");
        // Don't log password!
    }
    else
    {
        Debug.WriteLine("[DMSService] WARNING: Hyland credentials not configured!");
    }
}
```

### Check Response Headers

```csharp
System.Diagnostics.Debug.WriteLine("Response Headers:");
foreach (var header in response.Headers)
{
    Debug.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
}
```

---

## Security Checklist

- ✅ Using environment variables (not hardcoded)
- ✅ HTTPS endpoint only
- ✅ No credentials in Git repository
- ✅ No credentials in logs
- ✅ Separate credentials for Dev/QA/Prod
- ✅ Regular credential rotation planned
- ✅ Minimal permission scope
- ✅ Service account (not personal account)

---

## Common Errors & Solutions

### Error: "401 Unauthorized"
```
Cause: Invalid username or password
Solution: 
  1. Verify credentials in AppSettings
  2. Check environment variable names are correct
  3. Ensure no extra spaces in values
  4. Check password doesn't contain special characters that need escaping
```

### Error: "Headers already sent"
```
Cause: Adding headers after sending request
Solution:
  - Ensure headers are added in constructor
  - Don't modify headers after HttpClient is used
  - Create new HttpClient instance if needed
```

### Error: "Connection timeout"
```
Cause: Endpoint unreachable
Solution:
  1. Verify DMS endpoint URL is correct
  2. Check network connectivity
  3. Verify firewall allows HTTPS
  4. Check DMSUploadTimeoutMinutes setting
```

---

## Migration from API Key to Hyland Auth

### Before (Deprecated)
```csharp
public static string DMSApiKey { get; set; } = "your-api-key";
```

### After (Current)
```csharp
public static string HylandUsername { get; set; } = 
    Environment.GetEnvironmentVariable("HYLAND_USERNAME") ?? "";

public static string HylandPassword { get; set; } = 
    Environment.GetEnvironmentVariable("HYLAND_PASSWORD") ?? "";
```

### Cleanup
1. Remove `DMSApiKey` from AppSettings.cs ✓
2. Remove API key header setup ✓
3. Add Hyland header setup ✓
4. Update environment variables
5. Test with new credentials
6. Remove old API keys from systems

---

## Testing Checklist

- [ ] Development environment set up with test credentials
- [ ] QA environment set up with QA credentials
- [ ] Production environment secured with Key Vault
- [ ] Test upload succeeds with correct credentials
- [ ] Test upload fails gracefully with wrong credentials
- [ ] Error messages are helpful (no credential exposure)
- [ ] Headers logged correctly in debugging
- [ ] No credentials in application logs
- [ ] No credentials in error messages

---

**Version**: 2.0
**Authentication Method**: Hyland-Username / Hyland-Password Headers
**Last Updated**: January 15, 2025
**Status**: Current ✓
