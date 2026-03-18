# DMS Configuration Quick Start

## 🚀 Quick Setup for Different Environments

### Development Environment
```csharp
// Configuration/AppSettings.cs
public static string DMSEndpoint { get; set; } = "https://localhost:44304/api/document/upload";
public static string? DMSApiKey { get; set; } = null;
```

### QA Environment
```csharp
// Configuration/AppSettings.cs
public static string DMSEndpoint { get; set; } = "https://qa-dms.tripless.com/api/document/upload";
public static string? DMSApiKey { get; set; } = "qa-api-key-12345";
```

### Production Environment
```csharp
// Configuration/AppSettings.cs
public static string DMSEndpoint { get; set; } = "https://dms.tripless.com/api/document/upload";
public static string? DMSApiKey { get; set; } = "prod-api-key-67890";
```

## ⚙️ Configuration Steps

1. **Open Configuration File**
   - Navigate to `Configuration/AppSettings.cs`

2. **Update Endpoint**
   - Replace `DMSEndpoint` with your DMS API endpoint URL

3. **Set API Key (if required)**
   - If authentication is needed, set `DMSApiKey` to your API key
   - If not needed, leave as `null`

4. **Adjust Timeout (optional)**
   - Default: 5 minutes
   - For large files or slow networks, increase `DMSUploadTimeoutMinutes`

5. **Set Max File Size (optional)**
   - Default: 50 MB
   - Adjust `MaxFileSizeMB` based on DMS limits

6. **Rebuild Application**
   ```bash
   dotnet publish -c Release -f net9.0-windows10.0.19041.0 -p:RuntimeIdentifierOverride=win10-x64
   ```

## 🧪 Testing the Integration

### Test Upload Flow
1. **Create Test Record**
   - Complete an SOA or Enrollment form
   - Generate PDF

2. **Verify PDF Creation**
   - Check that PDF was created in AppData directory
   - Confirm file is not empty/corrupted

3. **Upload to DMS**
   - Go to Dashboard
   - Find your record in the list
   - Click the ⬆ upload button

4. **Verify Success**
   - Success message should show Document ID
   - Upload button should change to green checkmark (✓)
   - Record should be marked as uploaded in CSV

### Debug Logging
Enable Visual Studio debug output:
1. Run application in Debug mode
2. Open **Output** window (View → Output)
3. Look for `[DMSService]` log entries
4. Check for:
   - File path and size
   - Base64 encoding length
   - Request/response details
   - Error messages

### Manual API Testing (curl)
```bash
# Test with sample JSON
curl -X POST https://localhost:44304/api/document/upload \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d @Documentation/sample-enrollment-request.json

# Expected success response:
# {
#   "success": true,
#   "documentId": "DOC-2025-01234",
#   "message": "Document uploaded successfully",
#   "timestamp": "2025-01-15T12:34:56Z"
# }
```

### Manual API Testing (Postman)
1. **Create New Request**
   - Method: POST
   - URL: `https://localhost:44304/api/document/upload`

2. **Headers**
   - Content-Type: `application/json`
   - X-API-Key: `your-api-key` (if required)

3. **Body**
   - Select "raw" and "JSON"
   - Paste content from `Documentation/sample-enrollment-request.json`
   - Replace `fileContent` with actual Base64 encoded PDF

4. **Send Request**
   - Check response status (should be 200)
   - Verify `success: true` in response body

## 🔍 Troubleshooting

### Common Issues

#### 1. Connection Refused / Network Error
**Symptoms:**
- Error: "Network error: No connection could be made"
- Upload fails immediately

**Solutions:**
- ✅ Verify DMS endpoint URL is correct
- ✅ Check if DMS server is running
- ✅ Test with `curl` or Postman
- ✅ Check firewall settings
- ✅ Verify SSL certificate (for HTTPS)

#### 2. Authentication Failed
**Symptoms:**
- Error: "401 Unauthorized"
- "Invalid API key"

**Solutions:**
- ✅ Verify API key is correct
- ✅ Check if API key is expired
- ✅ Ensure API key header name matches DMS expectation
- ✅ Contact DMS admin for valid API key

