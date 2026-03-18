# Triple-S Enrollment Form - AcroForm Field Mapping Reference

## Document Purpose
This document provides a complete mapping of all 82 data fields captured by the Triple-S Enrollment Wizard to their corresponding AcroForm field names for PDF template generation.

**Version:** 1.0  
**Last Updated:** 2024  
**Total Fields:** 82 (39 Required + 43 Optional)

---

## SECTION 1 - REQUIRED FIELDS (39 fields)

### Step 1: Plan and Beneficiary Information

| # | ViewModel Property | Data Type | AcroForm Field Name | Notes |
|---|-------------------|-----------|---------------------|-------|
| 1 | `ScopeOfAppointmentNumber` | string | `ScopeOfAppointment` | Required, SOA tracking number |
| 2 | `SelectedPlanName` | string | `SelectedPlan_Radio` | **Radio button group** with 10 options. Export values: "Óptimo Plus (PPO)", "Brillante (HMO-POS)", "Enlace Plus (HMO)", "Ahorro Plus (HMO)", "ContigoEnMente (HMO-SNP)", "Contigo Plus (HMO-SNP)", "Platino Plus (HMO-SNP)", "Platino Advance (HMO-SNP)", "Platino Blindao (HMO-SNP)", "Platino Enlace (HMO-SNP)" |
| 3 | `GroupCoverage` | string | `GroupCoverage` | Optional, group plan name if applicable |
| 4 | `GroupPlanType` | string | `GroupPlanType` | Optional, "HMO" or "PPO" |
| 5 | `GroupMonthlyPremium` | string | `GroupMonthlyPremium` | Optional, dollar amount |
| 6 | `GroupEffectiveDate` | DateTime | `GroupEffectiveDate` | Optional, MM/dd/yyyy format |
| 7 | `GroupSocialSecurityNumber` | string | `GroupSSN` | Optional, only for group plans |
| 8 | `FirstName` | string | `FirstName` | Required |
| 9 | `LastName` | string | `LastName` | Required |
| 10 | `MiddleInitial` | string | `MiddleInitial` | Optional, single character |
| 11 | `BirthDate` | DateTime | `BirthDate` | Required, MM/dd/yyyy format |
| 12 | `Sex` | string | `Sex` | Required, "F" or "M" |
| 13 | `HomePhone` | string | `HomePhone` | Required, telephone format |
| 14 | `HomePhoneIsCell` | bool | `HomePhoneIsCell` | Checkbox |
| 15 | `AlternatePhone` | string | `AlternatePhone` | Optional, telephone format |
| 16 | `AlternatePhoneIsCell` | bool | `AlternatePhoneIsCell` | Checkbox |

### Step 2: Address and Medicare Information

| # | ViewModel Property | Data Type | AcroForm Field Name | Notes |
|---|-------------------|-----------|---------------------|-------|
| 17 | `PermanentAddressLine1` | string | `PermanentAddressLine1` | Required, no PO Box |
| 18 | `PermanentAddressLine2` | string | `PermanentAddressLine2` | Optional |
| 19 | `PermanentCity` | string | `PermanentCity` | Required |
| 20 | `PermanentState` | string | `PermanentState` | Required, default "PR" |
| 21 | `PermanentZipCode` | string | `PermanentZipCode` | Required |
| 22 | `MailingAddressDifferent` | bool | `MailingAddressDifferent` | Checkbox |
| 23 | `MailingAddressLine1` | string | `MailingAddressLine1` | Conditional, if different |
| 24 | `MailingAddressLine2` | string | `MailingAddressLine2` | Conditional, if different |
| 25 | `MailingCity` | string | `MailingCity` | Conditional, if different |
| 26 | `MailingState` | string | `MailingState` | Conditional, if different |
| 27 | `MailingZipCode` | string | `MailingZipCode` | Conditional, if different |
| 28 | `MedicareNumber` | string | `MedicareNumber` | Required, format: ____-___-____ |

### Step 3: Coverage Questions

