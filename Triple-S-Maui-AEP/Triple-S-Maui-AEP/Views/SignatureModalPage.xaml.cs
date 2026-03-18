using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace Triple_S_Maui_AEP.Views
{
    /// <summary>
    /// Modal page for capturing signatures without scroll interference
    /// Solves Android touch/scroll conflicts by presenting signature pad in a modal
    /// </summary>
    public partial class SignatureModalPage : ContentPage
    {
        private string? _signatureBase64;
        private readonly TaskCompletionSource<string?> _completionSource = new();

        public SignatureModalPage(string title = "Signature", string instruction = "Please sign in the box below")
        {
            InitializeComponent();
            
            TitleLabel.Text = title;
            InstructionLabel.Text = instruction;
            
            // Monitor signature pad for changes to hide placeholder
            SignaturePad.StartInteraction += (s, e) => PlaceholderLabel.IsVisible = false;
        }

        /// <summary>
        /// Shows the modal and waits for signature completion
        /// Returns null if cancelled, otherwise returns base64 signature
        /// </summary>
        public Task<string?> GetSignatureAsync()
        {
            return _completionSource.Task;
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            SignaturePad.Clear();
            PlaceholderLabel.IsVisible = true;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                // Check if signature is empty
                if (SignaturePad.IsEmpty)
                {
                    await DisplayAlert("Empty Signature", "Please sign before saving.", "OK");
                    return;
                }

                // Capture the signature
                _signatureBase64 = await SignaturePad.GetSignatureAsBase64Async();

                if (string.IsNullOrEmpty(_signatureBase64))
                {
                    await DisplayAlert("Error", "Failed to capture signature. Please try again.", "OK");
                    return;
                }

                // Close modal and return signature
                _completionSource.SetResult(_signatureBase64);
                await Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving signature: {ex.Message}");
                await DisplayAlert("Error", $"Failed to save signature: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Handle Android back button
        /// </summary>
        protected override bool OnBackButtonPressed()
        {
            // User cancelled
            _completionSource.TrySetResult(null);
            return base.OnBackButtonPressed();
        }
    }
}
