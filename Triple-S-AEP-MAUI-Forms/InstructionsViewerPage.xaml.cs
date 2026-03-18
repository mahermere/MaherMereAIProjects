namespace Triple_S_AEP_MAUI_Forms
{
    public partial class InstructionsViewerPage : ContentPage
    {
        private static readonly string[] DefaultInstructionsImageAssets = { "enUS-Enrollment-Request-Form-1.jpg" };
        private const double MinZoom = 0.35;
        private const double MaxZoom = 5;
        private readonly string[] _imageAssets;
        private int _currentImageIndex;
        private double _currentScale = 1;
        private double _startScale = 1;
        private double _xOffset;
        private double _yOffset;
        private double _panStartX;
        private double _panStartY;
        private byte[]? _imageBytes;
        private string? _loadedImageAsset;

        public InstructionsViewerPage() : this(DefaultInstructionsImageAssets)
        {
        }

        public InstructionsViewerPage(params string[] imageAssets)
        {
            InitializeComponent();
            _imageAssets = imageAssets is { Length: > 0 } ? imageAssets : DefaultInstructionsImageAssets;
            UpdateImageNavigationUi();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCurrentImageAsync();
        }

        private void OnImageViewportSizeChanged(object? sender, EventArgs e)
        {
            UpdateViewportFit();
        }

        private void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
        {
            switch (e.Status)
            {
                case GestureStatus.Started:
                    _startScale = InstructionImage.Scale;
                    break;
                case GestureStatus.Running:
                    ApplyScale(_startScale * e.Scale);
                    break;
            }
        }

        private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
        {
            if (_currentScale <= 1)
            {
                return;
            }

            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    _panStartX = InstructionImage.TranslationX;
                    _panStartY = InstructionImage.TranslationY;
                    break;
                case GestureStatus.Running:
                    InstructionImage.TranslationX = _panStartX + e.TotalX;
                    InstructionImage.TranslationY = _panStartY + e.TotalY;
                    break;
                case GestureStatus.Completed:
                    _xOffset = InstructionImage.TranslationX;
                    _yOffset = InstructionImage.TranslationY;
                    break;
            }
        }

        private void OnZoomInClicked(object? sender, EventArgs e) => ApplyScale(_currentScale + 0.25);

        private void OnZoomOutClicked(object? sender, EventArgs e) => ApplyScale(_currentScale - 0.25);

        private void OnResetZoomClicked(object? sender, EventArgs e)
        {
            ApplyScale(1);
            ResetPan();
        }

        private async void OnPreviousImageClicked(object? sender, EventArgs e)
        {
            if (_currentImageIndex == 0)
            {
                return;
            }

            _currentImageIndex--;
            UpdateImageNavigationUi();
            await LoadCurrentImageAsync(forceReload: true);
        }

        private async void OnNextImageClicked(object? sender, EventArgs e)
        {
            if (_currentImageIndex >= _imageAssets.Length - 1)
            {
                return;
            }

            _currentImageIndex++;
            UpdateImageNavigationUi();
            await LoadCurrentImageAsync(forceReload: true);
        }

        private async void OnCloseClicked(object? sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void OnPrintClicked(object? sender, EventArgs e)
        {
            if (_imageBytes == null || string.IsNullOrWhiteSpace(_loadedImageAsset))
            {
                return;
            }

            try
            {
                var htmlPath = await CreateLetterSizedHtmlAsync();
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Print image",
                    File = new ShareFile(htmlPath)
                });
            }
            catch
            {
                await DisplayAlert("Print unavailable", "Unable to prepare the image for printing.", "OK");
            }
        }

        private async void OnEmailClicked(object? sender, EventArgs e)
        {
            if (_imageBytes == null || string.IsNullOrWhiteSpace(_loadedImageAsset))
            {
                return;
            }

            if (!Email.Default.IsComposeSupported)
            {
                await DisplayAlert("Email unavailable", "Email is not supported on this device.", "OK");
                return;
            }

            try
            {
                var attachmentPath = await CreateImageAttachmentAsync();
                var message = new EmailMessage
                {
                    Subject = "Enrollment Request Form Image",
                    Body = "Attached is the enrollment request form image.",
                    BodyFormat = EmailBodyFormat.PlainText,
                    Attachments = [new EmailAttachment(attachmentPath)]
                };

                await Email.Default.ComposeAsync(message);
            }
            catch
            {
                await DisplayAlert("Email unavailable", "Unable to compose email with the image attachment.", "OK");
            }
        }

        private void ApplyScale(double scale)
        {
            _currentScale = Math.Clamp(scale, MinZoom, MaxZoom);
            InstructionImage.Scale = _currentScale;

            if (_currentScale <= 1)
            {
                ResetPan();
            }
        }

        private void UpdateViewportFit()
        {
            if (ImageViewport.Width <= 0 || ImageViewport.Height <= 0)
            {
                return;
            }

            InstructionImage.WidthRequest = ImageViewport.Width;
            InstructionImage.HeightRequest = ImageViewport.Height;

            if (_currentScale <= 1)
            {
                ResetPan();
            }
        }

        private void ResetPan()
        {
            _xOffset = 0;
            _yOffset = 0;
            _panStartX = 0;
            _panStartY = 0;
            InstructionImage.TranslationX = 0;
            InstructionImage.TranslationY = 0;
        }

        private async Task LoadCurrentImageAsync(bool forceReload = false)
        {
            var currentAsset = _imageAssets[_currentImageIndex];

            if (!forceReload && _loadedImageAsset == currentAsset && _imageBytes != null)
            {
                return;
            }

            try
            {
                await using var stream = await FileSystem.OpenAppPackageFileAsync(currentAsset);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                _imageBytes = memoryStream.ToArray();
                _loadedImageAsset = currentAsset;
                InstructionImage.Source = ImageSource.FromStream(() => new MemoryStream(_imageBytes));
                ApplyScale(1);
                ResetPan();
                UpdateViewportFit();
            }
            catch
            {
                await DisplayAlert("Image not found", $"Could not load {currentAsset}.", "OK");
            }
        }

        private void UpdateImageNavigationUi()
        {
            var hasMultipleImages = _imageAssets.Length > 1;
            PreviousImageButton.IsVisible = hasMultipleImages;
            NextImageButton.IsVisible = hasMultipleImages;
            PreviousImageButton.IsEnabled = hasMultipleImages && _currentImageIndex > 0;
            NextImageButton.IsEnabled = hasMultipleImages && _currentImageIndex < _imageAssets.Length - 1;
        }

        private async Task<string> CreateImageAttachmentAsync()
        {
            var fileName = _loadedImageAsset ?? $"instruction-{_currentImageIndex + 1}.jpg";
            var path = Path.Combine(FileSystem.CacheDirectory, fileName);
            await File.WriteAllBytesAsync(path, _imageBytes!);
            return path;
        }

        private async Task<string> CreateLetterSizedHtmlAsync()
        {
            var base64Image = Convert.ToBase64String(_imageBytes!);
            var htmlContent = $@"<!doctype html>
<html>
<head>
<meta charset='utf-8' />
<style>
@page {{ size: 8.5in 11in; margin: 0.5in; }}
html, body {{ margin: 0; padding: 0; }}
body {{ width: 7.5in; height: 10in; display: flex; align-items: center; justify-content: center; }}
img {{ max-width: 100%; max-height: 100%; object-fit: contain; }}
</style>
</head>
<body>
<img src='data:image/jpeg;base64,{base64Image}' alt='Enrollment form image' />
</body>
</html>";

            var htmlPath = Path.Combine(FileSystem.CacheDirectory, $"print-{Path.GetFileNameWithoutExtension(_loadedImageAsset) ?? "instruction"}.html");
            await File.WriteAllTextAsync(htmlPath, htmlContent);
            return htmlPath;
        }
    }
}
