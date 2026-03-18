using System.Windows.Input;

namespace Triple_S_Maui_AEP.ViewModels
{
    public class TripleSFormWizardViewModel : BaseViewModel
    {
        private int _currentStep = 1;
        private string? _validationMessage;
        private bool _isSubmitting;

        // Step 1 - Instructions
        private string? _showInstructionsAnswer;

        // Step 2 - Section 1: Scope and Plan Selection
        private string? _scopeOfAppointmentNumber;
        private string? _selectedPlanName;
        private string? _groupCoverage;
        private string? _groupPlanType;
        private string? _groupMonthlyPremium;
        private DateTime _groupEffectiveDate = DateTime.Today;
        private string? _groupSocialSecurityNumber;
        private string? _firstName;
        private string? _lastName;
        private string? _middleInitial;
        private DateTime _birthDate = DateTime.Today.AddYears(-65);
        private string? _sex;
        private string? _homePhone;
        private bool _homePhoneIsCell;
        private string? _alternatePhone;
        private bool _alternatePhoneIsCell;

        // Step 3 - Address and Medicare
        private string? _permanentAddressLine1;
        private string? _permanentAddressLine2;
        private string? _permanentCity;
        private string? _permanentState = "PR";
        private string? _permanentZipCode;
        private string? _medicareNumber;
        private string? _otherRxCoverageAnswer;
        private string? _otherCoverageName;
        private string? _otherCoverageMemberNumber;
        private string? _otherCoverageGroupNumber;
        private string? _medicaidProgramAnswer;
        private string? _medicaidNumber;
        private string? _contigoPlusChronicCondition;
        private string? _contigoEnMenteDementiaAnswer;

        // Step 4 - Signature and Authorization
        private bool _ackPartAandB;
        private bool _ackSingleMAPlan;
        private bool _ackInfoCorrect;
        private string? _applicantSignatureName;
        private DateTime _signatureDate = DateTime.Today;
        private bool _enrollNowChecked;
        private DateTime _enrollNowDate = DateTime.Today;
        private string? _phoneEnrollmentCallNumber;
        private string? _phoneEnrollmentWitnessSignature;
        private DateTime _phoneEnrollmentWitnessDate = DateTime.Today;
        private string? _authorizedRepName;
        private string? _authorizedRepAddress;
        private string? _authorizedRepPhone;
        private string? _authorizedRepRelationship;

        public int TotalSteps => 4;
        public int CurrentStep
        {
            get => _currentStep;
            set
            {
                if (SetProperty(ref _currentStep, value))
                {
                    OnPropertyChanged(nameof(CanGoBack));
                    OnPropertyChanged(nameof(CanGoNext));
                    OnPropertyChanged(nameof(IsOnFinalStep));
                    OnPropertyChanged(nameof(StepTitle));
                    RaiseStepVisibilityChanged();
                    ValidationMessage = string.Empty;
                }
            }
        }

        public string StepTitle => CurrentStep switch
        {
            1 => "Step 1 - Instructions",
            2 => "2026 Enrollment Request Form P.2",
            3 => "2026 Enrollment Request Form P.3",
            4 => "2026 Enrollment Request Form P.4",
            _ => "Triple-S Form"
        };

        public bool CanGoBack => CurrentStep > 1;
        public bool CanGoNext => CurrentStep < TotalSteps;
        public bool IsOnFinalStep => CurrentStep == TotalSteps;
        public bool IsStep1Visible => CurrentStep == 1;
        public bool IsStep2Visible => CurrentStep == 2;
        public bool IsStep3Visible => CurrentStep == 3;
        public bool IsStep4Visible => CurrentStep == 4;

        public bool IsInstructionsImageVisible => IsYes(ShowInstructionsAnswer);
        public string InstructionsImagePath => "documentation/enUS-Enrollment-Request-Form-1.jpg";

        public bool IsOtherCoverageDetailsVisible => IsYes(OtherRxCoverageAnswer);
        public bool IsPlatinoPlanSelected => IsPlatinoPlan(SelectedPlanName);
        public bool IsMedicaidNumberVisible => IsPlatinoPlanSelected && IsYes(MedicaidProgramAnswer);
        public bool IsContigoPlusConditionVisible => IsContigoPlusPlan(SelectedPlanName);
        public bool IsContigoEnMenteQuestionVisible => IsContigoEnMentePlan(SelectedPlanName);

