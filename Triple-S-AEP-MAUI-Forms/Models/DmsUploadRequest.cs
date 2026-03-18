namespace Triple_S_AEP_MAUI_Forms.Models;

public sealed class DmsUploadRequest
{
    public int DocumentTypeId { get; set; }
    public int FileTypeId { get; set; } = 16;
    public string Base64Document { get; set; } = string.Empty;
    public List<DmsKeyword> Keywords { get; set; } = [];
}

public sealed class DmsKeyword
{
    public int KeywordTypeId { get; set; }
    public string Value { get; set; } = string.Empty;
}
