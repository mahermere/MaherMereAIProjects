using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Maui.Controls;
using Triple_S_Maui_AEP.Controls;
using Triple_S_Maui_AEP.ViewModels;

namespace Triple_S_Maui_AEP.Views
{
    public partial class EnrollmentWizardPage : ContentPage
    {
        private readonly EnrollmentWizardViewModel _viewModel = new EnrollmentWizardViewModel();
        private int currentStep = 1;
        private readonly List<VerticalStackLayout> stepPanels = new();
        private readonly List<DependentEntry> dependents = new();
        private readonly string soaFirstPageCsvPath = Path.Combine(FileSystem.Current.AppDataDirectory, "soa_firstpage_records.csv");
        private string _enrollmentNumber = string.Empty;

        // Triple-S required plans
        private static readonly List<(string English, string Spanish)> TripleSPlans = new()
        {
            ("Óptimo Plus (PPO)", "Óptimo Plus (PPO)"),
            ("Brillante (HMO-POS)", "Brillante (HMO-POS)"),
            ("Enlace Plus (HMO)", "Enlace Plus (HMO)"),
            ("Ahorro Plus (HMO)", "Ahorro Plus (HMO)"),
            ("ContigoEnMente (HMO-SNP)", "ContigoEnMente (HMO-SNP)"),
            ("Contigo Plus (HMO-SNP)", "Contigo Plus (HMO-SNP)"),
            ("Platino Plus (HMO-SNP)", "Platino Plus (HMO-SNP)"),
            ("Platino Advance (HMO-SNP)", "Platino Advance (HMO-SNP)"),
            ("Platino Blindao (HMO-SNP)", "Platino Blindao (HMO-SNP)"),
            ("Platino Enlace (HMO-SNP)", "Platino Enlace (HMO-SNP)")
        };

        private static readonly List<(string English, string Spanish)> GenderOptions = new()
        {
            ("Male", "Masculino"),
            ("Female", "Femenino"),
            ("Non-Binary", "No binario"),
            ("Prefer Not to Answer", "Prefiero no responder")
        };

