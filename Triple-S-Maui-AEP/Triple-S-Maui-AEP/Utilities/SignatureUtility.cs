using System;

namespace Triple_S_Maui_AEP.Utilities
{
    /// <summary>
    /// Utility class for handling signature data and operations
    /// </summary>
    public static class SignatureUtility
    {
        /// <summary>
        /// Create an audit trail record for a signature
        /// </summary>
        public static SignatureAuditTrail CreateAuditTrail(
            string printedName,
            string formId,
            string signatureMethod = "Touch")
        {
            return new SignatureAuditTrail
            {
                Timestamp = DateTime.UtcNow.ToString("o"),
                PrintedName = printedName,
                SessionId = Guid.NewGuid().ToString(),
                FormId = formId,
                SignatureMethod = signatureMethod
            };
        }

        /// <summary>
        /// Validate signature base64 data
        /// </summary>
        public static bool IsValidSignature(string? signatureBase64)
        {
            if (string.IsNullOrWhiteSpace(signatureBase64))
                return false;

            try
            {
                byte[] imageBytes = Convert.FromBase64String(signatureBase64);
                // Check minimum size (at least a few bytes for a valid image)
                return imageBytes.Length > 100;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error validating signature: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Save signature to file system
        /// </summary>
        public static async Task<string> SaveSignatureAsync(
            string? signatureBase64,
            string subFolder = "signatures")
        {
            if (string.IsNullOrWhiteSpace(signatureBase64))
            {
                System.Diagnostics.Debug.WriteLine("Cannot save signature: signatureBase64 is null or empty");
                return string.Empty;
            }

            try
            {
                var appDataPath = FileSystem.AppDataDirectory;
                var signaturePath = Path.Combine(appDataPath, subFolder);

                if (!Directory.Exists(signaturePath))
                {
                    Directory.CreateDirectory(signaturePath);
                }

                var fileName = $"signature_{Guid.NewGuid()}.png";
                var filePath = Path.Combine(signaturePath, fileName);

                byte[] imageBytes = Convert.FromBase64String(signatureBase64);
                await File.WriteAllBytesAsync(filePath, imageBytes);

                return filePath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving signature: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Load signature from file as base64
        /// </summary>
        public static async Task<string> LoadSignatureAsBase64Async(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                System.Diagnostics.Debug.WriteLine("Cannot load signature: filePath is null or empty");
                return string.Empty;
            }

            try
            {
                if (File.Exists(filePath))
                {
                    byte[] imageBytes = await File.ReadAllBytesAsync(filePath);
                    return Convert.ToBase64String(imageBytes);
                }

                System.Diagnostics.Debug.WriteLine($"Signature file not found: {filePath}");
                return string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading signature: {ex.Message}");
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// Represents audit trail information for a signature
    /// </summary>
    public class SignatureAuditTrail
    {
        public string Timestamp { get; set; } = string.Empty;
        public string PrintedName { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string FormId { get; set; } = string.Empty;
        public string SignatureMethod { get; set; } = "Touch";
    }
}
