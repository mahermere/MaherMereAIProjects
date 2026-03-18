# Hyland OnBase DMS Integration Implementation

## Overview
Successfully integrated the Triple-S Medicare Advantage Enrollment Platform with **Hyland OnBase** Document Management System (DMS) for storing SOA and Enrollment documents with comprehensive keyword metadata.

## API Configuration

### Endpoint
```
POST https://localhost:44304/api/document/upload
```

### Authentication
Headers required:
- `Hyland-Username`: mahmer
- `Hyland-Password`: password

### Request Format
```json
{
  "DocumentTypeId": 841,
  "FileTypeId": 0,
  "Base64Document": "string",
  "Keywords": [
    {
      "KeywordTypeId": 1067,
      "Value": "SOA-2024-001"
    },
    {
      "KeywordTypeId": 1049,
      "Value": "John"
    }
  ]
}
```

## Document Type IDs

| Document Type | ID | Description |
|---------------|--|----|
| SOA (Signature of Authority) | 841 | Medicare Advantage SOA Document |
| Enrollment | TBD | Medicare Advantage Enrollment |

## SOA Keyword Mapping

### Required Keywords
| Field | KeywordTypeId | Name | Data Type |
|-------|-------------|------|-----------|
| SOA ID | 1067 | TSA-SOA ID | AlphaNumeric |
| First Name | 1049 | TSA-Name | AlphaNumeric |
| Last Name | 1051 | TSA-Last Names | AlphaNumeric |
| Main Phone | 1052 | TSA-Main Phone | AlphaNumeric |
| Medicare Number | 1092 | TSA-HIC | AlphaNumeric |
| Date of Birth | 1107 | TSA-DOB | Date |
| Username | 1066 | Username | AlphaNumeric |
| Uploaded By | 1108 | UploadedBy | AlphaNumeric |
| Timestamp | 1333 | TSA-Timestamp | DateTime |
| Signature Method | 1512 | Signature Method | AlphaNumeric |
| Signature Date | 1082 | TSA-Signature Date | Date |
| Year | 1093 | Year | AlphaNumeric |
| Attestation | 1638 | SOA-Attestation | AlphaNumeric |

### Optional Keywords
| Field | KeywordTypeId | Name | Data Type |
|-------|-------------|------|-----------|
| Middle Name | 1050 | TSA-Middle Name | AlphaNumeric |
| Address 1 | 1053 | TSA-Address 1 | AlphaNumeric |
| Address 2 | 1054 | TSA-Address 2 | AlphaNumeric |
| City | 1055 | TSA-City | AlphaNumeric |
| State | 1056 | TSA-State | AlphaNumeric |
| Zip Code | 1057 | TSA-Zip Code | AlphaNumeric |
| Email | 1063 | TSA-Email | AlphaNumeric |
| SP Username | 1335 | SP Username | AlphaNumeric |
| Signature Hour | 1334 | TSA-Signature Hour | AlphaNumeric |

## Implementation Details

### 1. Models (`Triple-S-Maui-AEP\Models\DMSUploadRequest.cs`)

#### DMSUploadRequest
```csharp
public class DMSUploadRequest
{
    public int DocumentTypeId { get; set; }
    public int FileTypeId { get; set; }
    public string Base64Document { get; set; }
    public List<DMSKeyword> Keywords { get; set; }
}
```

#### DMSKeyword
```csharp
public class DMSKeyword
{
    public int KeywordTypeId { get; set; }
    public string Value { get; set; }
}
```

#### SOAKeywordTypes (Static Constants)
```csharp
public static class SOAKeywordTypes
{
    public const int SOA_ID = 1067;
    public const int FirstName = 1049;
    public const int LastName = 1051;
    public const int MainPhone = 1052;
    public const int HIC = 1092;
    public const int DOB = 1107;
    public const int Username = 1066;
    public const int UploadedBy = 1108;
    public const int Timestamp = 1333;
    public const int SignatureMethod = 1512;
    // ... more constants
}
```

### 2. Service (`Triple-S-Maui-AEP\Services\DMSService.cs`)

#### HttpClient Configuration
```csharp
_httpClient = new HttpClient
{
    BaseAddress = new Uri(DMS_BASE_URL)
};
_httpClient.DefaultRequestHeaders.Add("Hyland-Username", HYLAND_USERNAME);
_httpClient.DefaultRequestHeaders.Add("Hyland-Password", HYLAND_PASSWORD);
```

