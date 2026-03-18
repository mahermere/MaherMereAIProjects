using System.Windows.Input;

namespace Triple_S_Maui_AEP.ViewModels
{
    public class TripleSEnrollmentWizardViewModel : BaseViewModel
    {
        private int _currentStep = 1;
        private string? _validationMessage;
        private bool _isSubmitting;

        // Step 1 - Plan and Beneficiary
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

        // Step 2 - Address
        private string? _permanentAddressLine1;
        private string? _permanentAddressLine2;
        private string? _permanentCity;
        private string? _permanentState = "PR";
        private string? _permanentZipCode;
        private bool _mailingAddressDifferent;
        private string? _mailingAddressLine1;
        private string? _mailingAddressLine2;
        private string? _mailingCity;
        private string? _mailingState;
        private string? _mailingZipCode;
        private string? _medicareNumber;

        // Step 3 - Coverage
        private string? _otherRxCoverageAnswer;
        private string? _otherCoverageName;
        private string? _otherCoverageMemberNumber;
        private string? _otherCoverageGroupNumber;
        private string? _medicaidProgramAnswer;
        private string? _medicaidNumber;
        private string? _contigoPlusChronicCondition;
        private string? _contigoEnMenteDementiaAnswer;

        // Step 4 - Signature
        private bool _ackPartAandB;
        private bool _ackSingleMAPlan;
        private bool _ackInfoCorrect;
        private string? _applicantSignatureName;
        private DateTime _signatureDate = DateTime.Today;
        private string? _authorizedRepName;
        private string? _authorizedRepAddress;
        private string? _authorizedRepPhone;
        private string? _authorizedRepRelationship;

        // Electronic/Phone Enrollment Methods
        private bool _enrollNowChecked;
        private DateTime _enrollNowDate = DateTime.Today;
        private string? _phoneEnrollmentCallNumber;
        private string? _phoneEnrollmentWitnessSignature;
        private DateTime _phoneEnrollmentWitnessDate = DateTime.Today;

        // Step 5 - Preferences
        private string? _preferredSpokenLanguage;
        private string? _preferredAccessibleFormat;
        private bool _formatBraille;
        private bool _formatLargeText;
        private bool _formatAudioCD;
        private bool _formatDataCD;
        private string? _workStatusAnswer;
        private string? _spouseWorkStatusAnswer;
        private string? _pcpName;
        private string? _pcpPhone;
        private bool _wantsProviderDirectoryByEmail;
        private bool _wantsAnnualNoticeByEmail;
        private bool _wantsEvidenceOfCoverageByEmail;
        private bool _wantsSummaryOfBenefitsByEmail;
        private bool _wantsFormularyByEmail;
        private bool _wantsPromotionalByEmail;
        private bool _wantsEnrollmentConfirmationByEmail;
        private string? _textConsentAnswer;
        private string? _textConsentNumber;
        private string? _emailConsentAnswer;
        private string? _emailConsentAddress;
        private string? _emergencyContactName;
        private string? _emergencyContactPhone;
        private string? _emergencyContactRelationship;
        private string? _isRetireeAnswer;
        private DateTime _retirementDate = DateTime.Today;
        private string? _retireeName;
        private string? _coversSpouseOrDependentsAnswer;
        private string? _spouseName;
        private string? _dependentNames;
        private string? _isLongTermCareResidentAnswer;
        private string? _ltcInstitutionName;
        private string? _ltcAdministratorName;
        private string? _ltcPhone;
        private string? _currentHealthPlan;
        private string? _otherHealthPlanName;

        // Step 6 - Transition
        private string? _transitionLastNames;
        private string? _transitionName;
        private string? _transitionInitial;
        private DateTime _transitionDateOfBirth = DateTime.Today.AddYears(-65);
        private string? _transitionTelephone1;
        private string? _transitionTelephone2;
        private string? _transitionBenefitPlan;
        private DateTime _transitionEffectivityDate = DateTime.Today;
        private string? _transitionShicMedicareNumber;
        private string? _transitionEquipmentServices;
        private string? _transitionProviderCompany;
        private string? _transitionEquipmentEffectivity;
        private string? _transitionPreviousHealthPlan;
        private string? _transitionComments;
        private string? _transitionInformationProvidedBy;
        private string? _transitionPlanRepresentative;
        private string? _transitionRegion;
        private DateTime _transitionFormDate = DateTime.Today;

        // Step 7 - Payment
        private string? _paymentOption = "Coupon Book";
        private string? _eftAccountHolderName;
        private string? _eftRoutingNumber;
        private string? _eftAccountNumber;
        private string? _eftAccountType;
        private string? _creditCardType;
        private string? _creditCardHolderName;
        private string? _creditCardNumber;
        private string? _creditCardExpiration;
        private string? _autoDeductionBenefitSource;

