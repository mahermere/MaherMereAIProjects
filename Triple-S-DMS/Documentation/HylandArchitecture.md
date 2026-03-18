# Hyland Document Management Service - Architecture Design

## Overview

This project implements a robust Hyland Document Management Service using the Factory pattern with advanced query metering and Disconnected Mode capabilities. The design follows best practices for high-volume document operations while managing licensing costs and ensuring optimal performance.

## Key Features

### 🏭 Factory Pattern Architecture
- **Connection Factory**: Centralized connection management with pooling
- **Service Factory**: Abstracted service creation for different document operations
- **Flexible Configuration**: Environment-based configuration for different deployment scenarios

### 📊 Query Metering Management
- **Hourly Query Limits**: Configurable limits to manage licensing costs
- **Real-time Monitoring**: Track query usage in real-time
- **Warning Thresholds**: Proactive alerts before hitting limits
- **Automatic Cleanup**: Automatic removal of old query timestamps

### 🔄 Disconnected Mode Support
- **Stateless Operations**: Optimized for high-volume scenarios
- **Session Pool Management**: Efficient session reuse
- **Connection Limitations Handling**: Proper handling of disconnected mode restrictions

## Architecture Components

### Core Services

#### HylandConnectionFactory
```csharp
IHylandConnectionFactory
├── CreateConnectionAsync() // Standard connected mode
├── CreateDisconnectedConnectionAsync() // Optimized for high volume
├── ConfigureQueryMetering() // Runtime configuration
└── GetQueryMeteringStatusAsync() // Monitoring
```

**Key Features:**
- Connection pooling with min/max connection limits
- Automatic query limit enforcement
- Disconnected mode optimization
- Thread-safe operations

#### QueryMeteringManager
```csharp
QueryMeteringManager
├── CheckQueryLimitAsync() // Pre-query validation
├── RecordQueryAsync() // Usage tracking
├── GetStatusAsync() // Real-time status
└── ConfigureQueryLimits() // Runtime reconfiguration
```

**Capabilities:**
- Sliding window query tracking (1-hour rolling)
- Configurable warning thresholds
- Automatic timestamp cleanup
- Thread-safe concurrent access

#### HylandDocumentService
```csharp
IHylandDocumentService
├── GetAllDocumentsAsync()
├── GetDocumentByIdAsync()
├── CreateDocumentAsync()
├── UpdateDocumentAsync()
├── DeleteDocumentAsync()
├── SearchDocumentsAsync()
├── ArchiveDocumentAsync()
├── GetDocumentContentAsync()
├── GetQueryMeteringStatusAsync()
└── ConfigureQueryMetering()
```

## Disconnected Mode Implementation

### What is Disconnected Mode?
As per Hyland documentation, Disconnected Mode is a stateless connection approach that:
- Manages session pools on the server side
- Optimizes for high-volume external API consumption
- Reduces licensing overhead for public-facing applications
- Eliminates client-side session management

### Implementation Details

#### Connection Configuration
```csharp
// Disconnected mode is enabled in authentication properties
authProps.DisconnectedMode = true;
authProps.LicenseType = LicenseType.QueryMetering;
```

#### Session Pool Management
The factory maintains a pool of connections:
- **Minimum Sessions**: 2 (configurable)
- **Maximum Sessions**: 10 (configurable)
- **Pool Behavior**: 
  - Pre-warm minimum connections on startup
  - Create additional sessions up to maximum as needed
  - Queue requests when pool is exhausted

#### Disconnected Mode Limitations
Based on Hyland documentation, the following limitations are handled:

1. **Query Results Paging**: 
   - Avoid `ExecuteQueryResults()` method
   - Use `Execute()` method instead
   - Limit results to single page (100 documents default)

2. **Document Locking**:
   - Avoid `DocumentLock` usage
   - Implement workflow-based processing for documents requiring locks
   - Use optimistic concurrency where possible

### Configuration

#### Application Settings
```json
{
  "Hyland": {
    "Username": "MANAGER",
    "Password": "",
    "DataSource": "OnBase",
    "UseQueryMetering": true,
    "UseDisconnectedMode": true,
    "MaxQueriesPerHour": 1000,
    "MinConnections": 2,
    "MaxConnections": 10,
    "QueryMeteringWarningThreshold": 80
  }
}
```

#### Server Configuration (web.config)
For the Hyland Application Server, configure session pooling:

