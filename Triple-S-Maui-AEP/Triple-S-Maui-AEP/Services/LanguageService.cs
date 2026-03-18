using Triple_S_Maui_AEP.Models;

namespace Triple_S_Maui_AEP.Services
{
    public class LanguageService
    {
        private static LanguageService? _instance;
        private Language _currentLanguage = Language.English;

        public static LanguageService Instance
        {
            get { return _instance ??= new LanguageService(); }
        }

        public Language CurrentLanguage
        {
            get { return _currentLanguage; }
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    LanguageChanged?.Invoke(value);
                }
            }
        }

        public event Action<Language>? LanguageChanged;

        public void SetLanguage(Language language)
        {
            CurrentLanguage = language;
        }

        public string GetLanguageCode()
        {
            return CurrentLanguage == Language.English ? "en" : "es";
        }
    }
}
