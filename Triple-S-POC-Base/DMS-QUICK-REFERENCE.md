# DMS Keyword Mapping - Quick Reference Card

## 🎯 32 Enrollment Keywords at a Glance

```
PRIMARY INFO              DEMOGRAPHICS             INSURANCE
1254: Member ID          1107: DOB                1137: Medicare
1049: First Name         1111: Gender            1127: Medicaid
1051: Last Name          1120: SSN
1050: Middle Name

CONTACT                  PERMANENT ADDRESS        MAILING ADDRESS
1052: Phone              1053: Address 1          1113: Address 1
1253: Cell Phone         1054: Address 2          1112: Address 2
1063: Email              1055: City               1114: City
                         1056: State              1115: State
                         1057: Zip                1116: Zip

EMERGENCY CONTACT        PLAN                     METADATA
1128: Name               1110: Plan Name          1179: Language
1129: Phone              1121: Contract           1100: Contact Method
1136: Relationship       1122: PBP                1280: Agent NPN
                                                  1082: Signature Date
```

---

## 📝 Code Usage Template

```csharp
var dmsService = new DMSService();

var response = await dmsService.UploadEnrollmentAsync(
    filePath: pdfPath,
    enrollmentNumber: "ENR-12345678-20250115123456-Smith",
    firstName: "John",
    lastName: "Smith",
    dateOfBirth: "01/15/1950",      // MM/DD/YYYY
    gender: "Male",
    ssn: "123-45-6789",               // XXX-XX-XXXX
    medicare: "1234567890A",          // XXXXXXXXXXX-X
    primaryPhone: "787-555-0123",     // XXX-XXX-XXXX
    address1: "123 Main Street",
    city: "San Juan",
    state: "PR",                      // 2-letter code
    zipCode: "00901",                 // 5 digits
    planName: "Óptimo Plus (PPO)",
    contractNumber: "H1234",
    language: "English"
);

if (response.Success)
{
    Debug.WriteLine($"Upload OK: {response.DocumentId}");
}
else
{
    Debug.WriteLine($"Upload Failed: {response.Message}");
}
```

---

## 📊 JSON Structure (Minimal Example)

```json
{
  "DocumentTypeId": 834,
  "FileTypeId": 16,
  "Base64Document": "JVBERi0xLjQK...",
  "Keywords": [
    { "KeywordTypeId": 1254, "Value": "ENR-123..." },
    { "KeywordTypeId": 1049, "Value": "John" },
    { "KeywordTypeId": 1051, "Value": "Smith" },
    { "KeywordTypeId": 1107, "Value": "01/15/1950" },
    { "KeywordTypeId": 1111, "Value": "Male" },
    { "KeywordTypeId": 1120, "Value": "123-45-6789" },
    { "KeywordTypeId": 1137, "Value": "1234567890A" }
    // ... 25 more keywords
  ]
}
```

---

## ⏱️ Data Format Quick Reference

| Type | Format | Example |
|------|--------|---------|
| Date of Birth | MM/DD/YYYY | 01/15/1950 |
| Signature Date | YYYY-MM-DD | 2025-01-15 |
| Phone | XXX-XXX-XXXX | 787-555-0123 |
| State | XX | PR |
| Zip | XXXXX | 00901 |
| Medicare | XXXXXXXXXXX-X | 1234567890A |
| SSN | XXX-XX-XXXX | 123-45-6789 |

---

## 🚀 Integration in 3 Steps

### Step 1: Add Using Statement
```csharp
using TripleS.SOA.AEP.UI.Services;
```

### Step 2: Call After PDF Generation
```csharp
var dmsService = new DMSService();
var response = await dmsService.UploadEnrollmentAsync(
    outputPath,
    enrollmentNumber,
    FirstNameBox.Text,
    LastNameBox.Text
    // ... other fields
);
```

### Step 3: Handle Response
```csharp
if (response.Success)
{
    DisplayAlert("Success", response.DocumentId, "OK");
}
else
{
    DisplayAlert("Error", response.Message, "OK");
}
```

---

## 🔍 Verify Before Upload

- [ ] PDF file exists
- [ ] PDF is readable
- [ ] First name filled
- [ ] Last name filled
- [ ] Date format: MM/DD/YYYY
- [ ] State: 2 letters
- [ ] Zip: 5 digits
- [ ] Medicare: 11 digits + letter
- [ ] Phone: XXX-XXX-XXXX

---

## 🐛 Troubleshooting

| Problem | Solution |
|---------|----------|
| Network error | Check endpoint URL, firewall |
| Invalid format | Verify date/phone/state formats |
| 400 Bad Request | Check keyword IDs, JSON syntax |
| File not found | Verify PDF generation |
| Timeout | Increase timeout in AppSettings |

---

## 📚 Documentation Files

**START HERE:**
- `DELIVERY-SUMMARY.md` - Quick overview
- `DMS-KEYWORD-MAPPING-COMPLETE.md` - Full summary

**FOR REFERENCE:**
- `DMS-Keyword-Mapping.md` - All 32 fields
- `sample-enrollment-request.json` - Real example
- `DMS-CODE-IMPLEMENTATION-GUIDE.md` - Integration

**FOR HELP:**
- `DMS-Quick-Start.md` - Troubleshooting
- `DMS-Payload-Visual-Reference.md` - Diagrams

---

## 📞 Support Information

### DMS Endpoint (Dev)
```
https://localhost:44304/api/document/upload
```

### Document Types
- **834** = Enrollment
- **650** = SOA

### File Types
- **16** = PDF
- **2** = Image

### Configuration File
`Configuration/AppSettings.cs`

---

## ✅ Checklist

- [ ] Read DELIVERY-SUMMARY.md
- [ ] Review keyword mapping table
- [ ] Check sample-enrollment-request.json
- [ ] Follow DMS-CODE-IMPLEMENTATION-GUIDE.md
- [ ] Update AppSettings.cs endpoint
- [ ] Add UploadEnrollmentAsync() call
- [ ] Handle response
- [ ] Test with sample data
- [ ] Deploy to production

---

**Quick Ref Version**: 1.0
**Date**: January 15, 2025
**Status**: Ready to Use

🎯 Everything is ready! Start with DELIVERY-SUMMARY.md
