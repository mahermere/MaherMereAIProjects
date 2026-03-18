using System.ComponentModel;
using System.Runtime.CompilerServices;
using Triple_S_Maui_AEP.Models;
using Triple_S_Maui_AEP.Services;

namespace Triple_S_Maui_AEP.ViewModels
{
    /// <summary>
    /// Base class for ViewModels implementing INotifyPropertyChanged
    /// Includes language support for bilingual applications
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        protected Language CurrentLanguage => LanguageService.Instance.CurrentLanguage;
        protected bool IsEnglish => CurrentLanguage == Language.English;

        public event PropertyChangedEventHandler? PropertyChanged;

        public BaseViewModel()
        {
            // Subscribe to language changes
            LanguageService.Instance.LanguageChanged += OnLanguageChanged;
        }

        /// <summary>
        /// Called when language is changed - override in subclasses to update UI text
        /// </summary>
        protected virtual void OnLanguageChanged(Language newLanguage)
        {
            OnPropertyChanged(nameof(CurrentLanguage));
            OnPropertyChanged(nameof(IsEnglish));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Helper method to get localized text
        /// </summary>
        protected string GetLocalizedText(string englishText, string spanishText)
        {
            return IsEnglish ? englishText : spanishText;
        }
    }
}
