using TripleS.SOA.AEP.Models;
using TripleS.SOA.AEP.Models.Domain;
using TripleS.SOA.AEP.Models.ViewModels;

namespace TripleS.SOA.AEP.UI.ViewModels
{
    /// <summary>
    /// ViewModel for Enrollment Wizard (9-step process)
    /// Manages the complete enrollment workflow
    /// </summary>
    public class EnrollmentWizardViewModel : BaseViewModel
    {
        private int _currentStep;
        private Enrollment? _currentEnrollment;
        private string? _errorMessage;
        private bool _isLoading;
        private string? _soaNumber;
        /// <summary>
        /// Gets or sets the SOA number associated with this enrollment
        /// </summary>
        public string? SOANumber
        {
            get => _soaNumber;
            set => SetProperty(ref _soaNumber, value);
        }

        // Step 1 - Personal Information
        private string? _firstName;
        private string? _lastName;
        private DateTime _dateOfBirth;
        private string? _gender;
        private string? _phoneNumber;
        private string? _emailAddress;
        private string? _medicareNumber;

        // Step 2 - Address Information
        private string? _addressLine1;
        private string? _addressLine2;
        private string? _city;
        private string? _state = "PR";
        private string? _zipCode;
        private bool _differentMailingAddress;

        // Step 3 - Current Coverage
        private bool _hasOtherInsurance;
        private string? _otherCoverageType;
        private bool _currentlyInMA;
        private string? _currentPlanName;
        private string? _reasonForChange;

        // Step 4 - Emergency Contact
        private string? _emergencyContactName;
        private string? _emergencyContactPhone;
        private string? _emergencyRelationship;

        // Step 5 - Plan Selection
        private string? _selectedPlanName;
        private string? _paymentMethod;

        // Step 6 - Special Circumstances
        private bool _qualifiesSNP;
        private string? _snpType;
        private bool _wantsMSA;
        private bool _dualEligible;
        private bool _hasESRD;
        private bool _receivesLIS;

        // Step 7 - Dependents
        private List<string> _dependents = new();

        // Step 8 - Signature
        private byte[]? _beneficiarySignatureData;
        
            // Triple-S-specific fields
            private string? _deviceInfo;
            private string? _ipAddress;
            private string? _gpsCoordinates;
            private string? _formVariant;
            private string? _ombControlNumber;
            private DateTime _attestationTimestamp;
            private string? _submissionLocationType;
            private string? _applicationDate;
            private string? _enrollmentMechanism;
            private string? _formIdentifier;
            private DateTime _effectiveDate;
            private DateTime _createdDate;
            private DateTime _lastModifiedDate;

        public int CurrentStep
        {
            get => _currentStep;
            set => SetProperty(ref _currentStep, value);
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Step 1 Properties
        public string? FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string? LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set => SetProperty(ref _dateOfBirth, value);
        }

        public string? Gender
        {
            get => _gender;
            set => SetProperty(ref _gender, value);
        }

        public string? PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        public string? EmailAddress
        {
            get => _emailAddress;
            set => SetProperty(ref _emailAddress, value);
        }

        public string? MedicareNumber
        {
            get => _medicareNumber;
            set => SetProperty(ref _medicareNumber, value);
        }

        // Step 2 Properties
        public string? AddressLine1
        {
            get => _addressLine1;
            set => SetProperty(ref _addressLine1, value);
        }

        public string? AddressLine2
        {
            get => _addressLine2;
            set => SetProperty(ref _addressLine2, value);
        }

        public string? City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }

        public string? State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        public string? ZipCode
        {
            get => _zipCode;
            set => SetProperty(ref _zipCode, value);
        }

        public bool DifferentMailingAddress
        {
            get => _differentMailingAddress;
            set => SetProperty(ref _differentMailingAddress, value);
        }

        // Step 3 Properties
        public bool HasOtherInsurance
        {
            get => _hasOtherInsurance;
            set => SetProperty(ref _hasOtherInsurance, value);
        }

        public string? OtherCoverageType
        {
            get => _otherCoverageType;
            set => SetProperty(ref _otherCoverageType, value);
        }

        public bool CurrentlyInMA
        {
            get => _currentlyInMA;
            set => SetProperty(ref _currentlyInMA, value);
        }

        public string? CurrentPlanName
        {
            get => _currentPlanName;
            set => SetProperty(ref _currentPlanName, value);
        }

        public string? ReasonForChange
        {
            get => _reasonForChange;
            set => SetProperty(ref _reasonForChange, value);
        }

        // Step 4 Properties
        public string? EmergencyContactName
        {
            get => _emergencyContactName;
            set => SetProperty(ref _emergencyContactName, value);
        }

        public string? EmergencyContactPhone
        {
            get => _emergencyContactPhone;
            set => SetProperty(ref _emergencyContactPhone, value);
        }

        public string? EmergencyRelationship
        {
            get => _emergencyRelationship;
            set => SetProperty(ref _emergencyRelationship, value);
        }

        // Step 5 Properties
        public string? SelectedPlanName
        {
            get => _selectedPlanName;
            set => SetProperty(ref _selectedPlanName, value);
        }

        public string? PaymentMethod
        {
            get => _paymentMethod;
            set => SetProperty(ref _paymentMethod, value);
        }

        // Step 6 Properties
        public bool QualifiesSNP
        {
            get => _qualifiesSNP;
            set => SetProperty(ref _qualifiesSNP, value);
        }

        public string? SNPType
        {
            get => _snpType;
            set => SetProperty(ref _snpType, value);
        }

        public bool WantsMSA
        {
            get => _wantsMSA;
            set => SetProperty(ref _wantsMSA, value);
        }

        public bool DualEligible
        {
            get => _dualEligible;
            set => SetProperty(ref _dualEligible, value);
        }

