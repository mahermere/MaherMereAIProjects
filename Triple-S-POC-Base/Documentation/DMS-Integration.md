# Triple-S Document Management System (DMS) Integration

## Overview
This integration allows the application to upload SOA and Enrollment PDF documents to the Triple-S DMS via REST API.

## Architecture

### Components
1. **DMSService** (`Services/DMSService.cs`) - Main service for uploading documents
2. **DMSUploadRequest** (`Models/DMSUploadRequest.cs`) - Request model with Base64 encoded PDF
3. **DMSUploadResponse** (`Models/DMSUploadResponse.cs`) - Response model from DMS
4. **AppSettings** (`Configuration/AppSettings.cs`) - Configuration for endpoints and settings

### Flow
```
User clicks Upload → DMSService → Convert PDF to Base64 → POST to DMS API → Update UI
```

## Configuration

### Endpoint Configuration
Update the DMS endpoint in `Configuration/AppSettings.cs`:

```csharp
public static string DMSEndpoint { get; set; } = "https://localhost:44304/api/document/upload";
```

**Environments:**
- **Development**: `https://localhost:44304/api/document/upload`
- **QA**: Update to QA DMS endpoint
- **Production**: Update to Production DMS endpoint

### API Authentication
If the DMS API requires authentication, set the API key:

```csharp
public static string? DMSApiKey { get; set; } = "your-api-key-here";
```

### Timeout Settings
Adjust upload timeout (default 5 minutes):

```csharp
public static int DMSUploadTimeoutMinutes { get; set; } = 5;
```

## Request Format

### JSON Structure
```json
{
  "documentType": "Enrollment",
  "fileName": "Enrollment_20250115_123456.pdf",
  "fileContent": "JVBERi0xLjQKJeLjz9MKMSAwIG9iag...",
  "metadata": {
    "enrollmentNumber": "ENR-12345678-20250115123456-Smith",
    "beneficiaryFirstName": "John",
    "beneficiaryLastName": "Smith",
    "medicareNumber": "1234567890A",
    "dateOfBirth": "1950-01-15",
    "agentNPN": "12345678",
    "uploadDate": "2025-01-15 12:34:56",
    "planName": "Óptimo Plus (PPO)",
    "contractNumber": "H1234"
  }
}
```

### Fields
- **documentType**: "Enrollment" or "SOA"
- **fileName**: Original PDF filename
- **fileContent**: PDF file encoded as Base64 string
- **metadata**: Document metadata (see below)

### Metadata Fields

#### Enrollment Metadata
- `enrollmentNumber`: Unique enrollment identifier
- `beneficiaryFirstName`: Enrollee first name
- `beneficiaryLastName`: Enrollee last name
- `medicareNumber`: Medicare number (optional)
- `dateOfBirth`: Date of birth (optional)
- `agentNPN`: Agent NPN number
- `uploadDate`: Upload timestamp
- `planName`: Selected plan name (optional)
- `contractNumber`: Contract number (optional)

#### SOA Metadata
- `soaNumber`: SOA number
- `beneficiaryFirstName`: Beneficiary first name
- `beneficiaryLastName`: Beneficiary last name
- `medicareNumber`: Medicare number (optional)
- `dateOfBirth`: Date of birth (optional)
- `agentNPN`: Agent NPN number
- `uploadDate`: Upload timestamp

## Response Format

### Success Response
```json
{
  "success": true,
  "documentId": "DOC-2025-01234",
  "message": "Document uploaded successfully",
  "timestamp": "2025-01-15T12:34:56Z"
}
```

### Error Response
```json
{
  "success": false,
  "message": "Upload failed: Invalid document format",
  "errorCode": "INVALID_FORMAT",
  "timestamp": "2025-01-15T12:34:56Z"
}
```

## Usage

### Upload from Dashboard (Manual Upload)
1. Navigate to Dashboard
2. Expand SOA or Enrollment section
3. Click the **⬆** upload button next to a record
4. The app will:
   - Read the PDF file
   - Convert to Base64
   - POST to DMS API
   - Show success/error message
   - Update UI (show checkmark if successful)

### Automatic Upload After Submission
Enrollments can be automatically uploaded after PDF generation by implementing the auto-upload in `SubmitEnrollment()`.

## Error Handling

### Common Errors

| Error Code | Description | Resolution |
|------------|-------------|------------|
| `FILE_NOT_FOUND` | PDF file doesn't exist | Ensure PDF was generated successfully |
| `NETWORK_ERROR` | Cannot connect to DMS | Check endpoint URL and network connection |
| `INVALID_FORMAT` | Invalid JSON or Base64 | Check request format |
| `UPLOAD_ERROR` | General upload error | Check logs for details |

### Debugging
Enable detailed logging in `DMSService.cs`:
- Check debug output window for `[DMSService]` logs
- Inspect request/response content
- Verify Base64 encoding length

## Testing

### Test with Sample JSON
You can test the DMS endpoint using the sample JSON file provided:

1. **Update Endpoint**: Modify `AppSettings.DMSEndpoint` if needed
2. **Run Application**: Build and run the app
3. **Create Test Record**: Complete an enrollment or SOA
4. **Upload**: Click the upload button on the dashboard
5. **Verify**: Check debug logs and DMS system

### Mock Testing
For local testing without DMS backend:
1. Comment out the actual `PostAsync` call in `DMSService.cs`
2. Return a mock success response
3. Test UI flow and CSV updates

## Security Notes

⚠️ **Important Security Considerations:**
1. **HTTPS Only**: Always use HTTPS in production
2. **API Keys**: Store API keys securely (not in source code)
3. **PHI Protection**: PDF files contain PHI - ensure DMS is HIPAA compliant
4. **Access Control**: Implement proper authentication/authorization
5. **Audit Logging**: Log all uploads with timestamps and user info

## Customization

### Adjusting Request Format
If Triple-S DMS expects different field names or structure, update:
1. `DMSUploadRequest.cs` - Modify property names and `JsonPropertyName` attributes
2. `DMSMetadata.cs` - Add/remove metadata fields
3. `DMSService.cs` - Update metadata population logic

### Adding New Document Types
To support additional document types:
1. Add new upload method to `DMSService.cs` (e.g., `UploadMedicalRecordAsync`)
2. Create appropriate metadata structure
3. Update UI to call the new method

## Troubleshooting

### Upload Button Not Working
- Check if `IsUploaded` is already `true`
- Verify file path is valid
- Check network connection

### Base64 Encoding Issues
- Ensure PDF file is not corrupted
- Check file size (max 50 MB by default)
- Verify file read permissions

### API Connection Issues
- Verify endpoint URL is correct
- Check SSL/TLS certificate (for HTTPS)
- Test endpoint with Postman or curl

## Sample API Test (curl)
```bash
curl -X POST https://localhost:44304/api/document/upload \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d @sample-request.json
```

## Maintenance

### Monitoring
- Monitor upload success/failure rates
- Track average upload time
- Log failed uploads for retry

### Performance
- Consider chunking for large files (>10 MB)
- Implement retry logic for transient failures
- Add upload queue for offline scenarios

## Contact
For DMS API questions or issues, contact Triple-S IT Support.
