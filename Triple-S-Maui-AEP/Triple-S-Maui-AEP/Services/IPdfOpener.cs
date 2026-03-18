namespace Triple_S_Maui_AEP.Services
{
    public interface IPdfOpener
    {
        Task OpenPdfAsync(byte[] pdfData, string fileName);
    }
}
