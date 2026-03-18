// Android-only PDF generation utility for SOA using Syncfusion PDF.
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System;
using System.Drawing;
using System.IO;

namespace TripleS.SOA.AEP.UI.Platforms.Android
{
    public static class PdfService
    {
        public static byte[] GenerateSOAPdf(
            string beneficiaryName,
            DateTime dob,
            string medicareNumber,
            string phone,
            DateTime meetingDate,
            TimeSpan meetingTime,
            string meetingLocation,
            string agentName,
            string contactMethod,
            bool planAdvantage,
            bool planDrug,
            bool planSupplement,
            bool planDental,
            bool attested,
            byte[]? witnessSignatureBytes = null)
        {
            using var document = new PdfDocument();
            var page = document.Pages.Add();
            var font = new PdfStandardFont(PdfFontFamily.Helvetica, 16);
            var smallFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
            float y = 20;
            page.Graphics.DrawString("Signature of Authority (SOA)", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
            y += 40;
            page.Graphics.DrawString($"Beneficiary: {beneficiaryName}", smallFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
            y += 25;
            page.Graphics.DrawString($"Date of Birth: {dob:MM/dd/yyyy}", smallFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
            y += 25;
            page.Graphics.DrawString($"Medicare #: {medicareNumber}", smallFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
            y += 25;
            page.Graphics.DrawString($"Phone: {phone}", smallFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
            y += 25;
            page.Graphics.DrawString($"Meeting Date: {meetingDate:MM/dd/yyyy}", smallFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
            y += 25;
            page.Graphics.DrawString($"Meeting Time: {meetingTime}", smallFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
            y += 25;
            page.Graphics.DrawString($"Meeting Location: {meetingLocation}", smallFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
            y += 25;
            page.Graphics.DrawString($"Agent: {agentName}", smallFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
            y += 25;
            page.Graphics.DrawString($"Contact Method: {contactMethod}", smallFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
            y += 25;
            page.Graphics.DrawString("Plan Types:", smallFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
            y += 20;
            if (planAdvantage) { page.Graphics.DrawString("- Medicare Advantage (Part C)", smallFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(40, y)); y += 20; }
            if (planDrug) { page.Graphics.DrawString("- Prescription Drug Plan (Part D)", smallFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(40, y)); y += 20; }
            if (planSupplement) { page.Graphics.DrawString("- Medicare Supplement (Medigap)", smallFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(40, y)); y += 20; }
            if (planDental) { page.Graphics.DrawString("- Dental / Vision", smallFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(40, y)); y += 20; }
            y += 10;
            page.Graphics.DrawString($"Attestation: {(attested ? "Yes" : "No")}", smallFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
            y += 40;
            if (witnessSignatureBytes != null)
            {
                using var sigStream = new MemoryStream(witnessSignatureBytes);
                var sigImage = Syncfusion.Pdf.Graphics.PdfBitmap.FromStream(sigStream);
                page.Graphics.DrawString("Witness Signature:", smallFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
                y += 5;
                page.Graphics.DrawImage(sigImage, new Syncfusion.Drawing.RectangleF(20, y, 200, 60));
                y += 70;
            }
            else
            {
                page.Graphics.DrawString("Signature: __________________________", smallFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
                y += 40;
            }
            page.Graphics.DrawString($"Generated on {DateTime.Now:MM/dd/yyyy HH:mm}", smallFont, PdfBrushes.Gray, new Syncfusion.Drawing.PointF(20, y));
            using var ms = new MemoryStream();
            document.Save(ms);
            return ms.ToArray();
        }
    }
}