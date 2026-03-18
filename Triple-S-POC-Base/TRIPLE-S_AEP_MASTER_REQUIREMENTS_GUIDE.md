# Triple-S AEP Master Requirements Guide
**CMS Medicare Advantage Enrollment Processing**

**Version**: 2026.1  
**Effective Date**: February 5, 2026  
**Applies To**: All Triple-S AEP Implementations (Windows, Android, Android2)  
**CMS Authority**: Form 0938-1378 (CY 2026), CMS Enrollment and Disenrollment Guidance  
**Status**: ✅ OFFICIAL REQUIREMENTS BASELINE

---

## Document Purpose

This is the **single authoritative requirements document** for all Triple-S Annual Enrollment Period (AEP) applications across all platforms. All implementations (Windows WPF, Android, Android2) must comply with these requirements to ensure CMS Medicare Advantage enrollment compliance.

---

## Table of Contents

1. [CMS Data Elements (32 Required)](#1-cms-data-elements-32-required)
2. [Enrollment Form Types (4 Required)](#2-enrollment-form-types-4-required)
3. [Form Sections (9 Required per CMS Form 0938-1378)](#3-form-sections-9-required-per-cms-form-0938-1378)
4. [Plan Types (5 Required)](#4-plan-types-5-required)
5. [Form Variants (Conditional Sections)](#5-form-variants-conditional-sections)
6. [Special Enrollment Periods (16 CMS Reasons)](#6-special-enrollment-periods-16-cms-reasons)
7. [Enrollment Periods](#7-enrollment-periods)
8. [Special Populations](#8-special-populations)
9. [Validation Rules](#9-validation-rules)
10. [Business Rules](#10-business-rules)
11. [Bilingual Support (English/Spanish)](#11-bilingual-support-englishspanish)
12. [Electronic Signature Requirements](#12-electronic-signature-requirements)
13. [Compliance Requirements](#13-compliance-requirements)
14. [Export Formats](#14-export-formats)
15. [Error Messages](#15-error-messages)
16. [Verification Checklist](#16-verification-checklist)

---

## 1. CMS Data Elements (32 Required)

All implementations MUST capture and process the following 32 CMS data elements per CMS Appendix 2.

### 1.1 Personal Information (Elements 1-12, 17)

| Element # | Field Name | Required | Data Type | Validation | Notes |
|-----------|------------|----------|-----------|------------|-------|
| 2 | First Name | **YES** | String(50) | Non-empty, alphabetic | No special characters except hyphen, apostrophe |
| 2 | Middle Initial | NO | String(1) | Alphabetic | Optional |
| 2 | Last Name | **YES** | String(50) | Non-empty, alphabetic | No special characters except hyphen, apostrophe, spaces |
| 3 | Date of Birth | **YES** | Date | MM/DD/YYYY | Must be 18+, not future date, realistic (1900+) |
| 4 | Sex/Gender | **YES** | Enum | Male, Female, Non-Binary, Prefer Not to Answer | Required as of 2026 CMS guidance |
| 5 | Primary Phone Number | **YES** | String(15) | (XXX) XXX-XXXX or 10-15 digits | US/PR format |
| 5 | Secondary Phone Number | NO | String(15) | (XXX) XXX-XXXX or 10-15 digits | Optional contact |
| 10 | Email Address | NO | String(100) | Standard email format | Optional, validate if provided |
| 11 | Medicare Number | **YES** | String(11-15) | Alphanumeric | Various formats (11-char new format, legacy formats) |
| 12 | Medicare Card Image | NO | Binary/Path | Image file (JPG, PNG, PDF) | For verification purposes |
| 17 | Social Security Number | NO | String(9) | 9 digits, XXX-XX-XXXX | Optional but commonly required by plans |
| 25 | Preferred Contact Method | **YES** | Enum | Phone, Email, Mail, In-Person | Default: Phone |

**Implementation Note**: All platforms must include these fields in their enrollment forms with identical validation.

---

### 1.2 Address Information (Elements 6-8)

| Element # | Field Name | Required | Data Type | Validation | Notes |
|-----------|------------|----------|-----------|------------|-------|
| 6 | Street Address Line 1 | **YES** | String(100) | Non-empty | Permanent residence |
| 6 | Street Address Line 2 | NO | String(50) | Optional | Apartment, suite, etc. |
| 6 | City | **YES** | String(50) | Non-empty | Permanent residence city |
| 6 | State/Territory | **YES** | String(2) | Two-letter code | Default: PR for Triple-S Puerto Rico |
| 7 | County | NO | String(50) | Optional | Auto-derive from ZIP if possible |
| 6 | ZIP Code | **YES** | String(5-10) | 5-digit or ZIP+4 | Puerto Rico ZIP codes |
| 8 | Mailing Address (if different) | NO | Full Address | Same as above | Checkbox to enable separate mailing |

**Implementation Note**: All platforms must support optional separate mailing address with checkbox toggle.

---

### 1.3 Emergency and Additional Contacts (Element 9)

| Element # | Field Name | Required | Data Type | Validation | Notes |
|-----------|------------|----------|-----------|------------|-------|
| 9 | Emergency Contact Name | **YES** | String(100) | Non-empty | Full name |
| 9 | Emergency Contact Phone | **YES** | String(15) | Phone format | US/PR format |
| 9 | Emergency Contact Relationship | NO | String(50) | Optional | Relationship to beneficiary |

---

### 1.4 Dependents (Element 18 - Loop Structure)

| Element # | Field Name | Required | Data Type | Validation | Notes |
|-----------|------------|----------|-----------|------------|-------|
| 18 | Dependent Name | If included | String(100) | First, Last | If family enrollment |
| 18 | Dependent Relationship | If included | Enum | Spouse, Child, Other | Relationship to primary |
| 18 | Dependent Date of Birth | If included | Date | MM/DD/YYYY | Valid date |
| 18 | Enrolled in Plan | If included | Boolean | Yes/No | Whether dependent is enrolling |

**Implementation Note**: Support multiple dependents with add/remove functionality.

---

### 1.5 Plan Selection (Elements 1, 13, 14, 20)

| Element # | Field Name | Required | Data Type | Validation | Notes |
|-----------|------------|----------|-----------|------------|-------|
| 1 | MA Plan Name | **YES** | String(100) | Selected from available plans | Triple-S Medicare Advantage plans |
| 1 | Plan Contract Number | **YES** | String(10-15) | Alphanumeric | Assigned by CMS |
| 1 | Plan ID | **YES** | String(10-15) | Alphanumeric | Unique plan identifier |
| 13 | Premium Payment Method | **YES** | Enum | Bank Account, Credit Card, Check, Money Order, Monthly Billing | Payment preference |
| 13 | Bank Account Number | Conditional | String(20) | Numeric, masked | If Bank Account selected |
| 13 | Routing Number | Conditional | String(9) | 9 digits | If Bank Account selected |
| 13 | Credit Card Number | Conditional | String(16) | 16 digits, masked | If Credit Card selected |
| 14 | Long-Term Care Indicator | NO | Boolean | Yes/No | In nursing home or similar |
| 14 | LTC Facility Name | Conditional | String(100) | Non-empty | If in long-term care |
| 20 | Primary Care Provider (PCP) | NO | String(100) | Physician name | PCP selection if required by plan |
| 20 | PCP Clinic Name | NO | String(100) | Clinic/facility | PCP practice location |

---

### 1.6 Current Coverage (Elements 15, 19)

| Element # | Field Name | Required | Data Type | Validation | Notes |
|-----------|------------|----------|-----------|------------|-------|
| 15 | Other Insurance (COB) | NO | Boolean | Yes/No | Coordination of Benefits |
| 15 | Other Coverage Type | Conditional | String(50) | Description | If other insurance exists |
| 15 | Other Coverage Policy # | Conditional | String(30) | Alphanumeric | Policy number |
| 19 | Current MA Plan Member | NO | Boolean | Yes/No | Currently enrolled in MA plan |
| 19 | Current Plan Name | Conditional | String(100) | Text | If currently enrolled |
| 19 | Current Plan Contract # | Conditional | String(15) | Alphanumeric | Current plan identifier |
| 19 | Current Plan ID | Conditional | String(15) | Alphanumeric | Current plan ID |
| 19 | Coverage Start Date | Conditional | Date | MM/DD/YYYY | When current coverage began |
| 19 | Reason for Change | Conditional | Text(500) | Multi-line text | Why changing plans |

---

### 1.7 Special Circumstances (Elements 26, 27)

| Element # | Field Name | Required | Data Type | Validation | Notes |
|-----------|------------|----------|-----------|------------|-------|
| 26 | SNP Eligibility Indicator | Conditional | Boolean | Yes/No | Special Needs Plan qualification |
| 26 | SNP Eligibility Type | Conditional | Enum | Chronic, Dual, Institutional | Type of SNP qualification |
| 26 | Chronic Condition Type | Conditional | String(100) | From CMS approved list | If Chronic SNP |
| 27 | MSA Plan Indicator | Conditional | Boolean | Yes/No | Medical Savings Account plan |
| 27 | MSA Deposit Amount | Conditional | Decimal(10,2) | Positive amount | Annual contribution |
| - | PFFS Plan Indicator | Conditional | Boolean | Yes/No | Private Fee-For-Service |
| - | Dual Eligible (Medicare+Medicaid) | NO | Boolean | Yes/No | Special enrollment rights |
| - | Low Income Subsidy (LIS) | NO | Boolean | Yes/No | Extra Help recipient |
| - | End Stage Renal Disease (ESRD) | NO | Boolean | Yes/No | Special enrollment rights |
| - | Institutional Care Status | NO | Boolean | Yes/No | Open enrollment availability |

---

### 1.8 SEP Information (Not explicitly numbered, but required for SEP enrollment)

| Element # | Field Name | Required | Data Type | Validation | Notes |
|-----------|------------|----------|-----------|------------|-------|
| - | SEP Reason | If SEP | Enum | 16 CMS-approved reasons | See Section 6 |
| - | SEP Event Date | If SEP | Date | MM/DD/YYYY | Date of qualifying event |
| - | SEP Event Description | If SEP | Text(500) | Multi-line | Description of event |
| - | SEP Documentation Path | If SEP | String(500) | File path/URL | Supporting documentation |
| - | Good Cause Status | If SEP | Enum | NotDetermined, Pending, Approved, Denied, WaitingForDocumentation | Good cause determination |
| - | Good Cause Notes | If SEP | Text(1000) | Multi-line | Administrative notes |

---

### 1.9 Language and Accessibility (Element 16)

| Element # | Field Name | Required | Data Type | Validation | Notes |
|-----------|------------|----------|-----------|------------|-------|
| 16 | Language Preference | **YES** | Enum | English, Spanish (es-PR) | Communication language |
| - | Simplified Language Preference | NO | Boolean | Yes/No | Plain language materials |

---

### 1.10 Employer/Union Information (Element 24)

| Element # | Field Name | Required | Data Type | Validation | Notes |
|-----------|------------|----------|-----------|------------|-------|
| 24 | Employer Group Plan | NO | Boolean | Yes/No | Employer/union sponsored |
| 24 | Employer Name | Conditional | String(100) | Non-empty | If employer plan |
| 24 | Union Name | Conditional | String(100) | Non-empty | If union plan |
| 24 | Group Number | Conditional | String(30) | Alphanumeric | Employer group identifier |

---

### 1.11 Electronic Signature (Elements 21-23)

| Element # | Field Name | Required | Data Type | Validation | Notes |
|-----------|------------|----------|-----------|------------|-------|
| 21 | Electronic Signature | **YES** | Binary | Base64 encoded image | Touch/stylus signature capture |
| 22 | Signature Date/Timestamp | **YES** | DateTime | ISO 8601 format | Auto-captured at signing |
| 21 | Printed Name | **YES** | String(100) | Auto-populated | From personal information |
| 21 | Signer Role | **YES** | Enum | Beneficiary, Authorized Representative | Who is signing |
| 23 | Authorized Rep Name | Conditional | String(100) | Non-empty | If not beneficiary signing |
| 23 | Authorized Rep Phone | Conditional | String(15) | Phone format | Contact for auth rep |
| 23 | Authorized Rep Email | Conditional | String(100) | Email format | Contact for auth rep |
| 23 | Authorized Rep Relationship | Conditional | String(50) | Text | Relationship to beneficiary |
| - | Device Information | **YES** | String(200) | Auto-captured | Device model, OS for audit |
| - | IP Address | **YES** | String(45) | IPv4 or IPv6 | Source IP for audit |
| - | GPS Coordinates | NO | String(50) | Lat/Long | Optional location capture |

---

### 1.12 Agent/Broker Information (Element 32)

| Element # | Field Name | Required | Data Type | Validation | Notes |
|-----------|------------|----------|-----------|------------|-------|
| 32 | Agent/Broker NPN Number | Conditional | String(10) | 10 digits | National Producer Number (if assisted) |
| 32 | Agent License Number | Conditional | String(20) | Alphanumeric | State license number |
| 32 | Agent Name | Conditional | String(100) | Non-empty | Full name if assisted |
| 32 | Agent Phone | Conditional | String(15) | Phone format | Agent contact |
| 32 | Agent Email | Conditional | String(100) | Email format | Agent contact |
| 32 | Agency Name | Conditional | String(100) | Text | Brokerage firm |
| 31 | Assisted with Form Completion | **YES** | Boolean | Yes/No | Whether agent/broker assisted |

---

### 1.13 Required Attestations (Elements 28, 29, 30)

| Element # | Field Name | Required | Data Type | Validation | Notes |
|-----------|------------|----------|-----------|------------|-------|
| 28 | Confirms Enrollment Request | **YES** | Boolean | Must be true | CMS attestation language |
| 28 | Understands Plan Benefits | **YES** | Boolean | Must be true | Benefits understanding |
| 28 | Understands Plan Network | **YES** | Boolean | Must be true | Network limitations |
| 28 | Certifies Information Accuracy | **YES** | Boolean | Must be true | Accuracy certification |
| 29 | Authorizes Information Release | **YES** | Boolean | Must be true | HIPAA authorization |
| 29 | Acknowledges Privacy Notice | **YES** | Boolean | Must be true | Privacy rights notification |
| 30 | Accepts Electronic Materials | NO | Boolean | Yes/No | Electronic communications preference |
| 28 | Understands Marketing Guidelines | **YES** | Boolean | Must be true | Marketing rules acknowledgment |
| - | Attestation Timestamp | **YES** | DateTime | ISO 8601 | When attestations accepted |

**Implementation Note**: All attestations must be checked before form submission is allowed. No defaults to "true" - user must actively check each box.

---

### 1.14 Form Metadata (Administrative Elements)

| Element | Field Name | Required | Data Type | Validation | Notes |
|---------|------------|----------|-----------|------------|-------|
| - | Form Type | **YES** | Enum | EnrollmentRequest, Disenrollment, PlanChange, SEP | Type of enrollment |
| - | Form Status | **YES** | Enum | Draft, InProgress, Submitted, Approved, Rejected | Status tracking |
| - | Form Identifier | **YES** | String(50) | UUID or unique ID | Tracking number |
| - | Application Date | **YES** | Date | MM/DD/YYYY | Per CMS Appendix 3 logic |
| - | Enrollment Mechanism | **YES** | Enum | 7 mechanisms per Appendix 3 | How enrollment received |
| - | Submission Timestamp | **YES** | DateTime | ISO 8601 | When form submitted |
| - | Submission Location Type | NO | Enum | Home, Broker Office, Company Office, Rural, Other | Where form completed |
| - | OMB Control Number | **YES** | String(20) | 0938-1378 | OMB form identifier |
| - | OMB Expiration Date | **YES** | Date | 12/31/2026 | Form expiration |
| - | Effective Date | **YES** | Date | MM/DD/YYYY | Coverage start date |
| - | Created Date | **YES** | DateTime | ISO 8601 | When form created |
| - | Last Modified Date | **YES** | DateTime | ISO 8601 | Last edit timestamp |
| - | Form Variant | **YES** | Enum | Standard, Simplified, MSASpecific, PFFSSpecific, PlanChange | Conditional sections |

---

## 2. Enrollment Form Types (4 Required)

All implementations MUST support these four enrollment form types:

### 2.1 Medicare Advantage Enrollment Request (Initial Enrollment)

**Purpose**: Initial enrollment into a Triple-S Medicare Advantage plan  
**CMS Form**: 0938-1378 (Medicare Advantage Individual Enrollment Request Form)  
**Steps**: 5 standard steps (or 6 with SEP)  
**Required Sections**: All 9 sections (Personal Info, Contact, Address, Current Coverage, Plan Selection, Special Circumstances, Agent/Broker, Attestations, Signature)

**Use Cases**:
- New Medicare beneficiary enrolling in MA plan for first time
- Beneficiary switching from Original Medicare to Medicare Advantage
- Beneficiary enrolling during AEP, OEP, or with SEP

**Implementation Requirements**:
- ✅ Complete form with all 32 CMS data elements
- ✅ All 9 sections must be present
- ✅ Electronic signature required
- ✅ Bilingual support (English/Spanish)
- ✅ PDF generation capability
- ✅ CMS 834 XML export capability
- ✅ Draft saving and retrieval
- ✅ Validation before submission

---

### 2.2 Medicare Advantage Disenrollment Request

**Purpose**: Voluntary disenrollment from a Medicare Advantage plan  
**CMS Form**: Medicare Advantage Disenrollment Request Form  
**Steps**: 5 standard steps  
**Required Sections**: Personal Info, Contact, Address, Current Coverage (required), Plan Selection (reason only), Attestations, Signature

**Use Cases**:
- Returning to Original Medicare
- Moving to another MA plan (handled by enrollment into new plan)
- Leaving due to dissatisfaction, relocation, or other reasons

**Implementation Requirements**:
- ✅ Personal information verification
- ✅ Current plan details required (contract #, plan ID, plan name)
- ✅ Reason for disenrollment (required text field)
- ✅ Disenrollment effective date (1st of month)
- ✅ Electronic signature required
- ✅ Disenrollment confirmation notice generation
- ✅ Same validation as enrollment form

**Business Rules**:
- Disenrollment effective first of month after request processed
- Cannot disenroll if only in Medicare Advantage due to ESRD
- Must have other coverage or return to Original Medicare

---

### 2.3 Plan Change Request (Mid-Year Changes)

**Purpose**: Switching from one Medicare Advantage plan to another (same or different carrier)  
**Steps**: 5 standard steps with plan change variant  
**Required Sections**: All 9 sections, with emphasis on Current Coverage and Plan Selection

**Use Cases**:
- Switching between Triple-S MA plans
- Switching from another carrier to Triple-S during allowed periods
- Plan-initiated plan changes due to contract terminations

**Implementation Requirements**:
- ✅ Current plan information required (all fields)
- ✅ Reason for plan change required
- ✅ New plan selection required
- ✅ Coverage transition timeline displayed
- ✅ Effective date handling (first of month rules)
- ✅ Plan availability validation by ZIP code
- ✅ Plan Change confirmation notice

**Special Fields**:
- `FormVariant.PlanChangeSpecific` sections activated
- Transition timeline information
- Premium comparison (if available)
- Network comparison information

**Business Rules**:
- Must be during AEP, MA OEP, or valid SEP
- New plan must be available in beneficiary's service area
- Cannot have gap in coverage

---

### 2.4 Special Enrollment Period (SEP) Enrollment

**Purpose**: Mid-year enrollment or plan change using a Special Enrollment Period qualifying event  
**Steps**: 6 steps (SEP reason step + 5 standard steps)  
**Required Sections**: All 9 sections PLUS SEP-specific section

**Use Cases**:
- Moving out of plan service area
- Loss of other coverage (employer, Medicaid, etc.)
- Dual eligible status changes
- Chronic condition SEP
- Five-star plan enrollment
- Other qualifying life events

**Implementation Requirements**:
- ✅ SEP reason selection (dropdown with 16 CMS reasons)
- ✅ Qualifying event date capture (required)
- ✅ Qualifying event description (text field)
- ✅ 60-day window validation (from event date)
- ✅ Supporting documentation upload capability
- ✅ Good Cause determination tracking
- ✅ Good Cause status (NotDetermined, Pending, Approved, Denied, WaitingForDocumentation)
- ✅ Good Cause determination notice generation

**SEP-Specific Validations**:
- Event date must be within last 60 days or up to 2 months in future (for anticipated events)
- Documentation required for most SEP reasons
- Good Cause determination required if outside standard SEP window

**Business Rules**:
- SEP enrollment effective date rules vary by reason
- Some SEPs allow enrollment month-of or first-of-next-month
- Good Cause SEPs have special effective date rules

---

## 3. Form Sections (9 Required per CMS Form 0938-1378)

All enrollment forms MUST include these 9 sections:

### Section 1: Personal Information
**Required Fields**: First Name, Last Name, Date of Birth, Gender, Social Security Number, Medicare Number  
**Optional Fields**: Middle Initial, Medicare Card Image  
**Validation**: All required fields must be non-empty, DOB must indicate 18+, SSN format, Medicare # format

---

### Section 2: Contact Information
**Required Fields**: Primary Phone, Preferred Contact Method  
**Optional Fields**: Secondary Phone, Email Address  
**Validation**: Phone format (XXX) XXX-XXXX, email format if provided

---

### Section 3: Address Information
**Required Fields**: Street Address 1, City, State/Territory, ZIP Code  
**Optional Fields**: Street Address 2, Mailing Address (if different)  
**Special Feature**: Checkbox to enable separate mailing address  
**Validation**: All required address fields, ZIP code format (5-digit or ZIP+4)

---

### Section 4: Current Medicare Coverage
**Optional Fields**: Current Plan Name, Contract Number, Plan ID, Coverage Start Date, Reason for Change  
**Conditional**: Becomes required if form type is Disenrollment or PlanChange  
**Validation**: If changing plans, reason required

---

### Section 5: Plan Selection
**Required Fields**: Desired Plan Name, Premium Payment Method, Effective Date  
**Optional Fields**: PCP Selection (if required by plan type), Long-Term Care information  
**Validation**: Plan must be available in beneficiary's ZIP code, effective date must be 1st of month and future date, payment method details if bank/card selected

---

### Section 6: Special Circumstances
**Optional Fields**: SEP Reason (if applicable), Dual Eligible checkbox, LIS checkbox, ESRD checkbox, Institutional Care checkbox  
**Special Features**: Plan type indicators (MSA, PFFS, SNP), conditional fields based on plan type  
**Validation**: If SEP selected, event date and description required

---

### Section 7: Agent/Broker Information
**Conditional Fields**: All agent/broker fields become required IF "Assisted with Form Completion" = Yes  
**Required if Assisted**: NPN Number, Agent License, Agent Name, Agent Phone  
**Optional if Assisted**: Agent Email, Agency Name  
**Validation**: NPN must be 10 digits, license number format

---

### Section 8: Attestations and Agreements
**Required Checkboxes**: 
- ✅ Enrollment Agreement (confirms enrollment request)
- ✅ Privacy Notice Acknowledgment
- ✅ Marketing Guidelines Understanding
- ✅ Information Accuracy Certification
- ✅ Authorizes Information Release

**Optional Checkboxes**:
- Electronic Materials Acceptance

**Validation**: All required attestations must be checked before submission allowed

---

### Section 9: Electronic Signature
**Required Elements**: Signature pad, Printed name (auto-populated), Signature date (auto-captured)  
**Audit Trail**: Device information, IP address, timestamp  
**Optional Elements**: GPS coordinates, Authorized Representative information (if not beneficiary signing)  
**Validation**: Signature must be captured (not empty), if auth rep signing, auth rep details required

---

## 4. Plan Types (5 Required)

All implementations MUST support these 5 Medicare Advantage plan types:

### 4.1 Standard MA (Medicare Advantage)
**Description**: Traditional Medicare Advantage HMO or PPO plans  
**Form Variant**: Standard  
**Special Sections**: None (baseline form)  
**Network Requirements**: PCP selection may be required

---

### 4.2 MSA (Medical Savings Account)
**Description**: High-deductible health plan with savings account  
**Form Variant**: MSASpecific  
**Special Sections**:
- ✅ MSA deposit amount
- ✅ HSA/MSA account information
- ✅ Tax-deductible contributions information
- ✅ High-deductible plan acknowledgment

**Special Requirements**:
- Element 27 fields activated
- MSA-specific attestations
- Bank account information required for deposits

---

### 4.3 PFFS (Private Fee-For-Service)
**Description**: Medicare Advantage plan without network restrictions  
**Form Variant**: PFFSSpecific  
**Special Sections**:
- ✅ PFFS plan characteristics acknowledgment
- ✅ Network-free enrollment information
- ✅ Direct contracting with providers emphasis

**Special Requirements**:
- PFFS indicator set to true
- PCP selection not required
- Special attestation about provider acceptance

---

### 4.4 SNP (Special Needs Plan)
**Description**: Plans for specific populations (Dual, Chronic, Institutional)  
**Form Variant**: Standard (with SNP sections activated)  
**Special Sections**:
- ✅ SNP eligibility type (Dual, Chronic, Institutional)
- ✅ Chronic condition selection (if C-SNP)
- ✅ Dual eligible verification (if D-SNP)
- ✅ Institutional status (if I-SNP)

**Special Requirements**:
- Element 26 fields activated
- SNP eligibility documentation may be required
- Special attestations about eligibility

**SNP Types**:
1. **D-SNP (Dual Eligible)**: Medicare + Medicaid
2. **C-SNP (Chronic Condition)**: Specific chronic conditions from CMS list
3. **I-SNP (Institutional)**: Nursing home or similar facility residents

---

### 4.5 Other
**Description**: Catch-all for non-standard or future plan types  
**Form Variant**: Standard  
**Special Sections**: None or custom as needed  
**Usage**: Rarely used, for plans that don't fit other categories

---

## 5. Form Variants (Conditional Sections)

Form variants activate conditional sections based on form type or plan selection:

### 5.1 Standard Variant
**Trigger**: All forms by default  
**Sections**: All 9 baseline sections  
**Usage**: Most enrollments

---

### 5.2 Simplified Variant
**Trigger**: User selects "Plain Language" option  
**Sections**: Same 9 sections with simplified language  
**Changes**:
- Shorter field labels
- Plain English instead of technical terms
- Simpler instructions
- Larger fonts (accessibility)

**Compliance**: PLAIN Act requirements for federal forms

---

### 5.3 MSASpecific Variant
**Trigger**: Plan Type = MSA  
**Additional Sections**:
- MSA contribution amount
- Bank account for MSA deposits
- High-deductible plan acknowledgments
- Tax implications notice

**Required Attestations**:
- Understands MSA contribution limits
- Acknowledges high deductible
- Agrees to bank account deposits

---

### 5.4 PFFSSpecific Variant
**Trigger**: Plan Type = PFFS  
**Additional Sections**:
- PFFS plan characteristics
- Provider acceptance acknowledgment
- Network-free benefits explanation

**Required Attestations**:
- Understands providers can reject PFFS terms
- Acknowledges no network restrictions
- Understands direct contracting process

---

### 5.5 PlanChangeSpecific Variant
**Trigger**: Form Type = PlanChange  
**Additional Sections**:
- Current plan details (all fields required)
- Reason for change (required)
- Coverage transition timeline
- Premium comparison
- Network comparison

**Emphasis**:
- Effective date calculation
- No gap in coverage assurance
- Benefits comparison information

---

## 6. Special Enrollment Periods (16 CMS Reasons)

All implementations MUST support these 16 CMS-approved SEP reasons:

| # | SEP Reason | Description | Documentation Required | Effective Date Rules |
|---|------------|-------------|------------------------|---------------------|
| 0 | None | Not applying for SEP (filter out) | N/A | Standard AEP/OEP rules |
| 1 | Moved out of plan area | Relocated outside plan's service area | Proof of address | Month of move or following month |
| 2 | Loss of other coverage - Death | Other coverage ended due to death of policyholder | Death certificate | Month of event or next month |
| 3 | Loss of other coverage - Divorce | Other coverage ended due to divorce | Divorce decree | Month of event or next month |
| 4 | Loss of other coverage - Employer | Lost employer/union coverage | COBRA notice or termination letter | Month of loss or next month |
| 5 | Dual eligible status change | Became or lost Medicaid eligibility | Medicaid card or termination notice | Medicaid effective date |
| 6 | Chronic condition SEP | Have chronic condition from CMS list | Physician documentation | Month of request or next month |
| 7 | Five-star plan available | Enrolling in 5-star rated MA plan | Plan rating documentation (auto) | Month of request or next month |
| 8 | Plan contract termination | Plan involuntarily ending contract | CMS notification (auto) | Plan end date |
| 9 | Plan service area reduction | Plan reducing service area | CMS notification (auto) | Reduction effective date |
| 10 | Other qualifying event - Marriage | Got married | Marriage certificate | Month of marriage or next |
| 11 | Other qualifying event - Newborn | Newborn or adopted child | Birth/adoption certificate | Birth/adoption date |
| 12 | Institutional status change | Moved into/out of institution | Facility documentation | Month of change or next |
| 13 | Loss of creditable coverage | Lost creditable Rx coverage | Notice from coverage provider | Month of loss or next |
| 14 | Natural disaster | Affected by federal disaster | FEMA declaration (auto) | Disaster declaration date |
| 15 | Released from incarceration | Released from jail/prison | Release documentation | Month of release or next |
| 16 | Plan marketing violation | Plan committed marketing violation | CMS determination | Month of request |

### SEP Business Rules

**60-Day Enrollment Window**: Most SEPs allow enrollment within 60 days of the qualifying event

**Effective Date Logic**:
- **Month-of enrollment**: If enroll during same month as event (coverage starts same month)
- **Retroactive**: Some SEPs allow retroactive coverage to event date
- **Prospective**: Most SEPs allow coverage starting 1st of month after enrollment

**Good Cause Determination**: If enrollment is outside standard SEP window, beneficiary can request Good Cause determination

**Good Cause Reasons**:
- Misrepresentation by plan or agent
- Natural disaster
- Serious illness
- Provided wrong information by CMS or plan
- Other circumstances beyond beneficiary's control

---

## 7. Enrollment Periods

All implementations MUST enforce these CMS enrollment period rules:

### 7.1 AEP (Annual Coordinated Election Period)
**Dates**: October 15 - December 7 (every year)  
**Coverage Effective**: January 1 of following year  
**Usage**: Primary enrollment period for most beneficiaries  
**Allowed Actions**:
- ✅ Enroll in MA plan
- ✅ Switch MA plans
- ✅ Disenroll from MA and return to Original Medicare
- ✅ Add/drop Part D (if standalone)

**Implementation**: System should highlight AEP dates, auto-calculate January 1 effective date

---

### 7.2 MA OEP (Medicare Advantage Open Enrollment Period)
**Dates**: January 1 - March 31 (every year)  
**Coverage Effective**: 1st of month after enrollment  
**Usage**: Limited to MA-to-MA changes or MA-to-Original Medicare  
**Restrictions**: Can only use ONCE per year  
**Allowed Actions**:
- ✅ Switch from one MA plan to another MA plan
- ✅ Disenroll from MA and return to Original Medicare
- ✅ Add standalone Part D (if returning to Original Medicare)
- ❌ Enroll in MA from Original Medicare (must use AEP or SEP)

**Implementation**: Track MA OEP usage per beneficiary per year, prevent multiple uses

---

### 7.3 ICEP (Initial Coverage Election Period)
**Dates**: 3 months before 65th birthday, birth month, 3 months after (7-month window)  
**Coverage Effective**: Varies by enrollment month  
**Usage**: For new Medicare beneficiaries  
**Allowed Actions**:
- ✅ Enroll in MA plan
- ✅ Enroll in Part D
- ✅ Stay with Original Medicare

**Effective Date Rules**:
- Enroll 3 months before: Coverage starts birthday month
- Enroll during birthday month: Coverage starts following month
- Enroll after birthday month: Coverage delayed 1-3 months

**Implementation**: Auto-detect if within ICEP based on DOB, calculate effective date

---

### 7.4 SEP (Special Enrollment Period)
**Dates**: Year-round, triggered by qualifying events  
**Coverage Effective**: Varies by SEP reason  
**Usage**: Mid-year enrollment or changes due to life events  
**Allowed Actions**: Depends on SEP reason (see Section 6)

**Implementation**: Require SEP reason selection, event date, validate 60-day window

---

### 7.5 Continuous Enrollment for Special Populations
**Dual Eligible**: Can change plans monthly  
**Institutionalized**: Can change plans monthly  
**LIS Recipients**: Can change Part D plans monthly  
**Implementation**: Auto-enable monthly election if special population status = true

---

### 7.6 Application Date Calculation (CMS Appendix 3)

**7 Enrollment Mechanisms** (determines Application Date):

1. **Paper application received by mail**
   - Application Date = Postmark date (or received date if no postmark)

2. **Paper application hand-delivered**
   - Application Date = Date received

3. **Electronic application (online)**
   - Application Date = Date electronically signed

4. **Telephonic enrollment**
   - Application Date = Date of call with signed telephonic verification

5. **In-person enrollment**
   - Application Date = Date signed in presence of agent/broker

6. **Fax application**
   - Application Date = Date/time fax received

7. **Plan-to-plan direct enrollment**
   - Application Date = Date plan receives complete application

**Implementation**: Track enrollment mechanism, calculate application date per rules, determine effective date based on application date and current enrollment period

---

## 8. Special Populations

All implementations MUST identify and handle these special populations:

### 8.1 Dual Eligible Individuals
**Definition**: Eligible for both Medicare and Medicaid  
**Special Rights**:
- ✅ Can change MA plans monthly
- ✅ Can change Part D plans monthly
- ✅ Qualify for Low Income Subsidy (Extra Help)

**Form Field**: `isDualEligible` checkbox in Special Circumstances section  
**Validation**: If checked, may qualify for multiple SEPs  
**Notices**: Special notices about monthly election rights

---

### 8.2 Low Income Subsidy (LIS) Recipients
**Definition**: Receive Extra Help with prescription drug costs  
**Special Rights**:
- ✅ Can change Part D plans monthly
- ✅ No late enrollment penalty
- ✅ Reduced or eliminated premiums

**Form Field**: `hasLowIncomeSubsidy` checkbox  
**Validation**: May auto-qualify if Dual Eligible  
**Notices**: Information about Extra Help benefits

---

### 8.3 End Stage Renal Disease (ESRD) Patients
**Definition**: Permanent kidney failure requiring dialysis or transplant  
**Special Rights**:
- ✅ Special enrollment rights during limited periods
- ✅ Can enroll in certain MA plans
- ✅ Protected from disenrollment in some cases

**Form Field**: `hasEndStageRenalDisease` checkbox  
**Validation**: ESRD status affects plan eligibility  
**Restrictions**: Some MA plans don't accept ESRD enrollees (except during specific periods)

**Business Rule**: If ESRD and already in MA plan, may not be able to voluntarily disenroll

---

### 8.4 Institutionalized Individuals
**Definition**: Reside in long-term care facility (nursing home, etc.)  
**Special Rights**:
- ✅ Can change MA plans monthly
- ✅ Open enrollment year-round
- ✅ Special considerations for facility participation

**Form Field**: `isInInstitutionalCare` checkbox  
**Additional Fields**: LTC facility name (if in long-term care)  
**Validation**: Facility must be in plan's network or plan must cover institutional care

---

### 8.5 Part D Low-Income Subsidy (LIS) Auto-Enrollment
**Definition**: Beneficiaries automatically enrolled by CMS  
**Handling**: May receive auto-enrolled beneficiaries who want to change plans  
**Form Type**: Typically Plan Change or SEP enrollment

---

## 9. Validation Rules

All implementations MUST enforce these validation rules:

### 9.1 Field-Level Validation Rules

| Field | Rule | Format | Error Message (EN) | Error Message (ES) |
|-------|------|--------|-------------------|-------------------|
| First Name | Required, alphabetic, 1-50 chars | String | "First name is required" | "El nombre es requerido" |
| Last Name | Required, alphabetic, 1-50 chars | String | "Last name is required" | "El apellido es requerido" |
| Date of Birth | Required, age 18+, not future, > 1900 | MM/DD/YYYY | "Must be 18 or older" | "Debe tener 18 años o más" |
| SSN | 9 digits, format XXX-XX-XXXX | String(9) | "Social Security Number must be 9 digits" | "El Número de Seguro Social debe tener 9 dígitos" |
| Medicare Number | Required, 11+ alphanumeric | String(11-15) | "Invalid Medicare Number format" | "Formato de Número de Medicare inválido" |
| Primary Phone | Required, 10-15 digits | (XXX) XXX-XXXX | "Primary phone is required" | "El teléfono principal es requerido" |
| Email | Valid email format (if provided) | name@domain.com | "Please provide a valid email address" | "Por favor proporcione una dirección de email válida" |
| Street Address 1 | Required, 5-100 chars | String | "Street address is required" | "La dirección es requerida" |
| City | Required, 2-50 chars | String | "City is required" | "La ciudad es requerida" |
| State | Required, 2-letter code | String(2) | "State is required" | "El estado es requerido" |
| ZIP Code | Required, 5-digit or ZIP+4 | NNNNN or NNNNN-NNNN | "ZIP code must be 5 digits" | "El código postal debe tener 5 dígitos" |
| Plan Selection | Required, from available list | Enum | "Please select a plan" | "Por favor seleccione un plan" |
| Effective Date | Required, 1st of month, future date | MM/DD/YYYY | "Effective date must be the first of the month" | "La fecha efectiva debe ser el primero del mes" |
| Payment Method | Required, valid enum | Enum | "Payment method is required" | "El método de pago es requerido" |
| Electronic Signature | Required, not empty | Binary | "Electronic signature is required to submit" | "La firma electrónica es requerida para enviar" |

### 9.2 Form-Level Validation Rules

**All Required Fields Validation**:
```
Rule: Before submission, all fields marked "Required" must have values
Error: "Please complete all required fields before submitting"
Error (ES): "Por favor complete todos los campos requeridos antes de enviar"
```

**All Attestations Validation**:
```
Rule: All required attestation checkboxes must be checked
Error: "You must agree to all attestations before submitting"
Error (ES): "Debe aceptar todas las certificaciones antes de enviar"
```

**Dependent Information Validation**:
```
Rule: If any dependent field is filled, all dependent required fields must be completed
Error: "Please complete all dependent information or remove the dependent"
Error (ES): "Por favor complete toda la información del dependiente o elimínelo"
```

**Conditional Field Validation**:
```
Rule: If "Mailing Address Different" checked, all mailing address fields required
Rule: If "Other Insurance" = Yes, other coverage details required
Rule: If "In LTC" = Yes, facility name required
Rule: If payment method = "Bank Account", account and routing numbers required
Rule: If "Assisted by Agent" = Yes, all agent fields required
Error: "Please complete all required fields for your selection"
Error (ES): "Por favor complete todos los campos requeridos para su selección"
```

---

### 9.3 Business Rule Validations

**Enrollment Period Validation**:
```
Rule: Enrollment must be during valid enrollment period OR have valid SEP
Check: Current date within AEP, MA OEP, ICEP, or valid SEP with event date
Error: "Enrollment is only available during Open Enrollment Period (October 15 - December 7) or with a qualifying Special Enrollment Period"
Error (ES): "La inscripción solo está disponible durante el Período de Inscripción Abierta (15 de octubre - 7 de diciembre) o con un Período de Inscripción Especial calificado"
```

**SEP 60-Day Window Validation**:
```
Rule: For SEP enrollment, event date must be within 60 days prior OR up to 2 months future (anticipated events)
Check: (Current Date - Event Date) <= 60 days OR (Event Date - Current Date) <= 60 days
Error: "Special Enrollment Period event must have occurred within the last 60 days"
Error (ES): "El evento del Período de Inscripción Especial debe haber ocurrido en los últimos 60 días"
```

**Plan Availability Validation**:
```
Rule: Selected plan must be available in beneficiary's ZIP code
Check: Query plan availability table with selected plan ID and beneficiary ZIP
Error: "Selected plan is not available in your ZIP code"
Error (ES): "El plan seleccionado no está disponible en su código postal"
```

**Duplicate Enrollment Validation**:
```
Rule: Beneficiary cannot have active enrollment for current year
Check: Query enrollment database for beneficiary Medicare # and current year
Error: "You already have an active enrollment for this year"
Error (ES): "Ya tiene una inscripción activa para este año"
```

**MA OEP Usage Validation**:
```
Rule: Can only use MA OEP once per year
Check: Query enrollment history for MA OEP usage in current year
Error: "Medicare Advantage Open Enrollment Period can only be used once per year"
Error (ES): "El Período de Inscripción Abierta de Medicare Advantage solo se puede usar una vez al año"
```

**Effective Date Validation**:
```
Rule: Effective date must be 1st of month and cannot be in the past
Check: Day = 1 AND date >= today
Error: "Effective date must be the first of the month and cannot be in the past"
Error (ES): "La fecha efectiva debe ser el primero del mes y no puede ser en el pasado"
```

**Age Validation**:
```
Rule: Beneficiary must be at least 18 years old (typically 65 for Medicare, but 18 for disability)
Check: (Current Date - Date of Birth) >= 18 years
Error: "Beneficiary must be at least 18 years old"
Error (ES): "El beneficiario debe tener al menos 18 años"
```

**Medicare Number Uniqueness**:
```
Rule: Medicare Number must be unique (not already enrolled in same plan)
Check: Query enrollment database for combination of Medicare # and Plan ID
Error: "This Medicare Number is already enrolled in the selected plan"
Error (ES): "Este Número de Medicare ya está inscrito en el plan seleccionado"
```

---

### 9.4 Real-Time Validation Requirements

**Implementation**: All platforms MUST provide real-time validation feedback:

✅ **On Field Blur**: Validate individual field immediately when user leaves field  
✅ **On Form Submit**: Validate entire form before allowing submission  
✅ **Visual Indicators**: Red border or icon for invalid fields, green for valid  
✅ **Error Messages**: Display specific error message near invalid field  
✅ **Error Summary**: Show list of all errors at top of form on submit attempt  
✅ **Scroll to Error**: Auto-scroll to first error field on validation failure

---

## 10. Business Rules

All implementations MUST enforce these business rules:

### 10.1 Enrollment Period Rules

**AEP Period**:
- Dates: October 15 - December 7 annually
- Effective Date: Always January 1 of following year
- When to use: Primary enrollment period for plan changes

**MA OEP Period**:
- Dates: January 1 - March 31 annually
- Effective Date: 1st of month after enrollment processed
- When to use: MA-to-MA changes only, once per year
- Restriction: Cannot be used to enroll from Original Medicare

**ICEP**:
- Duration: 7 months (3 before birthday month, birthday month, 3 after)
- Effective Date: Varies based on enrollment month
- When to use: For new Medicare beneficiaries turning 65

**SEP**:
- Duration: 60 days from qualifying event (most reasons)
- Effective Date: Varies by SEP reason
- When to use: Qualifying life events or circumstances

---

### 10.2 Coverage Rules

**No Gap in Coverage**:
- When changing plans, new coverage must start same month old coverage ends
- System should auto-calculate to ensure no gap

**Cannot Have Both MA and Medicare Supplement (Medigap)**:
- If enrolling in MA, any Medigap policy should be noted (may not be able to re-enroll)
- Warning message if transitioning from Medigap

**Part D Coverage with MA**:
- Most MA plans include Part D (MA-PD plans)
- If enrolling in MA-PD, must disenroll from standalone Part D
- System should check for existing Part D and warn

---

### 10.3 Plan Eligibility Rules

**Service Area Requirement**:
- Beneficiary must reside in plan's service area
- Validate ZIP code against plan's service ZIP codes
- Moving out of service area = valid SEP reason

**ESRD Restrictions**:
- If ESRD status = Yes, check plan's ESRD acceptance policy
- Some plans only accept ESRD during specific enrollment periods
- Provide clear messaging if plan not available

**Employer Group Plans**:
- If employer group plan selected, additional validation may be required
- Group number must match employer's contract

**Institutional Coverage**:
- If institutionalized, plan must contract with facility
- Verify facility is in plan's network or plan covers institutional care

---

### 10.4 Payment Method Rules

**Bank Account Draft**:
- Routing number must be 9 digits
- Account number must be valid format
- Validate routing number against bank registry (if available)
- Warning: First payment may take 1-2 billing cycles to process

**Credit/Debit Card**:
- Card number must be 16 digits (or 15 for Amex)
- Expiration date must be future date
- CVV required (3-4 digits)
- PCI-DSS compliance required for storage/transmission

**Check/Money Order**:
- Provide mailing address for payment
- Warning: Late payments may result in coverage termination

---

### 10.5 Special Population Rules

**Dual Eligible**:
- Auto-qualified for LIS (Extra Help)
- Monthly election rights enabled
- May be auto-enrolled by CMS in benchmark plan
- Can change plans with minimal restrictions

**Institutionalized**:
- Monthly election rights enabled
- Can change plans any time
- Facility network participation validated

**LIS Recipients**:
- Monthly election for Part D changes
- Premium subsidy auto-applied
- No late enrollment penalty

---

### 10.6 Agent/Broker Rules

**Scope of Appointment (SOA) Required**:
- Before agent can assist with enrollment
- Must be documented (separate from enrollment form)
- System should track SOA status

**Agent Licensing**:
- Agent must be licensed in state/territory (Puerto Rico for Triple-S)
- NPN number must be valid and active
- System should validate NPN against NIPR database (if integrated)

**Commission Eligibility**:
- Agent only receives commission if assisted with enrollment
- "Assisted with Form Completion" flag must be checked

---

### 10.7 Data Retention Rules

**7-Year Retention**:
- All Medicare enrollment records must be retained for 7 years
- Includes: Enrollment forms, signatures, supporting documentation
- Applies to: Paper records, electronic records, audit trails

**Audit Trail Required**:
- System must log all changes to enrollment records
- Who, what, when for every modification
- Immutable audit log (cannot be edited or deleted)

---

## 11. Bilingual Support (English/Spanish)

All implementations MUST provide full bilingual support for English and Spanish (Puerto Rico dialect):

### 11.1 Language Requirements

**Languages**: 
- English (en-US) - PRIMARY
- Spanish (es-PR) - Puerto Rico dialect

**Language Selection**:
- User must be able to select language preference
- Language preference tracked in Element 16 (Language Preference)
- Option to switch language at any time during enrollment
- Language selection persists across sessions

**Element 16 Implementation**:
```
Field: LanguagePreference
Type: Enum
Values: "English" | "Spanish"
Default: Based on browser/system locale, with option to change
```

---

### 11.2 Translation Requirements

**What Must Be Translated**:

✅ **UI Labels and Buttons**: All form field labels, button text, headings
✅ **Error Messages**: All validation error messages (field-level and form-level)
✅ **Help Text**: All tooltips, instructions, guidance text
✅ **Attestation Language**: All CMS attestation text (using official CMS Spanish translations)
✅ **Notices**: Enrollment acknowledgment, confirmation, Good Cause notices
✅ **Section Headers**: All 9 section headers
✅ **Dropdown Options**: All dropdown/select options
✅ **Validation Messages**: Real-time validation feedback

**What Should NOT Be Translated**:
- Beneficiary-entered data (names, addresses, etc.)
- Technical codes (plan IDs, contract numbers, Medicare numbers)
- Form identification numbers

---

### 11.3 CMS Attestation Language

**Source**: Use official CMS Spanish translations from CMS Form 0938-1378 Spanish version

**Required Attestations** (English | Spanish):

1. **Enrollment Agreement**:
   - EN: "I confirm that I am requesting enrollment in the plan indicated above."
   - ES: "Confirmo que estoy solicitando inscribirme en el plan indicado anteriormente."

2. **Privacy Notice**:
   - EN: "I understand that my signature allows this organization to release my health information to Medicare."
   - ES: "Entiendo que mi firma permite que esta organización divulgue mi información de salud a Medicare."

3. **Information Accuracy**:
   - EN: "I certify that the information I have provided is correct to the best of my knowledge."
   - ES: "Certifico que la información que he proporcionado es correcta según mi leal saber y entender."

4. **Marketing Guidelines**:
   - EN: "I understand the marketing guidelines and disclosure requirements."
   - ES: "Entiendo las pautas de mercadeo y los requisitos de divulgación."

---

### 11.4 Error Message Translations

**Common Error Messages** (from Section 9.1):

| English | Spanish (es-PR) |
|---------|----------------|
| "This field is required" | "Este campo es requerido" |
| "Please provide a valid email address" | "Por favor proporcione una dirección de email válida" |
| "Social Security Number must be 9 digits" | "El Número de Seguro Social debe tener 9 dígitos" |
| "Invalid Medicare Number format" | "Formato de Número de Medicare inválido" |
| "Must be 18 or older" | "Debe tener 18 años o más" |
| "Date cannot be in the future" | "La fecha no puede ser en el futuro" |
| "Please select a plan from the available options" | "Por favor seleccione un plan de las opciones disponibles" |
| "Electronic signature is required to submit the form" | "La firma electrónica es requerida para enviar el formulario" |
| "Effective date must be the first of the month" | "La fecha efectiva debe ser el primero del mes" |
| "Plan not available in your ZIP code" | "El plan no está disponible en su código postal" |
| "Enrollment is only available during Open Enrollment Period (October 15 - December 7)" | "La inscripción solo está disponible durante el Período de Inscripción Abierta (15 de octubre - 7 de diciembre)" |
| "You already have an active enrollment for this year" | "Ya tiene una inscripción activa para este año" |

---

### 11.5 Notice Generation

**All Generated Notices Must Be Bilingual**:

1. **Enrollment Acknowledgment Notice**
   - Generated: Upon form submission
   - Contains: Form details, submission date, next steps
   - Format: PDF with side-by-side English/Spanish OR separate EN/ES PDFs

2. **Enrollment Confirmation Notice**
   - Generated: Upon approval
   - Contains: Effective date, plan details, member ID
   - Format: PDF with side-by-side English/Spanish OR separate EN/ES PDFs

3. **Good Cause Determination Notice**
   - Generated: For SEP Good Cause requests
   - Contains: Request details, determination status, appeal rights
   - Format: PDF with side-by-side English/Spanish OR separate EN/ES PDFs

4. **Disenrollment Confirmation Notice**
   - Generated: Upon disenrollment approval
   - Contains: Disenrollment effective date, coverage end date, next steps
   - Format: PDF with side-by-side English/Spanish OR separate EN/ES PDFs

**Implementation**: Generate notices in selected language preference (Element 16), with option to receive both languages

---

### 11.6 Implementation Guidelines

**String Resources**:
- Store all translatable strings in resource files (not hard-coded)
- Windows: RESX files (Resources.resx, Resources.es.resx)
- Android: strings.xml (values/strings.xml, values-es/strings.xml)
- Format: Key-value pairs with descriptive keys

**Example** (Android):
```xml
<!-- values/strings.xml (English) -->
<string name="field_first_name">First Name</string>
<string name="error_required_field">This field is required</string>
<string name="button_submit">Submit Enrollment</string>

<!-- values-es/strings.xml (Spanish) -->
<string name="field_first_name">Nombre</string>
<string name="error_required_field">Este campo es requerido</string>
<string name="button_submit">Enviar Inscripción</string>
```

**Dynamic Text**:
- Use string formatting for dynamic content
- Example: "Coverage effective {date}" / "Cobertura efectiva el {date}"

**Date/Number Formatting**:
- Dates: MM/DD/YYYY for both languages (US standard for Puerto Rico)
- Currency: $X,XXX.XX format (US dollar)
- Phone: (XXX) XXX-XXXX format

**Right-to-Left (RTL) Support**:
- Not required (both English and Spanish are LTR languages)

---

## 12. Electronic Signature Requirements

All implementations MUST meet federal e-signature standards:

### 12.1 Federal E-Sign Act Compliance

**Requirements** (per Electronic Signatures in Global and National Commerce Act):

✅ **Intent to Sign**: User must clearly intend to sign (action button "Sign" or "I Agree")
✅ **Consent to Electronic Records**: User must consent to electronic format (attestation)
✅ **Association with Record**: Signature must be associated with specific enrollment form
✅ **Retention**: Signed record must be retained in accessible format
✅ **Audit Trail**: Must capture who, what, when, where, how

---

### 12.2 Signature Capture Requirements

**Method**: Touch or stylus-based signature capture

**Technical Requirements**:
- ✅ Canvas or signature pad component
- ✅ Smooth stroke rendering (minimum 60fps)
- ✅ Variable line width based on pressure (if supported)
- ✅ Black ink on white background (for clarity)
- ✅ Minimum canvas size: 300x150 pixels
- ✅ Responsive sizing (adapt to device screen)

**User Controls**:
- ✅ "Clear" button to reset signature
- ✅ Visual indication when signature is captured
- ✅ Cannot submit form until signature captured

**Accessibility**:
- ✅ Keyboard accessibility (for users unable to use touch/mouse)
- ✅ Alternative: Type full name as electronic signature option
- ✅ Screen reader compatible

---

### 12.3 Signature Data Storage

**Format**: Base64-encoded PNG or JPEG image

**Resolution**: Minimum 300 DPI for print quality

**Storage**:
- Database: Store Base64 string in text field
- File System: Store as image file with reference in database
- Both: Store image file AND Base64 in database for redundancy

**Example Structure**:
```json
{
  "signatureBase64": "iVBORw0KGgoAAAANSUhEUg...",
  "signatureFilePath": "/signatures/2026/02/enrollment_12345_signature.png",
  "signatureMimeType": "image/png",
  "signatureResolution": "300dpi"
}
```

---

### 12.4 Audit Trail Requirements

**Required Audit Data** (Elements 21-23 + additional):

| Audit Field | Required | Source | Example |
|-------------|----------|--------|---------|
| Signature Timestamp | **YES** | System clock | 2026-02-05T14:23:15-04:00 |
| Printed Name | **YES** | Auto-populated from Element 2 | "Maria Rodriguez" |
| Signer Role | **YES** | User selection | "Beneficiary" or "Authorized Representative" |
| IP Address | **YES** | Network request | "192.168.1.100" or "2001:db8::1" |
| Device Type | **YES** | User agent | "Samsung Galaxy Fold 5" |
| Operating System | **YES** | User agent | "Android 14" |
| Browser/App Version | **YES** | User agent | "Chrome 120.0.6099.199" or "Triple-S AEP v2.3.1" |
| GPS Coordinates | NO | Device GPS (with permission) | "18.4655° N, 66.1057° W" |
| Session ID | **YES** | Application session | "sess_abc123xyz789" |
| Form ID | **YES** | Enrollment form identifier | "ENR-2026-000123" |
| Signature Method | **YES** | System | "Touch", "Mouse", "TypedName" |

**Audit Trail Storage**:
- Store as JSON or XML structure
- Immutable (append-only, no edits)
- Linked to enrollment form record
- Include in form exports (834 XML metadata)

**Example Audit Trail JSON**:
```json
{
  "signatureAudit": {
    "timestamp": "2026-02-05T14:23:15-04:00",
    "printedName": "Maria Rodriguez",
    "signerRole": "Beneficiary",
    "ipAddress": "192.168.1.100",
    "deviceType": "Samsung Galaxy Fold 5",
    "operatingSystem": "Android 14",
    "browserApp": "Triple-S AEP Android v2.3.1",
    "gpsCoordinates": "18.4655,-66.1057",
    "sessionId": "sess_abc123xyz789",
    "formId": "ENR-2026-000123",
    "signatureMethod": "Touch"
  }
}
```

---

### 12.5 Authorized Representative Signatures

**When Required**: If beneficiary cannot sign due to:
- Physical inability
- Mental incapacity
- Legal guardianship
- Power of attorney

**Additional Fields Required** (Element 23):
- ✅ Authorized Representative Name
- ✅ Relationship to Beneficiary
- ✅ Authorized Representative Phone
- ✅ Authorized Representative Email
- ✅ Reason beneficiary cannot sign (dropdown)
- ✅ Documentation of authority (file upload)

**Validation**:
- If Signer Role = "Authorized Representative", all auth rep fields become required
- Documentation must be uploaded (power of attorney, guardianship papers, etc.)

---

### 12.6 Multi-Platform Signature Capture

**Windows Implementation**:
- SignaturePad control or custom canvas
- Mouse or stylus input
- Optimize for Surface and tablet devices

**Android Implementation**:
- Custom View with touch handling
- WebView with JavaScript signature library
- Optimize for foldable devices (Samsung Fold, etc.)

**Signature Pad Libraries** (Recommended):
- Android: SignaturePad library, or custom Canvas implementation
- Windows: InkCanvas control (WPF), or custom drawing surface
- Web: signature_pad.js, or HTML5 Canvas

---

### 12.7 Signature Verification

**On Form Review**:
- Display captured signature image
- Show signed name and timestamp
- Allow re-signing if needed (clear and re-capture)

**On Form Submission**:
- Validate signature is not empty
- Validate signature dimensions (minimum size)
- Encode to Base64
- Attach to form data

**On Form Retrieval**:
- Decode Base64 to display signature
- Show alongside enrollment details
- Print on PDF forms

---

## 13. Compliance Requirements

All implementations MUST meet these compliance standards:

### 13.1 HIPAA Compliance (Health Insurance Portability and Accountability Act)

**Privacy Rule Requirements**:

✅ **Privacy Notice**:
- Provide CMS Privacy Notice to all enrollees
- Available in English and Spanish
- Acknowledgment required (attestation checkbox)

✅ **Authorization for Information Release**:
- Required attestation: "I authorize this organization to release my health information to Medicare and other relevant entities"
- Must be explicit opt-in (no pre-checked boxes)

✅ **Minimum Necessary Standard**:
- Only collect PII/PHI necessary for enrollment
- Limit access to enrollment data to authorized personnel only

✅ **Secure Transmission**:
- All data transmission over HTTPS/TLS 1.2 or higher
- End-to-end encryption for sensitive data
- No unencrypted email of enrollment forms

✅ **Secure Storage**:
- Database encryption at rest (AES-256)
- Encrypted file storage for documents
- Access controls (role-based access)

✅ **Audit Logs**:
- Log all access to enrollment records
- Track who viewed, edited, or exported data
- Immutable audit trail (no deletion or modification)
- Retain audit logs for 7 years

---

### 13.2 Data Security Requirements

**Sensitive Data Masking**:

| Field | Display | Storage | Export |
|-------|---------|---------|--------|
| SSN | XXX-XX-1234 (last 4 shown) | Encrypted full value | Encrypted or last 4 only |
| Medicare Number | XXXX-XXX-X123 (last 3 shown) | Full value (may be encrypted) | Full value |
| Bank Account Number | XXXX-XXX-1234 (last 4 shown) | Encrypted full value | Encrypted or last 4 only |
| Credit Card Number | XXXX-XXXX-XXXX-1234 (last 4 shown) | NOT stored (PCI-DSS) | NOT exported |

**Encryption Standards**:
- **At Rest**: AES-256 encryption for database and file storage
- **In Transit**: TLS 1.2 or higher for all network communication
- **Key Management**: Secure key storage (Azure Key Vault, AWS KMS, or equivalent)

**Access Controls**:
- Role-Based Access Control (RBAC)
- Multi-Factor Authentication (MFA) for admin access
- Session timeouts (15-30 minutes of inactivity)
- Failed login attempt lockout (5 attempts)

---

### 13.3 PCI-DSS Compliance (Payment Card Industry Data Security Standard)

**Requirements** (if collecting credit/debit card information):

✅ **Never Store CVV**: Card verification value (CVV/CVC) cannot be stored after authorization

✅ **Tokenization**: Use payment gateway tokenization instead of storing card numbers

✅ **PCI-DSS Certified Gateway**: Use certified payment processor (Stripe, PayPal, Authorize.net, etc.)

✅ **Limited Data Storage**: If storing card numbers, only store encrypted token, not full PAN

✅ **Scope Reduction**: Minimize systems that handle card data

**Recommendation**: Use payment gateway redirect or iframe to avoid PCI-DSS scope

---

### 13.4 Data Retention Requirements

**7-Year Retention** (CMS Requirement):

✅ **Enrollment Forms**: All submitted enrollment forms, including PDF and XML exports

✅ **Supporting Documentation**: SEP documentation, Medicare cards, supporting documents

✅ **Electronic Signatures**: Signature images and full audit trail

✅ **Audit Logs**: All access logs, modification logs, submission logs

✅ **Notices**: All generated notices (acknowledgment, confirmation, Good Cause)

**Storage Format**:
- Original format (PDF, XML, JSON)
- Read-only after submission (no modifications)
- Searchable and retrievable
- Regular backups with offsite storage

**Destruction After 7 Years**:
- Secure deletion (data wiping, not just deletion)
- Documented destruction process
- Compliance with state/federal disposal requirements

---

### 13.5 CMS Submission Requirements

**CMS 834 Transaction**:

✅ **Format**: X12 834 EDI (Electronic Data Interchange) format

✅ **Required Elements**: All 32 CMS data elements properly mapped

✅ **Transmission**: Secure SFTP or AS2 to CMS hub

✅ **Acknowledgment**: Process 999 acknowledgment and 277CA claims acceptance

✅ **Error Handling**: Handle rejection codes, resubmit corrected forms

**XML Export** (Alternative or Supplementary):
- Custom XML schema with all CMS elements
- Include Base64-encoded signature
- Include audit trail metadata

---

### 13.6 Accessibility Requirements (Section 508 / WCAG 2.1)

**Requirements** (for digital enrollment forms):

✅ **Keyboard Navigation**: All controls accessible via keyboard (Tab, Enter, Arrow keys)

✅ **Screen Reader Compatibility**: Proper ARIA labels, semantic HTML, descriptive alt text

✅ **Color Contrast**: WCAG 2.1 AA standard (4.5:1 for normal text, 3:1 for large text)

✅ **Text Resizing**: Support up to 200% zoom without loss of functionality

✅ **Focus Indicators**: Visible focus indicators on all interactive elements

✅ **Error Identification**: Errors clearly identified and associated with fields

✅ **Simplified Language Option**: Plain language variant of form available

**Testing**:
- Test with screen readers (JAWS, NVDA, VoiceOver)
- Test with keyboard-only navigation
- Run automated accessibility scans (aXe, WAVE)

---

### 13.7 OMB Requirements

**OMB Control Number**: 0938-1378

**OMB Expiration Date**: 12/31/2026 (CY 2026 form)

**Paperwork Reduction Act Statement**:
- Must be displayed on enrollment form
- "According to the Paperwork Reduction Act of 1995, no persons are required to respond to a collection of information unless it displays a valid OMB control number. The valid OMB control number for this information collection is 0938-1378. The time required to complete this information collection is estimated to average 20 minutes per response..."

---

## 14. Export Formats

All implementations MUST support these export formats:

### 14.1 CMS 834 XML Export (Primary)

**Purpose**: Electronic submission to CMS for enrollment processing

**Format**: Custom XML schema with all 32 CMS data elements

**Structure**:
```xml
<?xml version="1.0" encoding="UTF-8"?>
<EnrollmentTransaction xmlns="http://cms.gov/medicare/834" version="2026.1">
  <Header>
    <TransactionID>ENR-2026-000123</TransactionID>
    <SubmissionDate>2026-02-05T14:23:15-04:00</SubmissionDate>
    <FormType>EnrollmentRequest</FormType>
    <OMBControlNumber>0938-1378</OMBControlNumber>
  </Header>
  <Beneficiary>
    <FirstName>Maria</FirstName>
    <LastName>Rodriguez</LastName>
    <DateOfBirth>1958-04-12</DateOfBirth>
    <Gender>Female</Gender>
    <MedicareNumber>1EG4-TE5-MK73</MedicareNumber>
    <!-- Element 2-12, etc. -->
  </Beneficiary>
  <PlanSelection>
    <PlanName>Triple-S Advantage Plus</PlanName>
    <ContractNumber>H3259</ContractNumber>
    <PlanID>001</PlanID>
    <EffectiveDate>2027-01-01</EffectiveDate>
    <!-- Element 1, 13, 20, etc. -->
  </PlanSelection>
  <Signature>
    <SignatureBase64>iVBORw0KGgoAAAANSUhEUg...</SignatureBase64>
    <SignatureTimestamp>2026-02-05T14:23:15-04:00</SignatureTimestamp>
    <SignerName>Maria Rodriguez</SignerName>
    <IPAddress>192.168.1.100</IPAddress>
    <!-- Element 21-23 + audit trail -->
  </Signature>
  <!-- Additional sections for all 32 elements -->
</EnrollmentTransaction>
```

**Required Elements**:
- All 32 CMS data elements properly mapped
- Base64-encoded signature
- Complete audit trail
- Timestamps in ISO 8601 format
- Data masking for sensitive fields (SSN, account numbers)

**Validation**:
- XML schema validation before export
- All required elements present
- No PII leakage in XML comments or attributes

---

### 14.2 PDF Form Generation

**Purpose**: Human-readable enrollment form for printing, email, and records

**Format**: PDF/A (archival format) for long-term retention

**Content Requirements**:

✅ **Page 1: Form Header**
- CMS Form 0938-1378 identifier
- OMB Control Number and expiration date
- Form submission date
- Form identifier (tracking number)
- Beneficiary name and Medicare number (last 4 digits visible)

✅ **Pages 2-N: Form Sections**
- All 9 sections with filled data
- Section headers clearly visible
- All field labels and values
- Checkbox states (checked/unchecked)
- Conditional sections included if applicable

✅ **Signature Page**
- Electronic signature image
- Printed name
- Signature timestamp
- Authorized representative info (if applicable)
- Audit trail summary

✅ **Footer**
- Page numbers (Page X of Y)
- Form ID on every page
- Generation timestamp

**Formatting**:
- Font: Arial or similar sans-serif, 10-12pt
- Margins: 1 inch all sides
- Black text on white background (for printing)
- Bilingual: Generate separate EN and ES PDFs, or side-by-side

**Security**:
- Mark as "Official Enrollment Form - Do Not Edit"
- Optional: Password protection (view-only)
- Optional: Digital signature (certificate-based)

**Libraries** (Recommended):
- Windows: iTextSharp, PdfSharp, Aspose.PDF
- Android: iText for Android, PDFBox
- Cross-platform: iText 7

---

### 14.3 JSON Export (Internal Use)

**Purpose**: Data exchange between systems, backup/restore, API responses

**Format**: JSON with all enrollment data

**Structure**:
```json
{
  "enrollmentForm": {
    "formId": "ENR-2026-000123",
    "formType": "EnrollmentRequest",
    "formStatus": "Submitted",
    "createdDate": "2026-02-05T10:00:00-04:00",
    "submittedDate": "2026-02-05T14:23:15-04:00",
    "beneficiary": {
      "firstName": "Maria",
      "lastName": "Rodriguez",
      "dateOfBirth": "1958-04-12",
      "gender": "Female",
      "medicareNumber": "1EG4-TE5-MK73",
      "ssnMasked": "XXX-XX-5678"
    },
    "address": {
      "street1": "123 Calle Principal",
      "street2": "Apt 4B",
      "city": "San Juan",
      "state": "PR",
      "zipCode": "00901"
    },
    "planSelection": {
      "planName": "Triple-S Advantage Plus",
      "contractNumber": "H3259",
      "planId": "001",
      "effectiveDate": "2027-01-01",
      "paymentMethod": "BankAccount"
    },
    "signature": {
      "signatureBase64": "iVBORw0KGgoAAAANSUhEUg...",
      "timestamp": "2026-02-05T14:23:15-04:00",
      "signerName": "Maria Rodriguez",
      "signerRole": "Beneficiary",
      "ipAddress": "192.168.1.100",
      "deviceInfo": "Samsung Galaxy Fold 5, Android 14"
    }
  }
}
```

**Data Handling**:
- Exclude sensitive fields (or mark as encrypted)
- Include all enum values as strings
- ISO 8601 dates/timestamps
- Boolean values as true/false

---

### 14.4 Package Assembly (ZIP)

**Purpose**: Complete enrollment package for submission or archival

**Contents**:
1. **enrollment_form.json** - Complete form data in JSON
2. **enrollment_form_834.xml** - CMS 834 XML for submission
3. **enrollment_form_en.pdf** - English PDF form
4. **enrollment_form_es.pdf** - Spanish PDF form (optional)
5. **signature.png** - Signature image file
6. **Notices/**
   - enrollment_acknowledgment_en.pdf
   - enrollment_acknowledgment_es.pdf
7. **Documentation/** (if SEP)
   - sep_supporting_document.pdf
   - medicare_card.jpg
   - other_documentation.pdf

**File Naming Convention**:
```
EnrollmentPackage_{MedicareNumber}_{FormID}_{YYYYMMDD}.zip

Example:
EnrollmentPackage_1EG4TE5MK73_ENR2026000123_20260205.zip
```

**Compression**: ZIP format, standard compression (deflate)

**Security**: Optional password protection (AES-256)

---

## 15. Error Messages

All implementations MUST provide clear, actionable error messages in both English and Spanish.

### 15.1 Required Field Errors

| Scenario | English Error | Spanish Error |
|----------|---------------|---------------|
| Empty required field | "This field is required" | "Este campo es requerido" |
| First name missing | "First name is required" | "El nombre es requerido" |
| Last name missing | "Last name is required" | "El apellido es requerido" |
| Date of birth missing | "Date of birth is required" | "La fecha de nacimiento es requerida" |
| Medicare number missing | "Medicare Number is required" | "El Número de Medicare es requerido" |
| Plan selection missing | "Please select a plan" | "Por favor seleccione un plan" |
| Signature missing | "Electronic signature is required to submit the form" | "La firma electrónica es requerida para enviar el formulario" |

---

### 15.2 Format Validation Errors

| Scenario | English Error | Spanish Error |
|----------|---------------|---------------|
| Invalid email format | "Please provide a valid email address" | "Por favor proporcione una dirección de email válida" |
| Invalid SSN format | "Social Security Number must be 9 digits" | "El Número de Seguro Social debe tener 9 dígitos" |
| Invalid phone format | "Phone number must be 10 digits: (XXX) XXX-XXXX" | "El número de teléfono debe tener 10 dígitos: (XXX) XXX-XXXX" |
| Invalid ZIP code | "ZIP code must be 5 digits" | "El código postal debe tener 5 dígitos" |
| Invalid date format | "Date must be in MM/DD/YYYY format" | "La fecha debe estar en formato MM/DD/YYYY" |
| Invalid Medicare number | "Invalid Medicare Number format" | "Formato de Número de Medicare inválido" |

---

### 15.3 Business Rule Errors

| Scenario | English Error | Spanish Error |
|----------|---------------|---------------|
| Outside enrollment period | "Enrollment is only available during Open Enrollment Period (October 15 - December 7) or with a qualifying Special Enrollment Period" | "La inscripción solo está disponible durante el Período de Inscripción Abierta (15 de octubre - 7 de diciembre) o con un Período de Inscripción Especial calificado" |
| Plan not available in ZIP | "Selected plan is not available in your ZIP code" | "El plan seleccionado no está disponible en su código postal" |
| Duplicate enrollment | "You already have an active enrollment for this year" | "Ya tiene una inscripción activa para este año" |
| Age restriction | "Beneficiary must be at least 18 years old" | "El beneficiario debe tener al menos 18 años" |
| Future date not allowed | "Date cannot be in the future" | "La fecha no puede ser en el futuro" |
| Effective date not 1st | "Effective date must be the first of the month" | "La fecha efectiva debe ser el primero del mes" |
| SEP window expired | "Special Enrollment Period event must have occurred within the last 60 days" | "El evento del Período de Inscripción Especial debe haber ocurrido en los últimos 60 días" |
| MA OEP already used | "Medicare Advantage Open Enrollment Period can only be used once per year" | "El Período de Inscripción Abierta de Medicare Advantage solo se puede usar una vez al año" |

---

### 15.4 Attestation Errors

| Scenario | English Error | Spanish Error |
|----------|---------------|---------------|
| Attestation not checked | "You must agree to all attestations before submitting" | "Debe aceptar todas las certificaciones antes de enviar" |
| Privacy notice not acknowledged | "You must acknowledge the privacy notice" | "Debe reconocer el aviso de privacidad" |
| Information accuracy not certified | "You must certify that the information is accurate" | "Debe certificar que la información es exacta" |

---

### 15.5 Conditional Field Errors

| Scenario | English Error | Spanish Error |
|----------|---------------|---------------|
| Mailing address incomplete | "Please complete all mailing address fields or uncheck 'Mailing Address Different'" | "Por favor complete todos los campos de dirección postal o desmarque 'Dirección Postal Diferente'" |
| Agent info incomplete | "Please complete all agent/broker information fields" | "Por favor complete todos los campos de información del agente/corredor" |
| Dependent info incomplete | "Please complete all dependent information or remove the dependent" | "Por favor complete toda la información del dependiente o elimínelo" |
| SEP documentation missing | "Supporting documentation is required for the selected SEP reason" | "Se requiere documentación de respaldo para la razón de Período de Inscripción Especial seleccionada" |
| Bank account info incomplete | "Bank account number and routing number are required for bank draft payment" | "Se requiere el número de cuenta bancaria y número de ruta para el pago por débito bancario" |

---

### 15.6 System Errors

| Scenario | English Error | Spanish Error |
|----------|---------------|---------------|
| Network error | "Unable to submit form. Please check your internet connection and try again." | "No se puede enviar el formulario. Por favor verifique su conexión a internet e intente de nuevo." |
| Save failed | "Unable to save form draft. Please try again." | "No se puede guardar el borrador del formulario. Por favor intente de nuevo." |
| Export failed | "Unable to export form. Please try again or contact support." | "No se puede exportar el formulario. Por favor intente de nuevo o contacte soporte." |
| Form load failed | "Unable to load form. Please refresh and try again." | "No se puede cargar el formulario. Por favor actualice e intente de nuevo." |

---

### 15.7 Success Messages

| Scenario | English Message | Spanish Message |
|----------|----------------|-----------------|
| Form submitted | "Enrollment form submitted successfully! You will receive a confirmation email shortly." | "¡Formulario de inscripción enviado exitosamente! Recibirá un correo de confirmación pronto." |
| Draft saved | "Form draft saved successfully" | "Borrador del formulario guardado exitosamente" |
| Signature captured | "Signature captured successfully" | "Firma capturada exitosamente" |
| Form exported | "Form exported successfully" | "Formulario exportado exitosamente" |

---

## 16. Verification Checklist

Use this checklist to verify compliance across all implementations:

### 16.1 Data Elements Verification (32 Elements)

- [ ] Element 1: MA Plan Name (field present, valid selection)
- [ ] Element 2: Beneficiary Name (First, Last, MI - all fields present)
- [ ] Element 3: Date of Birth (field present, date picker, validation)
- [ ] Element 4: Sex/Gender (field present, 4 options minimum)
- [ ] Element 5: Telephone Numbers (Primary required, Secondary optional)
- [ ] Element 6: Permanent Address (all address fields present)
- [ ] Element 7: County (field present or auto-derived)
- [ ] Element 8: Mailing Address (optional separate address with toggle)
- [ ] Element 9: Emergency Contact (name and phone required)
- [ ] Element 10: Email Address (optional, validated if provided)
- [ ] Element 11: Medicare Number (required, validated format)
- [ ] Element 12: Medicare Card Image (optional upload capability)
- [ ] Element 13: Premium Payment Method (required, all options)
- [ ] Element 14: Long-Term Care Indicator (checkbox + facility name)
- [ ] Element 15: Other Insurance (checkbox + details if yes)
- [ ] Element 16: Language Preference (EN/ES selection)
- [ ] Element 17: SSN (field present, 9-digit validation)
- [ ] Element 18: Dependents (loop structure with add/remove)
- [ ] Element 19: Current Plan Status (fields for current plan details)
- [ ] Element 20: PCP Selection (field present if required by plan)
- [ ] Element 21: Electronic Signature (signature pad + capture)
- [ ] Element 22: Signature Date/Timestamp (auto-captured)
- [ ] Element 23: Authorized Rep Contact (conditional fields)
- [ ] Element 24: Employer/Union Information (conditional fields)
- [ ] Element 25: Preferred Contact Method (dropdown selection)
- [ ] Element 26: SNP Eligibility Criteria (conditional fields)
- [ ] Element 27: MSA Plan Requirements (conditional fields)
- [ ] Element 28: CMS Attestations (all required checkboxes)
- [ ] Element 29: Release of Information (checkbox + attestation)
- [ ] Element 30: Electronic Materials (checkbox option)
- [ ] Element 31: Assistance Flag (checkbox for agent help)
- [ ] Element 32: Agent/Broker NPN (conditional, 10-digit validation)

---

### 16.2 Form Types Verification (4 Types)

- [ ] Enrollment Request form type implemented
- [ ] Disenrollment Request form type implemented
- [ ] Plan Change Request form type implemented
- [ ] Special Enrollment Period form type implemented
- [ ] Form type selection mechanism present
- [ ] Each form type includes appropriate sections
- [ ] Form type correctly tracked in metadata

---

### 16.3 Form Sections Verification (9 Sections)

- [ ] Section 1: Personal Information (all required fields)
- [ ] Section 2: Contact Information (phone, email, contact method)
- [ ] Section 3: Address Information (including optional mailing)
- [ ] Section 4: Current Medicare Coverage (optional but complete)
- [ ] Section 5: Plan Selection (plan, payment, effective date)
- [ ] Section 6: Special Circumstances (SEP, dual eligible, etc.)
- [ ] Section 7: Agent/Broker Information (conditional fields)
- [ ] Section 8: Attestations and Agreements (all checkboxes)
- [ ] Section 9: Electronic Signature (signature pad + audit trail)

---

### 16.4 Plan Types Verification (5 Types)

- [ ] Standard MA plan type supported
- [ ] MSA plan type supported with MSASpecific variant
- [ ] PFFS plan type supported with PFFSSpecific variant
- [ ] SNP plan type supported with SNP eligibility fields
- [ ] Other plan type supported
- [ ] Plan type indicators (isMSAPlan, isPFFSPlan, isSNPPlan) implemented
- [ ] Conditional sections activate based on plan type

---

### 16.5 SEP Reasons Verification (16 Reasons)

- [ ] All 16 CMS SEP reasons in dropdown
- [ ] SEP event date field present
- [ ] SEP event description field present
- [ ] SEP documentation upload field present
- [ ] 60-day window validation implemented
- [ ] Good Cause determination tracking implemented
- [ ] SEP-specific effective date logic implemented

---

### 16.6 Validation Rules Verification

**Field-Level Validation**:
- [ ] SSN format validation (9 digits, XXX-XX-XXXX)
- [ ] Medicare Number format validation
- [ ] Email format validation (if provided)
- [ ] Phone format validation (10-15 digits)
- [ ] ZIP code validation (5-digit or ZIP+4)
- [ ] Date of Birth validation (18+, not future, realistic)
- [ ] Effective Date validation (1st of month, future)
- [ ] All required fields validated before submission

**Business Rule Validation**:
- [ ] AEP period dates enforced (Oct 15 - Dec 7)
- [ ] MA OEP period dates enforced (Jan 1 - Mar 31)
- [ ] SEP 60-day window enforced
- [ ] Plan availability by ZIP code validated
- [ ] Duplicate enrollment check implemented
- [ ] Age restriction enforced (18+)
- [ ] Effective date logic correct

---

### 16.7 Bilingual Support Verification

- [ ] Language preference field (Element 16) present
- [ ] English language support complete
- [ ] Spanish (es-PR) language support complete
- [ ] All field labels translated
- [ ] All error messages translated
- [ ] All attestation text translated (using official CMS translations)
- [ ] All notices generated bilingually
- [ ] Language switching works dynamically
- [ ] Language preference persists across sessions

---

### 16.8 Electronic Signature Verification

- [ ] Signature capture pad implemented
- [ ] Touch/stylus input supported
- [ ] "Clear" button to reset signature
- [ ] Signature required before submission
- [ ] Base64 encoding of signature
- [ ] Signature timestamp auto-captured (Element 22)
- [ ] Printed name auto-populated (Element 21)
- [ ] Signer role selection (Beneficiary/Auth Rep)
- [ ] IP address captured
- [ ] Device information captured
- [ ] Complete audit trail stored
- [ ] Authorized representative fields conditional

---

### 16.9 Compliance Verification

**HIPAA Compliance**:
- [ ] Privacy notice provided and acknowledged
- [ ] Authorization for information release obtained
- [ ] Secure transmission (HTTPS/TLS 1.2+)
- [ ] Data encryption at rest (AES-256)
- [ ] Access controls implemented (RBAC)
- [ ] Audit logging implemented
- [ ] 7-year retention capability

**Data Security**:
- [ ] SSN masking (XXX-XX-1234)
- [ ] Bank account masking (XXXX-1234)
- [ ] Medicare number handling (full or last 3)
- [ ] No CVV storage (PCI-DSS)
- [ ] Session timeouts implemented
- [ ] Failed login lockout

**Accessibility**:
- [ ] Keyboard navigation supported
- [ ] Screen reader compatible
- [ ] Color contrast meets WCAG 2.1 AA
- [ ] Focus indicators visible
- [ ] Error identification clear
- [ ] Simplified language option available

**OMB Requirements**:
- [ ] OMB Control Number displayed (0938-1378)
- [ ] OMB Expiration Date displayed (12/31/2026)
- [ ] Paperwork Reduction Act statement present

---

### 16.10 Export Formats Verification

- [ ] CMS 834 XML export implemented
- [ ] XML includes all 32 CMS elements
- [ ] XML includes Base64 signature
- [ ] XML includes audit trail
- [ ] PDF generation implemented
- [ ] PDF includes all form sections
- [ ] PDF includes signature image
- [ ] Bilingual PDF generation (EN/ES)
- [ ] JSON export implemented
- [ ] JSON includes complete form data
- [ ] ZIP package assembly implemented
- [ ] Package includes all required files

---

### 16.11 Enrollment Periods Verification

- [ ] AEP period dates correct (Oct 15 - Dec 7)
- [ ] MA OEP period dates correct (Jan 1 - Mar 31)
- [ ] ICEP logic implemented (7-month window)
- [ ] SEP year-round availability
- [ ] Dual eligible monthly election enabled
- [ ] Institutionalized monthly election enabled
- [ ] Enrollment period validation before submission
- [ ] Effective date calculated per CMS Appendix 3
- [ ] Application date tracking (7 mechanisms)

---

### 16.12 User Experience Verification

- [ ] Form loads quickly (<3 seconds)
- [ ] Navigation between sections smooth
- [ ] Progress indicator shows current section/step
- [ ] Draft saving works automatically
- [ ] Draft retrieval works correctly
- [ ] Form data persists on navigation back
- [ ] Error messages appear near fields
- [ ] Error summary appears on submit attempt
- [ ] Success messages appear on completion
- [ ] Responsive design (works on various screen sizes)

---

### 16.13 Testing Verification

- [ ] Unit tests for validation rules
- [ ] Integration tests for form submission
- [ ] End-to-end tests for complete enrollment flow
- [ ] Accessibility testing completed
- [ ] Cross-browser testing (if web-based)
- [ ] Cross-device testing (if mobile)
- [ ] Bilingual testing (both EN and ES)
- [ ] Performance testing (load times, responsiveness)
- [ ] Security testing (penetration testing, vulnerability scanning)

---

## Appendix A: Quick Reference Tables

### A.1 CMS Data Elements Quick Reference

| # | Element Name | Required | Section | Data Type |
|---|--------------|----------|---------|-----------|
| 1 | MA Plan Name | YES | Plan Selection | String |
| 2 | Beneficiary Name | YES | Personal Info | String |
| 3 | Date of Birth | YES | Personal Info | Date |
| 4 | Sex/Gender | YES | Personal Info | Enum |
| 5 | Telephone Numbers | YES (Primary) | Contact Info | String |
| 6 | Permanent Address | YES | Address Info | Address |
| 7 | County | NO | Address Info | String |
| 8 | Mailing Address | NO | Address Info | Address |
| 9 | Emergency Contact | YES | Personal Info | Contact |
| 10 | Email Address | NO | Contact Info | String |
| 11 | Medicare Number | YES | Personal Info | String |
| 12 | Medicare Card Image | NO | Personal Info | Binary |
| 13 | Premium Payment | YES | Plan Selection | Enum |
| 14 | Long-Term Care | NO | Plan Selection | Boolean |
| 15 | Other Insurance | NO | Current Coverage | Boolean |
| 16 | Language Preference | YES | Personal Info | Enum |
| 17 | SSN | NO | Personal Info | String |
| 18 | Dependents | NO | Personal Info | Loop |
| 19 | Current Plan Status | NO | Current Coverage | Object |
| 20 | PCP Selection | NO | Plan Selection | String |
| 21 | Electronic Signature | YES | Signature | Binary |
| 22 | Signature Date | YES | Signature | DateTime |
| 23 | Auth Rep Contact | Conditional | Signature | Object |
| 24 | Employer/Union Info | NO | Special Circumstances | Object |
| 25 | Preferred Contact Method | YES | Contact Info | Enum |
| 26 | SNP Eligibility | Conditional | Special Circumstances | Object |
| 27 | MSA Requirements | Conditional | Special Circumstances | Object |
| 28 | CMS Attestations | YES | Attestations | Boolean |
| 29 | Release of Info | YES | Attestations | Boolean |
| 30 | Electronic Materials | NO | Attestations | Boolean |
| 31 | Assistance Flag | YES | Agent/Broker | Boolean |
| 32 | Agent/Broker NPN | Conditional | Agent/Broker | String |

---

### A.2 Enrollment Periods Quick Reference

| Period | Dates | Duration | Effective Date | Usage |
|--------|-------|----------|----------------|-------|
| AEP | Oct 15 - Dec 7 | 54 days | Jan 1 (next year) | Primary enrollment period |
| MA OEP | Jan 1 - Mar 31 | 90 days | 1st of next month | MA-to-MA changes only |
| ICEP | 3 months before - 3 months after 65th birthday | 7 months | Varies | New Medicare beneficiaries |
| SEP | Year-round (event-triggered) | 60 days from event | Varies by SEP reason | Qualifying life events |

---

### A.3 SEP Reasons Quick Reference

| # | SEP Reason | Documentation | Effective Date |
|---|------------|---------------|----------------|
| 1 | Moved out of plan area | Proof of address | Month of move or next |
| 2 | Loss of coverage - Death | Death certificate | Month of event or next |
| 3 | Loss of coverage - Divorce | Divorce decree | Month of event or next |
| 4 | Loss of coverage - Employer | COBRA notice | Month of loss or next |
| 5 | Dual eligible status change | Medicaid card | Medicaid effective date |
| 6 | Chronic condition SEP | Physician documentation | Month of request or next |
| 7 | Five-star plan | Plan rating (auto) | Month of request or next |
| 8 | Plan contract termination | CMS notification (auto) | Plan end date |
| 9-16 | Other SEP reasons | Varies | Varies |

---

### A.4 Plan Types Quick Reference

| Plan Type | Form Variant | Special Sections | PCP Required |
|-----------|--------------|------------------|--------------|
| Standard MA | Standard | None | Often |
| MSA | MSASpecific | HSA/MSA deposit, high deductible | No |
| PFFS | PFFSSpecific | Network-free acknowledgment | No |
| SNP | Standard | SNP eligibility criteria | Varies |
| Other | Standard | Custom as needed | Varies |

---

## Appendix B: Official CMS Forms Reference

### Required CMS Forms for 2026 Calendar Year:

1. **CMS Form 0938-1378** - Medicare Advantage Individual Enrollment Request Form
   - CY 2026 version
   - OMB Control Number: 0938-1378
   - OMB Expiration: 12/31/2026
   - Source: https://www.cms.gov/Medicare/CMS-Forms

2. **CY 2026 MA Appendices and Exhibits**
   - Appendix 2: Data Elements Table (32 elements)
   - Appendix 3: Application Date and Enrollment Mechanisms
   - Source: CMS HPMS Memos

3. **CY 2026 Enrollment and Disenrollment Guidance**
   - Medicare Advantage
   - Medicare Cost Plans
   - Coordinated Care Plans
   - Source: CMS.gov

4. **Chart of Approved MA Organizations for Default Enrollment**
   - Updated quarterly (Q1, Q2, Q3, Q4 2025)
   - Lists approved MA organizations
   - Source: CMS.gov

---

## Appendix C: Glossary

**AEP** - Annual Coordinated Election Period (Oct 15 - Dec 7)  
**CMS** - Centers for Medicare & Medicaid Services  
**COB** - Coordination of Benefits (other insurance)  
**ESRD** - End Stage Renal Disease  
**HIPAA** - Health Insurance Portability and Accountability Act  
**ICEP** - Initial Coverage Election Period (around 65th birthday)  
**LIS** - Low Income Subsidy (Extra Help with drug costs)  
**LTC** - Long-Term Care  
**MA** - Medicare Advantage  
**MA OEP** - Medicare Advantage Open Enrollment Period (Jan 1 - Mar 31)  
**MSA** - Medical Savings Account  
**NPN** - National Producer Number (for agents/brokers)  
**OMB** - Office of Management and Budget  
**PCP** - Primary Care Provider  
**PFFS** - Private Fee-For-Service  
**PHI** - Protected Health Information  
**PII** - Personally Identifiable Information  
**SEP** - Special Enrollment Period  
**SNP** - Special Needs Plan (D-SNP, C-SNP, I-SNP)  
**SSN** - Social Security Number  

---

## Document Control

**Version History**:
- v2026.1 - February 5, 2026 - Initial master requirements guide

**Approval**:
- [ ] CMS Compliance Officer
- [ ] IT Security Officer
- [ ] Development Team Lead (Windows)
- [ ] Development Team Lead (Android)
- [ ] Development Team Lead (Android2)
- [ ] Quality Assurance Lead

**Next Review Date**: Upon release of CY 2027 CMS guidance (expected Q4 2026)

**Distribution**: All Triple-S AEP development teams, QA, compliance, management

---
**Triple-S English Form**:
The Following is the form conents that we need to makesure our Enrollment wizard asks and collects all the relevant data to completely fill in all thats is bveing asked here: 
ADVANTAGE
2026 ENROLLMENT REQUEST FORM
TO ENROLL IN A MEDICARE ADVANTAGE PLAN (PART C)
Who can use this form?
People with Medicare who want to join a Medicare
Advantage Plan
To join a plan, you must:
• Be a United States citizen or be lawfully present
in the U.S.
• Live in the plan’s service area
Important: To join a Medicare Advantage Plan,
you must also have both:
• Medicare Part A (Hospital Insurance)
• Medicare Part B (Medical Insurance)
When do I use this form?
You can join a plan:
• Between October 15–December 7 each year
(for coverage starting January 1)
• Within 3 months of first getting Medicare
• In certain situations where you’re allowed to join
or switch plans
Visit Medicare.gov to learn more about when you
can sign up for a plan.
What do I need to complete this form?
• Your Medicare Number (the number on your red,
white, and blue Medicare card)
• Your permanent address and phone number
Note: You must complete all items in Section 1.
The items in Section 2 are optional — you can’t be
denied coverage because you don’t fill them out.
Reminders:
• If you want to join a plan during fall open
enrollment (October 15–December 7), the plan
must get your completed form by December 7.
• Your plan will send you a bill for the plan’s
premium. You can choose to sign up to have
your premium payments deducted from your
bank account or your monthly Social Security
(or Railroad Retirement Board) benefit.
What happens next?
Send your completed and signed form to:
Triple-S Advantage, Inc. – Enrollment Department
PO Box 11320
San Juan, Puerto Rico 00922-1320
Once they process your request to join,
they’ll contact you.
How do I get help with this form?
Call Triple-S Advantage at 1-888-620-1919.
TTY users can call 1-866-620-2520.
Or, call Medicare at 1-800-MEDICARE
(1-800-633-4227). TTY users can call
1-877-486-2048.
En español: Llame a Triple-S Advantage al
1-888-620-1919 / usuarios de TTY
1-866-620-2520 o a Medicare gratis al
1-800-633-4227 y oprima el 8 para asistencia en
español y un representante estará disponible para
asistirle.
Individuals experiencing homelessness
If you want to join a plan but have no permanent
residence, a Post Office Box, an address of a
shelter or clinic, or the address where you receive
mail (e.g. social security checks) may be
considered as your permanent residence address.
According to the Paperwork Reduction Act of 1995, no persons are required to respond to a collection of information unless it displays a valid OMB control number.
The valid OMB control number for this information collection is 0938-1378. The time required to complete this information is estimated to average 20 minutes per
response, including the time to review instructions, search existing data resources, gather the data needed, and complete and review the information collection. If
you have any comments concerning the accuracy of the time estimate(s) or suggestions for improving this form, please write to: CMS,7500 Security Boulevard,
Attn: PRA Reports Clearance Ocer, Mail Stop C4-26-05, Baltimore, Maryland 21244-1850.
IMPORTANT: Do not send this form or any items with your personal information (such as claims, payments, medical records, etc.) to the PRA Reports
Clearance Oce. Any items we get that aren’t about how to improve this form or its collection burden (outlined in OMB0938-1378) will be destroyed.
It will not be kept, reviewed, or forwarded to the plan. See “What happens next?” on this page to send your completed form to the plan.
Y0082_107026E001_C CMS Approved 6/17/2025
OMB No. 0938-1378
Expires: 12/31/2026
SECTION 1
All Fields on this page/section are required (unless marked optional):
Scope of Appointment #: _______________
Select the plan you want to join:
2026 ENROLLMENT REQUEST FORM P.2
All plans have Prescription Drug Coverage (Part D)
____ Óptimo Plus (PPO)
____ Brillante (HMO-POS)
____ Enlace Plus (HMO)
____ Ahorro Plus (HMO)
____ ContigoEnMente (HMO-SNP)
____ Contigo Plus (HMO-SNP)
____ Platino Plus (HMO-SNP)
____ Platino Advance (HMO-SNP)
____ Platino Blindao (HMO-SNP
____ Platino Enlace (HMO-SNP) )
$0 monthly premium for all plans
Please indicate in which Group plan you want to enroll in (if applicable):
Coverage: ______________________________ Choose: __ (HMO) __ (PPO)
Monthly Premium: _______________________ Effective Date: __________________________
Social Security Number (Only for group plans.): _____________________________________
Beneficiary Information:
First Name: ___________________ Last Names: _____________________ [Optional: Middle Initial]: _____
Birth Date (MM/DD/YYYY): ________________________ Sex: ____ F ____ M
Home Phone Number: ______________________ Alternate Phone Number: ________________________
(____ Check here if cellular) (____ Check here if cellular)
Y0082_107026E001_C CMS Approved 6/17/2025
2026 ENROLLMENT REQUEST FORM P.3
Permanent Residence Street Address (Don’t enter a P.O. Box. Note: For individuals experiencing
homelessness, a PO Box may be considered your permanent residence address.):
__________________________________________________________________________________________
__________________________________________________________________________________________
City: ___________________, State _________________________ Zip Code: ____________________
Mailing Address, if different from your Permanent Residence Address. (PO Box allowed):
Street Address: _________________________________________________________________________
__________________________________________________________________________________________
City: ___________________, State _________________________ Zip Code: ____________________
Your Medicare Information:
Medicare Number: _ _ _ _ -_ _ _-_ _ _ _
Answer these important questions:
Will you have other prescription drug coverage (like VA, TRICARE, etc.) in addition to Triple-S
Advantage? Yes ____ No ____
If “Yes”, please list your other coverage and your identification (ID) number(s) for this coverage:
Name of other coverage: Member number for this coverage: Group number for this coverage:
__________________________ ______________________________ ____________________________
If you choose any of our Platino plans, please answer the following:
Are you enrolled in your States Medicaid Program? Yes ____ No ____
If “Yes”, please provide your Medicaid number (MPI): ___________________________________________
If you choose to enroll in Contigo Plus (HMO-SNP), please select the chronic condition that you
have been diagnosed with:
____Diabetes Mellitus ____Cardiovascular Disorder ____Chronic Heart Failure
If you choose to enroll in ContigoEnMente (HMO-SNP), please confirm if you have been
diagnosed with any symptoms of dementia: Yes ____ No ____
Y0082_107026E001_C CMS Approved 6/17/2025
2026 ENROLLMENT REQUEST FORM P.4
IMPORTANT: Read and sign below:
By completing this enrollment application, I agree to the following:
1. I must keep both Hospital (Part A) and Medical (Part B) to stay in Triple-S Advantage.
2. By joining this Medicare Advantage Plan, I acknowledge that Triple-S Advantage will share my
information with Medicare, who may use it to track my enrollment, to make payments, and for other
purposes allowed by Federal law that authorize the collection of this information (see Privacy Act
Statement below). Your response to this form is voluntary. However, failure to respond may affect
enrollment in the plan.
3. I understand that I can be enrolled in only one MA plan at a time – and that enrollment in this plan will
automatically end my enrollment in another MA plan (exceptions apply for MA PFFS, MA MSA plans).
4. I understand that when my Triple-S Advantage coverage begins, I must get all of my medical and
prescription drug benefits from Triple-S Advantage. Benefits and services provided by Triple-S
Advantage and contained in my Triple-S Advantage “Evidence of Coverage” document (also known as
a member contract or subscriber agreement) will be covered. Neither Medicare nor Triple-S Advantage
will pay for benefits or services that are not covered.
5. The information on this enrollment form is correct to the best of my knowledge. In understand if I
intentionally provide false information on this form, I will be disenrolled from the plan.
6. I understand that my signature (or the signature of the person legally authorized to act on my behalf)
on this application means that I have read and understand the contents of this application. If signed by
an authorized representative (as described above), this signature certifies that:
a. This person is authorized under State law to complete this enrollment, and
b. Documentation of this authority is available upon request by Medicare.
Signature: ____________________________________ Today’s date: _______________________
Only for Electronic Enrollment Application completed in person:
Checking “Enroll Now” is considered your signature.
Enroll Now: ____________________ Today’s date: ______________________________________
Only for Enrollment Application completed by phone:
Call number (UCID): ______________________ Today’s date: _____________________________
Witness: _____________________________________ Today’s date: _______________________
If you are the authorized representative/legal representative, you must sign above and fill out these fields:
Name: ___________________________ Address: ____________________________________________
Phone Number: _____________________ Relationship to Enrollee: _______________________________
Y0082_107026E001_C CMS Approved 6/17/2025
2026 ENROLLMENT REQUEST FORM P.5
SECTION 2
All fields in this section are optional:
Answering these questions is your choice. You can’t be denied coverage because you
don’t fill them out.
Select one if you want us to send you information in a language other than English:
Spanish ____ Other (indicate): ______________
Select one if you want us to send you information in an accessible format
Braille ____ Large Print ____ Audio CD ____ Data CD_____
Please contact Triple-S Advantage at 1-888-620-1919 if you need information in an accessible format
other than what’s listed above. Our office hours are Monday through Sunday from 8:00 a.m. to 8:00 p.m.
TTY users can call 1-866-620-2520.
Do you work? ___ Yes ___ No Does your spouse work? ___ Yes ___ No
For HMO Plans - Please, choose the name of a Primary Care Physicians (PCP), clinic, or health
center from our Providers Directory: ______________________________________
Phone Number __________________________________________________________
If you do not choose a PCP, one will be assigned to you automatically.
I want to get the following materials and information via e-mail, (select one or more):
____ Provider Directory
____ Annual Notice of Changes
____ Evidence of Coverage
____ Summary of Benefits
____ Prescription Drug Formulary
____ Promotional materials to maintain your health, appointment reminders, and any other health
communications of the Plan.
____ Electronic Enrollment Confirmation
Do you agree to be contacted using the contact information provided by you or by means
authorized by law?
Text message: ____Yes ___No Number: _______________________________________
Email: ____Yes ___No Address: _______________________________________
Y0082_107026E001_C CMS Approved 6/17/2025
2026 ENROLLMENT REQUEST FORM P.6
For opting out of receiving communications via e-mail or text messages, you can communicate anytime
with our Member Services Center at 1-888-620-1919, Monday through Sunday from 8:00 a.m. to 8:00
p.m. TTY (Hearing Impaired) should call 1- 866-620-2520. Please note that if you opt out of receiving
communications via e-mail or text messages you will still be receiving transactional communications such
as Service Coverage Determinations among others.
Emergency Contact: ___________________________ Phone Number: _____________________
Relationship to you: ____________________________
Are you the retiree? Yes ___ No ___ (Only for employer groups.)
If “Yes”, retirement date (month/date/year): _________________________
If no, name of retiree: ___________________________________________
Are you covering a spouse or dependents under this employer or union plan? (Only for
employer groups.)
__ _Yes ___ No ___ Not applicable
If “Yes”, name of spouse: ________________________________________
Name(s) of dependent(s): ________________________________________
Are you a resident in a long-term care facility, such as a nursing / elderly home? Yes ___ No ____
If “Yes,” please provide the following information:
Name of Institution: _____________________________________________
Administrator’s name: ___________________________________________
Institution or administrator’s phone number: ________________________
Current Health Plan: ___MMM ___Humana ___MCS ___Medicare Original ___USA Plan
___Other:___________________________________
Y0082_107026E001_C CMS Approved 6/17/2025
2026 ENROLLMENT REQUEST FORM P.7
Paying your Plan Premium:
You can pay your monthly plan premium (including any late enrollment penalty that you currently
have or may owe) by mail, Electronic Funds Transfer (EFT), or credit card each month. You can
also choose to pay your premium by having it automatically taken out of your Social
Security or Railroad Retirement Board (RRB) benefit check each month.
If you have to pay a Part D-Income Related Monthly Adjustment Amount (Part D-IRMAA),
you must pay this extra amount in addition to your plan premium. DON’T pay Triple-S
Advantage, Inc. the Part D-IRMAA.
Please select a premium and/or late enrollment penalty payment option:
If you don’t select a payment option, you will get a coupon book.
_____ Get a coupon book.
_____ Electronic Funds Transfer (EFT) from your bank account each month.
Please enclose a VOIDED check or provide the following:
Account holder’s name: ______________________
Bank routing number: __ __ __ __ __ __ __ __ __
Bank account number: __ __ __ __ __ __ __ __ __
Account type: _____Checking _____Savings
_____ Credit Card. Please provide the following information:
Type of card: __ Visa __ Master Card
Name of account holder as it appears on card:
___________________________________________________
Card number: __ __ __ __ __ __ __ __ __ __ __ __ __ __ __ __
Expiration date: __ __ /__ __ __ __ (MM/YYYY)
_____ Automatic deduction from your monthly Social Security or Railroad Retirement Boar
(RRB) benefit check.
I get monthly benefits from: ____ Social Security ____ RRB
Y0082_107026E001_C CMS Approved 6/17/2025
2026 ENROLLMENT REQUEST FORM P.8
(The Social Security/RRB deduction may take two or more months to begin after Social Security
or RRB approves the deduction. In most cases, if Social Security or RRB accepts your request for
automatic deduction, the first deduction from your Social Security or RRB benefit check will
include all premiums due from your enrollment effective date up to the point withholding begins. If
Social Security or RRB does not approve your request for automatic deduction, we will send you
a paper bill for your monthly premiums.)
Important information about Special Supplemental Benefits
for the Chronically Ill:
Some of our plans offer Special Supplemental Benefits for the Chronically Ill (SSBCI), this means
that to be eligible to receive these benefits, the member must comply with all the following:
• Have one or more comorbid and medically complex chronic conditions that are
life-threatening or significantly limit the member’s overall health or function;
• Have a high risk of hospitalization or other adverse health outcomes; and
• Require intensive care coordination.
If you elected coverage that includes Special Supplemental Benefits for the Chronically Ill (SSBCI),
please note that in order to receive these benefits you must meet all of the requirements set forth
above, and that Triple-S will perform a clinical verification in order to validate your eligibility. If after
the clinical validation you do not meet the requirements, you will be eligible to receive all other
benefits in your plan package except for the Special Supplemental Benefits for the Chronically Ill
(SSBCI).
____ Initial Package (Summary of Benefits, Pre-Enrollment Checklist)
____ Medicare Star Ratings Notice
____ Notice of web availability of Evidence of Coverage, Drug Formulary and Provider
and Pharmacy Directory
____ Enrollment Confirmation (if applicable)
____ Enrollment Form Copy (if applicable)
____ Attestation of Eligibility for an Enrollment Period (if applicable)
____ Precertification of Chronic Diseases (if applicable)
____ Authorization to Disclose Protected Health Information (PHI form) (if applicable)
I certify to have received the following documents
from the Triple-S Advantage representative:
Y0082_107026E001_C CMS Approved 6/17/2025
2026 ENROLLMENT REQUEST FORM P.9
The following only applies if the Notice of web availability of Evidence of Coverage,
Drug Formulary and Provider and Pharmacy Directory was not provided:
____ Evidence of Coverage and Durable Medical Equipment Formulary (if applicable)
____ Provider and Pharmacy Directory (if applicable)
____ Drug Formulary (if applicable)
For individuals helping enrollee with completing this form only:
Complete this section if you’re an individual (i.e. agents, brokers, SHIP counselors, family
members, or other third parties) helping an enrollee fill out this form.
Name: __________________________ Relationship to enrollee: ________________________
Signature: _______________________
National Producer Number (Agents/Brokers only):_____________________________________
Official Use Only:
Receipt Date: _____________________________________________________________________
Plan ID #: ___________________ Effective Date of Coverage _________________________
PRIVACY ACT STATEMENT
The Centers for Medicare & Medicaid Services (CMS) collects information from Medicare plans to track
beneficiary enrollment in Medicare Advantage (MA) Plans, improve care, and for the payment of Medicare
benefits. Sections 1851 of the Social Security Act and 42 CFR §§ 422.50 and 422.60 authorize the
collection of this information. CMS may use, disclose and exchange enrollment data from Medicare
beneficiaries as specified in the System of Records Notice (SORN) “Medicare Advantage Prescription Drug
(MARx)”, System No. 09-70-0588. Your response to this form is voluntary. However, failure to respond
may affect enrollment in the plan.
Triple-S Advantage, Inc. is an independent licensee of BlueCross BlueShield Association. Platino plans are
available to anyone who has both Medical Assistance from the State and Medicare.
Y0082_107026E001_C CMS Approved 6/17/2025
2026 ENROLLMENT REQUEST FORM P.10
Notice of Availability of Language Assistance Services and Auxiliary Aids and Services
Aviso de Disponibilidad de Servicios de Asistencia Lingüística, Ayudas y Servicios Auxiliares /
English: ATTENTION: If you speak English, free language assistance services are available to you.
Appropriate auxiliary aids and services to provide information in accessible formats are also available free
of charge. Call 1-888-620-1919 (TTY/TDD 1-866-620-2520) or speak to your provider.
Español: ATENCION: Si usted habla español, servicios de asistencia lingüística gratuitos están
disponibles para usted. También están disponibles ayudas y servicios auxiliares apropiados para
proporcionar información en formatos accesibles, sin costo adicional. Llame al 1-888-620-1919
(TTY/TDD 1-866-620-2520) o hable con su proveedor.
Português: ATENÇÃO: Se você fala português do Brasil, tem à disposição serviços gratuitos de
assistência linguística. Auxílios e serviços auxiliares apropriados para fornecer informações em formatos
acessíveis também estão disponíveis gratuitamente. Ligue para 1-888-620-1919 (TTY 1-866-620-2520)
ou fale com seu provedor.
Français: ATTENTION: si vous parlez Français, des services d’assistance linguistique gratuits sont à votre
disposition. Des aides et services auxiliaires appropriés pour fournir des informations dans des formats
accessibles sont également disponibles gratuitement. Appelez le 1-888-620-1919 (TTY 1-866-620-2520)
ou parlez à votre fournisseur.
Italiano: ATTENZIONE: se parli italiano, sono disponibili servizi di assistenza linguistica gratuiti. Sono
inoltre disponibili gratuitamente ausili e servizi ausiliari adeguati per fornire informazioni in formati
accessibili. Chiama l’1-888-620-1919 (TTY 1-866-620-2520) o parla con il tuo fornitore.
Deutsch: ACHTUNG: Wenn Sie Deutsch sprechen, stehen Ihnen kostenlose Sprachassistenzdienste zur
Verfügung. Entsprechende Hilfsmittel und Dienste zur Bereitstellung von Informationen in barrierefreien
Formaten stehen ebenfalls kostenlos zur Verfügung. Rufen Sie 1-888-620-1919 (TTY +1-866-620-2520)
an oder sprechen Sie mit Ihrem Provider
Tagalog: PAALALA: Kung nagsasalita ka ng Tagalog, magagamit mo ang mga libreng serbisyong tulong
sa wika. Magagamit din nang libre ang mga naaangkop na auxiliary na tulong at serbisyo upang magbigay
ng impormasyon sa mga naa-access na format. Tumawag sa 1-888-620-1919 (TTY: 1-866-620-2520) o
makipag-usap sa iyong provider.
Y0082_107026E001_C CMS Approved 6/17/2025
2026 ENROLLMENT REQUEST FORM P.11
Y0082_107026E001_C CMS Approved 6/17/2025
ADVANTAGE
WORKING AGED SURVEY
Beneficiary’s Name: ________________________________________________________________________
Medicare Identification Number:_____________________________________________________________
1. Are you currently working for a private, public, or non-profit organization? _______ Yes _______ No
2. Do you have healthcare insurance provided by your employer?
_______ Yes _______ No _______Not applicable
3. If you are married, do you have a healthcare insurance provided by your spouse’s employer?
_______ Yes _______ No _______Not applicable
4. In case that you have healthcare insurance provided by an employer please provide the following
information:
Employer’s Name:________________________________________________________________________
Employer’s Address: _____________________________________________________________________
Healthcare Insurance Name: ______________________________________________________________
Policyholder’s Name: _____________________________________________________________________
Group Number: _________________________________________________________________________
Contract: ______________________________________________________________________________
5. The employer has more than 20 employees? _______ Yes _______ No _______Not applicable
6. Do you have plans to disenroll from your employer’s coverage soon?
_______Three months _______Six months _______One year
_______Not yet _______Not applicable
The Centers for Medicare and Medicaid Services (CMS) requests that we report the current working status
of our members. In order to report your status accurately, please complete this survey. This survey will not
affect your Medicare coverage or your membership in the Medicare plan you have elected. Please check
the appropriate boxes.
WORKING AGED SURVEY P.2
7. Do you have your own business? _______ Yes _______ No
8. If you are a business owner, do you have healthcare insurance through your business?
_______ Yes _______ No _______Not applicable
9. If you have healthcare insurance through your business, please provide the following information:
Company’s Name:_______________________________________________________________________
Company’s Address:_____________________________________________________________________
Healthcare Insurance Name: ______________________________________________________________
Group Number: _________________________________________________________________________
Contract Number: _______________________________________________________________________
10. Have you been rejected from an employer group healthcare insurance? _______ Yes _______ No
Y0082_26CI002E_C Triple-S Advantage is an independent Licensee of BlueCross and BlueShield Association.
ATTESTATION OF ELIGIBILITY FOR AN ENROLLMENT PERIOD
ADVANTAGE
Typically, you may enroll in a Medicare Advantage plan only during the annual enrollment period
from October 15 through December 7 of each year. There are exceptions that may allow you to enroll
in a Medicare Advantage plan outside of this period.
Please read the following statements carefully and check the box if the statement applies to you. By
checking any of the following boxes you are certifying that, to the best of your knowledge, you are eligible
for an Enrollment Period. If we later determine that this information is incorrect, you may be disenrolled.
I am new to Medicare.
I am enrolled in a Medicare Advantage plan and want to make a change during the Medicare Advantage
Open Enrollment Period (MA OEP).
I recently moved outside of the service area for my current plan or I recently moved and have new options
available to me. I moved on (insert date) ______________________________________________________.
I recently was released from incarceration. I was released on (insert date) _________________________.
I recently returned to the United States after living permanently outside of the U.S. I returned to the U.S.
on (insert date) __________________________________________________________________________.
I recently obtained lawful presence status in the United States. I got this status on (insert date)
______________________________________________.
I recently had a change in my Medicaid (newly got Medicaid, had a change in level of Medicaid
assistance, or lost Medicaid) on (insert date) __________________________________________________.
I recently had a change in my Extra Help paying for Medicare prescription drug coverage (newly got
Extra Help, had a change in the level of Extra Help, or lost Extra Help) on (insert date) ______________.
I have Medicare and get full Medicaid benefits. I want to join or switch to a plan that coordinates coverage
between my Medicare and Medicaid managed care plans (called an integrated Dual Eligible Special
Needs Plan (D-SNP)).
I am moving into, live in, or recently moved out of a Long-Term Care Facility (for example, a nursing home
or long term care facility). I moved/will move into/out of the facility on (insert date) __________________.
I recently left a PACE program on (insert date) ________________________________________________.
I recently involuntarily lost my creditable prescription drug coverage (coverage as good as Medicare’s).
I lost my drug coverage on (insert date) _____________________________________________________.
I am leaving employer or union coverage on (insert date) ________________________________________.
Y0082_26CI003E_C
ATTESTATION OF ELIGIBILITY FOR AN ENROLLMENT PERIOD P.2
ADVANTAGE
I’m in a qualified State Pharmaceutical Assistance Program, or I’m losing help from a State
Pharmaceutical Assistance Program.
My plan is ending its contract with Medicare, or Medicare is ending its contract with my plan.
I was enrolled in a plan by Medicare (or my State) and I want to choose a different plan. My enrollment in
that plan started on (insert date) ______________________________.
I was enrolled in a Special Needs Plan (SNP) but I have lost the special needs qualification required to be
in that plan. I was disenrolled from the SNP on (insert date) _____________________________.
I was affected by an emergency or major disaster (as declared by the Federal Emergency Management
Agency (FEMA) or by a Federal, State, or local government entity). One of the other statements here
applied to me, but I was unable to make my enrollment request because of the disaster.
Y0082_26CI003E_C Triple-S Advantage is an independent licensee of the Blue Cross Blue Shield Association.
NEW MEMBER SERVICES TRANSITION FORM
Last Names: Name: Initial:
_________________________________________________ ____________________________ ______
Date of Birth Month:_______ Day:_______ Year:_______
Telephone 1:_______________________________ Telephone 2:________________________________
Benefit Plan: ___________________________________________________________________________
Effectivity date: Month:_______ Day:_______ Year:_______
__ Hospital Bed
__ Wheelchair
__ Glucometer*
__ Insulin Pump
__ Nebulizer**
__ Therapy Drugs
(Respiratory)
__ Tube Feeding (formula)
or by mouth
__ Ostomy/Urostomy***
__ Ulcer healing
__ Nursing and
Therapy Services
__ Oxygen
__ CPAP
__ Drug injections / IV
Equipment/Supply/ Services Provider/ Effectivity Comments
(month/year)
* Glucometer – medical device for measuring approximate concentration for glucose in the blood.
** Nebulizer – respiratory therapy equipment
*** Ostomy – gastrostomies, colostomies, etc. See care supply necessity.
Information provided by: _________________________________________________________________
________________________________ _____________________ _____________
Plan Representative Region Date
Y0082_26CI017E_C
SHIC # (MEDICARE NUMBER): __________________________________________________________
Company
Previous
Health Plan
Please revise if member has the following equipment and/or Home Health Services
ADVANTAGE
PRE-ENROLLMENT CHECKLIST
ADVANTAGE
Before making an enrollment decision, it is important that you fully understand our benefits and rules. If you
have any questions, you can call and speak to a customer service representative at 1-888-620-1919 (TTY
1-866-620-2520).
Y0082_26CI005E_C
UNDERSTANDING THE BENEFITS
 The Evidence of Coverage (EOC) provides a complete list of all coverages and services. It is important to
review the plan's coverage, costs, and benefits before enrolling. Visit www.sssadvantage.com or call
1-888-620-1919 (TTY 1-866-6202520) to view a copy of the EOC.
 Review the Provider Directory (or ask your doctor) to make sure the doctors you see now are in the
network. If they are not listed, it means you will likely have to select a new doctor.
 Review the Pharmacy Directory to make sure the pharmacy you use for any prescription medicines is in
the network. If the pharmacy is not listed, you will likely have to select a new pharmacy for your
prescriptions.
 Review the Formulary to make sure your drugs are covered.
UNDERSTANDING IMPORTANT RULES
 In addition to your monthly plan premium (if applicable), you must continue to pay your Medicare Part B
premium. This premium is normally taken out of your Social Security check each month.
 Benefits, premiums and/or copayments/coinsurance may change on January 1, 2027.
 Except in emergency or urgent situations, we do not cover services by out-ofnetwork providers
(doctors who are not listed in the Provider Directory).
 For PPO and HMO-POS plans - Our plan allows you to see providers outside of our network
(non-contracted providers). However, while we will pay for covered services or certain covered services
provided by a non-contracted provider, the provider must agree to treat you. Except in an emergency or
urgent situations, non-contracted providers may deny care. In addition, you may pay a higher cost for
services received by non-contracted providers.
 For Chronic Conditions Special Needs plan - This plan is a chronic condition special needs plan
(C-SNP). Your ability to enroll will be based on verification that you have a qualifying specific severe or
disabling chronic condition.
 For Platino plans - This plan is a dual eligible special needs plan (D-SNP). Your ability to enroll will be
based on verification that you are entitled to both Medicare and medical assistance from a State plan
under Medicaid.
 If you are currently enrolled in a Medicare Advantage plan, your current Medicare Advantage healthcare
coverage will end once your new Medicare Advantage coverage starts. If you have Tricare, your
coverage may be affected once your new Medicare Advantage coverage starts. Please contact Tricare
for more information. If you have a Medigap plan, once your Medicare Advantage coverage starts, you
may want to drop your Medigap policy because you will be paying for coverage you cannot use.
Triple-S Advantage is an independent licensee of the BlueCross BlueShield Association


**Triple-S Spanish Form**:

ADVANTAGE
FORMULARIO DE SOLICITUD DE AFILIACIÓN 2026
PARA AFILIARSE EN UN PLAN MEDICARE ADVANTAGE (PARTE C)
¿Quién puede utilizar este formulario?
Personas con Medicare que desean unirse a un
plan Medicare Advantage
Para unirse a un plan, debe:
• Ser ciudadano de los Estados Unidos o
encontrarse legalmente en los Estados Unidos
• Vivir en el área de servicio del plan
Importante: Para afiliarse en un plan Medicare
Advantage, también debe tener ambos:
• Medicare Parte A (Seguro Hospitalario)
• Medicare Parte B (Seguro Médico)
¿Cuándo utilizo este formulario?
Puede unirse a un plan:
• Entre el 15 de octubre al 7 de diciembre de
cada año (para comenzar su cubierta el 1 de enero)
• Dentro de los 3 meses de haber obtenido
Medicare por primera vez
• En determinadas situaciones en las que se le
permite unirse o cambiar de plan.
Visite Medicare.gov para obtener más información
sobre cuándo puede afiliarse en un plan.
¿Qué necesito para completar este formulario?
• Su número de Medicare (el número en su tarjeta
roja, blanca y azul de Medicare)
• Su dirección y número de teléfono permanentes
Nota: Debe completar todos los campos de la
Sección 1. Los campos de la Sección 2 son
opcionales; no se le puede negar la cubierta
porque no los complete.
Recordatorios:
• Si desea unirse a un plan durante la afiliación
abierta de otoño (del 15 de octubre al 7 de
diciembre), el plan debe recibir su formulario
completado en o antes del 7 de diciembre.
• Su plan le enviará una factura por la prima del
plan. Puede optar por inscribirse para que los
pagos de sus primas se deduzcan de su cuenta
bancaria o de su beneficio mensual del Seguro
Social (o de la Junta de Retiro Ferroviario).
¿Qué pasa después?
Envíe su formulario completo y firmado a:
Triple-S Advantage, Inc. – Departamento de Matrícula
PO Box 11320
San Juan, Puerto Rico 00922-1320
Una vez que procesen su solicitud para afiliarse,
se comunicarán con usted.
¿Cómo obtengo ayuda con este formulario?
Llame a Triple-S Advantage al 1-888-620-1919.
Los usuarios de TTY pueden llamar al
1-866-620-2520.
O, llame a Medicare al 1-800-MEDICARE
(1-800-633-4227) y oprima el 8 para asistencia en
español y un representante estará disponible para
asistirle. Los usuarios de TTY pueden llamar al
1-877-486-2048.
In English: Call Triple-S Advantage at
1-888-620-1919 / TTY users 1-866-620-2520 or
call Medicare toll free at
1-800-633-4227.
Personas sin hogar
Si desea afiliarse a un plan, pero no tiene residencia
permanente, se puede considerar como dirección
de residencia permanente un apartado de correos,
la dirección de un refugio o de una clínica, o la
dirección donde recibe el correo (por ejemplo, los
cheques del seguro social).
De acuerdo con la Ley de Reducción de Papeleo de 1995, ninguna persona está obligada a responder a la recopilación de información a menos que muestre un número
de control válido de la OMB. El número de control OMB válido para esta recopilación de información es el 0938-1378. El tiempo necesario para completar esta
información se estima en un promedio de 20 minutos por respuesta, incluyendo el tiempo para revisar las instrucciones, buscar los recursos de datos existentes, reunir
los datos necesarios, y completar y revisar la recopilación de información. Si tiene algún comentario sobre la exactitud del tiempo estimado o sus sugerencias para
mejorar este formulario, por favor escriba a: CMS,7500 Security Boulevard, Attn: PRA Reports Clearance Ocer, Mail Stop C4-26-05, Baltimore, Maryland 21244-1850.
IMPORTANTE: No envíe este formulario ni ningún documento con su información personal (como reclamaciones, pagos, registros médicos, etc.) a la
Ocina de Autorización de Informes PRA. Cualquier artículo que recibamos que no sea sobre cómo mejorar este formulario o su carga de recopilación
(indicada en OMB0938-1378) será destruido. No se conservará, ni se revisará, ni se enviará al plan. Consulte la sección "¿Qué pasa después?" de esta
página para enviar su formulario completado al plan.
Y0082_107026S001_C CMS Approved 6/17/2025
OMB No. 0938-1378
Expires: 12/31/2026
SECCIÓN 1
Todos los campos en esta página / sección son requeridos
(a menos que indique ser opcional):
# Boleta: _______________
Seleccione el plan al que desea afiliarse:
SOLICITUD DE AFILIACIÓN 2026 P.2
Todos los planes tienen cubierta de Farmacia (Parte D)
____ Óptimo Plus (PPO)
____ Brillante (HMO-POS)
____ Enlace Plus (HMO)
____ Ahorro Plus (HMO)
____ ContigoEnMente (HMO-SNP)
____ Contigo Plus (HMO-SNP)
____ Platino Plus (HMO-SNP)
____ Platino Advance (HMO-SNP)
____ Platino Blindao (HMO-SNP
____ Platino Enlace (HMO-SNP) )
$0 prima mensual para todos los planes
Favor indicar a cuál plan Grupal desea afiliarse (si aplica):
Cubierta: __________________________ Seleccionar: __ (HMO) __ (PPO)
Prima Mensual: _____________________ Fecha de Efectividad: ____________________
Número Seguro Social (Sólo para cubiertas grupales.): ________________________________________
Información del Beneficiario:
Nombre: __________________________ Apellidos: _____________________ [Opcional: Inicial]: ____ ____
Fecha de Nacimiento (MM/DD/AAAA): ________________ Sexo: __ F __ M
Número Teléfono Residencial: ________________________ Número Teléfono Alterno: _________________
(____ Marque aquí si es celular) (____ Marque aquí si es celular)
Y0082_107026S001_C CMS Approved 6/17/2025
SOLICITUD DE AFILIACIÓN 2026 P.3
Dirección Residencial Permanente (No indique PO Box. Nota: Para las personas sin hogar, un apartado
de correo puede considerarse su dirección de residencia permanente.): __________________________
_________________________________________________________________________________________
_________________________________________________________________________________________
Pueblo: ___________________, Estado __________________ Código Postal: _____________________
Dirección Postal, si es diferente de su dirección de residencia permanente. (Se permite PO Box):
Calle: ____________________________________________________________________________________
_________________________________________________________________________________________
Pueblo: ___________________, Estado___________________ Código Postal: _____________________
Información del su Seguro de Medicare:
Número de Medicare _ _ _ _ -_ _ _-_ _ _ _
Conteste estas preguntas importantes:
¿Va a tener otra cubierta de medicamentos recetados (como VA, TRICARE, etc.) además de Triple-S
Advantage? ___Sí ___No
Si la respuesta es “Sí”, por favor, haga una lista de las otras cubiertas y el(los) número(s) de identificación
para la(s) cubierta(s):
Nombre de la otra cubierta: Número de afiliado de la cubierta: Número de grupo de la cubierta:
_______________________ _____________________________ ____________________________
Si escogió alguna de nuestras cubiertas Platino, por favor conteste lo siguiente:
¿Está inscrito en el Programa Estatal de Medicaid? ___Sí ___No
Si respondió “Sí”, por favor proporcione su número de Medicaid (MPI): ___________________________
Si usted escoge afiliarse a Contigo Plus (HMO-SNP), por favor seleccione la condición crónica
con la que ha sido diagnosticado:
____ Diabetes Mellitus ____ Desorden Cardiovascular ____ Fallo Cardíaco Congestivo
Si usted escoge afiliarse a ContigoEnMente (HMO-SNP), por favor confirme si usted ha sido
diagnosticado con cualquier síntoma de demencia: ___Sí ___No
Y0082_107026S001_C CMS Approved 6/17/2025
SOLICITUD DE AFILIACIÓN 2026 P.4
IMPORTANTE: Leer y firmar abajo:
Al completar esta solicitud de afiliación, estoy de acuerdo con lo siguiente:
1. Al mantener ambos, la Parte A (Hospital) y Parte B (Cubierta Médica) para permanecer en Triple-S
Advantage.
2. Al unirme a este Plan Medicare Advantage, reconozco que Triple-S Advantage compartirá mi
información con Medicare, quien puede usarla para realizar seguimiento de mi afiliación, efectuar
pagos y para otros fines permitidos por la ley federal que autoriza la recopilación de esta información
(vea la Declaración de Privacidad al final de este formulario). Su respuesta a este formulario es
voluntaria. Sin embargo, la falta de respuesta puede afectar a la inscripción en el plan.
3. Entiendo que sólo puedo estar inscrito en un plan MA a la vez - y que la inscripción en este plan
terminará automáticamente mi inscripción en otro plan MA (se aplican excepciones para los planes
MA PFFS, MA MSA).
4. Entiendo que cuando comience mi cubierta de Triple-S Advantage, debo obtener todos mis
beneficios médicos y de medicamentos recetados de Triple-S Advantage. Los beneficios y servicios
proporcionados por Triple-S Advantage y contenidos en mi documento de “Evidencia de Cubierta” de
Triple-S Advantage (también conocido como contrato de afiliado o acuerdo con el afiliado) serán
cubiertos. Ni Medicare ni Triple-S Advantage pagarán los beneficios o servicios que no estén
cubiertos.
5. La información de este formulario de inscripción es correcta a mi mejor conocimiento. Entiendo que,
si proporciono intencionadamente información falsa en este formulario, se me dará de baja del plan.
6. Entiendo que mi firma (o la firma de la persona legalmente autorizada a actuar en mi nombre) en esta
solicitud significa que he leído y entendido el contenido de esta aplicación. Si lo firma una persona
autorizada (tal como se explicó antes), esta firma certifica que:
a . E s t a p e r s o n a e s t á a u t o r i z a d a bajo la ley del estado a completar esta solicitud, y
b. Los documentos de esta autorización estarán disponibles si se solicitan por Medicare.
Firma: ________________________________________ Fecha de hoy: _________________________
Solo para Solicitud de Afiliación Electrónica completada de forma presencial: Marcar “Afiliarme
Ahora” es considerado su firma. Afiliarme Ahora: _____ Fecha de hoy: ______________________
Solo para Solicitud de Afiliación completada por teléfono:
Número de Llamada (UCID): ___________________________ Fecha de hoy: ______________________
Testigo: _______________________________ Fecha de hoy: __________________________________
Si usted es un representante autorizado/representante legal, deberá firmar arriba y completar estos campos:
Nombre: ___________________________ Dirección: ____________________________________________
Teléfono: __________________________ Relación con Solicitante: ________________________________
Y0082_107026S001_C CMS Approved 6/17/2025
SOLICITUD DE AFILIACIÓN 2026 P.5
SECCIÓN 2
Todos los campos a continuación son opcionales:
Contestar estas preguntas es a su discreción. No se le puede denegar cubierta por no completarlas.
Seleccione si desea que le enviemos información en un idioma que no sea español:
Inglés ____ Otro (indique): __________________
Seleccione si desea que le enviemos información en un formato accesible:
Braille _____ Letra Agrandada ____ Audio CD _____ Data CD _____
Por favor, comuníquese con Triple-S Advantage al 1-888-620-1919 si usted necesita información en un
formato accesible o idioma además del mencionado anteriormente. Nuestro horario es de lunes a
domingo de 8:00 a.m. a 8:00 p.m. Los usuarios de TTY deben llamar al 1-866-620-2520.
¿Usted trabaja? ___Sí ___No ¿Su cónyuge trabaja? ___Sí ___No
Para cubiertas HMO - Por favor, seleccione el nombre de su Médico de Cuidado Primario
(PCP, por sus siglas en inglés), clínica o centro de salud de nuestro Directorio de Proveedores:
______________________________________ Teléfono: ________________________________
De no escoger un PCP, se le asignará uno de forma automática.
Deseo recibir los siguientes materiales e información por correo electrónico, seleccione uno o más:
____ Directorio de Proveedores
____ Notificación Anual de Cambios
____ Evidencia de Cubierta
____ Resumen de Beneficios
____ Formulario de Medicamentos Recetados
____ Material de promoción para mantener su salud, recordatorios de citas médicas,
y cualquier otra comunicación del Plan
____ Confirmación de Afiliación Electrónica
¿Usted acepta que nos comuniquemos utilizando la información de contacto provista por usted
o por medios autorizados por ley?
Mensaje de Texto: ____ Sí ____No Número ________________________________________
Correo electrónico: ____ Sí ____No Dirección: ______________________________________
Y0082_107026S001_C CMS Approved 6/17/2025
SOLICITUD DE AFILIACIÓN 2026 P.6
Para optar por no recibir comunicaciones a través de correo electrónico o mensajes de texto, puede
comunicarse en cualquier momento a nuestro Centro de Servicios al Afiliado al 1-888-620-1919 de lunes
a domingo de 8:00 a.m. a 8:00 p.m. Los usuarios de TTY (audio-impedidos) deben llamar al
1-866-620-2520. Tenga en cuenta que si opta por no recibir comunicaciones a través de correo
electrónico o mensajes de texto usted aún estará recibiendo comunicaciones transaccionales tales como
Determinaciones de Cubierta de Servicios entre otras.
Contacto de Emergencia: _____________________________ Teléfono: ___________________
Relación con usted: __________________________________
¿Es usted el retirado? Sí ___ No ___ (Solo para grupos patronales).
Si contestó “Sí”, indique fecha de retiro (mes/día/año): _________________
Si contestó no, indique el nombre del retirado: _______________________
¿Está cubriendo a su cónyuge o dependientes bajo este plan patronal? (Solo para grupos
patronales).
___Sí ___No ___No aplica
Si la contestación es “Sí”, nombre del cónyuge: ___________________________________
Nombre(s) de dependiente(s): __________________________________________________
¿Es usted residente de una institución de cuidados a largo plazo, como un hogar o asilo de
ancianos? ___Sí ___No
Si respondió “Sí”, por favor proporcione la siguiente información:
Nombre de la Institución: ______________________________________________________
Nombre del Administrador: ____________________________________________________
Número de teléfono de la Institución o Administrador: ______________________________
Plan Médico Actual: ___MMM ___Humana ___MCS ___Medicare Original ___Plan USA
___Otro:___________________________________
Y0082_107026S001_C CMS Approved 6/17/2025
SOLICITUD DE AFILIACIÓN 2026 P.7
Pagando su prima del plan:
Usted puede pagar su prima mensual del plan (incluyendo cualquier penalidad por inscripción
tardía que usted actualmente tenga o deba) por correo, transferencia electrónica de fondos (EFT,
por sus siglas en inglés) o tarjeta de crédito cada mes. También puede optar por pagar su
prima descontándola automáticamente cada mes de su beneficio del Seguro Social o de la
Junta de Retiro Ferroviario (RRB, por sus siglas en inglés).
Si tiene que pagar una cantidad de ajuste mensual relacionada con los ingresos de la Parte
D (Parte D-IRMAA), debe pagar esta cantidad extra además de su prima del plan. NO pague
la Parte D-IRMAA a Triple-S Advantage, Inc.
Por favor seleccione una opción para el pago de la prima
y/o penalidad por inscripción tardía:
Si usted no selecciona una opción de pago, se le enviará una libreta de cupones.
_____ Recibir una libreta de cupones.
_____ Transferencia electrónica de fondos (EFT, por sus siglas en inglés) de su cuenta bancaria
mensualmente.
Por favor, incluya un cheque con la palabra “VOIDED” escrita o provea lo siguiente:
Nombre del dueño de la cuenta: ______________________
Número de ruta del banco: __ __ __ __ __ __ __ __ __
Número de cuenta bancaria: __ __ __ __ __ __ __ __ __
Tipo de cuenta: _____ Cheques _____Ahorros
_____ Tarjeta de Crédito. Por favor, provea la siguiente información:
Tipo de tarjeta: __ Visa __ Master Card
Nombre del dueño de la cuenta según aparece en la tarjeta:
___________________________________________________
Número de la tarjeta: __ __ __ __ __ __ __ __ __ __ __ __ __ __ __ __
Fecha de Expiración: __ __ /__ __ __ __ (MM/AAAA)
_____ Deducción automática de su cheque mensual de beneficios del Seguro Social o Junta de
Retiro Ferroviario (RRB, por sus siglas en inglés).
Recibo los beneficios mensuales de: _____ Seguro Social _____ RRB
Y0082_107026S001_C CMS Approved 6/17/2025
SOLICITUD DE AFILIACIÓN 2026 P.8
(La deducción del Seguro Social/RRB puede tomar dos meses o más para comenzar después
que el Seguro Social o Junta de Retiro Ferroviario (RRB, por sus siglas en inglés) aprueba la
deducción. En la mayoría de los casos, si el Seguro Social o Junta de Retiro Ferroviario acepta su
solicitud de deducción automática, la primera deducción de su cheque de beneficios del Seguro
Social o Junta de Retiro Ferroviario incluirá todas las primas que deba de su inscripción desde la
fecha que entra en vigor hasta que comience la retención. Si el Seguro Social o la Junta de Retiro
Ferroviario no aprueba su solicitud para deducción automática, le enviaremos una factura por su
prima mensual.)
Información importante sobre los
Beneficios Suplementarios Especiales para Enfermos Crónicos:
Algunos de nuestros planes ofrecen Beneficios Suplementarios Especiales para Enfermos
Crónicos (SSBCI, por sus siglas en inglés), esto significa que, para ser elegible para recibir estos
beneficios, el afiliado debe cumplir con todo lo siguiente:
• Tener una o más afecciones crónicas comórbidas y médicamente complejas que ponen
en peligro la vida o limitan significativamente la salud general o la función del afiliado;
• Tener un alto riesgo de hospitalización u otros resultados adversos para la salud; y
• Requerir coordinación de cuidado intensivo.
Si eligió una cubierta que incluye Beneficios Suplementarios Especiales para Enfermos Crónicos
(SSBCI, por sus siglas en inglés), note que, para recibir estos beneficios debe cumplir con todos
los requisitos establecidos anteriormente, y que Triple-S realizará una verificación clínica para
poder validar su elegibilidad. Si después de la validación clínica no cumple con los requisitos, será
elegible para recibir todos los demás beneficios en el paquete de su plan a excepción de los
Beneficios Suplementarios Especiales para Enfermos Crónicos (SSBCI).
_____ Paquete Inicial (Resumen de Beneficios, Lista de Verificación de Pre-Afiliación)
_____ Notificación de Clasificación de Estrellas de Medicare
_____ Notificación de disponibilidad electrónica de la Evidencia de Cubierta, Formulario de
Medicamentos y Directorio de Proveedores y Farmacias
_____ Confirmación de Afiliación (si aplica)
_____ Copia de la Solicitud de Afiliación (si aplica)
_____ Certificación de Elegibilidad para un Periodo de Afiliación (si aplica)
_____ Precertificación de Enfermedades Crónicas (si aplica)
_____ Autorización para la Divulgación de Información Protegida de Salud (formulario PHI) (si aplica)
Certifico que recibí los siguientes documentos
por parte del representante de Triple-S Advantage:
Y0082_107026S001_C CMS Approved 6/17/2025
SOLICITUD DE AFILIACIÓN 2026 P.9
Lo siguiente solo aplica si no se proporcionó la Notificación de disponibilidad electrónica de
la Evidencia de Cubierta, el Formulario de Medicamentos y el Directorio de Proveedores y
Farmacias:
_____ Evidencia de Cubierta y Formulario de Equipo Médico Duradero (si aplica)
_____ Directorio de Proveedores y Farmacias (si aplica)
_____ Formulario de Medicamentos (si aplica)
Sólo para personas que ayuden al afiliado a completar este formulario
Complete esta sección si es usted un individuo (es decir, agente, bróker, un asesor de SHIP,
familiar u otros terceros) que ayuda a un afiliado a completar este formulario.
Nombre: ___________________________ Relación con el afiliado: ____________________
Firma: _____________________________
Número Nacional de Productor (NPN) (agentes/brókers solamente): _____________________
Para uso oficial solamente:
Fecha de Recibo: _______________________________________
ID # de Plan: ___________________ Fecha Efectiva de Cubierta: ___________________________
DECLARACIÓN DE LA LEY DE PRIVACIDAD
Los Centros de Servicios de Medicare y Medicaid (CMS, por sus siglas en inglés) recopilan
información de los planes de Medicare para rastrear la afiliación de los beneficiarios en planes
Medicare Advantage (MA), mejorar cuidado y para el pago de los beneficios de Medicare. Las
secciones 1851 de la Ley del Seguro Social y 42 CFR §§ 422.50 y 422.60 autorizan la
recopilación de esta información. CMS puede usar, divulgar e intercambiar datos de afiliación de
beneficiarios de Medicare como se especifica en el Aviso del Sistema de Registros (SORN, por
sus siglas en inglés) “Medicamentos recetados de Medicare Advantage (MARx)”, Sistema No.
09-70-0588. Su respuesta a este formulario es voluntaria. Sin embargo, no responder puede
afectar la afiliación en el plan.
Triple-S Advantage, Inc. es un concesionario independiente de BlueCross BlueShield Association.
Las cubiertas Platino están disponibles para cualquier persona que tenga tanto Asistencia Médica
del Estado como de Medicare.
Y0082_107026S001_C CMS Approved 6/17/2025
SOLICITUD DE AFILIACIÓN 2026 P.10
Aviso de Disponibilidad de Servicios de Asistencia Lingüística, Ayudas y Servicios Auxiliares /
Notice of Availability of Language Assistance Services and Auxiliary Aids and Services
English: ATTENTION: If you speak English, free language assistance services are available to you.
Appropriate auxiliary aids and services to provide information in accessible formats are also available free
of charge. Call 1-888-620-1919 (TTY/TDD 1-866-620-2520) or speak to your provider.
Español: ATENCION: Si usted habla español, servicios de asistencia lingüística gratuitos están
disponibles para usted. También están disponibles ayudas y servicios auxiliares apropiados para
proporcionar información en formatos accesibles, sin costo adicional. Llame al 1-888-620-1919
(TTY/TDD 1-866-620-2520) o hable con su proveedor.
Português: ATENÇÃO: Se você fala português do Brasil, tem à disposição serviços gratuitos de
assistência linguística. Auxílios e serviços auxiliares apropriados para fornecer informações em formatos
acessíveis também estão disponíveis gratuitamente. Ligue para 1-888-620-1919 (TTY 1-866-620-2520)
ou fale com seu provedor.
Français: ATTENTION: si vous parlez Français, des services d’assistance linguistique gratuits sont à votre
disposition. Des aides et services auxiliaires appropriés pour fournir des informations dans des formats
accessibles sont également disponibles gratuitement. Appelez le 1-888-620-1919 (TTY 1-866-620-2520)
ou parlez à votre fournisseur.
Italiano: ATTENZIONE: se parli italiano, sono disponibili servizi di assistenza linguistica gratuiti. Sono
inoltre disponibili gratuitamente ausili e servizi ausiliari adeguati per fornire informazioni in formati
accessibili. Chiama l’1-888-620-1919 (TTY 1-866-620-2520) o parla con il tuo fornitore.
Deutsch: ACHTUNG: Wenn Sie Deutsch sprechen, stehen Ihnen kostenlose Sprachassistenzdienste zur
Verfügung. Entsprechende Hilfsmittel und Dienste zur Bereitstellung von Informationen in barrierefreien
Formaten stehen ebenfalls kostenlos zur Verfügung. Rufen Sie 1-888-620-1919 (TTY +1-866-620-2520)
an oder sprechen Sie mit Ihrem Provider
Tagalog: PAALALA: Kung nagsasalita ka ng Tagalog, magagamit mo ang mga libreng serbisyong tulong
sa wika. Magagamit din nang libre ang mga naaangkop na auxiliary na tulong at serbisyo upang magbigay
ng impormasyon sa mga naa-access na format. Tumawag sa 1-888-620-1919 (TTY: 1-866-620-2520) o
makipag-usap sa iyong provider.
Y0082_107026S001_C CMS Approved 6/17/2025
SOLICITUD DE AFILIACIÓN 2026 P.11
Y0082_107026S001_C CMS Approved 6/17/2025
ADVANTAGE
CUESTIONARIO DE TRABAJO DE PERSONAS DE EDAD AVANZADA
Apellidos: Nombre: Inicial:
__________________________________________ ______________________________ _______
Número de Identificador de Medicare:_______________________________________________
1. ¿Actualmente, ¿usted se encuentra empleado por alguna entidad pública, privada o sin fines de
lucro? _______ Sí _______ No
2. ¿Tiene usted seguro médico provisto por su patrono? _______ Sí _______ No _______No aplica
3. Si usted es casado (a), ¿tiene algún seguro médico a través del patrono de su cónyuge?
_______ Sí _______ No _______No aplica
4. De tener seguro médico provisto por un patrono, por favor provea la siguiente información:
Nombre de su patrono:___________________________________________________________________
Dirección de su patrono: _________________________________________________________________
Nombre de la aseguradora del seguro médico: ______________________________________________
Nombre del asegurado principal: ___________________________________________________________
Número de grupo: _______________________________________________________________________
Número de Contrato: ____________________________________________________________________
5. ¿Tiene este patrono más de 20 empleados? _______ Sí _______ No _______No aplica
6. ¿Tiene usted planes de desafiliarse de esta cubierta patronal próximamente?
_______Tres meses _______Seis meses _______Un año _______Todavía no _______No aplica
7. ¿Tiene usted su propio negocio? _______ Sí _______ No
Los Centros de Servicio de Medicare y Medicaid (CMS, por sus siglas en inglés) requieren que reportemos
el estatus de trabajo actual de nuestros afiliados. Para poder reportar su estado de manera precisa, por
favor conteste esta encuesta. Esta encuesta no afectará su cubierta de Medicare o su membresía en el
Plan de Medicare que haya elegido. Por favor, complete la siguiente información.
CUESTIONARIO DE TRABAJO DE PERSONAS DE EDAD AVANZADA P.2
8. Si tiene su propio negocio, ¿tiene seguro médico a través de su negocio?
_______ Sí _______ No _______No aplica
9. Si tiene seguro médico a través de su negocio, por favor provea la siguiente información:
Nombre de su negocio: __________________________________________________________________
Dirección de su negocio: _________________________________________________________________
Nombre de la aseguradora del seguro médico de su negocio: _________________________________
Número de grupo:_______________________________________________________________________
Número de Contrato: ____________________________________________________________________
10. ¿Alguna vez le han rechazado de un seguro médico a través de su patrono? _______ Sí _______ No
Y0082_26CI002S_C Triple-S Advantage es un concesionario independiente de BlueCross BlueShield Association.
CERTIFICACIÓN DE ELEGIBILIDAD PARA UN PERIODO DE AFILIACIÓN
ADVANTAGE
Típicamente, usted se puede afiliar a un plan Advantage de Medicare solamente durante el periodo
anual de afiliación, desde el 15 de octubre al 7 de diciembre de cada año. Existen excepciones que
le permiten afiliarse a un plan Advantage de Medicare fuera de este periodo.
Por favor, lea las siguientes declaraciones cuidadosamente y marque el encasillado si la aseveración le
aplica. Al marcar cualquiera de los siguientes encasillados usted está certificando que, a su mejor entender,
usted es elegible para un Periodo de Afiliación. Si nosotros determinamos que esta información es
incorrecta, usted podrá ser desafiliado.
Soy nuevo en Medicare.
Estoy afiliado en un plan Medicare Advantage y quiero hacer un cambio durante el Periodo de
Afiliación Abierta de Medicare Advantage (OEP MA).
Recientemente me mudé fuera del área de servicio de mi plan actual o recientemente me mudé y
tengo nuevas opciones para mí. Me mudé en (indique la fecha) _________________________.
Recientemente fui excarcelado. Fui liberado (indique la fecha) ________________________________.
Recientemente regresé a los Estados Unidos, después de vivir permanentemente fuera de EE.UU.
Regresé a los EE.UU. en (indique la fecha) ________________________________________________.
Recientemente he obtenido presencia legal en los EE.UU. (indique la fecha) ____________________.
Recientemente tuve un cambio en mi Medicaid (recientemente adquirí Medicaid, tuve un cambio en
el nivel de asistencia de Medicaid, o perdí Medicaid) en (inserte la fecha) _______________________.
Recientemente sufrí un cambio en mi Ayuda Adicional para pagar la cubierta de medicamentos
recetados de Medicare (recientemente recibí Ayuda Adicional, tuve un cambio en el nivel de Ayuda
Adicional o perdí Ayuda Adicional) en (inserte la fecha) ______________________________________.
Tengo Medicare y recibo todos los beneficios de Medicaid. Quiero inscribirme o cambiarme a un
plan que coordine la cubierta entre mis planes de Medicare y Medicaid (llamado Plan de
Necesidades Especiales de Doble Elegibilidad (D-SNP) integrado).
Me estoy mudando, estoy viviendo, o recientemente me mudé fuera de una Institución de Cuidado
Prolongado (por ejemplo, un asilo de ancianos o una institución de cuidado prolongado). Me mudé
a/fuera de una institución en (indique la fecha) _____________________________________________.
Recientemente dejé un programa PACE en (indique la fecha) ________________________________.
Recientemente perdí involuntariamente mi cubierta acreditable de medicamentos recetados (una
cubierta tan buena como la de Medicare). Perdí la cubierta de medicamentos en (indique la fecha)
_________________________.
Y0082_26CI003S_C
CERTIFICACIÓN DE ELEGIBILIDAD PARA UN PERIODO DE AFILIACIÓN P.2
ADVANTAGE
Estoy dejando una cubierta patronal en (indique la fecha) ____________________________________.
Estoy en un Programa de Asistencia Farmacéutica Estatal calificado o estoy perdiendo la ayuda de un
Programa de Asistencia Farmacéutica Estatal.
Mi plan está terminando su contrato con Medicare, o Medicare está terminando el contrato con mi plan.
Fui afiliado en un plan por Medicare (o mi estado) y deseo elegir un plan diferente. Mi afiliación en ese
plan comenzó en (inserte la fecha) ______________________________.
Estaba afiliado a un Plan de Necesidades Especiales (SNP, por sus siglas en inglés) pero ya no
cumplo con los requisitos para pertenecer a un plan de necesidades especiales. Fui desafiliado del
SNP en (indique la fecha) _________________________________________________.
Fui afectado por una emergencia o un desastre mayor (según lo declarado por la Agencia Federal para
el Manejo de Emergencias (FEMA, por sus siglas en inglés) o por una entidad Federal, o de gobierno
estatal o local). Una de las otras declaraciones aquí me aplicaba, pero no pude completar mi solicitud
de afiliación debido al desastre natural.
Si ninguna de estas declaraciones le aplica o no está seguro, por favor llame a Triple-S Advantage al
1-888-620-1919 para información adicional. Si usted es usuario de equipo TTY debe llamar al
1-866-620-2520 para verificar si es elegible para afiliarse. Nuestro horario de oficina es de lunes a
domingo de 8:00 a.m. a 8:00 p.m.
Y0082_26CI003S_C Triple-S Advantage, Inc. Es un concesionario independiente de la BlueCross and BlueShield Association.
TRANSICIÓN DE SERVICIOS PARA NUEVOS AFILIADOS
Apellidos: Nombre: Inicial:
_________________________________________________ ____________________________ ______
Fecha de Nacimiento Mes:_______ Día:_______ Año:_______
Teléfono 1:__________________________________ Teléfono 2:________________________________
Plan de beneficios: ______________________________________________________________________
Fecha de efectividad Mes:_______ Día:_______ Año:_______
__ Cama eléctrica
__ Silla de Ruedas
__ Glucómetro*
__ Bomba de Insulina
__ Nebulizador**
__ Medicamentos de
terapia (Respiratoria)
__ Alimentación por tubo
o boca (formula)
__ Ostomía/Urostomía***
__ Curación de úlceras
__ Servicio de enfermería
o terapias
__ Oxígeno
__ CPAP
__ Medicamentos
inyectables /IV
Por favor verifique si el asegurado tiene el siguiente Equipo/Servicio en el Hogar
Equipo/Suplidos/Servicio Proveedor/ Desde cuando Comentarios
(mes y año)
* Glucómetro – máquina para medir nivel de glucosa en diabéticos.
** Nebulizador – máquina para terapias respiratorias
*** Ostomías – gastrostomías, colostomías, etc. Ver necesidad de suplidos para el cuidado.
Información provista por: ________________________________________________________________
________________________________ _____________________ _____________
Firma del Representante del Plan Región Fecha
Y0082_26CI017S_C
SHIC # (NUMERO DE MEDICARE): ________________________________________________________
Compañía
Plan Médico
Previo
ADVANTAGE
LISTA DE VERIFICACIÓN DE PRE-AFILIACIÓN
ADVANTAGE
Antes de tomar una decisión de afiliación, es importante que entienda completamente nuestros beneficios
y reglas. Si tiene alguna pregunta, puede llamar y hablar con un representante de servicio al cliente al
1-888-620-1919 (TTY 1-866-620-2520).
Y0082_26CI005S_C
ENTENDIENDO LOS BENEFICIOS
 La Evidencia de Cubierta (EOC, por sus siglas en inglés) proporciona una lista completa de todo los
que se cubre y los servicios. Es importante revisar la cubierta del plan, los costos y beneficios antes de
afiliarse. Visite www.sssadvantage.com o llame al 1-888-620-1919 (TTY 1-866-620-2520) para
solicitar una copia del EOC.
 Revise el Directorio de Proveedores (o consulte a su médico) para asegurarse de que los médicos que
ve ahora estén en la red. Si no están en la lista, significa que probablemente tendrá que seleccionar un
nuevo médico.
 Revise el Directorio de Farmacias para asegurarse de que la farmacia que usa para cualquier
medicamento con receta se encuentre en la red. Si la farmacia no está en la lista, es probable que
deba seleccionar una nueva farmacia para sus recetas.
 Revise el formulario para asegurarse de que sus medicamentos están cubiertos.
ENTENDIENDO REGLAS IMPORTANTES
 Además de su prima mensual del plan (si aplica), debe continuar pagando su prima de la Parte B de
Medicare. Esta prima generalmente se saca de su cheque de Seguro Social cada mes.
 Los beneficios, las primas y/o los copagos/coaseguros pueden cambiar el 1 de enero de 2027.
 Excepto en situaciones de emergencia o urgencia, no cubrimos servicios de proveedores fuera de la
red (médicos que no están incluidos en el Directorio de Proveedores).
 Para cubiertas PPO y HMO-POS - Nuestro plan le permite ver proveedores fuera de nuestra red
(proveedores no contratados). Sin embargo, aunque pagaremos los servicios cubiertos o ciertos
servicios cubiertos provistos por un proveedor no contratado, el proveedor debe aceptar tratarlo.
Excepto en situaciones de emergencia o urgencia, los proveedores no contratados pueden denegar
atenderle. Además, puede pagar un costo más alto por los servicios recibidos por proveedores no
contratados.
 Para cubierta de Necesidades Especiales para Condiciones Crónicas - Este plan es un plan de
necesidades especiales para condiciones crónicas (C-SNP, por sus siglas en inglés). Su capacidad para
afiliarse se basará en la verificación de que usted tiene una condición crónica severa o incapacitante
específica calificada.
 Para cubiertas Platino - Este plan es un plan de elegibilidad dual para necesidades especiales (D-SNP,
por sus siglas en inglés). Su capacidad para afiliarse se basará en la verificación de que tiene derecho
tanto a Medicare como al plan de asistencia médica del estado bajo Medicaid.
 Si actualmente está afiliado a un plan Medicare Advantage, su cubierta médica actual de Medicare
Advantage finalizará una vez comience su nueva cubierta de Medicare Advantage. Si tiene Tricare, su
cubierta puede verse afectada una vez comience su nueva cubierta de Medicare Advantage. Para más
información, favor de contactar a Tricare. Si tiene un plan Medigap, una vez comience su cubierta
Medicare Advantage, es posible que desee cancelar su póliza Medigap porque estará pagando por una
cubierta que no podrá utilizar.
Triple-S Advantage es un concesionario

**END OF MASTER REQUIREMENTS GUIDE**
