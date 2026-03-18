namespace Triple_S_Maui_AEP.Services
{
    /// <summary>
    /// Service for managing SOA (Signature of Authority) number generation
    /// </summary>
    public class SOANumberService
    {
        private static int _soaCounter = 1000;

        /// <summary>
        /// Generates an SOA ID for Hyland OnBase (TSA-SOA ID keyword)
        /// Format: SOA-{YYMM}-{NNNNN}-{NPN} (max 20 characters)
        /// Example: SOA-2401-00001-5678 (18 characters with 4-digit NPN)
        /// This format fits the 20-character limit for TSA-SOA ID in DMS and includes agent NPN for uniqueness
        /// </summary>
        public string GenerateSOANumber()
        {
            _soaCounter++;
            var yearMonth = DateTime.Now.ToString("yymm");
            var sequence = _soaCounter.ToString("D5");  // 5-digit sequence
            var npn = AgentSessionService.CurrentAgentNPN ?? "0000";
            
            // Use last 4 digits of NPN for uniqueness (or all if less than 4)
            var npnPart = npn.Length >= 4 ? npn.Substring(npn.Length - 4) : npn.PadLeft(4, '0');
            
            var soaId = $"SOA-{yearMonth}-{sequence}-{npnPart}";
            
            // Safety check - ensure it doesn't exceed 20 characters
            if (soaId.Length > 20)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ SOA ID exceeds limit: {soaId} ({soaId.Length} chars), truncating...");
                return soaId.Substring(0, 20);
            }
            
            System.Diagnostics.Debug.WriteLine($"Generated SOA ID: {soaId} ({soaId.Length} chars) ✓");
            return soaId;
        }

        /// <summary>
        /// Generates an SOA number with explicit NPN (for testing or special cases)
        /// Format: SOA-{YYMM}-{NNNNN}-{NPN}
        /// </summary>
        public string GenerateSOANumber(string npn)
        {
            _soaCounter++;
            var yearMonth = DateTime.Now.ToString("yymm");
            var sequence = _soaCounter.ToString("D5");
            
            // Use last 4 digits of NPN
            var npnPart = npn.Length >= 4 ? npn.Substring(npn.Length - 4) : npn.PadLeft(4, '0');
            
            var soaId = $"SOA-{yearMonth}-{sequence}-{npnPart}";
            
            if (soaId.Length > 20)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ SOA ID exceeds limit: {soaId} ({soaId.Length} chars), truncating...");
                return soaId.Substring(0, 20);
            }
            
            return soaId;
        }

        public string GenerateEnrollmentNumber()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd");
            var random = new Random().Next(100000, 999999);
            return $"ENR{timestamp}{random}";
        }

        public bool IsValidSOANumber(string soaNumber)
        {
            return !string.IsNullOrWhiteSpace(soaNumber) && soaNumber.StartsWith("SOA");
        }

        public bool IsValidEnrollmentNumber(string enrollmentNumber)
        {
            return !string.IsNullOrWhiteSpace(enrollmentNumber) && enrollmentNumber.StartsWith("ENR");
        }

        /// <summary>
        /// Validates that an SOA ID fits within the 20-character DMS limit
        /// </summary>
        public bool IsValidSOAIdLength(string soaId)
        {
            return !string.IsNullOrWhiteSpace(soaId) && soaId.Length <= 20;
        }
    }
}
