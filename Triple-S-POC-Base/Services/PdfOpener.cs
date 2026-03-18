using TripleS.SOA.AEP.UI.Services;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TripleS.SOA.AEP.UI.Platforms
{
    public class PdfOpener : IPdfOpener
    {
        public void OpenPdf(string filePath)
        {
#if WINDOWS
            try
            {
                if (!System.IO.File.Exists(filePath)) return;
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
                Debug.WriteLine($"Failed to open PDF on Windows: {ex.Message}");
            }
#elif ANDROID
            try
            {
                var context = global::Android.App.Application.Context;
                if (context == null) { Debug.WriteLine("Android context is null."); return; }
                global::Android.Net.Uri? uri = null;
                var file = new global::Java.IO.File(filePath);
                if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.N)
                {
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
                    Debug.WriteLine("Android URI is null.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to open PDF on Android: {ex.Message}");
            }
#else
            Debug.WriteLine("PDF opening not supported on this platform.");
#endif
        }
    }
}
