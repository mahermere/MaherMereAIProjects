using System.Collections.Generic;
using System.Linq;

namespace TripleS.SOA.AEP.UI.Services
{
    /// <summary>
    /// Service to manage active SOA numbers for dashboard and enrollment linking.
    /// </summary>
    public static class SOANumberService
    {
        public class SOARecord
        {
            public string SOANumber { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string BeneficiaryName => $"{FirstName} {LastName}";
            public System.DateTime DateCreated { get; set; }
        }

        private static readonly List<SOARecord> _activeSOARecords = new();
        public static IReadOnlyList<SOARecord> ActiveSOARecords => _activeSOARecords;

        public static void AddSOARecord(SOARecord record)
        {
            if (record != null && !_activeSOARecords.Any(r => r.SOANumber == record.SOANumber))
                _activeSOARecords.Add(record);
        }

        public static void RemoveSOARecord(string soaNumber)
        {
            var rec = _activeSOARecords.FirstOrDefault(r => r.SOANumber == soaNumber);
            if (rec != null)
                _activeSOARecords.Remove(rec);
        }

        public static void Clear()
        {
            _activeSOARecords.Clear();
        }
    }
}
