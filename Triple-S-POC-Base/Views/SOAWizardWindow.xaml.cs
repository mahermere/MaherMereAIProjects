using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui;
using Microsoft.Maui.Controls;      
using System;
using System.IO;
#if ANDROID
using Android.Content;
using Android.App;
using Android.Net;
using Java.IO;
#endif


namespace TripleS.SOA.AEP.UI.Views
{
    public partial class SOAWizardWindow : ContentPage
    {
        private int currentStep = 1;
        private readonly TripleS.SOA.AEP.UI.Services.IPdfOpener _pdfOpener;
        public SOAWizardWindow() : this(TripleS.SOA.AEP.UI.Services.ServiceLocator.PdfOpener) {}
        public SOAWizardWindow(TripleS.SOA.AEP.UI.Services.IPdfOpener pdfOpener)
        {
            _pdfOpener = pdfOpener;
            InitializeComponent();
            var backButton = this.FindByName<Button>("BackButton");
            var nextButton = this.FindByName<Button>("NextButton");
            var clearSignatureButton = this.FindByName<Button>("ClearSignatureButton");
            if (backButton != null)
                backButton.Clicked += BackButton_Click;
            if (nextButton != null)
                nextButton.Clicked += NextButton_Click;
            if (clearSignatureButton != null)
                clearSignatureButton.Clicked += ClearSignatureButton_Click;
            this.Title = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Title;
            SetLocalizedText();
            var contactMethodPicker = this.FindByName<Picker>("ContactMethodPicker2");
            SetContactMethodPickerItems(contactMethodPicker);
            TripleSPOC.Services.LanguageService.Instance.LanguageChanged += OnLanguageChanged;
        }


        // Helper to retrieve SOA number from ViewModel
        private string GetSOANumber()
        {
            return (BindingContext as TripleS.SOA.AEP.UI.ViewModels.SOAWizardViewModel)?.SOANumber ?? "SOA-UNKNOWN";
        }