        // Step 8 - Documents
        private bool _receivedInitialPackage;
        private bool _receivedStarRatingsNotice;
        private bool _receivedWebAvailabilityNotice;
        private bool _receivedEnrollmentConfirmation;
        private bool _receivedEnrollmentFormCopy;
        private bool _receivedAttestationOfEligibility;
        private bool _receivedPrecertificationChronicDiseases;
        private bool _receivedPhiAuthorization;

        // Step 9/10/11
        private string? _helperName;
        private string? _helperRelationship;
        private string? _helperSignature;
        private string? _helperNpn;
        private string? _officialReceiptDate;
        private string? _officialPlanId;
        private string? _officialCoverageEffectiveDate;
        private bool _confirmSection1Complete;
        private bool _confirmSection2Reviewed;

        public int TotalSteps => 11;
        public int CurrentStep { get => _currentStep; set { if (SetProperty(ref _currentStep, value)) { OnPropertyChanged(nameof(CanGoBack)); OnPropertyChanged(nameof(CanGoNext)); OnPropertyChanged(nameof(IsOnFinalStep)); OnPropertyChanged(nameof(StepTitle)); RaiseStepVisibilityChanged(); ValidationMessage = string.Empty; } } }

        public string StepTitle => CurrentStep switch
        {
            1 => "Section 1 - Plan and Beneficiary",
            2 => "Section 1 - Address and Medicare",
            3 => "Section 1 - Coverage Questions",
            4 => "Section 1 - Signature and Authorization",
            5 => "Section 2 - Preferences and Communications",
            6 => "Section 2 - New Member Services Transition Form",
            7 => "Section 2 - Premium Payment",
            8 => "Section 2 - Documents Received",
            9 => "Section 2 - Helper Information",
            10 => "Official Use",
            11 => "Review and Validation",
            _ => "Triple-S Enrollment Wizard"
        };

        public bool CanGoBack => CurrentStep > 1;
        public bool CanGoNext => CurrentStep < TotalSteps;
        public bool IsOnFinalStep => CurrentStep == TotalSteps;
        public bool IsStep1Visible => CurrentStep == 1;
        public bool IsStep2Visible => CurrentStep == 2;
        public bool IsStep3Visible => CurrentStep == 3;
        public bool IsStep4Visible => CurrentStep == 4;
        public bool IsStep5Visible => CurrentStep == 5;
        public bool IsStep6Visible => CurrentStep == 6;
        public bool IsStep7Visible => CurrentStep == 7;
        public bool IsStep8Visible => CurrentStep == 8;
        public bool IsStep9Visible => CurrentStep == 9;
        public bool IsStep10Visible => CurrentStep == 10;
        public bool IsStep11Visible => CurrentStep == 11;

        public bool IsOtherCoverageDetailsVisible => IsYes(OtherRxCoverageAnswer);
        public bool IsPlatinoPlanSelected => IsPlatinoPlan(SelectedPlanName);
        public bool IsMedicaidNumberVisible => IsPlatinoPlanSelected && IsYes(MedicaidProgramAnswer);
        public bool IsContigoPlusConditionVisible => IsContigoPlusPlan(SelectedPlanName);
        public bool IsContigoEnMenteQuestionVisible => IsContigoEnMentePlan(SelectedPlanName);
        public bool IsTextNumberVisible => IsYes(TextConsentAnswer);
        public bool IsEmailAddressVisible => IsYes(EmailConsentAnswer);
        public bool IsTransitionEquipmentDetailsVisible => !string.IsNullOrWhiteSpace(TransitionEquipmentServices);
        public bool IsEftFieldsVisible => string.Equals(PaymentOption, "EFT", StringComparison.OrdinalIgnoreCase);
        public bool IsCreditCardFieldsVisible => string.Equals(PaymentOption, "Credit Card", StringComparison.OrdinalIgnoreCase);
        public bool IsAutoDeductionFieldsVisible => string.Equals(PaymentOption, "Auto Deduction", StringComparison.OrdinalIgnoreCase);
        public bool IsMailingAddressVisible => MailingAddressDifferent;
        public bool IsRetirementDetailsVisible => IsYes(IsRetireeAnswer) || string.Equals(IsRetireeAnswer, "No", StringComparison.OrdinalIgnoreCase);
        public bool IsRetirementDateVisible => IsYes(IsRetireeAnswer);
        public bool IsRetireeNameVisible => string.Equals(IsRetireeAnswer, "No", StringComparison.OrdinalIgnoreCase);
        public bool IsSpouseDependentDetailsVisible => IsYes(CoversSpouseOrDependentsAnswer);
        public bool IsLtcDetailsVisible => IsYes(IsLongTermCareResidentAnswer);
        public bool IsOtherHealthPlanNameVisible => string.Equals(CurrentHealthPlan, "Other", StringComparison.OrdinalIgnoreCase);

