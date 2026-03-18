using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Triple_S_Maui_AEP.Models;
using Triple_S_Maui_AEP.Services;

namespace Triple_S_Maui_AEP.ViewModels
{
    /// <summary>
    /// ViewModel for Dashboard (main hub after login)
    /// Displays agent statistics, quick actions, and recent activity
    /// </summary>
    public class DashboardViewModel : BaseViewModel
    {
        private string? _agentName;
        private int _monthlySOACount;
        private int _monthlyEnrollmentCount;
        private int _pendingSOACount;
        private int _completedSOACount;
        private bool _isLoading;
        private ICommand? _uploadSOACommand;
        private ICommand? _uploadEnrollmentCommand;

        public string? AgentName
        {
            get => _agentName;
            set => SetProperty(ref _agentName, value);
        }

        public int MonthlySOACount
        {
            get => _monthlySOACount;
            set => SetProperty(ref _monthlySOACount, value);
        }

        public int MonthlyEnrollmentCount
        {
            get => _monthlyEnrollmentCount;
            set => SetProperty(ref _monthlyEnrollmentCount, value);
        }

        public int PendingSOACount
        {
            get => _pendingSOACount;
            set => SetProperty(ref _pendingSOACount, value);
        }

        public int CompletedSOACount
        {
            get => _completedSOACount;
            set => SetProperty(ref _completedSOACount, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand UploadSOACommand
        {
            get => _uploadSOACommand ??= new Command<string>(async (soaNumber) => await UploadSOAAsync(soaNumber));
        }

        public ICommand UploadEnrollmentCommand
        {
            get => _uploadEnrollmentCommand ??= new Command<string>(async (enrollmentNumber) => await UploadEnrollmentAsync(enrollmentNumber));
        }

        public ObservableCollection<EnrollmentItemViewModel> Enrollments { get; } = new();
        public ObservableCollection<SOAItemViewModel> SOAItems { get; } = new();

        public DashboardViewModel()
        {
            AgentName = "Agent";
            MonthlySOACount = 0;
            MonthlyEnrollmentCount = 0;
            PendingSOACount = 0;
            CompletedSOACount = 0;
            
            // Load data asynchronously without blocking the constructor
            MainThread.BeginInvokeOnMainThread(async () => await LoadDashboardData());
        }

        public void Reload()
        {
            MainThread.BeginInvokeOnMainThread(async () => await LoadDashboardData());
        }

        public async Task UploadSOAAsync(string soaNumber)
        {
            try
            {
                IsLoading = true;
                
                // Find the SOA item in the UI list first
                var soaItem = SOAItems.FirstOrDefault(s => s.SOANumber == soaNumber);
                if (soaItem?.IsUploaded == true)
                {
                    // Already uploaded, ignore
                    return;
                }

                var soaRecords = await SOAService.GetActiveSOARecordsAsync();
                var soaRecord = soaRecords.FirstOrDefault(r => r.SOANumber == soaNumber);
                if (soaRecord == null || string.IsNullOrWhiteSpace(soaRecord.FilePath))
                {
                    return;
                }

                if (!File.Exists(soaRecord.FilePath))
                {
                    System.Diagnostics.Debug.WriteLine($"SOA file not found: {soaRecord.FilePath}");
                    return;
                }

                var fileContent = await File.ReadAllBytesAsync(soaRecord.FilePath);
                var fileBase64 = Convert.ToBase64String(fileContent);

                var dmsService = new DMSService();
                
                // Use the new API format with helper method
                var uploadRequest = DMSService.CreateSOAUploadRequest(
                    soaNumber: soaNumber,
                    base64Document: fileBase64,
                    firstName: soaRecord.FirstName,
                    lastName: soaRecord.LastName,
                    signatureDate: soaRecord.DateCreated,
                    agentUsername: AgentSessionService.CurrentAgentNPN ?? AgentSessionService.CurrentAgentName,
                    signatureMethod: "Digital"
                );

                var response = await dmsService.UploadDocumentAsync(uploadRequest);
                if (response.Success)
                {
                    if (soaItem != null)
                    {
                        soaItem.IsUploaded = true;
                    }

                    // Update database with upload status
                    await SOAService.UpdateUploadStatusAsync(soaNumber, true);

                    var updatedRecords = await SOAService.GetActiveSOARecordsAsync();
                    CompletedSOACount = updatedRecords.Count(r => r.IsUploaded);
                    PendingSOACount = MonthlySOACount - CompletedSOACount;
                    
                    System.Diagnostics.Debug.WriteLine($"SOA uploaded successfully: {soaNumber}, DocumentId: {response.DocumentId}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"SOA upload failed: {response.Message}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SOA upload error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task UploadEnrollmentAsync(string enrollmentNumber)
        {
            try
            {
                IsLoading = true;
                
                // Find the enrollment item in the UI list first
                var enrollmentItem = Enrollments.FirstOrDefault(e => e.EnrollmentNumber == enrollmentNumber);
                if (enrollmentItem?.IsUploaded == true)
                {
                    // Already uploaded, ignore
                    return;
                }

                var enrollmentRecords = await EnrollmentService.GetActiveEnrollmentRecordsAsync();
                var enrollmentRecord = enrollmentRecords.FirstOrDefault(r => r.EnrollmentNumber == enrollmentNumber);
                if (enrollmentRecord == null || string.IsNullOrWhiteSpace(enrollmentRecord.FilePath))
                {
                    return;
                }

                if (!File.Exists(enrollmentRecord.FilePath))
                {
                    System.Diagnostics.Debug.WriteLine($"Enrollment file not found: {enrollmentRecord.FilePath}");
                    return;
                }

                var fileContent = await File.ReadAllBytesAsync(enrollmentRecord.FilePath);
                var fileBase64 = Convert.ToBase64String(fileContent);

                var dmsService = new DMSService();
                
                // Use the new API format with helper method for Enrollment (DocumentTypeId 842)
                var uploadRequest = DMSService.CreateEnrollmentUploadRequest(
                    enrollmentNumber: enrollmentNumber,
                    base64Document: fileBase64,
                    firstName: enrollmentRecord.FirstName,
                    lastName: enrollmentRecord.LastName,
                    signatureDate: enrollmentRecord.DateCreated,
                    agentUsername: AgentSessionService.CurrentAgentNPN ?? AgentSessionService.CurrentAgentName,
                    signatureMethod: "Digital"
                );

                var response = await dmsService.UploadDocumentAsync(uploadRequest);
                if (response.Success)
                {
                    if (enrollmentItem != null)
                    {
                        enrollmentItem.IsUploaded = true;
                    }

                    // Update database with upload status
                    await EnrollmentService.UpdateUploadStatusAsync(enrollmentNumber, true);
                    
                    System.Diagnostics.Debug.WriteLine($"Enrollment uploaded successfully: {enrollmentNumber}, DocumentId: {response.DocumentId}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Enrollment upload failed: {response.Message}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Enrollment upload error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadDashboardData()
        {
            try
            {
                IsLoading = true;

                AgentName = AgentSessionService.CurrentAgentName ?? AgentSessionService.CurrentAgentNPN ?? "Agent";

                var activeEnrollmentRecords = await EnrollmentService.GetActiveEnrollmentRecordsAsync();
                var activeSoaRecords = await SOAService.GetActiveSOARecordsAsync();

                MonthlyEnrollmentCount = activeEnrollmentRecords.Count;
                MonthlySOACount = activeSoaRecords.Count;
                CompletedSOACount = activeSoaRecords.Count(r => r.IsUploaded);
                PendingSOACount = MonthlySOACount - CompletedSOACount;

                Enrollments.Clear();
                foreach (var record in activeEnrollmentRecords.OrderByDescending(r => r.DateCreated))
                {
                    Enrollments.Add(new EnrollmentItemViewModel(this)
                    {
                        EnrollmentNumber = record.EnrollmentNumber,
                        BeneficiaryName = record.BeneficiaryName,
                        Status = record.IsUploaded ? "Uploaded" : "Pending",
                        DateCreated = record.DateCreated,
                        IsUploaded = record.IsUploaded
                    });
                }

                SOAItems.Clear();
                foreach (var record in activeSoaRecords.OrderByDescending(r => r.DateCreated))
                {
                    SOAItems.Add(new SOAItemViewModel(this)
                    {
                        SOANumber = record.SOANumber,
                        BeneficiaryName = record.BeneficiaryName,
                        Status = record.IsUploaded ? "Uploaded" : "Pending",
                        DateCreated = record.DateCreated,
                        IsUploaded = record.IsUploaded
                    });
                }

                await Task.Delay(200);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Dashboard load error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    /// <summary>
    /// View model item for enrollment list
    /// </summary>
    public class EnrollmentItemViewModel : INotifyPropertyChanged
    {
        private bool _isUploaded;
        private string _status = string.Empty;
        private ICommand? _uploadCommand;
        private readonly DashboardViewModel _parentViewModel;

        public EnrollmentItemViewModel(DashboardViewModel parentViewModel)
        {
            _parentViewModel = parentViewModel;
        }

        public string EnrollmentNumber { get; set; } = string.Empty;
        public string BeneficiaryName { get; set; } = string.Empty;
        
        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StatusColor));
                }
            }
        }
        
        public DateTime DateCreated { get; set; }
        
        public bool IsUploaded
        {
            get => _isUploaded;
            set
            {
                if (_isUploaded != value)
                {
                    _isUploaded = value;
                    Status = value ? "Uploaded" : "Pending";
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UploadButtonText));
                    OnPropertyChanged(nameof(UploadButtonColor));
                    OnPropertyChanged(nameof(StatusColor));
                    OnPropertyChanged(nameof(IsUploadEnabled));
                }
            }
        }

        public ICommand UploadCommand
        {
            get => _uploadCommand ??= new Command(async () => 
            {
                System.Diagnostics.Debug.WriteLine($"Enrollment upload command triggered for: {EnrollmentNumber}");
                await _parentViewModel.UploadEnrollmentAsync(EnrollmentNumber);
            });
        }

        public string UploadButtonText => IsUploaded ? "✓" : "↑";
        public Color UploadButtonColor => IsUploaded ? Color.FromArgb("#27AE60") : Color.FromArgb("#1976D2");
        public Color StatusColor => IsUploaded ? Color.FromArgb("#27AE60") : Color.FromArgb("#F57C00");
        public bool IsUploadEnabled => !IsUploaded;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// View model item for SOA list
    /// </summary>
    public class SOAItemViewModel : INotifyPropertyChanged
    {
        private bool _isUploaded;
        private string _status = string.Empty;
        private ICommand? _uploadCommand;
        private readonly DashboardViewModel _parentViewModel;

        public SOAItemViewModel(DashboardViewModel parentViewModel)
        {
            _parentViewModel = parentViewModel;
        }

        public string SOANumber { get; set; } = string.Empty;
        public string BeneficiaryName { get; set; } = string.Empty;
        
        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StatusColor));
                }
            }
        }
        
        public DateTime DateCreated { get; set; }
        
        public bool IsUploaded
        {
            get => _isUploaded;
            set
            {
                if (_isUploaded != value)
                {
                    _isUploaded = value;
                    Status = value ? "Uploaded" : "Pending";
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UploadButtonText));
                    OnPropertyChanged(nameof(UploadButtonColor));
                    OnPropertyChanged(nameof(StatusColor));
                    OnPropertyChanged(nameof(IsUploadEnabled));
                }
            }
        }

        public ICommand UploadCommand
        {
            get => _uploadCommand ??= new Command(async () => 
            {
                System.Diagnostics.Debug.WriteLine($"SOA upload command triggered for: {SOANumber}");
                await _parentViewModel.UploadSOAAsync(SOANumber);
            });
        }

        public string UploadButtonText => IsUploaded ? "✓" : "↑";
        public Color UploadButtonColor => IsUploaded ? Color.FromArgb("#27AE60") : Color.FromArgb("#1976D2");
        public Color StatusColor => IsUploaded ? Color.FromArgb("#27AE60") : Color.FromArgb("#F57C00");
        public bool IsUploadEnabled => !IsUploaded;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
