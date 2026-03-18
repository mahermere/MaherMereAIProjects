using CommunityToolkit.Maui.Storage;
using Triple_S_AEP_MAUI_Forms.Models;
using Triple_S_AEP_MAUI_Forms.Services;

namespace Triple_S_AEP_MAUI_Forms;

public partial class WorkingAgedSurveyPage : ContentPage
{
    private readonly PdfFlattenService _pdfFlattenService = new();
    private readonly EnrollmentDatabaseService _dbService = EnrollmentDatabaseService.Instance;
    private readonly DmsUploadService _dmsUploadService = new();
    private readonly TaskCompletionSource<(bool Completed, string Message)> _result = new();
    private EnrollmentRecord? _linkedRecord;

    public WorkingAgedSurveyPage(string? medicareNumber, string? firstName, string? middleInitial, string? lastName, bool beneficiaryWorks, bool spouseWorks)
    {
        InitializeComponent();

        MedicareNumberEntry.Text = medicareNumber;
        BeneficiaryFirstNameEntry.Text = firstName;
        BeneficiaryMiddleInitialEntry.Text = middleInitial;
        BeneficiaryLastNameEntry.Text = lastName;

        if (beneficiaryWorks)
        {
            CurrentlyWorkingYes.IsChecked = true;
            EmployerInsuranceNa.IsChecked = false;
        }
        else
        {
            CurrentlyWorkingNo.IsChecked = true;
            EmployerInsuranceNa.IsChecked = true;
        }

        if (!spouseWorks)
        {
            SpouseEmployerInsuranceNa.IsChecked = true;
        }
    }

    public WorkingAgedSurveyPage(EnrollmentRecord record, string? medicareNumber, string? firstName, string? middleInitial, string? lastName, bool beneficiaryWorks, bool spouseWorks)
    {
        InitializeComponent();

        _linkedRecord = record;

        MedicareNumberEntry.Text = medicareNumber;
        BeneficiaryFirstNameEntry.Text = firstName;
        BeneficiaryMiddleInitialEntry.Text = middleInitial;
        BeneficiaryLastNameEntry.Text = lastName;

        // Auto-fill from linked SOA if available
        if (record != null)
        {
            if (!string.IsNullOrEmpty(record.BeneficiaryPhone))
                MedicareNumberEntry.Text = medicareNumber; // Keep original, but can be overridden

            // Set work status based on beneficiary
            if (beneficiaryWorks)
            {
                CurrentlyWorkingYes.IsChecked = true;
                EmployerInsuranceNa.IsChecked = false;
            }
            else
            {
                CurrentlyWorkingNo.IsChecked = true;
                EmployerInsuranceNa.IsChecked = true;
            }

            // Set spouse work status
            if (!spouseWorks)
            {
                SpouseEmployerInsuranceNa.IsChecked = true;
            }
        }
        else
        {
            // Fallback behavior if no record
            if (beneficiaryWorks)
            {
                CurrentlyWorkingYes.IsChecked = true;
                EmployerInsuranceNa.IsChecked = false;
            }
            else
            {
                CurrentlyWorkingNo.IsChecked = true;
                EmployerInsuranceNa.IsChecked = true;
            }

            if (!spouseWorks)
            {
                SpouseEmployerInsuranceNa.IsChecked = true;
            }
        }
    }