```xml
<Hyland.Services>
  <poolSettings minSessions="2" maxSessions="10">
    <parameters>
      <parameter name="licenseType" value="queryMeter"/>
    </parameters>
  </poolSettings>
</Hyland.Services>
```

## Query Metering Strategy

### Why Query Metering?
- **Cost Management**: Control licensing costs through query limits
- **Performance Optimization**: Prevent resource exhaustion
- **Fair Usage**: Ensure equitable access across consumers
- **Compliance**: Meet licensing agreement requirements

### Implementation Features

#### Real-time Monitoring
```csharp
var status = await documentService.GetQueryMeteringStatusAsync();
// Returns: current usage, remaining queries, utilization percentage
```

#### Dynamic Configuration
```csharp
documentService.ConfigureQueryMetering(maxQueriesPerHour: 2000, warningThreshold: 85);
```

#### Automatic Enforcement
- Pre-query validation prevents limit violations
- Graceful degradation with meaningful error messages
- Automatic cleanup of expired usage data

## API Endpoints

### Hyland Documents Controller
- `GET /api/hylanddocuments` - Get all documents
- `GET /api/hylanddocuments/{id}` - Get specific document
- `POST /api/hylanddocuments` - Create new document
- `PUT /api/hylanddocuments/{id}` - Update document
- `DELETE /api/hylanddocuments/{id}` - Delete document
- `POST /api/hylanddocuments/search` - Search documents
- `POST /api/hylanddocuments/{id}/archive` - Archive document
- `GET /api/hylanddocuments/{id}/content` - Download content
- `GET /api/hylanddocuments/query-metering/status` - Get metering status
- `POST /api/hylanddocuments/query-metering/configure` - Configure limits

### Error Handling
- **429 Too Many Requests**: Query limit exceeded
- **500 Internal Server Error**: Connection or operation failures
- **404 Not Found**: Document not found
- **400 Bad Request**: Invalid request data

## Best Practices

### Connection Management
1. Use disconnected mode for high-volume scenarios
2. Configure appropriate pool sizes based on load
3. Monitor connection usage and adjust limits
4. Implement connection health checks

### Query Optimization
1. Limit search results to avoid paging issues
2. Use specific search criteria to reduce result sets
3. Implement caching for frequently accessed documents
4. Monitor query patterns and optimize accordingly

### Error Handling
1. Implement retry logic for transient failures
2. Provide meaningful error messages to clients
3. Log detailed error information for troubleshooting
4. Handle query limit exceeded scenarios gracefully

### Security
1. Store credentials securely (environment variables, key vault)
2. Use secure connections (HTTPS/TLS)
3. Implement proper authentication and authorization
4. Audit document access and operations

## Monitoring and Observability

### Logging
- Structured logging with correlation IDs
- Performance metrics (query times, connection usage)
- Error tracking and alerting
- Query metering statistics

### Health Checks
- Connection pool status
- Query metering status
- Hyland service availability
- Application performance metrics

## Deployment Considerations

### Environment Configuration
- Development: In-memory simulation
- Testing: Limited query limits for cost control
- Production: Full Hyland integration with optimized settings

### Scaling
- Horizontal scaling with shared query metering
- Load balancing considerations
- Connection pool distribution
- Session affinity requirements

## Example Usage

```csharp
// Inject the service
public class DocumentController : ControllerBase
{
    private readonly IHylandDocumentService _documentService;
    
    public async Task<IActionResult> GetDocuments()
    {
        try
        {
            var documents = await _documentService.GetAllDocumentsAsync();
            return Ok(documents);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Query limit"))
        {
            return StatusCode(429, "Query limit exceeded. Please try again later.");
        }
    }
}
```

## Future Enhancements

### Planned Features
1. **Caching Layer**: Redis-based document metadata caching
2. **Event Sourcing**: Document operation audit trail
3. **Workflow Integration**: Advanced document lifecycle management
4. **Metrics Dashboard**: Real-time monitoring interface
5. **Auto-scaling**: Dynamic connection pool adjustment

### Performance Optimizations
1. **Connection Warmup**: Predictive connection creation
2. **Query Batching**: Batch multiple operations
3. **Async Streaming**: Large result set streaming
4. **Compression**: Response compression for bandwidth optimization

This architecture provides a robust, scalable, and cost-effective solution for Hyland document management while leveraging the benefits of Disconnected Mode and query metering.