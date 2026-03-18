using System;
using Triple_S_Maui_AEP.Models;
using Triple_S_Maui_AEP.Services;

namespace Triple_S_Maui_AEP.ViewModels
{
    /// <summary>
    /// ViewModel for Enrollment Wizard (9-step process)
    /// Manages the complete enrollment workflow
    /// </summary>
    public class EnrollmentWizardViewModel : BaseViewModel
    {
        private int _currentStep;
        private EnrollmentRecord? _currentEnrollment;
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

        /// <summary>
        /// Gets the current enrollment record
        /// </summary>
        public EnrollmentRecord CurrentEnrollment
        {
            get => _currentEnrollment ??= new EnrollmentRecord();
            set => SetProperty(ref _currentEnrollment, value);
        }

        // Step 1 - Personal Information
        private string? _firstName;
        private string? _lastName;
        private string? _middleInitial;
        private DateTime _dateOfBirth;
        private string? _gender;
        private string? _phoneNumber;
        private string? _emailAddress;
        private string? _medicareNumber;
        private string? _ssn;

        // Step 2 - Contact Information
        private string? _primaryPhone;
        private bool _primaryPhoneIsMobile;
        private string? _secondaryPhone;
        private bool _secondaryPhoneIsMobile;
        private string? _preferredContactMethod;

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
        private string? _beneficiarySignatureBase64;
        private bool _usesXMark;
        private string? _enrolleeSignatureBase64;
        private string? _agentSignatureBase64;
        private string? _witnessSignatureBase64;

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

        // Properties
        public int CurrentStep
        {
            get => _currentStep;
            set => SetProperty(ref _currentStep, value);
        }

        public int TotalSteps => 9;

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

