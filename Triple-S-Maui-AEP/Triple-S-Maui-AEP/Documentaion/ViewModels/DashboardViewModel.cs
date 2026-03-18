using TripleS.SOA.AEP.Models;
using TripleS.SOA.AEP.Models.ViewModels;

namespace TripleS.SOA.AEP.UI.ViewModels
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

        public DashboardViewModel()
        {
            AgentName = "Juan Perez (NPN: 123456789)";
            MonthlySOACount = 24;
            MonthlyEnrollmentCount = 18;
            PendingSOACount = 3;
            CompletedSOACount = 21;
            LoadDashboardData();
        }

        private async void LoadDashboardData()
        {
            try
            {
                IsLoading = true;

                // TODO: Load actual data from services
                // - Recent SOA documents
                // - Recent enrollments
                // - Agent statistics
                // - Pending tasks

                await Task.Delay(500); // Simulate network call
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
}
