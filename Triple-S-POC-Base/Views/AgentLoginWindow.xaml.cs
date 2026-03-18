using System;
using TripleS.SOA.AEP.UI.ViewModels;
using Microsoft.Maui.Controls;

namespace TripleS.SOA.AEP.UI.Views
{
    public partial class AgentLoginWindow : ContentPage
    {
        public AgentLoginWindow()
        {
            // MAUI auto-generates InitializeComponent, but if error persists, ensure x:Class matches namespace
            InitializeComponent();
            BindingContext = new TripleSPOC.ViewModels.AgentLoginViewModel();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var npnEntry = this.FindByName<Entry>("NPNTextBox");
            var passwordEntry = this.FindByName<Entry>("PasswordBox");
            var npn = npnEntry?.Text ?? string.Empty;
            var password = passwordEntry?.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(npn) || string.IsNullOrWhiteSpace(password))
            {
                await ShowError("Please enter both NPN and password");
                return;
            }

            // TODO: Implement actual authentication against database
            // For now, accept any non-empty credentials
            if (npn.Length >= 8 && password.Length >= 6)
            {
                // Navigate to Dashboard
                await Navigation.PushAsync(new DashboardWindow());
            }
            else
            {
                await ShowError("Invalid credentials. NPN must be 8+ digits, password 6+ characters.");
            }
        }

        private async Task ShowError(string message)
        {
            var errorMessage = this.FindByName<Label>("ErrorMessage");
            var errorBorder = this.FindByName<Border>("ErrorBorder");
            await DisplayAlert("Error", message, "OK");
            if (errorMessage != null) errorMessage.Text = message;
            if (errorBorder != null) errorBorder.IsVisible = true;
        }
    }
}
