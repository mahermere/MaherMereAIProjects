using System.Collections.Concurrent;

namespace TripleSService.Services
{
    public class QueryMeteringManager : IDisposable
    {
        private readonly ConcurrentQueue<DateTime> _queryTimestamps;
        private readonly SemaphoreSlim _queryLimitSemaphore;
        private readonly ILogger _logger;
        private readonly Timer _cleanupTimer;
        
        private int _maxQueriesPerHour;
        private int _warningThresholdPercent;
        private bool _disposed = false;

        public QueryMeteringManager(int maxQueriesPerHour, ILogger logger)
        {
            _maxQueriesPerHour = maxQueriesPerHour;
            _warningThresholdPercent = 80; // Default warning at 80%
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            _queryTimestamps = new ConcurrentQueue<DateTime>();
            _queryLimitSemaphore = new SemaphoreSlim(1, 1);
            
            // Cleanup old timestamps every 5 minutes
            _cleanupTimer = new Timer(CleanupOldTimestamps, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
            
            _logger.LogInformation("Query metering manager initialized with {MaxQueries} queries per hour", maxQueriesPerHour);
        }

        public async Task CheckQueryLimitAsync()
        {
            await _queryLimitSemaphore.WaitAsync();
            try
            {
                var currentTime = DateTime.UtcNow;
                var oneHourAgo = currentTime.AddHours(-1);
                
                // Count queries in the last hour
                var recentQueries = _queryTimestamps.Count(timestamp => timestamp > oneHourAgo);
                
                if (recentQueries >= _maxQueriesPerHour)
                {
                    _logger.LogWarning("Query limit exceeded: {CurrentQueries}/{MaxQueries} queries in the last hour", 
                        recentQueries, _maxQueriesPerHour);
                    throw new QueryLimitExceededException($"Query limit of {_maxQueriesPerHour} queries per hour has been exceeded");
                }

                var warningThreshold = (_maxQueriesPerHour * _warningThresholdPercent) / 100;
                if (recentQueries >= warningThreshold)
                {
                    _logger.LogWarning("Approaching query limit: {CurrentQueries}/{MaxQueries} queries in the last hour ({Percentage}%)", 
                        recentQueries, _maxQueriesPerHour, (recentQueries * 100) / _maxQueriesPerHour);
                }
            }
            finally
            {
                _queryLimitSemaphore.Release();
            }
        }

        public async Task RecordQueryAsync()
        {
            _queryTimestamps.Enqueue(DateTime.UtcNow);
            await Task.CompletedTask;
        }

        public void ConfigureQueryLimits(int maxQueriesPerHour, int warningThresholdPercent)
        {
            _maxQueriesPerHour = maxQueriesPerHour;
            _warningThresholdPercent = Math.Min(100, Math.Max(0, warningThresholdPercent));
            
            _logger.LogInformation("Updated query limits: {MaxQueries} per hour, {WarningThreshold}% warning threshold", 
                maxQueriesPerHour, _warningThresholdPercent);
        }

        public async Task<QueryMeteringStatus> GetStatusAsync()
        {
            await _queryLimitSemaphore.WaitAsync();
            try
            {
                var currentTime = DateTime.UtcNow;
                var oneHourAgo = currentTime.AddHours(-1);
                
                var recentQueries = _queryTimestamps.Count(timestamp => timestamp > oneHourAgo);
                var utilizationPercent = (_maxQueriesPerHour > 0) ? (recentQueries * 100) / _maxQueriesPerHour : 0;
                
                return new QueryMeteringStatus
                {
                    MaxQueriesPerHour = _maxQueriesPerHour,
                    QueriesUsedInLastHour = recentQueries,
                    RemainingQueries = Math.Max(0, _maxQueriesPerHour - recentQueries),
                    UtilizationPercent = utilizationPercent,
                    WarningThresholdPercent = _warningThresholdPercent,
                    IsNearLimit = utilizationPercent >= _warningThresholdPercent,
                    IsAtLimit = recentQueries >= _maxQueriesPerHour,
                    LastResetTime = oneHourAgo,
                    NextResetTime = GetNextResetTime()
                };
            }
            finally
            {
                _queryLimitSemaphore.Release();
            }
        }

        private DateTime GetNextResetTime()
        {
            if (!_queryTimestamps.TryPeek(out var oldestQuery))
                return DateTime.UtcNow.AddHours(1);
                
            return oldestQuery.AddHours(1);
        }

        private void CleanupOldTimestamps(object? state)
        {
            try
            {
                var oneHourAgo = DateTime.UtcNow.AddHours(-1);
                var removedCount = 0;
                
                while (_queryTimestamps.TryPeek(out var timestamp) && timestamp <= oneHourAgo)
                {
                    if (_queryTimestamps.TryDequeue(out _))
                        removedCount++;
                }
                
                if (removedCount > 0)
                {
                    _logger.LogDebug("Cleaned up {RemovedCount} old query timestamps", removedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during query timestamp cleanup");
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            
            _cleanupTimer?.Dispose();
            _queryLimitSemaphore?.Dispose();
            
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    public class QueryMeteringStatus
    {
        public int MaxQueriesPerHour { get; set; }
        public int QueriesUsedInLastHour { get; set; }
        public int RemainingQueries { get; set; }
        public int UtilizationPercent { get; set; }
        public int WarningThresholdPercent { get; set; }
        public bool IsNearLimit { get; set; }
        public bool IsAtLimit { get; set; }
        public DateTime LastResetTime { get; set; }
        public DateTime NextResetTime { get; set; }
    }

    public class QueryLimitExceededException : Exception
    {
        public QueryLimitExceededException(string message) : base(message) { }
        public QueryLimitExceededException(string message, Exception innerException) : base(message, innerException) { }
    }
}