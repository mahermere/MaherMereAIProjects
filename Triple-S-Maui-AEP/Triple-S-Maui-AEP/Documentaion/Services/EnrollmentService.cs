using System.Collections.Generic;
using System.Linq;

namespace TripleS.SOA.AEP.UI.Services
{
    /// <summary>
    /// Service to manage active Enrollment records for dashboard tracking.
    /// </summary>
    public static class EnrollmentService
    {
        public class EnrollmentRecord
        {
            public string EnrollmentNumber { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string BeneficiaryName => $"{FirstName} {LastName}";
            public System.DateTime DateCreated { get; set; }
            public string FilePath { get; set; } = string.Empty;
            public bool IsUploaded { get; set; } = false;
        }

        private static readonly List<EnrollmentRecord> _activeEnrollmentRecords = new();
        public static IReadOnlyList<EnrollmentRecord> ActiveEnrollmentRecords => _activeEnrollmentRecords;

        public static void AddEnrollmentRecord(EnrollmentRecord record)
        {
            if (record != null && !_activeEnrollmentRecords.Any(r => r.EnrollmentNumber == record.EnrollmentNumber))
                _activeEnrollmentRecords.Add(record);
        }

        public static void RemoveEnrollmentRecord(string enrollmentNumber)
        {
            var rec = _activeEnrollmentRecords.FirstOrDefault(r => r.EnrollmentNumber == enrollmentNumber);
            if (rec != null)
                _activeEnrollmentRecords.Remove(rec);
        }

        public static void Clear()
        {
            _activeEnrollmentRecords.Clear();
        }
    }
}
