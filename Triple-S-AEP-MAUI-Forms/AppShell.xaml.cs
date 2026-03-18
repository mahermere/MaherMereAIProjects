namespace Triple_S_AEP_MAUI_Forms
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(AepFormWizardPage), typeof(AepFormWizardPage));
            Routing.RegisterRoute(nameof(SoaFormWizardPage), typeof(SoaFormWizardPage));
            Routing.RegisterRoute(nameof(SoaSelectorPage), typeof(SoaSelectorPage));
            Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
        }
    }
}
