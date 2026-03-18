# DMS Keyword Separation for Document Types

## Overview
This document explains the separation of keywords between SOA and Enrollment document types in Hyland OnBase DMS integration.

## Problem Statement
Previously, both SOA (DocumentTypeId 841) and Enrollment (DocumentTypeId 842) shared the same keyword type definitions. This caused upload failures because:
1. **KeywordTypeId 1335 (SP Username)** was being sent for Enrollment documents, but it's only configured for SOA documents in Hyland OnBase
2. Other SOA-specific keywords were unnecessarily included in Enrollment uploads
3. The shared keyword list made it unclear which fields were valid for each document type

## Solution
Separated the keyword definitions into two distinct classes:
- `SOAKeywordTypes` - Keywords configured for SOA documents (DocumentTypeId 841)
- `EnrollmentKeywordTypes` - Keywords configured for Enrollment documents (DocumentTypeId 842)

---

## SOA Document Keywords (DocumentTypeId 841)

### Required Keywords
- **SOA_ID (1067)**: TSA-SOA ID (max 20 characters)

### Identity & Basic Info
- FirstName (1049): TSA-Name
- MiddleName (1050): TSA-Middle Name
- LastName (1051): TSA-Last Names
- DOB (1107): TSA-DOB
- ProspectID (1062): TSA-Prospect ID

### Contact Information
- MainPhone (1052): TSA-Main Phone
- Phone2 (1059): TSA-Phone 2
- Phone3 (1060): TSA-Phone 3
- Email (1063): TSA-Email

### Address Information
- Address1 (1053): TSA-Address 1
- Address2 (1054): TSA-Address 2
- City (1055): TSA-City
- State (1056): TSA-State
- ZipCode (1057): TSA-Zip Code
- ZipCode4 (1058): TSA-Zip Code 4
- Region (1061): TSA-Region

### Medicare Information
- HIC (1092): TSA-HIC (Medicare Number)

### Agent/User Information
- Username (1066): Username
- UploadedBy (1108): UploadedBy
- **SPUsername (1335): SP Username** ⚠️ **SOA ONLY**

### Signature & Dates
- SignatureDate (1082): TSA-Signature Date
- PresentationDate (1070): TSA-Presentation Date
- SignatureIndicator (1109): SignatureIndicator
- SignatureHour (1334): TSA-Signature Hour
- SignatureMethod (1512): Signature Method
- Year (1093): Year
- Timestamp (1333): TSA-Timestamp

### SOA-Specific Fields
- **Attestation (1638): SOA-Attestation** ⚠️ **SOA ONLY**

---

## Enrollment Document Keywords (DocumentTypeId 842)

### Required Keywords
- **EnrollmentID (1067)**: TSA-SOA ID (reused for enrollment ID)

### Identity & Basic Info
- FirstName (1049): TSA-Name
- MiddleName (1050): TSA-Middle Name
- LastName (1051): TSA-Last Names
- DOB (1107): TSA-DOB
- ProspectID (1062): TSA-Prospect ID

### Contact Information
- MainPhone (1052): TSA-Main Phone
- Phone2 (1059): TSA-Phone 2
- Phone3 (1060): TSA-Phone 3
- Email (1063): TSA-Email

### Address Information
- Address1 (1053): TSA-Address 1
- Address2 (1054): TSA-Address 2
- City (1055): TSA-City
- State (1056): TSA-State
- ZipCode (1057): TSA-Zip Code
- ZipCode4 (1058): TSA-Zip Code 4
- Region (1061): TSA-Region

### Medicare Information
- HIC (1092): TSA-HIC (Medicare Number)

### Agent/User Information
- Username (1066): Username
- UploadedBy (1108): UploadedBy
- ❌ **SPUsername (1335) NOT included** - SOA only

### Signature & Dates
- SignatureDate (1082): TSA-Signature Date
- PresentationDate (1070): TSA-Presentation Date
- SignatureIndicator (1109): SignatureIndicator
- SignatureMethod (1512): Signature Method
- Year (1093): Year
- Timestamp (1333): TSA-Timestamp

### Keywords NOT Used for Enrollment
- ❌ **SPUsername (1335)** - Only for SOA documents
- ❌ **Attestation (1638)** - Only for SOA documents
- ❌ **SignatureHour (1334)** - Not typically used for Enrollment

---

## Key Differences

| Keyword | KeywordTypeId | SOA | Enrollment | Notes |
|---------|--------------|-----|------------|-------|
| SP Username | 1335 | ✅ | ❌ | **SOA-specific** - causes upload failure if included in Enrollment |
| Attestation | 1638 | ✅ | ❌ | **SOA-specific field** |
| SignatureHour | 1334 | ✅ | ❌ | Not required for Enrollment |
| All Others | Various | ✅ | ✅ | Shared between both document types |

---

## Implementation Details

### DMSService Helper Methods

#### CreateSOAUploadRequest()
- Uses `SOAKeywordTypes` constants
- Includes all SOA-specific keywords
- **Includes SPUsername (1335)** and Attestation (1638)

#### CreateEnrollmentUploadRequest()
- Uses `EnrollmentKeywordTypes` constants
- **Excludes SPUsername (1335)** to prevent upload failures
- **Excludes Attestation (1638)** as it's not applicable
- Simpler keyword set focused on enrollment data

### Code Example

```csharp
// SOA Upload - includes SPUsername
var soaRequest = DMSService.CreateSOAUploadRequest(
    soaNumber: "SOA-2024-001",
    base64Document: pdfBase64,
    firstName: "John",
    lastName: "Doe",
    agentUsername: "agent123",  // Will be added to SPUsername (1335)
    attestation: "Accepted"     // SOA-specific
);

// Enrollment Upload - excludes SPUsername
var enrollmentRequest = DMSService.CreateEnrollmentUploadRequest(
    enrollmentNumber: "ENR-2024-001",
    base64Document: pdfBase64,
    firstName: "John",
    lastName: "Doe",
    agentUsername: "agent123"   // Will NOT be added to SPUsername (1335)
    // No attestation parameter - not applicable
);
```

---

## Testing & Validation

### Before Separation
❌ Enrollment uploads failed with KeywordTypeId 1335 error
❌ Unclear which keywords were valid for each document type
❌ Potential for other keyword mismatches

### After Separation
✅ Enrollment uploads succeed without KeywordTypeId 1335
✅ Clear separation of SOA vs Enrollment keywords
✅ Type-safe keyword usage with dedicated constants
✅ Better maintainability and documentation

---

## Future Considerations

1. **Add New Keywords**: 
   - Add to the appropriate class (SOAKeywordTypes or EnrollmentKeywordTypes)
   - Document whether it's shared or document-specific

2. **Hyland OnBase Configuration**:
   - Verify keyword configuration in Hyland OnBase before adding to code
   - Consult Hyland admin if keywords don't match

3. **Validation**:
   - Consider adding keyword validation before upload
   - Log warnings if required keywords are missing

4. **Additional Document Types**:
   - If new document types are added, create dedicated keyword classes
   - Follow the same separation pattern

---

## References
- DMSUploadRequest.cs - Keyword type definitions
- DMSService.cs - Helper methods using keywords
- HYLAND_ONBASE_DMS_INTEGRATION.md - Overall DMS integration guide

## Change Log
- **2024-01-XX**: Initial separation of SOA and Enrollment keywords
- **2024-01-XX**: Removed SPUsername (1335) from Enrollment uploads to fix upload failures
