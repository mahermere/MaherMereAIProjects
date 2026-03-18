using System;

namespace Triple_S_Maui_AEP.Models
{
    public class SOAFirstPageRecord
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PrimaryPhone { get; set; } = string.Empty;
        public string MedicareNumber { get; set; } = string.Empty;

        // Signature Fields
        public string EmployeeSignatureBase64 { get; set; } = string.Empty;
        public string EmployeeSignatureFilePath { get; set; } = string.Empty;
        public bool UsesXMark { get; set; } = false;
        public string AgentSignatureBase64 { get; set; } = string.Empty;
        public string AgentSignatureFilePath { get; set; } = string.Empty;

        // Signature Audit Trail
        public string EmployeeSignatureTimestamp { get; set; } = string.Empty;
        public string EmployeeSignatureMethod { get; set; } = string.Empty;
        public string AgentSignatureTimestamp { get; set; } = string.Empty;

        public static SOAFirstPageRecord FromCsv(string[] fields)
        {
            return new SOAFirstPageRecord
            {
                FirstName = fields.Length > 0 ? fields[0] : string.Empty,
                LastName = fields.Length > 1 ? fields[1] : string.Empty,
                DateOfBirth = fields.Length > 2 && DateTime.TryParse(fields[2], out var dob) ? dob : DateTime.MinValue,
                Gender = fields.Length > 3 ? fields[3] : string.Empty,
                PrimaryPhone = fields.Length > 4 ? fields[4] : string.Empty,
                MedicareNumber = fields.Length > 5 ? fields[5] : string.Empty
            };
        }

        public static string ToCsv(SOAFirstPageRecord rec)
        {
            return string.Join(",",
                rec.FirstName,
                rec.LastName,
                rec.DateOfBirth.ToString("yyyy-MM-dd"),
                rec.Gender,
                rec.PrimaryPhone,
                rec.MedicareNumber
            );
        }
    }
}
