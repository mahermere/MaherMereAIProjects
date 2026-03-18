using System;
using Triple_S_Maui_AEP.Models;
using Triple_S_Maui_AEP.Services;

namespace Triple_S_Maui_AEP.ViewModels
{
    /// <summary>
    /// ViewModel for SOA (Signature of Authority) Wizard
    /// Manages the 4-step SOA creation workflow
    /// </summary>
    public class SOAWizardViewModel : BaseViewModel
    {
        private int _currentStep;
        private SOAFirstPageRecord? _currentSOA;
        private string? _soaNumber;
        private string? _errorMessage;
        private bool _isLoading;
        private string? _generatedPdfPath;

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
        private string? _beneficiarySignatureBase64;
        private string? _agentSignatureBase64;

        public SOAFirstPageRecord? CurrentSOA
        {
            get => _currentSOA ??= new SOAFirstPageRecord();
            set => SetProperty(ref _currentSOA, value);
        }

        public int CurrentStep
        {
            get => _currentStep;
            set => SetProperty(ref _currentStep, value);
        }

        public int TotalSteps => 4;

        /// <summary>
        /// Gets the unique SOA number for this wizard instance
        /// </summary>
        public string? SOANumber
        {
            get => _soaNumber;
            set => SetProperty(ref _soaNumber, value);
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

        public string? GeneratedPdfPath
        {
            get => _generatedPdfPath;
            set => SetProperty(ref _generatedPdfPath, value);
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
        public string? BeneficiarySignatureBase64
        {
            get => _beneficiarySignatureBase64;
            set => SetProperty(ref _beneficiarySignatureBase64, value);
        }

        public string? AgentSignatureBase64
        {
            get => _agentSignatureBase64;
            set => SetProperty(ref _agentSignatureBase64, value);
        }

        public SOAWizardViewModel()
        {
            CurrentStep = 1;
            BeneficiaryDOB = DateTime.Now.AddYears(-65);
            MeetingDate = DateTime.Now;
            MedicareAdvantageSelected = true;
            ProductInformationProvided = true;
            SOANumber = new SOANumberService().GenerateSOANumber();
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

        /// <summary>
        /// Validates and saves the current step data
        /// </summary>
        public async Task<bool> SaveCurrentStepAsync()
        {
            ErrorMessage = string.Empty;

            var (isValid, validationError) = await ValidateCurrentStepAsync();
            if (!isValid)
            {
                ErrorMessage = validationError;
                return false;
            }

            if (CurrentStep < TotalSteps)
            {
                CurrentStep++;
            }

            await Task.Delay(200);
            return true;
        }

        public async Task<(bool Success, string ErrorMessage)> ValidateCurrentStepAsync()
        {
            return await Task.FromResult(CurrentStep switch
            {
                1 => ValidateStep1() ? (true, string.Empty) : (false, "Please fill in all required beneficiary information"),
                2 => ValidateStep2() ? (true, string.Empty) : (false, "Please select at least one product"),
                3 => ValidateStep3() ? (true, string.Empty) : (false, "Please provide meeting details and confirm compliance"),
                4 => ValidateStep4() ? (true, string.Empty) : (false, "Please provide all required signatures"),
                _ => (false, "Invalid step")
            });
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
            return ProductInformationProvided &&
                   (MedicareAdvantageSelected || PartDSelected || SupplementalSelected ||
                    DentalVisionSelected || HearingAidSelected || WellnessSelected);
        }

        private bool ValidateStep3()
        {
            return ComplianceDocumentsProvided && MeetingDate != default;
        }

        private bool ValidateStep4()
        {
            return !string.IsNullOrWhiteSpace(BeneficiarySignatureBase64)
                && !string.IsNullOrWhiteSpace(AgentSignatureBase64);
        }

        /// <summary>
        /// Submits the SOA document after all steps are completed
        /// </summary>
        public async Task<bool> SubmitSOAAsync()
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

                if (CurrentSOA == null)
                {
                    CurrentSOA = new Models.SOAFirstPageRecord();
                }

                // Update CurrentSOA with all properties
                CurrentSOA.FirstName = BeneficiaryFirstName ?? string.Empty;
                CurrentSOA.LastName = BeneficiaryLastName ?? string.Empty;
                CurrentSOA.DateOfBirth = BeneficiaryDOB;
                CurrentSOA.MedicareNumber = MedicareNumber ?? string.Empty;
                CurrentSOA.PrimaryPhone = PhoneNumber ?? string.Empty;
                CurrentSOA.EmployeeSignatureBase64 = BeneficiarySignatureBase64 ?? string.Empty;
                CurrentSOA.AgentSignatureBase64 = AgentSignatureBase64 ?? string.Empty;

                var selectedProducts = new List<string>();
                if (MedicareAdvantageSelected) selectedProducts.Add("Medicare Advantage");
                if (PartDSelected) selectedProducts.Add("Part D");
                if (SupplementalSelected) selectedProducts.Add("Supplemental");
                if (DentalVisionSelected) selectedProducts.Add("Dental/Vision");
                if (HearingAidSelected) selectedProducts.Add("Hearing Aid");
                if (WellnessSelected) selectedProducts.Add("Wellness");

                var pdfService = new PdfService();
                var pdfBytes = await pdfService.GenerateSOAPdfAsync(
                    CurrentSOA,
                    SOANumber ?? string.Empty,
                    MeetingDate,
                    MeetingLocation,
                    ProductInformationProvided,
                    ComplianceDocumentsProvided,
                    selectedProducts);

                GeneratedPdfPath = await pdfService.SavePdfAsync(
                    pdfBytes,
                    $"{SOANumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");

                if (string.IsNullOrWhiteSpace(GeneratedPdfPath))
                {
                    throw new InvalidOperationException("Failed to save SOA PDF");
                }

                await SOAService.AddSOARecordAsync(new SOAService.SOARecord
                {
                    SOANumber = SOANumber ?? string.Empty,
                    FirstName = BeneficiaryFirstName ?? string.Empty,
                    LastName = BeneficiaryLastName ?? string.Empty,
                    DateCreated = DateTime.Now,
                    FilePath = GeneratedPdfPath,
                    IsUploaded = false
                });

                await Task.Delay(100);

                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error submitting SOA: {ex.Message}";
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Gets SOA summary for final review
        /// </summary>
        public string GetSOASummary()
        {
            return $"Beneficiary: {BeneficiaryFirstName} {BeneficiaryLastName}\n" +
                   $"Medicare #: {MedicareNumber}\n" +
                   $"Meeting Date: {MeetingDate:MM/dd/yyyy}\n" +
                   $"Location: {MeetingLocation}";
        }

        public string GetStepTitle()
        {
            return CurrentStep switch
            {
                1 => "Beneficiary Information",
                2 => "Coverage Scope",
                3 => "Meeting Details",
                4 => "Signatures",
                _ => "SOA Wizard"
            };
        }

        public string GetStepTitleSpanish()
        {
            return CurrentStep switch
            {
                1 => "Información del Beneficiario",
                2 => "Alcance de Cobertura",
                3 => "Detalles de la Reunión",
                4 => "Firmas",
                _ => "Asistente de SOA"
            };
        }

        /// <summary>
        /// Resets all form fields to defaults for the next SOA
        /// </summary>
        public void ResetForm()
        {
            CurrentStep = 1;
            ErrorMessage = string.Empty;
            IsLoading = false;
            CurrentSOA = new SOAFirstPageRecord();
            SOANumber = new SOANumberService().GenerateSOANumber();
            GeneratedPdfPath = string.Empty;

            // Reset Step 1 - Beneficiary Info
            BeneficiaryFirstName = string.Empty;
            BeneficiaryLastName = string.Empty;
            BeneficiaryDOB = DateTime.Now.AddYears(-65);
            MedicareNumber = string.Empty;
            PhoneNumber = string.Empty;

            // Reset Step 2 - Scope
            MedicareAdvantageSelected = false;
            PartDSelected = false;
            SupplementalSelected = false;
            DentalVisionSelected = false;
            HearingAidSelected = false;
            WellnessSelected = false;
            ProductInformationProvided = false;

            // Reset Step 3 - Meeting Details
            MeetingDate = DateTime.Now;
            MeetingLocation = string.Empty;
            ComplianceDocumentsProvided = false;

            // Reset Step 4 - Signatures
            BeneficiarySignatureBase64 = string.Empty;
            AgentSignatureBase64 = string.Empty;

            System.Diagnostics.Debug.WriteLine("SOA form reset successfully");
        }
    }
}
