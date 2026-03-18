# Triple-S Enrollment Wizard Field Mapping and Review

**Last reviewed:** February 21, 2026

## Overview
This document maps each field in the Enrollment Wizard to the corresponding Triple-S CMS form question, confirms label/field structure for clarity and bilingual support, and notes any special validation or audit requirements.

---

## Step-by-Step Field Mapping

### Step 1: Personal Information
| Field Name                | Label (EN)                | CMS Form Question / Notes                |
|--------------------------|---------------------------|------------------------------------------|
| FirstNameBox             | First Name                | Applicant's First Name                   |
| MiddleInitialBox         | Middle Initial            | Applicant's Middle Initial               |
| LastNameBox              | Last Name                 | Applicant's Last Name                    |
| DOBPicker                | Date of Birth             | Date of Birth                            |
| GenderCombo              | Gender                    | Gender                                   |
| PrimaryPhoneBox          | Primary Phone             | Primary Phone Number                     |
| SecondaryPhoneBox        | Secondary Phone           | Secondary Phone Number                   |
| EmailBox                 | Email Address             | Email Address                            |
| MedicareBox              | Medicare Number           | Medicare Claim Number                    |
| SSNBox                   | Social Security Number    | Social Security Number                   |
| ContactMethodPicker      | Preferred Contact Method  | Preferred Contact Method                 |

### Step 2: Permanent Residence Address
| Field Name                | Label (EN)                | CMS Form Question / Notes                |
|--------------------------|---------------------------|------------------------------------------|
| Address1Box               | Street Address Line 1     | Permanent Residence Address              |
| Address2Box               | Street Address Line 2     | Permanent Residence Address (cont.)      |
| CityBox                   | City                      | City                                     |
| StateBox                  | State                     | State                                    |
| CountyBox                 | County                    | County                                   |
| ZIPBox                    | ZIP Code                  | ZIP Code                                 |
| DifferentMailingCheckbox  | Mailing address is different | Mailing Address Indicator            |
| MailingAddress1Box        | Mailing Street Address Line 1 | Mailing Address                      |
| MailingAddress2Box        | Mailing Street Address Line 2 | Mailing Address (cont.)               |
| MailingCityBox            | Mailing City              | Mailing City                              |
| MailingStateBox           | Mailing State             | Mailing State                             |
| MailingZIPBox             | Mailing ZIP Code          | Mailing ZIP Code                          |

### Step 3: Emergency Contact
| Field Name                | Label (EN)                | CMS Form Question / Notes                |
|--------------------------|---------------------------|------------------------------------------|
| EmergencyNameBox          | Emergency Contact Name    | Emergency Contact Name                    |
| EmergencyPhoneBox         | Emergency Contact Phone   | Emergency Contact Phone                   |
| EmergencyRelationshipCombo| Relationship              | Relationship to Applicant                 |

### Step 4: Dependents
| Field Name                | Label (EN)                | CMS Form Question / Notes                |
|--------------------------|---------------------------|------------------------------------------|
| Dependents (dynamic)      | Dependents                | List of Dependents                        |

### Step 5: Plan Selection
| Field Name                | Label (EN)                | CMS Form Question / Notes                |
|--------------------------|---------------------------|------------------------------------------|
| PlanNamePicker            | Plan Name                 | Selected Plan Name                        |
| PlanContractBox           | Plan Contract Number      | Plan Contract Number                      |
| PlanIDBox                 | Plan ID                   | Plan ID                                   |
| PremiumPaymentMethodPicker| Premium Payment Method    | Payment Method                            |
| BankAccountBox            | Bank Account Number       | Bank Account Number                       |
| RoutingNumberBox          | Routing Number            | Routing Number                            |
| CreditCardBox             | Credit Card Number        | Credit Card Number                        |
| LTCIndicatorCheckbox      | LTC Indicator             | Long-Term Care Indicator                  |
| LTCFacilityBox            | LTC Facility Name         | LTC Facility Name                         |
| PCPBox                    | Primary Care Provider     | Primary Care Provider                     |
| PCPClinicBox              | PCP Clinic Name           | PCP Clinic Name                           |

### Step 6: Current Coverage
| Field Name                | Label (EN)                | CMS Form Question / Notes                |
|--------------------------|---------------------------|------------------------------------------|
| OtherInsuranceCheckbox    | Other Insurance           | Other Insurance Indicator                 |
| OtherCoverageTypeBox      | Other Coverage Type       | Other Coverage Type                       |
| OtherCoveragePolicyBox    | Other Coverage Policy #   | Other Coverage Policy Number              |
| CurrentMAPlanCheckbox     | Current MA Plan           | Current Medicare Advantage Plan Indicator |
| CurrentPlanNameBox        | Current Plan Name         | Current Plan Name                         |
| CurrentPlanContractBox    | Current Plan Contract #   | Current Plan Contract Number              |
| CurrentPlanIDBox          | Current Plan ID           | Current Plan ID                           |
| CoverageStartDatePicker   | Coverage Start Date       | Coverage Start Date                       |
| ReasonChangeBox           | Reason for Change         | Reason for Plan Change                    |