| # | ViewModel Property | Data Type | AcroForm Field Name | Notes |
|---|-------------------|-----------|---------------------|-------|
| 29 | `OtherRxCoverageAnswer` | string | `OtherRxCoverage` | Required, "Yes" or "No" |
| 30 | `OtherCoverageName` | string | `OtherCoverageName` | Conditional, if Yes |
| 31 | `OtherCoverageMemberNumber` | string | `OtherCoverageMemberNumber` | Conditional, if Yes |
| 32 | `OtherCoverageGroupNumber` | string | `OtherCoverageGroupNumber` | Conditional, if Yes |
| 33 | `MedicaidProgramAnswer` | string | `MedicaidProgram` | Conditional, Platino plans only |
| 34 | `MedicaidNumber` | string | `MedicaidNumber` | Conditional, if Yes to Medicaid |
| 35 | `ContigoPlusChronicCondition` | string | `ContigoPlusCondition` | Conditional, Contigo Plus plan only |
| 36 | `ContigoEnMenteDementiaAnswer` | string | `ContigoEnMenteDementia` | Conditional, ContigoEnMente plan only |

### Step 4: Signature and Authorization

| # | ViewModel Property | Data Type | AcroForm Field Name | Notes |
|---|-------------------|-----------|---------------------|-------|
| 37 | `AckPartAandB` | bool | `AckPartAandB` | Required checkbox |
| 38 | `AckSingleMAPlan` | bool | `AckSingleMAPlan` | Required checkbox |
| 39 | `AckInfoCorrect` | bool | `AckInfoCorrect` | Required checkbox |
| 40 | `ApplicantSignatureName` | string | `ApplicantSignatureName` | Required |
| 41 | `SignatureDate` | DateTime | `SignatureDate` | Required, MM/dd/yyyy format |
| 42 | `AuthorizedRepName` | string | `AuthorizedRepName` | Optional, if applicable |
| 43 | `AuthorizedRepAddress` | string | `AuthorizedRepAddress` | Optional, if applicable |
| 44 | `AuthorizedRepPhone` | string | `AuthorizedRepPhone` | Optional, if applicable |
| 45 | `AuthorizedRepRelationship` | string | `AuthorizedRepRelationship` | Optional, if applicable |
| 46 | `EnrollNowChecked` | bool | `EnrollNowChecked` | Electronic enrollment checkbox |
| 47 | `EnrollNowDate` | DateTime | `EnrollNowDate` | Conditional, if EnrollNowChecked |
| 48 | `PhoneEnrollmentCallNumber` | string | `PhoneEnrollmentUCID` | Phone enrollment UCID |
| 49 | `PhoneEnrollmentWitnessSignature` | string | `PhoneEnrollmentWitness` | Phone enrollment witness |
| 50 | `PhoneEnrollmentWitnessDate` | DateTime | `PhoneEnrollmentWitnessDate` | Phone enrollment date |

---

## SECTION 2 - OPTIONAL FIELDS (43 fields)

### Step 5: Preferences and Communications

