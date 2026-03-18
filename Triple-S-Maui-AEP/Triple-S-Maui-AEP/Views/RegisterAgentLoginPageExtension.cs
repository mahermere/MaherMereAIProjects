using Triple_S_Maui_AEP.Views;

namespace Triple_S_Maui_AEP
{
    public static class MauiAppBuilderExtensions
    {
        public static MauiAppBuilder RegisterAgentLoginPage(this MauiAppBuilder builder)
        {
            builder.ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
            return builder;
        }
    }
}
