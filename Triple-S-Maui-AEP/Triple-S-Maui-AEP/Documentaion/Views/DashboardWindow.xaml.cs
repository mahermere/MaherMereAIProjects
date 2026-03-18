using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Maui.Storage;
using TripleS.SOA.AEP.UI.ViewModels;
using TripleS.SOA.AEP.UI.Platforms;
using TripleSPOC.Services;

namespace TripleS.SOA.AEP.UI.Views
{
    public partial class DashboardWindow : ContentPage
    {
        private static readonly string DailyCountsFile = Path.Combine(FileSystem.AppDataDirectory, "dashboard_counts.csv");
        private int _todaySOACount = 0;
        private int _todayEnrollmentCount = 0;
        private DateTime _today = DateTime.Today;

        public System.Collections.Generic.IReadOnlyList<TripleS.SOA.AEP.UI.Services.SOANumberService.SOARecord> SOARecords => TripleS.SOA.AEP.UI.Services.SOANumberService.ActiveSOARecords;

        public DashboardWindow()
        {
            BindingContext = this;
            InitializeComponent();

            // Sync SOANumberService with CSV on dashboard load
            LoadSOARecordsFromCsv();

            TripleSPOC.Services.LanguageService.Instance.LanguageChanged -= OnLanguageChangedFromService;
            TripleSPOC.Services.LanguageService.Instance.LanguageChanged += OnLanguageChangedFromService;

            // Initialize language picker items and set index only once
            InitializeLanguagePicker();

            // Set all UI text from resources (do not re-initialize picker here)
            SetLocalizedText();

            // Load today's counts from CSV
            LoadTodayCounts();
            UpdateCountLabels();
        }

        private readonly string soaCsvPath = Path.Combine(FileSystem.AppDataDirectory, "soa_records.csv");

        private void LoadSOARecordsFromCsv()
        {
            // Clear and reload SOANumberService from CSV
            TripleS.SOA.AEP.UI.Services.SOANumberService.Clear();
            var csvRecords = TripleS.Utilities.CsvDataUtility.LoadCsv(soaCsvPath, fields =>
                new TripleS.SOA.AEP.UI.Services.SOANumberService.SOARecord {
                    SOANumber = fields.Length > 0 ? fields[0] : string.Empty,
                    FirstName = fields.Length > 1 ? fields[1] : string.Empty,
                    LastName = fields.Length > 2 ? fields[2] : string.Empty,
                    DateCreated = fields.Length > 3 && System.DateTime.TryParse(fields[3], out var dt) ? dt : System.DateTime.MinValue
                });
            foreach (var rec in csvRecords)
                TripleS.SOA.AEP.UI.Services.SOANumberService.AddSOARecord(rec);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = this; // Refresh binding so SOARecords updates
        }

        private void InitializeLanguagePicker()
        {
            Picker newPicker = LanguagePicker ?? throw new InvalidOperationException("LanguagePicker not found in XAML.");

            newPicker.SelectedIndexChanged -= OnLanguageChanged;
            newPicker.Items.Clear();
            newPicker.Items.Add(TripleSPOC.Resources.Localization.AppResources.English);
            newPicker.Items.Add(TripleSPOC.Resources.Localization.AppResources.Spanish);
            var lang = TripleSPOC.Services.LanguageService.Instance.CurrentLanguage;
            newPicker.SelectedIndex = lang == TripleSPOC.Models.Language.English ? 0 : 1;
            newPicker.SelectedIndexChanged += OnLanguageChanged;
        }

        private void OnLanguageChangedFromService(TripleSPOC.Models.Language lang)
        {
            // Update picker index if needed
            if (LanguagePicker.Items.Count >= 2)
                LanguagePicker.SelectedIndex = lang == TripleSPOC.Models.Language.English ? 0 : 1;
            SetLocalizedText();
        }