        public EnrollmentWizardPage()
        {
            try
            {
                InitializeComponent();
                BindingContext = _viewModel;

                stepPanels.AddRange(new[]
                {
                    Step1Panel,
                    Step2Panel,
                    Step3Panel,
                    Step4Panel,
                    Step5Panel,
                    Step6Panel,
                    Step7Panel,
                    Step8Panel,
                    Step9Panel
                });

                // Generate enrollment number
                _enrollmentNumber = new Services.SOANumberService().GenerateEnrollmentNumber();

                // Initialize all pickers and combos
                InitializeLanguagePicker();
                InitializeGenderCombo();
                InitializeContactMethodPicker();
                InitializeEmergencyRelationshipCombo();
                InitializePlanNamePicker();
                InitializePremiumPaymentMethodPicker();
                InitializeSNPTypePicker();
                InitializeSEPReasonPicker();
                InitializeGoodCauseStatusPicker();

                // Set up event handlers
                BackButton.Clicked += BackButton_Click;
                NextButton.Clicked += NextButton_Click;
                CancelButton.Clicked += CancelButton_Click;
                AddDependentButton.Clicked += AddDependentButton_Click;
                // Signature capture is now done via modal buttons (no clear buttons needed)
                DifferentMailingCheckbox.CheckedChanged += (s, e) => {
                    MailingAddressPanel.IsVisible = DifferentMailingCheckbox.IsChecked;
                };
                XMarkCheckbox.CheckedChanged += XMarkCheckbox_CheckedChanged;
                SOANumberPicker.SelectedIndexChanged += SOANumberPicker_SelectedIndexChanged;

                // Set all UI text from language resources
                SetLocalizedText();
                Services.LanguageService.Instance.LanguageChanged += _ => SetLocalizedText();
                
                // Preload page 1 fields from CSV if available
                PreloadPage1FieldsFromCsv();

                // Refresh SOA dropdown
                _ = RefreshSOADropdownAsync();

                // Show step 1
                SetStep(1);
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
            LanguagePicker.SelectedIndex = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? 0 : 1;
            LanguagePicker.SelectedIndexChanged += (s, e) => {
                var newLang = LanguagePicker.SelectedIndex == 1 ? Models.Language.Spanish : Models.Language.English;
                Services.LanguageService.Instance.CurrentLanguage = newLang;
            };
        }

        private void InitializeGenderCombo()
        {
            GenderCombo.Items.Clear();
            var isEnglish = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English;
            foreach (var gender in GenderOptions)
            {
                GenderCombo.Items.Add(isEnglish ? gender.English : gender.Spanish);
            }
        }

        private void InitializeContactMethodPicker()
        {
            ContactMethodPicker.Items.Clear();
            ContactMethodPicker.Items.Add("Phone");
            ContactMethodPicker.Items.Add("Email");
            ContactMethodPicker.Items.Add("Text Message");
            ContactMethodPicker.Items.Add("Mail");
            ContactMethodPicker.SelectedIndex = 0;
        }

        private void InitializeEmergencyRelationshipCombo()
        {
            EmergencyRelationshipCombo.Items.Clear();
            EmergencyRelationshipCombo.Items.Add("Spouse");
            EmergencyRelationshipCombo.Items.Add("Child");
            EmergencyRelationshipCombo.Items.Add("Parent");
            EmergencyRelationshipCombo.Items.Add("Sibling");
            EmergencyRelationshipCombo.Items.Add("Friend");
            EmergencyRelationshipCombo.Items.Add("Other");
        }

        private void InitializePlanNamePicker()
        {
            PlanNamePicker.Items.Clear();
            var isEnglish = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English;
            foreach (var plan in TripleSPlans)
            {
                PlanNamePicker.Items.Add(isEnglish ? plan.English : plan.Spanish);
            }
        }

        private void InitializePremiumPaymentMethodPicker()
        {
            PremiumPaymentMethodPicker.Items.Clear();
            PremiumPaymentMethodPicker.Items.Add("Bank Account");
            PremiumPaymentMethodPicker.Items.Add("Credit Card");
            PremiumPaymentMethodPicker.Items.Add("Check");
            PremiumPaymentMethodPicker.Items.Add("Social Security Deduction");
            PremiumPaymentMethodPicker.Items.Add("Other");
            PremiumPaymentMethodPicker.SelectedIndex = 0;
        }

        private void InitializeSNPTypePicker()
        {
            SNPTypePicker.Items.Clear();
            SNPTypePicker.Items.Add("C-SNP (Chronic Condition)");
            SNPTypePicker.Items.Add("D-SNP (Dual Eligible)");
            SNPTypePicker.Items.Add("I-SNP (Institutional)");
        }

        private void InitializeSEPReasonPicker()
        {
            SEPReasonPicker.Items.Clear();
            SEPReasonPicker.Items.Add("Moved out of service area");
            SEPReasonPicker.Items.Add("Loss of other coverage");
            SEPReasonPicker.Items.Add("Gained Medicaid");
            SEPReasonPicker.Items.Add("Released from incarceration");
            SEPReasonPicker.Items.Add("Other");
        }

        private void InitializeGoodCauseStatusPicker()
        {
            GoodCauseStatusPicker.Items.Clear();
            GoodCauseStatusPicker.Items.Add("Approved");
            GoodCauseStatusPicker.Items.Add("Pending");
            GoodCauseStatusPicker.Items.Add("Denied");
            GoodCauseStatusPicker.Items.Add("N/A");
        }

        private void PreloadPage1FieldsFromCsv()
        {
            try
            {
                if (!File.Exists(soaFirstPageCsvPath))
                {
                    return;
                }

                var lines = File.ReadAllLines(soaFirstPageCsvPath);
                if (lines.Length <= 1)
                {
                    return;
                }

                var lastLine = lines[lines.Length - 1];
                var fields = Utilities.CsvDataUtility.ParseCsvLine(lastLine);
                var rec = Models.SOAFirstPageRecord.FromCsv(fields);

                FirstNameBox.Text = rec.FirstName;
                LastNameBox.Text = rec.LastName;
                DOBPicker.Date = rec.DateOfBirth != DateTime.MinValue ? rec.DateOfBirth : DateTime.Today;
                if (!string.IsNullOrEmpty(rec.Gender))
                {
                    for (int i = 0; i < GenderCombo.Items.Count; i++)
                    {
                        if (GenderCombo.Items[i].Equals(rec.Gender, StringComparison.OrdinalIgnoreCase))
                        {
                            GenderCombo.SelectedIndex = i;
                            break;
                        }
                    }
                }
                PrimaryPhoneBox.Text = rec.PrimaryPhone;
                MedicareBox.Text = rec.MedicareNumber;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error preloading fields: {ex.Message}");
            }
        }

        public async Task RefreshSOADropdownAsync()
        {
            SOANumberPicker.ItemsSource = null;
            var soaRecords = await Services.SOAService.GetActiveSOARecordsAsync();
            SOANumberPicker.ItemsSource = soaRecords
                .Select(r => $"{r.SOANumber} - {r.BeneficiaryName}")
                .ToList();
        }

        private async void SOANumberPicker_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (SOANumberPicker.SelectedIndex >= 0)
            {
                var selectedSOA = SOANumberPicker.SelectedItem as string;
                await PopulateFromSOAAsync(selectedSOA);
            }
        }