| # | ViewModel Property | Data Type | AcroForm Field Name | Notes |
|---|-------------------|-----------|---------------------|-------|
| 51 | `PreferredSpokenLanguage` | string | `PreferredLanguage` | Optional |
| 52 | `FormatBraille` | bool | `FormatBraille` | Checkbox |
| 53 | `FormatLargeText` | bool | `FormatLargeText` | Checkbox |
| 54 | `FormatAudioCD` | bool | `FormatAudioCD` | Checkbox |
| 55 | `FormatDataCD` | bool | `FormatDataCD` | Checkbox |
| 56 | `WorkStatusAnswer` | string | `WorkStatus` | Optional, "Yes" or "No" |
| 57 | `SpouseWorkStatusAnswer` | string | `SpouseWorkStatus` | Optional, "Yes" or "No" |
| 58 | `PcpName` | string | `PCPName` | Optional, HMO plans |
| 59 | `PcpPhone` | string | `PCPPhone` | Optional, HMO plans |
| 60 | `WantsProviderDirectoryByEmail` | bool | `EmailProviderDirectory` | Checkbox |
| 61 | `WantsAnnualNoticeByEmail` | bool | `EmailAnnualNotice` | Checkbox |
| 62 | `WantsEvidenceOfCoverageByEmail` | bool | `EmailEvidenceOfCoverage` | Checkbox |
| 63 | `WantsSummaryOfBenefitsByEmail` | bool | `EmailSummaryOfBenefits` | Checkbox |
| 64 | `WantsFormularyByEmail` | bool | `EmailFormulary` | Checkbox |
| 65 | `WantsPromotionalByEmail` | bool | `EmailPromotional` | Checkbox |
| 66 | `WantsEnrollmentConfirmationByEmail` | bool | `EmailEnrollmentConfirmation` | Checkbox |
| 67 | `TextConsentAnswer` | string | `TextConsent` | Optional, "Yes" or "No" |
| 68 | `TextConsentNumber` | string | `TextConsentNumber` | Conditional, if Yes |
| 69 | `EmailConsentAnswer` | string | `EmailConsent` | Optional, "Yes" or "No" |
| 70 | `EmailConsentAddress` | string | `EmailConsentAddress` | Conditional, if Yes |
| 71 | `EmergencyContactName` | string | `EmergencyContactName` | Optional |
| 72 | `EmergencyContactPhone` | string | `EmergencyContactPhone` | Optional |
| 73 | `EmergencyContactRelationship` | string | `EmergencyContactRelationship` | Optional |
| 74 | `IsRetireeAnswer` | string | `IsRetiree` | Optional, "Yes"/"No"/"N/A" |
| 75 | `RetirementDate` | DateTime | `RetirementDate` | Conditional, if Yes |
| 76 | `RetireeName` | string | `RetireeName` | Conditional, if No |
| 77 | `CoversSpouseOrDependentsAnswer` | string | `CoversSpouseDependents` | Optional, "Yes"/"No"/"N/A" |
| 78 | `SpouseName` | string | `SpouseName` | Conditional, if Yes |
| 79 | `DependentNames` | string | `DependentNames` | Conditional, if Yes |
| 80 | `IsLongTermCareResidentAnswer` | string | `IsLTCResident` | Optional, "Yes" or "No" |
| 81 | `LtcInstitutionName` | string | `LTCInstitutionName` | Conditional, if Yes |
| 82 | `LtcAdministratorName` | string | `LTCAdministratorName` | Conditional, if Yes |
| 83 | `LtcPhone` | string | `LTCPhone` | Conditional, if Yes |
| 84 | `CurrentHealthPlan` | string | `CurrentHealthPlan` | Optional, dropdown |
| 85 | `OtherHealthPlanName` | string | `OtherHealthPlanName` | Conditional, if "Other" |

### Step 6: Transition Form (New Member Services)

| # | ViewModel Property | Data Type | AcroForm Field Name | Notes |
|---|-------------------|-----------|---------------------|-------|
| 86 | `TransitionLastNames` | string | `TransitionLastNames` | Required for transition |
| 87 | `TransitionName` | string | `TransitionName` | Required for transition |
| 88 | `TransitionInitial` | string | `TransitionInitial` | Optional |
| 89 | `TransitionDateOfBirth` | DateTime | `TransitionDOB` | Optional |
| 90 | `TransitionTelephone1` | string | `TransitionTelephone1` | Required for transition |
| 91 | `TransitionTelephone2` | string | `TransitionTelephone2` | Optional |
| 92 | `TransitionBenefitPlan` | string | `TransitionBenefitPlan` | Required for transition |
| 93 | `TransitionEffectivityDate` | DateTime | `TransitionEffectivityDate` | Optional |
| 94 | `TransitionShicMedicareNumber` | string | `TransitionShicMedicareNumber` | Required for transition |
| 95 | `TransitionEquipmentServices` | string | `TransitionEquipmentServices` | Optional, dropdown |
| 96 | `TransitionProviderCompany` | string | `TransitionProviderCompany` | Conditional |
| 97 | `TransitionEquipmentEffectivity` | string | `TransitionEquipmentEffectivity` | Conditional |
| 98 | `TransitionPreviousHealthPlan` | string | `TransitionPreviousHealthPlan` | Conditional, dropdown |
| 99 | `TransitionComments` | string | `TransitionComments` | Optional, multi-line |
| 100 | `TransitionInformationProvidedBy` | string | `TransitionInformationProvidedBy` | Required for transition |
| 101 | `TransitionPlanRepresentative` | string | `TransitionPlanRepresentative` | Required for transition |
| 102 | `TransitionRegion` | string | `TransitionRegion` | Required for transition |
| 103 | `TransitionFormDate` | DateTime | `TransitionFormDate` | Optional |

### Step 7: Premium Payment