    public Task<(bool Completed, string Message)> Result => _result.Task;

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        _result.TrySetResult((false, "Working Aged Survey canceled."));
        await Navigation.PopModalAsync();
    }

    private async void OnSubmitClicked(object? sender, EventArgs e)
    {
        var (submitted, message, surveyPath, pdfBytes) = await SubmitFlattenRequestAsync();

        if (submitted && _linkedRecord != null)
        {
            _linkedRecord.WorkingAgeSurveyPdfPath = surveyPath;
            _linkedRecord.WorkingAgeSurveyUploadStatus = EnrollmentUploadStatus.Pending;
            _linkedRecord.SubmittedDate = DateTime.Now;

            // Upload survey to DMS directly from in-memory bytes
            if (pdfBytes != null && pdfBytes.Length > 0)
            {
                var (uploadSuccess, docId, uploadMsg) = await _dmsUploadService.UploadPdfAsync(DmsUploadService.DocumentTypeIdWorkingAgeSurvey, pdfBytes);
                _linkedRecord.WorkingAgeSurveyUploadStatus = uploadSuccess ? EnrollmentUploadStatus.Uploaded : EnrollmentUploadStatus.Failed;
                _linkedRecord.WorkingAgeSurveyDmsDocumentId = docId;
                if (!uploadSuccess)
                    System.Diagnostics.Debug.WriteLine($"DMS survey upload failed: {uploadMsg}");
            }

            try
            {
                _dbService.AddOrUpdateRecord(_linkedRecord);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Database Error", $"Failed to update record: {ex.Message}", "OK");
            }
        }

        _result.TrySetResult((submitted, message));

        if (submitted)
        {
            await DisplayAlert("Working Aged Survey", "Survey PDF generated successfully.", "OK");
        }
        else
        {
            await DisplayAlert("Working Aged Survey", message, "OK");
        }

        await Navigation.PopModalAsync();
    }

    private async Task<(bool Submitted, string Message, string? FilePath, byte[]? PdfBytes)> SubmitFlattenRequestAsync()
    {
        var templateBase64 = await LoadTemplatePdfBase64Async();
        if (templateBase64 is null)
        {
            return (false, "Template PDF 'enUS-Working-Aged-Survey-Fillable.pdf' was not found in Resources/Raw.", null, null);
        }

        var request = new PdfFlattenRequest
        {
            Base64TemplatePdf = templateBase64,
            Fields = BuildAcroFormFieldMap(),
            Images = new Dictionary<string, string>()
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
            await using var stream = await FileSystem.OpenAppPackageFileAsync("enUS-Working-Aged-Survey-Fillable.pdf");
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
            var fileName = $"Working-Aged-Survey-{DateTime.Now:yyyyMMdd-HHmmss}.pdf";
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
            ["MedicareNumber"] = MedicareNumberEntry.Text,
            ["CurrentlyWorking"] = CurrentlyWorkingYes.IsChecked ? "Yes" : CurrentlyWorkingNo.IsChecked ? "No" : null,
            ["EmployerHealthInsurance"] = EmployerInsuranceYes.IsChecked ? "Yes" : EmployerInsuranceNo.IsChecked ? "No" : EmployerInsuranceNa.IsChecked ? "NA" : null,
            ["SpouseEmployerHealthInsurance"] = SpouseEmployerInsuranceYes.IsChecked ? "Yes" : SpouseEmployerInsuranceNo.IsChecked ? "No" : SpouseEmployerInsuranceNa.IsChecked ? "NA" : null,
            ["HealthInsuranceEmployer_Name"] = EmployerNameEntry.Text,
            ["HealthInsuranceEmployer_Address"] = EmployerAddressEntry.Text,
            ["HealthInsuranceEmployer_InsuranceName"] = InsuranceNameEntry.Text,
            ["HealthInsuranceEmployer_PolicyholderName"] = PolicyholderNameEntry.Text,
            ["HealthInsuranceEmployer_GroupNumber"] = EmployerGroupNumberEntry.Text,
            ["HealthInsuranceEmployer_Contract"] = EmployerContractEntry.Text,
            ["HealthInsuranceEmployer_Morethan20Employees"] = MoreThan20Yes.IsChecked ? "Yes" : MoreThan20No.IsChecked ? "No" : MoreThan20Na.IsChecked ? "NA" : null,
            ["HealthInsuranceEmployer_PlanonDisenroll"] =
                DisenrollThreeMonths.IsChecked ? "ThreeMonths" :
                DisenrollSixMonths.IsChecked ? "SixMonths" :
                DisenrollOneYear.IsChecked ? "OneYear" :
                DisenrollNotYet.IsChecked ? "NotYet" :
                DisenrollNa.IsChecked ? "NA" : null,
            ["BusinessOwner"] = BusinessOwnerYes.IsChecked ? "Yes" : BusinessOwnerNo.IsChecked ? "No" : null,
            ["BusinessOwner_BusinessHealthInsurance"] = BusinessInsuranceYes.IsChecked ? "Yes" : BusinessInsuranceNo.IsChecked ? "No" : BusinessInsuranceNa.IsChecked ? "NA" : null,
            ["BusinessOwner_CompanyName"] = CompanyNameEntry.Text,
            ["BusinessOwner_CompanyAddress"] = CompanyAddressEntry.Text,
            ["BusinessOwner_HealthcareInsuranceName"] = CompanyInsuranceNameEntry.Text,
            ["BusinessOwner_HealthInsuranceGroupNumber"] = CompanyGroupNumberEntry.Text,
            ["BusinessOwner_HealthInsuranceContractNumber"] = CompanyContractNumberEntry.Text,
            ["RejectedFromEmployerGroupHealthCare"] = RejectedYes.IsChecked ? "Yes" : RejectedNo.IsChecked ? "No" : null,
            ["BeneficiaryFirstName"] = BeneficiaryFirstNameEntry.Text,
            ["BeneficiaryMiddleInitial"] = BeneficiaryMiddleInitialEntry.Text,
            ["BeneficiaryLastName"] = BeneficiaryLastNameEntry.Text
        };
    }
}