        private async Task PopulateFromSOAAsync(string? selectedSOA)
        {
            if (string.IsNullOrWhiteSpace(selectedSOA))
                return;

            try
            {
                var soaNumber = selectedSOA.Split(" - ")[0].Trim();
                var activeRecords = await Services.SOAService.GetActiveSOARecordsAsync();
                var soaRecord = activeRecords.FirstOrDefault(r => r.SOANumber == soaNumber);

                if (soaRecord == null)
                {
                    System.Diagnostics.Debug.WriteLine($"SOA record not found: {soaNumber}");
                    return;
                }

                FirstNameBox.Text = soaRecord.FirstName;
                LastNameBox.Text = soaRecord.LastName;

                if (!File.Exists(soaFirstPageCsvPath))
                {
                    return;
                }

                var lines = File.ReadAllLines(soaFirstPageCsvPath);
                foreach (var line in lines.Skip(1))
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    var fields = Utilities.CsvDataUtility.ParseCsvLine(line);
                    var rec = Models.SOAFirstPageRecord.FromCsv(fields);

                    if (rec.FirstName.Equals(soaRecord.FirstName, StringComparison.OrdinalIgnoreCase) &&
                        rec.LastName.Equals(soaRecord.LastName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (rec.DateOfBirth != DateTime.MinValue)
                        {
                            DOBPicker.Date = rec.DateOfBirth;
                        }

                        if (!string.IsNullOrWhiteSpace(rec.Gender))
                        {
                            for (int i = 0; i < GenderCombo.Items.Count; i++)
                            {
                                if (GenderCombo.Items[i].Equals(rec.Gender, StringComparison.OrdinalIgnoreCase))
                                {
                                    GenderCombo.SelectedIndex = i;
                                    break;
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(rec.PrimaryPhone))
                        {
                            PrimaryPhoneBox.Text = rec.PrimaryPhone;
                        }

                        if (!string.IsNullOrWhiteSpace(rec.MedicareNumber))
                        {
                            MedicareBox.Text = rec.MedicareNumber;
                        }

                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            var lang = LanguagePicker.SelectedIndex == 1 ? "es-PR" : "en";
                            await DisplayAlert(
                                lang == "es-PR" ? "Datos cargados" : "Data Loaded",
                                lang == "es-PR"
                                    ? $"Los datos del beneficiario de SOA {soaNumber} se han cargado en el formulario."
                                    : $"Beneficiary data from SOA {soaNumber} has been loaded into the form.",
                                "OK");
                        });

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error populating from SOA: {ex.Message}");
            }
        }

        private void SetLocalizedText()
        {
            var isEnglish = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English;

            if (PageTitle != null) PageTitle.Text = isEnglish ? "Enrollment Wizard" : "Asistente de Inscripción";
            if (StepLabel != null) StepLabel.Text = isEnglish ? "Step" : "Paso";
            if (LanguageLabel != null) LanguageLabel.Text = isEnglish ? "Language:" : "Idioma:";
            if (SOASelectionLabel != null) SOASelectionLabel.Text = isEnglish ? "Scope of Appointment Number (SOA)" : "Número de Alcance de Cita (SOA)";
            
            // Step 1
            if (PersonalInfoLabel != null) PersonalInfoLabel.Text = isEnglish ? "Personal Information" : "Información Personal";
            if (FirstNameLabel != null) FirstNameLabel.Text = isEnglish ? "First Name *" : "Nombre *";
            if (MiddleInitialLabel != null) MiddleInitialLabel.Text = isEnglish ? "Middle Initial" : "Inicial del Segundo Nombre";
            if (LastNameLabel != null) LastNameLabel.Text = isEnglish ? "Last Name *" : "Apellido *";
            if (DOBLabel != null) DOBLabel.Text = isEnglish ? "Date of Birth (MM/DD/YYYY) *" : "Fecha de Nacimiento (MM/DD/AAAA) *";
            if (GenderLabel != null) GenderLabel.Text = isEnglish ? "Sex/Gender *" : "Sexo/Género *";
            if (PrimaryPhoneLabel != null) PrimaryPhoneLabel.Text = isEnglish ? "Primary Phone Number *" : "Número de Teléfono Principal *";
            if (PrimaryPhoneIsMobileLabel != null) PrimaryPhoneIsMobileLabel.Text = isEnglish ? "Is Mobile" : "Es Móvil";
            if (SecondaryPhoneLabel != null) SecondaryPhoneLabel.Text = isEnglish ? "Secondary Phone" : "Teléfono Secundario";
            if (SecondaryPhoneIsMobileLabel != null) SecondaryPhoneIsMobileLabel.Text = isEnglish ? "Is Mobile" : "Es Móvil";
            if (EmailLabel != null) EmailLabel.Text = isEnglish ? "Email Address" : "Dirección de Correo Electrónico";
            if (MedicareLabel != null) MedicareLabel.Text = isEnglish ? "Medicare Number *" : "Número de Medicare *";
            if (SSNLabel != null) SSNLabel.Text = isEnglish ? "Social Security Number" : "Número de Seguro Social";
            if (ContactMethodLabel != null) ContactMethodLabel.Text = isEnglish ? "Preferred Contact Method" : "Método de Contacto Preferido";
            
            // Step 2
            if (PermanentAddressLabel != null) PermanentAddressLabel.Text = isEnglish ? "Permanent Residence Address" : "Dirección de Residencia Permanente";
            if (Address1Label != null) Address1Label.Text = isEnglish ? "Street Address Line 1 *" : "Línea de Dirección 1 *";
            if (Address2Label != null) Address2Label.Text = isEnglish ? "Street Address Line 2" : "Línea de Dirección 2";
            if (CityLabel != null) CityLabel.Text = isEnglish ? "City *" : "Ciudad *";
            if (StateLabel != null) StateLabel.Text = isEnglish ? "State *" : "Estado *";
            if (CountyLabel != null) CountyLabel.Text = isEnglish ? "County" : "Condado";
            if (ZIPLabel != null) ZIPLabel.Text = isEnglish ? "ZIP Code *" : "Código Postal *";
            if (DifferentMailingLabel != null) DifferentMailingLabel.Text = isEnglish ? "Mailing address is different" : "La dirección de envío es diferente";
            
            // Step 3
            if (EmergencyContactLabel != null) EmergencyContactLabel.Text = isEnglish ? "Emergency Contact Information" : "Información de Contacto de Emergencia";
            if (EmergencyNameLabel != null) EmergencyNameLabel.Text = isEnglish ? "Emergency Contact Name *" : "Nombre del Contacto de Emergencia *";
            if (EmergencyPhoneLabel != null) EmergencyPhoneLabel.Text = isEnglish ? "Emergency Contact Phone *" : "Teléfono de Contacto de Emergencia *";
            if (EmergencyRelationshipLabel != null) EmergencyRelationshipLabel.Text = isEnglish ? "Relationship" : "Relación";
            
            // Step 4
            if (DependentsLabel != null) DependentsLabel.Text = isEnglish ? "Dependents" : "Dependientes";
            if (AddDependentButton != null) AddDependentButton.Text = isEnglish ? "Add Dependent" : "Agregar Dependiente";
            
            // Step 5
            if (PlanSelectionLabel != null) PlanSelectionLabel.Text = isEnglish ? "Plan Selection" : "Selección de Plan";
            if (PlanNameLabel != null) PlanNameLabel.Text = isEnglish ? "Plan Name" : "Nombre del Plan";
            if (PlanContractLabel != null) PlanContractLabel.Text = isEnglish ? "Plan Contract Number *" : "Número de Contrato del Plan *";
            if (PlanIDLabel != null) PlanIDLabel.Text = isEnglish ? "Plan ID *" : "ID del Plan *";
            
            // Step 6
            if (CurrentCoverageLabel != null) CurrentCoverageLabel.Text = isEnglish ? "Current Coverage" : "Cobertura Actual";
            
            // Step 7
            if (SpecialCircumstancesLabel != null) SpecialCircumstancesLabel.Text = isEnglish ? "Special Circumstances" : "Circunstancias Especiales";
            
            // Step 8
            if (SEPLabel != null) SEPLabel.Text = isEnglish ? "Special Enrollment Period (SEP)" : "Período Especial de Inscripción (SEP)";
            
            // Step 9
            if (SignaturesLabel != null) SignaturesLabel.Text = isEnglish ? "Signatures" : "Firmas";
            if (EnrolleeSignatureLabel != null) EnrolleeSignatureLabel.Text = isEnglish ? "Enrollee Signature or X *" : "Firma o X del Afiliado *";
            if (AgentSignatureLabel != null) AgentSignatureLabel.Text = isEnglish ? "Agent Signature *" : "Firma del Agente *";
            if (WitnessSignatureLabel != null) WitnessSignatureLabel.Text = isEnglish ? "Witness Signature (if X)" : "Firma del Testigo (si X)";
            
            // Buttons
            if (BackButton != null) BackButton.Text = isEnglish ? "Back" : "Atrás";
            if (NextButton != null) NextButton.Text = isEnglish ? "Next" : "Siguiente";
            if (CancelButton != null) CancelButton.Text = isEnglish ? "Cancel" : "Cancelar";
            // Signature buttons removed - using modal capture now
        }

        private void SetStep(int step)
        {
            currentStep = step;
            _viewModel.CurrentStep = step;
            for (int i = 0; i < stepPanels.Count; i++)
            {
                stepPanels[i].IsVisible = (i == step - 1);
            }
            BackButton.IsEnabled = (step > 1);
            
            var isEnglish = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English;
            NextButton.Text = (step == stepPanels.Count) ? (isEnglish ? "Submit" : "Enviar") : (isEnglish ? "Next" : "Siguiente");
        }

        private void BackButton_Click(object? sender, EventArgs e)
        {
            if (currentStep > 1)
            {
                SetStep(currentStep - 1);
            }
        }

        private async void NextButton_Click(object? sender, EventArgs e)
        {
            var lang = LanguagePicker.SelectedIndex == 1 ? "es-PR" : "en";
            
            // Validate current step
            if (!await ValidateCurrentStep(lang))
            {
                return;
            }

            if (currentStep < stepPanels.Count)
            {
                SetStep(currentStep + 1);
            }
            else
            {
                // Submit enrollment
                await SubmitEnrollment();
            }
        }

        private async Task<bool> ValidateCurrentStep(string lang)
        {
            // Add validation logic for each step
            switch (currentStep)
            {
                case 1:
                    if (string.IsNullOrWhiteSpace(FirstNameBox.Text))
                    {
                        await DisplayAlert(lang == "es-PR" ? "Nombre requerido" : "First Name Required", 
                            lang == "es-PR" ? "Por favor ingrese el nombre." : "Please enter the first name.", "OK");
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(LastNameBox.Text))
                    {
                        await DisplayAlert(lang == "es-PR" ? "Apellido requerido" : "Last Name Required", 
                            lang == "es-PR" ? "Por favor ingrese el apellido." : "Please enter the last name.", "OK");
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(MedicareBox.Text))
                    {
                        await DisplayAlert(lang == "es-PR" ? "Medicare requerido" : "Medicare Required", 
                            lang == "es-PR" ? "Ingrese el número de Medicare." : "Enter Medicare number.", "OK");
                        return false;
                    }
                    break;
                case 2:
                    if (string.IsNullOrWhiteSpace(Address1Box.Text) || string.IsNullOrWhiteSpace(CityBox.Text) || 
                        string.IsNullOrWhiteSpace(ZIPBox.Text))
                    {
                        await DisplayAlert(lang == "es-PR" ? "Dirección requerida" : "Address Required", 
                            lang == "es-PR" ? "Complete la dirección permanente." : "Complete permanent address.", "OK");
                        return false;
                    }
                    break;
                case 3:
                    if (string.IsNullOrWhiteSpace(EmergencyNameBox.Text) || string.IsNullOrWhiteSpace(EmergencyPhoneBox.Text))
                    {
                        await DisplayAlert(lang == "es-PR" ? "Contacto de emergencia requerido" : "Emergency Contact Required", 
                            lang == "es-PR" ? "Complete el contacto de emergencia." : "Complete emergency contact.", "OK");
                        return false;
                    }
                    break;
                case 9:
                    // Check if signatures are captured via modal (stored in ViewModel)
                    if (string.IsNullOrEmpty(_viewModel.EnrolleeSignatureBase64) && !XMarkCheckbox.IsChecked)
                    {
                        await DisplayAlert(lang == "es-PR" ? "Firma requerida" : "Signature Required", 
                            lang == "es-PR" ? "Capture la firma o marque X." : "Capture signature or mark X.", "OK");
                        return false;
                    }
                    if (string.IsNullOrEmpty(_viewModel.AgentSignatureBase64))
                    {
                        await DisplayAlert(lang == "es-PR" ? "Firma del agente requerida" : "Agent Signature Required", 
                            lang == "es-PR" ? "Capture la firma del agente." : "Capture agent signature.", "OK");
                        return false;
                    }
                    if (XMarkCheckbox.IsChecked && string.IsNullOrEmpty(_viewModel.WitnessSignatureBase64))
                    {
                        await DisplayAlert(lang == "es-PR" ? "Firma del testigo requerida" : "Witness Signature Required", 
                            lang == "es-PR" ? "Se requiere firma del testigo." : "Witness signature required.", "OK");
                        return false;
                    }
                    break;
            }
            return true;
        }

        private async Task SubmitEnrollment()
        {
            var lang = LanguagePicker.SelectedIndex == 1 ? "es-PR" : "en";
            
            try
            {
                // Create full enrollment model for PDF filling
                var hasDifferentMailingAddress = DifferentMailingCheckbox.IsChecked;
                var enrollmentModel = new Models.EnrollmentRecord
                {
                    FirstName = FirstNameBox.Text ?? string.Empty,
                    MiddleInitial = MiddleInitialBox.Text ?? string.Empty,
                    LastName = LastNameBox.Text ?? string.Empty,
                    DateOfBirth = DOBPicker.Date,
                    Gender = GenderCombo.SelectedItem?.ToString() ?? string.Empty,
                    PrimaryPhone = PrimaryPhoneBox.Text ?? string.Empty,
                    PrimaryPhoneIsMobile = PrimaryPhoneIsMobileCheckbox.IsChecked,
                    SecondaryPhone = SecondaryPhoneBox.Text ?? string.Empty,
                    SecondaryPhoneIsMobile = SecondaryPhoneIsMobileCheckbox.IsChecked,
                    Email = EmailBox.Text ?? string.Empty,
                    MedicareNumber = MedicareBox.Text ?? string.Empty,
                    SSN = SSNBox.Text ?? string.Empty,
                    PreferredContactMethod = ContactMethodPicker.SelectedItem?.ToString() ?? string.Empty,
                    Address1 = Address1Box.Text ?? string.Empty,
                    Address2 = Address2Box.Text ?? string.Empty,
                    City = CityBox.Text ?? string.Empty,
                    State = StateBox.Text ?? string.Empty,
                    County = CountyBox.Text ?? string.Empty,
                    ZipCode = ZIPBox.Text ?? string.Empty,
                    DifferentMailingAddress = hasDifferentMailingAddress,
                    MailingAddress1 = hasDifferentMailingAddress ? (MailingAddress1Box.Text ?? string.Empty) : (Address1Box.Text ?? string.Empty),
                    MailingAddress2 = hasDifferentMailingAddress ? (MailingAddress2Box.Text ?? string.Empty) : (Address2Box.Text ?? string.Empty),
                    MailingCity = hasDifferentMailingAddress ? (MailingCityBox.Text ?? string.Empty) : (CityBox.Text ?? string.Empty),
                    MailingState = hasDifferentMailingAddress ? (MailingStateBox.Text ?? string.Empty) : (StateBox.Text ?? string.Empty),
                    MailingZipCode = hasDifferentMailingAddress ? (MailingZIPBox.Text ?? string.Empty) : (ZIPBox.Text ?? string.Empty),
                    EnrolleeSignatureBase64 = _viewModel.EnrolleeSignatureBase64 ?? string.Empty,
                    AgentSignatureBase64 = _viewModel.AgentSignatureBase64 ?? string.Empty,
                    WitnessSignatureBase64 = _viewModel.WitnessSignatureBase64 ?? string.Empty,
                    UsesXMark = XMarkCheckbox.IsChecked,
                    EnrolleeSignatureTimestamp = DateTime.Now.ToString("o"),
                    AgentSignatureTimestamp = DateTime.Now.ToString("o"),
                    WitnessSignatureTimestamp = XMarkCheckbox.IsChecked ? DateTime.Now.ToString("o") : string.Empty,
                    EnrolleeSignatureMethod = XMarkCheckbox.IsChecked ? "XMark" : "Touch"
                };

                // Fill PDF template with enrollment data
                var pdfService = new Services.PdfService();
                var pdfBytes = await pdfService.FillEnrollmentPdfAsync(
                    _enrollmentNumber,
                    enrollmentModel,
                    _viewModel.EnrolleeSignatureBase64 ?? string.Empty,
                    _viewModel.AgentSignatureBase64 ?? string.Empty,
                    _viewModel.WitnessSignatureBase64 ?? string.Empty,
                    XMarkCheckbox.IsChecked
                );

                // Save filled PDF to file system
                var pdfFileName = $"{_enrollmentNumber}.pdf";
                var pdfPath = await pdfService.SavePdfAsync(pdfBytes, pdfFileName, "enrollments");

                // Create enrollment record for the service
                var enrollment = new Services.EnrollmentService.EnrollmentRecord
                {
                    EnrollmentNumber = _enrollmentNumber,
                    FirstName = FirstNameBox.Text ?? string.Empty,
                    LastName = LastNameBox.Text ?? string.Empty,
                    DateCreated = DateTime.Now,
                    FilePath = pdfPath ?? string.Empty,
                    IsUploaded = false
                };

                // Add to enrollment service
                await Services.EnrollmentService.AddEnrollmentRecordAsync(enrollment);

                // Clear form immediately after PDF is generated
                _viewModel.ResetForm();

                await DisplayAlert(
                    lang == "es-PR" ? "Éxito" : "Success",
                    lang == "es-PR" ? "Inscripción guardada exitosamente." : "Enrollment saved successfully.",
                    "OK"
                );

                await Shell.Current.GoToAsync("///DashboardPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error submitting enrollment: {ex.Message}");
                await DisplayAlert(
                    lang == "es-PR" ? "Error" : "Error",
                    lang == "es-PR" ? $"Error al guardar: {ex.Message}" : $"Save error: {ex.Message}",
                    "OK"
                );
            }
        }

        private async void CancelButton_Click(object? sender, EventArgs e)
        {
            var isEnglish = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English;
            bool confirm = await DisplayAlert(
                isEnglish ? "Cancel Enrollment" : "Cancelar Inscripción",
                isEnglish ? "Are you sure you want to cancel?" : "¿Está seguro de que desea cancelar?",
                isEnglish ? "Yes" : "Sí",
                isEnglish ? "No" : "No"
            );

            if (confirm)
            {
                await Shell.Current.GoToAsync("///DashboardPage");
            }
        }

        private void AddDependentButton_Click(object? sender, EventArgs e)
        {
            var dependent = new DependentEntry();
            dependents.Add(dependent);
            DependentsListPanel.Add(dependent.GetView(RemoveDependent));
        }

        private void RemoveDependent(DependentEntry dependent)
        {
            dependents.Remove(dependent);
            DependentsListPanel.Remove(dependent.View);
        }

        private void ClearAllSignatures()
        {
            // Reset signature status for modal captures
            if (EnrolleeSignatureStatus != null)
            {
                EnrolleeSignatureStatus.Text = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "❌ Not Captured" : "❌ No Capturada";
                EnrolleeSignatureStatus.TextColor = Color.FromArgb("#C62828");
            }
            if (EnrolleeSignaturePreviewBorder != null)
                EnrolleeSignaturePreviewBorder.IsVisible = false;
            
            if (AgentSignatureStatus != null)
            {
                AgentSignatureStatus.Text = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "❌ Not Captured" : "❌ No Capturada";
                AgentSignatureStatus.TextColor = Color.FromArgb("#C62828");
            }
            if (AgentSignaturePreviewBorder != null)
                AgentSignaturePreviewBorder.IsVisible = false;
            
            if (WitnessSignatureStatus != null)
            {
                WitnessSignatureStatus.Text = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "❌ Not Captured" : "❌ No Capturada";
                WitnessSignatureStatus.TextColor = Color.FromArgb("#C62828");
            }
            if (WitnessSignaturePreviewBorder != null)
                WitnessSignaturePreviewBorder.IsVisible = false;
            
            XMarkCheckbox.IsChecked = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetStep(1);
            ClearAllSignatures();
            ClearAllFormFields();
            _ = RefreshSOADropdownAsync();
        }

        private void ClearAllFormFields()
        {
            // Step 1 - Personal Information
            FirstNameBox.Text = string.Empty;
            MiddleInitialBox.Text = string.Empty;
            LastNameBox.Text = string.Empty;
            DOBPicker.Date = DateTime.Now.AddYears(-65);
            GenderCombo.SelectedIndex = -1;
            PrimaryPhoneBox.Text = string.Empty;
            PrimaryPhoneIsMobileCheckbox.IsChecked = false;
            SecondaryPhoneBox.Text = string.Empty;
            SecondaryPhoneIsMobileCheckbox.IsChecked = false;
            EmailBox.Text = string.Empty;
            MedicareBox.Text = string.Empty;
            SSNBox.Text = string.Empty;
            ContactMethodPicker.SelectedIndex = 0;

            // Step 2 - Address Information
            Address1Box.Text = string.Empty;
            Address2Box.Text = string.Empty;
            CityBox.Text = string.Empty;
            StateBox.Text = "PR";
            CountyBox.Text = string.Empty;
            ZIPBox.Text = string.Empty;
            DifferentMailingCheckbox.IsChecked = false;
            MailingAddressPanel.IsVisible = false;
            MailingAddress1Box.Text = string.Empty;
            MailingAddress2Box.Text = string.Empty;
            MailingCityBox.Text = string.Empty;
            MailingStateBox.Text = string.Empty;
            MailingZIPBox.Text = string.Empty;

            // Step 3 - Emergency Contact
            EmergencyNameBox.Text = string.Empty;
            EmergencyPhoneBox.Text = string.Empty;
            EmergencyRelationshipCombo.SelectedIndex = -1;

            // Step 4 - Dependents
            dependents.Clear();
            DependentsListPanel.Clear();

            // Step 5 - Plan Selection
            PlanNamePicker.SelectedIndex = -1;
            PlanContractBox.Text = string.Empty;
            PlanIDBox.Text = string.Empty;
            PremiumPaymentMethodPicker.SelectedIndex = 0;
            BankAccountBox.Text = string.Empty;
            RoutingNumberBox.Text = string.Empty;
            CreditCardBox.Text = string.Empty;
            LTCIndicatorCheckbox.IsChecked = false;
            LTCFacilityBox.Text = string.Empty;
            PCPBox.Text = string.Empty;
            PCPClinicBox.Text = string.Empty;

            // Step 6 - Current Coverage
            OtherInsuranceCheckbox.IsChecked = false;
            OtherCoverageTypeBox.Text = string.Empty;
            OtherCoveragePolicyBox.Text = string.Empty;
            CurrentMAPlanCheckbox.IsChecked = false;
            CurrentPlanNameBox.Text = string.Empty;
            CurrentPlanContractBox.Text = string.Empty;
            CurrentPlanIDBox.Text = string.Empty;
            CoverageStartDatePicker.Date = DateTime.Now;
            ReasonChangeBox.Text = string.Empty;

            // Step 7 - Special Circumstances
            SNPIndicatorCheckbox.IsChecked = false;
            SNPTypePicker.SelectedIndex = -1;
            ChronicConditionBox.Text = string.Empty;
            MSAIndicatorCheckbox.IsChecked = false;
            MSADepositBox.Text = string.Empty;
            PFFSIndicatorCheckbox.IsChecked = false;
            DualEligibleCheckbox.IsChecked = false;
            LISCheckbox.IsChecked = false;
            ESRDCheckbox.IsChecked = false;
            InstitutionalCareCheckbox.IsChecked = false;

            // Step 8 - SEP Information
            SEPReasonPicker.SelectedIndex = -1;
            SEPEventDatePicker.Date = DateTime.Now;
            SEPEventDescriptionBox.Text = string.Empty;
            SEPDocumentationBox.Text = string.Empty;
            GoodCauseStatusPicker.SelectedIndex = -1;
            GoodCauseNotesBox.Text = string.Empty;

            // Reset SOA Picker
            SOANumberPicker.SelectedIndex = -1;
        }

        private void XMarkCheckbox_CheckedChanged(object? sender, CheckedChangedEventArgs e)
        {
            // When X mark is checked, witness signature is required
            // No inline pads to enable/disable anymore - using modal
        }

        private async void OnCaptureEnrolleeSignatureClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"signaturemodal?type=enrollee");
        }

        private async void OnCaptureAgentSignatureClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"signaturemodal?type=agent");
        }

        private async void OnCaptureWitnessSignatureClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"signaturemodal?type=witness");
        }

        public class DependentEntry
        {
            public VerticalStackLayout View { get; private set; }
            public Entry NameBox { get; private set; }
            public Picker RelationshipPicker { get; private set; }
            public DatePicker DOBPicker { get; private set; }
            public CheckBox EnrolledCheckBox { get; private set; }
            public Button RemoveButton { get; private set; }

            public DependentEntry()
            {
                NameBox = new Entry { Placeholder = "Name", FontSize = 14 };
                RelationshipPicker = new Picker { FontSize = 14 };
                RelationshipPicker.Items.Add("Spouse");
                RelationshipPicker.Items.Add("Child");
                RelationshipPicker.Items.Add("Parent");
                RelationshipPicker.Items.Add("Sibling");
                RelationshipPicker.Items.Add("Other");
                DOBPicker = new DatePicker { Format = "MM/dd/yyyy", FontSize = 14 };
                EnrolledCheckBox = new CheckBox();
                RemoveButton = new Button { Text = "Remove", BackgroundColor = Color.FromArgb("#F57C00"), TextColor = Colors.White, FontSize = 12 };
                
                View = new VerticalStackLayout
                {
                    Spacing = 8,
                    Padding = new Thickness(8),
                    BackgroundColor = Color.FromArgb("#F5F5F5"),
                    Children = { 
                        new Label { Text = "Name", FontSize = 12, TextColor = Color.FromArgb("#2C3E50") },
                        NameBox, 
                        new Label { Text = "Relationship", FontSize = 12, TextColor = Color.FromArgb("#2C3E50") },
                        RelationshipPicker, 
                        new Label { Text = "Date of Birth", FontSize = 12, TextColor = Color.FromArgb("#2C3E50") },
                        DOBPicker,
                        new HorizontalStackLayout { 
                            Spacing = 8, 
                            Children = { 
                                EnrolledCheckBox, 
                                new Label { Text = "Is Enrolled", FontSize = 12, VerticalOptions = LayoutOptions.Center } 
                            } 
                        },
                        RemoveButton 
                    }
                };
            }

            public VerticalStackLayout GetView(Action<DependentEntry> removeAction)
            {
                RemoveButton.Clicked += (s, e) => removeAction(this);
                return View;
            }
        }
    }
}
