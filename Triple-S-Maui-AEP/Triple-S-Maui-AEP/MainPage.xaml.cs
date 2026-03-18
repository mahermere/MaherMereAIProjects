namespace Triple_S_Maui_AEP
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            InitializeLanguagePicker();
            LoadAgentInfo();
            
            // Set all UI text from language resources
            SetLocalizedText();
            Services.LanguageService.Instance.LanguageChanged += _ => SetLocalizedText();
        }

        private void InitializeLanguagePicker()
        {
            LanguagePicker.Items.Clear();
            LanguagePicker.Items.Add("English");
            LanguagePicker.Items.Add("Español");
            
            // Set current language
            var currentLang = Services.LanguageService.Instance.CurrentLanguage;
            LanguagePicker.SelectedIndex = currentLang == Models.Language.English ? 0 : 1;
            
            // Subscribe to language changes
            LanguagePicker.SelectedIndexChanged += (s, e) =>
            {
                var newLang = LanguagePicker.SelectedIndex == 1 ? Models.Language.Spanish : Models.Language.English;
                Services.LanguageService.Instance.CurrentLanguage = newLang;
            };
        }

        private void SetLocalizedText()
        {
            var isEnglish = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English;

            // Update all text elements
            if (Title != null) Title = isEnglish ? "Triple-S Annual Enrollment" : "Inscripción Anual Triple-S";
            
            var headerLabel = this.FindByName<Label>("AnnualEnrollmentLabel");
            if (headerLabel != null) headerLabel.Text = isEnglish ? "Annual Enrollment Portal" : "Portal de Inscripción Anual";
            
            var welcomeLabel = this.FindByName<Label>("WelcomeLabel");
            if (welcomeLabel != null) welcomeLabel.Text = isEnglish 
                ? "Welcome to the Triple-S Medicare Advantage Annual Enrollment Process" 
                : "Bienvenido al Proceso de Inscripción Anual de Triple-S Medicare Advantage";
            
            var agentInfoLabel = this.FindByName<Label>("AgentInfoTitleLabel");
            if (agentInfoLabel != null) agentInfoLabel.Text = isEnglish ? "Agent Information" : "Información del Agente";
            
            var agentNpnHeaderLabel = this.FindByName<Label>("AgentNPNHeaderLabel");
            if (agentNpnHeaderLabel != null) agentNpnHeaderLabel.Text = isEnglish ? "Agent NPN:" : "NPN del Agente:";
            
            var agentNameHeaderLabel = this.FindByName<Label>("AgentNameHeaderLabel");
            if (agentNameHeaderLabel != null) agentNameHeaderLabel.Text = isEnglish ? "Agent Name:" : "Nombre del Agente:";
            
            var sessionStatusHeaderLabel = this.FindByName<Label>("SessionStatusHeaderLabel");
            if (sessionStatusHeaderLabel != null) sessionStatusHeaderLabel.Text = isEnglish ? "Session Status:" : "Estado de la Sesión:";
            
            var quickActionsLabel = this.FindByName<Label>("QuickActionsLabel");
            if (quickActionsLabel != null) quickActionsLabel.Text = isEnglish ? "Quick Actions" : "Acciones Rápidas";
            
            // Update buttons
            var newEnrollmentBtn = this.FindByName<Button>("NewEnrollmentButton");
            if (newEnrollmentBtn != null) newEnrollmentBtn.Text = isEnglish ? "Create New Enrollment" : "Crear Nueva Inscripción";

            var newTripleSEnrollmentBtn = this.FindByName<Button>("NewTripleSEnrollmentButton");
            if (newTripleSEnrollmentBtn != null) newTripleSEnrollmentBtn.Text = isEnglish ? "Create New Triple-S Enrollment" : "Crear Nueva Inscripción Triple-S";
            
            var newSOABtn = this.FindByName<Button>("NewSOAButton");
            if (newSOABtn != null) newSOABtn.Text = isEnglish ? "Create SOA (Signature of Authority)" : "Crear SOA (Firma de Autoridad)";
            
            var dashboardBtn = this.FindByName<Button>("DashboardButton");
            if (dashboardBtn != null) dashboardBtn.Text = isEnglish ? "View Dashboard" : "Ver Panel de Control";
            
            var logoutBtn = this.FindByName<Button>("LogoutButton");
            if (logoutBtn != null) logoutBtn.Text = isEnglish ? "Logout" : "Cerrar Sesión";
        }

        private void LoadAgentInfo()
        {
            var agentNPN = Services.AgentSessionService.CurrentAgentNPN;
            var agentName = Services.AgentSessionService.CurrentAgentName ?? "Agent";

            AgentNPNLabel.Text = agentNPN;
            AgentNameLabel.Text = agentName;
        }

        private async void OnNewEnrollmentClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///TripleSEnrollmentWizardPage");
        }

        private async void OnNewTripleSEnrollmentClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///TripleSEnrollmentWizardPage");
        }

        private async void OnNewSOAClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///SOAWizardPage");
        }

        private async void OnViewDashboardClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///DashboardPage");
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            var isEnglish = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English;
            var result = await DisplayAlert(
                isEnglish ? "Logout" : "Cerrar Sesión",
                isEnglish ? "Are you sure you want to logout?" : "¿Está seguro de que desea cerrar sesión?",
                isEnglish ? "Yes" : "Sí",
                isEnglish ? "No" : "No");
            
            if (result)
            {
                Services.AgentSessionService.ClearSession();
                await Shell.Current.GoToAsync("///AgentLoginPage");
            }
        }
    }
}
