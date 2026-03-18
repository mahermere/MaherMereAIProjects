# DMS Enrollment Payload - Visual Reference

## Complete Enrollment Payload Structure

```
┌─────────────────────────────────────────────────────────┐
│         DMS ENROLLMENT UPLOAD REQUEST                    │
├─────────────────────────────────────────────────────────┤
│                                                           │
│  ┌──────────────────────────────────────────────────┐  │
│  │ DocumentTypeId: 834 (Enrollment Form)             │  │
│  │ FileTypeId: 16 (PDF)                             │  │
│  │ Base64Document: [Base64 encoded PDF content]     │  │
│  └──────────────────────────────────────────────────┘  │
│                                                           │
│  ┌──────────────────────────────────────────────────┐  │
│  │ KEYWORDS ARRAY (32 fields)                       │  │
│  ├──────────────────────────────────────────────────┤  │
│  │                                                  │  │
│  │ PRIMARY IDENTIFICATION                          │  │
│  │ ├─ 1254: ENR-12345678-20250115123456-Smith    │  │
│  │ ├─ 1049: John                                 │  │
│  │ ├─ 1051: Smith                                │  │
│  │ └─ 1050: M                                    │  │
│  │                                                  │  │
│  │ DEMOGRAPHICS                                     │  │
│  │ ├─ 1107: 01/15/1950                            │  │
│  │ ├─ 1111: Male                                 │  │
│  │ └─ 1120: 123-45-6789                          │  │
│  │                                                  │  │
│  │ INSURANCE NUMBERS                               │  │
│  │ ├─ 1137: 1234567890A (Medicare)              │  │
│  │ └─ 1127: [Medicaid if applicable]            │  │
│  │                                                  │  │
│  │ CONTACT INFORMATION                             │  │
│  │ ├─ 1052: 787-555-0123 (Primary Phone)        │  │
│  │ ├─ 1253: 787-555-0124 (Cell)                 │  │
│  │ └─ 1063: john.smith@example.com              │  │
│  │                                                  │  │
│  │ PERMANENT ADDRESS                               │  │
│  │ ├─ 1053: 123 Main Street                      │  │
│  │ ├─ 1054: Apt 4B                               │  │
│  │ ├─ 1055: San Juan                             │  │
│  │ ├─ 1056: PR                                   │  │
│  │ └─ 1057: 00901                                │  │
│  │                                                  │  │
│  │ MAILING ADDRESS (if different)                  │  │
│  │ ├─ 1113: 123 Mailing Ave                      │  │
│  │ ├─ 1112: Suite 100                            │  │
│  │ ├─ 1114: San Juan                             │  │
│  │ ├─ 1115: PR                                   │  │
│  │ └─ 1116: 00901                                │  │
│  │                                                  │  │
│  │ EMERGENCY CONTACT                               │  │
│  │ ├─ 1128: Maria Smith                          │  │
│  │ ├─ 1129: 787-555-0199                         │  │
│  │ └─ 1136: Spouse                               │  │
│  │                                                  │  │
│  │ PLAN INFORMATION                                │  │
│  │ ├─ 1110: Óptimo Plus (PPO)                   │  │
│  │ ├─ 1121: H1234 (Contract)                    │  │
│  │ └─ 1122: 001 (PBP)                           │  │
│  │                                                  │  │
│  │ PREFERENCES & METADATA                          │  │
│  │ ├─ 1179: English (Language)                   │  │
│  │ ├─ 1100: Phone (Contact Method)              │  │
│  │ ├─ 1280: 12345678 (Agent NPN)                │  │
│  │ └─ 1082: 01/15/2025 (Signature Date)         │  │
│  │                                                  │  │
│  └──────────────────────────────────────────────────┘  │
│                                                           │
└─────────────────────────────────────────────────────────┘
```

## Keyword ID Quick Reference

### By Category

```
SECTION: Primary Identification
────────────────────────────────
1254  │  Enrollment Number
1049  │  First Name
1051  │  Last Name
1050  │  Middle Name

SECTION: Demographics
───────────────────────
1107  │  Date of Birth
1111  │  Gender
1120  │  Social Security Number

SECTION: Insurance Numbers
──────────────────────────
1137  │  Medicare Number
1127  │  Medicaid Number

SECTION: Contact Information
──────────────────────────────
1052  │  Primary Phone
1253  │  Secondary/Cell Phone
1063  │  Email Address

SECTION: Permanent Address
────────────────────────────
1053  │  Address Line 1
1054  │  Address Line 2
1055  │  City
1056  │  State
1057  │  Zip Code

SECTION: Mailing Address
─────────────────────────
1113  │  Mailing Address 1
1112  │  Mailing Address 2
1114  │  Mailing City
1115  │  Mailing State
1116  │  Mailing Zip Code

SECTION: Emergency Contact
──────────────────────────
1128  │  Emergency Contact Name
1129  │  Emergency Contact Phone
1136  │  Emergency Contact Relationship

SECTION: Plan Information
─────────────────────────
1110  │  Plan Name
1121  │  Plan Contract
1122  │  Plan PBP

SECTION: Preferences & Metadata
──────────────────────────────
1179  │  Language
1100  │  Contact Method
1280  │  Agent NPN
1082  │  Signature Date
```

