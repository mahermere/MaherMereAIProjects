using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Triple_S_Maui_AEP.ViewModels;

namespace Triple_S_Maui_AEP.Views
{
    public partial class AgentLoginPage : ContentPage
    {
        private readonly AgentLoginViewModel _viewModel = new AgentLoginViewModel();

        private void OnExitClicked(object sender, EventArgs e)
        {
#if WINDOWS
            Microsoft.UI.Xaml.Window.Current.Close();
#else
            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
#endif
        }

        public AgentLoginPage()
        {
            try
            {
                InitializeComponent();
                BindingContext = _viewModel;

                InitializeLanguagePicker();

                // Set picker to current language
                var lang = Services.LanguageService.Instance.CurrentLanguage;
                LanguagePicker.SelectedIndex = lang == Models.Language.English ? 0 : 1;
                LanguagePicker.SelectedIndexChanged += OnLanguageChanged;

                // Set all UI text from resources
                SetLocalizedText();
                Services.LanguageService.Instance.LanguageChanged += _ => SetLocalizedText();
            }
            catch (Exception ex)
            {
                // Show error in the UI if possible
                if (this.FindByName<Label>("ErrorMessageLabel") is Label errorLabel)
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
        }

        private void SetLocalizedText()
        {
            if (LanguagePicker.Items.Count == 0)
            {
                LanguagePicker.Items.Add("English");
                LanguagePicker.Items.Add("Español");
            }

            SignInButton.Text = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "Sign In" : "Iniciar Sesión";
            NpnEntry.Placeholder = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "Enter NPN" : "Ingrese NPN";
            PasswordEntry.Placeholder = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "Enter password" : "Ingrese contraseña";
            
            var rememberLabel = this.FindByName<Label>("RememberMyNPNLabel");
            if (rememberLabel != null) rememberLabel.Text = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "Remember my NPN" : "Recordar mi NPN";
            
            var forgotLabel = this.FindByName<Label>("ForgotPasswordLabel");
            if (forgotLabel != null) forgotLabel.Text = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "Forgot Password?" : "¿Olvidó su contraseña?";
            
            var firstTimeLabel = this.FindByName<Label>("FirstTimeUserLabel");
            if (firstTimeLabel != null) firstTimeLabel.Text = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "First Time User?" : "¿Primer Usuario?";
            
            var contactLabel = this.FindByName<Label>("ContactOfficeManagerLabel");
            if (contactLabel != null) contactLabel.Text = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "Contact your office manager for your NPN and temporary password." : "Comuníquese con su gerente de oficina para obtener su NPN y contraseña temporal.";
            
            var portalAccessLabel = this.FindByName<Label>("AgentPortalAccessLabel");
            if (portalAccessLabel != null) portalAccessLabel.Text = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "Agent Portal Access" : "Acceso al Portal de Agentes";
            
            var enterCredsLabel = this.FindByName<Label>("EnterCredentialsLabel");
            if (enterCredsLabel != null) enterCredsLabel.Text = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English ? "Enter your credentials to access SOA and enrollment tools." : "Ingrese sus credenciales para acceder a las herramientas SOA e inscripción.";
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            if (LanguagePicker.SelectedIndex == 0)
                Services.LanguageService.Instance.CurrentLanguage = Models.Language.English;
            else if (LanguagePicker.SelectedIndex == 1)
                Services.LanguageService.Instance.CurrentLanguage = Models.Language.Spanish;
        }

        private async void OnSignInClicked(object sender, EventArgs e)
        {
            var npn = NpnEntry.Text?.Trim() ?? string.Empty;
            var password = PasswordEntry.Text ?? string.Empty;
            ErrorMessageLabel.IsVisible = false;
            
            var loginStatusLabel = this.FindByName<Label>("LoginStatusLabel");
            if (loginStatusLabel != null) loginStatusLabel.IsVisible = false;

            // Disable button and show loading state
            SignInButton.IsEnabled = false;
            SignInButton.Text = "⏳"; // Hourglass icon
            SignInButton.BackgroundColor = Color.FromArgb("#95A5A6"); // Gray color

            // Show authenticating status
            var isEnglish = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English;
            if (loginStatusLabel != null)
            {
                loginStatusLabel.Text = isEnglish ? "🔒 Authenticating..." : "🔒 Autenticando...";
                loginStatusLabel.IsVisible = true;
            }

            System.Diagnostics.Debug.WriteLine($"🔐 Login attempt started: NPN={npn}");

            try
            {
                var result = await _viewModel.AuthenticateAsync(npn, password);
                var success = result.Success;
                var message = result.Message;
                var agentId = result.AgentId;
                
                if (success)
                {
                    // Show success state
                    SignInButton.Text = "✓";
                    SignInButton.BackgroundColor = Color.FromArgb("#27AE60"); // Green
                    
                    if (loginStatusLabel != null)
                    {
                        loginStatusLabel.Text = isEnglish ? "✓ Login successful! Redirecting..." : "✓ ¡Inicio de sesión exitoso! Redirigiendo...";
                        loginStatusLabel.TextColor = Color.FromArgb("#27AE60");
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"✓ Login successful for NPN: {npn}");

                    await DisplayAlert(
                        isEnglish ? "Success" : "Éxito",
                        isEnglish ? "Login successful!" : "¡Inicio de sesión exitoso!",
                        "OK");

                    // Store both NPN and password for use in DMS operations
                    Services.AgentSessionService.SetSession(agentId ?? npn, agentName: null, password: password);
                    
                    await Shell.Current.GoToAsync("///MainPage");
                }
                else
                {
                    // Show failure state - re-enable button
                    SignInButton.IsEnabled = true;
                    SignInButton.Text = isEnglish ? "Sign In" : "Iniciar Sesión";
                    SignInButton.BackgroundColor = Color.FromArgb("#1976D2"); // Blue
                    
                    if (loginStatusLabel != null) loginStatusLabel.IsVisible = false;
                    
                    ErrorMessageLabel.Text = message;
                    ErrorMessageLabel.IsVisible = true;
                    
                    System.Diagnostics.Debug.WriteLine($"✗ Login failed: {message}");
                }
            }
            catch (Exception ex)
            {
                // Show error state - re-enable button
                SignInButton.IsEnabled = true;
                SignInButton.Text = isEnglish ? "Sign In" : "Iniciar Sesión";
                SignInButton.BackgroundColor = Color.FromArgb("#1976D2"); // Blue
                
                if (loginStatusLabel != null) loginStatusLabel.IsVisible = false;
                
                var errorMsg = isEnglish 
                    ? $"Login error: {ex.Message}" 
                    : $"Error de inicio de sesión: {ex.Message}";
                
                ErrorMessageLabel.Text = errorMsg;
                ErrorMessageLabel.IsVisible = true;
                
                System.Diagnostics.Debug.WriteLine($"✗ Login exception: {ex.Message}");
            }
        }
    }
}
