using System;
using Microsoft.Maui.Controls;
using Triple_S_Maui_AEP.ViewModels;

namespace Triple_S_Maui_AEP.Views
{
    public partial class SOAWizardPage : ContentPage
    {
        private readonly SOAWizardViewModel _viewModel = new SOAWizardViewModel();

        public SOAWizardPage()
        {
            try
            {
                InitializeComponent();
                BindingContext = _viewModel;

                InitializeLanguagePicker();

                SetLocalizedText();
                Services.LanguageService.Instance.LanguageChanged += _ => SetLocalizedText();

                ClearAllSignatures();
                UpdateStepUI();
            }
            catch (Exception ex)
            {
                if (this.FindByName<Label>("ErrorMessageLabel") is Label errorLabel)
                {
                    errorLabel.Text = $"Error: {ex.Message}";
                    errorLabel.IsVisible = true;
                }
            }
        }

        private void InitializeLanguagePicker()
        {
            LanguagePicker.Items.Clear();
            LanguagePicker.Items.Add("English");
            LanguagePicker.Items.Add("Español");
            
            // Set current language
            var currentLang = Services.LanguageService.Instance.CurrentLanguage;
            LanguagePicker.SelectedIndex = currentLang == Models.Language.English ? 0 : 1;
            
            // Subscribe to language changes
            LanguagePicker.SelectedIndexChanged += (s, e) =>
            {
                var newLang = LanguagePicker.SelectedIndex == 1 ? Models.Language.Spanish : Models.Language.English;
                Services.LanguageService.Instance.CurrentLanguage = newLang;
            };
        }

        private void SetLocalizedText()
        {
            var isEnglish = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English;

            LanguageLabel.Text = isEnglish ? "Language:" : "Idioma:";
            PageTitle.Text = isEnglish ? "SOA Wizard" : "Asistente de SOA";
            StepLabel.Text = isEnglish ? "Step" : "Paso";
            SOANumberLabel.Text = isEnglish ? "SOA #" : "SOA #";

            Step1TitleLabel.Text = isEnglish ? "Beneficiary Information" : "Información del Beneficiario";
            EmployeeFirstNameLabel.Text = isEnglish ? "Beneficiary First Name" : "Nombre del Beneficiario";
            EmployeeLastNameLabel.Text = isEnglish ? "Beneficiary Last Name" : "Apellido del Beneficiario";
            DOBLabel.Text = isEnglish ? "Date of Birth" : "Fecha de Nacimiento";
            MedicareNumberLabel.Text = isEnglish ? "Medicare Number" : "Número de Medicare";
            PhoneNumberLabel.Text = isEnglish ? "Phone Number" : "Número de Teléfono";

            Step2TitleLabel.Text = isEnglish ? "Coverage Scope" : "Alcance de Cobertura";
            ProductInformationProvidedLabel.Text = isEnglish ? "Product information provided" : "Información del producto provista";
            MedicareAdvantageLabel.Text = isEnglish ? "Medicare Advantage" : "Medicare Advantage";
            PartDLabel.Text = isEnglish ? "Part D" : "Parte D";
            SupplementalLabel.Text = isEnglish ? "Supplemental" : "Suplementario";
            DentalVisionLabel.Text = isEnglish ? "Dental/Vision" : "Dental/Visión";
            HearingAidLabel.Text = isEnglish ? "Hearing Aid" : "Audífonos";
            WellnessLabel.Text = isEnglish ? "Wellness" : "Bienestar";

            Step3TitleLabel.Text = isEnglish ? "Meeting Details" : "Detalles de la Reunión";
            MeetingDateLabel.Text = isEnglish ? "Meeting Date" : "Fecha de la Reunión";
            MeetingLocationLabel.Text = isEnglish ? "Meeting Location" : "Lugar de la Reunión";
            ComplianceDocumentsLabel.Text = isEnglish ? "Compliance documents provided" : "Documentos de cumplimiento provistos";

            Step4TitleLabel.Text = isEnglish ? "Signatures" : "Firmas";
            EmployeeSignatureLabel.Text = isEnglish ? "Beneficiary Signature" : "Firma del Beneficiario";
            BeneficiarySignatureHintLabel.Text = isEnglish ? "Tap button to capture signature" : "Toque el botón para capturar la firma";
            UseXMarkLabel.Text = isEnglish ? "Use X Mark" : "Usar marca X";
            AgentSignatureLabel.Text = isEnglish ? "Agent Signature" : "Firma del Agente";
            AgentSignatureHintLabel.Text = isEnglish ? "Tap button to capture signature" : "Toque el botón para capturar la firma";

            PreviousButton.Text = isEnglish ? "Previous" : "Anterior";
            NextButton.Text = isEnglish ? "Next" : "Siguiente";
            SubmitButton.Text = isEnglish ? "Submit" : "Enviar";
            CancelButton.Text = isEnglish ? "Cancel" : "Cancelar";
        }