        public string? ValidationMessage { get => _validationMessage; set => SetProperty(ref _validationMessage, value); }
        public bool IsSubmitting { get => _isSubmitting; set => SetProperty(ref _isSubmitting, value); }
        public string? ScopeOfAppointmentNumber { get => _scopeOfAppointmentNumber; set => SetProperty(ref _scopeOfAppointmentNumber, value); }
        public string? SelectedPlanName { get => _selectedPlanName; set { if (SetProperty(ref _selectedPlanName, value)) { OnPropertyChanged(nameof(IsPlatinoPlanSelected)); OnPropertyChanged(nameof(IsMedicaidNumberVisible)); OnPropertyChanged(nameof(IsContigoPlusConditionVisible)); OnPropertyChanged(nameof(IsContigoEnMenteQuestionVisible)); } } }
        public string? GroupCoverage { get => _groupCoverage; set => SetProperty(ref _groupCoverage, value); }
        public string? GroupPlanType { get => _groupPlanType; set => SetProperty(ref _groupPlanType, value); }
        public string? GroupMonthlyPremium { get => _groupMonthlyPremium; set => SetProperty(ref _groupMonthlyPremium, value); }
        public DateTime GroupEffectiveDate { get => _groupEffectiveDate; set => SetProperty(ref _groupEffectiveDate, value); }
        public string? GroupSocialSecurityNumber { get => _groupSocialSecurityNumber; set => SetProperty(ref _groupSocialSecurityNumber, value); }
        public string? FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
        public string? LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
        public string? MiddleInitial { get => _middleInitial; set => SetProperty(ref _middleInitial, value); }
        public DateTime BirthDate { get => _birthDate; set => SetProperty(ref _birthDate, value); }
        public string? Sex { get => _sex; set => SetProperty(ref _sex, value); }
        public string? HomePhone { get => _homePhone; set => SetProperty(ref _homePhone, value); }
        public bool HomePhoneIsCell { get => _homePhoneIsCell; set => SetProperty(ref _homePhoneIsCell, value); }
        public string? AlternatePhone { get => _alternatePhone; set => SetProperty(ref _alternatePhone, value); }
        public bool AlternatePhoneIsCell { get => _alternatePhoneIsCell; set => SetProperty(ref _alternatePhoneIsCell, value); }
        public string? PermanentAddressLine1 { get => _permanentAddressLine1; set => SetProperty(ref _permanentAddressLine1, value); }
        public string? PermanentAddressLine2 { get => _permanentAddressLine2; set => SetProperty(ref _permanentAddressLine2, value); }
        public string? PermanentCity { get => _permanentCity; set => SetProperty(ref _permanentCity, value); }
        public string? PermanentState { get => _permanentState; set => SetProperty(ref _permanentState, value); }
        public string? PermanentZipCode { get => _permanentZipCode; set => SetProperty(ref _permanentZipCode, value); }
        public bool MailingAddressDifferent { get => _mailingAddressDifferent; set { if (SetProperty(ref _mailingAddressDifferent, value)) OnPropertyChanged(nameof(IsMailingAddressVisible)); } }
        public string? MailingAddressLine1 { get => _mailingAddressLine1; set => SetProperty(ref _mailingAddressLine1, value); }
        public string? MailingAddressLine2 { get => _mailingAddressLine2; set => SetProperty(ref _mailingAddressLine2, value); }
        public string? MailingCity { get => _mailingCity; set => SetProperty(ref _mailingCity, value); }
        public string? MailingState { get => _mailingState; set => SetProperty(ref _mailingState, value); }
        public string? MailingZipCode { get => _mailingZipCode; set => SetProperty(ref _mailingZipCode, value); }
        public string? MedicareNumber { get => _medicareNumber; set => SetProperty(ref _medicareNumber, value); }
        public string? OtherRxCoverageAnswer { get => _otherRxCoverageAnswer; set { if (SetProperty(ref _otherRxCoverageAnswer, value)) OnPropertyChanged(nameof(IsOtherCoverageDetailsVisible)); } }
        public string? OtherCoverageName { get => _otherCoverageName; set => SetProperty(ref _otherCoverageName, value); }
        public string? OtherCoverageMemberNumber { get => _otherCoverageMemberNumber; set => SetProperty(ref _otherCoverageMemberNumber, value); }
        public string? OtherCoverageGroupNumber { get => _otherCoverageGroupNumber; set => SetProperty(ref _otherCoverageGroupNumber, value); }
        public string? MedicaidProgramAnswer { get => _medicaidProgramAnswer; set { if (SetProperty(ref _medicaidProgramAnswer, value)) OnPropertyChanged(nameof(IsMedicaidNumberVisible)); } }
        public string? MedicaidNumber { get => _medicaidNumber; set => SetProperty(ref _medicaidNumber, value); }
        public string? ContigoPlusChronicCondition { get => _contigoPlusChronicCondition; set => SetProperty(ref _contigoPlusChronicCondition, value); }
        public string? ContigoEnMenteDementiaAnswer { get => _contigoEnMenteDementiaAnswer; set => SetProperty(ref _contigoEnMenteDementiaAnswer, value); }
        public bool AckPartAandB { get => _ackPartAandB; set => SetProperty(ref _ackPartAandB, value); }
        public bool AckSingleMAPlan { get => _ackSingleMAPlan; set => SetProperty(ref _ackSingleMAPlan, value); }
        public bool AckInfoCorrect { get => _ackInfoCorrect; set => SetProperty(ref _ackInfoCorrect, value); }
        public string? ApplicantSignatureName { get => _applicantSignatureName; set => SetProperty(ref _applicantSignatureName, value); }
        public DateTime SignatureDate { get => _signatureDate; set => SetProperty(ref _signatureDate, value); }
        public string? AuthorizedRepName { get => _authorizedRepName; set => SetProperty(ref _authorizedRepName, value); }
        public string? AuthorizedRepAddress { get => _authorizedRepAddress; set => SetProperty(ref _authorizedRepAddress, value); }
        public string? AuthorizedRepPhone { get => _authorizedRepPhone; set => SetProperty(ref _authorizedRepPhone, value); }
        public string? AuthorizedRepRelationship { get => _authorizedRepRelationship; set => SetProperty(ref _authorizedRepRelationship, value); }
        public bool EnrollNowChecked { get => _enrollNowChecked; set { if (SetProperty(ref _enrollNowChecked, value)) OnPropertyChanged(nameof(IsEnrollNowChecked)); } }
        public bool IsEnrollNowChecked => EnrollNowChecked;
        public DateTime EnrollNowDate { get => _enrollNowDate; set => SetProperty(ref _enrollNowDate, value); }
        public string? PhoneEnrollmentCallNumber { get => _phoneEnrollmentCallNumber; set => SetProperty(ref _phoneEnrollmentCallNumber, value); }
        public string? PhoneEnrollmentWitnessSignature { get => _phoneEnrollmentWitnessSignature; set => SetProperty(ref _phoneEnrollmentWitnessSignature, value); }
        public DateTime PhoneEnrollmentWitnessDate { get => _phoneEnrollmentWitnessDate; set => SetProperty(ref _phoneEnrollmentWitnessDate, value); }
        public string? PreferredSpokenLanguage { get => _preferredSpokenLanguage; set => SetProperty(ref _preferredSpokenLanguage, value); }
        public string? PreferredAccessibleFormat { get => _preferredAccessibleFormat; set => SetProperty(ref _preferredAccessibleFormat, value); }
        public bool FormatBraille { get => _formatBraille; set => SetProperty(ref _formatBraille, value); }
        public bool FormatLargeText { get => _formatLargeText; set => SetProperty(ref _formatLargeText, value); }
        public bool FormatAudioCD { get => _formatAudioCD; set => SetProperty(ref _formatAudioCD, value); }
        public bool FormatDataCD { get => _formatDataCD; set => SetProperty(ref _formatDataCD, value); }
        public string? WorkStatusAnswer { get => _workStatusAnswer; set => SetProperty(ref _workStatusAnswer, value); }
        public string? SpouseWorkStatusAnswer { get => _spouseWorkStatusAnswer; set => SetProperty(ref _spouseWorkStatusAnswer, value); }
        public string? PcpName { get => _pcpName; set => SetProperty(ref _pcpName, value); }
        public string? PcpPhone { get => _pcpPhone; set => SetProperty(ref _pcpPhone, value); }
        public bool WantsProviderDirectoryByEmail { get => _wantsProviderDirectoryByEmail; set => SetProperty(ref _wantsProviderDirectoryByEmail, value); }
        public bool WantsAnnualNoticeByEmail { get => _wantsAnnualNoticeByEmail; set => SetProperty(ref _wantsAnnualNoticeByEmail, value); }
        public bool WantsEvidenceOfCoverageByEmail { get => _wantsEvidenceOfCoverageByEmail; set => SetProperty(ref _wantsEvidenceOfCoverageByEmail, value); }
        public bool WantsSummaryOfBenefitsByEmail { get => _wantsSummaryOfBenefitsByEmail; set => SetProperty(ref _wantsSummaryOfBenefitsByEmail, value); }
        public bool WantsFormularyByEmail { get => _wantsFormularyByEmail; set => SetProperty(ref _wantsFormularyByEmail, value); }
        public bool WantsPromotionalByEmail { get => _wantsPromotionalByEmail; set => SetProperty(ref _wantsPromotionalByEmail, value); }
        public bool WantsEnrollmentConfirmationByEmail { get => _wantsEnrollmentConfirmationByEmail; set => SetProperty(ref _wantsEnrollmentConfirmationByEmail, value); }
        public string? TextConsentAnswer { get => _textConsentAnswer; set { if (SetProperty(ref _textConsentAnswer, value)) OnPropertyChanged(nameof(IsTextNumberVisible)); } }
        public string? TextConsentNumber { get => _textConsentNumber; set => SetProperty(ref _textConsentNumber, value); }
        public string? EmailConsentAnswer { get => _emailConsentAnswer; set { if (SetProperty(ref _emailConsentAnswer, value)) OnPropertyChanged(nameof(IsEmailAddressVisible)); } }
        public string? EmailConsentAddress { get => _emailConsentAddress; set => SetProperty(ref _emailConsentAddress, value); }
        public string? EmergencyContactName { get => _emergencyContactName; set => SetProperty(ref _emergencyContactName, value); }
        public string? EmergencyContactPhone { get => _emergencyContactPhone; set => SetProperty(ref _emergencyContactPhone, value); }
        public string? EmergencyContactRelationship { get => _emergencyContactRelationship; set => SetProperty(ref _emergencyContactRelationship, value); }
        public string? IsRetireeAnswer { get => _isRetireeAnswer; set { if (SetProperty(ref _isRetireeAnswer, value)) { OnPropertyChanged(nameof(IsRetirementDetailsVisible)); OnPropertyChanged(nameof(IsRetirementDateVisible)); OnPropertyChanged(nameof(IsRetireeNameVisible)); } } }
        public DateTime RetirementDate { get => _retirementDate; set => SetProperty(ref _retirementDate, value); }
        public string? RetireeName { get => _retireeName; set => SetProperty(ref _retireeName, value); }
        public string? CoversSpouseOrDependentsAnswer { get => _coversSpouseOrDependentsAnswer; set { if (SetProperty(ref _coversSpouseOrDependentsAnswer, value)) OnPropertyChanged(nameof(IsSpouseDependentDetailsVisible)); } }
        public string? SpouseName { get => _spouseName; set => SetProperty(ref _spouseName, value); }
        public string? DependentNames { get => _dependentNames; set => SetProperty(ref _dependentNames, value); }
        public string? IsLongTermCareResidentAnswer { get => _isLongTermCareResidentAnswer; set { if (SetProperty(ref _isLongTermCareResidentAnswer, value)) OnPropertyChanged(nameof(IsLtcDetailsVisible)); } }
        public string? LtcInstitutionName { get => _ltcInstitutionName; set => SetProperty(ref _ltcInstitutionName, value); }
        public string? LtcAdministratorName { get => _ltcAdministratorName; set => SetProperty(ref _ltcAdministratorName, value); }
        public string? LtcPhone { get => _ltcPhone; set => SetProperty(ref _ltcPhone, value); }
        public string? CurrentHealthPlan { get => _currentHealthPlan; set { if (SetProperty(ref _currentHealthPlan, value)) OnPropertyChanged(nameof(IsOtherHealthPlanNameVisible)); } }
        public string? OtherHealthPlanName { get => _otherHealthPlanName; set => SetProperty(ref _otherHealthPlanName, value); }
        public string? TransitionLastNames { get => _transitionLastNames; set => SetProperty(ref _transitionLastNames, value); }
        public string? TransitionName { get => _transitionName; set => SetProperty(ref _transitionName, value); }
        public string? TransitionInitial { get => _transitionInitial; set => SetProperty(ref _transitionInitial, value); }
        public DateTime TransitionDateOfBirth { get => _transitionDateOfBirth; set => SetProperty(ref _transitionDateOfBirth, value); }
        public string? TransitionTelephone1 { get => _transitionTelephone1; set => SetProperty(ref _transitionTelephone1, value); }
        public string? TransitionTelephone2 { get => _transitionTelephone2; set => SetProperty(ref _transitionTelephone2, value); }
        public string? TransitionBenefitPlan { get => _transitionBenefitPlan; set => SetProperty(ref _transitionBenefitPlan, value); }
        public DateTime TransitionEffectivityDate { get => _transitionEffectivityDate; set => SetProperty(ref _transitionEffectivityDate, value); }
        public string? TransitionShicMedicareNumber { get => _transitionShicMedicareNumber; set => SetProperty(ref _transitionShicMedicareNumber, value); }
        public string? TransitionEquipmentServices { get => _transitionEquipmentServices; set { if (SetProperty(ref _transitionEquipmentServices, value)) OnPropertyChanged(nameof(IsTransitionEquipmentDetailsVisible)); } }
        public string? TransitionProviderCompany { get => _transitionProviderCompany; set => SetProperty(ref _transitionProviderCompany, value); }
        public string? TransitionEquipmentEffectivity { get => _transitionEquipmentEffectivity; set => SetProperty(ref _transitionEquipmentEffectivity, value); }
        public string? TransitionPreviousHealthPlan { get => _transitionPreviousHealthPlan; set => SetProperty(ref _transitionPreviousHealthPlan, value); }
        public string? TransitionComments { get => _transitionComments; set => SetProperty(ref _transitionComments, value); }
        public string? TransitionInformationProvidedBy { get => _transitionInformationProvidedBy; set => SetProperty(ref _transitionInformationProvidedBy, value); }
        public string? TransitionPlanRepresentative { get => _transitionPlanRepresentative; set => SetProperty(ref _transitionPlanRepresentative, value); }
        public string? TransitionRegion { get => _transitionRegion; set => SetProperty(ref _transitionRegion, value); }
        public DateTime TransitionFormDate { get => _transitionFormDate; set => SetProperty(ref _transitionFormDate, value); }
        public string? PaymentOption { get => _paymentOption; set { if (SetProperty(ref _paymentOption, value)) { OnPropertyChanged(nameof(IsEftFieldsVisible)); OnPropertyChanged(nameof(IsCreditCardFieldsVisible)); OnPropertyChanged(nameof(IsAutoDeductionFieldsVisible)); } } }
        public string? EftAccountHolderName { get => _eftAccountHolderName; set => SetProperty(ref _eftAccountHolderName, value); }
        public string? EftRoutingNumber { get => _eftRoutingNumber; set => SetProperty(ref _eftRoutingNumber, value); }
        public string? EftAccountNumber { get => _eftAccountNumber; set => SetProperty(ref _eftAccountNumber, value); }
        public string? EftAccountType { get => _eftAccountType; set => SetProperty(ref _eftAccountType, value); }
        public string? CreditCardType { get => _creditCardType; set => SetProperty(ref _creditCardType, value); }
        public string? CreditCardHolderName { get => _creditCardHolderName; set => SetProperty(ref _creditCardHolderName, value); }
        public string? CreditCardNumber { get => _creditCardNumber; set => SetProperty(ref _creditCardNumber, value); }
        public string? CreditCardExpiration { get => _creditCardExpiration; set => SetProperty(ref _creditCardExpiration, value); }
        public string? AutoDeductionBenefitSource { get => _autoDeductionBenefitSource; set => SetProperty(ref _autoDeductionBenefitSource, value); }
        public bool ReceivedInitialPackage { get => _receivedInitialPackage; set => SetProperty(ref _receivedInitialPackage, value); }
        public bool ReceivedStarRatingsNotice { get => _receivedStarRatingsNotice; set => SetProperty(ref _receivedStarRatingsNotice, value); }
        public bool ReceivedWebAvailabilityNotice { get => _receivedWebAvailabilityNotice; set => SetProperty(ref _receivedWebAvailabilityNotice, value); }
        public bool ReceivedEnrollmentConfirmation { get => _receivedEnrollmentConfirmation; set => SetProperty(ref _receivedEnrollmentConfirmation, value); }
        public bool ReceivedEnrollmentFormCopy { get => _receivedEnrollmentFormCopy; set => SetProperty(ref _receivedEnrollmentFormCopy, value); }
        public bool ReceivedAttestationOfEligibility { get => _receivedAttestationOfEligibility; set => SetProperty(ref _receivedAttestationOfEligibility, value); }
        public bool ReceivedPrecertificationChronicDiseases { get => _receivedPrecertificationChronicDiseases; set => SetProperty(ref _receivedPrecertificationChronicDiseases, value); }
        public bool ReceivedPhiAuthorization { get => _receivedPhiAuthorization; set => SetProperty(ref _receivedPhiAuthorization, value); }
        public string? HelperName { get => _helperName; set => SetProperty(ref _helperName, value); }
        public string? HelperRelationship { get => _helperRelationship; set => SetProperty(ref _helperRelationship, value); }
        public string? HelperSignature { get => _helperSignature; set => SetProperty(ref _helperSignature, value); }
        public string? HelperNpn { get => _helperNpn; set => SetProperty(ref _helperNpn, value); }
        public string? OfficialReceiptDate { get => _officialReceiptDate; set => SetProperty(ref _officialReceiptDate, value); }
        public string? OfficialPlanId { get => _officialPlanId; set => SetProperty(ref _officialPlanId, value); }
        public string? OfficialCoverageEffectiveDate { get => _officialCoverageEffectiveDate; set => SetProperty(ref _officialCoverageEffectiveDate, value); }
        public bool ConfirmSection1Complete { get => _confirmSection1Complete; set => SetProperty(ref _confirmSection1Complete, value); }
        public bool ConfirmSection2Reviewed { get => _confirmSection2Reviewed; set => SetProperty(ref _confirmSection2Reviewed, value); }

