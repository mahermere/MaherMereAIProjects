using CommunityToolkit.Maui;

using Microsoft.Extensions.Logging;
using TripleSPOC.Views;

namespace TripleSPOC;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.RegisterAgentLoginPage();

	 #if DEBUG
	 builder.Logging.AddDebug();
	 #endif

	// Register LanguageService as singleton
	builder.Services.AddSingleton<TripleSPOC.Services.LanguageService>(_ => TripleSPOC.Services.LanguageService.Instance);

#if ANDROID
	builder.Services.AddTransient<TripleS.SOA.AEP.UI.Services.IPdfOpener, TripleS.SOA.AEP.UI.Platforms.Android.PdfOpener>();
#endif

	var app = builder.Build();
#if ANDROID
	var pdfOpener = app.Services.GetService<TripleS.SOA.AEP.UI.Services.IPdfOpener>();
	if (pdfOpener != null)
		TripleS.SOA.AEP.UI.Services.ServiceLocator.PdfOpener = pdfOpener;
#endif
	return app;
	}
}
