using CommunityToolkit.Maui.Storage;
using Triple_S_AEP_MAUI_Forms.Models;
using Triple_S_AEP_MAUI_Forms.Services;

namespace Triple_S_AEP_MAUI_Forms
{
    public partial class AepFormWizardPage : ContentPage
    {
        private readonly PdfFlattenService _pdfFlattenService = new();
        private readonly EnrollmentDatabaseService _dbService = EnrollmentDatabaseService.Instance;
        private readonly DmsUploadService _dmsUploadService = new();
        private int _currentStep = 1;
        private string? _applicantSignatureDataUrl;
        private string? _witnessSignatureDataUrl;
        private string? _helperSignatureDataUrl;
        private bool _isSubmitting = false;
        private bool _isFirstAppearance = true;
        private EnrollmentRecord? _currentRecord;
        private EnrollmentRecord? _linkedSoaRecord;

        public AepFormWizardPage()
        {
            InitializeComponent();
            UpdateStepUi();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Only reset form on first appearance from dashboard
            if (_isFirstAppearance)
            {
                _isFirstAppearance = false;
                return;
            }
            
            // Only reset after successful submission completes
            if (!_isSubmitting)
            {
                // Don't reset during normal workflow - only track if we're back from submission
                return;
            }
        }

        private void OnReviewInstructionsChanged(object? sender, CheckedChangedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.Content?.ToString() == "Yes" && e.Value)
            {
                OpenInstructionsViewerButton.IsVisible = true;
                return;
            }

            if (sender is RadioButton noButton && noButton.Content?.ToString() == "No" && e.Value)
            {
                OpenInstructionsViewerButton.IsVisible = false;
            }
        }