        public ICommand NextCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand SubmitCommand { get; }

        public TripleSEnrollmentWizardViewModel()
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
            ValidationMessage = "Enrollment form validation passed and is ready for PDF field mapping.";
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
                    if (string.IsNullOrWhiteSpace(ScopeOfAppointmentNumber) || string.IsNullOrWhiteSpace(SelectedPlanName) || string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) || string.IsNullOrWhiteSpace(Sex) || string.IsNullOrWhiteSpace(HomePhone))
                    { message = "Complete required Step 1 fields."; return false; }
                    if (!string.IsNullOrWhiteSpace(GroupCoverage) && (string.IsNullOrWhiteSpace(GroupPlanType) || string.IsNullOrWhiteSpace(GroupSocialSecurityNumber)))
                    { message = "Group Plan Type and SSN are required for group coverage."; return false; }
                    break;
                case 2:
                    if (string.IsNullOrWhiteSpace(PermanentAddressLine1) || string.IsNullOrWhiteSpace(PermanentCity) || string.IsNullOrWhiteSpace(PermanentState) || string.IsNullOrWhiteSpace(PermanentZipCode) || string.IsNullOrWhiteSpace(MedicareNumber))
                    { message = "Complete required Step 2 fields."; return false; }
                    if (MailingAddressDifferent && (string.IsNullOrWhiteSpace(MailingAddressLine1) || string.IsNullOrWhiteSpace(MailingCity) || string.IsNullOrWhiteSpace(MailingState) || string.IsNullOrWhiteSpace(MailingZipCode)))
                    { message = "Complete mailing address fields."; return false; }
                    break;
                case 3:
                    if (!IsYesNo(OtherRxCoverageAnswer)) { message = "Select Yes or No for other Rx coverage."; return false; }
                    if (IsYes(OtherRxCoverageAnswer) && (string.IsNullOrWhiteSpace(OtherCoverageName) || string.IsNullOrWhiteSpace(OtherCoverageMemberNumber) || string.IsNullOrWhiteSpace(OtherCoverageGroupNumber)))
                    { message = "Complete other coverage details."; return false; }
                    if (IsPlatinoPlan(SelectedPlanName) && !IsYesNo(MedicaidProgramAnswer)) { message = "Select Yes or No for Medicaid (Platino)."; return false; }
                    if (IsPlatinoPlan(SelectedPlanName) && IsYes(MedicaidProgramAnswer) && string.IsNullOrWhiteSpace(MedicaidNumber)) { message = "Medicaid number is required."; return false; }
                    if (IsContigoPlusPlan(SelectedPlanName) && string.IsNullOrWhiteSpace(ContigoPlusChronicCondition)) { message = "Select chronic condition for Contigo Plus."; return false; }
                    if (IsContigoEnMentePlan(SelectedPlanName) && !IsYesNo(ContigoEnMenteDementiaAnswer)) { message = "Select dementia answer for ContigoEnMente."; return false; }
                    break;
                case 4:
                    if (!AckPartAandB || !AckSingleMAPlan || !AckInfoCorrect || string.IsNullOrWhiteSpace(ApplicantSignatureName))
                    { message = "Complete acknowledgements and applicant signature name."; return false; }
                    break;
                case 5:
                    if (!IsEmptyOrYesNo(WorkStatusAnswer) || !IsEmptyOrYesNo(SpouseWorkStatusAnswer) || !IsEmptyOrYesNo(TextConsentAnswer) || !IsEmptyOrYesNo(EmailConsentAnswer))
                    { message = "Yes/No fields in Step 5 must be valid."; return false; }
                    if (IsYes(TextConsentAnswer) && string.IsNullOrWhiteSpace(TextConsentNumber)) { message = "Text number required when consent is Yes."; return false; }
                    if (IsYes(EmailConsentAnswer) && string.IsNullOrWhiteSpace(EmailConsentAddress)) { message = "Email address required when consent is Yes."; return false; }
                    if (IsYes(IsLongTermCareResidentAnswer) && (string.IsNullOrWhiteSpace(LtcInstitutionName) || string.IsNullOrWhiteSpace(LtcAdministratorName) || string.IsNullOrWhiteSpace(LtcPhone)))
                    { message = "Complete LTC institution details when resident = Yes."; return false; }
                    if (string.Equals(CurrentHealthPlan, "Other", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(OtherHealthPlanName))
                    { message = "Specify other health plan name when Other is selected."; return false; }
                    break;
                case 6:
                    if (string.IsNullOrWhiteSpace(TransitionLastNames) || string.IsNullOrWhiteSpace(TransitionName) || string.IsNullOrWhiteSpace(TransitionTelephone1) || string.IsNullOrWhiteSpace(TransitionBenefitPlan) || string.IsNullOrWhiteSpace(TransitionShicMedicareNumber) || string.IsNullOrWhiteSpace(TransitionInformationProvidedBy) || string.IsNullOrWhiteSpace(TransitionPlanRepresentative) || string.IsNullOrWhiteSpace(TransitionRegion))
                    { message = "Complete required transition form fields."; return false; }
                    if (!string.IsNullOrWhiteSpace(TransitionEquipmentServices) && (string.IsNullOrWhiteSpace(TransitionProviderCompany) || string.IsNullOrWhiteSpace(TransitionEquipmentEffectivity)))
                    { message = "Provider and effectivity required when equipment/service is selected."; return false; }
                    break;
                case 7:
                    if (string.IsNullOrWhiteSpace(PaymentOption)) { message = "Select payment option."; return false; }
                    if (IsEftFieldsVisible && (string.IsNullOrWhiteSpace(EftAccountHolderName) || string.IsNullOrWhiteSpace(EftRoutingNumber) || string.IsNullOrWhiteSpace(EftAccountNumber) || string.IsNullOrWhiteSpace(EftAccountType)))
                    { message = "Complete EFT fields."; return false; }
                    if (IsCreditCardFieldsVisible && (string.IsNullOrWhiteSpace(CreditCardType) || string.IsNullOrWhiteSpace(CreditCardHolderName) || string.IsNullOrWhiteSpace(CreditCardNumber) || string.IsNullOrWhiteSpace(CreditCardExpiration)))
                    { message = "Complete Credit Card fields."; return false; }
                    if (IsAutoDeductionFieldsVisible && string.IsNullOrWhiteSpace(AutoDeductionBenefitSource))
                    { message = "Benefit source required for Auto Deduction."; return false; }
                    break;
                case 11:
                    if (!ConfirmSection1Complete || !ConfirmSection2Reviewed)
                    { message = "Confirm final review checkboxes."; return false; }
                    break;
            }
            return true;
        }

        private static bool IsYes(string? value) => string.Equals(value, "Yes", StringComparison.OrdinalIgnoreCase);
        private static bool IsYesNo(string? value) => string.Equals(value, "Yes", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "No", StringComparison.OrdinalIgnoreCase);
        private static bool IsEmptyOrYesNo(string? value) => string.IsNullOrWhiteSpace(value) || IsYesNo(value);
        private static bool IsPlatinoPlan(string? plan) => !string.IsNullOrWhiteSpace(plan) && plan.Contains("Platino", StringComparison.OrdinalIgnoreCase);
        private static bool IsContigoPlusPlan(string? plan) => !string.IsNullOrWhiteSpace(plan) && plan.Contains("Contigo Plus", StringComparison.OrdinalIgnoreCase);
        private static bool IsContigoEnMentePlan(string? plan) => !string.IsNullOrWhiteSpace(plan) && plan.Contains("ContigoEnMente", StringComparison.OrdinalIgnoreCase);

        private void RaiseStepVisibilityChanged()
        {
            OnPropertyChanged(nameof(IsStep1Visible));
            OnPropertyChanged(nameof(IsStep2Visible));
            OnPropertyChanged(nameof(IsStep3Visible));
            OnPropertyChanged(nameof(IsStep4Visible));
            OnPropertyChanged(nameof(IsStep5Visible));
            OnPropertyChanged(nameof(IsStep6Visible));
            OnPropertyChanged(nameof(IsStep7Visible));
            OnPropertyChanged(nameof(IsStep8Visible));
            OnPropertyChanged(nameof(IsStep9Visible));
            OnPropertyChanged(nameof(IsStep10Visible));
            OnPropertyChanged(nameof(IsStep11Visible));
        }
    }
}
