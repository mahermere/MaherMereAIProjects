# ViewModel Quick Reference Guide

## EnrollmentWizardViewModel

### Properties Summary
```
CurrentStep: int (1-5)
TotalSteps: int (returns 5)
CurrentEnrollment: EnrollmentRecord
SOANumber: string

Step 1 - Personal Information:
  - FirstName, LastName, MiddleInitial
  - DateOfBirth, Gender
  - MedicareNumber, SSN, EmailAddress

Step 2 - Contact Information:
  - PrimaryPhone, SecondaryPhone
  - PrimaryPhoneIsMobile, SecondaryPhoneIsMobile
  - EmailAddress, PreferredContactMethod

Step 3 - Medicare Information:
  - HasOtherInsurance, OtherCoverageType
  - CurrentlyInMA, CurrentPlanName, ReasonForChange

Step 4 - Review & Confirmation:
  - EmergencyContactName, EmergencyContactPhone, EmergencyRelationship

Step 5 - Signature Capture:
  - BeneficiarySignatureBase64
  - UsesXMark

Triple-S Fields:
  - DeviceInfo, IPAddress, GPSCoordinates
  - FormVariant, OMBControlNumber, AttestationTimestamp
  - SubmissionLocationType, ApplicationDate, EnrollmentMechanism
  - FormIdentifier, EffectiveDate, CreatedDate, LastModifiedDate
```

### Key Methods
```csharp
// Navigation
await GoToNextStepAsync()      // Move to next step
await GoToPreviousStepAsync()  // Move to previous step

// Validation
var (isValid, error) = await ValidateCurrentStepAsync()

// Submission
bool success = await SubmitEnrollmentAsync()

// Display
string summary = GetEnrollmentSummary()
string title = GetStepTitle()      // English
string titulo = GetStepTitleSpanish() // Spanish
```

### Step Validation Rules
```
Step 1: FirstName, LastName, DateOfBirth, MedicareNumber (required)
Step 2: PrimaryPhone, EmailAddress (required)
Step 3: Conditional - if HasOtherInsurance, need OtherCoverageType
        Conditional - if CurrentlyInMA, need PlanName & ReasonForChange
Step 4: EmergencyContactName, EmergencyContactPhone (required)
Step 5: UsesXMark OR BeneficiarySignatureBase64 (required)
```

---

## SOAWizardViewModel

### Properties Summary
```
CurrentStep: int (1-4)
TotalSteps: int (returns 4)
CurrentSOA: SOAFirstPageRecord
SOANumber: string

Step 1 - Beneficiary Information:
  - BeneficiaryFirstName, BeneficiaryLastName
  - BeneficiaryDOB
  - MedicareNumber
  - PhoneNumber

Step 2 - Coverage Scope:
  - MedicareAdvantageSelected
  - PartDSelected
  - SupplementalSelected
  - DentalVisionSelected
  - HearingAidSelected
  - WellnessSelected
  - ProductInformationProvided

Step 3 - Meeting Details:
  - MeetingDate
  - MeetingLocation
  - ComplianceDocumentsProvided

Step 4 - Signatures:
  - BeneficiarySignatureBase64
  - AgentSignatureBase64
```

### Key Methods
```csharp
// Navigation
await GoToNextStepAsync()       // Move to next step
await GoToPreviousStepAsync()   // Move to previous step

// Validation & Saving
bool success = await SaveCurrentStepAsync()

// Validation
var (isValid, error) = await ValidateCurrentStepAsync()

// Submission
bool success = await SubmitSOAAsync()

// Display
string summary = GetSOASummary()
string title = GetStepTitle()      // English
string titulo = GetStepTitleSpanish() // Spanish
```

### Step Validation Rules
```
Step 1: BeneficiaryFirstName, BeneficiaryLastName, MedicareNumber, PhoneNumber (required)
Step 2: ProductInformationProvided = true
        At least one product selected (MA, PartD, Supplemental, Dental/Vision, Hearing, Wellness)
Step 3: ComplianceDocumentsProvided = true
        MeetingDate != default
Step 4: BeneficiarySignatureBase64 (required)
        AgentSignatureBase64 (required)
```

---

## DashboardViewModel

### Properties Summary
```
AgentName: string              // Loaded from AgentSessionService
MonthlySOACount: int           // Count of SOAs this month
MonthlyEnrollmentCount: int    // Count of enrollments this month
PendingSOACount: int           // Count of pending SOAs
CompletedSOACount: int         // Count of completed SOAs
IsLoading: bool                // Indicates data loading state

Enrollments: ObservableCollection<EnrollmentItemViewModel>
  - EnrollmentNumber
  - BeneficiaryName
  - Status (Uploaded/Pending)
  - DateCreated
```

