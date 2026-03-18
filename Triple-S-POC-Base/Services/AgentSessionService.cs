using System;

namespace TripleSPOC.Services
{
    /// <summary>
    /// Service to manage the current logged-in agent's session
    /// </summary>
    public static class AgentSessionService
    {
        private static string? _currentAgentNPN;
        private static string? _currentAgentName;

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
        /// Check if an agent is currently logged in
        /// </summary>
        public static bool IsAgentLoggedIn => !string.IsNullOrWhiteSpace(_currentAgentNPN);

        /// <summary>
        /// Clear the current session
        /// </summary>
        public static void ClearSession()
        {
            _currentAgentNPN = null;
            _currentAgentName = null;
        }

        /// <summary>
        /// Set the current agent session
        /// </summary>
        public static void SetSession(string npn, string? agentName = null)
        {
            _currentAgentNPN = npn;
            _currentAgentName = agentName;
        }
    }
}
