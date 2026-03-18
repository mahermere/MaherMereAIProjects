using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Triple_S_Maui_AEP.Services
{
    /// <summary>
    /// Service to manage active Enrollment records for dashboard tracking.
    /// Now uses encrypted SQLite database instead of CSV files.
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

        private static readonly string _legacyCsvFilePath = Path.Combine(FileSystem.AppDataDirectory, "data", "enrollments.csv");
        private static bool _isMigrated = false;

        /// <summary>
        /// Get all active enrollment records from encrypted database
        /// </summary>
        public static async Task<List<EnrollmentRecord>> GetActiveEnrollmentRecordsAsync()
        {
            await MigrateLegacyDataAsync();

            var db = await SecureDatabaseService.Instance.GetDatabaseAsync();
            var dbRecords = await db.Table<SecureDatabaseService.EnrollmentRecord>().ToListAsync();

            return dbRecords.Select(r => new EnrollmentRecord
            {
                EnrollmentNumber = r.EnrollmentNumber,
                FirstName = r.FirstName,
                LastName = r.LastName,
                DateCreated = r.DateCreated,
                FilePath = r.FilePath,
                IsUploaded = r.IsUploaded
            }).ToList();
        }

        /// <summary>
        /// Add a new enrollment record to encrypted database
        /// </summary>
        public static async Task AddEnrollmentRecordAsync(EnrollmentRecord record)
        {
            if (record == null)
                return;

            await MigrateLegacyDataAsync();

            var db = await SecureDatabaseService.Instance.GetDatabaseAsync();

            // Check if already exists
            var existing = await db.Table<SecureDatabaseService.EnrollmentRecord>()
                .Where(r => r.EnrollmentNumber == record.EnrollmentNumber)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                var dbRecord = new SecureDatabaseService.EnrollmentRecord
                {
                    EnrollmentNumber = record.EnrollmentNumber,
                    FirstName = record.FirstName,
                    LastName = record.LastName,
                    DateCreated = record.DateCreated,
                    FilePath = record.FilePath,
                    IsUploaded = record.IsUploaded
                };

                await db.InsertAsync(dbRecord);
                System.Diagnostics.Debug.WriteLine($"✅ Added enrollment record: {record.EnrollmentNumber}");
            }
        }

        /// <summary>
        /// Remove an enrollment record from encrypted database
        /// </summary>
        public static async Task RemoveEnrollmentRecordAsync(string enrollmentNumber)
        {
            var db = await SecureDatabaseService.Instance.GetDatabaseAsync();
            
            var record = await db.Table<SecureDatabaseService.EnrollmentRecord>()
                .Where(r => r.EnrollmentNumber == enrollmentNumber)
                .FirstOrDefaultAsync();

            if (record != null)
            {
                await db.DeleteAsync(record);
                System.Diagnostics.Debug.WriteLine($"🗑️ Removed enrollment record: {enrollmentNumber}");
            }
        }

        /// <summary>
        /// Update upload status for an enrollment record
        /// </summary>
        public static async Task UpdateUploadStatusAsync(string enrollmentNumber, bool isUploaded)
        {
            var db = await SecureDatabaseService.Instance.GetDatabaseAsync();
            
            var record = await db.Table<SecureDatabaseService.EnrollmentRecord>()
                .Where(r => r.EnrollmentNumber == enrollmentNumber)
                .FirstOrDefaultAsync();

            if (record != null)
            {
                record.IsUploaded = isUploaded;
                await db.UpdateAsync(record);
                System.Diagnostics.Debug.WriteLine($"✅ Updated enrollment: {enrollmentNumber}, IsUploaded={isUploaded}");
            }
        }

        /// <summary>
        /// Clear all enrollment records from encrypted database
        /// </summary>
        public static async Task ClearAsync()
        {
            var db = await SecureDatabaseService.Instance.GetDatabaseAsync();
            await db.DeleteAllAsync<SecureDatabaseService.EnrollmentRecord>();
            System.Diagnostics.Debug.WriteLine("🗑️ Cleared all enrollment records");
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
                System.Diagnostics.Debug.WriteLine("🔄 Migrating legacy CSV enrollment data to encrypted database...");

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
                    var enrollmentNumber = fields[0];
                    var existing = await db.Table<SecureDatabaseService.EnrollmentRecord>()
                        .Where(r => r.EnrollmentNumber == enrollmentNumber)
                        .FirstOrDefaultAsync();

                    if (existing == null)
                    {
                        var record = new SecureDatabaseService.EnrollmentRecord
                        {
                            EnrollmentNumber = enrollmentNumber,
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

                System.Diagnostics.Debug.WriteLine($"✅ Successfully migrated enrollment data. CSV archived to: {archivePath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Failed to migrate enrollment data: {ex.Message}");
            }
        }

        #region Legacy Synchronous Methods (Deprecated - for backward compatibility)

        [Obsolete("Use GetActiveEnrollmentRecordsAsync instead")]
        public static IReadOnlyList<EnrollmentRecord> ActiveEnrollmentRecords
        {
            get
            {
                // Return empty list and log warning
                System.Diagnostics.Debug.WriteLine("⚠️ ActiveEnrollmentRecords property is deprecated. Use GetActiveEnrollmentRecordsAsync() instead.");
                return Array.Empty<EnrollmentRecord>();
            }
        }

        [Obsolete("Use AddEnrollmentRecordAsync instead")]
        public static void AddEnrollmentRecord(EnrollmentRecord record)
        {
            System.Diagnostics.Debug.WriteLine("⚠️ AddEnrollmentRecord is deprecated. Use AddEnrollmentRecordAsync() instead.");
            Task.Run(async () => await AddEnrollmentRecordAsync(record)).Wait();
        }

        [Obsolete("Use RemoveEnrollmentRecordAsync instead")]
        public static void RemoveEnrollmentRecord(string enrollmentNumber)
        {
            System.Diagnostics.Debug.WriteLine("⚠️ RemoveEnrollmentRecord is deprecated. Use RemoveEnrollmentRecordAsync() instead.");
            Task.Run(async () => await RemoveEnrollmentRecordAsync(enrollmentNumber)).Wait();
        }

        [Obsolete("Use UpdateUploadStatusAsync instead")]
        public static void UpdateUploadStatus(string enrollmentNumber, bool isUploaded)
        {
            System.Diagnostics.Debug.WriteLine("⚠️ UpdateUploadStatus is deprecated. Use UpdateUploadStatusAsync() instead.");
            Task.Run(async () => await UpdateUploadStatusAsync(enrollmentNumber, isUploaded)).Wait();
        }

        [Obsolete("Use ClearAsync instead")]
        public static void Clear()
        {
            System.Diagnostics.Debug.WriteLine("⚠️ Clear is deprecated. Use ClearAsync() instead.");
            Task.Run(async () => await ClearAsync()).Wait();
        }

        [Obsolete("No longer needed with database approach")]
        public static void Reload()
        {
            System.Diagnostics.Debug.WriteLine("⚠️ Reload is deprecated and no longer needed with database approach.");
        }

        #endregion
    }
}