## JSON Structure Example

```json
{
  "DocumentTypeId": 834,
  "FileTypeId": 16,
  "Base64Document": "JVBERi0xLjQK...",
  "Keywords": [
    {
      "KeywordTypeId": 1254,
      "Value": "ENR-12345678-20250115123456-Smith"
    },
    {
      "KeywordTypeId": 1049,
      "Value": "John"
    },
    {
      "KeywordTypeId": 1051,
      "Value": "Smith"
    }
    // ... 29 more keywords
  ]
}
```

## Data Format Requirements

```
╔════════════════════════════════════════════════╗
║           DATA FORMAT REFERENCE                ║
╠════════════════════════════════════════════════╣
║                                                ║
║ Date of Birth      → MM/DD/YYYY                ║
║ Signature Date     → YYYY-MM-DD                ║
║ Phone Numbers      → XXX-XXX-XXXX              ║
║ State Code         → XX (2 letters)            ║
║ Zip Code           → XXXXX (5 digits)          ║
║ Medicare Number    → XXXXXXXXXXX-X (11+1)     ║
║ SSN                → XXX-XX-XXXX               ║
║ Gender             → Male/Female/etc.          ║
║ Language           → English/Spanish           ║
║ Contact Method     → Phone/Email/Mail/etc.     ║
║                                                ║
╚════════════════════════════════════════════════╝
```

## Enrollment Form Field → Keyword ID Mapping

```
FORM INPUT                          KEYWORD ID      KEYWORD NAME
═══════════════════════════════════════════════════════════════════
Step 1: Personal Information
─────────────────────────────────────────────────────────────────
First Name                    →     1049           TSA-Name
Middle Initial                →     1050           TSA-Middle Name
Last Name                     →     1051           TSA-Last Names
Date of Birth                 →     1107           TSA-DOB
Gender                        →     1111           TSA-Gender
Primary Phone                 →     1052           TSA-Main Phone
Is Mobile Checkbox            →     1253           TSA-Cell Phone (indicator)
Secondary Phone               →     1253           TSA-Cell Phone Number
Email Address                 →     1063           TSA-Email
Medicare Number               →     1137           TSA-Medicare Part A
Social Security Number        →     1120           TSA-SSN
Preferred Contact Method      →     1100           TSA-Contacted

Step 2: Address Information
─────────────────────────────────────────────────────────────────
Permanent Address 1           →     1053           TSA-Address 1
Permanent Address 2           →     1054           TSA-Address 2
City                          →     1055           TSA-City
State                         →     1056           TSA-State
Zip Code                      →     1057           TSA-Zip Code
Different Mailing Address     →     (checkbox)     (conditional)

If Different Mailing Address:
Mailing Address 1             →     1113           TSA-Mailing Address 1
Mailing Address 2             →     1112           TSA-Mailing Address 2
Mailing City                  →     1114           TSA-Mailing City
Mailing State                 →     1115           TSA-Mailing State
Mailing Zip Code              →     1116           TSA-Mailing Zip Code

Step 3: Emergency Contact
─────────────────────────────────────────────────────────────────
Emergency Contact Name        →     1128           TSA-Emergency Contact Name
Emergency Contact Phone       →     1129           TSA-Emergency Contact Phone
Relationship                  →     1136           TSA-Emergency Contact Relationship

Step 5: Plan Information
─────────────────────────────────────────────────────────────────
Plan Name                     →     1110           TSA-Plan
Plan Contract Number          →     1121           TSA-Plan Contract
Plan PBP ID                   →     1122           TSA-Plan PBP

Step 1: Language & Admin
─────────────────────────────────────────────────────────────────
Language Selection            →     1179           TSA-Language
Enrollment Number (System)    →     1254           TSA-Member ID

Automatic Fields (System-Added)
─────────────────────────────────────────────────────────────────
Agent NPN                     →     1280           TSA-NPI
Signature Date                →     1082           TSA-Signature Date
Medicaid Number (if present)  →     1127           TSA-Medicaid Number
```

## Upload Response Example

```json
{
  "success": true,
  "documentId": "DOC-2025-00001234",
  "message": "Document uploaded successfully",
  "timestamp": "2025-01-15T12:34:56Z"
}
```

## Error Response Example

```json
{
  "success": false,
  "documentId": null,
  "message": "Invalid DocumentTypeId: 999",
  "errorCode": "INVALID_DOCTYPE",
  "timestamp": "2025-01-15T12:34:56Z"
}
```

## Integration Checklist

```
□ DocumentTypeId set to 834 (Enrollment)
□ FileTypeId set to 16 (PDF)
□ PDF converted to Base64 string
□ All 32 keywords properly populated
□ Empty values filtered out
□ Dates formatted as MM/DD/YYYY
□ Phone numbers include hyphens
□ State codes are 2 letters
□ Zip codes are 5 digits
□ Medicare number format: XXXXXXXXXXX-X
□ JSON properly serialized
□ HTTPS endpoint configured
□ API key header added (if required)
□ Response parsed correctly
□ Document ID captured
□ Enrollment record updated with DocumentId
```

---

**Quick Reference Version**: 1.1
**Last Updated**: January 15, 2025
