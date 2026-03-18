using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TripleSPOC.ViewModels
{
    public class AgentLoginViewModel : INotifyPropertyChanged
    {
        private string? _npn;
        private string? _errorMessage;
        private bool _isLoading;

        public string? NPN
        {
            get => _npn;
            set { _npn = value; OnPropertyChanged(); }
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public AgentLoginViewModel()
        {
            NPN = string.Empty;
            ErrorMessage = string.Empty;
            IsLoading = false;
        }

        public async Task<(bool Success, string Message, string? AgentId)> AuthenticateAsync(string npn, string password)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(npn))
                    return (false, "NPN is required", null);

                if (string.IsNullOrWhiteSpace(password))
                    return (false, "Password is required", null);

                if (npn.Length < 8 || npn.Length > 10)
                    return (false, "NPN must be 8-10 digits", null);

                if (!npn.All(char.IsDigit))
                    return (false, "NPN must contain only digits", null);

                if (password.Length < 6)
                    return (false, "Password must be at least 6 characters", null);

                await Task.Delay(500);
                return (true, "Authentication successful", npn);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Authentication error: {ex.Message}";
                return (false, ErrorMessage, null);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
