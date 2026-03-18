using Microsoft.Extensions.DependencyInjection;

namespace TripleSPOC;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		// Set initial culture
		SetCultureFromLanguage(Services.LanguageService.Instance.CurrentLanguage);
		Services.LanguageService.Instance.LanguageChanged += lang => SetCultureFromLanguage(lang);
	}

	private void SetCultureFromLanguage(Models.Language lang)
	{
		var culture = lang == Models.Language.English ? "en-US" : "es-PR";
		TripleSPOC.Resources.Localization.AppResources.Culture = new System.Globalization.CultureInfo(culture);
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}