namespace TripleS.SOA.AEP.UI.Configuration
{
    /// <summary>
    /// Configuration settings for the application
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// Triple-S Document Management System (DMS) endpoint
        /// Change this for different environments (Dev, QA, Prod)
        /// </summary>
        public static string DMSEndpoint { get; set; } = "https://localhost:44304/api/document/upload";

        // Hyland Authentication - Use environment variables for security
        public static string HylandUsername { get; set; } = 
            Environment.GetEnvironmentVariable("HYLAND_USERNAME") ?? "mahmer";
        
        public static string HylandPassword { get; set; } = 
            Environment.GetEnvironmentVariable("HYLAND_PASSWORD") ?? "password";

        /// <summary>
        /// Timeout for DMS uploads in minutes
        /// </summary>
        public static int DMSUploadTimeoutMinutes { get; set; } = 5;

        /// <summary>
        /// Maximum file size for upload in MB
        /// </summary>
        public static int MaxFileSizeMB { get; set; } = 50;
    }
}