### Key Methods
```csharp
// Data Loading (called automatically in constructor)
LoadDashboardData() // async - loads from services

// Automatic features:
// - Loads agent name from AgentSessionService
// - Loads enrollment records from EnrollmentService
// - Populates Enrollments collection
// - Calculates statistics
```

### Data Sources
```
AgentSessionService:
  - CurrentAgentName: Agent's display name
  - CurrentAgentNPN: Agent's NPN number

EnrollmentService:
  - ActiveEnrollmentRecords: List of enrollment records
    - Each record includes: EnrollmentNumber, BeneficiaryName, DateCreated, IsUploaded
```

---

## AgentLoginViewModel

### Properties Summary
```
NPN: string                    // Agent's NPN number
ErrorMessage: string           // Display validation/error messages
IsLoading: bool               // Indicates authentication in progress
```

### Key Methods
```csharp
// Authentication
var (success, message, agentId) = await AuthenticateAsync(npn, password)
```

### Validation Rules
```
NPN:
  - Required (non-empty)
  - Must be 8-10 characters
  - Must contain only digits

Password:
  - Required (non-empty)
  - Must be at least 6 characters
```

### Return Values
```
(success: bool, message: string, agentId: string?)

Examples:
- (false, "NPN is required", null)
- (false, "NPN must be 8-10 digits", null)
- (false, "NPN must contain only digits", null)
- (false, "Password is required", null)
- (false, "Password must be at least 6 characters", null)
- (true, "Authentication successful", "123456789")
```

---

## Common Usage Examples

### Enrollment Wizard Navigation
```csharp
// Move to next step with validation
if (enrollmentVM.CurrentStep < enrollmentVM.TotalSteps)
{
    var (isValid, error) = await enrollmentVM.ValidateCurrentStepAsync();
    if (isValid)
    {
        await enrollmentVM.GoToNextStepAsync();
    }
}

// Submit enrollment
bool success = await enrollmentVM.SubmitEnrollmentAsync();
if (success)
{
    // Navigate to success page
}
```

### SOA Wizard Navigation
```csharp
// Save current step and move to next
bool success = await soaVM.SaveCurrentStepAsync();
if (success)
{
    // Step was validated and saved, user moved to next step
}
else
{
    // Show error message: soaVM.ErrorMessage
}
```

### Dashboard Loading
```csharp
// Automatically loads on instantiation
var dashboardVM = new DashboardViewModel();

// Access data (already loaded)
foreach (var enrollment in dashboardVM.Enrollments)
{
    // Display enrollment item
}
```

### Agent Login
```csharp
var loginVM = new AgentLoginViewModel();
var (success, message, agentId) = await loginVM.AuthenticateAsync(npn, password);

if (success)
{
    // Set session and navigate to dashboard
    AgentSessionService.SetSession(agentId, agentName);
}
else
{
    // Display error: message
}
```

---

## Properties Change Notification

All ViewModels properly implement `INotifyPropertyChanged` through inheritance from `BaseViewModel`. 
This means UI bindings will automatically update when properties change.

Example XAML binding:
```xaml
<Entry Text="{Binding FirstName}" />
<Label Text="{Binding Binding ErrorMessage, StringFormat='Error: {0}'}" />
```

---

## Error Handling Pattern

All async methods follow this pattern:
```csharp
try
{
    IsLoading = true;
    ErrorMessage = string.Empty;
    // ... do work ...
    return success;
}
catch (Exception ex)
{
    ErrorMessage = $"Error: {ex.Message}";
    return false;
}
finally
{
    IsLoading = false;
}
```

---

## Language Support

All ViewModels with UI text provide bilingual support:

```csharp
// English
string title = viewModel.GetStepTitle();

// Spanish (Español)
string titulo = viewModel.GetStepTitleSpanish();

// For dynamic language switching, integrate with LanguageService
// The UI code-behind handles updating text via SetLocalizedText() pattern
```

---

## Service Integration Points

### Required Services
1. **SOANumberService** - Generate unique document numbers
2. **EnrollmentService** - Manage enrollment records
3. **AgentSessionService** - Current agent session info

### Optional Services (for TODO items)
1. **AuthenticationService** - Actual credential validation (currently mocked)
2. **DocumentService** - PDF generation
3. **FileUploadService** - DMS integration
4. **NotificationService** - Email/SMS notifications
