# DMS Hyland Authentication Update

## Authentication Method Change

The DMS API now requires **Hyland authentication headers** instead of API key authentication.

### Previous Method (Deprecated)
```csharp
// NO LONGER USED
X-API-Key: [api-key-value]
```

### New Method (Current)
```
Hyland-Username: [username]
Hyland-Password: [password]
```

---

## Configuration

### Update AppSettings.cs

```csharp
// Hyland Authentication
public static string HylandUsername { get; set; } = "your-hyland-username";
public static string HylandPassword { get; set; } = "your-hyland-password";
```

### Environment-Specific Configuration

#### Development
```csharp
public static string HylandUsername { get; set; } = "dev-user";
public static string HylandPassword { get; set; } = "dev-password";
```

#### QA
```csharp
public static string HylandUsername { get; set; } = "qa-user";
public static string HylandPassword { get; set; } = "qa-password";
```

#### Production
```csharp
public static string HylandUsername { get; set; } = "prod-user";
public static string HylandPassword { get; set; } = "prod-password";
```

---

## DMSService Implementation

The DMSService constructor now adds Hyland authentication headers:

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
    }
}
```

---

## HTTP Request Headers

When uploading a document, the following headers are sent:

```http
POST /api/document/upload HTTP/1.1
Host: dms.example.com
Content-Type: application/json
Hyland-Username: your-username
Hyland-Password: your-password
Content-Length: [size]

{
  "DocumentTypeId": 834,
  "FileTypeId": 16,
  "Base64Document": "[base64-content]",
  "Keywords": [...]
}
```

---

## Security Best Practices

### ⚠️ Important Security Notes

1. **Never commit credentials** to version control
   - Store credentials in environment variables
   - Use configuration files that are not tracked by Git
   - Use Azure Key Vault or similar secure storage in production

2. **Use environment variables**
   ```csharp
   public static string HylandUsername { get; set; } = 
       Environment.GetEnvironmentVariable("HYLAND_USERNAME") ?? "";
   
   public static string HylandPassword { get; set; } = 
       Environment.GetEnvironmentVariable("HYLAND_PASSWORD") ?? "";
   ```

3. **Use secrets in CI/CD**
   - Store credentials in Azure DevOps secrets
   - Inject at build/deployment time
   - Never log or display credentials

4. **HTTPS Only**
   - Always use HTTPS endpoints (not HTTP)
   - Credentials are transmitted in headers - encryption is essential

5. **Minimal Scope**
   - Use service accounts with minimal required permissions
   - Avoid using personal credentials for API access
   - Rotate credentials regularly

---

## Troubleshooting

### Authentication Failures

**Issue**: "401 Unauthorized"
- **Solution**: Verify username and password in AppSettings
- **Check**: Credentials have correct case sensitivity
- **Verify**: Credentials are not empty or null

**Issue**: "403 Forbidden"
- **Solution**: Account may not have upload permissions
- **Check**: Verify account permissions in Hyland system
- **Contact**: DMS administrator to grant upload rights

**Issue**: "500 Internal Server Error"
- **Solution**: Headers may be malformed
- **Check**: Ensure no extra whitespace in credentials
- **Verify**: Headers are added before sending request

---

## Testing

### Verify Hyland Credentials

```csharp
var dmsService = new DMSService();

// This will use credentials from AppSettings
var response = await dmsService.UploadEnrollmentAsync(
    filePath: "test.pdf",
    enrollmentNumber: "ENR-TEST-001",
    firstName: "Test",
    lastName: "User"
);

if (response.Success)
{
    Debug.WriteLine("Authentication successful!");
}
else
{
    Debug.WriteLine($"Authentication failed: {response.Message}");
}
```

---

## Migration Checklist

- [ ] Update `AppSettings.cs` with Hyland credentials
- [ ] Remove any API key references
- [ ] Test with development credentials
- [ ] Verify headers are sent (check browser F12 Network tab)
- [ ] Test with QA credentials
- [ ] Test with production credentials
- [ ] Store credentials securely (not in code)
- [ ] Update deployment scripts
- [ ] Document new credentials in team wiki

---

## Reference

**Header Format**:
```
Hyland-Username: [string]
Hyland-Password: [string]
```

**Endpoint**: 
```
POST https://[dms-server]/api/document/upload
```

**Response**: Still returns standard DMSUploadResponse
```json
{
  "success": true,
  "documentId": "DOC-2025-XXXXX",
  "message": "Document uploaded successfully"
}
```

---

**Updated**: January 15, 2025
**Version**: 2.0 (Hyland Authentication)
**Status**: Current
