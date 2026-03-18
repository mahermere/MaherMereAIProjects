# Technical Implementation Guide

## Architecture Overview

The ViewModels follow the MVVM (Model-View-ViewModel) pattern, providing a clean separation between UI and business logic.

```
UI Layer (XAML Pages)
    ↓ Data Binding
ViewModel Layer (Properties + Commands)
    ↓ Service Calls
Service Layer (Business Logic)
    ↓
Data Layer (Services + Models)
```

## Class Hierarchy

```
BaseViewModel : INotifyPropertyChanged
    ├── EnrollmentWizardViewModel
    ├── SOAWizardViewModel
    ├── DashboardViewModel
    └── AgentLoginViewModel
```

## Property Binding Pattern

All ViewModels use the same property pattern:

```csharp
private string? _fieldName;

public string? FieldName
{
    get => _fieldName;
    set => SetProperty(ref _fieldName, value);
}
```

The `SetProperty<T>()` method:
- Compares old and new values (prevents unnecessary updates)
- Updates the backing field
- Triggers PropertyChanged event
- Enables automatic UI updates

## Validation Pattern

Each ViewModel implements step-specific validation:

```csharp
public async Task<(bool Success, string ErrorMessage)> ValidateCurrentStepAsync()
{
    return await Task.FromResult(CurrentStep switch
    {
        1 => ValidateStep1() ? (true, string.Empty) : (false, "Error message"),
        // ...
        _ => (false, "Invalid step")
    });
}

private bool ValidateStep1()
{
    return !string.IsNullOrWhiteSpace(FirstName)
        && !string.IsNullOrWhiteSpace(LastName);
}
```

This pattern allows:
- Step-specific validation rules
- Clear error messages
- Async-ready validation
- Easy unit testing

## Navigation Pattern

```csharp
public async Task<bool> GoToNextStepAsync()
{
    if (CurrentStep < TotalSteps)
    {
        CurrentStep++;
        await Task.Delay(200); // Allow UI to update
        return true;
    }
    return false;
}
```

Usage in UI:
```csharp
if (await _viewModel.ValidateCurrentStepAsync())
{
    await _viewModel.GoToNextStepAsync();
    UpdateUI();
}
```

## Error Handling Pattern

All async methods follow this pattern:

```csharp
public async Task<bool> SubmitAsync()
{
    try
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        
        // Perform work
        var result = await DoWork();
        
        return true;
    }
    catch (Exception ex)
    {
        ErrorMessage = ex.Message;
        return false;
    }
    finally
    {
        IsLoading = false;
    }
}
```

Benefits:
- User-friendly error messages
- Loading state management
- Guaranteed cleanup
- Easy error propagation

## Data Flow Examples

### Example 1: Enrollment Submission

```
User fills out Step 1
    ↓
User clicks "Next"
    ↓
UI calls ValidateCurrentStepAsync()
    ↓
ViewModel validates FirstName, LastName, etc.
    ↓
If valid: UI calls GoToNextStepAsync()
    ↓
ViewModel increments CurrentStep
    ↓
UI updates to show Step 2
```

### Example 2: Dashboard Loading

```
DashboardPage constructor
    ↓
Creates DashboardViewModel
    ↓
ViewModel constructor calls LoadDashboardData()
    ↓
LoadDashboardData() calls services
    ↓
EnrollmentService returns active records
    ↓
ViewModel populates Enrollments collection
    ↓
XAML bindings automatically update UI
```

### Example 3: Login Flow

```
User enters NPN and Password
    ↓
User clicks Login
    ↓
UI calls AuthenticateAsync(npn, password)
    ↓
ViewModel validates inputs
    ↓
ViewModel calls authentication service
    ↓
If success: Returns AgentId
    ↓
UI saves session and navigates to Dashboard
```

## Service Integration

### SOANumberService

```csharp
// Generate unique numbers
string soaNumber = new SOANumberService().GenerateSOANumber();
string enrollmentNumber = new SOANumberService().GenerateEnrollmentNumber();

// Add records
EnrollmentService.AddEnrollmentRecord(new EnrollmentService.EnrollmentRecord
{
    EnrollmentNumber = number,
    FirstName = name,
    DateCreated = DateTime.Now,
    FilePath = path
});
```

### AgentSessionService

```csharp
// Check if logged in
if (AgentSessionService.IsAgentLoggedIn)
{
    // Get current agent info
    string npn = AgentSessionService.CurrentAgentNPN;
    string name = AgentSessionService.CurrentAgentName;
}

// Set session
AgentSessionService.SetSession("123456789", "John Doe");

// Clear session (logout)
AgentSessionService.ClearSession();
```

### EnrollmentService

```csharp
// Get active records
var records = EnrollmentService.ActiveEnrollmentRecords;

// Each record has:
// - EnrollmentNumber
// - BeneficiaryName
// - DateCreated
// - IsUploaded
// - FilePath
```

## Testing Guidelines

