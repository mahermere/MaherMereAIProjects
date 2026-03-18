using System.Net.Http.Json;
using System.Text.Json;
using Triple_S_AEP_MAUI_Forms.Models;

namespace Triple_S_AEP_MAUI_Forms.Services;

public sealed class PdfFlattenService
{
    private static readonly Uri FlattenEndpoint = new("https://localhost:44304/api/pdf/flatten");

    private readonly HttpClient _httpClient;

    public PdfFlattenService()
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

    public async Task<(bool IsSuccess, byte[]? PdfBytes, string Message)> FlattenAsync(PdfFlattenRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(FlattenEndpoint, request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return (false, null, errorContent);
            }

            var mediaType = response.Content.Headers.ContentType?.MediaType;
            if (string.Equals(mediaType, "application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                var pdfBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                return (true, pdfBytes, "OK");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var base64 = TryExtractBase64(content);
            if (string.IsNullOrWhiteSpace(base64))
            {
                return (false, null, "API did not return a base64 PDF payload.");
            }

            var cleanBase64 = StripDataUriPrefix(base64);
            var bytes = Convert.FromBase64String(cleanBase64);
            return (true, bytes, "OK");
        }
        catch (Exception ex)
        {
            return (false, null, ex.Message);
        }
    }

    private static string? TryExtractBase64(string responseContent)
    {
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return null;
        }

        var trimmed = responseContent.Trim();

        if (!trimmed.StartsWith("{"))
        {
            if (trimmed.StartsWith('"') && trimmed.EndsWith('"'))
            {
                try
                {
                    return JsonSerializer.Deserialize<string>(trimmed);
                }
                catch
                {
                    return trimmed.Trim('"');
                }
            }

            return trimmed;
        }

        try
        {
            using var doc = JsonDocument.Parse(trimmed);
            var root = doc.RootElement;
            var candidates = new[]
            {
                "Base64Pdf", "base64Pdf", "PdfBase64", "pdfBase64", "Base64", "base64", "Data", "data", "Result", "result"
            };

            foreach (var key in candidates)
            {
                if (root.TryGetProperty(key, out var valueElement) && valueElement.ValueKind == JsonValueKind.String)
                {
                    return valueElement.GetString();
                }
            }
        }
        catch
        {
            // ignored
        }

        return null;
    }

    private static string StripDataUriPrefix(string value)
    {
        const string marker = "base64,";
        var index = value.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        return index >= 0 ? value[(index + marker.Length)..] : value;
    }
}