        private async void OnPreviousClicked(object sender, EventArgs e)
        {
            bool success = await _viewModel.GoToPreviousStepAsync();
            if (!success)
            {
                await DisplayAlert(
                    Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "Info" : " información",
                    Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "Already on first step" : "Ya estás en el primer paso",
                    "OK"
                );
            }
            else
            {
                UpdateStepUI();
            }
        }

        private async void OnNextClicked(object sender, EventArgs e)
        {
            bool success = await _viewModel.SaveCurrentStepAsync();
            if (!success)
            {
                ErrorMessageLabel.Text = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English
                    ? _viewModel.ErrorMessage
                    : "Por favor, complete todos los campos requeridos";
                ErrorMessageLabel.IsVisible = true;
                return;
            }

            ErrorMessageLabel.IsVisible = false;
            UpdateStepUI();
        }

        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            var isEnglish = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English;

            var (captured, captureError) = await CaptureSignaturesAsync();
            if (!captured)
            {
                ErrorMessageLabel.Text = captureError;
                ErrorMessageLabel.IsVisible = true;
                return;
            }

            bool success = await _viewModel.SubmitSOAAsync();
            if (success)
            {
                // Clear form immediately after PDF is generated
                _viewModel.ResetForm();

                await DisplayAlert(
                    isEnglish ? "Success" : "Éxito",
                    isEnglish ? "SOA submitted successfully!" : "¡SOA enviado exitosamente!",
                    "OK"
                );

                await Shell.Current.GoToAsync("///DashboardPage");
            }
            else
            {
                ErrorMessageLabel.Text = isEnglish ? "Failed to submit SOA" : "Error al enviar SOA";
                ErrorMessageLabel.IsVisible = true;
            }
        }

        private async Task<(bool Success, string ErrorMessage)> CaptureSignaturesAsync()
        {
            var isEnglish = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English;

            if (_viewModel.CurrentSOA is null)
            {
                return (false, isEnglish ? "SOA data is not initialized" : "Los datos de SOA no están inicializados");
            }

            try
            {
                // Check if beneficiary signature is captured (via modal or X mark)
                if (_viewModel.CurrentSOA.UsesXMark)
                {
                    _viewModel.BeneficiarySignatureBase64 = "XMARK";
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(_viewModel.BeneficiarySignatureBase64))
                    {
                        return (false, isEnglish ? "Please capture beneficiary signature or use X mark" : "Capture la firma del beneficiario o use marca X");
                    }
                }

                // Check if agent signature is captured (via modal)
                if (string.IsNullOrWhiteSpace(_viewModel.AgentSignatureBase64))
                {
                    return (false, isEnglish ? "Please capture agent signature" : "Capture la firma del agente");
                }

                // Store signatures in SOA record
                _viewModel.CurrentSOA.EmployeeSignatureBase64 = _viewModel.BeneficiarySignatureBase64 ?? string.Empty;
                _viewModel.CurrentSOA.AgentSignatureBase64 = _viewModel.AgentSignatureBase64 ?? string.Empty;
                _viewModel.CurrentSOA.EmployeeSignatureMethod = _viewModel.CurrentSOA.UsesXMark ? "XMark" : "Touch";
                _viewModel.CurrentSOA.EmployeeSignatureTimestamp = DateTime.UtcNow.ToString("o");
                _viewModel.CurrentSOA.AgentSignatureTimestamp = DateTime.UtcNow.ToString("o");

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, isEnglish ? $"Error capturing signatures: {ex.Message}" : $"Error al capturar firmas: {ex.Message}");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert(
                Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "Confirm" : "Confirmar",
                Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "Discard changes and go back?" : "¿Descartar cambios y volver?",
                Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "Yes" : "Sí",
                Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "No" : "No"
            );