| # | ViewModel Property | Data Type | AcroForm Field Name | Notes |
|---|-------------------|-----------|---------------------|-------|
| 104 | `PaymentOption` | string | `PaymentOption` | Optional, 4 options dropdown |
| 105 | `EftAccountHolderName` | string | `EFTAccountHolderName` | Conditional, if EFT |
| 106 | `EftRoutingNumber` | string | `EFTRoutingNumber` | Conditional, if EFT |
| 107 | `EftAccountNumber` | string | `EFTAccountNumber` | Conditional, if EFT |
| 108 | `EftAccountType` | string | `EFTAccountType` | Conditional, "Checking"/"Savings" |
| 109 | `CreditCardType` | string | `CreditCardType` | Conditional, "Visa"/"Master Card" |
| 110 | `CreditCardHolderName` | string | `CreditCardHolderName` | Conditional, if CC |
| 111 | `CreditCardNumber` | string | `CreditCardNumber` | Conditional, if CC |
| 112 | `CreditCardExpiration` | string | `CreditCardExpiration` | Conditional, MM/YYYY |
| 113 | `AutoDeductionBenefitSource` | string | `AutoDeductionSource` | Conditional, "Social Security"/"RRB" |

### Step 8: Documents Received

| # | ViewModel Property | Data Type | AcroForm Field Name | Notes |
|---|-------------------|-----------|---------------------|-------|
| 114 | `ReceivedInitialPackage` | bool | `ReceivedInitialPackage` | Checkbox |
| 115 | `ReceivedStarRatingsNotice` | bool | `ReceivedStarRatingsNotice` | Checkbox |
| 116 | `ReceivedWebAvailabilityNotice` | bool | `ReceivedWebAvailabilityNotice` | Checkbox |
| 117 | `ReceivedEnrollmentConfirmation` | bool | `ReceivedEnrollmentConfirmation` | Checkbox |
| 118 | `ReceivedEnrollmentFormCopy` | bool | `ReceivedEnrollmentFormCopy` | Checkbox |
| 119 | `ReceivedAttestationOfEligibility` | bool | `ReceivedAttestationOfEligibility` | Checkbox |
| 120 | `ReceivedPrecertificationChronicDiseases` | bool | `ReceivedPrecertificationChronicDiseases` | Checkbox |
| 121 | `ReceivedPhiAuthorization` | bool | `ReceivedPhiAuthorization` | Checkbox |

### Step 9: Helper Information

| # | ViewModel Property | Data Type | AcroForm Field Name | Notes |
|---|-------------------|-----------|---------------------|-------|
| 122 | `HelperName` | string | `HelperName` | Optional |
| 123 | `HelperRelationship` | string | `HelperRelationship` | Optional |
| 124 | `HelperSignature` | string | `HelperSignature` | Optional |
| 125 | `HelperNpn` | string | `HelperNPN` | Optional, Agents/Brokers |

### Step 10: Official Use Only

| # | ViewModel Property | Data Type | AcroForm Field Name | Notes |
|---|-------------------|-----------|---------------------|-------|
| 126 | `OfficialReceiptDate` | string | `OfficialReceiptDate` | For internal use |
| 127 | `OfficialPlanId` | string | `OfficialPlanID` | For internal use |
| 128 | `OfficialCoverageEffectiveDate` | string | `OfficialCoverageEffectiveDate` | For internal use |

---

## ADDITIONAL FIELDS (Not in main form but needed for validation)

| # | ViewModel Property | Data Type | AcroForm Field Name | Notes |
|---|-------------------|-----------|---------------------|-------|
| 129 | `ConfirmSection1Complete` | bool | `ConfirmSection1Complete` | Final review checkbox |
| 130 | `ConfirmSection2Reviewed` | bool | `ConfirmSection2Reviewed` | Final review checkbox |

---

## FIELD GROUPINGS BY OFFICIAL FORM PAGES

### English Form: enUS-Enrollment-Request-Form.MD.txt
### Spanish Form: 1_s LONG_Digital.txt

| Page | Section | Field Count | Required? |
|------|---------|-------------|-----------|
| P.2 | Plan and Beneficiary Info | 16 | ✅ Required |
| P.3 | Address and Medicare | 12 | ✅ Required |
| P.4 | Coverage Questions | 11 | ✅ Required |
| P.4 | Signature and Authorization | 10 | ✅ Required |
| P.5 | Preferences and Communications | 20 | ❌ Optional |
| P.6 | Emergency, Retiree, LTC, Current Plan | 12 | ❌ Optional |
| P.6 | Transition Form | 18 | ❌ Optional |
| P.7 | Premium Payment | 9 | ❌ Optional |
| P.8 | Documents Received | 8 | ❌ Optional |
| P.9 | Helper Information | 4 | ❌ Optional |
| P.9 | Official Use | 3 | ❌ Internal |

