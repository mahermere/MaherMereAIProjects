using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Triple_S_Maui_AEP.ViewModels;
using Triple_S_Maui_AEP.Services;

namespace Triple_S_Maui_AEP.Views
{
    public partial class DashboardPage : ContentPage
    {
        private readonly DashboardViewModel _viewModel = new DashboardViewModel();
        private bool _soasCollapsed = false;
        private bool _enrollmentsCollapsed = false;

        public DashboardPage()
        {
            try
            {
                InitializeComponent();
                BindingContext = _viewModel;

                InitializeLanguagePicker();

                // Set all UI text from language resources
                SetLocalizedText();
                Services.LanguageService.Instance.LanguageChanged += _ => SetLocalizedText();
                
                // Populate SOAs and Enrollments
                _ = LoadDashboardRecordsAsync();
            }
            catch (Exception ex)
            {
                // Show error in the UI if possible
                if (this.FindByName("ErrorMessageLabel") is Label errorLabel)
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

        private void PopulateSOARecords()
        {
            _ = PopulateSOARecordsAsync();
        }

        private View CreateSOAItemView(SOAService.SOARecord record)
        {
            var grid = new Grid 
            { 
                ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                ColumnSpacing = 12
            };
            
            // Left column - SOA info
            var infoStack = new VerticalStackLayout { Spacing = 4 };
            infoStack.Add(new Label 
            { 
                Text = record.SOANumber, 
                FontSize = 13, 
                FontAttributes = FontAttributes.Bold, 
                TextColor = Color.FromArgb("#2C3E50") 
            });
            infoStack.Add(new Label 
            { 
                Text = record.BeneficiaryName, 
                FontSize = 12, 
                TextColor = Color.FromArgb("#7F8C8D") 
            });
            infoStack.Add(new Label 
            { 
                Text = record.DateCreated.ToString("MM/dd/yyyy"), 
                FontSize = 11, 
                TextColor = Color.FromArgb("#95A5A6") 
            });
            
            grid.Add(infoStack, 0, 0);
            
            // Right column - Status, View PDF Button, and Upload Button
            var rightStack = new VerticalStackLayout { Spacing = 4, VerticalOptions = LayoutOptions.Center };
            
            var statusColor = record.IsUploaded ? Color.FromArgb("#27AE60") : Color.FromArgb("#F57C00");
            var statusText = record.IsUploaded ? "Uploaded" : "Pending";
            rightStack.Add(new Label 
            { 
                Text = statusText, 
                FontSize = 11, 
                FontAttributes = FontAttributes.Bold, 
                TextColor = statusColor,
                HorizontalOptions = LayoutOptions.End 
            });
            
            // Button container - horizontal layout for View and Upload buttons
            var buttonStack = new HorizontalStackLayout { Spacing = 8, HorizontalOptions = LayoutOptions.End };
            
            // View PDF Button
            var viewPdfBtn = new Button
            {
                Text = "📄",
                BackgroundColor = Color.FromArgb("#607D8B"),
                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                HeightRequest = 36,
                WidthRequest = 36,
                CornerRadius = 18,
                Padding = new Thickness(0),
                IsEnabled = !string.IsNullOrEmpty(record.FilePath) && File.Exists(record.FilePath),
                HorizontalOptions = LayoutOptions.End,
                CommandParameter = record.FilePath
            };
            viewPdfBtn.Clicked += OnViewSOAPdfClicked;
            
            // Upload Button
            var uploadBtn = new Button
            {
                Text = record.IsUploaded ? "✓" : "↑",
                BackgroundColor = record.IsUploaded ? Color.FromArgb("#27AE60") : Color.FromArgb("#1976D2"),
                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                HeightRequest = 36,
                WidthRequest = 36,
                CornerRadius = 18,
                Padding = new Thickness(0),
                IsEnabled = !record.IsUploaded,
                HorizontalOptions = LayoutOptions.End,
                CommandParameter = record.SOANumber
            };
            uploadBtn.Clicked += OnUploadSOAClicked;
            
            buttonStack.Add(viewPdfBtn);
            buttonStack.Add(uploadBtn);
            rightStack.Add(buttonStack);
            
            grid.Add(rightStack, 1, 0);
            
            return grid;
        }

        private void PopulateEnrollmentRecords()
        {
            _ = PopulateEnrollmentRecordsAsync();
        }

        private View CreateEnrollmentItemView(EnrollmentService.EnrollmentRecord record)
        {
            var grid = new Grid 
            { 
                ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                ColumnSpacing = 12
            };
            
            // Left column - Enrollment info
            var infoStack = new VerticalStackLayout { Spacing = 4 };
            infoStack.Add(new Label 
            { 
                Text = record.EnrollmentNumber, 
                FontSize = 13, 
                FontAttributes = FontAttributes.Bold, 
                TextColor = Color.FromArgb("#2C3E50") 
            });
            infoStack.Add(new Label 
            { 
                Text = record.BeneficiaryName, 
                FontSize = 12, 
                TextColor = Color.FromArgb("#7F8C8D") 
            });
            infoStack.Add(new Label 
            { 
                Text = record.DateCreated.ToString("MM/dd/yyyy"), 
                FontSize = 11, 
                TextColor = Color.FromArgb("#95A5A6") 
            });
            
            grid.Add(infoStack, 0, 0);
            
            // Right column - Status, View PDF Button, and Upload Button
            var rightStack = new VerticalStackLayout { Spacing = 4, VerticalOptions = LayoutOptions.Center };
            
            var statusColor = record.IsUploaded ? Color.FromArgb("#27AE60") : Color.FromArgb("#F57C00");
            var statusText = record.IsUploaded ? "Uploaded" : "Pending";
            rightStack.Add(new Label 
            { 
                Text = statusText, 
                FontSize = 11, 
                FontAttributes = FontAttributes.Bold, 
                TextColor = statusColor,
                HorizontalOptions = LayoutOptions.End 
            });
            
            // Button container - horizontal layout for View and Upload buttons
            var buttonStack = new HorizontalStackLayout { Spacing = 8, HorizontalOptions = LayoutOptions.End };
            
            // View PDF Button
            var viewPdfBtn = new Button
            {
                Text = "📄",
                BackgroundColor = Color.FromArgb("#607D8B"),
                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                HeightRequest = 36,
                WidthRequest = 36,
                CornerRadius = 18,
                Padding = new Thickness(0),
                IsEnabled = !string.IsNullOrEmpty(record.FilePath) && File.Exists(record.FilePath),
                HorizontalOptions = LayoutOptions.End,
                CommandParameter = record.FilePath
            };
            viewPdfBtn.Clicked += OnViewEnrollmentPdfClicked;
            
            // Upload Button
            var uploadBtn = new Button
            {
                Text = record.IsUploaded ? "✓" : "↑",
                BackgroundColor = record.IsUploaded ? Color.FromArgb("#27AE60") : Color.FromArgb("#1976D2"),
                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                HeightRequest = 36,
                WidthRequest = 36,
                CornerRadius = 18,
                Padding = new Thickness(0),
                IsEnabled = !record.IsUploaded,
                HorizontalOptions = LayoutOptions.End,
                CommandParameter = record.EnrollmentNumber
            };
            uploadBtn.Clicked += OnUploadEnrollmentClicked;
            
            buttonStack.Add(viewPdfBtn);
            buttonStack.Add(uploadBtn);
            rightStack.Add(buttonStack);
            
            grid.Add(rightStack, 1, 0);
            
            return grid;
        }

        private void SetLocalizedText()
        {
            var isEnglish = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English;

            if (LanguageLabel != null) LanguageLabel.Text = isEnglish ? "Language:" : "Idioma:";
            if (PageTitle != null) PageTitle.Text = isEnglish ? "Agent Dashboard" : "Panel del Agente";
            if (StatisticsLabel != null) StatisticsLabel.Text = isEnglish ? "Statistics" : "Estadísticas";
            if (MonthlyEnrollmentsLabel != null) MonthlyEnrollmentsLabel.Text = isEnglish ? "Monthly Enrollments" : "Inscripciones Mensuales";
            if (MonthlySOALabel != null) MonthlySOALabel.Text = isEnglish ? "Monthly SOAs" : "SOAs Mensuales";
            if (CompletedSOALabel != null) CompletedSOALabel.Text = isEnglish ? "Completed SOAs" : "SOAs Completados";
            if (PendingSOALabel != null) PendingSOALabel.Text = isEnglish ? "Pending SOAs" : "SOAs Pendientes";

            var soasLabel = FindByName("SOAsListLabel") as Label;
            if (soasLabel != null) soasLabel.Text = isEnglish ? "Signature of Authority (SOAs)" : "Firma de Autoridad (SOAs)";

            if (EnrollmentsListLabel != null) EnrollmentsListLabel.Text = isEnglish ? "Recent Enrollments" : "Inscripciones Recientes";
            if (NewEnrollmentButton != null) NewEnrollmentButton.Text = isEnglish ? "New Enrollment" : "Nueva Inscripción";
            if (NewTripleSEnrollmentButton != null) NewTripleSEnrollmentButton.Text = isEnglish ? "New Triple-S Enrollment" : "Nueva Inscripción Triple-S";
            if (NewSOAButton != null) NewSOAButton.Text = isEnglish ? "New SOA" : "Nuevo SOA";
            if (RefreshButton != null) RefreshButton.Text = isEnglish ? "Refresh" : "Actualizar";
            if (LogoutButton != null) LogoutButton.Text = isEnglish ? "Logout" : "Cerrar Sesión";
        }

        private void OnToggleSOAsClicked(object? sender, EventArgs e)
        {
            _soasCollapsed = !_soasCollapsed;
            var soasView = FindByName("SOAsCollectionView") as VerticalStackLayout;
            if (soasView != null) soasView.IsVisible = !_soasCollapsed;

            var soasBtn = FindByName("ToggleSOAsButton") as Button;
            if (soasBtn != null) soasBtn.Text = _soasCollapsed ? "▶" : "▼";
        }

        private void OnToggleEnrollmentsClicked(object? sender, EventArgs e)
        {
            _enrollmentsCollapsed = !_enrollmentsCollapsed;
            var enrollView = FindByName("EnrollmentsCollectionView") as VerticalStackLayout;
            if (enrollView != null) enrollView.IsVisible = !_enrollmentsCollapsed;

            var enrollBtn = FindByName("ToggleEnrollmentsButton") as Button;
            if (enrollBtn != null) enrollBtn.Text = _enrollmentsCollapsed ? "▶" : "▼";
        }

        private async void OnNewEnrollmentClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///TripleSEnrollmentWizardPage");
        }

        private async void OnNewTripleSEnrollmentClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///TripleSEnrollmentWizardPage");
        }