        private async void OnOpenInstructionsViewerClicked(object? sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new InstructionsViewerPage());
        }

        private async void OnSelectSoaRecordClicked(object? sender, EventArgs e)
        {
            var soaSelector = new SoaSelectorPage();
            await Navigation.PushModalAsync(soaSelector);
            
            var selectedSoa = await soaSelector.Result;
            if (selectedSoa != null)
            {
                _linkedSoaRecord = selectedSoa;
                
                // Auto-fill all matching fields from linked SOA
                if (!string.IsNullOrEmpty(selectedSoa.BeneficiaryFirstName))
                    FirstNameEntry.Text = selectedSoa.BeneficiaryFirstName;
                
                if (!string.IsNullOrEmpty(selectedSoa.BeneficiaryLastName))
                    LastNameEntry.Text = selectedSoa.BeneficiaryLastName;
                
                if (!string.IsNullOrEmpty(selectedSoa.BeneficiaryMiddleInitial))
                    MiddleInitialEntry.Text = selectedSoa.BeneficiaryMiddleInitial;
                
                if (selectedSoa.BeneficiaryDOB.HasValue)
                    BirthDatePicker.Date = selectedSoa.BeneficiaryDOB.Value;
                
                if (!string.IsNullOrEmpty(selectedSoa.BeneficiaryPhone))
                    HomePhoneEntry.Text = selectedSoa.BeneficiaryPhone;
                
                if (!string.IsNullOrEmpty(selectedSoa.BeneficiaryAltPhone))
                    AlternatePhoneEntry.Text = selectedSoa.BeneficiaryAltPhone;
                
                if (!string.IsNullOrEmpty(selectedSoa.BeneficiaryEmail))
                    EmailAddressEntry.Text = selectedSoa.BeneficiaryEmail;
                
                if (!string.IsNullOrEmpty(selectedSoa.BeneficiaryAddress1))
                    ResidenceStreetEntry.Text = selectedSoa.BeneficiaryAddress1;
                
                if (!string.IsNullOrEmpty(selectedSoa.BeneficiaryAddress2))
                    ResidenceStreet2Entry.Text = selectedSoa.BeneficiaryAddress2;
                
                if (!string.IsNullOrEmpty(selectedSoa.BeneficiaryCity))
                    ResidenceCityEntry.Text = selectedSoa.BeneficiaryCity;
                
                if (!string.IsNullOrEmpty(selectedSoa.BeneficiaryState))
                    ResidenceStateEntry.Text = selectedSoa.BeneficiaryState;
                
                if (!string.IsNullOrEmpty(selectedSoa.BeneficiaryZip))
                    ResidenceZipEntry.Text = selectedSoa.BeneficiaryZip;
                
                if (!string.IsNullOrEmpty(selectedSoa.SOANumber))
                    ScopeOfAppointmentEntry.Text = selectedSoa.SOANumber;
                
                // Show linked SOA confirmation
                LinkedSoaFrame.IsVisible = true;
                LinkedSoaLabel.Text = $"SOA Linked: {selectedSoa.SOANumber} ({selectedSoa.DisplayName})";
            }
        }

        private async void OnOpenLanguageAvailabilityNoticesClicked(object? sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new InstructionsViewerPage(
                "enUS-Enrollment-Request-Form-10.jpg",
                "enUS-Enrollment-Request-Form-11.jpg"));
        }

        private async void OnOpenCurrentStepImageClicked(object? sender, EventArgs e)
        {
            var images = _currentStep switch
            {
                2 => new[] { "enUS-Enrollment-Request-Form-2.jpg" },
                3 => new[] { "enUS-Enrollment-Request-Form-3.jpg" },
                4 => new[] { "enUS-Enrollment-Request-Form-4.jpg" },
                5 => new[] { "enUS-Enrollment-Request-Form-5.jpg" },
                6 => new[] { "enUS-Enrollment-Request-Form-6.jpg" },
                7 => new[] { "enUS-Enrollment-Request-Form-7.jpg" },
                8 => new[] { "enUS-Enrollment-Request-Form-8.jpg" },
                9 => new[]
                {
                    "enUS-Enrollment-Request-Form-9.jpg",
                    "enUS-Enrollment-Request-Form-10.jpg",
                    "enUS-Enrollment-Request-Form-11.jpg"
                },
                _ => new[] { "enUS-Enrollment-Request-Form-1.jpg" }
            };

            await Navigation.PushModalAsync(new InstructionsViewerPage(images));
        }

        private async void OnNextClicked(object? sender, EventArgs e)
        {
            if (_currentStep == 9)
            {
                _isSubmitting = true;

                var (submitted, message, enrollmentPath, enrollmentBytes) = await SubmitFlattenRequestAsync();
                if (!submitted)
                {
                    _isSubmitting = false;
                    await DisplayAlert("PDF Flatten", message, "OK");
                    return;
                }

                var enteredSoaNumber = ScopeOfAppointmentEntry.Text?.Trim();
                var soaNumber = !string.IsNullOrWhiteSpace(enteredSoaNumber)
                    ? enteredSoaNumber
                    : (_linkedSoaRecord?.SOANumber ?? string.Empty);

                _currentRecord = _linkedSoaRecord ?? _dbService.GetRecordBySoaNumber(soaNumber) ?? new EnrollmentRecord();

                _currentRecord.BeneficiaryFirstName = FirstNameEntry.Text ?? string.Empty;
                _currentRecord.BeneficiaryLastName = LastNameEntry.Text ?? string.Empty;
                _currentRecord.BeneficiaryMiddleInitial = MiddleInitialEntry.Text ?? string.Empty;
                _currentRecord.SOANumber = soaNumber;
                _currentRecord.LinkedSoaRecordId = _linkedSoaRecord?.Id;
                _currentRecord.EnrollmentFormPdfPath = enrollmentPath;
                _currentRecord.EnrollmentUploadStatus = EnrollmentUploadStatus.Pending;
                if (_currentRecord.CreatedDate == default)
                {
                    _currentRecord.CreatedDate = DateTime.Now;
                }

                // Upload enrollment form to DMS directly from in-memory bytes
                if (enrollmentBytes != null && enrollmentBytes.Length > 0)
                {
                    var (uploadSuccess, docId, uploadMsg) = await _dmsUploadService.UploadPdfAsync(DmsUploadService.DocumentTypeIdEnrollment, enrollmentBytes);
                    _currentRecord.EnrollmentUploadStatus = uploadSuccess ? EnrollmentUploadStatus.Uploaded : EnrollmentUploadStatus.Failed;
                    _currentRecord.EnrollmentFormDmsDocumentId = docId;
                    if (!uploadSuccess)
                        System.Diagnostics.Debug.WriteLine($"DMS enrollment upload failed: {uploadMsg}");
                }

                if (RequiresWorkingAgedSurvey())
                {
                    var (completed, surveyMessage) = await OpenWorkingAgedSurveyAsync();
                    if (!completed)
                    {
                        _isSubmitting = false;
                        await DisplayAlert("Working Aged Survey", surveyMessage, "OK");
                        return;
                    }
                }

                // Save record to database
                try
                {
                    _dbService.AddOrUpdateRecord(_currentRecord);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Database Error", $"Failed to save record: {ex.Message}", "OK");
                }

                _isSubmitting = false;
                await DisplayAlert("Triple-S AEP", "Form submitted successfully.", "OK");
                
                // Close the AEP wizard and return to main page
                await Navigation.PopAsync();
                return;
            }

            _currentStep++;
            UpdateStepUi();
        }

        private async void OnCloseClicked(object? sender, EventArgs e)
        {
            var result = await DisplayAlert("Close AEP Wizard?", 
                "Are you sure you want to close without saving?", 
                "Close", 
                "Cancel");
            
            if (result)
            {
                await Navigation.PopAsync();
            }
        }

        private void OnBackClicked(object? sender, EventArgs e)
        {
            if (_currentStep == 1)
            {
                return;
            }

            _currentStep--;
            UpdateStepUi();
        }

        private void UpdateStepUi()
        {
            Step1Layout.IsVisible = _currentStep == 1;
            Step2Layout.IsVisible = _currentStep == 2;
            Step3Layout.IsVisible = _currentStep == 3;
            Step4Layout.IsVisible = _currentStep == 4;
            Step5Layout.IsVisible = _currentStep == 5;
            Step6Layout.IsVisible = _currentStep == 6;
            Step7Layout.IsVisible = _currentStep == 7;
            Step8Layout.IsVisible = _currentStep == 8;
            Step9Layout.IsVisible = _currentStep == 9;

            OpenCurrentStepImageButton.IsVisible = _currentStep > 1;
            OpenCurrentStepImageButton.Text = _currentStep == 9
                ? "Open Step 9 Images"
                : $"Open Step {_currentStep} Image";

            BackButton.IsEnabled = _currentStep > 1;
            NextButton.Text = _currentStep == 9 ? "Submit" : "Next";

            StepHeaderLabel.Text = $"Step {_currentStep} of 9";

            StepTitleLabel.Text = _currentStep switch
            {
                1 => "Review Instructions",
                2 => "Enrollment Details",
                3 => "Residence, Medicare & Questions",
                4 => "Read & Sign",
                5 => "Optional Information",
                6 => "Additional Information",
                7 => "Payment Options",
                8 => "Supplemental Benefits & Documents",
                _ => "Helper Info & Privacy Statement"
            };

            // Scroll to top of form when step changes
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await FormScrollView.ScrollToAsync(0, 0, false);
            });
        }

        private async Task<(bool Submitted, string Message, string? FilePath, byte[]? PdfBytes)> SubmitFlattenRequestAsync()
        {
            var templateBase64 = await LoadTemplatePdfBase64Async();
            if (templateBase64 is null)
            {
                return (false, "Template PDF 'enUS-Enrollment-Request-Form-Fillable.pdf' was not found in Resources/Raw.", null, null);
            }

            var request = new PdfFlattenRequest
            {
                Base64TemplatePdf = templateBase64,
                Fields = BuildAcroFormFieldMap(),
                Images = BuildAcroFormImageMap()
            };

            var (isSuccess, pdfBytes, message) = await _pdfFlattenService.FlattenAsync(request);
            if (!isSuccess || pdfBytes is null || pdfBytes.Length == 0)
            {
                return (false, $"Flatten API request failed: {message}", null, null);
            }

            var (savedOk, saveMsg, filePath) = await SavePdfWhereUserChoosesAsync(pdfBytes);
            return (savedOk, saveMsg, filePath, savedOk ? pdfBytes : null);
        }

        private static async Task<(bool Submitted, string Message, string? FilePath)> SavePdfWhereUserChoosesAsync(byte[] pdfBytes)
        {
            try
            {
                var fileName = $"Enrollment-Request-{DateTime.Now:yyyyMMdd-HHmmss}.pdf";
                using var stream = new MemoryStream(pdfBytes);

                var result = await FileSaver.Default.SaveAsync(fileName, stream, CancellationToken.None);
                if (!result.IsSuccessful)
                {
                    return (false, result.Exception?.Message ?? "Save canceled.", null);
                }

                return (true, $"Saved: {result.FilePath}", result.FilePath);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        private async Task<string?> LoadTemplatePdfBase64Async()
        {
            try
            {
                await using var stream = await FileSystem.OpenAppPackageFileAsync("enUS-Enrollment-Request-Form-Fillable.pdf");
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                return Convert.ToBase64String(memoryStream.ToArray());
            }
            catch
            {
                return null;
            }
        }

        private Dictionary<string, object?> BuildAcroFormFieldMap()
        {
            var mailingDifferent =
                !string.IsNullOrWhiteSpace(MailingStreetEntry.Text) ||
                !string.IsNullOrWhiteSpace(MailingStreet2Entry.Text) ||
                !string.IsNullOrWhiteSpace(MailingCityEntry.Text) ||
                !string.IsNullOrWhiteSpace(MailingStateEntry.Text) ||
                !string.IsNullOrWhiteSpace(MailingZipEntry.Text);

            var currentHealthPlan =
                CurrentPlanMmm.IsChecked ? "MMM" :
                CurrentPlanHumana.IsChecked ? "Humana" :
                CurrentPlanMcs.IsChecked ? "MCS" :
                CurrentPlanMedicareOriginal.IsChecked ? "Medicare_Original" :
                CurrentPlanUsaPlan.IsChecked ? "USA_Plan" :
                !string.IsNullOrWhiteSpace(CurrentPlanOtherEntry.Text) ? "Other" : null;

            var selectedPlan =
                OptimoPlusPlan.IsChecked ? "Ãptimo Plus (PPO)" :
                BrillantePlan.IsChecked ? "Brillante (HMO-POS)" :
                EnlacePlusPlan.IsChecked ? "Enlace Plus (HMO)" :
                AhorroPlusPlan.IsChecked ? "Ahorro Plus (HMO)" :
                ContigoEnMentePlan.IsChecked ? "ContigoEnMente (HMO-SNP)" :
                ContigoPlusPlan.IsChecked ? "Contigo Plus (HMO-SNP)" :
                PlatinoPlusPlan.IsChecked ? "Platino Plus (HMO-SNP)" :
                PlatinoAdvancePlan.IsChecked ? "Platino Advance (HMO-SNP)" :
                PlatinoBlindaoPlan.IsChecked ? "Platino Blindao (HMO-SNP)" :
                PlatinoEnlacePlan.IsChecked ? "Platino Enlace (HMO-SNP)" : null;

            return new Dictionary<string, object?>
            {
                ["ScopeOfAppointment"] = ScopeOfAppointmentEntry.Text,
                ["GroupCoverage"] = GroupCoverageEntry.Text,
                ["GroupMonthlyPremium"] = GroupMonthlyPremiumEntry.Text,
                ["GroupEffectiveDate"] = GroupEffectiveDatePicker.Date.ToString("MM/dd/yyyy"),
                ["GroupSSN"] = GroupSsnEntry.Text,
                ["MailingAddressLine1"] = MailingStreetEntry.Text,
                ["MailingAddressLine2"] = MailingStreet2Entry.Text,
                ["MailingCity"] = MailingCityEntry.Text,
                ["MailingState"] = MailingStateEntry.Text,
                ["MailingZipCode"] = MailingZipEntry.Text,
                ["MedicareNumber"] = MedicareNumberEntry.Text,
                ["OtherRxCoverage"] = OtherCoverageYes.IsChecked ? "Yes" : OtherCoverageNo.IsChecked ? "No" : null,
                ["OtherCoverageName"] = OtherCoverageNameEntry.Text,
                ["OtherCoverageMemberNumber"] = OtherCoverageMemberNumberEntry.Text,
                ["OtherCoverageGroupNumber"] = OtherCoverageGroupNumberEntry.Text,
                ["MedicaidProgram"] = MedicaidYes.IsChecked ? "Yes" : MedicaidNo.IsChecked ? "No" : null,
                ["MedicaidNumber"] = MedicaidMpiEntry.Text,
                ["ContigoPlusCondition"] =
                    ConditionDiabetes.IsChecked ? "Value_eagr" :
                    ConditionCardio.IsChecked ? "Value_zzjw" :
                    ConditionHeartFailure.IsChecked ? "Value_tlqd" : null,
                ["ContigoEnMenteDementia"] = DementiaYes.IsChecked ? "Yes" : DementiaNo.IsChecked ? "No" : null,
                ["ApplicantSignatureDate"] = ApplicantSignatureDatePicker.Date.ToString("MM/dd/yyyy"),
                ["PhoneEnrollmentUCID"] = UcidCallNumberEntry.Text,
                ["PhoneEnrollmentUCIDDate"] = PhoneEnrollmentDatePicker.Date.ToString("MM/dd/yyyy"),
                ["AuthorizedRepSignatureDate"] = WitnessDatePicker.Date.ToString("MM/dd/yyyy"),
                ["AuthorizedRepName"] = RepresentativeNameEntry.Text,
                ["AuthorizedRepAddress"] = RepresentativeAddressEntry.Text,
                ["AuthorizedRepPhone"] = RepresentativePhoneEntry.Text,
                ["AuthorizedRepRelationship"] = RepresentativeRelationshipEntry.Text,
                ["InfoLanguage_Other"] = OtherLanguageEntry.Text,
                ["InfoAcccessibleFormat"] =
                    BrailleFormat.IsChecked ? "Braille" :
                    LargePrintFormat.IsChecked ? "Large_Print" :
                    AudioCdFormat.IsChecked ? "Audio_CD" :
                    DataCdFormat.IsChecked ? "Data_CD" : null,
                ["WorkStatus"] = WorkYes.IsChecked ? "Yes" : WorkNo.IsChecked ? "No" : null,
                ["SpouseWorkStatus"] = SpouseWorkYes.IsChecked ? "Yes" : SpouseWorkNo.IsChecked ? "No" : null,
                ["PCPName"] = PcpNameEntry.Text,
                ["PcpPhone"] = PcpPhoneEntry.Text,
                ["ContactTextMessage"] = TextYes.IsChecked ? "Yes" : TextNo.IsChecked ? "No" : null,
                ["ContactEmail"] = EmailContactYes.IsChecked ? "Yes" : EmailContactNo.IsChecked ? "No" : null,
                ["ContactTextPhoneNumber"] = TextNumberEntry.Text,
                ["ContactEmailAddress"] = EmailAddressEntry.Text,
                ["EmergencyContactName"] = EmergencyContactEntry.Text,
                ["EmergencyContactPhone"] = EmergencyPhoneEntry.Text,
                ["EmergencyContactRelationship"] = EmergencyRelationshipEntry.Text,
                ["RetireeIsSelf"] = RetireeYes.IsChecked ? "Yes" : RetireeNo.IsChecked ? "No" : null,
                ["RetirementDate"] = RetirementDatePicker.Date.ToString("MM/dd/yyyy"),
                ["RetireeName"] = RetireeNameEntry.Text,
                ["DependantsCovered"] = CoveringYes.IsChecked ? "Yes" : CoveringNo.IsChecked ? "No" : CoveringNotApplicable.IsChecked ? "Not_applicable" : null,
                ["SpouseName"] = SpouseNameEntry.Text,
                ["DependentsName1"] = DependentsNamesEntry.Text,
                ["DependentName2"] = null,
                ["DependentName3"] = null,
                ["DependentName4"] = null,
                ["LongtermcareResident"] = LongTermCareYes.IsChecked ? "Yes" : LongTermCareNo.IsChecked ? "No" : null,
                ["LongtermcareResidentInstitution"] = InstitutionNameEntry.Text,
                ["LongtermcareResidentAdministrator"] = AdministratorNameEntry.Text,
                ["LongtermcareResidentPhone"] = InstitutionPhoneEntry.Text,
                ["CurrentHealthPlan"] = currentHealthPlan,
                ["CurrentHealthPlanOther"] = CurrentPlanOtherEntry.Text,
                ["PaymentOption"] =
                    PaymentCouponBook.IsChecked ? "Get_a_coupon_book" :
                    PaymentEft.IsChecked ? "EFT" :
                    PaymentCreditCard.IsChecked ? "Credit_Card" :
                    PaymentAutoDeduction.IsChecked ? "SSorRRB" : null,
                ["EFT_AccountHolderName"] = EftAccountHolderEntry.Text,
                ["EFT_BankRoutingNumber"] = EftRoutingNumberEntry.Text,
                ["EFT_BankAccountNumber"] = EftAccountNumberEntry.Text,
                ["EFT_AccountType"] = EftChecking.IsChecked ? "Checking" : EftSavings.IsChecked ? "Savings" : null,
                ["CreditCardType"] = CreditCardVisa.IsChecked ? "Visa" : CreditCardMasterCard.IsChecked ? "Master_Card" : null,
                ["CreditCard_AccountHolderName"] = CreditCardHolderEntry.Text,
                ["CreditCard_Number"] = CreditCardNumberEntry.Text,
                ["CreditCard_ExpirationDate"] = CreditCardExpirationPicker.Date.ToString("MM/dd/yyyy"),
                ["MonthlyBenefitsFrom"] = BenefitsSocialSecurity.IsChecked ? "SS" : BenefitsRrb.IsChecked ? "RRB" : null,
                ["AuthorizedRepNPN"] = HelperNpnEntry.Text,
                ["OfficialUseReceiptDate"] = OfficialReceiptDatePicker.Date.ToString("MM/dd/yyyy"),
                ["OfficialUsePlanID"] = OfficialPlanIdEntry.Text,
                ["OfficialUseEffectiveDate"] = OfficialEffectiveDatePicker.Date.ToString("MM/dd/yyyy"),
                ["BeneficiaryFirstName"] = FirstNameEntry.Text,
                ["BeneficiaryLastName"] = LastNameEntry.Text,
                ["BeneficiaryMiddleInitial"] = MiddleInitialEntry.Text,
                ["BeneficiaryDOB"] = BirthDatePicker.Date.ToString("MM/dd/yyyy"),
                ["BeneficiarySex"] = FemaleOption.IsChecked ? "F" : MaleOption.IsChecked ? "M" : null,
                ["BeneficiaryPhone"] = HomePhoneEntry.Text,
                ["BeneficiaryAltPhone"] = AlternatePhoneEntry.Text,
                ["BeneficiaryPermanentAddress1"] = ResidenceStreetEntry.Text,
                ["BeneficiaryAddress2"] = ResidenceStreet2Entry.Text,
                ["BeneficiaryCity"] = ResidenceCityEntry.Text,
                ["BeneficiaryState"] = ResidenceStateEntry.Text,
                ["BeneficiaryZip"] = ResidenceZipEntry.Text,
                ["EnrollNowDate"] = EnrollNowDatePicker.Date.ToString("MM/dd/yyyy"),
                ["HomePhoneIsCell"] = HomeCellularCheckBox.IsChecked,
                ["AlternatePhoneIsCell"] = AlternateCellularCheckBox.IsChecked,
                ["BeneficiaryMailingAddressDifferent"] = mailingDifferent,
                ["AckPartAandB"] = true,
                ["AckShareInfoMedicare"] = true,
                ["AckSingleMAPlan"] = true,
                ["AckTripleSServicesandBenifits"] = true,
                ["AckInfoCorrect"] = true,
                ["AckSignature"] = true,
                ["EnrollNowChecked"] = !string.IsNullOrWhiteSpace(EnrollNowSignatureEntry.Text),
                ["InfoLanguage_Spanish"] = SpanishLanguage.IsChecked,
                ["InfoViaEmail_ProviderDirectory"] = EmailProviderDirectory.IsChecked,
                ["InfoViaEmail_AnnualNoticeofChange"] = EmailAnnualNotice.IsChecked,
                ["InfoViaEmail_EvidenceofCoverage"] = EmailEvidenceOfCoverage.IsChecked,
                ["InfoViaEmail_SummaryofBenifits"] = EmailSummaryOfBenefits.IsChecked,
                ["InfoViaEmail_PrescriptionDrugFormulary"] = EmailDrugFormulary.IsChecked,
                ["InfoViaEmail_PromotionalMaterial"] = EmailPromotionalMaterials.IsChecked,
                ["InfoViaEmail_ElectronicEnrollmentConfirm"] = EmailEnrollmentConfirmation.IsChecked,
                ["DocsInitialPackage"] = DocInitialPackage.IsChecked,
                ["DocsMedicareStarRating"] = DocMedicareStarRatings.IsChecked,
                ["DocsNoticeofWeb"] = DocWebAvailability.IsChecked,
                ["DocsEnrollmentConfirmation"] = DocEnrollmentConfirmation.IsChecked,
                ["DocsEnrollmentFormCopy"] = DocEnrollmentFormCopy.IsChecked,
                ["DocsAttestationEligibility"] = DocAttestationEligibility.IsChecked,
                ["DocsPrecertificationofChronic"] = DocPrecertificationChronic.IsChecked,
                ["DocsAuthorizationToSharePHI"] = DocAuthorizationDisclose.IsChecked,
                ["CheckEvidenceDME"] = NoticeEvidenceOfCoverage.IsChecked,
                ["CheckProviderandPharmacy"] = NoticeProviderDirectory.IsChecked,
                ["CheckDrugFormulary"] = NoticeDrugFormulary.IsChecked,
                ["SelectedPlan_Radio"] = selectedPlan,
                ["GroupPlanType"] = GroupHmoOption.IsChecked ? "HMO" : GroupPpoOption.IsChecked ? "PPO" : null
            };
        }

        private Dictionary<string, string> BuildAcroFormImageMap()
        {
            var images = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(_applicantSignatureDataUrl))
            {
                images["ApplicantSignature"] = ExtractBase64(_applicantSignatureDataUrl);
            }

            if (!string.IsNullOrWhiteSpace(_witnessSignatureDataUrl))
            {
                images["WitnessSignature"] = ExtractBase64(_witnessSignatureDataUrl);
            }

            if (!string.IsNullOrWhiteSpace(_helperSignatureDataUrl))
            {
                images["AgentSignature"] = ExtractBase64(_helperSignatureDataUrl);
            }

            return images;
        }

        private async void OnCaptureApplicantSignatureClicked(object? sender, EventArgs e)
        {
            _applicantSignatureDataUrl = await CaptureSignatureAsync("Capture Beneficiary Signature");
            UpdateSignaturePreview(_applicantSignatureDataUrl, ApplicantSignaturePreview, ClearApplicantSignatureButton);
        }

        private async void OnCaptureWitnessSignatureClicked(object? sender, EventArgs e)
        {
            _witnessSignatureDataUrl = await CaptureSignatureAsync("Capture Witness Signature");
            UpdateSignaturePreview(_witnessSignatureDataUrl, WitnessSignaturePreview, ClearWitnessSignatureButton);
        }

        private async void OnCaptureHelperSignatureClicked(object? sender, EventArgs e)
        {
            _helperSignatureDataUrl = await CaptureSignatureAsync("Capture Agent Signature");
            UpdateSignaturePreview(_helperSignatureDataUrl, HelperSignaturePreview, ClearHelperSignatureButton);
        }

        private void OnClearApplicantSignatureClicked(object? sender, EventArgs e)
        {
            _applicantSignatureDataUrl = null;
            UpdateSignaturePreview(null, ApplicantSignaturePreview, ClearApplicantSignatureButton);
        }

        private void OnClearWitnessSignatureClicked(object? sender, EventArgs e)
        {
            _witnessSignatureDataUrl = null;
            UpdateSignaturePreview(null, WitnessSignaturePreview, ClearWitnessSignatureButton);
        }

        private void OnClearHelperSignatureClicked(object? sender, EventArgs e)
        {
            _helperSignatureDataUrl = null;
            UpdateSignaturePreview(null, HelperSignaturePreview, ClearHelperSignatureButton);
        }

        private async Task<string?> CaptureSignatureAsync(string title)
        {
            var page = new SignatureCapturePage(title);
            await Navigation.PushModalAsync(page);
            return await page.Result;
        }

        private static void UpdateSignaturePreview(string? dataUrl, Image preview, Button clearButton)
        {
            if (string.IsNullOrWhiteSpace(dataUrl))
            {
                preview.Source = null;
                preview.IsVisible = false;
                clearButton.IsVisible = false;
                return;
            }

            preview.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(ExtractBase64(dataUrl))));
            preview.IsVisible = true;
            clearButton.IsVisible = true;
        }

        private static string ExtractBase64(string dataUrl)
        {
            const string marker = "base64,";
            var index = dataUrl.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            return index >= 0 ? dataUrl[(index + marker.Length)..] : dataUrl;
        }

        private bool RequiresWorkingAgedSurvey()
        {
            return WorkYes.IsChecked || SpouseWorkYes.IsChecked;
        }

        private async Task<(bool Completed, string Message)> OpenWorkingAgedSurveyAsync()
        {
            var page = new WorkingAgedSurveyPage(
                _currentRecord!,
                MedicareNumberEntry.Text,
                FirstNameEntry.Text,
                MiddleInitialEntry.Text,
                LastNameEntry.Text,
                WorkYes.IsChecked,
                SpouseWorkYes.IsChecked);

            await Navigation.PushModalAsync(page);
            return await page.Result;
        }
    }
}
