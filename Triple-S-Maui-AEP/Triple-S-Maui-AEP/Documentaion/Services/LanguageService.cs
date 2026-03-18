using System;
using Microsoft.Maui.Storage;

namespace TripleSPOC.Services
{
    public class LanguageService
    {
        private static readonly Lazy<LanguageService> _instance = new(() => new LanguageService());
        public static LanguageService Instance => _instance.Value;

        public event Action<Models.Language>? LanguageChanged;

        private const string LanguageKey = "AppLanguage";
        private Models.Language _currentLanguage;

        public Models.Language CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    Preferences.Set(LanguageKey, value.ToString());
                    LanguageChanged?.Invoke(_currentLanguage);
                }
            }
        }

        private LanguageService()
        {
            var saved = Preferences.Get(LanguageKey, Models.Language.English.ToString());
            if (Enum.TryParse(saved, out Models.Language lang))
                _currentLanguage = lang;
            else
                _currentLanguage = Models.Language.English;
        }
    }
}
