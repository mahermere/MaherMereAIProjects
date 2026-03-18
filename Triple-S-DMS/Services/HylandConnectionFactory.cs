using System.Collections.Concurrent;

namespace TripleSService.Services
{
    public interface IHylandConnectionFactory
    {
        Task<IHylandConnection> CreateConnectionAsync();
        Task<IHylandConnection> CreateDisconnectedConnectionAsync();
        void ConfigureQueryMetering(int maxQueriesPerHour, int warningThreshold = 80);
        Task<QueryMeteringStatus> GetQueryMeteringStatusAsync();
        void Dispose();
    }

    public class HylandConnectionFactory : IHylandConnectionFactory, IDisposable
    {
        private readonly HylandConnectionConfiguration _config;
        private readonly SemaphoreSlim _connectionSemaphore;
        private readonly ConcurrentQueue<IHylandConnection> _connectionPool;
        private readonly QueryMeteringManager _queryMeteringManager;
        private readonly ILogger<HylandConnectionFactory> _logger;
        private bool _disposed = false;
        private bool _poolInitialized = false;
        private readonly object _poolInitLock = new object();

        public HylandConnectionFactory(
            HylandConnectionConfiguration config,
            ILogger<HylandConnectionFactory> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            _connectionSemaphore = new SemaphoreSlim(_config.MaxConnections, _config.MaxConnections);
            _connectionPool = new ConcurrentQueue<IHylandConnection>();
            _queryMeteringManager = new QueryMeteringManager(_config.MaxQueriesPerHour, logger);

            // Don't initialize pool during startup - make it lazy
            _logger.LogInformation("Hyland connection factory initialized (lazy connection creation enabled)");
        }

        public async Task<IHylandConnection> CreateConnectionAsync()
        {
            await _connectionSemaphore.WaitAsync();
            try
            {
                if (_connectionPool.TryDequeue(out var pooledConnection) && pooledConnection.IsConnected)
                {
                    _logger.LogDebug("Reusing pooled Hyland connection");
                    return pooledConnection;
                }

                var connection = CreateHylandConnection(useDisconnectedMode: false);
                if (!await connection.ConnectAsync())
                {
                    throw new HylandConnectionException("Failed to connect to Hyland OnBase");
                }
                _logger.LogDebug("Created and connected new Hyland connection");
                return connection;
            }
            catch (Exception ex)
            {
                _connectionSemaphore.Release();
                _logger.LogError(ex, "Failed to create Hyland connection");
                throw new HylandConnectionException("Failed to create Hyland connection", ex);
            }
        }

        private async Task EnsurePoolInitializedAsync()
        {
            if (_poolInitialized) return;
            
            lock (_poolInitLock)
            {
                if (_poolInitialized) return;
                
                _logger.LogInformation("Lazily initializing Hyland connection pool with {MinConnections} minimum connections", 
                    _config.MinConnections);
                
                try
                {
                    for (int i = 0; i < _config.MinConnections; i++)
                    {
                        var connection = CreateHylandConnection(useDisconnectedMode: true);
                        _connectionPool.Enqueue(connection);
                    }
                    
                    _poolInitialized = true;
                    _logger.LogInformation("Hyland connection pool initialized successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to initialize connection pool - will create connections on-demand");
                    _poolInitialized = true; // Mark as initialized even if failed, to avoid repeated attempts
                }
            }
        }

        public async Task<IHylandConnection> CreateDisconnectedConnectionAsync()
        {
            await _queryMeteringManager.CheckQueryLimitAsync();

            var connection = CreateHylandConnection(useDisconnectedMode: true);
            if (!await connection.ConnectAsync())
            {
                throw new HylandConnectionException("Failed to connect to Hyland OnBase (disconnected mode)");
            }
            await _queryMeteringManager.RecordQueryAsync();

            _logger.LogDebug("Created and connected disconnected Hyland connection");
            return connection;
        }

        private IHylandConnection CreateHylandConnection(bool useDisconnectedMode)
        {
            // In a real implementation, this would create actual Hyland Unity API connections
            // For now, we'll return a mock connection that demonstrates the pattern
            
            var connectionSettings = new HylandConnectionSettings
            {
                AppServerUrl = _config.AppServerUrl,
                Username = _config.Username,
                Password = _config.Password,
                DataSource = _config.DataSource,
                UseDisconnectedMode = useDisconnectedMode,
                UseQueryMetering = _config.UseQueryMetering,
                Timeout = _config.ConnectionTimeout
            };

            return new HylandConnection(connectionSettings, _logger);
        }

        private void ReturnConnectionToPool(IHylandConnection connection)
        {
            try
            {
                if (connection?.IsConnected == true && _connectionPool.Count < _config.MaxConnections)
                {
                    _connectionPool.Enqueue(connection);
                    _logger.LogDebug("Returned connection to pool");
                }
                else
                {
                    connection?.Dispose();
                    _logger.LogDebug("Disposed connection (pool full or connection invalid)");
                }
            }
            finally
            {
                _connectionSemaphore.Release();
            }
        }

        public void ConfigureQueryMetering(int maxQueriesPerHour, int warningThreshold = 80)
        {
            _queryMeteringManager.ConfigureQueryLimits(maxQueriesPerHour, warningThreshold);
            _logger.LogInformation("Configured query metering: {MaxQueries} queries per hour, {WarningThreshold}% warning threshold", 
                maxQueriesPerHour, warningThreshold);
        }

        public async Task<QueryMeteringStatus> GetQueryMeteringStatusAsync()
        {
            return await _queryMeteringManager.GetStatusAsync();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _logger.LogInformation("Disposing Hyland connection factory");
            
            while (_connectionPool.TryDequeue(out var connection))
            {
                connection?.Dispose();
            }

            _connectionSemaphore?.Dispose();
            _queryMeteringManager?.Dispose();
            
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