### Step 7: Special Circumstances
| Field Name                | Label (EN)                | CMS Form Question / Notes                |
|--------------------------|---------------------------|------------------------------------------|
| SNPIndicatorCheckbox      | SNP Indicator             | Special Needs Plan Indicator              |
| SNPTypePicker             | SNP Type                  | SNP Type                                  |
| ChronicConditionBox       | Chronic Condition Type    | Chronic Condition Type                    |
| MSAIndicatorCheckbox      | MSA Indicator             | Medical Savings Account Indicator         |
| MSADepositBox             | MSA Deposit Amount        | MSA Deposit Amount                        |
| PFFSIndicatorCheckbox     | PFFS Indicator            | Private Fee-for-Service Indicator          |
| DualEligibleCheckbox      | Dual Eligible             | Dual Eligible Indicator                    |
| LISCheckbox               | LIS Indicator             | Low Income Subsidy Indicator               |
| ESRDCheckbox              | ESRD Indicator            | End-Stage Renal Disease Indicator          |
| InstitutionalCareCheckbox | Institutional Care        | Institutional Care Indicator               |

### Step 8: Special Enrollment Period (SEP)
| Field Name                | Label (EN)                | CMS Form Question / Notes                |
|--------------------------|---------------------------|------------------------------------------|
| SEPReasonPicker           | SEP Reason                | SEP Reason                                |
| SEPEventDatePicker        | SEP Event Date            | SEP Event Date                            |
| SEPEventDescriptionBox    | SEP Event Description     | SEP Event Description                     |
| SEPDocumentationBox       | SEP Documentation Path    | SEP Documentation Path                    |
| GoodCauseStatusPicker     | Good Cause Status         | Good Cause Status                         |
| GoodCauseNotesBox         | Good Cause Notes          | Good Cause Notes                          |

### Step 9: Signatures & Triple-S Audit Fields
| Field Name                | Label (EN)                | CMS Form Question / Notes                |
|--------------------------|---------------------------|------------------------------------------|
| EnrolleeSignaturePad      | Enrollee Signature or X   | Enrollee Signature or X Mark              |
| XMarkCheckbox             | X Mark Indicator          | If enrollee cannot sign, marks X          |
| AgentSignaturePad         | Agent Signature           | Agent Signature                           |
| WitnessSignaturePad       | Witness Signature         | Witness Signature (if X)                  |
| DeviceInfoBox             | Device Information        | Device info for audit                     |
| IPAddressBox              | IP Address                | IP address for audit                      |
| GPSCoordinatesBox         | GPS Coordinates           | GPS location for audit (optional)         |
| FormVariantBox            | Form Variant              | CMS form variant used                     |
| OMBControlNumberBox       | OMB Control Number        | CMS OMB control number                    |
| AttestationTimestampPicker| Attestation Timestamp     | Date of attestation                       |
| SubmissionLocationTypeBox | Submission Location Type  | Location type for submission              |
| ApplicationDateBox        | Application Date          | Date application submitted                |
| EnrollmentMechanismBox    | Enrollment Mechanism      | How enrollment was submitted              |
| FormIdentifierBox         | Form Identifier           | Unique identifier for submission          |
| EffectiveDatePicker       | Effective Date            | Coverage effective date                   |
| CreatedDatePicker         | Created Date              | Record creation date                      |
| LastModifiedDatePicker    | Last Modified Date        | Record last modified date                 |

---

## Label/Field Structure & Bilingual Support
- All fields use explicit label/field pairs for clarity.
- Labels are present in English and should be mapped in `AppResources.resx` and `AppResources.es-PR.resx` for bilingual support.
- Any new or changed labels should be added to both `.resx` files.

## Validation & Audit Notes
- Required fields are marked with `*` in the UI.
- Signature/X mark logic and witness requirements are enforced in code-behind.
- Device, IP, GPS, and audit fields are auto-captured where possible.

## Pending Actions
- [x] Confirm all labels are present in both English and Spanish `.resx` files.
- [x] Add explicit labels to any missing fields.
- [x] Update `.resx` files for new/changed labels.

## Next Steps
- If you add or change any labels in the future, update both `.resx` files for bilingual support.
- Review translations periodically for accuracy and completeness.
- Use this mapping as a checklist for future form updates or audits.

---

**This document is auto-generated for review and traceability.**