        public bool HasESRD
        {
            get => _hasESRD;
            set => SetProperty(ref _hasESRD, value);
        }

        public bool ReceivesLIS
        {
            get => _receivesLIS;
            set => SetProperty(ref _receivesLIS, value);
        }

        // Step 7 Properties
        public List<string> Dependents
        {
            get => _dependents;
            set => SetProperty(ref _dependents, value);
        }

        // Step 8 Properties
        public byte[]? BeneficiarySignatureData
        {
            get => _beneficiarySignatureData;
            set => SetProperty(ref _beneficiarySignatureData, value);
        }
        
            // Triple-S-specific properties
            public string? DeviceInfo
            {
                get => _deviceInfo;
                set => SetProperty(ref _deviceInfo, value);
            }

            public string? IPAddress
            {
                get => _ipAddress;
                set => SetProperty(ref _ipAddress, value);
            }

            public string? GPSCoordinates
            {
                get => _gpsCoordinates;
                set => SetProperty(ref _gpsCoordinates, value);
            }

            public string? FormVariant
            {
                get => _formVariant;
                set => SetProperty(ref _formVariant, value);
            }

            public string? OMBControlNumber
            {
                get => _ombControlNumber;
                set => SetProperty(ref _ombControlNumber, value);
            }

            public DateTime AttestationTimestamp
            {
                get => _attestationTimestamp;
                set => SetProperty(ref _attestationTimestamp, value);
            }

            public string? SubmissionLocationType
            {
                get => _submissionLocationType;
                set => SetProperty(ref _submissionLocationType, value);
            }

            public string? ApplicationDate
            {
                get => _applicationDate;
                set => SetProperty(ref _applicationDate, value);
            }

            public string? EnrollmentMechanism
            {
                get => _enrollmentMechanism;
                set => SetProperty(ref _enrollmentMechanism, value);
            }

            public string? FormIdentifier
            {
                get => _formIdentifier;
                set => SetProperty(ref _formIdentifier, value);
            }

            public DateTime EffectiveDate
            {
                get => _effectiveDate;
                set => SetProperty(ref _effectiveDate, value);
            }

            public DateTime CreatedDate
            {
                get => _createdDate;
                set => SetProperty(ref _createdDate, value);
            }

            public DateTime LastModifiedDate
            {
                get => _lastModifiedDate;
                set => SetProperty(ref _lastModifiedDate, value);
            }

        public EnrollmentWizardViewModel()
        {
            CurrentStep = 1;
            DateOfBirth = DateTime.Now.AddYears(-65);
            State = "PR";
            _dependents = new();
        }

        /// <summary>
        /// Validates all enrollment data and creates the enrollment
        /// </summary>
        public async Task<(bool Success, string Message, string? EnrollmentId)> CreateEnrollmentAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                // Validate all steps
                if (!ValidateAllSteps())
                    throw new InvalidOperationException("One or more validation rules failed");

                // Build Enrollment object
                _currentEnrollment = new Enrollment
                {
                    BeneficiaryId = "0", // TODO: Get from context or parameter
                    PlanId = "1", // TODO: Map from SelectedPlanName
                    Status = "Active",
                    EnrollmentDate = DateTime.Now,
                    EffectiveDate = DateTime.Now
                };

                // TODO: Call service to save enrollment to database
                await Task.Delay(500); // Simulate network call

                return (true, "Enrollment created successfully", _currentEnrollment.EnrollmentId.ToString());
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating enrollment: {ex.Message}";
                return (false, ErrorMessage, null);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool ValidateAllSteps()
        {
            return ValidateStep1() &&
                   ValidateStep2() &&
                   ValidateStep3() &&
                   ValidateStep4() &&
                   ValidateStep5() &&
                   ValidateStep8();
        }

        private bool ValidateStep1()
        {
            return !string.IsNullOrWhiteSpace(FirstName)
                && !string.IsNullOrWhiteSpace(LastName)
                && DateOfBirth != default
                && !string.IsNullOrWhiteSpace(PhoneNumber)
                && !string.IsNullOrWhiteSpace(MedicareNumber);
        }

        private bool ValidateStep2()
        {
            return !string.IsNullOrWhiteSpace(AddressLine1)
                && !string.IsNullOrWhiteSpace(City)
                && !string.IsNullOrWhiteSpace(State)
                && !string.IsNullOrWhiteSpace(ZipCode);
        }

        private bool ValidateStep3()
        {
            if (HasOtherInsurance && string.IsNullOrWhiteSpace(OtherCoverageType))
                return false;

            if (CurrentlyInMA && (string.IsNullOrWhiteSpace(CurrentPlanName) || string.IsNullOrWhiteSpace(ReasonForChange)))
                return false;

            return true;
        }

        private bool ValidateStep4()
        {
            return !string.IsNullOrWhiteSpace(EmergencyContactName)
                && !string.IsNullOrWhiteSpace(EmergencyContactPhone);
        }

        private bool ValidateStep5()
        {
            return !string.IsNullOrWhiteSpace(SelectedPlanName)
                && !string.IsNullOrWhiteSpace(PaymentMethod);
        }

        private bool ValidateStep8()
        {
            return BeneficiarySignatureData != null && BeneficiarySignatureData.Length > 0;
        }

        /// <summary>
        /// Gets enrollment summary for final review
        /// </summary>
        public string GetEnrollmentSummary()
        {
            return $"Beneficiary: {FirstName} {LastName}\n" +
                   $"Plan: {SelectedPlanName}\n" +
                   $"Effective Date: {DateTime.Now.AddMonths(1):MM/dd/yyyy}\n" +
                   $"Payment: {PaymentMethod}";
        }
    }
}
