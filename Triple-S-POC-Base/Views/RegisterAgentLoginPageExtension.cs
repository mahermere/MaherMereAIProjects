using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;

namespace TripleSPOC.Views
{
    public static class RegisterAgentLoginPageExtension
    {
        public static MauiAppBuilder RegisterAgentLoginPage(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<AgentLoginPage>();
            return builder;
        }
    }
}
