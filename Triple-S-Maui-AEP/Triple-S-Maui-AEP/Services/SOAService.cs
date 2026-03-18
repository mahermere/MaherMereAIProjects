using System.Text;

namespace Triple_S_Maui_AEP.Services
{
    /// <summary>
    /// Service to manage SOA records for dashboard tracking.
    /// Now uses encrypted SQLite database instead of CSV files.
    /// </summary>
    public static class SOAService
    {
        public class SOARecord
        {
            public string SOANumber { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string BeneficiaryName => $"{FirstName} {LastName}";
            public DateTime DateCreated { get; set; }
            public string FilePath { get; set; } = string.Empty;
            public bool IsUploaded { get; set; }
        }

        private static readonly string _legacyCsvFilePath = Path.Combine(FileSystem.AppDataDirectory, "data", "soa.csv");
        private static bool _isMigrated = false;

        /// <summary>
        /// Get all active SOA records from encrypted database
        /// </summary>
        public static async Task<List<SOARecord>> GetActiveSOARecordsAsync()
        {
            await MigrateLegacyDataAsync();

            var db = await SecureDatabaseService.Instance.GetDatabaseAsync();
            var dbRecords = await db.Table<SecureDatabaseService.SOARecord>().ToListAsync();

            return dbRecords.Select(r => new SOARecord
            {
                SOANumber = r.SOANumber,
                FirstName = r.FirstName,
                LastName = r.LastName,
                DateCreated = r.DateCreated,
                FilePath = r.FilePath,
                IsUploaded = r.IsUploaded
            }).ToList();
        }

        /// <summary>
        /// Add a new SOA record to encrypted database
        /// </summary>
        public static async Task AddSOARecordAsync(SOARecord record)
        {
            if (record == null)
                return;

            await MigrateLegacyDataAsync();

            var db = await SecureDatabaseService.Instance.GetDatabaseAsync();

            // Check if already exists
            var existing = await db.Table<SecureDatabaseService.SOARecord>()
                .Where(r => r.SOANumber == record.SOANumber)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                var dbRecord = new SecureDatabaseService.SOARecord
                {
                    SOANumber = record.SOANumber,
                    FirstName = record.FirstName,
                    LastName = record.LastName,
                    DateCreated = record.DateCreated,
                    FilePath = record.FilePath,
                    IsUploaded = record.IsUploaded
                };

                await db.InsertAsync(dbRecord);
                System.Diagnostics.Debug.WriteLine($"✅ Added SOA record: {record.SOANumber}");
            }
        }

        /// <summary>
        /// Update upload status for a SOA record
        /// </summary>
        public static async Task UpdateUploadStatusAsync(string soaNumber, bool isUploaded)
        {
            var db = await SecureDatabaseService.Instance.GetDatabaseAsync();
            
            var record = await db.Table<SecureDatabaseService.SOARecord>()
                .Where(r => r.SOANumber == soaNumber)
                .FirstOrDefaultAsync();

            if (record != null)
            {
                record.IsUploaded = isUploaded;
                await db.UpdateAsync(record);
                System.Diagnostics.Debug.WriteLine($"✅ Updated SOA: {soaNumber}, IsUploaded={isUploaded}");
            }
        }

        /// <summary>
        /// Remove a SOA record from encrypted database
        /// </summary>
        public static async Task RemoveSOARecordAsync(string soaNumber)
        {
            var db = await SecureDatabaseService.Instance.GetDatabaseAsync();
            
            var record = await db.Table<SecureDatabaseService.SOARecord>()
                .Where(r => r.SOANumber == soaNumber)
                .FirstOrDefaultAsync();

            if (record != null)
            {
                await db.DeleteAsync(record);
                System.Diagnostics.Debug.WriteLine($"🗑️ Removed SOA record: {soaNumber}");
            }
        }

        /// <summary>
        /// Clear all SOA records from encrypted database
        /// </summary>
        public static async Task ClearAsync()
        {
            var db = await SecureDatabaseService.Instance.GetDatabaseAsync();
            await db.DeleteAllAsync<SecureDatabaseService.SOARecord>();
            System.Diagnostics.Debug.WriteLine("🗑️ Cleared all SOA records");
        }

        /// <summary>
        /// Migrate legacy CSV data to encrypted SQLite database (one-time operation)
        /// </summary>
        private static async Task MigrateLegacyDataAsync()
        {
            if (_isMigrated)
                return;

            _isMigrated = true;

            if (!File.Exists(_legacyCsvFilePath))
                return;

            try
            {
                System.Diagnostics.Debug.WriteLine("🔄 Migrating legacy CSV SOA data to encrypted database...");

                var lines = File.ReadAllLines(_legacyCsvFilePath);
                var db = await SecureDatabaseService.Instance.GetDatabaseAsync();

                foreach (var line in lines.Skip(1))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var fields = Utilities.CsvDataUtility.ParseCsvLine(line);
                    if (fields.Length < 6)
                        continue;

                    // Check if already exists in database
                    var soaNumber = fields[0];
                    var existing = await db.Table<SecureDatabaseService.SOARecord>()
                        .Where(r => r.SOANumber == soaNumber)
                        .FirstOrDefaultAsync();

                    if (existing == null)
                    {
                        var record = new SecureDatabaseService.SOARecord
                        {
                            SOANumber = soaNumber,
                            FirstName = fields[1],
                            LastName = fields[2],
                            DateCreated = DateTime.TryParse(fields[3], out var created) ? created : DateTime.Now,
                            FilePath = fields[4],
                            IsUploaded = bool.TryParse(fields[5], out var uploaded) && uploaded
                        };

                        await db.InsertAsync(record);
                    }
                }

                // Archive the old CSV file
                var archivePath = _legacyCsvFilePath + ".migrated";
                File.Move(_legacyCsvFilePath, archivePath);

                System.Diagnostics.Debug.WriteLine($"✅ Successfully migrated SOA data. CSV archived to: {archivePath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Failed to migrate SOA data: {ex.Message}");
            }
        }

        #region Legacy Synchronous Methods (Deprecated - for backward compatibility)

        [Obsolete("Use GetActiveSOARecordsAsync instead")]
        public static IReadOnlyList<SOARecord> ActiveSOARecords
        {
            get
            {
                System.Diagnostics.Debug.WriteLine("⚠️ ActiveSOARecords property is deprecated. Use GetActiveSOARecordsAsync() instead.");
                return Array.Empty<SOARecord>();
            }
        }

        [Obsolete("Use AddSOARecordAsync instead")]
        public static void AddSOARecord(SOARecord record)
        {
            System.Diagnostics.Debug.WriteLine("⚠️ AddSOARecord is deprecated. Use AddSOARecordAsync() instead.");
            Task.Run(async () => await AddSOARecordAsync(record)).Wait();
        }

        [Obsolete("Use UpdateUploadStatusAsync instead")]
        public static void UpdateUploadStatus(string soaNumber, bool isUploaded)
        {
            System.Diagnostics.Debug.WriteLine("⚠️ UpdateUploadStatus is deprecated. Use UpdateUploadStatusAsync() instead.");
            Task.Run(async () => await UpdateUploadStatusAsync(soaNumber, isUploaded)).Wait();
        }

        [Obsolete("No longer needed with database approach")]
        public static void Reload()
        {
            System.Diagnostics.Debug.WriteLine("⚠️ Reload is deprecated and no longer needed with database approach.");
        }

        #endregion
    }
}