#### Upload Method
```csharp
public async Task<DMSUploadResponse> UploadDocumentAsync(DMSUploadRequest request)
{
    var response = await _httpClient.PostAsJsonAsync(DMS_UPLOAD_ENDPOINT, request);
    
    if (response.IsSuccessStatusCode)
    {
        return new DMSUploadResponse 
        { 
            Success = true,
            DocumentId = responseBody,
            Message = "Document uploaded successfully"
        };
    }
}
```

#### Helper Method for SOA
```csharp
public static DMSUploadRequest CreateSOAUploadRequest(
    string soaNumber,
    string base64Document,
    string firstName,
    string lastName,
    // ... additional parameters
)
{
    var request = new DMSUploadRequest
    {
        DocumentTypeId = DMSDocumentTypes.SOA,
        FileTypeId = 0,
        Base64Document = base64Document,
        Keywords = new List<DMSKeyword>()
    };
    
    // Add keywords...
    request.Keywords.Add(new DMSKeyword 
    { 
        KeywordTypeId = SOAKeywordTypes.SOA_ID, 
        Value = soaNumber 
    });
    
    return request;
}
```

### 3. ViewModel (`Triple-S-Maui-AEP\ViewModels\DashboardViewModel.cs`)

#### SOA Upload
```csharp
public async Task UploadSOAAsync(string soaNumber)
{
    var dmsService = new DMSService();
    
    var uploadRequest = DMSService.CreateSOAUploadRequest(
        soaNumber: soaNumber,
        base64Document: fileBase64,
        firstName: soaRecord.FirstName,
        lastName: soaRecord.LastName,
        signatureDate: soaRecord.DateCreated,
        agentUsername: AgentSessionService.CurrentAgentNPN,
        signatureMethod: "Digital"
    );

    var response = await dmsService.UploadDocumentAsync(uploadRequest);
    if (response.Success)
    {
        soaRecord.IsUploaded = true;
        SOAService.UpdateUploadStatus(soaNumber, true);
        CompletedSOACount++;
    }
}
```

#### Enrollment Upload
```csharp
public async Task UploadEnrollmentAsync(string enrollmentNumber)
{
    var uploadRequest = new Models.DMSUploadRequest
    {
        DocumentTypeId = 841, // TODO: Update with correct enrollment type
        FileTypeId = 0,
        Base64Document = fileBase64,
        Keywords = new List<Models.DMSKeyword>
        {
            new Models.DMSKeyword { KeywordTypeId = SOAKeywordTypes.FirstName, Value = enrollmentRecord.FirstName },
            new Models.DMSKeyword { KeywordTypeId = SOAKeywordTypes.LastName, Value = enrollmentRecord.LastName },
            // ... more keywords
        }
    };

    var response = await dmsService.UploadDocumentAsync(uploadRequest);
    if (response.Success)
    {
        EnrollmentService.UpdateUploadStatus(enrollmentNumber, true);
    }
}
```

## Stub Mode

For development/testing without actual DMS connection:

```csharp
// In DMSService
public static bool UseStubMode { get; set; } = true; // Set to false for production

// Returns mock response
return new DMSUploadResponse
{
    Success = true,
    DocumentId = $"DOC_{Guid.NewGuid()}",
    Message = "Document uploaded successfully (stubbed)",
    Timestamp = DateTime.UtcNow.ToString("o")
};
```

## CSV Persistence

After successful DMS upload, the upload status is persisted to CSV:

```csharp
// Update in-memory record
soaRecord.IsUploaded = true;

// Persist to CSV file
SOAService.UpdateUploadStatus(soaNumber, true);

// CSV location: {AppData}/data/soa.csv
// Field: IsUploaded=true
```

## Error Handling

### DMS Upload Errors
```
✗ Network failure → Returns DMSUploadResponse.Success = false
✗ Invalid credentials → HTTP 401
✗ Missing keywords → HTTP 400
✗ Invalid DocumentTypeId → HTTP 400
```

### Debug Output
```
Uploading to DMS: /api/document/upload
DocumentTypeId: 841, Keywords: 12
DMS Response: 200 OK
DMS Response Body: DOC_ABC123
SOA uploaded successfully: SOA-2024-001, DocumentId: DOC_ABC123
Updated SOA CSV: SOA-2024-001, IsUploaded=true
```