---

## DATA TYPE CONVERSION GUIDE

### For PDF Generation Service

| .NET Type | AcroForm Field Type | Conversion Notes |
|-----------|-------------------|------------------|
| `string` | Text | Direct mapping |
| `bool` | CheckBox | Map `true` → checked, `false` → unchecked |
| `DateTime` | Text | Format as `MM/dd/yyyy` |
| Dropdown (Picker) | ComboBox | Provide dropdown options in PDF |
| Dropdown (Picker) | **Radio Button Group** | Set field value to matching export value. All buttons share same field name. |
| Multi-line (Editor) | Text (multiline) | Use text area field type |

---

## RADIO BUTTON GROUPS IN ACROFORMS

### Understanding Radio Button Behavior

When a PDF form uses **radio buttons** instead of dropdowns:

1. **All radio buttons in a group share the same field name**
   - Example: `SelectedPlan_Radio`

2. **Each radio button has a unique export value**
   - Export value matches the plan name: `"Óptimo Plus (PPO)"`, `"Brillante (HMO-POS)"`, etc.

3. **To select a radio button, set the field value to its export value**
   ```csharp
   // This will automatically select the correct radio button
   SetTextField(form, "SelectedPlan_Radio", "Óptimo Plus (PPO)");
   ```

4. **Only one radio button can be selected at a time** (mutually exclusive)

### Radio Button Fields in This Form

| ViewModel Property | Field Name | Export Values |
|-------------------|------------|---------------|
| `SelectedPlanName` | `SelectedPlan_Radio` | "Óptimo Plus (PPO)", "Brillante (HMO-POS)", "Enlace Plus (HMO)", "Ahorro Plus (HMO)", "ContigoEnMente (HMO-SNP)", "Contigo Plus (HMO-SNP)", "Platino Plus (HMO-SNP)", "Platino Advance (HMO-SNP)", "Platino Blindao (HMO-SNP)", "Platino Enlace (HMO-SNP)" |
| `GroupPlanType` | `GroupPlanType_Radio` | "HMO", "PPO" |
| `Sex` | `Sex_Radio` | "F", "M" |
| `EftAccountType` | `EFTAccountType_Radio` | "Checking", "Savings" |
| `CreditCardType` | `CreditCardType_Radio` | "Visa", "Master Card" |
| `AutoDeductionBenefitSource` | `AutoDeductionSource_Radio` | "Social Security", "RRB" |

*Note: These fields may be radio buttons or dropdowns depending on PDF template design*

---

## CONDITIONAL FIELD LOGIC

### Fields That Depend on Other Fields

| Parent Field | Parent Value | Child Fields |
|-------------|--------------|--------------|
| `MailingAddressDifferent` | `true` | `MailingAddressLine1`, `MailingAddressLine2`, `MailingCity`, `MailingState`, `MailingZipCode` |
| `OtherRxCoverageAnswer` | `"Yes"` | `OtherCoverageName`, `OtherCoverageMemberNumber`, `OtherCoverageGroupNumber` |
| `SelectedPlanName` | Contains "Platino" | `MedicaidProgramAnswer`, `MedicaidNumber` |
| `SelectedPlanName` | `"Contigo Plus"` | `ContigoPlusChronicCondition` |
| `SelectedPlanName` | `"ContigoEnMente"` | `ContigoEnMenteDementiaAnswer` |
| `EnrollNowChecked` | `true` | `EnrollNowDate` |
| `TextConsentAnswer` | `"Yes"` | `TextConsentNumber` |
| `EmailConsentAnswer` | `"Yes"` | `EmailConsentAddress` |
| `IsRetireeAnswer` | `"Yes"` | `RetirementDate` |
| `IsRetireeAnswer` | `"No"` | `RetireeName` |
| `CoversSpouseOrDependentsAnswer` | `"Yes"` | `SpouseName`, `DependentNames` |
| `IsLongTermCareResidentAnswer` | `"Yes"` | `LtcInstitutionName`, `LtcAdministratorName`, `LtcPhone` |
| `CurrentHealthPlan` | `"Other"` | `OtherHealthPlanName` |
| `TransitionEquipmentServices` | Not empty | `TransitionProviderCompany`, `TransitionEquipmentEffectivity`, `TransitionPreviousHealthPlan`, `TransitionComments` |
| `PaymentOption` | `"EFT"` | `EftAccountHolderName`, `EftRoutingNumber`, `EftAccountNumber`, `EftAccountType` |
| `PaymentOption` | `"Credit Card"` | `CreditCardType`, `CreditCardHolderName`, `CreditCardNumber`, `CreditCardExpiration` |
| `PaymentOption` | `"Auto Deduction"` | `AutoDeductionBenefitSource` |

