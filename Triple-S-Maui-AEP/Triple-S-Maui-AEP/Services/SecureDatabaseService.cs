using SQLite;
using System.Security.Cryptography;

namespace Triple_S_Maui_AEP.Services
{
    /// <summary>
    /// Secure database service that manages encrypted SQLite database
    /// and stores encryption keys in SecureStorage
    /// </summary>
    public class SecureDatabaseService
    {
        private const string DB_ENCRYPTION_KEY = "TripleS_AEP_DB_Key";
        private const string DB_NAME = "triples_aep_secure.db3";
        private static SecureDatabaseService? _instance;
        private static readonly object _lock = new();
        private SQLiteAsyncConnection? _database;
        private string? _databasePath;
        private bool _isInitialized;

        private SecureDatabaseService()
        {
        }

        public static SecureDatabaseService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new SecureDatabaseService();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Initialize the encrypted database
        /// </summary>
        public async Task InitializeAsync()
        {
            if (_isInitialized)
                return;

            try
            {
                // Get or create encryption key from SecureStorage
                var encryptionKey = await GetOrCreateEncryptionKeyAsync();

                // Set database path
                _databasePath = Path.Combine(FileSystem.AppDataDirectory, "data", DB_NAME);

                // Ensure directory exists
                var directory = Path.GetDirectoryName(_databasePath);
                if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Initialize SQLite with encryption
                var options = new SQLiteConnectionString(_databasePath, 
                    SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex,
                    storeDateTimeAsTicks: true,
                    key: encryptionKey);

                _database = new SQLiteAsyncConnection(options);

                // Create tables
                await _database.CreateTableAsync<EnrollmentRecord>();
                await _database.CreateTableAsync<SOARecord>();

                _isInitialized = true;

                System.Diagnostics.Debug.WriteLine($"✅ Secure database initialized at: {_databasePath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Failed to initialize secure database: {ex.Message}");
                throw new InvalidOperationException("Failed to initialize secure database", ex);
            }
        }

        /// <summary>
        /// Get or create encryption key stored in SecureStorage
        /// </summary>
        private async Task<string> GetOrCreateEncryptionKeyAsync()
        {
            try
            {
                // Try to retrieve existing key
                var existingKey = await SecureStorage.GetAsync(DB_ENCRYPTION_KEY);
                
                if (!string.IsNullOrEmpty(existingKey))
                {
                    System.Diagnostics.Debug.WriteLine("🔑 Retrieved existing encryption key from SecureStorage");
                    return existingKey;
                }

                // Generate new encryption key (256-bit)
                var keyBytes = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(keyBytes);
                }

                var newKey = Convert.ToBase64String(keyBytes);
                
                // Store in SecureStorage
                await SecureStorage.SetAsync(DB_ENCRYPTION_KEY, newKey);
                
                System.Diagnostics.Debug.WriteLine("🔑 Generated and stored new encryption key in SecureStorage");
                return newKey;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Failed to get/create encryption key: {ex.Message}");
                throw new InvalidOperationException("Failed to manage encryption key", ex);
            }
        }

        /// <summary>
        /// Get the database connection (ensures initialization)
        /// </summary>
        public async Task<SQLiteAsyncConnection> GetDatabaseAsync()
        {
            if (!_isInitialized)
            {
                await InitializeAsync();
            }

            return _database ?? throw new InvalidOperationException("Database is not initialized");
        }

        /// <summary>
        /// Close database connection
        /// </summary>
        public async Task CloseAsync()
        {
            if (_database != null)
            {
                await _database.CloseAsync();
                _database = null;
                _isInitialized = false;
                System.Diagnostics.Debug.WriteLine("🔒 Database connection closed");
            }
        }

        /// <summary>
        /// Delete the database (for testing or reset purposes)
        /// </summary>
        public async Task DeleteDatabaseAsync()
        {
            await CloseAsync();

            if (!string.IsNullOrEmpty(_databasePath) && File.Exists(_databasePath))
            {
                File.Delete(_databasePath);
                System.Diagnostics.Debug.WriteLine("🗑️ Database file deleted");
            }

            // Optionally remove encryption key
            SecureStorage.Remove(DB_ENCRYPTION_KEY);
            System.Diagnostics.Debug.WriteLine("🗑️ Encryption key removed from SecureStorage");
        }

        #region Database Models

        [Table("Enrollments")]
        public class EnrollmentRecord
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }

            [Indexed, Unique]
            public string EnrollmentNumber { get; set; } = string.Empty;

            public string FirstName { get; set; } = string.Empty;

            public string LastName { get; set; } = string.Empty;

            public DateTime DateCreated { get; set; }

            public string FilePath { get; set; } = string.Empty;

            public bool IsUploaded { get; set; }

            [Ignore]
            public string BeneficiaryName => $"{FirstName} {LastName}";
        }

        [Table("SOA")]
        public class SOARecord
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }

            [Indexed, Unique]
            public string SOANumber { get; set; } = string.Empty;

            public string FirstName { get; set; } = string.Empty;

            public string LastName { get; set; } = string.Empty;

            public DateTime DateCreated { get; set; }

            public string FilePath { get; set; } = string.Empty;

            public bool IsUploaded { get; set; }

            [Ignore]
            public string BeneficiaryName => $"{FirstName} {LastName}";
        }

        #endregion
    }
}
