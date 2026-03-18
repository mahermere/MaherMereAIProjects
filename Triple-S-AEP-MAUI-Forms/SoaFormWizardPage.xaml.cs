using CommunityToolkit.Maui.Storage;
using Triple_S_AEP_MAUI_Forms.Models;
using Triple_S_AEP_MAUI_Forms.Services;

namespace Triple_S_AEP_MAUI_Forms;

public partial class SoaFormWizardPage : ContentPage
{
    private readonly PdfFlattenService _pdfFlattenService = new();
    private readonly EnrollmentDatabaseService _dbService = EnrollmentDatabaseService.Instance;
    private readonly DmsUploadService _dmsUploadService = new();
    private string? _beneficiarySignatureDataUrl;
    private string? _authorizedRepSignatureDataUrl;
    private EnrollmentRecord? _linkedRecord;

    public SoaFormWizardPage()
    {
        InitializeComponent();
    }

    public SoaFormWizardPage(EnrollmentRecord record) : this()
    {
        _linkedRecord = record;
    }

    private async void OnSubmitClicked(object? sender, EventArgs e)
    {
        var (submitted, message, soaPath, pdfBytes) = await SubmitFlattenRequestAsync();
        if (!submitted)
        {
            await DisplayAlert("PDF Flatten", message, "OK");
            return;
        }

        // Upload to DMS directly from in-memory bytes
        string? dmsDocId = null;
        var soaUploadStatus = EnrollmentUploadStatus.Pending;
        if (pdfBytes != null && pdfBytes.Length > 0)
        {
            var (uploadSuccess, docId, uploadMsg) = await _dmsUploadService.UploadPdfAsync(DmsUploadService.DocumentTypeIdSoa, pdfBytes);
            dmsDocId = docId;
            soaUploadStatus = uploadSuccess ? EnrollmentUploadStatus.Uploaded : EnrollmentUploadStatus.Failed;
            if (!uploadSuccess)
                System.Diagnostics.Debug.WriteLine($"DMS SOA upload failed: {uploadMsg}");
        }

        try
        {
            // If linked to an enrollment record, update it
            if (_linkedRecord != null)
            {
                _linkedRecord.SoaFormPdfPath = soaPath;
                _linkedRecord.SoaUploadStatus = soaUploadStatus;
                _linkedRecord.SoaFormDmsDocumentId = dmsDocId;
                _dbService.AddOrUpdateRecord(_linkedRecord);
            }
            else
            {
                // Create new SOA record if not linked to enrollment
                var soaRecord = new EnrollmentRecord
                {
                    BeneficiaryFirstName = BeneficiaryFirstNameEntry?.Text ?? string.Empty,
                    BeneficiaryLastName = BeneficiaryLastNameEntry?.Text ?? string.Empty,
                    BeneficiaryMiddleInitial = BeneficiaryMiddleInitialEntry?.Text ?? string.Empty,
                    BeneficiaryPhone = BeneficiaryPhoneEntry?.Text ?? string.Empty,
                    BeneficiaryAltPhone = BeneficiaryAltPhoneEntry?.Text ?? string.Empty,
                    BeneficiaryEmail = BeneficiaryEmailEntry?.Text ?? string.Empty,
                    BeneficiaryAddress1 = BeneficiaryAddress1Entry?.Text ?? string.Empty,
                    BeneficiaryAddress2 = BeneficiaryAddress2Entry?.Text ?? string.Empty,
                    BeneficiaryCity = BeneficiaryCityEntry?.Text ?? string.Empty,
                    BeneficiaryState = BeneficiaryStateEntry?.Text ?? string.Empty,
                    BeneficiaryZip = BeneficiaryZipCodeEntry?.Text ?? string.Empty,
                    BeneficiaryDOB = BeneficiaryDobPicker?.Date,
                    AuthorizedRepFirstName = AuthorizedRepFirstNameEntry?.Text ?? string.Empty,
                    AuthorizedRepLastName = AuthorizedRepLastNameEntry?.Text ?? string.Empty,
                    AuthorizedRepMiddleInitial = AuthorizedRepMiddleInitialEntry?.Text ?? string.Empty,
                    AuthorizedRepRelationship = AuthorizedRepRelationshipEntry?.Text ?? string.Empty,
                    SOANumber = SoaNumberEntry?.Text ?? string.Empty,
                    CampaignNumber = CampaignNumberEntry?.Text ?? string.Empty,
                    CampaignName = CampaignNameEntry?.Text ?? string.Empty,
                    ProductType = ProductTypeEntry?.Text ?? string.Empty,
                    InitialContactMethod = InitialContactMethodEntry?.Text ?? string.Empty,
                    BeneficiaryWalkedIn = BeneficiaryWalkedInCheckBox?.IsChecked ?? false,
                    NewToMedicare = NewToMedicareCheckBox?.IsChecked ?? false,
                    SoaFormPdfPath = soaPath,
                    SoaUploadStatus = soaUploadStatus,
                    SoaFormDmsDocumentId = dmsDocId,
                    CreatedDate = DateTime.Now
                };

                _dbService.AddOrUpdateRecord(soaRecord);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Database Error", $"Failed to save record: {ex.Message}", "OK");
            return;
        }

        await DisplayAlert("Triple-S SOA", "Form submitted successfully.", "OK");
        await Shell.Current.GoToAsync("..");
    }

    private async Task<(bool Submitted, string Message, string? FilePath, byte[]? PdfBytes)> SubmitFlattenRequestAsync()
    {
        var templateBase64 = await LoadTemplatePdfBase64Async();
        if (templateBase64 is null)
        {
            return (false, "Template PDF 'enUS-SOA-Fillable.pdf' was not found in Resources/Raw.", null, null);
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

    private async Task<string?> LoadTemplatePdfBase64Async()
    {
        try
        {
            await using var stream = await FileSystem.OpenAppPackageFileAsync("enUS-SOA-Fillable.pdf");
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return Convert.ToBase64String(memoryStream.ToArray());
        }
        catch
        {
            return null;
        }
    }

    private static async Task<(bool Submitted, string Message, string? FilePath)> SavePdfWhereUserChoosesAsync(byte[] pdfBytes)
    {
        try
        {
            var fileName = $"SOA-Request-{DateTime.Now:yyyyMMdd-HHmmss}.pdf";
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

    private Dictionary<string, object?> BuildAcroFormFieldMap()
    {
        return new Dictionary<string, object?>
        {
            ["SOANumber"] = SoaNumberEntry.Text,
            ["BeneficiaryFirstName"] = BeneficiaryFirstNameEntry.Text,
            ["BeneficiaryMiddleInitial"] = BeneficiaryMiddleInitialEntry.Text,
            ["BeneficiaryLastName"] = BeneficiaryLastNameEntry.Text,
            ["AuthorizedRepFirstName"] = AuthorizedRepFirstNameEntry.Text,
            ["AuthorizedRepLastName"] = AuthorizedRepLastNameEntry.Text,
            ["AuthorizedRepMiddleInitial"] = AuthorizedRepMiddleInitialEntry.Text,
            ["BeneficiaryPhone"] = BeneficiaryPhoneEntry.Text,
            ["BeneficiaryAltPhone"] = BeneficiaryAltPhoneEntry.Text,
            ["BeneficiaryAddress1"] = BeneficiaryAddress1Entry.Text,
            ["BeneficiaryAddress2"] = BeneficiaryAddress2Entry.Text,
            ["BeneficiaryCity"] = BeneficiaryCityEntry.Text,
            ["BeneficiaryState"] = BeneficiaryStateEntry.Text,
            ["BeneficiaryZipCode"] = BeneficiaryZipCodeEntry.Text,
            ["AuthorizedRepRelationship"] = AuthorizedRepRelationshipEntry.Text,
            ["BeneficiaryEmail"] = BeneficiaryEmailEntry.Text,
            ["CampaignNumber"] = CampaignNumberEntry.Text,
            ["InitialContactMethod"] = InitialContactMethodEntry.Text,
            ["BeneficiaryWalkedIn"] = BeneficiaryWalkedInCheckBox.IsChecked,
            ["CampaignName"] = CampaignNameEntry.Text,
            ["text_21unus"] = ProductTypeEntry.Text,
            ["checkbox_22toml"] = NewToMedicareCheckBox.IsChecked,
            ["radio_group_23dooo"] = CurrentPlanMmmCheckBox.IsChecked ? "Value_qxrq" : null,
            ["radio_group_24eaaj"] = CurrentPlanMcsCheckBox.IsChecked ? "Value_qeoa" : null,
            ["radio_group_25zkwj"] = CurrentPlanHumanaCheckBox.IsChecked ? "Value_lpgu" : null,
            ["radio_group_26cchk"] = CurrentPlanUsPlanCheckBox.IsChecked ? "Value_ptjg" : null,
            ["radio_group_27zlzt"] = CurrentPlanVeteranCheckBox.IsChecked ? "Value_cmbb" : null,
            ["text_29oxzt"] = CurrentPlanOtherEntry.Text,
            ["BeneficiaryDOB"] = BeneficiaryDobPicker.Date.ToString("MM/dd/yyyy"),
            ["BeneficiarySignedDate"] = BeneficiarySignedDatePicker.Date.ToString("MM/dd/yyyy"),
            ["AuthorizedRepSignedDate"] = AuthorizedRepSignedDatePicker.Date.ToString("MM/dd/yyyy"),
            ["BeneSignatureElectronic"] = BeneSignatureElectronicCheckBox.IsChecked,
            ["BeneficiaryAcceptedTandC"] = BeneficiaryAcceptedTandCCheckBox.IsChecked,
            ["BeneficiaPhoneUID"] = BeneficiaPhoneUidEntry.Text,
            ["AgentName"] = AgentNameEntry.Text,
            ["AgentPhoneNumber"] = AgentPhoneNumberEntry.Text,
            ["AgentNPN"] = AgentNpnEntry.Text,
            ["SOADate"] = SoaDatePicker.Date.ToString("MM/dd/yyyy"),
            ["SalesPromoterName"] = SalesPromoterNameEntry.Text,
            ["SalesPromoterID"] = SalesPromoterIdEntry.Text,
            ["MedicarePartC"] = MedicarePartCCheckBox.IsChecked,
            ["MedicareMedigap"] = MedicareMedigapCheckBox.IsChecked,
            ["MedicareAncillaryProducts"] = MedicareAncillaryProductsCheckBox.IsChecked,
            ["MedicareDirecto"] = MedicareDirectoCheckBox.IsChecked,
            ["Additionalnotes"] = AdditionalNotesEditor.Text
        };
    }

    private Dictionary<string, string> BuildAcroFormImageMap()
    {
        var images = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(_beneficiarySignatureDataUrl))
        {
            images["BeneficiarySignature"] = ExtractBase64(_beneficiarySignatureDataUrl);
        }

        if (!string.IsNullOrWhiteSpace(_authorizedRepSignatureDataUrl))
        {
            images["AuthorizedRepSignature"] = ExtractBase64(_authorizedRepSignatureDataUrl);
        }

        return images;
    }

    private async void OnCaptureBeneficiarySignatureClicked(object? sender, EventArgs e)
    {
        _beneficiarySignatureDataUrl = await CaptureSignatureAsync("Capture Beneficiary Signature");
        UpdateSignaturePreview(_beneficiarySignatureDataUrl, BeneficiarySignaturePreview, ClearBeneficiarySignatureButton);
    }

    private async void OnCaptureAuthorizedRepSignatureClicked(object? sender, EventArgs e)
    {
        _authorizedRepSignatureDataUrl = await CaptureSignatureAsync("Capture Authorized Representative Signature");
        UpdateSignaturePreview(_authorizedRepSignatureDataUrl, AuthorizedRepSignaturePreview, ClearAuthorizedRepSignatureButton);
    }

    private void OnClearBeneficiarySignatureClicked(object? sender, EventArgs e)
    {
        _beneficiarySignatureDataUrl = null;
        UpdateSignaturePreview(null, BeneficiarySignaturePreview, ClearBeneficiarySignatureButton);
    }

    private void OnClearAuthorizedRepSignatureClicked(object? sender, EventArgs e)
    {
        _authorizedRepSignatureDataUrl = null;
        UpdateSignaturePreview(null, AuthorizedRepSignaturePreview, ClearAuthorizedRepSignatureButton);
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
}
