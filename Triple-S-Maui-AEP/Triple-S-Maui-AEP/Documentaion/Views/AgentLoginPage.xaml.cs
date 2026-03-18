using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using TripleSPOC.ViewModels;

namespace TripleSPOC.Views
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
            LanguagePicker.Items.Add(TripleSPOC.Resources.Localization.AppResources.English);
            LanguagePicker.Items.Add(TripleSPOC.Resources.Localization.AppResources.Spanish);
        }

        private void SetLocalizedText()
        {

            if  (LanguagePicker.Items.Count == 0)
            {
                LanguagePicker.Items.Add(TripleSPOC.Resources.Localization.AppResources.English);
                LanguagePicker.Items.Add(TripleSPOC.Resources.Localization.AppResources.Spanish);
            }
            else
            {
                LanguagePicker.Items[0] = TripleSPOC.Resources.Localization.AppResources.English;
                LanguagePicker.Items[1] = TripleSPOC.Resources.Localization.AppResources.Spanish;
            }
           
            SignInButton.Text = TripleSPOC.Resources.Localization.AppResources.SignIn;
            NpnEntry.Placeholder = TripleSPOC.Resources.Localization.AppResources.NPNNumber;
            PasswordEntry.Placeholder = TripleSPOC.Resources.Localization.AppResources.Password;
            var rememberLabel = this.FindByName<Label>("RememberMyNPNLabel");
            if (rememberLabel != null) rememberLabel.Text = TripleSPOC.Resources.Localization.AppResources.RememberMyNPN;
            // FindByName for labels and set their Text as well
            var forgotLabel = this.FindByName<Label>("ForgotPasswordLabel");
            if (forgotLabel != null) forgotLabel.Text = TripleSPOC.Resources.Localization.AppResources.ForgotPassword;
            var firstTimeLabel = this.FindByName<Label>("FirstTimeUserLabel");
            if (firstTimeLabel != null) firstTimeLabel.Text = TripleSPOC.Resources.Localization.AppResources.FirstTimeUser;
            var contactLabel = this.FindByName<Label>("ContactOfficeManagerLabel");
            if (contactLabel != null) contactLabel.Text = TripleSPOC.Resources.Localization.AppResources.ContactOfficeManager;
            var portalAccessLabel = this.FindByName<Label>("AgentPortalAccessLabel");
            if (portalAccessLabel != null) portalAccessLabel.Text = TripleSPOC.Resources.Localization.AppResources.AgentPortalAccess;
            var enterCredsLabel = this.FindByName<Label>("EnterCredentialsLabel");
            if (enterCredsLabel != null) enterCredsLabel.Text = TripleSPOC.Resources.Localization.AppResources.EnterCredentials;
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

            var (success, message, agentId) = await _viewModel.AuthenticateAsync(npn, password);
            if (success)
            {
                await DisplayAlert("Success", "Login successful!", "OK");
                // Replace the root page of the current window with the dashboard
                if (Application.Current?.Windows.Count > 0 && Application.Current.Windows[0] is Window win)
                {
                    win.Page = new NavigationPage(new TripleS.SOA.AEP.UI.Views.DashboardWindow());
                }
            }
            else
            {
                ErrorMessageLabel.Text = message;
                ErrorMessageLabel.IsVisible = true;
            }
        }
    }
}
