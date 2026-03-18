# DMS Keyword Configuration - CORRECTED

## Issue Resolution
**Problem**: Enrollment uploads were failing with KeywordTypeId 1335 error because incorrect keywords from SOA (841) were being sent with Enrollment (842) documents.

**Root Cause**: The codebase was using shared keyword definitions and including SOA-specific keywords in Enrollment uploads, which caused Hyland OnBase to reject the upload.

---

## Corrected Keyword Lists

### SOA Document (DocumentTypeId 841) - ONLY Keywords

Based on Swagger API `/api/document/keywords/841`:

**SOA-Specific Keywords (NOT in Enrollment):**
- ✅ **1333 - TSA-Timestamp** (DateTime)
- ✅ **1334 - TSA-Signature Hour** (AlphaNumeric)
- ✅ **1335 - SP Username** (AlphaNumeric)
- ✅ **1638 - SOA-Attestation** (AlphaNumeric)

Plus SOA-specific fields:
- 1062 - TSA-Prospect ID
- 1070 - TSA-Presentation Date
- 1075 - TSA-Disposition
- 1079 - TSA-Campaign ID
- 1078 - TSA-Campaign Name
- 1080 - TSA-SP ID
- 1076 - TSA-SP Name
- 1077 - TSA-SP Region
- 1071 - TSA-SOA Status
- 1500 - SOA-Initial Contact
- And more...

**Shared with Enrollment:**
- 1067 - TSA-SOA ID
- 1049 - TSA-Name
- 1050 - TSA-Middle Name
- 1051 - TSA-Last Names
- 1052 - TSA-Main Phone
- 1053-1058 - Address fields
- 1063 - TSA-Email
- 1066 - Username
- 1082 - TSA-Signature Date
- 1092 - TSA-HIC
- 1093 - Year
- 1107 - TSA-DOB
- 1108 - UploadedBy
- 1109 - SignatureIndicator
- 1512 - Signature Method

---

### Enrollment Document (DocumentTypeId 842) - ONLY Keywords

Based on Swagger API `/api/document/keywords/842`:

**Does NOT Have These SOA Keywords:**
- ❌ **1333 - TSA-Timestamp** (use 1241 - UploadedDate instead)
- ❌ **1334 - TSA-Signature Hour**
- ❌ **1335 - SP Username**
- ❌ **1638 - SOA-Attestation**

**Enrollment-Specific Keywords (NOT in SOA):**
- ✅ **1241 - UploadedDate** (DateTime) - Replaces Timestamp
- 1099 - OnBase ID
- 1111 - TSA-Gender
- 1113-1117 - Mailing Address fields
- 1120 - TSA-SSN
- 1123 - TSA-Coverage Effective Date
- 1128 - TSA-Emergency Contact Name
- 1129 - TSA-Emergency Contact Phone
- 1136 - TSA-Emergency Contact Relationship
- 1144 - TSA-Guardian
- Plus many plan, medical, and enrollment-specific fields

**Shared with SOA:**
- Same core personal, contact, and address fields as listed above

---

## Code Changes Made

### 1. DMSUploadRequest.cs
- ✅ Separated `SOAKeywordTypes` and `EnrollmentKeywordTypes` classes
- ✅ Each class contains ONLY keywords from Swagger API for that document type
- ✅ Clear comments indicating which keywords are document-specific

### 2. DMSService.cs

#### CreateEnrollmentUploadRequest() - FIXED
```csharp
// NO LONGER INCLUDES:
// - Timestamp (1333) ❌
// - SignatureHour (1334) ❌
// - SPUsername (1335) ❌

// NOW INCLUDES:
// - UploadedDate (1241) ✅
// - UploadedBy (1108) ✅
// - Username (1066) ✅
```

#### CreateSOAUploadRequest() - ENHANCED
```csharp
// NOW INCLUDES SOA-SPECIFIC:
// - Timestamp (1333) ✅
// - SignatureHour (1334) ✅
// - SPUsername (1335) ✅
// - Attestation (1638) ✅
```

---

## Testing Results

### Before Fix
❌ Enrollment uploads **FAILED** with error:
```
KeywordTypeId 1335 (SP Username) not configured for DocumentTypeId 842
```

### After Fix
✅ Enrollment uploads **SUCCESS**
- Keywords sent match Hyland OnBase configuration
- No invalid KeywordTypeIds
- Agent tracking still works via Username (1066) and UploadedBy (1108)

✅ SOA uploads **STILL WORK**
- All SOA-specific keywords retained
- Proper timestamp and signature tracking
- SP Username properly recorded

---

## Keyword Usage Summary

| Keyword | ID | SOA 841 | Enroll 842 | Notes |
|---------|----|---------|-----------| ------|
| TSA-SOA ID | 1067 | ✅ | ✅ | REQUIRED for both |
| Username | 1066 | ✅ | ✅ | Agent tracking |
| UploadedBy | 1108 | ✅ | ✅ | Agent tracking |
| **Timestamp** | **1333** | **✅** | **❌** | **SOA ONLY** |
| **Signature Hour** | **1334** | **✅** | **❌** | **SOA ONLY** |
| **SP Username** | **1335** | **✅** | **❌** | **SOA ONLY** |
| **Attestation** | **1638** | **✅** | **❌** | **SOA ONLY** |
| UploadedDate | 1241 | ❌ | ✅ | Enrollment ONLY |
| Signature Date | 1082 | ✅ | ✅ | Both |
| Signature Method | 1512 | ✅ | ✅ | Both |

---

## Implementation Guidelines

### Adding New Keywords

1. **Always check Swagger API first**:
   ```
   GET /api/document/keywords/841  (for SOA)
   GET /api/document/keywords/842  (for Enrollment)
   ```

2. **Never assume keywords are shared** - verify each document type

3. **Add to correct class only**:
   - SOA → `SOAKeywordTypes`
   - Enrollment → `EnrollmentKeywordTypes`

4. **Document with comments**:
   ```csharp
   public const int MyKeyword = 1234;  // Description - SOA ONLY (if applicable)
   ```

### Testing New Uploads

1. Check debug logs for keywords being sent
2. Verify against Swagger API keyword list
3. Test actual upload to Hyland OnBase
4. Confirm document appears in OnBase with correct metadata

---

## References
- Swagger API: `https://localhost:44304/swagger`
- Document Types: 841 (SOA), 842 (Enrollment)
- DMSUploadRequest.cs - Keyword type definitions
- DMSService.cs - Upload request builders
- HYLAND_ONBASE_DMS_INTEGRATION.md - Overall integration guide

---

## Change Log
- **2024-01-XX**: Fixed enrollment upload failures by removing unsupported keywords (1333, 1334, 1335)
- **2024-01-XX**: Separated SOA and Enrollment keyword definitions based on actual Swagger API data
- **2024-01-XX**: Added UploadedDate (1241) for Enrollment instead of Timestamp (1333)
- **2024-01-XX**: Enhanced SOA uploads with all SOA-specific keywords including Attestation (1638)