### Unit Testing Validation Logic

```csharp
[Fact]
public void ValidateStep1_WithoutFirstName_ReturnsFalse()
{
    var vm = new EnrollmentWizardViewModel();
    vm.LastName = "Doe";
    vm.MedicareNumber = "123456";
    
    var result = vm.ValidateStep1();
    
    Assert.False(result);
}
```

### Unit Testing Navigation

```csharp
[Fact]
public async Task GoToNextStepAsync_IncrementCurrentStep()
{
    var vm = new EnrollmentWizardViewModel();
    var initialStep = vm.CurrentStep;
    
    await vm.GoToNextStepAsync();
    
    Assert.Equal(initialStep + 1, vm.CurrentStep);
}
```

### Integration Testing with Mock Service

```csharp
[Fact]
public async Task LoadDashboardData_WithMockService_PopulatesEnrollments()
{
    var mockService = new Mock<IEnrollmentService>();
    mockService.Setup(s => s.ActiveEnrollmentRecords)
        .Returns(new List<EnrollmentRecord> { /* ... */ });
    
    var vm = new DashboardViewModel(mockService.Object);
    
    Assert.NotEmpty(vm.Enrollments);
}
```

## Common Implementation Patterns

### Pattern 1: Optional Fields

```csharp
// Optional field - may be null
private string? _optionalField;

public string? OptionalField
{
    get => _optionalField;
    set => SetProperty(ref _optionalField, value);
}

// In validation:
if (!string.IsNullOrWhiteSpace(OptionalField))
{
    // Validate if provided
}
```

### Pattern 2: Dependent Fields

```csharp
// Step 3 example: Conditional validation
private bool _currentlyInMA;

if (CurrentlyInMA)
{
    // Require CurrentPlanName and ReasonForChange
    return !string.IsNullOrWhiteSpace(CurrentPlanName)
        && !string.IsNullOrWhiteSpace(ReasonForChange);
}
```

### Pattern 3: Collection Properties

```csharp
// Use ObservableCollection for lists that update UI
private List<string> _dependents = new();

public List<string> Dependents
{
    get => _dependents;
    set => SetProperty(ref _dependents, value);
}

// Usage:
Dependents.Add("Dependent Name");
```

### Pattern 4: Enum-Style Properties

```csharp
// Instead of enum, use string properties with limited values
private string? _gender;

public string? Gender
{
    get => _gender;
    set => SetProperty(ref _gender, value);
}

// In validation or UI:
if (Gender == "Male" || Gender == "Female" || Gender == "Other")
{
    // Valid
}
```

## Performance Optimization Tips

### 1. Minimize Property Changes

```csharp
// Bad - triggers PropertyChanged 3 times
FirstName = "John";
LastName = "Doe";
EmailAddress = "john@example.com";

// Better - do all at once
var enrollment = CurrentEnrollment;
enrollment.FirstName = "John";
enrollment.LastName = "Doe";
enrollment.EmailAddress = "john@example.com";
```

### 2. Use Async for Long Operations

```csharp
// Bad - blocks UI
var records = LoadAllRecords(); // Could take 5 seconds

// Good - allows UI updates
var records = await LoadAllRecordsAsync();
```

### 3. Cache Service Results

```csharp
// Don't call service multiple times
var records = EnrollmentService.ActiveEnrollmentRecords;
var count1 = records.Count;
var count2 = records.Count; // Uses same collection
```

## Debugging Tips

### Enable Binding Debugging in XAML

```xaml
<Entry Text="{Binding FirstName, Mode=TwoWay, 
    UpdateSourceEventName=TextChanged, 
    StringFormat='First Name: {0}'}" />
```

### Monitor PropertyChanged Events

```csharp
// In ViewModel
public event PropertyChangedEventHandler? PropertyChanged;

protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
{
    System.Diagnostics.Debug.WriteLine($"Property changed: {propertyName}");
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
```

### Log Validation Results

```csharp
private bool ValidateStep1()
{
    bool isValid = !string.IsNullOrWhiteSpace(FirstName);
    System.Diagnostics.Debug.WriteLine($"Step 1 validation: {isValid}");
    return isValid;
}
```

## Best Practices

✅ **DO**
- Use async/await for all I/O operations
- Validate user input in ViewModel, not UI
- Keep ViewModel independent of UI framework
- Use ObservableCollection for dynamic lists
- Provide meaningful error messages
- Always implement try-catch-finally for safety
- Clear ErrorMessage at start of async operations

❌ **DON'T**
- Perform long operations on UI thread
- Hardcode values (use services)
- Mix UI logic with ViewModel logic
- Create tight coupling to services (use DI)
- Ignore async/await (causes UI freezing)
- Leave IsLoading = true if exception occurs (use finally)
- Create deeply nested property paths

---

## Version Information

- **Framework**: .NET 9
- **Platform**: .NET MAUI
- **Language**: C# 13.0
- **Pattern**: MVVM
- **Status**: Production Ready

