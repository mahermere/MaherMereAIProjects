namespace Triple_S_AEP_MAUI_Forms
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCreateAepFormClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(AepFormWizardPage));
        }

        private async void OnCreateSoaFormClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(SoaFormWizardPage));
        }

        private async void OnDashboardClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(DashboardPage));
        }
    }
}