        public string? MiddleInitial
        {
            get => _middleInitial;
            set => SetProperty(ref _middleInitial, value);
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

        public string? SSN
        {
            get => _ssn;
            set => SetProperty(ref _ssn, value);
        }

        // Step 2 Properties
        public string? PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        public string? PrimaryPhone
        {
            get => _primaryPhone;
            set => SetProperty(ref _primaryPhone, value);
        }

        public bool PrimaryPhoneIsMobile
        {
            get => _primaryPhoneIsMobile;
            set => SetProperty(ref _primaryPhoneIsMobile, value);
        }

        public string? SecondaryPhone
        {
            get => _secondaryPhone;
            set => SetProperty(ref _secondaryPhone, value);
        }

        public bool SecondaryPhoneIsMobile
        {
            get => _secondaryPhoneIsMobile;
            set => SetProperty(ref _secondaryPhoneIsMobile, value);
        }

        public string? PreferredContactMethod
        {
            get => _preferredContactMethod;
            set => SetProperty(ref _preferredContactMethod, value);
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
        public string? BeneficiarySignatureBase64
        {
            get => _beneficiarySignatureBase64;
            set => SetProperty(ref _beneficiarySignatureBase64, value);
        }

        public bool UsesXMark
        {
            get => _usesXMark;
            set => SetProperty(ref _usesXMark, value);
        }

        public string? EnrolleeSignatureBase64
        {
            get => _enrolleeSignatureBase64;
            set => SetProperty(ref _enrolleeSignatureBase64, value);
        }

        public string? AgentSignatureBase64
        {
            get => _agentSignatureBase64;
            set => SetProperty(ref _agentSignatureBase64, value);
        }

        public string? WitnessSignatureBase64
        {
            get => _witnessSignatureBase64;
            set => SetProperty(ref _witnessSignatureBase64, value);
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
            _dependents = new();
            SOANumber = new SOANumberService().GenerateEnrollmentNumber();
        }

        public async Task<bool> GoToNextStepAsync()
        {
            if (CurrentStep < TotalSteps)
            {
                CurrentStep++;
                await Task.Delay(200);
                return true;
            }
            return false;
        }

        public async Task<bool> GoToPreviousStepAsync()
        {
            if (CurrentStep > 1)
            {
                CurrentStep--;
                await Task.Delay(200);
                return true;
            }
            return false;
        }

        public async Task<(bool Success, string ErrorMessage)> ValidateCurrentStepAsync()
        {
            return await Task.FromResult(CurrentStep switch
            {
                1 => ValidateStep1() ? (true, string.Empty) : (false, "Please fill in all required personal information"),
                2 => ValidateStep2() ? (true, string.Empty) : (false, "Please fill in all required contact information"),
                3 => ValidateStep3() ? (true, string.Empty) : (false, "Please answer all coverage questions"),
                4 => ValidateStep4() ? (true, string.Empty) : (false, "Please fill in emergency contact information"),
                5 => ValidateStep5() ? (true, string.Empty) : (false, "Please provide a signature or mark"),
                _ => (false, "Invalid step")
            });
        }

        private bool ValidateStep1()
        {
            return !string.IsNullOrWhiteSpace(FirstName)
                && !string.IsNullOrWhiteSpace(LastName)
                && DateOfBirth != default
                && !string.IsNullOrWhiteSpace(MedicareNumber);
        }

        private bool ValidateStep2()
        {
            return !string.IsNullOrWhiteSpace(PrimaryPhone)
                && !string.IsNullOrWhiteSpace(EmailAddress);
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
            return UsesXMark || !string.IsNullOrWhiteSpace(BeneficiarySignatureBase64);
        }

        /// <summary>
        /// Submits the enrollment after all steps are completed
        /// </summary>
        public async Task<bool> SubmitEnrollmentAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                // Update CurrentEnrollment with all properties
                CurrentEnrollment.FirstName = FirstName ?? string.Empty;
                CurrentEnrollment.MiddleInitial = MiddleInitial ?? string.Empty;
                CurrentEnrollment.LastName = LastName ?? string.Empty;
                CurrentEnrollment.DateOfBirth = DateOfBirth;
                CurrentEnrollment.Gender = Gender ?? string.Empty;
                CurrentEnrollment.PrimaryPhone = PrimaryPhone ?? string.Empty;
                CurrentEnrollment.PrimaryPhoneIsMobile = PrimaryPhoneIsMobile;
                CurrentEnrollment.SecondaryPhone = SecondaryPhone ?? string.Empty;
                CurrentEnrollment.SecondaryPhoneIsMobile = SecondaryPhoneIsMobile;
                CurrentEnrollment.Email = EmailAddress ?? string.Empty;
                CurrentEnrollment.MedicareNumber = MedicareNumber ?? string.Empty;
                CurrentEnrollment.SSN = SSN ?? string.Empty;
                CurrentEnrollment.PreferredContactMethod = PreferredContactMethod ?? string.Empty;
                CurrentEnrollment.EnrolleeSignatureBase64 = BeneficiarySignatureBase64 ?? string.Empty;
                CurrentEnrollment.UsesXMark = UsesXMark;

                // Add enrollment record to encrypted database
                await EnrollmentService.AddEnrollmentRecordAsync(new EnrollmentService.EnrollmentRecord
                {
                    EnrollmentNumber = SOANumber ?? string.Empty,
                    FirstName = FirstName ?? string.Empty,
                    LastName = LastName ?? string.Empty,
                    DateCreated = DateTime.Now,
                    FilePath = string.Empty
                });

                await Task.Delay(500);

                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error submitting enrollment: {ex.Message}";
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Gets enrollment summary for final review
        /// </summary>
        public string GetEnrollmentSummary()
        {
            return $"Beneficiary: {FirstName} {LastName}\n" +
                   $"Medicare #: {MedicareNumber}\n" +
                   $"Contact: {PrimaryPhone}\n" +
                   $"Email: {EmailAddress}";
        }

        /// <summary>
        /// Resets all form fields to defaults for the next enrollment
        /// </summary>
        public void ResetForm()
        {
            CurrentStep = 1;
            ErrorMessage = string.Empty;
            IsLoading = false;
            CurrentEnrollment = new EnrollmentRecord();
            SOANumber = new SOANumberService().GenerateEnrollmentNumber();

            // Reset Step 1
            FirstName = string.Empty;
            LastName = string.Empty;
            MiddleInitial = string.Empty;
            DateOfBirth = DateTime.Now.AddYears(-65);
            Gender = string.Empty;
            MedicareNumber = string.Empty;
            SSN = string.Empty;

            // Reset Step 2
            PrimaryPhone = string.Empty;
            PrimaryPhoneIsMobile = false;
            SecondaryPhone = string.Empty;
            SecondaryPhoneIsMobile = false;
            EmailAddress = string.Empty;
            PreferredContactMethod = string.Empty;

            // Reset Step 3
            HasOtherInsurance = false;
            OtherCoverageType = string.Empty;
            CurrentlyInMA = false;
            CurrentPlanName = string.Empty;
            ReasonForChange = string.Empty;

            // Reset Step 4
            EmergencyContactName = string.Empty;
            EmergencyContactPhone = string.Empty;
            EmergencyRelationship = string.Empty;

            // Reset Step 5
            SelectedPlanName = string.Empty;
            PaymentMethod = string.Empty;

            // Reset Step 6
            QualifiesSNP = false;
            SNPType = string.Empty;
            WantsMSA = false;
            DualEligible = false;
            HasESRD = false;
            ReceivesLIS = false;

            // Reset Step 7
            Dependents.Clear();

            // Reset Step 8
            BeneficiarySignatureBase64 = string.Empty;
            UsesXMark = false;
            EnrolleeSignatureBase64 = string.Empty;
            AgentSignatureBase64 = string.Empty;
            WitnessSignatureBase64 = string.Empty;

            System.Diagnostics.Debug.WriteLine("Enrollment form reset successfully");
        }

        public string GetStepTitle()
        {
            return CurrentStep switch
            {
                1 => "Personal Information",
                2 => "Contact Information",
                3 => "Medicare Information",
                4 => "Review & Confirmation",
                5 => "Signature Capture",
                _ => "Enrollment"
            };
        }

        public string GetStepTitleSpanish()
        {
            return CurrentStep switch
            {
                1 => "Información Personal",
                2 => "Información de Contacto",
                3 => "Información de Medicare",
                4 => "Revisar y Confirmar",
                5 => "Captura de Firma",
                _ => "Inscripción"
            };
        }
    }
}