        private async void SetLocalizedText()
        {
            this.Title = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Title;
            var missingElements = new System.Collections.Generic.List<string>();

            // Step 1 labels
            var firstNameLabel1 = this.FindByName<Label>("FirstNameLabel1");
            if (firstNameLabel1 != null) firstNameLabel1.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_FirstName;
            else missingElements.Add("FirstNameLabel1");
            var lastNameLabel1 = this.FindByName<Label>("LastNameLabel1");
            if (lastNameLabel1 != null) lastNameLabel1.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_LastName;
            else missingElements.Add("LastNameLabel1");
            var dobLabel1 = this.FindByName<Label>("DOBLabel1");
            if (dobLabel1 != null) dobLabel1.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_DOB;
            else missingElements.Add("DOBLabel1");
            var medicareLabel1 = this.FindByName<Label>("MedicareLabel1");
            if (medicareLabel1 != null) medicareLabel1.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Medicare;
            else missingElements.Add("MedicareLabel1");
            var phoneLabel1 = this.FindByName<Label>("PhoneLabel1");
            if (phoneLabel1 != null) phoneLabel1.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Phone;
            else missingElements.Add("PhoneLabel1");

            // Step 2 labels
            var appointmentDateLabel2 = this.FindByName<Label>("AppointmentDateLabel2");
            if (appointmentDateLabel2 != null) appointmentDateLabel2.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_ApptDate;
            else missingElements.Add("AppointmentDateLabel2");
            var appointmentTimeLabel2 = this.FindByName<Label>("AppointmentTimeLabel2");
            if (appointmentTimeLabel2 != null) appointmentTimeLabel2.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_ApptTime;
            else missingElements.Add("AppointmentTimeLabel2");
            var agentNameLabel2 = this.FindByName<Label>("AgentNameLabel2");
            if (agentNameLabel2 != null) agentNameLabel2.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_AgentName;
            else missingElements.Add("AgentNameLabel2");
            var contactMethodLabel2 = this.FindByName<Label>("ContactMethodLabel2");
            if (contactMethodLabel2 != null) contactMethodLabel2.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_ContactMethod;
            else missingElements.Add("ContactMethodLabel2");

            // Step 3 labels (checkboxes)
            var advantageCheckLabel3 = this.FindByName<Label>("AdvantageCheckLabel3");
            if (advantageCheckLabel3 != null) advantageCheckLabel3.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Advantage;
            else missingElements.Add("AdvantageCheckLabel3");
            var drugCheckLabel3 = this.FindByName<Label>("DrugCheckLabel3");
            if (drugCheckLabel3 != null) drugCheckLabel3.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Drug;
            else missingElements.Add("DrugCheckLabel3");
            var supplementCheckLabel3 = this.FindByName<Label>("SupplementCheckLabel3");
            if (supplementCheckLabel3 != null) supplementCheckLabel3.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Supplement;
            else missingElements.Add("SupplementCheckLabel3");
            var dentalCheckLabel3 = this.FindByName<Label>("DentalCheckLabel3");
            if (dentalCheckLabel3 != null) dentalCheckLabel3.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Dental;
            else missingElements.Add("DentalCheckLabel3");

            // Step 4 labels
            var attestCheckLabel4 = this.FindByName<Label>("AttestCheckLabel4");
            if (attestCheckLabel4 != null) attestCheckLabel4.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Attest;
            else missingElements.Add("AttestCheckLabel4");

            // Wizard step headers and section labels
            var step1Label = this.FindByName<Label>("Step1Label");
            if (step1Label != null) step1Label.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Step1;
            else missingElements.Add("Step1Label");
            var step2Label = this.FindByName<Label>("Step2Label");
            if (step2Label != null) step2Label.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Step2;
            else missingElements.Add("Step2Label");
            var step3Label = this.FindByName<Label>("Step3Label");
            if (step3Label != null) step3Label.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Step3;
            else missingElements.Add("Step3Label");
            var selectPlansLabel3 = this.FindByName<Label>("SelectPlansLabel3");
            if (selectPlansLabel3 != null) selectPlansLabel3.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_SelectPlans;
            else missingElements.Add("SelectPlansLabel3");
            var advantageLabel3 = this.FindByName<Label>("AdvantageLabel3");
            if (advantageLabel3 != null) advantageLabel3.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Advantage;
            else missingElements.Add("AdvantageLabel3");
            var drugLabel3 = this.FindByName<Label>("DrugLabel3");
            if (drugLabel3 != null) drugLabel3.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Drug;
            else missingElements.Add("DrugLabel3");
            var supplementLabel3 = this.FindByName<Label>("SupplementLabel3");
            if (supplementLabel3 != null) supplementLabel3.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Supplement;
            else missingElements.Add("SupplementLabel3");
            var dentalLabel3 = this.FindByName<Label>("DentalLabel3");
            if (dentalLabel3 != null) dentalLabel3.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Dental;
            else missingElements.Add("DentalLabel3");
            var signatureLabel3 = this.FindByName<Label>("SignatureLabel3");
            if (signatureLabel3 != null) signatureLabel3.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Signature;
            else missingElements.Add("SignatureLabel3");
            var step4Label = this.FindByName<Label>("Step4Label");
            if (step4Label != null) step4Label.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Step4;
            else missingElements.Add("Step4Label");
            var attestLabel4 = this.FindByName<Label>("AttestLabel4");
            if (attestLabel4 != null) attestLabel4.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Attest;
            else missingElements.Add("AttestLabel4");

            // Buttons
            var clearSignatureButton3 = this.FindByName<Button>("ClearSignatureButton3");
            if (clearSignatureButton3 != null) clearSignatureButton3.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_ClearSignature;
            else missingElements.Add("ClearSignatureButton3");
            var backButton = this.FindByName<Button>("BackButton");
            if (backButton != null) backButton.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Back;
            else missingElements.Add("BackButton");
            var nextButton = this.FindByName<Button>("NextButton");
            if (nextButton != null) nextButton.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Next;
            else missingElements.Add("NextButton");

            // Placeholders for Entry fields
            var firstNameBox1 = this.FindByName<Entry>("FirstNameBox1");
            if (firstNameBox1 != null) firstNameBox1.Placeholder = TripleSPOC.Resources.Localization.AppResources.SOAWizard_FirstName;
            else missingElements.Add("FirstNameBox1");
            var lastNameBox1 = this.FindByName<Entry>("LastNameBox1");
            if (lastNameBox1 != null) lastNameBox1.Placeholder = TripleSPOC.Resources.Localization.AppResources.SOAWizard_LastName;
            else missingElements.Add("LastNameBox1");
            var medicareBox1 = this.FindByName<Entry>("MedicareBox1");
            if (medicareBox1 != null) medicareBox1.Placeholder = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Medicare;
            else missingElements.Add("MedicareBox1");
            var phoneBox1 = this.FindByName<Entry>("PhoneBox1");
            if (phoneBox1 != null) phoneBox1.Placeholder = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Phone;
            else missingElements.Add("PhoneBox1");
            var agentNameBox2 = this.FindByName<Entry>("AgentNameBox2");
            if (agentNameBox2 != null) agentNameBox2.Placeholder = TripleSPOC.Resources.Localization.AppResources.SOAWizard_AgentName;
            else missingElements.Add("AgentNameBox2");

            if (missingElements.Count > 0)
            {
                var errorMsg = $"The following UI elements are missing in SOAWizardWindow XAML:\n" + string.Join("\n", missingElements);
                await this.DisplayAlert("SOAWizardWindow Error", errorMsg, "OK");
            }
        }