        private async void OnNewSOAClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///SOAWizardPage");
        }

        private async void OnRefreshClicked(object? sender, EventArgs e)
        {
            _viewModel.Reload();
            await LoadDashboardRecordsAsync();
        }

        private async void OnLogoutClicked(object? sender, EventArgs e)
        {
            bool confirm = await DisplayAlert(
                Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "Confirm Logout" : "Confirmar Cierre de Sesión",
                Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "Are you sure you want to logout?" : "¿Está seguro de que desea cerrar sesión?",
                Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "Yes" : "Sí",
                Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "No" : "No"
            );

            if (confirm)
            {
                Services.AgentSessionService.ClearSession();
                await Shell.Current.GoToAsync("///AgentLoginPage");
            }
        }

        private async void OnUploadSOAClicked(object? sender, EventArgs e)
        {
            if (sender is not Button btn || btn.CommandParameter is not string soaNumber)
                return;

            try
            {
                // Disable button and show loading state
                btn.IsEnabled = false;
                btn.Text = "⏳";
                btn.BackgroundColor = Color.FromArgb("#95A5A6"); // Gray color

                System.Diagnostics.Debug.WriteLine($"Uploading SOA: {soaNumber}");
                await _viewModel.UploadSOAAsync(soaNumber);
                
                // Success - refresh list to show green checkmark
                PopulateSOARecords();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error uploading SOA: {ex.Message}");
                
                // Re-enable button on error and restore original state
                btn.IsEnabled = true;
                btn.Text = "↑";
                btn.BackgroundColor = Color.FromArgb("#1976D2");
                
                await DisplayAlert("Error", $"Upload failed: {ex.Message}", "OK");
            }
        }

