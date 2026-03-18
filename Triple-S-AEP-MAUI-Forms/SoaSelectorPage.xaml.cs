using Triple_S_AEP_MAUI_Forms.Models;
using Triple_S_AEP_MAUI_Forms.Services;
using LiteDB;

namespace Triple_S_AEP_MAUI_Forms;

public partial class SoaSelectorPage : ContentPage
{
    private readonly EnrollmentDatabaseService _dbService = EnrollmentDatabaseService.Instance;
    private readonly TaskCompletionSource<EnrollmentRecord?> _result = new();
    private List<EnrollmentRecord> _allSoaRecords = [];
    private EnrollmentRecord? _selectedRecord;

    public SoaSelectorPage()
    {
        InitializeComponent();
        LoadSoaRecords();
    }

    public Task<EnrollmentRecord?> Result => _result.Task;

    private void LoadSoaRecords()
    {
        try
        {
            _allSoaRecords = _dbService.GetUnlinkedSoaRecords().ToList();
            SoaRecordsCollectionView.ItemsSource = _allSoaRecords;
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Failed to load SOA records: {ex.Message}", "OK");
        }
    }

    private void OnSoaSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(searchText))
        {
            SoaRecordsCollectionView.ItemsSource = _allSoaRecords;
            return;
        }

        try
        {
            var filtered = _dbService.SearchSoaRecords(searchText).ToList();
            SoaRecordsCollectionView.ItemsSource = filtered;
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Search failed: {ex.Message}", "OK");
        }
    }

    private void OnSelectSoaClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is EnrollmentRecord record)
        {
            _selectedRecord = record;
            _result.TrySetResult(record);
            Navigation.PopModalAsync();
        }
    }

    private async void OnSkipClicked(object? sender, EventArgs e)
    {
        _result.TrySetResult(null);
        await Navigation.PopModalAsync();
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        _result.TrySetResult(null);
        await Navigation.PopModalAsync();
    }
}
