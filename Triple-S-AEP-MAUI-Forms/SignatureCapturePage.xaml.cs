namespace Triple_S_AEP_MAUI_Forms;

public partial class SignatureCapturePage : ContentPage
{
    private readonly TaskCompletionSource<string?> _result = new();

    public SignatureCapturePage(string title)
    {
        InitializeComponent();
        HeaderLabel.Text = title;
    }

    public Task<string?> Result => _result.Task;

    protected override bool OnBackButtonPressed()
    {
        ClearSignature();
        _result.TrySetResult(null);
        return base.OnBackButtonPressed();
    }

    private void OnClearClicked(object? sender, EventArgs e)
    {
        ClearSignature();
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        ClearSignature();
        _result.TrySetResult(null);
        await Navigation.PopModalAsync();
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (SignatureDrawingView.Lines.Count == 0)
        {
            await DisplayAlert("Signature required", "Please provide a signature before saving.", "OK");
            return;
        }

        try
        {
            var width = (int)Math.Max(SignatureDrawingView.Width, 600);
            var height = (int)Math.Max(SignatureDrawingView.Height, 320);

            await using var imageStream = await SignatureDrawingView.GetImageStream(width, height);
            using var memoryStream = new MemoryStream();
            await imageStream.CopyToAsync(memoryStream);
            var dataUrl = $"data:image/png;base64,{Convert.ToBase64String(memoryStream.ToArray())}";

            _result.TrySetResult(dataUrl);
            await Navigation.PopModalAsync();
        }
        catch
        {
            await DisplayAlert("Signature unavailable", "Unable to capture the signature image.", "OK");
        }
    }

    private void ClearSignature()
    {
        SignatureDrawingView.Lines.Clear();
    }
}
