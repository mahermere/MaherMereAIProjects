namespace Triple_S_AEP_MAUI_Forms.Models;

public sealed class PdfFlattenRequest
{
    public string Base64TemplatePdf { get; set; } = string.Empty;

    public Dictionary<string, object?> Fields { get; set; } = new();

    public Dictionary<string, string> Images { get; set; } = new();
}