---

## PDF TEMPLATE CREATION CHECKLIST

### For Adobe Acrobat Pro or PDF Library

- [ ] Create 130 AcroForm fields using names from "AcroForm Field Name" column
- [ ] Set field types: Text, CheckBox, ComboBox, Date
- [ ] Mark required fields with red asterisk or required flag
- [ ] Set validation rules for:
  - [ ] Medicare Number format: ____-___-____
  - [ ] Phone numbers: (___) ___-____
  - [ ] ZIP codes: 5 digits
  - [ ] SSN: ###-##-####
  - [ ] Date fields: MM/dd/yyyy
  - [ ] Credit card expiration: MM/YYYY
- [ ] Configure dropdown options for:
  - [ ] `SelectedPlan` (10 plans)
  - [ ] `GroupPlanType` (2 options)
  - [ ] `Sex` (2 options: F, M)
  - [ ] `EFTAccountType` (2 options)
  - [ ] `CreditCardType` (2 options)
  - [ ] `AutoDeductionSource` (2 options)
  - [ ] `CurrentHealthPlan` (6 options)
  - [ ] `TransitionEquipmentServices` (13 options)
  - [ ] `TransitionPreviousHealthPlan` (6 options)
  - [ ] `ContigoPlusCondition` (3 options)
- [ ] Test conditional field visibility/logic if supported by PDF library

---

## IMPLEMENTATION NOTES FOR DEVELOPERS

### PDF Service Integration

```csharp
// Example: Mapping ViewModel to AcroForm fields (including radio buttons)
public async Task<byte[]> FillEnrollmentPdfAsync(
    TripleSEnrollmentWizardViewModel viewModel)
{
    var pdfDocument = PdfReader.Open("EnrollmentTemplate.pdf", PdfDocumentOpenMode.Modify);
    var form = pdfDocument.AcroForm;
    
    // Section 1 - Required
    SetTextField(form, "ScopeOfAppointment", viewModel.ScopeOfAppointmentNumber);
    
    // Radio button group - just set the value to match one of the export values
    SetTextField(form, "SelectedPlan_Radio", viewModel.SelectedPlanName);
    // This automatically selects the radio button with matching export value
    
    SetTextField(form, "FirstName", viewModel.FirstName);
    SetTextField(form, "LastName", viewModel.LastName);
    SetTextField(form, "MiddleInitial", viewModel.MiddleInitial);
    SetDateField(form, "BirthDate", viewModel.BirthDate);
    
    // Another radio button group (Sex)
    SetTextField(form, "Sex_Radio", viewModel.Sex); // "F" or "M"
    
    SetTextField(form, "HomePhone", viewModel.HomePhone);
    SetCheckBox(form, "HomePhoneIsCell", viewModel.HomePhoneIsCell);
    // ... continue for all 130 fields
    
    return pdfDocument.Save();
}

// Helper method works the same for both text fields and radio buttons
private void SetTextField(PdfAcroForm form, string fieldName, string? value)
{
    if (form.Fields[fieldName] is PdfTextField textField && !string.IsNullOrWhiteSpace(value))
    {
        textField.Value = new PdfString(value);
    }
    // Radio buttons are also PdfTextField type in most PDF libraries
    // Setting the value selects the button with matching export value
}

// Example: Handling group plan type radio buttons
SetTextField(form, "GroupPlanType_Radio", viewModel.GroupPlanType); // "HMO" or "PPO"

// Example: Handling payment method radio buttons
if (viewModel.EftAccountType != null)
{
    SetTextField(form, "EFTAccountType_Radio", viewModel.EftAccountType); // "Checking" or "Savings"
}
```

### Radio Button Validation