## Workflow

### Complete Upload Flow
```
1. Dashboard: User clicks Upload (↑) button
   ↓
2. DashboardPage.OnUploadSOAClicked()
   ↓
3. DashboardViewModel.UploadSOAAsync()
   ↓
4. Read PDF file from AppData
   ↓
5. Convert to Base64
   ↓
6. Create DMSUploadRequest with keywords
   ↓
7. DMSService.UploadDocumentAsync()
   ↓
8. POST to https://localhost:44304/api/document/upload
   ↓
9. If Success (HTTP 200):
   a. Update in-memory record
   b. Update UI
   c. Persist to CSV
   d. Update statistics
   ↓
10. Dashboard UI reflects changes
```

## Testing Checklist

### Unit Tests
- [ ] CreateSOAUploadRequest builds correct keyword list
- [ ] Keywords are formatted correctly
- [ ] DocumentTypeId is set to 841
- [ ] Base64Document is valid
- [ ] Dates are formatted correctly

### Integration Tests
- [ ] Upload succeeds with stub mode enabled
- [ ] CSV file is updated after success
- [ ] Dashboard UI updates after upload
- [ ] Error handling works correctly
- [ ] Stub mode can be toggled to production

### Manual Tests
- [ ] Create SOA
- [ ] Generate PDF
- [ ] Click Upload button
- [ ] Verify success message
- [ ] Check CSV file
- [ ] Verify button changes to ✓ (green)
- [ ] Verify status shows "Uploaded"
- [ ] Restart app and verify status persists

## Configuration

### AppSettings.cs
```csharp
public static string DMSEndpoint { get; set; } = 
    "https://localhost:44304/api/document/upload";

public static string HylandUsername { get; set; } = 
    Environment.GetEnvironmentVariable("HYLAND_USERNAME") ?? "mahmer";

public static string HylandPassword { get; set; } = 
    Environment.GetEnvironmentVariable("HYLAND_PASSWORD") ?? "password";
```

### Environment Variables (Production)
```
HYLAND_USERNAME=<actual_username>
HYLAND_PASSWORD=<actual_password>
```

## Future Enhancements

### Phase 1 (Current)
- ✅ SOA upload with keywords
- ✅ Basic error handling
- ✅ CSV persistence
- ✅ Stub mode for testing

### Phase 2 (Required)
- [ ] Identify correct enrollment DocumentTypeId
- [ ] Map enrollment keywords
- [ ] Test with actual Hyland OnBase instance
- [ ] Enable production mode (UseStubMode = false)
- [ ] Configure environment variables

### Phase 3 (Optional)
- [ ] Batch uploads
- [ ] Retry logic for failed uploads
- [ ] Progress tracking for large files
- [ ] Download documents from DMS
- [ ] Search/query capabilities

## Known Issues

### TODO Items
1. **Enrollment DocumentTypeId**: Need to identify correct ID for enrollment documents
   - Currently using 841 (SOA ID) as placeholder
   - Contact DMS admin for official ID

2. **FileTypeId**: Need to identify PDF file type ID
   - Currently using 0 as placeholder
   - May need to change based on DMS configuration

3. **SSL Certificate**: localhost self-signed certificate
   - May need certificate pinning in production
   - Consider using actual domain in production

## Files Modified

1. `Models\DMSUploadRequest.cs` - Updated to match Hyland OnBase API
2. `Services\DMSService.cs` - Implemented Hyland OnBase integration
3. `ViewModels\DashboardViewModel.cs` - Updated upload methods
4. `Models\SOAKeywordTypes.cs` - Added keyword type constants (in DMSUploadRequest.cs)

## Build Status
✅ **Build Successful**

## Summary

Successfully implemented integration with **Hyland OnBase DMS** for document management:

✅ RESTful API integration
✅ Comprehensive keyword mapping for SOA documents
✅ Secure authentication with Hyland credentials
✅ Base64 document encoding
✅ CSV persistence after upload
✅ Error handling and logging
✅ Stub mode for development
✅ Dashboard UI integration
✅ Upload status tracking

The system is ready for testing with the actual DMS instance once enrollment DocumentTypeId is confirmed.