        private void SetContactMethodPickerItems(Picker? picker)
        {
            if (picker == null) return;
            var selectedIndex = picker.SelectedIndex;
            picker.Items.Clear();
            picker.Items.Add(TripleSPOC.Resources.Localization.AppResources.SOAWizard_ContactMethod_Phone);
            picker.Items.Add(TripleSPOC.Resources.Localization.AppResources.SOAWizard_ContactMethod_InPerson);
            picker.Items.Add(TripleSPOC.Resources.Localization.AppResources.SOAWizard_ContactMethod_Video);
            // Try to preserve selection if possible
            if (selectedIndex >= 0 && selectedIndex < picker.Items.Count)
                picker.SelectedIndex = selectedIndex;
        }

        private void OnLanguageChanged(TripleSPOC.Models.Language lang)
        {
            SetLocalizedText();
            var contactMethodPicker = this.FindByName<Picker>("ContactMethodPicker2");
            SetContactMethodPickerItems(contactMethodPicker);
            this.Title = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Title;
        }

        private void SetStep(int step)
        {
            currentStep = step;
            var step1Panel = this.FindByName<StackLayout>("Step1Panel");
            var step2Panel = this.FindByName<StackLayout>("Step2Panel");
            var step3Panel = this.FindByName<StackLayout>("Step3Panel");
            var step4Panel = this.FindByName<StackLayout>("Step4Panel");
            var backButton = this.FindByName<Button>("BackButton");
            var nextButton = this.FindByName<Button>("NextButton");
            if (step1Panel != null) step1Panel.IsVisible = (step == 1);
            if (step2Panel != null) step2Panel.IsVisible = (step == 2);
            if (step3Panel != null) step3Panel.IsVisible = (step == 3);
            if (step4Panel != null) step4Panel.IsVisible = (step == 4);
            if (backButton != null) backButton.IsEnabled = (step > 1);
            if (nextButton != null)
            {
                // Use SOAWizard_Next for both Next and Submit, since Submit is not defined in resources
                nextButton.Text = TripleSPOC.Resources.Localization.AppResources.SOAWizard_Next ?? ((step == 4) ? "Submit" : "Next");
            }
        }

        private void BackButton_Click(object? sender, EventArgs e)
        {
            if (currentStep > 1)
                SetStep(currentStep - 1);
        }

