using Triple_S_AEP_MAUI_Forms.Models;
using Triple_S_AEP_MAUI_Forms.Services;

namespace Triple_S_AEP_MAUI_Forms;

public partial class DashboardPage : ContentPage
{
    private readonly EnrollmentDatabaseService _dbService = EnrollmentDatabaseService.Instance;
    private readonly DmsUploadService _dmsUploadService = new();
    private List<EnrollmentRecord> _allRecords = [];

    public DashboardPage()
    {
        InitializeComponent();
        DateFilterPicker.Date = DateTime.Today;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshData();
    }

    private void RefreshData()
    {
        try
        {
            _allRecords = _dbService.GetAllRecords().ToList();
            TotalRecordsLabel.Text = _dbService.GetTotalRecordsCount().ToString();
            
            var todayCount = _dbService.GetRecordsCountByDate(DateTime.Today);
            TodayRecordsLabel.Text = todayCount.ToString();

            var selectedDate = DateFilterPicker.Date;
            var recordsByDate = _dbService.GetRecordsByDate(selectedDate).ToList();
            RecordsCollectionView.ItemsSource = recordsByDate;
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Failed to load records: {ex.Message}", "OK");
        }
    }

    private void OnDateSelected(object? sender, DateChangedEventArgs e)
    {
        try
        {
            var recordsByDate = _dbService.GetRecordsByDate(e.NewDate).ToList();
            RecordsCollectionView.ItemsSource = recordsByDate;
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Failed to filter records: {ex.Message}", "OK");
        }
    }

    private async void OnViewAllRecordsClicked(object? sender, EventArgs e)
    {
        try
        {
            RecordsCollectionView.ItemsSource = _allRecords;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to view records: {ex.Message}", "OK");
        }
    }

    private async void OnViewFilesClicked(object? sender, EventArgs e)
    {
        try
        {
            if (sender is Button button && button.BindingContext is EnrollmentRecord record)
            {
                var files = new List<string>();
                
                if (!string.IsNullOrEmpty(record.EnrollmentFormPdfPath) && File.Exists(record.EnrollmentFormPdfPath))
                    files.Add($"Enrollment: {Path.GetFileName(record.EnrollmentFormPdfPath)}");
                    
                if (!string.IsNullOrEmpty(record.SoaFormPdfPath) && File.Exists(record.SoaFormPdfPath))
                    files.Add($"SOA: {Path.GetFileName(record.SoaFormPdfPath)}");
                    
                if (!string.IsNullOrEmpty(record.WorkingAgeSurveyPdfPath) && File.Exists(record.WorkingAgeSurveyPdfPath))
                    files.Add($"Survey: {Path.GetFileName(record.WorkingAgeSurveyPdfPath)}");

                var message = files.Count > 0 
                    ? string.Join("\n", files) 
                    : "No files found for this record";

                await DisplayAlert("Files for " + record.DisplayName, message, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to view files: {ex.Message}", "OK");
        }
    }

    private async void OnDeleteRecordClicked(object? sender, EventArgs e)
    {
        try
        {
            if (sender is Button button && button.BindingContext is EnrollmentRecord record)
            {
                var confirmed = await DisplayAlert(
                    "Delete Record",
                    $"Are you sure you want to delete {record.DisplayName}'s record?",
                    "Delete",
                    "Cancel");

                if (confirmed)
                {
                    _dbService.DeleteRecord(record.Id);
                    RefreshData();
                    await DisplayAlert("Success", "Record deleted successfully", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to delete record: {ex.Message}", "OK");
        }
    }

    private async void OnClearDatabaseClicked(object? sender, EventArgs e)
    {
        try
        {
            var confirmed = await DisplayAlert(
                "Clear Database",
                "Are you sure you want to delete ALL records? This cannot be undone.",
                "Clear All",
                "Cancel");

            if (confirmed)
            {
                var records = _dbService.GetAllRecords().ToList();
                foreach (var record in records)
                {
                    _dbService.DeleteRecord(record.Id);
                }

                RefreshData();
                await DisplayAlert("Success", "Database cleared successfully", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to clear database: {ex.Message}", "OK");
        }
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnUploadToDmsClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button || button.BindingContext is not EnrollmentRecord record)
            return;

        var results = new List<string>();
        var anyUploaded = false;

        if (record.SoaUploadStatus != EnrollmentUploadStatus.Uploaded && !string.IsNullOrEmpty(record.SoaFormPdfPath))
        {
            var bytes = await TryReadBytesAsync(record.SoaFormPdfPath);
            if (bytes != null)
            {
                var (ok, docId, msg) = await _dmsUploadService.UploadPdfAsync(DmsUploadService.DocumentTypeIdSoa, bytes);
                record.SoaUploadStatus = ok ? EnrollmentUploadStatus.Uploaded : EnrollmentUploadStatus.Failed;
                record.SoaFormDmsDocumentId = docId;
                results.Add($"SOA: {(ok ? "✓ Uploaded" : $"✗ {msg}")}");
                if (ok) anyUploaded = true;
            }
            else
            {
                results.Add("SOA: File not found on device");
            }
        }

        if (record.EnrollmentUploadStatus != EnrollmentUploadStatus.Uploaded && !string.IsNullOrEmpty(record.EnrollmentFormPdfPath))
        {
            var bytes = await TryReadBytesAsync(record.EnrollmentFormPdfPath);
            if (bytes != null)
            {
                var (ok, docId, msg) = await _dmsUploadService.UploadPdfAsync(DmsUploadService.DocumentTypeIdEnrollment, bytes);
                record.EnrollmentUploadStatus = ok ? EnrollmentUploadStatus.Uploaded : EnrollmentUploadStatus.Failed;
                record.EnrollmentFormDmsDocumentId = docId;
                results.Add($"Enrollment: {(ok ? "✓ Uploaded" : $"✗ {msg}")}");
                if (ok) anyUploaded = true;
            }
            else
            {
                results.Add("Enrollment: File not found on device");
            }
        }

        if (record.WorkingAgeSurveyUploadStatus != EnrollmentUploadStatus.Uploaded && !string.IsNullOrEmpty(record.WorkingAgeSurveyPdfPath))
        {
            var bytes = await TryReadBytesAsync(record.WorkingAgeSurveyPdfPath);
            if (bytes != null)
            {
                var (ok, docId, msg) = await _dmsUploadService.UploadPdfAsync(DmsUploadService.DocumentTypeIdWorkingAgeSurvey, bytes);
                record.WorkingAgeSurveyUploadStatus = ok ? EnrollmentUploadStatus.Uploaded : EnrollmentUploadStatus.Failed;
                record.WorkingAgeSurveyDmsDocumentId = docId;
                results.Add($"Survey: {(ok ? "✓ Uploaded" : $"✗ {msg}")}");
                if (ok) anyUploaded = true;
            }
            else
            {
                results.Add("Survey: File not found on device");
            }
        }

        if (results.Count == 0)
        {
            await DisplayAlert("DMS Upload", "All documents for this record are already uploaded.", "OK");
            return;
        }

        if (anyUploaded)
        {
            try
            {
                _dbService.AddOrUpdateRecord(record);
                RefreshData();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Database Error", $"Failed to update record: {ex.Message}", "OK");
            }
        }

        await DisplayAlert("DMS Upload — " + record.DisplayName, string.Join("\n", results), "OK");
    }

    private static async Task<byte[]?> TryReadBytesAsync(string path)
    {
        try
        {
            return await File.ReadAllBytesAsync(path);
        }
        catch
        {
            return null;
        }
    }
}
