using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
using System;
using System.Diagnostics.Metrics;
using System.IO;

namespace PdfFieldExtractor
{
    class Program
    {
        static string GetFieldValue(PdfField field)
        {
            try
            {
                return field switch
                {
                    PdfLoadedTextBoxField textBox => textBox.Text ?? string.Empty,
                    PdfLoadedCheckBoxField checkBox => checkBox.Checked.ToString(),
                    PdfLoadedComboBoxField comboBox => comboBox.SelectedValue?.ToString() ?? string.Empty,
                    PdfLoadedListBoxField listBox => listBox.SelectedValue?.ToString() ?? string.Empty,
                    PdfLoadedRadioButtonListField radioButton => $"Selected: {radioButton.SelectedValue ?? "(none)"}",
                    _ => string.Empty
                };
            }
            catch
            {
                return string.Empty;
            }
        }

        static void WriteFieldInfo(StreamWriter writer, PdfField field)
        {
            if (field is PdfLoadedRadioButtonListField radioButtonList)
            {
                writer.WriteLine($"Field: {field.Name}, Type: {field.GetType().Name}, Selected Value: {radioButtonList.SelectedValue ?? "(none)"}");
                
                // Output each radio button option
                for (int i = 0; i < radioButtonList.Items.Count; i++)
                {
                    var item = radioButtonList.Items[i];
                    writer.WriteLine($"  Option {i}: Value = {item.Value}");
                }
            }
            else
            {
                string fieldValue = GetFieldValue(field);
                writer.WriteLine($"Field: {field.Name}, Type: {field.GetType().Name}, Value: {fieldValue}");
            }
        }

        static void Main(string[] args)
        {

                foreach (var filePath in Directory.GetFiles(@"C:\Users\maher\source\repos\Triple-S-AEP-MAUI-Forms\Resources\Raw\", "*Fillable.pdf"))
                {
                    if (!System.IO.File.Exists(filePath))
                    {
                        Console.WriteLine($"File not found: {filePath}");
                        continue;
                    }

                    using (var streamWriter = new StreamWriter(filePath + ".txt", append: true))
                    using (var templateStream = System.IO.File.OpenRead(filePath))
                    {
                        var loadedDoc = new PdfLoadedDocument(templateStream);

                        streamWriter.WriteLine("Fields for " + filePath);
                        foreach (PdfField field in loadedDoc.Form.Fields)
                        {
                            WriteFieldInfo(streamWriter, field);
                        }
                    }
                }
            
        }
    }
}
