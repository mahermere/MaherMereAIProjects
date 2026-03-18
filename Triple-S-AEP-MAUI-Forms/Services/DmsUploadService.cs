using System.Net.Http.Json;
using System.Text.Json;
using Triple_S_AEP_MAUI_Forms.Models;

namespace Triple_S_AEP_MAUI_Forms.Services;

public sealed class DmsUploadService
{
    private static readonly Uri UploadEndpoint = new("https://localhost:44304/api/document/upload");
    private static readonly Uri VerifyUserEndpoint = new("https://localhost:44304/api/document/verify-user");

    public const int FileTypeIdPdf = 16;
    public const int DocumentTypeIdSoa = 841;
    public const int DocumentTypeIdEnrollment = 842;
    public const int DocumentTypeIdWorkingAgeSurvey = 869;

    private readonly HttpClient _httpClient;

    public DmsUploadService()
    {
#if DEBUG
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        _httpClient = new HttpClient(handler);
#else
        _httpClient = new HttpClient();
#endif
    }

    public async Task<(bool IsSuccess, string? DocumentId, string Message)> UploadPdfAsync(
        int documentTypeId,
        byte[] pdfBytes,
        List<DmsKeyword>? keywords = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var body = new DmsUploadRequest
            {
                DocumentTypeId = documentTypeId,
                FileTypeId = FileTypeIdPdf,
                Base64Document = Convert.ToBase64String(pdfBytes),
                Keywords = keywords ?? []
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, UploadEndpoint)
            {
                Content = JsonContent.Create(body)
            };

            var session = SessionService.Instance;
            if (!string.IsNullOrEmpty(session.HylandUsername))
                httpRequest.Headers.TryAddWithoutValidation("Hyland-Username", session.HylandUsername);
            if (!string.IsNullOrEmpty(session.HylandPassword))
                httpRequest.Headers.TryAddWithoutValidation("Hyland-Password", session.HylandPassword);

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return (false, null, $"DMS upload failed ({(int)response.StatusCode}): {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var documentId = TryExtractDocumentId(content) ?? content;
            return (true, documentId, "OK");
        }
        catch (Exception ex)
        {
            return (false, null, ex.Message);
        }
    }

    public async Task<(bool IsSuccess, string Message)> VerifyUserAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, VerifyUserEndpoint);
            httpRequest.Headers.TryAddWithoutValidation("Hyland-Username", username);
            httpRequest.Headers.TryAddWithoutValidation("Hyland-Password", password);

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return (false, $"Login failed ({(int)response.StatusCode}): {errorContent}");
            }

            return (true, "OK");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    private static string? TryExtractDocumentId(string responseContent)
    {
        if (string.IsNullOrWhiteSpace(responseContent))
            return null;

        var trimmed = responseContent.Trim();
        if (!trimmed.StartsWith('{'))
            return trimmed.Trim('"');

        try
        {
            using var doc = JsonDocument.Parse(trimmed);
            var root = doc.RootElement;
            foreach (var key in new[] { "DocumentId", "documentId", "Id", "id", "DocumentID", "documentID" })
            {
                if (root.TryGetProperty(key, out var val))
                    return val.ToString();
            }
        }
        catch { }

        return null;
    }
}
