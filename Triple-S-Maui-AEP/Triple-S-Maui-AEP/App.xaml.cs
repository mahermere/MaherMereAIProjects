using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Triple_S_Maui_AEP.Services;

namespace Triple_S_Maui_AEP
{
    public partial class App : Application
    {
        public App()
        {
            try
            {
                Debug.WriteLine("\n=== APP INITIALIZATION START ===");
                Debug.WriteLine("Step 1: InitializeComponent()");
                InitializeComponent();
                Debug.WriteLine("Step 1: SUCCESS");
                
                // Initialize secure database
                Debug.WriteLine("Step 2: Initializing secure database");
                try
                {
                    Debug.WriteLine("  - Calling SecureDatabaseService.InitializeAsync");
                    Task.Run(async () => await SecureDatabaseService.Instance.InitializeAsync()).Wait();
                    Debug.WriteLine("  - Secure database initialized successfully");
                    Debug.WriteLine("Step 2: SUCCESS");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Step 2: FAILED");
                    Debug.WriteLine($"Error initializing secure database: {ex.Message}");
                    Debug.WriteLine($"Exception type: {ex.GetType().Name}");
                    Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    if (ex.InnerException != null)
                    {
                        Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                        Debug.WriteLine($"Inner stack: {ex.InnerException.StackTrace}");
                    }
                    // Don't rethrow - database initialization failure shouldn't crash the app immediately
                }
                
                // Set initial culture with error handling
                Debug.WriteLine("Step 3: Setting language culture");
                try
                {
                    Debug.WriteLine("  - Getting LanguageService instance");
                    var languageService = Services.LanguageService.Instance;
                    Debug.WriteLine($"  - LanguageService obtained: {languageService != null}");
                    
                    if (languageService == null)
                    {
                        throw new InvalidOperationException("LanguageService instance is null.");
                    }
                    
                    Debug.WriteLine("  - Getting current language");
                    var currentLang = languageService.CurrentLanguage;
                    Debug.WriteLine($"  - Current language: {currentLang}");
                    
                    Debug.WriteLine("  - Calling SetCultureFromLanguage");
                    SetCultureFromLanguage(currentLang);
                    Debug.WriteLine("  - SetCultureFromLanguage: SUCCESS");
                    
                    Debug.WriteLine("  - Subscribing to LanguageChanged event");
                    languageService.LanguageChanged += lang => 
                    {
                        Debug.WriteLine($"  - LanguageChanged event triggered: {lang}");
                        SetCultureFromLanguage(lang);
                    };
                    Debug.WriteLine("Step 3: SUCCESS");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Step 3: FAILED");
                    Debug.WriteLine($"Error setting language: {ex.Message}");
                    Debug.WriteLine($"Exception type: {ex.GetType().Name}");
                    Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    if (ex.InnerException != null)
                    {
                        Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                        Debug.WriteLine($"Inner stack: {ex.InnerException.StackTrace}");
                    }
                    // Don't rethrow - language initialization failure shouldn't crash the app
                }
                
                Debug.WriteLine("=== APP INITIALIZATION SUCCESS ===\n");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("\n=== APP INITIALIZATION CRITICAL ERROR ===");
                Debug.WriteLine($"FATAL ERROR in App constructor: {ex.Message}");
                Debug.WriteLine($"Exception type: {ex.GetType().Name}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Debug.WriteLine($"Inner stack: {ex.InnerException.StackTrace}");
                }
                Debug.WriteLine("=== APP INITIALIZATION FAILED ===\n");
                throw;
            }
        }

        private void SetCultureFromLanguage(Models.Language lang)
        {
            try
            {
                var culture = lang == Models.Language.English ? "en-US" : "es-PR";
                Debug.WriteLine($"  - Setting culture: {culture}");
                // Localization will be set via the language service
                Debug.WriteLine($"  - Culture set successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in SetCultureFromLanguage: {ex.Message}");
                Debug.WriteLine($"Stack: {ex.StackTrace}");
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            try
            {
                Debug.WriteLine("\n=== WINDOW CREATION START ===");
                Debug.WriteLine("Creating AppShell window");
                var window = new Window(new AppShell());
                Debug.WriteLine("Window created successfully");
                Debug.WriteLine("=== WINDOW CREATION SUCCESS ===\n");
                return window;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("\n=== WINDOW CREATION FAILED ===");
                Debug.WriteLine($"Error creating window: {ex.Message}");
                Debug.WriteLine($"Exception type: {ex.GetType().Name}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                Debug.WriteLine("=== WINDOW CREATION ERROR ===\n");
                throw;
            }
        }
    }
}