        private async void OnUploadEnrollmentClicked(object? sender, EventArgs e)
        {
            if (sender is not Button btn || btn.CommandParameter is not string enrollmentNumber)
                return;

            try
            {
                // Disable button and show loading state
                btn.IsEnabled = false;
                btn.Text = "⏳";
                btn.BackgroundColor = Color.FromArgb("#95A5A6"); // Gray color

                System.Diagnostics.Debug.WriteLine($"Uploading Enrollment: {enrollmentNumber}");
                await _viewModel.UploadEnrollmentAsync(enrollmentNumber);
                
                // Success - refresh list to show green checkmark
                PopulateEnrollmentRecords();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error uploading Enrollment: {ex.Message}");
                
                // Re-enable button on error and restore original state
                btn.IsEnabled = true;
                btn.Text = "↑";
                btn.BackgroundColor = Color.FromArgb("#1976D2");
                
                await DisplayAlert("Error", $"Upload failed: {ex.Message}", "OK");
            }
        }

        private async void OnViewSOAPdfClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.CommandParameter is string filePath)
                {
                    if (string.IsNullOrEmpty(filePath))
                    {
                        await DisplayAlert(
                            Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "No PDF" : "Sin PDF",
                            Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "PDF file not found." : "Archivo PDF no encontrado.",
                            "OK"
                        );
                        return;
                    }

                    if (!File.Exists(filePath))
                    {
                        await DisplayAlert(
                            Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "File Not Found" : "Archivo No Encontrado",
                            Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "The PDF file no longer exists." : "El archivo PDF ya no existe.",
                            "OK"
                        );
                        return;
                    }

                    System.Diagnostics.Debug.WriteLine($"Opening SOA PDF: {filePath}");
                    
                    // Use Launcher to open the PDF in the default viewer
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(filePath)
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening SOA PDF: {ex.Message}");
                await DisplayAlert(
                    Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "Error" : "Error",
                    Services.LanguageService.Instance.CurrentLanguage == Models.Language.English 
                        ? $"Could not open PDF: {ex.Message}" 
                        : $"No se pudo abrir el PDF: {ex.Message}",
                    "OK"
                );
            }
        }

        private async void OnViewEnrollmentPdfClicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.CommandParameter is string filePath)
                {
                    if (string.IsNullOrEmpty(filePath))
                    {
                        await DisplayAlert(
                            Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "No PDF" : "Sin PDF",
                            Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "PDF file not found." : "Archivo PDF no encontrado.",
                            "OK"
                        );
                        return;
                    }

                    if (!File.Exists(filePath))
                    {
                        await DisplayAlert(
                            Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "File Not Found" : "Archivo No Encontrado",
                            Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "The PDF file no longer exists." : "El archivo PDF ya no existe.",
                            "OK"
                        );
                        return;
                    }

                    System.Diagnostics.Debug.WriteLine($"Opening Enrollment PDF: {filePath}");
                    
                    // Use Launcher to open the PDF in the default viewer
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(filePath)
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening Enrollment PDF: {ex.Message}");
                await DisplayAlert(
                    Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "Error" : "Error",
                    Services.LanguageService.Instance.CurrentLanguage == Models.Language.English 
                        ? $"Could not open PDF: {ex.Message}" 
                        : $"No se pudo abrir el PDF: {ex.Message}",
                    "OK"
                );
            }
        }

        private async Task LoadDashboardRecordsAsync()
        {
            try
            {
                // Populate SOAs and Enrollments
                await PopulateSOARecordsAsync();
                await PopulateEnrollmentRecordsAsync();
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during loading
                System.Diagnostics.Debug.WriteLine($"Error loading dashboard records: {ex.Message}");
                
                if (this.FindByName("ErrorMessageLabel") is Label errorLabel)
                {
                    errorLabel.Text = $"Error: {ex.Message}";
                    errorLabel.IsVisible = true;
                }
            }
        }

        private async Task PopulateSOARecordsAsync()
        {
            var soasContainer = FindByName("SOAsCollectionView") as VerticalStackLayout;
            if (soasContainer == null) return;

            soasContainer.Clear();

            var soaRecords = await SOAService.GetActiveSOARecordsAsync();
            foreach (var record in soaRecords.OrderByDescending(r => r.DateCreated))
            {
                var itemContainer = new Border
                {
                    BackgroundColor = Colors.White,
                    Stroke = Color.FromArgb("#E0E0E0"),
                    StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(8) },
                    Padding = new Thickness(12),
                    Margin = new Thickness(0, 4),
                    Content = CreateSOAItemView(record)
                };

                soasContainer.Add(itemContainer);
            }
        }

        private async Task PopulateEnrollmentRecordsAsync()
        {
            var enrollmentsContainer = FindByName("EnrollmentsCollectionView") as VerticalStackLayout;
            if (enrollmentsContainer == null) return;

            enrollmentsContainer.Clear();

            var enrollmentRecords = await EnrollmentService.GetActiveEnrollmentRecordsAsync();
            foreach (var record in enrollmentRecords.OrderByDescending(r => r.DateCreated))
            {
                var itemContainer = new Border
                {
                    BackgroundColor = Colors.White,
                    Stroke = Color.FromArgb("#E0E0E0"),
                    StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(8) },
                    Padding = new Thickness(12),
                    Margin = new Thickness(0, 4),
                    Content = CreateEnrollmentItemView(record)
                };

                enrollmentsContainer.Add(itemContainer);
            }
        }
    }
}
