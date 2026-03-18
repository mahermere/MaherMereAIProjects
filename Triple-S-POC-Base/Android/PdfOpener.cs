#if ANDROID
using TripleS.SOA.AEP.UI.Services;
using System.Diagnostics;
namespace TripleS.SOA.AEP.UI.Platforms.Android
{
    public class PdfOpener : IPdfOpener
    {
        public void OpenPdf(string filePath)
        {
            var context = global::Android.App.Application.Context;
            if (context == null) { System.Diagnostics.Debug.WriteLine("Android context is null."); return; }
            global::Android.Net.Uri? uri = null;
            var file = new global::Java.IO.File(filePath);
            if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.N)
            {
                // Use FileProvider for Android 7+
                uri = global::AndroidX.Core.Content.FileProvider.GetUriForFile(
                    context,
                    "com.companyname.triplespoc.fileprovider",
                    file);
            }
            else
            {
                uri = global::Android.Net.Uri.FromFile(file);
            }
            if (uri != null)
            {
                var intent = new global::Android.Content.Intent(global::Android.Content.Intent.ActionView);
                intent.SetDataAndType(uri, "application/pdf");
                intent.SetFlags(global::Android.Content.ActivityFlags.NewTask);
                intent.AddFlags(global::Android.Content.ActivityFlags.GrantReadUriPermission);
                context.StartActivity(intent);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Android URI is null.");
            }
        }
    }
}
#elif WINDOWS
using TripleS.SOA.AEP.UI.Services;
using System.Diagnostics;
namespace TripleS.SOA.AEP.UI.Platforms.Android
{
    public class PdfOpener : IPdfOpener
    {
        public void OpenPdf(string filePath)
        {
            // Launch PDF in default viewer on Windows
            try
            {
                // Ensure the file path is absolute and exists
                if (!System.IO.File.Exists(filePath))
                    return;
                var psi = new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true,
                    Verb = "open"
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to open PDF: {ex.Message}");
            }
        }
    }
}
#else
using TripleS.SOA.AEP.UI.Services;
namespace TripleS.SOA.AEP.UI.Platforms.Android
{
    public class PdfOpener : IPdfOpener
    {
        public void OpenPdf(string filePath)
        {
            // Stub for other platforms
        }
    }
}
#endif
