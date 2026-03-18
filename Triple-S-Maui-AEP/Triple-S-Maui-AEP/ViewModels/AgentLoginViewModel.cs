using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Triple_S_Maui_AEP.Services;

namespace Triple_S_Maui_AEP.ViewModels
{
    /// <summary>
    /// ViewModel for Agent Login
    /// Handles authentication against OnBase API
    /// </summary>
    public class AgentLoginViewModel : BaseViewModel
    {
        private readonly OnBaseAuthenticationService _authService = new();
        private string? _npn;
        private string? _password;
        private string? _errorMessage;
        private bool _isLoading;

        public string? NPN
        {
            get => _npn;
            set => SetProperty(ref _npn, value);
        }

        public string? Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public AgentLoginViewModel()
        {
            NPN = string.Empty;
            Password = string.Empty;
            ErrorMessage = string.Empty;
            IsLoading = false;
        }

        /// <summary>
        /// Authenticates an agent against OnBase API using NPN and password
        /// </summary>
        public async Task<(bool Success, string Message, string? AgentId)> AuthenticateAsync(string npn, string password)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                // Validate NPN format
                if (string.IsNullOrWhiteSpace(npn))
                {
                    ErrorMessage = "NPN is required";
                    return (false, ErrorMessage, null);
                }

                if (!OnBaseAuthenticationService.IsValidNPNFormat(npn))
                {
                    ErrorMessage = "NPN must be 8-10 digits";
                    return (false, ErrorMessage, null);
                }

                // Validate password
                if (string.IsNullOrWhiteSpace(password))
                {
                    ErrorMessage = "Password is required";
                    return (false, ErrorMessage, null);
                }

                if (!OnBaseAuthenticationService.IsValidPassword(password))
                {
                    ErrorMessage = "Password must be at least 6 characters";
                    return (false, ErrorMessage, null);
                }

                System.Diagnostics.Debug.WriteLine($"Authenticating agent: {npn}");

                // Call OnBase authentication service
                var (success, message, agentId) = await _authService.VerifyUserAsync(npn, password);

                if (success)
                {
                    System.Diagnostics.Debug.WriteLine($"✓ Agent authenticated successfully: {npn}");
                    return (true, message, agentId);
                }
                else
                {
                    ErrorMessage = message;
                    System.Diagnostics.Debug.WriteLine($"✗ Authentication failed: {message}");
                    return (false, message, null);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Authentication error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"✗ Authentication exception: {ex.Message}");
                return (false, ErrorMessage, null);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