        public string? ValidationMessage { get => _validationMessage; set => SetProperty(ref _validationMessage, value); }
        public bool IsSubmitting { get => _isSubmitting; set => SetProperty(ref _isSubmitting, value); }

        public string? ShowInstructionsAnswer
        {
            get => _showInstructionsAnswer;
            set
            {
                if (SetProperty(ref _showInstructionsAnswer, value))
                {
                    OnPropertyChanged(nameof(IsInstructionsImageVisible));
                }
            }
        }

        public string? ScopeOfAppointmentNumber
        {
            get => _scopeOfAppointmentNumber;
            set => SetProperty(ref _scopeOfAppointmentNumber, value);
        }

        public string? SelectedPlanName
        {
            get => _selectedPlanName;
            set => SetProperty(ref _selectedPlanName, value);
        }

        public string? GroupCoverage
        {
            get => _groupCoverage;
            set => SetProperty(ref _groupCoverage, value);
        }

        public string? GroupPlanType
        {
            get => _groupPlanType;
            set => SetProperty(ref _groupPlanType, value);
        }

        public string? GroupMonthlyPremium
        {
            get => _groupMonthlyPremium;
            set => SetProperty(ref _groupMonthlyPremium, value);
        }

        public DateTime GroupEffectiveDate
        {
            get => _groupEffectiveDate;
            set => SetProperty(ref _groupEffectiveDate, value);
        }

        public string? GroupSocialSecurityNumber
        {
            get => _groupSocialSecurityNumber;
            set => SetProperty(ref _groupSocialSecurityNumber, value);
        }

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

        public DateTime BirthDate
        {
            get => _birthDate;
            set => SetProperty(ref _birthDate, value);
        }

        public string? Sex
        {
            get => _sex;
            set => SetProperty(ref _sex, value);
        }

        public string? HomePhone
        {
            get => _homePhone;
            set => SetProperty(ref _homePhone, value);
        }

        public bool HomePhoneIsCell
        {
            get => _homePhoneIsCell;
            set => SetProperty(ref _homePhoneIsCell, value);
        }

        public string? AlternatePhone
        {
            get => _alternatePhone;
            set => SetProperty(ref _alternatePhone, value);
        }

        public bool AlternatePhoneIsCell
        {
            get => _alternatePhoneIsCell;
            set => SetProperty(ref _alternatePhoneIsCell, value);
        }

        public string? PermanentAddressLine1
        {
            get => _permanentAddressLine1;
            set => SetProperty(ref _permanentAddressLine1, value);
        }

        public string? PermanentAddressLine2
        {
            get => _permanentAddressLine2;
            set => SetProperty(ref _permanentAddressLine2, value);
        }

        public string? PermanentCity
        {
            get => _permanentCity;
            set => SetProperty(ref _permanentCity, value);
        }

        public string? PermanentState
        {
            get => _permanentState;
            set => SetProperty(ref _permanentState, value);
        }

        public string? PermanentZipCode
        {
            get => _permanentZipCode;
            set => SetProperty(ref _permanentZipCode, value);
        }

        public string? MedicareNumber
        {
            get => _medicareNumber;
            set => SetProperty(ref _medicareNumber, value);
        }

        public string? OtherRxCoverageAnswer
        {
            get => _otherRxCoverageAnswer;
            set => SetProperty(ref _otherRxCoverageAnswer, value);
        }

        public string? OtherCoverageName
        {
            get => _otherCoverageName;
            set => SetProperty(ref _otherCoverageName, value);
        }

        public string? OtherCoverageMemberNumber
        {
            get => _otherCoverageMemberNumber;
            set => SetProperty(ref _otherCoverageMemberNumber, value);
        }

        public string? OtherCoverageGroupNumber
        {
            get => _otherCoverageGroupNumber;
            set => SetProperty(ref _otherCoverageGroupNumber, value);
        }

        public string? MedicaidProgramAnswer
        {
            get => _medicaidProgramAnswer;
            set
            {
                if (SetProperty(ref _medicaidProgramAnswer, value))
                {
                    OnPropertyChanged(nameof(IsMedicaidNumberVisible));
                }
            }
        }

        public string? MedicaidNumber
        {
            get => _medicaidNumber;
            set => SetProperty(ref _medicaidNumber, value);
        }

