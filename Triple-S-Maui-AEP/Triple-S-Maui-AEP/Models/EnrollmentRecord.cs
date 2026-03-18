using System;

namespace Triple_S_Maui_AEP.Models
{
    public class EnrollmentRecord
    {
        public string FirstName { get; set; } = string.Empty;
        public string MiddleInitial { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PrimaryPhone { get; set; } = string.Empty;
        public bool PrimaryPhoneIsMobile { get; set; }
        public string SecondaryPhone { get; set; } = string.Empty;
        public bool SecondaryPhoneIsMobile { get; set; }
        public string Email { get; set; } = string.Empty;
        public string MedicareNumber { get; set; } = string.Empty;
        public string SSN { get; set; } = string.Empty;
        public string PreferredContactMethod { get; set; } = string.Empty;

        // Address Fields
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string County { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public bool DifferentMailingAddress { get; set; }
        public string MailingAddress1 { get; set; } = string.Empty;
        public string MailingAddress2 { get; set; } = string.Empty;
        public string MailingCity { get; set; } = string.Empty;
        public string MailingState { get; set; } = string.Empty;
        public string MailingZipCode { get; set; } = string.Empty;

        // Signature Fields
        public string EnrolleeSignatureBase64 { get; set; } = string.Empty;
        public string EnrolleeSignatureFilePath { get; set; } = string.Empty;
        public bool UsesXMark { get; set; } = false;
        public string AgentSignatureBase64 { get; set; } = string.Empty;
        public string AgentSignatureFilePath { get; set; } = string.Empty;
        public string WitnessSignatureBase64 { get; set; } = string.Empty;
        public string WitnessSignatureFilePath { get; set; } = string.Empty;

        // Signature Audit Trail
        public string EnrolleeSignatureTimestamp { get; set; } = string.Empty;
        public string EnrolleeSignatureMethod { get; set; } = string.Empty;
        public string AgentSignatureTimestamp { get; set; } = string.Empty;
        public string WitnessSignatureTimestamp { get; set; } = string.Empty;

        public static EnrollmentRecord FromCsv(string[] fields)
        {
            return new EnrollmentRecord
            {
                FirstName = fields.Length > 0 ? fields[0] : string.Empty,
                MiddleInitial = fields.Length > 1 ? fields[1] : string.Empty,
                LastName = fields.Length > 2 ? fields[2] : string.Empty,
                DateOfBirth = fields.Length > 3 && DateTime.TryParse(fields[3], out var dob) ? dob : DateTime.MinValue,
                Gender = fields.Length > 4 ? fields[4] : string.Empty,
                PrimaryPhone = fields.Length > 5 ? fields[5] : string.Empty,
                PrimaryPhoneIsMobile = fields.Length > 6 && bool.TryParse(fields[6], out var pm) ? pm : false,
                SecondaryPhone = fields.Length > 7 ? fields[7] : string.Empty,
                SecondaryPhoneIsMobile = fields.Length > 8 && bool.TryParse(fields[8], out var sm) ? sm : false,
                Email = fields.Length > 9 ? fields[9] : string.Empty,
                MedicareNumber = fields.Length > 10 ? fields[10] : string.Empty,
                SSN = fields.Length > 11 ? fields[11] : string.Empty,
                PreferredContactMethod = fields.Length > 12 ? fields[12] : string.Empty,
                EnrolleeSignatureBase64 = fields.Length > 13 ? fields[13] : string.Empty,
                EnrolleeSignatureFilePath = fields.Length > 14 ? fields[14] : string.Empty,
                UsesXMark = fields.Length > 15 && bool.TryParse(fields[15], out var xMark) ? xMark : false,
                AgentSignatureBase64 = fields.Length > 16 ? fields[16] : string.Empty,
                AgentSignatureFilePath = fields.Length > 17 ? fields[17] : string.Empty,
                WitnessSignatureBase64 = fields.Length > 18 ? fields[18] : string.Empty,
                WitnessSignatureFilePath = fields.Length > 19 ? fields[19] : string.Empty,
                EnrolleeSignatureTimestamp = fields.Length > 20 ? fields[20] : string.Empty,
                EnrolleeSignatureMethod = fields.Length > 21 ? fields[21] : string.Empty,
                AgentSignatureTimestamp = fields.Length > 22 ? fields[22] : string.Empty,
                WitnessSignatureTimestamp = fields.Length > 23 ? fields[23] : string.Empty,
                Address1 = fields.Length > 24 ? fields[24] : string.Empty,
                Address2 = fields.Length > 25 ? fields[25] : string.Empty,
                City = fields.Length > 26 ? fields[26] : string.Empty,
                State = fields.Length > 27 ? fields[27] : string.Empty,
                County = fields.Length > 28 ? fields[28] : string.Empty,
                ZipCode = fields.Length > 29 ? fields[29] : string.Empty,
                DifferentMailingAddress = fields.Length > 30 && bool.TryParse(fields[30], out var differentMail) && differentMail,
                MailingAddress1 = fields.Length > 31 ? fields[31] : string.Empty,
                MailingAddress2 = fields.Length > 32 ? fields[32] : string.Empty,
                MailingCity = fields.Length > 33 ? fields[33] : string.Empty,
                MailingState = fields.Length > 34 ? fields[34] : string.Empty,
                MailingZipCode = fields.Length > 35 ? fields[35] : string.Empty
            };
        }

        public static string ToCsv(EnrollmentRecord rec)
        {
            return string.Join(",",
                rec.FirstName,
                rec.MiddleInitial,
                rec.LastName,
                rec.DateOfBirth.ToString("yyyy-MM-dd"),
                rec.Gender,
                rec.PrimaryPhone,
                rec.PrimaryPhoneIsMobile,
                rec.SecondaryPhone,
                rec.SecondaryPhoneIsMobile,
                rec.Email,
                rec.MedicareNumber,
                rec.SSN,
                rec.PreferredContactMethod,
                rec.EnrolleeSignatureBase64,
                rec.EnrolleeSignatureFilePath,
                rec.UsesXMark,
                rec.AgentSignatureBase64,
                rec.AgentSignatureFilePath,
                rec.WitnessSignatureBase64,
                rec.WitnessSignatureFilePath,
                rec.EnrolleeSignatureTimestamp,
                rec.EnrolleeSignatureMethod,
                rec.AgentSignatureTimestamp,
                rec.WitnessSignatureTimestamp,
                rec.Address1,
                rec.Address2,
                rec.City,
                rec.State,
                rec.County,
                rec.ZipCode,
                rec.DifferentMailingAddress,
                rec.MailingAddress1,
                rec.MailingAddress2,
                rec.MailingCity,
                rec.MailingState,
                rec.MailingZipCode
            );
        }
    }
}