        private void SetLocalizedText()
        {
            LanguageLabel.Text = TripleSPOC.Resources.Localization.AppResources.Language;
            // Update picker item text for localization, but do not clear or re-add items
            if (LanguagePicker.Items.Count == 2)
            {
                LanguagePicker.SelectedIndexChanged -= OnLanguageChanged;
                LanguagePicker.Items[0] = TripleSPOC.Resources.Localization.AppResources.English;
                LanguagePicker.Items[1] = TripleSPOC.Resources.Localization.AppResources.Spanish;
                LanguagePicker.SelectedIndexChanged += OnLanguageChanged;
            }
            QuickActionsLabel.Text = TripleSPOC.Resources.Localization.AppResources.QuickActions ?? "Quick Actions";
            HeaderTitleLabel.Text = TripleSPOC.Resources.Localization.AppResources.DashboardHeaderTitle ?? "Triple-S SOA/AEP";
            HeaderSubtitleLabel.Text = TripleSPOC.Resources.Localization.AppResources.DashboardHeaderSubtitle ?? "Agent Portal - Sales & Broker Activities";
            NewSOAButton.Text = TripleSPOC.Resources.Localization.AppResources.StartNewSOA ?? "Start New SOA";
            NewEnrollmentButton.Text = TripleSPOC.Resources.Localization.AppResources.StartNewEnrollment ?? "Start New Enrollment";
            ExitButton.Text = TripleSPOC.Resources.Localization.AppResources.Exit ?? "Exit";
            // Activity summary
            var activitySummaryLabel = this.FindByName<Label>("SOACountLabel");
            if (activitySummaryLabel != null)
                activitySummaryLabel.Text = string.Format(TripleSPOC.Resources.Localization.AppResources.SOACountToday ?? "SOAs Created Today: {0}", _todaySOACount);
            var enrollmentSummaryLabel = this.FindByName<Label>("EnrollmentCountLabel");
            if (enrollmentSummaryLabel != null)
                enrollmentSummaryLabel.Text = string.Format(TripleSPOC.Resources.Localization.AppResources.EnrollmentCountToday ?? "Enrollments Created Today: {0}", _todayEnrollmentCount);
            var todaysActivityLabel = this.FindByName<Label>("Today's Activity");
            if (todaysActivityLabel != null)
                todaysActivityLabel.Text = TripleSPOC.Resources.Localization.AppResources.TodaysActivity ?? "Today's Activity";
            UpdateCountLabels();
        }

        private void UpdateCountLabels()
        {
            SOACountLabel.Text = string.Format(TripleSPOC.Resources.Localization.AppResources.SOACountToday ?? "SOAs Created Today: {0}", _todaySOACount);
            EnrollmentCountLabel.Text = string.Format(TripleSPOC.Resources.Localization.AppResources.EnrollmentCountToday ?? "Enrollments Created Today: {0}", _todayEnrollmentCount);
        }

        private void LoadTodayCounts()
        {
            _todaySOACount = 0;
            _todayEnrollmentCount = 0;
            if (!File.Exists(DailyCountsFile))
                return;
            try
            {
                var lines = File.ReadAllLines(DailyCountsFile);
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 3 && DateTime.TryParse(parts[0], out var date))
                    {
                        if (date.Date == _today)
                        {
                            _todaySOACount = int.TryParse(parts[1], out var soa) ? soa : 0;
                            _todayEnrollmentCount = int.TryParse(parts[2], out var enr) ? enr : 0;
                            break;
                        }
                    }
                }
            }
            catch { /* Ignore file errors for now */ }
        }

        private void SaveTodayCounts()
        {
            var lines = new List<string>();
            if (File.Exists(DailyCountsFile))
                lines.AddRange(File.ReadAllLines(DailyCountsFile));
            bool found = false;
            for (int i = 0; i < lines.Count; i++)
            {
                var parts = lines[i].Split(',');
                if (parts.Length == 3 && DateTime.TryParse(parts[0], out var date) && date.Date == _today)
                {
                    lines[i] = $"{_today:yyyy-MM-dd},{_todaySOACount},{_todayEnrollmentCount}";
                    found = true;
                    break;
                }
            }
            if (!found)
                lines.Add($"{_today:yyyy-MM-dd},{_todaySOACount},{_todayEnrollmentCount}");
            File.WriteAllLines(DailyCountsFile, lines);
        }

        private void IncrementSOACount()
        {
            _todaySOACount++;
            SaveTodayCounts();
            UpdateCountLabels();
        }

        private void IncrementEnrollmentCount()
        {
            _todayEnrollmentCount++;
            SaveTodayCounts();
            UpdateCountLabels();
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            if (LanguagePicker.SelectedIndex == 0)
                TripleSPOC.Services.LanguageService.Instance.CurrentLanguage = TripleSPOC.Models.Language.English;
            else if (LanguagePicker.SelectedIndex == 1)
                TripleSPOC.Services.LanguageService.Instance.CurrentLanguage = TripleSPOC.Models.Language.Spanish;
        }

        private async void NewSOAButton_Click(object sender, EventArgs e)
        {
            try
            {
                var pdfOpener = new PdfOpener();
                await Navigation.PushAsync(new SOAWizardWindow(pdfOpener));
                IncrementSOACount();
            }
            catch (Exception ex)
            {
                await DisplayAlert("SOA Launch Error", $"An error occurred: {ex.Message}\n{ex.StackTrace}", "OK");
            }
        }

        private async void NewEnrollmentButton_Click(object sender, EventArgs e)
        {
            // Specify constructor parameters to resolve ambiguity, e.g. use default or provide required arguments
            await Navigation.PushAsync(new EnrollmentWizardPage(/* specify parameters if needed */));
            IncrementEnrollmentCount();
        }

        private async void LogoutButton_Click(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirm Logout", "Logout from Agent Portal?", "Yes", "No");
            if (confirm)
            {
                await Navigation.PushAsync(new AgentLoginWindow());
                Navigation.RemovePage(this);
            }
        }

        private void OnExitClicked(object sender, EventArgs e)
        {
#if WINDOWS
            Microsoft.UI.Xaml.Window.Current.Close();
#else
            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
#endif
        }
    }
}
