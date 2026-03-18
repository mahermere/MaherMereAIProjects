# DMS Keyword Type Mapping Reference

## Overview
This document maps enrollment and SOA form fields to Triple-S DMS KeywordTypeIds for document indexing and retrieval.

## Enrollment Form (DocumentTypeId: 834) Keyword Mapping

### Primary/Identification Information
| Field Name | KeywordTypeId | Keyword Name | Description |
|---|---|---|---|
| Enrollment Number | 1254 | TSA-Member ID | Unique enrollment identifier |
| First Name | 1049 | TSA-Name | Enrollee first name |
| Last Name | 1051 | TSA-Last Names | Enrollee last name |
| Middle Name | 1050 | TSA-Middle Name | Enrollee middle name/initial |

### Demographics
| Field Name | KeywordTypeId | Keyword Name | Description |
|---|---|---|---|
| Date of Birth | 1107 | TSA-DOB | Enrollee date of birth (MM/DD/YYYY) |
| Gender | 1111 | TSA-Gender | Male, Female, Non-Binary, etc. |
| Social Security Number | 1120 | TSA-SSN | Enrollee SSN (XXX-XX-XXXX) |

### Insurance Numbers
| Field Name | KeywordTypeId | Keyword Name | Description |
|---|---|---|---|
| Medicare Number | 1137 | TSA-Medicare Part A | Medicare HICN (11 digits + letter) |
| Medicaid Number | 1127 | TSA-Medicaid Number | State Medicaid number |

### Contact Information
| Field Name | KeywordTypeId | Keyword Name | Description |
|---|---|---|---|
| Primary Phone | 1052 | TSA-Main Phone | Primary contact phone number |
| Secondary Phone | 1253 | TSA-Cell Phone Number | Cell or secondary phone |
| Email Address | 1063 | TSA-Email | Email address |

### Permanent Address
| Field Name | KeywordTypeId | Keyword Name | Description |
|---|---|---|---|
| Address Line 1 | 1053 | TSA-Address 1 | Street address |
| Address Line 2 | 1054 | TSA-Address 2 | Apartment/Suite number |
| City | 1055 | TSA-City | City name |
| State | 1056 | TSA-State | State abbreviation (2 letters) |
| Zip Code | 1057 | TSA-Zip Code | 5-digit ZIP code |

### Mailing Address
| Field Name | KeywordTypeId | Keyword Name | Description |
|---|---|---|---|
| Mailing Address 1 | 1113 | TSA-Mailing Address 1 | Mailing street address |
| Mailing Address 2 | 1112 | TSA-Mailing Address 2 | Mailing apartment/suite |
| Mailing City | 1114 | TSA-Mailing City | Mailing city |
| Mailing State | 1115 | TSA-Mailing State | Mailing state (2 letters) |
| Mailing Zip Code | 1116 | TSA-Mailing Zip Code | Mailing ZIP code |

### Emergency Contact Information
| Field Name | KeywordTypeId | Keyword Name | Description |
|---|---|---|---|
| Emergency Contact Name | 1128 | TSA-Emergency Contact Name | Emergency contact person |
| Emergency Contact Phone | 1129 | TSA-Emergency Contact Phone | Emergency contact phone |
| Emergency Contact Relationship | 1136 | TSA-Emergency Contact Relationship | Spouse, Child, Parent, etc. |

### Plan Information
| Field Name | KeywordTypeId | Keyword Name | Description |
|---|---|---|---|
| Plan Name | 1110 | TSA-Plan | Selected plan name (e.g., Óptimo Plus) |
| Plan Contract | 1121 | TSA-Plan Contract | Contract number (e.g., H1234) |
| Plan PBP | 1122 | TSA-Plan PBP | Plan Benefit Package number |

### Preferences & Metadata
| Field Name | KeywordTypeId | Keyword Name | Description |
|---|---|---|---|
| Language | 1179 | TSA-Language | English, Spanish, etc. |
| Contact Method | 1100 | TSA-Contacted | Phone, Email, Mail, In-Person |
| Agent NPN | 1280 | TSA-NPI | Agent/Broker NPN number |
| Signature Date | 1082 | TSA-Signature Date | Date of signature (YYYY-MM-DD) |

---

## SOA Form (DocumentTypeId: 650) Keyword Mapping

| Field Name | KeywordTypeId | Keyword Name | Description |
|---|---|---|---|
| SOA Number | 1067 | TSA-SOA ID | Statement of Appointment number |
| First Name | 1049 | TSA-Name | SOA beneficiary first name |
| Last Name | 1051 | TSA-Last Names | SOA beneficiary last name |
| Date of Birth | 1107 | TSA-DOB | Beneficiary DOB (MM/DD/YYYY) |
| Medicare Number | 1137 | TSA-Medicare Part A | Medicare HICN |
| Phone Number | 1052 | TSA-Main Phone | Contact phone number |
| Agent NPN | 1280 | TSA-NPI | Agent/Broker NPN |
| Signature Date | 1082 | TSA-Signature Date | Date of signature |

---

## Notes

### Date Format
- Use `MM/DD/YYYY` format in the application
- Keywords accept date values as strings
- Example: "01/15/1950" for January 15, 1950

### Phone Format
- Include hyphens: `XXX-XXX-XXXX`
- Example: "787-555-0123"

### Zip Code Format
- Use 5-digit ZIP code: `XXXXX`
- Do not include ZIP+4 in the main ZIP field
- Example: "00901"

### State Code
- Use 2-letter abbreviation
- Example: "PR" for Puerto Rico

### Medicare Number
- Format: `HHHHHHHHHHH-D` (11 digits + 1 letter)
- Example: "1234567890A"

### Conditional Keywords
Some keywords are only populated when applicable:
- **Medicaid Number**: Only if enrollee has Medicaid
- **Mailing Address**: Only if different from permanent address
- **Secondary Phone**: Only if available
- **Middle Name**: Only if available

### Optional Keywords
The DMS system will handle empty/null keywords gracefully:
- Do not include keywords with null or empty values
- The service automatically filters out empty values
- Only populated fields are sent to the DMS

---

## Implementation Example

```csharp
// Upload enrollment with keywords
var uploadResponse = await dmsService.UploadEnrollmentAsync(
    filePath: "C:\AppData\Enrollment_20250115_123456.pdf",
    enrollmentNumber: "ENR-12345678-20250115123456-Smith",
    firstName: "John",
    lastName: "Smith",
    dateOfBirth: "01/15/1950",
    gender: "Male",
    medicare: "1234567890A",
    primaryPhone: "787-555-0123",
    address1: "123 Main Street",
    city: "San Juan",
    state: "PR",
    zipCode: "00901",
    planName: "Óptimo Plus (PPO)",
    contractNumber: "H1234"
);
```

---

## Verification Steps

1. **Validate All Required Fields**: Ensure name, DOB, and insurance numbers are populated
2. **Check Date Format**: Verify dates are MM/DD/YYYY or YYYY-MM-DD as expected
3. **Verify Phone Format**: Confirm phone numbers include hyphens (XXX-XXX-XXXX)
4. **Validate State Code**: Ensure 2-letter state abbreviation (e.g., "PR")
5. **Check Base64 Encoding**: PDF should be properly encoded in Base64
6. **Confirm Document Type**: Enrollment=834, SOA=650
7. **Verify File Type**: PDF=16, Image=2

---

## Reference Links
- Full Keyword List: See `Documentation/keyword types.md`
- DMS Integration Guide: See `Documentation/DMS-Integration.md`
- Quick Start Guide: See `Documentation/DMS-Quick-Start.md`

Last Updated: January 15, 2025
Version: 1.1