            if (confirm)
            {
                await Shell.Current.GoToAsync("///DashboardPage");
            }
        }

        /// <summary>
        /// Opens modal to capture beneficiary signature
        /// </summary>
        private async void OnCaptureBeneficiarySignatureClicked(object sender, EventArgs e)
        {
            try
            {
                var isEnglish = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English;
                
                var modalPage = new SignatureModalPage(
                    title: isEnglish ? "Beneficiary Signature" : "Firma del Beneficiario",
                    instruction: isEnglish ? "Beneficiary must sign below" : "El beneficiario debe firmar abajo"
                );

                await Navigation.PushModalAsync(modalPage);
                var signatureBase64 = await modalPage.GetSignatureAsync();

                if (!string.IsNullOrEmpty(signatureBase64))
                {
                    _viewModel.BeneficiarySignatureBase64 = signatureBase64;
                    
                    // Update UI
                    BeneficiarySignatureStatus.Text = isEnglish ? "✅ Captured" : "✅ Capturada";
                    BeneficiarySignatureStatus.TextColor = Colors.Green;
                    
                    // Show preview
                    BeneficiarySignaturePreview.Source = ImageSource.FromStream(
                        () => new MemoryStream(Convert.FromBase64String(signatureBase64))
                    );
                    BeneficiarySignaturePreviewBorder.IsVisible = true;

                    await DisplayAlert(
                        isEnglish ? "Success" : "Éxito",
                        isEnglish ? "Beneficiary signature captured!" : "¡Firma del beneficiario capturada!",
                        "OK"
                    );
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(
                    Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "Error" : "Error",
                    $"Failed to capture signature: {ex.Message}",
                    "OK"
                );
            }
        }

        /// <summary>
        /// Opens modal to capture agent signature
        /// </summary>
        private async void OnCaptureAgentSignatureClicked(object sender, EventArgs e)
        {
            try
            {
                var isEnglish = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English;
                
                var modalPage = new SignatureModalPage(
                    title: isEnglish ? "Agent Signature" : "Firma del Agente",
                    instruction: isEnglish ? "Agent must sign below" : "El agente debe firmar abajo"
                );

                await Navigation.PushModalAsync(modalPage);
                var signatureBase64 = await modalPage.GetSignatureAsync();

                if (!string.IsNullOrEmpty(signatureBase64))
                {
                    _viewModel.AgentSignatureBase64 = signatureBase64;
                    
                    // Update UI
                    AgentSignatureStatus.Text = isEnglish ? "✅ Captured" : "✅ Capturada";
                    AgentSignatureStatus.TextColor = Colors.Green;
                    
                    // Show preview
                    AgentSignaturePreview.Source = ImageSource.FromStream(
                        () => new MemoryStream(Convert.FromBase64String(signatureBase64))
                    );
                    AgentSignaturePreviewBorder.IsVisible = true;

                    await DisplayAlert(
                        isEnglish ? "Success" : "Éxito",
                        isEnglish ? "Agent signature captured!" : "¡Firma del agente capturada!",
                        "OK"
                    );
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(
                    Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "Error" : "Error",
                    $"Failed to capture signature: {ex.Message}",
                    "OK"
                );
            }
        }

        private void ClearAllSignatures()
        {
            // Reset signature status
            BeneficiarySignatureStatus.Text = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "❌ Not Captured" : "❌ No Capturada";
            BeneficiarySignatureStatus.TextColor = Color.FromArgb("#C62828");
            BeneficiarySignaturePreviewBorder.IsVisible = false;
            
            AgentSignatureStatus.Text = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "❌ Not Captured" : "❌ No Capturada";
            AgentSignatureStatus.TextColor = Color.FromArgb("#C62828");
            AgentSignaturePreviewBorder.IsVisible = false;
            
            _viewModel.BeneficiarySignatureBase64 = null;
            _viewModel.AgentSignatureBase64 = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateStepUI();
        }

        private void OnEmployeeXMarkToggled(object sender, CheckedChangedEventArgs e)
        {
            if (_viewModel?.CurrentSOA != null)
            {
                _viewModel.CurrentSOA.UsesXMark = e.Value;
                // When X mark is toggled, signature is not required (no inline pad to disable)
            }
        }

        private void UpdateStepUI()
        {
            bool isLastStep = _viewModel.CurrentStep == _viewModel.TotalSteps;

            Step1Layout.IsVisible = _viewModel.CurrentStep == 1;
            Step2Layout.IsVisible = _viewModel.CurrentStep == 2;
            Step3Layout.IsVisible = _viewModel.CurrentStep == 3;
            Step4Layout.IsVisible = _viewModel.CurrentStep == 4;

            PreviousButton.IsVisible = _viewModel.CurrentStep > 1;
            NextButton.IsVisible = !isLastStep;
            SubmitButton.IsVisible = isLastStep;

            ErrorMessageLabel.IsVisible = false;
        }
    }
}