#### 3. File Too Large
**Symptoms:**
- Error: "413 Payload Too Large"
- Upload timeout

**Solutions:**
- ✅ Check file size (should be < 50 MB)
- ✅ Increase `MaxFileSizeMB` in AppSettings
- ✅ Increase `DMSUploadTimeoutMinutes`
- ✅ Check DMS server file size limits

#### 4. Invalid Format
**Symptoms:**
- Error: "400 Bad Request"
- "Invalid JSON format"

**Solutions:**
- ✅ Verify JSON structure matches expected format
- ✅ Check Base64 encoding is valid
- ✅ Ensure all required fields are present
- ✅ Compare with sample JSON files

#### 5. File Not Found
**Symptoms:**
- Error: "FILE_NOT_FOUND"
- "PDF file not found"

**Solutions:**
- ✅ Verify PDF was generated successfully
- ✅ Check file path in CSV record
- ✅ Ensure AppData directory is accessible
- ✅ Check file permissions

## 📊 Monitoring & Maintenance

### Check Upload Status
```csharp
// View CSV records to see upload status
var enrollmentCsvPath = Path.Combine(FileSystem.Current.AppDataDirectory, "enrollment_records.csv");
var lines = File.ReadAllLines(enrollmentCsvPath);
foreach (var line in lines)
{
    Console.WriteLine(line);
    // Format: EnrollmentNumber,FirstName,LastName,DateCreated,FilePath,IsUploaded
}
```

### Retry Failed Uploads
1. Open Dashboard
2. Find records without checkmarks (not uploaded)
3. Click upload button to retry
4. Check logs for error details

### Performance Tips
- Upload during off-peak hours for better performance
- Monitor network bandwidth usage
- Consider batch uploads for multiple documents
- Implement upload queue for offline scenarios

## 🆘 Support

### Need Help?
1. **Check Documentation**
   - Review `Documentation/DMS-Integration.md`
   - Check sample JSON files

2. **Enable Debug Logging**
   - Run in Debug mode
   - Check Output window for `[DMSService]` logs

3. **Test API Manually**
   - Use curl or Postman to isolate issues
   - Verify endpoint is responding

4. **Contact IT Support**
   - Provide error messages and logs
   - Include request/response details
   - Share debug output window logs

## 📝 JSON Structure Reference

### Enrollment Upload
```json
{
  "documentType": "Enrollment",
  "fileName": "Enrollment_[timestamp].pdf",
  "fileContent": "[Base64 encoded PDF]",
  "metadata": {
    "enrollmentNumber": "ENR-[NPN]-[timestamp]-[LastName]",
    "beneficiaryFirstName": "[First Name]",
    "beneficiaryLastName": "[Last Name]",
    "medicareNumber": "[Medicare #]",
    "dateOfBirth": "[YYYY-MM-DD]",
    "agentNPN": "[Agent NPN]",
    "uploadDate": "[YYYY-MM-DD HH:mm:ss]",
    "planName": "[Plan Name]",
    "contractNumber": "[Contract #]"
  }
}
```

### SOA Upload
```json
{
  "documentType": "SOA",
  "fileName": "SOA_[timestamp].pdf",
  "fileContent": "[Base64 encoded PDF]",
  "metadata": {
    "soaNumber": "SOA-[NPN]-[timestamp]",
    "beneficiaryFirstName": "[First Name]",
    "beneficiaryLastName": "[Last Name]",
    "medicareNumber": "[Medicare #]",
    "dateOfBirth": "[YYYY-MM-DD]",
    "agentNPN": "[Agent NPN]",
    "uploadDate": "[YYYY-MM-DD HH:mm:ss]"
  }
}
```

## 🔐 Security Checklist

Before deploying to production:

- [ ] HTTPS endpoint configured (not HTTP)
- [ ] API key stored securely (not in source code)
- [ ] SSL certificate validated
- [ ] File size limits enforced
- [ ] Error messages don't expose PHI
- [ ] Audit logging enabled
- [ ] Access controls in place
- [ ] Network security configured
- [ ] Backup and disaster recovery planned

---

**Last Updated:** January 15, 2025
**Version:** 1.1