        public string? ContigoPlusChronicCondition
        {
            get => _contigoPlusChronicCondition;
            set => SetProperty(ref _contigoPlusChronicCondition, value);
        }

        public string? ContigoEnMenteDementiaAnswer
        {
            get => _contigoEnMenteDementiaAnswer;
            set => SetProperty(ref _contigoEnMenteDementiaAnswer, value);
        }

        public bool AckPartAandB
        {
            get => _ackPartAandB;
            set => SetProperty(ref _ackPartAandB, value);
        }

        public bool AckSingleMAPlan
        {
            get => _ackSingleMAPlan;
            set => SetProperty(ref _ackSingleMAPlan, value);
        }

        public bool AckInfoCorrect
        {
            get => _ackInfoCorrect;
            set => SetProperty(ref _ackInfoCorrect, value);
        }

        public string? ApplicantSignatureName
        {
            get => _applicantSignatureName;
            set => SetProperty(ref _applicantSignatureName, value);
        }

        public DateTime SignatureDate
        {
            get => _signatureDate;
            set => SetProperty(ref _signatureDate, value);
        }

        public bool EnrollNowChecked
        {
            get => _enrollNowChecked;
            set
            {
                if (SetProperty(ref _enrollNowChecked, value))
                {
                    OnPropertyChanged(nameof(IsEnrollNowChecked));
                }
            }
        }

        public bool IsEnrollNowChecked => EnrollNowChecked;

        public DateTime EnrollNowDate
        {
            get => _enrollNowDate;
            set => SetProperty(ref _enrollNowDate, value);
        }

        public string? PhoneEnrollmentCallNumber
        {
            get => _phoneEnrollmentCallNumber;
            set => SetProperty(ref _phoneEnrollmentCallNumber, value);
        }

        public string? PhoneEnrollmentWitnessSignature
        {
            get => _phoneEnrollmentWitnessSignature;
            set => SetProperty(ref _phoneEnrollmentWitnessSignature, value);
        }

        public DateTime PhoneEnrollmentWitnessDate
        {
            get => _phoneEnrollmentWitnessDate;
            set => SetProperty(ref _phoneEnrollmentWitnessDate, value);
        }

        public string? AuthorizedRepName
        {
            get => _authorizedRepName;
            set => SetProperty(ref _authorizedRepName, value);
        }

        public string? AuthorizedRepAddress
        {
            get => _authorizedRepAddress;
            set => SetProperty(ref _authorizedRepAddress, value);
        }

        public string? AuthorizedRepPhone
        {
            get => _authorizedRepPhone;
            set => SetProperty(ref _authorizedRepPhone, value);
        }

        public string? AuthorizedRepRelationship
        {
            get => _authorizedRepRelationship;
            set => SetProperty(ref _authorizedRepRelationship, value);
        }

        public List<string> AvailablePlans => new()
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

        public ICommand NextCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand SubmitCommand { get; }

        public TripleSFormWizardViewModel()
        {
            NextCommand = new Command(async () => await GoNextAsync());
            BackCommand = new Command(GoBack);
            SubmitCommand = new Command(async () => await SubmitAsync());
        }

        private async Task GoNextAsync()
        {
            if (!ValidateStep(CurrentStep, out var error))
            {
                ValidationMessage = error;
                return;
            }
            ValidationMessage = string.Empty;
            if (CurrentStep < TotalSteps) CurrentStep++;
            await Task.CompletedTask;
        }

        private void GoBack()
        {
            if (CurrentStep > 1)
            {
                ValidationMessage = string.Empty;
                CurrentStep--;
            }
        }

        private async Task SubmitAsync()
        {
            if (!ValidateAll(out var error))
            {
                ValidationMessage = error;
                return;
            }
            IsSubmitting = true;
            ValidationMessage = "Form validation passed.";
            await Task.Delay(100);
            IsSubmitting = false;
        }

        private bool ValidateAll(out string message)
        {
            for (var i = 1; i <= TotalSteps; i++)
            {
                if (!ValidateStep(i, out message))
                {
                    message = $"Step {i}: {message}";
                    return false;
                }
            }
            message = string.Empty;
            return true;
        }

