using Triple_S_AEP_MAUI_Forms.Services;

namespace Triple_S_AEP_MAUI_Forms
{
    public partial class AgentLoginPage : ContentPage
    {
        private readonly DmsUploadService _dmsService = new();

        public AgentLoginPage()
        {
            InitializeComponent();
        }

        private async void OnSignInClicked(object? sender, EventArgs e)
        {
            var username = AgentIdEntry.Text?.Trim();
            var password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Missing information", "Please enter your Agent ID and password.", "OK");
                return;
            }

            SignInButton.IsEnabled = false;
            SignInActivityIndicator.IsVisible = true;
            SignInActivityIndicator.IsRunning = true;

            try
            {
                var (isSuccess, message) = await _dmsService.VerifyUserAsync(username, password);

                if (!isSuccess)
                {
                    await DisplayAlert("Login failed", message, "OK");
                    return;
                }

                SessionService.Instance.SetCredentials(username, password);
                await Shell.Current.GoToAsync(nameof(MainPage));
            }
            finally
            {
                SignInButton.IsEnabled = true;
                SignInActivityIndicator.IsRunning = false;
                SignInActivityIndicator.IsVisible = false;
            }
        }
    }
}
