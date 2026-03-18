using System;

namespace Triple_S_Maui_AEP.Services
{
    /// <summary>
    /// Service to manage the current logged-in agent's session
    /// Stores credentials for use in DMS operations
    /// </summary>
    public static class AgentSessionService
    {
        private static string? _currentAgentNPN;
        private static string? _currentAgentName;
        private static string? _currentAgentPassword;

        /// <summary>
        /// Gets or sets the current logged-in agent's NPN
        /// </summary>
        public static string CurrentAgentNPN
        {
            get => _currentAgentNPN ?? "UNKNOWN";
            set => _currentAgentNPN = value;
        }

        /// <summary>
        /// Gets or sets the current logged-in agent's name
        /// </summary>
        public static string? CurrentAgentName
        {
            get => _currentAgentName;
            set => _currentAgentName = value;
        }

        /// <summary>
        /// Gets or sets the current logged-in agent's password (for use in DMS operations)
        /// ⚠️ SECURITY NOTE: This is stored in memory only. For production, consider using secure credential storage.
        /// </summary>
        public static string? CurrentAgentPassword
        {
            get => _currentAgentPassword;
            set => _currentAgentPassword = value;
        }

        /// <summary>
        /// Check if an agent is currently logged in
        /// </summary>
        public static bool IsAgentLoggedIn => !string.IsNullOrWhiteSpace(_currentAgentNPN);

        /// <summary>
        /// Clear the current session (clears all sensitive data)
        /// </summary>
        public static void ClearSession()
        {
            _currentAgentNPN = null;
            _currentAgentName = null;
            _currentAgentPassword = null;
            System.Diagnostics.Debug.WriteLine("✓ Agent session cleared (including credentials)");
        }

        /// <summary>
        /// Set the current agent session with credentials
        /// </summary>
        public static void SetSession(string npn, string? agentName = null, string? password = null)
        {
            _currentAgentNPN = npn;
            _currentAgentName = agentName;
            _currentAgentPassword = password;
            System.Diagnostics.Debug.WriteLine($"✓ Agent session set: NPN={npn}, HasPassword={!string.IsNullOrEmpty(password)}");
        }

        /// <summary>
        /// Get both NPN and Password for DMS operations
        /// </summary>
        public static (string NPN, string Password) GetDMSCredentials()
        {
            var npn = CurrentAgentNPN;
            var password = _currentAgentPassword ?? string.Empty;
            
            if (string.IsNullOrEmpty(password))
            {
                System.Diagnostics.Debug.WriteLine("⚠️ WARNING: DMS credentials requested but password is not set");
            }

            return (npn, password);
        }
    }
}
