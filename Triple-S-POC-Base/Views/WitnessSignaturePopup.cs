using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;

namespace TripleS.SOA.AEP.UI.Views
{
    public class WitnessSignaturePopup : Popup
    {
        public DrawingView SignaturePad { get; private set; }
        public Button ClearButton { get; private set; }
        public Button SaveButton { get; private set; }
        public Button CancelButton { get; private set; }
        public TaskCompletionSource<byte[]?> CompletionSource { get; } = new();

        public WitnessSignaturePopup()
        {
            var layout = new VerticalStackLayout { Padding = 20, Spacing = 16, BackgroundColor = Colors.White };
            layout.Children.Add(new Label { Text = "Witness Signature", FontSize = 18, FontAttributes = FontAttributes.Bold });
            SignaturePad = new DrawingView
            {
                HeightRequest = 300,
                WidthRequest = 300,
                LineColor = Colors.Black,
                LineWidth = 3,
                IsMultiLineModeEnabled = true,
                ShouldClearOnFinish = false,
                BackgroundColor = Colors.LightGray
            };
            layout.Children.Add(SignaturePad);
            var buttonLayout = new HorizontalStackLayout { Spacing = 12 };
            ClearButton = new Button { Text = "Clear" };
            SaveButton = new Button { Text = "Save" };
            CancelButton = new Button { Text = "Cancel" };
            buttonLayout.Children.Add(ClearButton);
            buttonLayout.Children.Add(SaveButton);
            buttonLayout.Children.Add(CancelButton);
            layout.Children.Add(buttonLayout);
            Content = layout;

            ClearButton.Clicked += (s, e) => SignaturePad.Lines.Clear();
            SaveButton.Clicked += async (s, e) =>
            {
                var stream = await SignaturePad.GetImageStream(300, 100);
                byte[]? pngBytes = null;
                if (stream != null)
                {
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    pngBytes = ms.ToArray();
                }
                CompletionSource.TrySetResult(pngBytes);
                Close();
            };
            CancelButton.Clicked += (s, e) => { CompletionSource.TrySetResult(null); Close(); };
        }

        public Task<byte[]?> GetSignatureAsync() => CompletionSource.Task;
    }
}
