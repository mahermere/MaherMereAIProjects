namespace Triple_S_AEP_MAUI_Forms.Services;

public sealed class SessionService
{
    private static SessionService? _instance;
    private static readonly object _lock = new();

    public static SessionService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new SessionService();
                }
            }
            return _instance;
        }
    }

    private SessionService() { }

    public string? HylandUsername { get; private set; }
    public string? HylandPassword { get; private set; }

    public void SetCredentials(string username, string password)
    {
        HylandUsername = username;
        HylandPassword = password;
    }

    public void ClearCredentials()
    {
        HylandUsername = null;
        HylandPassword = null;
    }
}