        private bool ValidateStep(int step, out string message)
        {
            message = string.Empty;
            switch (step)
            {
                case 1:
                    if (!IsYesNo(ShowInstructionsAnswer))
                    {
                        message = "Please select Yes or No for viewing instructions.";
                        return false;
                    }
                    break;
                case 2:
                    if (string.IsNullOrWhiteSpace(ScopeOfAppointmentNumber))
                    {
                        message = "Scope of Appointment # is required.";
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(SelectedPlanName))
                    {
                        message = "Please select a plan to join.";
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(FirstName))
                    {
                        message = "First Name is required.";
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(LastName))
                    {
                        message = "Last Name is required.";
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(Sex))
                    {
                        message = "Sex is required (M or F).";
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(HomePhone))
                    {
                        message = "Home Phone Number is required.";
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(AlternatePhone))
                    {
                        message = "Alternate Phone Number is required.";
                        return false;
                    }
                    break;
                case 3:
                    if (string.IsNullOrWhiteSpace(PermanentAddressLine1))
                    {
                        message = "Permanent Address Line 1 is required.";
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(PermanentCity))
                    {
                        message = "Permanent City is required.";
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(PermanentState))
                    {
                        message = "Permanent State is required.";
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(PermanentZipCode))
                    {
                        message = "Permanent Zip Code is required.";
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(MedicareNumber))
                    {
                        message = "Medicare Number is required.";
                        return false;
                    }
                    if (!IsYesNo(OtherRxCoverageAnswer))
                    {
                        message = "Please select Yes or No for other prescription drug coverage.";
                        return false;
                    }
                    if (IsYes(OtherRxCoverageAnswer) && (string.IsNullOrWhiteSpace(OtherCoverageName) || string.IsNullOrWhiteSpace(OtherCoverageMemberNumber) || string.IsNullOrWhiteSpace(OtherCoverageGroupNumber)))
                    {
                        message = "Complete other coverage details (Name, Member Number, Group Number).";
                        return false;
                    }
                    if (IsPlatinoPlanSelected && !IsYesNo(MedicaidProgramAnswer))
                    {
                        message = "Select Yes or No for Medicaid (Platino plans).";
                        return false;
                    }
                    if (IsPlatinoPlanSelected && IsYes(MedicaidProgramAnswer) && string.IsNullOrWhiteSpace(MedicaidNumber))
                    {
                        message = "Medicaid number (MPI) is required for Platino plans.";
                        return false;
                    }
                    if (IsContigoPlusConditionVisible && string.IsNullOrWhiteSpace(ContigoPlusChronicCondition))
                    {
                        message = "Select a chronic condition for Contigo Plus plan.";
                        return false;
                    }
                    if (IsContigoEnMenteQuestionVisible && !IsYesNo(ContigoEnMenteDementiaAnswer))
                    {
                        message = "Select Yes or No for dementia diagnosis (ContigoEnMente plan).";
                        return false;
                    }
                    break;
                case 4:
                    if (!AckPartAandB || !AckSingleMAPlan || !AckInfoCorrect)
                    {
                        message = "You must acknowledge all terms to proceed.";
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(ApplicantSignatureName))
                    {
                        message = "Applicant signature name is required.";
                        return false;
                    }
                    break;
            }
            return true;
        }

        private static bool IsYes(string? value) => string.Equals(value, "Yes", StringComparison.OrdinalIgnoreCase);
        private static bool IsYesNo(string? value) => string.Equals(value, "Yes", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "No", StringComparison.OrdinalIgnoreCase);

        private static bool IsPlatinoPlan(string? plan) => !string.IsNullOrWhiteSpace(plan) && plan.Contains("Platino", StringComparison.OrdinalIgnoreCase);
        private static bool IsContigoPlusPlan(string? plan) => !string.IsNullOrWhiteSpace(plan) && plan.Contains("Contigo Plus", StringComparison.OrdinalIgnoreCase);
        private static bool IsContigoEnMentePlan(string? plan) => !string.IsNullOrWhiteSpace(plan) && plan.Contains("ContigoEnMente", StringComparison.OrdinalIgnoreCase);

        private void RaiseStepVisibilityChanged()
        {
            OnPropertyChanged(nameof(IsStep1Visible));
            OnPropertyChanged(nameof(IsStep2Visible));
            OnPropertyChanged(nameof(IsStep3Visible));
            OnPropertyChanged(nameof(IsStep4Visible));
        }
    }
}