```csharp
// Verify radio button export values match ViewModel dropdown options
public bool ValidateRadioButtonExportValues(PdfAcroForm form)
{
    var planField = form.Fields["SelectedPlan_Radio"] as PdfRadioButtonField;
    if (planField == null) return false;
    
    // Expected export values from ViewModel
    var expectedPlans = new []
    {
        "Óptimo Plus (PPO)",
        "Brillante (HMO-POS)",
        "Enlace Plus (HMO)",
        "Ahorro Plus (HMO)",
        "ContigoEnMente (HMO-SNP)",
        "Contigo Plus (HMO-SNP)",
        "Platino Plus (HMO-SNP)",
        "Platino Advance (HMO-SNP)",
        "Platino Blindao (HMO-SNP)",
        "Platino Enlace (HMO-SNP)"
    };
    
    // Verify all expected values exist as radio button options
    foreach (var plan in expectedPlans)
    {
        if (!planField.HasOption(plan))
        {
            Console.WriteLine($"Warning: PDF radio button missing export value: {plan}");
            return false;
        }
    }
    
    return true;
}
```

---

## REVISION HISTORY

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 2024 | Initial field mapping document | GitHub Copilot |

---

## RELATED DOCUMENTS

- `Triple-S-Maui-AEP\Documentation\Forms\English\enUS-Enrollment-Request-Form.MD.txt`
- `Triple-S-Maui-AEP\Documentation\Forms\Spanish\1_s LONG_Digital.txt`
- `Triple-S-Maui-AEP\Documentation\Forms\English\enUS-New-Member-Services-Transition-Form.md.txt`
- `Triple-S-Maui-AEP\Services\PdfService.cs`
- `Triple-S-Maui-AEP\ViewModels\TripleSEnrollmentWizardViewModel.cs`
- `Triple-S-Maui-AEP\Views\TripleSEnrollmentWizardPage.xaml`

---

**End of Document**

---

## TROUBLESHOOTING RADIO BUTTONS

### Common Issues and Solutions

| Issue | Cause | Solution |
|-------|-------|----------|
| **Radio button not selected** | Export value mismatch | Verify ViewModel value exactly matches PDF radio button export value (case-sensitive, spacing, special characters) |
| **Wrong button selected** | Multiple buttons with same export value | Ensure each radio button in group has unique export value |
| **No button selected** | Field name incorrect | Check that field name matches radio button group name exactly |
| **Multiple buttons selected** | Not a radio button group | Ensure buttons are grouped (share same field name) in PDF template |

### Export Value Mapping Checklist

When creating PDF template with radio buttons:

- [ ] All radio buttons in a group have **identical field names**
- [ ] Each button has a **unique export value**
- [ ] Export values **exactly match** ViewModel Picker options:
  - [ ] Same capitalization
  - [ ] Same spacing
  - [ ] Same special characters (e.g., parentheses, hyphens)
  - [ ] No extra spaces or trailing characters

### Example: Creating Radio Button Group in Adobe Acrobat Pro

1. **Create first radio button:**
   - Field Name: `SelectedPlan_Radio`
   - Export Value: `Óptimo Plus (PPO)`
   - Appearance: Custom styling

2. **Create remaining buttons:**
   - Field Name: `SelectedPlan_Radio` (same as first)
   - Export Values: `Brillante (HMO-POS)`, `Enlace Plus (HMO)`, etc.
   - Appearance: Match first button

3. **Test in Acrobat:**
   - Only one button should be selectable at a time
   - Selecting a new button deselects the previous one

### Debugging Radio Button Values

```csharp
// Log all radio button options in PDF
public void DebugRadioButtonOptions(PdfAcroForm form, string fieldName)
{
    var field = form.Fields[fieldName];
    if (field is PdfRadioButtonField radioField)
    {
        Console.WriteLine($"Radio button group: {fieldName}");
        Console.WriteLine($"Number of options: {radioField.Options.Count}");
        foreach (var option in radioField.Options)
        {
            Console.WriteLine($"  Export value: '{option}'");
        }
    }
    else
    {
        Console.WriteLine($"Field {fieldName} is not a radio button group");
    }
}

// Usage:
DebugRadioButtonOptions(form, "SelectedPlan_Radio");
// Expected output:
// Radio button group: SelectedPlan_Radio
// Number of options: 10
//   Export value: 'Óptimo Plus (PPO)'
//   Export value: 'Brillante (HMO-POS)'
//   ...