        private async void NextButton_Click(object? sender, EventArgs e)
        {
            if (currentStep == 1)
            {
                // Step 1 validation
                var firstName = this.FindByName<Entry>("FirstNameBox1")?.Text;
                var lastName = this.FindByName<Entry>("LastNameBox1")?.Text;
                var dobPicker = this.FindByName<DatePicker>("DOBPicker1");
                var dob = dobPicker?.Date;
                var medicare = this.FindByName<Entry>("MedicareBox1")?.Text;
                var phone = this.FindByName<Entry>("PhoneBox1")?.Text;
                var gender = this.FindByName<Picker>("GenderCombo1")?.SelectedItem?.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(firstName) ||
                    string.IsNullOrWhiteSpace(lastName) ||
                    dob == null ||
                    string.IsNullOrWhiteSpace(medicare) ||
                    string.IsNullOrWhiteSpace(phone))
                {
                    await DisplayAlert("Validation Error", "All fields are required.", "OK");
                    return;
                }
                // DOB validation: must be at least 65 years old
                var today = DateTime.Today;
                var age = today.Year - dob.Value.Year;
                if (dob > today.AddYears(-age)) age--;
                if (age < 65)
                {
                    await DisplayAlert("Validation Error", "Beneficiary must be at least 65 years old for Medicare eligibility.", "OK");
                    return;
                }
                // Save to SOA first page CSV for enrollment prefill
                var soaCsvPath = Path.Combine(FileSystem.Current.AppDataDirectory, "soa_firstpage_records.csv");
                var rec = new TripleSPOC.Models.SOAFirstPageRecord
                {
                    FirstName = firstName ?? string.Empty,
                    LastName = lastName ?? string.Empty,
                    DateOfBirth = dob ?? DateTime.MinValue,
                    Gender = gender,
                    PrimaryPhone = phone ?? string.Empty,
                    MedicareNumber = medicare ?? string.Empty
                };
                TripleS.Utilities.CsvDataUtility.AppendCsv(soaCsvPath, rec, TripleSPOC.Models.SOAFirstPageRecord.ToCsv);
                SetStep(2);
            }
            else if (currentStep == 2)
            {
                // Step 2 validation
                var appointmentDate = this.FindByName<DatePicker>("AppointmentDatePicker2")?.Date;
                var appointmentTime = this.FindByName<TimePicker>("AppointmentTimePicker2")?.Time;
                var agentName = this.FindByName<Entry>("AgentNameBox2")?.Text;
                var contactMethodPicker = this.FindByName<Picker>("ContactMethodPicker2");
                var contactMethod = contactMethodPicker?.SelectedIndex >= 0 ? contactMethodPicker.SelectedItem?.ToString() : null;
                if (appointmentDate == null ||
                    appointmentTime == null ||
                    string.IsNullOrWhiteSpace(agentName) ||
                    string.IsNullOrWhiteSpace(contactMethod))
                {
                    await DisplayAlert("Validation Error", "All appointment details are required.", "OK");
                    return;
                }
                if (appointmentDate < DateTime.Today)
                {
                    await DisplayAlert("Validation Error", "Appointment date cannot be in the past.", "OK");
                    return;
                }
                SetStep(3);
            }
            else if (currentStep == 3)
            {
                // Step 3 validation
                var cbAdvantage = this.FindByName<CheckBox>("CBAdvantage3");
                var cbDrug = this.FindByName<CheckBox>("CBDrug3");
                var cbSupplement = this.FindByName<CheckBox>("CBSupplement3");
                var cbDental = this.FindByName<CheckBox>("CBDental3");
                bool anyChecked = (cbAdvantage?.IsChecked ?? false) || (cbDrug?.IsChecked ?? false) || (cbSupplement?.IsChecked ?? false) || (cbDental?.IsChecked ?? false);
                if (!anyChecked)
                {
                    await DisplayAlert("Validation Error", "Please select at least one plan type.", "OK");
                    return;
                }
                var signaturePad = this.FindByName<DrawingView>("SignaturePad3");
                if (signaturePad == null || signaturePad.Lines == null || signaturePad.Lines.Count == 0)
                {
                    await DisplayAlert("Validation Error", "Please provide a signature.", "OK");
                    return;
                }
                SetStep(4);
            }
            else if (currentStep == 4)
            {
                // Step 4 validation
                var attestCheckbox = this.FindByName<CheckBox>("AttestCheckbox4");
                if (attestCheckbox == null || !attestCheckbox.IsChecked)
                {
                    await DisplayAlert("Validation Error", "You must attest that all information is accurate.", "OK");
                    return;
                }

                // PDF generation
                try
                {
                    var signaturePad = this.FindByName<CommunityToolkit.Maui.Views.DrawingView>("SignaturePad3");
                    Stream? signatureStream = null;
                    if (signaturePad != null)
                    {
                        // Export signature as PNG stream
                        signatureStream = await signaturePad.GetImageStream(200, 80);
                    }

                    // Use the new PdfService from the Android-specific library
                    var firstName = this.FindByName<Entry>("FirstNameBox1")?.Text ?? string.Empty;
                    var lastName = this.FindByName<Entry>("LastNameBox1")?.Text ?? string.Empty;
                    var beneficiaryName = firstName + " " + lastName;
                    var dob = this.FindByName<DatePicker>("DOBPicker1")?.Date ?? DateTime.MinValue;
                    var medicareNumber = this.FindByName<Entry>("MedicareBox1")?.Text ?? string.Empty;
                    var phone = this.FindByName<Entry>("PhoneBox1")?.Text ?? string.Empty;
                    var meetingDate = this.FindByName<DatePicker>("AppointmentDatePicker2")?.Date ?? DateTime.Now;
                    var meetingTime = this.FindByName<TimePicker>("AppointmentTimePicker2")?.Time ?? TimeSpan.Zero;
                    var meetingLocation = phone;
                    var agentName = this.FindByName<Entry>("AgentNameBox2")?.Text ?? string.Empty;
                    var contactMethodPicker = this.FindByName<Picker>("ContactMethodPicker2");
                    var contactMethod = contactMethodPicker?.SelectedIndex >= 0 ? contactMethodPicker.SelectedItem?.ToString() ?? string.Empty : string.Empty;
                    var cbAdvantage = this.FindByName<CheckBox>("CBAdvantage3")?.IsChecked ?? false;
                    var cbDrug = this.FindByName<CheckBox>("CBDrug3")?.IsChecked ?? false;
                    var cbSupplement = this.FindByName<CheckBox>("CBSupplement3")?.IsChecked ?? false;
                    var cbDental = this.FindByName<CheckBox>("CBDental3")?.IsChecked ?? false;
                    var attestChecked = this.FindByName<CheckBox>("AttestCheckbox4")?.IsChecked ?? false;
                    byte[]? signatureBytes = null;
                    if (signatureStream != null)
                    {
                        using (var ms = new MemoryStream())
                        {
                            signatureStream.CopyTo(ms);
                            signatureBytes = ms.ToArray();
                        }
                    }
                    byte[] pdfBytes;
#if ANDROID
                    pdfBytes = TripleS.SOA.AEP.UI.Platforms.Android.PdfService.GenerateSOAPdf(
                        beneficiaryName,
                        dob,
                        medicareNumber,
                        phone,
                        meetingDate,
                        meetingTime,
                        meetingLocation,
                        agentName,
                        contactMethod,
                        cbAdvantage,
                        cbDrug,
                        cbSupplement,
                        cbDental,
                        attestChecked,
                        signatureBytes);
#else
                    pdfBytes = TripleS.SOA.AEP.UI.Services.PdfService.GenerateSOAPdf(
                        GetSOANumber(),
                        beneficiaryName,
                        dob,
                        medicareNumber,
                        phone,
                        meetingDate,
                        meetingTime,
                        meetingLocation,
                        agentName,
                        contactMethod,
                        cbAdvantage,
                        cbDrug,
                        cbSupplement,
                        cbDental,
                        attestChecked,
                        signatureBytes);
#endif
                    var fileName = $"SOA_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                    var filePath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
                    System.IO.File.WriteAllBytes(filePath, pdfBytes);
                    await DisplayAlert("Success", $"SOA PDF generated and saved: {filePath}", "OK");
                    // If attestation is complete, navigate to dashboard
                    if (currentStep == 4) // Assuming step 4 is attestation
                    {
                        // Add SOA record for dashboard and persist to CSV
                        var newRecord = new TripleS.SOA.AEP.UI.Services.SOANumberService.SOARecord {
                            SOANumber = GetSOANumber(),
                            FirstName = firstName,
                            LastName = lastName
                        };
                        TripleS.SOA.AEP.UI.Services.SOANumberService.AddSOARecord(newRecord);
                        // Append to CSV for persistence
                        var soaCsvPath = Path.Combine(FileSystem.Current.AppDataDirectory, "soa_records.csv");
                        TripleS.Utilities.CsvDataUtility.AppendCsv(soaCsvPath, newRecord, r => $"{r.SOANumber},{r.FirstName},{r.LastName}");
                        await Navigation.PopToRootAsync();
                        await Navigation.PushAsync(new DashboardWindow());
                        return;
                    }
                    // After SOA creation, refresh EnrollmentWizardPage dropdown if open
                    var nav = this.Navigation;
                    if (nav != null)
                    {
                        foreach (var page in nav.NavigationStack)
                        {
                            if (page is EnrollmentWizardPage enrollmentPage)
                            {
                                enrollmentPage.RefreshSOADropdown();
                            }
                        }
                    }
                    try
                    {
                        _pdfOpener?.OpenPdf(filePath);
                    }
                    catch (Exception openEx)
                    {
                        await DisplayAlert("Open PDF Error", openEx.Message, "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("PDF Error", ex.Message, "OK");
                }
            }
        }
    

        private void ClearSignatureButton_Click(object? sender, EventArgs e)
        {
            var signaturePad = this.FindByName<DrawingView>("SignaturePad");
            signaturePad?.Clear();
        }
    }
}
