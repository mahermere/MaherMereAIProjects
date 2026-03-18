using System;

namespace TripleSPOC.Models
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
                PreferredContactMethod = fields.Length > 12 ? fields[12] : string.Empty
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
                rec.PreferredContactMethod
            );
        }
    }
}