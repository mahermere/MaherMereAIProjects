using TripleS.SOA.AEP.Models;
using TripleS.SOA.AEP.Models.Domain;
using TripleS.SOA.AEP.Models.ViewModels;

namespace TripleS.SOA.AEP.UI.ViewModels
{
    /// <summary>
    /// ViewModel for SOA (Signature of Authority) Wizard
    /// Manages the 4-step SOA creation workflow
    /// </summary>
    public class SOAWizardViewModel : BaseViewModel
    {
        private int _currentStep;
        private SignatureOfAuthority? _currentSOA;
        private string? _soaNumber;
                /// <summary>
                /// Gets the unique SOA number for this wizard instance
                /// </summary>
                public string? SOANumber
                {
                    get => _soaNumber;
                    set => SetProperty(ref _soaNumber, value);
                }
        private string? _errorMessage;
        private bool _isLoading;

        // Step 1 - Beneficiary Info
        private string? _beneficiaryFirstName;
        private string? _beneficiaryLastName;
        private DateTime _beneficiaryDOB;
        private string? _medicareNumber;
        private string? _phoneNumber;

        // Step 2 - Scope
        private bool _medicareAdvantageSelected;
        private bool _partDSelected;
        private bool _supplementalSelected;
        private bool _dentalVisionSelected;
        private bool _hearingAidSelected;
        private bool _wellnessSelected;
        private bool _productInformationProvided;

        // Step 3 - Meeting Details
        private DateTime _meetingDate;
        private string? _meetingLocation;
        private bool _complianceDocumentsProvided;

        // Step 4 - Signatures
        private byte[]? _beneficiarySignatureData;
        private byte[]? _agentSignatureData;

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
        public string? BeneficiaryFirstName
        {
            get => _beneficiaryFirstName;
            set => SetProperty(ref _beneficiaryFirstName, value);
        }

        public string? BeneficiaryLastName
        {
            get => _beneficiaryLastName;
            set => SetProperty(ref _beneficiaryLastName, value);
        }

        public DateTime BeneficiaryDOB
        {
            get => _beneficiaryDOB;
            set => SetProperty(ref _beneficiaryDOB, value);
        }

        public string? MedicareNumber
        {
            get => _medicareNumber;
            set => SetProperty(ref _medicareNumber, value);
        }

        public string? PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        // Step 2 Properties
        public bool MedicareAdvantageSelected
        {
            get => _medicareAdvantageSelected;
            set => SetProperty(ref _medicareAdvantageSelected, value);
        }

        public bool PartDSelected
        {
            get => _partDSelected;
            set => SetProperty(ref _partDSelected, value);
        }

        public bool SupplementalSelected
        {
            get => _supplementalSelected;
            set => SetProperty(ref _supplementalSelected, value);
        }

        public bool DentalVisionSelected
        {
            get => _dentalVisionSelected;
            set => SetProperty(ref _dentalVisionSelected, value);
        }

        public bool HearingAidSelected
        {
            get => _hearingAidSelected;
            set => SetProperty(ref _hearingAidSelected, value);
        }

        public bool WellnessSelected
        {
            get => _wellnessSelected;
            set => SetProperty(ref _wellnessSelected, value);
        }

        public bool ProductInformationProvided
        {
            get => _productInformationProvided;
            set => SetProperty(ref _productInformationProvided, value);
        }

        // Step 3 Properties
        public DateTime MeetingDate
        {
            get => _meetingDate;
            set => SetProperty(ref _meetingDate, value);
        }

        public string? MeetingLocation
        {
            get => _meetingLocation;
            set => SetProperty(ref _meetingLocation, value);
        }

        public bool ComplianceDocumentsProvided
        {
            get => _complianceDocumentsProvided;
            set => SetProperty(ref _complianceDocumentsProvided, value);
        }

        // Step 4 Properties
        public byte[]? BeneficiarySignatureData
        {
            get => _beneficiarySignatureData;
            set => SetProperty(ref _beneficiarySignatureData, value);
        }

        public byte[]? AgentSignatureData
        {
            get => _agentSignatureData;
            set => SetProperty(ref _agentSignatureData, value);
        }

        public SOAWizardViewModel()
        {
            CurrentStep = 1;
            _currentSOA = new SignatureOfAuthority();
            BeneficiaryDOB = DateTime.Now.AddYears(-65);
            MeetingDate = DateTime.Now;
            MedicareAdvantageSelected = true;
            ProductInformationProvided = true;
        }

        /// <summary>
        /// Validates all SOA data and creates the SOA document
        /// </summary>
        public async Task<(bool Success, string Message, string? SOAId)> CreateSOAAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                // Validate all steps
                if (!ValidateStep1())
                    throw new InvalidOperationException("Step 1 validation failed");

                if (!ValidateStep2())
                    throw new InvalidOperationException("Step 2 validation failed");

                if (!ValidateStep3())
                    throw new InvalidOperationException("Step 3 validation failed");

                if (!ValidateStep4())
                    throw new InvalidOperationException("Step 4 validation failed");

                // Build SOA object
                // Generate unique SOA number (timestamp + random)
                SOANumber = $"SOA-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0,8)}";
                _currentSOA = new SignatureOfAuthority
                {
                    SOAId = 0, // If you want to use int, otherwise store SOANumber
                    BeneficiaryName = $"{BeneficiaryFirstName} {BeneficiaryLastName}",
                    MeetingDate = MeetingDate,
                    MeetingLocation = MeetingLocation ?? "",
                    MedicareAdvantageSelected = MedicareAdvantageSelected,
                    ProductInformationProvided = ProductInformationProvided,
                    ComplianceDocumentsProvided = ComplianceDocumentsProvided,
                    BeneficiarySignatureData = BeneficiarySignatureData,
                    Status = "Signed",
                    CompletedDate = DateTime.Now
                };

                // Add SOA record to shared service
                TripleS.SOA.AEP.UI.Services.SOANumberService.AddSOARecord(new TripleS.SOA.AEP.UI.Services.SOANumberService.SOARecord {
                    SOANumber = SOANumber,
                    FirstName = BeneficiaryFirstName ?? string.Empty,
                    LastName = BeneficiaryLastName ?? string.Empty
                });
                // TODO: Call service to save SOA to database
                await Task.Delay(500); // Simulate network call

                return (true, "SOA created successfully", SOANumber);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating SOA: {ex.Message}";
                return (false, ErrorMessage, null);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool ValidateStep1()
        {
            return !string.IsNullOrWhiteSpace(BeneficiaryFirstName)
                && !string.IsNullOrWhiteSpace(BeneficiaryLastName)
                && !string.IsNullOrWhiteSpace(MedicareNumber)
                && !string.IsNullOrWhiteSpace(PhoneNumber);
        }

        private bool ValidateStep2()
        {
            return ProductInformationProvided;
        }

        private bool ValidateStep3()
        {
            return ComplianceDocumentsProvided && MeetingDate != default;
        }

        private bool ValidateStep4()
        {
            return BeneficiarySignatureData != null && BeneficiarySignatureData.Length > 0
                && AgentSignatureData != null && AgentSignatureData.Length > 0;
        }
    }
}
