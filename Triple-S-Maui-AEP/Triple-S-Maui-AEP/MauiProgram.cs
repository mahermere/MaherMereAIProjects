using Microsoft.Extensions.Logging;
using Triple_S_Maui_AEP.Views;

namespace Triple_S_Maui_AEP
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("\n=== MAUIPROGRAM.CREATEMAUIAPP START ===");
                
                System.Diagnostics.Debug.WriteLine("Step 1: Creating MauiAppBuilder");
                var builder = MauiApp.CreateBuilder();
                System.Diagnostics.Debug.WriteLine("Step 1: SUCCESS");
                
                System.Diagnostics.Debug.WriteLine("Step 2: Configuring builder");
                builder
                    .UseMauiApp<App>()
                    .ConfigureFonts(fonts =>
                    {
                        System.Diagnostics.Debug.WriteLine("  - Adding OpenSans-Regular.ttf");
                        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                        System.Diagnostics.Debug.WriteLine("  - Adding OpenSans-Semibold.ttf");
                        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    });
                System.Diagnostics.Debug.WriteLine("Step 2: SUCCESS");

#if DEBUG
                System.Diagnostics.Debug.WriteLine("Step 3: Adding debug logging");
                builder.Logging.AddDebug();
                System.Diagnostics.Debug.WriteLine("Step 3: SUCCESS");
#endif

                System.Diagnostics.Debug.WriteLine("Step 4: Registering LanguageService singleton");
                try
                {
                    builder.Services.AddSingleton<Triple_S_Maui_AEP.Services.LanguageService>(_ => 
                    {
                        System.Diagnostics.Debug.WriteLine("  - Getting LanguageService.Instance");
                        var instance = Triple_S_Maui_AEP.Services.LanguageService.Instance;
                        System.Diagnostics.Debug.WriteLine($"  - LanguageService instance obtained: {instance != null}");
                        return instance!;
                    });
                    System.Diagnostics.Debug.WriteLine("Step 4: SUCCESS");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Step 4: FAILED");
                    System.Diagnostics.Debug.WriteLine($"  Error: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"  Stack: {ex.StackTrace}");
                    throw;
                }

#if ANDROID
                System.Diagnostics.Debug.WriteLine("Step 5: Android-specific configuration");
                try
                {
                    builder.Services.AddTransient<Triple_S_Maui_AEP.Services.IPdfOpener, PdfOpenerStub>();
                    System.Diagnostics.Debug.WriteLine("  - Registered IPdfOpener");
                    
                    // Ensure Android is initialized properly
                    Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("CustomEntry", (handler, view) => {
                        System.Diagnostics.Debug.WriteLine("  - Android entry handler mapped");
                    });
                    System.Diagnostics.Debug.WriteLine("Step 5: SUCCESS");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Step 5: FAILED");
                    System.Diagnostics.Debug.WriteLine($"  Error: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"  Stack: {ex.StackTrace}");
                    // Don't throw - Android-specific failure shouldn't crash startup
                }
#endif

                System.Diagnostics.Debug.WriteLine("Step 6: Building MauiApp");
                var app = builder.Build();
                System.Diagnostics.Debug.WriteLine("Step 6: SUCCESS");

#if ANDROID
                System.Diagnostics.Debug.WriteLine("Step 7: Registering PdfOpener in ServiceLocator");
                try
                {
                    var pdfOpener = app.Services.GetService<Triple_S_Maui_AEP.Services.IPdfOpener>();
                    System.Diagnostics.Debug.WriteLine($"  - Got PdfOpener from services: {pdfOpener != null}");
                    if (pdfOpener != null)
                    {
                        Triple_S_Maui_AEP.Services.ServiceLocator.PdfOpener = pdfOpener;
                        System.Diagnostics.Debug.WriteLine("  - Registered in ServiceLocator");
                    }
                    System.Diagnostics.Debug.WriteLine("Step 7: SUCCESS");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Step 7: FAILED");
                    System.Diagnostics.Debug.WriteLine($"  Error: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"  Stack: {ex.StackTrace}");
                    // Don't throw - ServiceLocator failure shouldn't crash startup
                }
#endif
                
                System.Diagnostics.Debug.WriteLine("=== MAUIPROGRAM.CREATEMAUIAPP SUCCESS ===\n");
                return app;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("\n=== MAUIPROGRAM.CREATEMAUIAPP FAILED ===");
                System.Diagnostics.Debug.WriteLine($"FATAL ERROR: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Exception type: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    System.Diagnostics.Debug.WriteLine($"Inner stack: {ex.InnerException.StackTrace}");
                }
                System.Diagnostics.Debug.WriteLine("=== MAUIPROGRAM.CREATEMAUIAPP ERROR ===\n");
                throw;
            }
        }
    }

    // Temporary stub for PDF opener - will be moved to Android platform
    public class PdfOpenerStub : Triple_S_Maui_AEP.Services.IPdfOpener
    {
        public async Task OpenPdfAsync(byte[] pdfData, string fileName)
        {
            System.Diagnostics.Debug.WriteLine($"PdfOpenerStub.OpenPdfAsync called for: {fileName}");
            try
            {
                // On Android, save to downloads or use sharing
                var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
                System.Diagnostics.Debug.WriteLine($"  - Saving to: {filePath}");
                await File.WriteAllBytesAsync(filePath, pdfData);
                System.Diagnostics.Debug.WriteLine($"  - File saved");
                
                // Try to open with default PDF viewer
                System.Diagnostics.Debug.WriteLine($"  - Launching file");
                await Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
                System.Diagnostics.Debug.WriteLine($"  - File launched");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"  - Error: {ex.Message}");
                throw;
            }
        }
    }
}
