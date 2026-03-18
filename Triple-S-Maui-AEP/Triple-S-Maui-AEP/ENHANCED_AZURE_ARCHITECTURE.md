# Triple-S MAUI AEP - HIPAA/CMS Compliant Azure Architecture

## Enhanced System Architecture with Azure Services

```
┌─────────────────────────────────────────────────────────────────────────────────────────────┐
│                                  MOBILE APPLICATION                                          │
│                               (.NET MAUI - Android/iOS)                                      │
│                                                                                             │
│  ┌────────────────────┐  ┌────────────────────┐  ┌────────────────────┐                  │
│  │   Agent Login      │  │   SOA Wizard       │  │ Enrollment Wizard  │                  │
│  │   - AAD Auth       │  │   - 4 Steps        │  │   - 9 Steps        │                  │
│  │   - MFA Support    │  │   - Signatures     │  │   - Signatures     │                  │
│  │   - Offline Mode   │  │   - Auto-Save      │  │   - Auto-Save      │                  │
│  └────────────────────┘  └────────────────────┘  └────────────────────┘                  │
│                                                                                             │
│  ┌──────────────────────────────────────────────────────────────────────────────────────┐ │
│  │                         LOCAL SERVICES (Enhanced)                                     │ │
│  │  • PdfService - Generate/Fill PDF Templates                                         │ │
│  │  • SignaturePadView - Capture Signatures                                            │ │
│  │  • LocalQueueService - Queue uploads when offline                                   │ │
│  │  • EncryptionService - Encrypt PHI data at rest                                     │ │
│  │  • AuditLogService - Local audit trail                                              │ │
│  │  • TokenCacheService - Secure OAuth token storage                                   │ │
│  └──────────────────────────────────────────────────────────────────────────────────────┘ │
│                                                                                             │
│  ┌──────────────────────────────────────────────────────────────────────────────────────┐ │
│  │                         AZURE SDK INTEGRATION                                         │ │
│  │  • Azure Service Bus Client - Message publishing                                     │ │
│  │  • Azure App Insights - Telemetry & monitoring                                       │ │
│  │  • Azure MSAL - Azure AD authentication                                              │ │
│  └──────────────────────────────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────────────────────────────┘
                                          │
                                          │ HTTPS + OAuth 2.0
                                          │ (HIPAA-compliant TLS 1.2+)
                                          ▼
┌─────────────────────────────────────────────────────────────────────────────────────────────┐
│                                AZURE CLOUD SERVICES                                          │
│                              (HIPAA Business Associate Agreement)                            │
│                                                                                             │
│  ┌──────────────────────────────────────────────────────────────────────────────────────┐ │
│  │                         AZURE ACTIVE DIRECTORY (AAD)                                  │ │
│  │  • Multi-Factor Authentication (MFA)                                                 │ │
│  │  • Conditional Access Policies                                                       │ │
│  │  • Role-Based Access Control (RBAC)                                                  │ │
│  │  • Audit Logs (CMS Compliance)                                                       │ │
│  └──────────────────────────────────────────────────────────────────────────────────────┘ │
│                                          │                                                  │
│                                          ▼                                                  │
│  ┌──────────────────────────────────────────────────────────────────────────────────────┐ │
│  │                          AZURE API MANAGEMENT                                         │ │
│  │                                                                                       │ │
│  │  ┌────────────────────────────────────────────────────────────────────────┐        │ │
│  │  │  HIPAA/CMS Compliance Policies                                          │        │ │
│  │  │  • TLS 1.2+ enforcement                                                 │        │ │
│  │  │  • OAuth 2.0 + JWT validation                                           │        │ │
│  │  │  • Rate limiting per agent                                              │        │ │
│  │  │  • Request/Response encryption                                          │        │ │
│  │  │  • PHI data masking in logs                                             │        │ │
│  │  │  • IP whitelisting                                                      │        │ │
│  │  └────────────────────────────────────────────────────────────────────────┘        │ │
│  │                                                                                       │ │
│  │  API Endpoints:                                                                      │ │
│  │  • POST /api/messages/upload - Publish to Service Bus                               │ │
│  │  • POST /api/auth/login - AAD authentication                                         │ │
│  │  • GET  /api/status/{messageId} - Check upload status                               │ │
│  │  • GET  /api/documents/{id} - Retrieve document                                      │ │
│  └──────────────────────────────────────────────────────────────────────────────────────┘ │
│                                          │                                                  │
│                                          ▼                                                  │
│  ┌──────────────────────────────────────────────────────────────────────────────────────┐ │
│  │                          AZURE SERVICE BUS                                            │ │
│  │                        (HIPAA-Compliant Message Broker)                               │ │
│  │                                                                                       │ │
│  │  Queue: document-upload-queue                                                        │ │
│  │  • Max message size: 256 KB (metadata + PDF reference)                              │ │
│  │  • TTL: 14 days                                                                      │ │
│  │  • Dead Letter Queue enabled                                                         │ │
│  │  • Duplicate detection enabled                                                       │ │
│  │  • Message encryption at rest                                                        │ │
│  │  • Access controlled via Managed Identity                                            │ │
│  │                                                                                       │ │
│  │  Topic: document-events                                                              │ │
│  │  └─► Subscription: dms-processor                                                     │ │
│  │  └─► Subscription: notification-service                                              │ │
│  │  └─► Subscription: audit-logger                                                      │ │
│  └──────────────────────────────────────────────────────────────────────────────────────┘ │
│                                          │                                                  │
│                                          ▼                                                  │
│  ┌──────────────────────────────────────────────────────────────────────────────────────┐ │
│  │                          AZURE KEY VAULT                                              │ │
│  │  • DMS credentials (OnBase username/password)                                        │ │
│  │  • API keys                                                                          │ │
│  │  • Encryption keys for PHI data                                                      │ │
│  │  • TLS/SSL certificates                                                              │ │
│  │  • Access via Managed Identity                                                       │ │
│  │  • Audit logging enabled                                                             │ │
│  └──────────────────────────────────────────────────────────────────────────────────────┘ │
│                                                                                             │
│  ┌──────────────────────────────────────────────────────────────────────────────────────┐ │
│  │                    AZURE BLOB STORAGE (Premium Hot Tier)                              │ │
│  │                         (HIPAA-Compliant Storage)                                     │ │
│  │                                                                                       │ │
│  │  Container: enrollment-pdfs                                                           │ │
│  │  • Encryption at rest (Azure Storage Service Encryption)                             │ │
│  │  • Encryption in transit (HTTPS only)                                                │ │
│  │  • Immutable storage (WORM - CMS compliance)                                         │ │
│  │  • Soft delete enabled (30-day retention)                                            │ │
│  │  • Versioning enabled                                                                │ │
│  │  • Access tier: Hot (frequent access)                                                │ │
│  │  • Lifecycle management: Archive after 7 years                                       │ │
│  │  • Private endpoint (no public access)                                               │ │
│  └──────────────────────────────────────────────────────────────────────────────────────┘ │
│                                                                                             │
│  ┌──────────────────────────────────────────────────────────────────────────────────────┐ │
│  │                      AZURE FUNCTIONS (Serverless Processors)                          │ │
│  │                                                                                       │ │
│  │  Function: DocumentUploadProcessor                                                    │ │
│  │  Trigger: Service Bus Queue (document-upload-queue)                                  │ │
│  │  • Download PDF from Blob Storage                                                    │ │
│  │  • Retrieve DMS credentials from Key Vault                                           │ │
│  │  • Upload to OnBase via VPN                                                          │ │
│  │  • Update status in Cosmos DB                                                        │ │
│  │  • Publish success/failure event to Service Bus Topic                                │ │
│  │  • Retry logic: 3 attempts with exponential backoff                                  │ │
│  │                                                                                       │ │
│  │  Function: NotificationProcessor                                                      │ │
│  │  Trigger: Service Bus Topic Subscription (notification-service)                      │ │
│  │  • Send push notifications to mobile app                                             │ │
│  │  • Update agent dashboard                                                            │ │
│  │                                                                                       │ │
│  │  Function: AuditLogger                                                                │ │
│  │  Trigger: Service Bus Topic Subscription (audit-logger)                              │ │
│  │  • Log all document operations to Log Analytics                                      │ │
│  │  • Store audit trail in immutable storage (CMS requirement)                          │ │
│  └──────────────────────────────────────────────────────────────────────────────────────┘ │
│                                                                                             │
│  ┌──────────────────────────────────────────────────────────────────────────────────────┐ │
│  │                         AZURE COSMOS DB (NoSQL)                                       │ │
│  │                      (HIPAA-Compliant Document Database)                              │ │
│  │                                                                                       │ │
│  │  Container: document-metadata                                                         │ │
│  │  • Partition Key: AgentID                                                            │ │
│  │  • Document schema:                                                                  │ │
│  │    {                                                                                 │ │
│  │      "id": "SOA-2652-01002-8888",                                                    │ │
│  │      "agentId": "AGENT123",                                                          │ │
│  │      "documentType": "SOA",                                                          │ │
│  │      "beneficiaryFirstName": "John",                                                 │ │
│  │      "beneficiaryLastName": "Doe",                                                   │ │
│  │      "medicareNumber": "***-**-6789A",  // Masked                                    │ │
│  │      "blobStorageUrl": "https://...",                                                │ │
│  │      "onBaseDocumentId": "12345",                                                    │ │
│  │      "uploadStatus": "Completed",                                                    │ │
│  │      "createdDate": "2024-01-15T10:30:00Z",                                          │ │
│  │      "uploadedDate": "2024-01-15T10:35:00Z",                                         │ │
│  │      "retryCount": 0,                                                                │ │
│  │      "auditTrail": [...]                                                             │ │
│  │    }                                                                                 │ │
│  │  • Encryption at rest                                                                │ │
│  │  • Point-in-time restore enabled                                                     │ │
│  │  • TTL: 7 years (CMS retention requirement)                                          │ │
│  └──────────────────────────────────────────────────────────────────────────────────────┘ │
│                                                                                             │
│  ┌──────────────────────────────────────────────────────────────────────────────────────┐ │
│  │                    AZURE MONITOR & APPLICATION INSIGHTS                               │ │
│  │                         (HIPAA Audit & Compliance)                                    │ │
│  │                                                                                       │ │
│  │  Log Analytics Workspace: triple-s-aep-logs                                          │ │
│  │  • Application logs (Function Apps, API Management)                                  │ │
│  │  • Diagnostic logs (Service Bus, Key Vault, Storage)                                 │ │
│  │  • Security audit logs (AAD, RBAC changes)                                           │ │
│  │  • Performance metrics                                                               │ │
│  │  • Custom events (document lifecycle tracking)                                       │ │
│  │  • Alerts: Failed uploads, authentication failures, etc.                             │ │
│  │  • Retention: 90 days hot, 2 years archive                                           │ │
│  │  • HIPAA audit reports generated monthly                                             │ │
│  └──────────────────────────────────────────────────────────────────────────────────────┘ │
│                                                                                             │
│  ┌──────────────────────────────────────────────────────────────────────────────────────┐ │
│  │                         AZURE SECURITY CENTER                                         │ │
│  │  • Continuous security assessment                                                    │ │
│  │  • Threat detection                                                                  │ │
│  │  • Vulnerability scanning                                                            │ │
│  │  • Compliance dashboard (HIPAA, CMS)                                                 │ │
│  │  • Security recommendations                                                          │ │
│  └──────────────────────────────────────────────────────────────────────────────────────┘ │
│                                                                                             │
│  ┌──────────────────────────────────────────────────────────────────────────────────────┐ │
│  │                            AZURE POLICY                                               │ │
│  │  • Enforce encryption at rest                                                        │ │
│  │  • Require TLS 1.2+                                                                  │ │
│  │  • Block public blob access                                                          │ │
│  │  • Require diagnostic logging                                                        │ │
│  │  • Enforce tag compliance                                                            │ │
│  └──────────────────────────────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────────────────────────────┘
                                          │
                                          │ VPN Tunnel (Site-to-Site)
                                          │ (AES-256 Encryption)
                                          ▼
┌─────────────────────────────────────────────────────────────────────────────────────────────┐
│                              TRIPLE-S DATACENTER                                             │
│                               (Internal Network)                                             │
│                                                                                             │
│  ┌──────────────────────────────────────────────────────────────────────────────────────┐ │
│  │                             VPN GATEWAY                                               │ │
│  │  • Connection from Azure VPN Gateway                                                 │ │
│  │  • Firewall rules (Azure Function IPs only)                                          │ │
│  │  • IDS/IPS monitoring                                                                │ │
│  └──────────────────────────────────────────────────────────────────────────────────────┘ │
│                                          │                                                  │
│                                          ▼                                                  │
│  ┌──────────────────────────────────────────────────────────────────────────────────────┐ │
│  │                      TRIPLE-S DMS (HYLAND ONBASE)                                     │ │
│  │                       (Final Document Repository)                                     │ │
│  │                                                                                       │ │
│  │  • Document storage with keywords/metadata                                            │ │
│  │  • Version control                                                                   │ │
│  │  • Workflow management                                                               │ │
│  │  • Access controlled by Windows AD                                                   │ │
│  │  • Audit trail for all document access                                               │ │
│  └──────────────────────────────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────────────────────────────┘
```

## Enhanced Data Flow with Service Bus

### 1. Agent Login (AAD Authentication)

```
Mobile App
  │
  ├─► Open Login Screen
  │
  ├─► Click "Sign In"
  │    └─► Redirect to Azure AD (MSAL)
  │         ├─► Enter credentials
  │         ├─► MFA challenge (if enabled)
  │         └─► Receive OAuth 2.0 token
  │
  ├─► Token stored securely in TokenCacheService
  │
  └─► Navigate to Dashboard
```

### 2. Document Creation (Offline-First)

```
Mobile App
  │
  ├─► Agent completes SOA/Enrollment wizard
  │    ├─► Collect beneficiary info
  │    ├─► Capture signatures
  │    └─► Generate PDF
  │
  ├─► Encrypt PHI data locally (EncryptionService)
  │
  ├─► Save to local storage
  │    ├─► PDF file (encrypted)
  │    ├─► Metadata (encrypted)
  │    └─► Queue entry (LocalQueueService)
  │
  └─► Display "Document saved locally"
```

### 3. Asynchronous Upload (When Online)

```
Mobile App (LocalQueueService)
  │
  ├─► Detect network connectivity
  │
  ├─► Process queued uploads
  │
  └─► For each queued document:
       │
       ├─► STEP 1: Upload PDF to Azure Blob Storage
       │    │
       │    ├─► API Management: POST /api/storage/upload
       │    │    └─► Validate OAuth token
       │    │         └─► Check RBAC permissions
       │    │
       │    ├─► Upload encrypted PDF to Blob Storage
       │    │    └─► Container: enrollment-pdfs
       │    │         └─► Blob name: {AgentId}/{DocumentId}.pdf
       │    │
       │    └─► Return Blob SAS URL
       │
       ├─► STEP 2: Publish message to Service Bus
       │    │
       │    ├─► API Management: POST /api/messages/upload
       │    │    └─► Validate OAuth token
       │    │         └─► Check rate limits
       │    │
       │    ├─► Create message:
       │    │    {
       │    │      "messageId": "msg-uuid",
       │    │      "documentId": "SOA-2652-01002-8888",
       │    │      "documentType": "SOA",
       │    │      "agentId": "AGENT123",
       │    │      "blobStorageUrl": "https://...",
       │    │      "metadata": {
       │    │        "beneficiaryFirstName": "John",
       │    │        "beneficiaryLastName": "Doe",
       │    │        "medicareNumber": "***-**-6789A",
       │    │        "soaNumber": "SOA-2652-01002-8888",
       │    │        "createdDate": "2024-01-15T10:30:00Z"
       │    │      },
       │    │      "keywords": [
       │    │        { "KeywordTypeID": 1, "Value": "SOA-2652-01002-8888" },
       │    │        { "KeywordTypeID": 2, "Value": "John" },
       │    │        ...
       │    │      ]
       │    │    }
       │    │
       │    ├─► Publish to Service Bus Queue: document-upload-queue
       │    │
       │    └─► Return message ID
       │
       ├─► STEP 3: Store metadata in Cosmos DB
       │    │
       │    ├─► Create document with status: "Queued"
       │    │
       │    └─► Store message ID for tracking
       │
       ├─► STEP 4: Update local status
       │    │
       │    ├─► Mark as "Uploaded to Cloud"
       │    │
       │    └─► Store message ID
       │
       └─► Display "Upload successful - Processing in background"
```

### 4. Background Processing (Azure Functions)

```
Azure Function: DocumentUploadProcessor
  │
  ├─► Triggered by Service Bus message
  │
  ├─► Parse message
  │
  ├─► Download PDF from Blob Storage
  │    └─► Using SAS URL from message
  │
  ├─► Retrieve DMS credentials from Key Vault
  │    └─► Using Managed Identity
  │
  ├─► Connect to OnBase DMS via VPN
  │    │
  │    ├─► Authenticate with OnBase
  │    │
  │    ├─► Upload document
  │    │    ├─► Document type: Based on message
  │    │    ├─► File data: From Blob Storage
  │    │    └─► Keywords: From message metadata
  │    │
  │    └─► Receive OnBase Document ID
  │
  ├─► Update Cosmos DB
  │    ├─► Status: "Completed"
  │    ├─► OnBaseDocumentId: "12345"
  │    ├─► UploadedDate: Timestamp
  │    └─► Add audit trail entry
  │
  ├─► Publish success event to Service Bus Topic
  │    └─► Topic: document-events
  │         └─► Event: DocumentUploadSuccess
  │
  ├─► Complete Service Bus message
  │
  └─► Log to Application Insights
```

### 5. Notification Flow

```
Azure Function: NotificationProcessor
  │
  ├─► Triggered by Service Bus Topic Subscription
  │    └─► Subscription: notification-service
  │         └─► Filter: DocumentUploadSuccess
  │
  ├─► Parse event
  │
  ├─► Send push notification to mobile app
  │    └─► Agent receives: "Document uploaded successfully"
  │
  ├─► Update agent's dashboard cache
  │
  └─► Log to Application Insights
```

### 6. Error Handling & Retry Logic

```
Azure Function: DocumentUploadProcessor
  │
  ├─► Upload fails (network error, DMS error, etc.)
  │
  ├─► Retry logic:
  │    ├─► Attempt 1: Immediate retry
  │    ├─► Attempt 2: Retry after 5 minutes
  │    └─► Attempt 3: Retry after 15 minutes
  │
  ├─► All retries failed
  │    │
  │    ├─► Update Cosmos DB
  │    │    ├─► Status: "Failed"
  │    │    ├─► ErrorMessage: Details
  │    │    └─► RetryCount: 3
  │    │
  │    ├─► Move message to Dead Letter Queue
  │    │
  │    ├─► Publish failure event to Service Bus Topic
  │    │
  │    ├─► Send alert to operations team
  │    │
  │    └─► Notify agent via push notification
  │         └─► "Upload failed - Contact support"
  │
  └─► Log error to Application Insights
```

## HIPAA/CMS Compliance Features

### Data Protection

```
┌─────────────────────────────────────────────────────────────┐
│                    ENCRYPTION AT REST                        │
│  • Mobile App: AES-256 encryption for local PHI data       │
│  • Azure Blob Storage: Azure Storage Service Encryption     │
│  • Azure Cosmos DB: Transparent Data Encryption (TDE)       │
│  • Azure Key Vault: Hardware Security Module (HSM) backed  │
└─────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                   ENCRYPTION IN TRANSIT                      │
│  • TLS 1.2+ for all communications                          │
│  • Certificate pinning in mobile app                        │
│  • VPN tunnel (AES-256) for DMS communication              │
└─────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    ACCESS CONTROL                            │
│  • Azure AD with MFA for agent authentication               │
│  • Role-Based Access Control (RBAC) at all layers          │
│  • Managed Identity for service-to-service auth             │
│  • Conditional Access Policies                             │
└─────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    AUDIT & LOGGING                           │
│  • All operations logged to Log Analytics                   │
│  • Immutable audit trail storage                            │
│  • 90-day hot retention, 7-year archive                     │
│  • Monthly HIPAA compliance reports                         │
│  • Real-time alerts for security events                     │
└─────────────────────────────────────────────────────────────┘
```

### CMS Requirements Compliance Matrix

| CMS Requirement | Implementation |
|----------------|----------------|
| **Data Retention (7 years)** | Cosmos DB TTL + Blob lifecycle policy |
| **Audit Trail** | Application Insights + Log Analytics |
| **Data Integrity** | Blob versioning + immutable storage |
| **Access Control** | Azure AD + RBAC + Conditional Access |
| **Encryption** | At rest + in transit (TLS 1.2+) |
| **Disaster Recovery** | Geo-redundant storage + backup policies |
| **Business Associate Agreement** | Azure HIPAA BAA signed |
| **Risk Assessment** | Azure Security Center continuous assessment |

## User Experience Improvements

### 1. Offline-First Architecture

```
Agent has network connectivity:
  ✓ Upload immediately via Service Bus
  ✓ Real-time status updates
  ✓ Instant success notification

Agent is offline:
  ✓ Save document locally (encrypted)
  ✓ Queue for upload when online
  ✓ Display "Saved - Will upload when online"
  ✓ Auto-retry when network returns
```

### 2. Asynchronous Processing

```
Before (Synchronous):
  Agent → Submit → Wait 30-60 seconds → Response
  
After (Asynchronous with Service Bus):
  Agent → Submit → Immediate acknowledgment (1-2 seconds)
          └─► Background processing (30-60 seconds)
               └─► Push notification when complete
```

### 3. Real-Time Status Tracking

```
Mobile App Dashboard:
  ┌─────────────────────────────────────────────┐
  │  Recent Documents                           │
  ├─────────────────────────────────────────────┤
  │  SOA-2652-01002-8888                       │
  │  Status: ✓ Completed                        │
  │  OnBase ID: 12345                           │
  │  Uploaded: 2024-01-15 10:35 AM             │
  ├─────────────────────────────────────────────┤
  │  ENR-2652-01003-9999                       │
  │  Status: ⏳ Processing...                   │
  │  Queued: 2024-01-15 10:40 AM               │
  ├─────────────────────────────────────────────┤
  │  SOA-2652-01004-7777                       │
  │  Status: ⚠ Failed (Retry in 5 min)         │
  │  Error: DMS connection timeout              │
  └─────────────────────────────────────────────┘
```

### 4. Push Notifications

```
Notification Types:
  ✓ Upload completed successfully
  ✓ Upload failed - Action required
  ⏳ Upload queued (offline mode)
  🔄 Processing in background
  ⚠ Manual review required
```

## Cost Optimization

### Azure Service Bus Pricing (Standard Tier)

- **Base Cost**: $9.81/month for 12.5M operations
- **Per-agent estimate**: ~100 uploads/day × 30 days = 3,000 messages/month
- **For 100 agents**: 300,000 messages/month = $0.78/month

### Azure Blob Storage (Hot Tier)

- **Storage**: $0.018/GB/month
- **Estimate**: 100 agents × 100 uploads/month × 500KB avg = 5GB
- **Cost**: 5GB × $0.018 = $0.09/month

### Azure Cosmos DB (Provisioned Throughput)

- **400 RU/s** (Request Units): $23.36/month
- **Storage**: $0.25/GB/month
- **Estimate**: 1GB metadata = $0.25/month
- **Total**: $23.61/month

### Azure Functions (Consumption Plan)

- **First 1M executions free**
- **Estimate**: 300,000 executions/month = Free

### Azure Key Vault

- **Standard Tier**: $0.03/10,000 operations
- **Estimate**: ~50,000 operations/month = $0.15/month

### Total Monthly Cost Estimate

| Service | Cost/Month |
|---------|------------|
| Azure Service Bus | $0.78 |
| Azure Blob Storage | $0.09 |
| Azure Cosmos DB | $23.61 |
| Azure Functions | $0.00 |
| Azure Key Vault | $0.15 |
| Azure Monitor/App Insights | $5.00 |
| **Total** | **~$29.63/month** |

*Note: Excludes Azure AD Premium P1 ($6/user/month) and VPN Gateway ($142/month - already existing)*

## Security Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    DEFENSE IN DEPTH                          │
├─────────────────────────────────────────────────────────────┤
│ Layer 1: Mobile App                                         │
│  • Local data encryption (AES-256)                          │
│  • OAuth token storage (secure enclave)                     │
│  • Certificate pinning                                      │
│  • Biometric authentication                                 │
├─────────────────────────────────────────────────────────────┤
│ Layer 2: Network                                            │
│  • TLS 1.2+ mandatory                                       │
│  • Certificate validation                                   │
│  • No self-signed certs in production                       │
├─────────────────────────────────────────────────────────────┤
│ Layer 3: Azure AD                                           │
│  • Multi-Factor Authentication (MFA)                        │
│  • Conditional Access (location, device, risk-based)        │
│  • Privileged Identity Management (PIM)                     │
├─────────────────────────────────────────────────────────────┤
│ Layer 4: API Management                                     │
│  • JWT validation                                           │
│  • Rate limiting (prevent DDoS)                             │
│  • IP whitelisting                                          │
│  • Request throttling                                       │
├─────────────────────────────────────────────────────────────┤
│ Layer 5: Azure Services                                     │
│  • Managed Identity (no passwords)                          │
│  • Private endpoints (no public access)                     │
│  • Network Security Groups (NSG)                            │
│  • Azure Firewall                                           │
├─────────────────────────────────────────────────────────────┤
│ Layer 6: Data Layer                                         │
│  • Encryption at rest (all services)                        │
│  • Customer-managed keys (optional)                         │
│  • Immutable storage (WORM)                                 │
│  • Soft delete + versioning                                 │
├─────────────────────────────────────────────────────────────┤
│ Layer 7: VPN Tunnel                                         │
│  • Site-to-Site VPN (AES-256)                               │
│  • BGP routing                                              │
│  • Redundant tunnels                                        │
├─────────────────────────────────────────────────────────────┤
│ Layer 8: DMS (OnBase)                                       │
│  • Windows AD authentication                                │
│  • Document-level permissions                               │
│  • Audit logs                                               │
└─────────────────────────────────────────────────────────────┘
```

## Disaster Recovery & Business Continuity

### Backup Strategy

```
┌─────────────────────────────────────────────────────────────┐
│                    BACKUP & RECOVERY                         │
├─────────────────────────────────────────────────────────────┤
│ Azure Blob Storage                                          │
│  • Geo-redundant storage (GRS)                              │
│  • Async replication to secondary region                    │
│  • 99.99999999999999% (16 9's) durability                  │
│  • Soft delete: 30 days                                     │
├─────────────────────────────────────────────────────────────┤
│ Azure Cosmos DB                                             │
│  • Multi-region writes (optional)                           │
│  • Automatic backup: Every 4 hours                          │
│  • Point-in-time restore: 30 days                           │
│  • Manual backups: On-demand                                │
├─────────────────────────────────────────────────────────────┤
│ Azure Key Vault                                             │
│  • Soft delete: 90 days                                     │
│  • Purge protection enabled                                 │
│  • Geo-replication built-in                                 │
├─────────────────────────────────────────────────────────────┤
│ Azure Service Bus                                           │
│  • Geo-disaster recovery (premium tier)                     │
│  • Message replication to secondary namespace               │
│  • Automatic failover                                       │
└─────────────────────────────────────────────────────────────┘
```

### Recovery Time Objectives (RTO) & Recovery Point Objectives (RPO)

| Component | RTO | RPO |
|-----------|-----|-----|
| Azure Blob Storage | < 1 hour | < 15 minutes |
| Azure Cosmos DB | < 1 hour | < 4 hours |
| Azure Functions | < 5 minutes | 0 (stateless) |
| Azure Service Bus | < 15 minutes | < 1 minute |
| OnBase DMS | < 4 hours | < 1 hour |

## Migration Path from Current Architecture

### Phase 1: Foundation (Week 1-2)

1. Set up Azure AD tenant
2. Configure HIPAA Business Associate Agreement
3. Deploy Azure Key Vault
4. Migrate DMS credentials to Key Vault
5. Set up Azure Policy for compliance
6. Deploy Azure Monitor & App Insights

### Phase 2: Service Bus Integration (Week 3-4)

1. Deploy Azure Service Bus (Standard tier)
2. Create document-upload-queue
3. Create document-events topic with subscriptions
4. Deploy Azure Functions:
   - DocumentUploadProcessor
   - NotificationProcessor
   - AuditLogger
5. Update API Management endpoints

### Phase 3: Storage Layer (Week 5-6)

1. Deploy Azure Blob Storage (Premium Hot tier)
2. Configure encryption and lifecycle policies
3. Deploy Azure Cosmos DB
4. Create document-metadata container
5. Set up geo-replication

### Phase 4: Mobile App Updates (Week 7-8)

1. Integrate Azure MSAL for AAD authentication
2. Implement Service Bus client SDK
3. Add LocalQueueService for offline support
4. Implement EncryptionService for local PHI data
5. Add push notification support
6. Update dashboard with real-time status

### Phase 5: Testing & Validation (Week 9-10)

1. End-to-end testing
2. Security testing (penetration testing)
3. HIPAA compliance audit
4. Performance testing (load/stress)
5. Disaster recovery testing
6. User acceptance testing (UAT)

### Phase 6: Production Rollout (Week 11-12)

1. Gradual rollout (10% → 50% → 100% of agents)
2. Monitor metrics and logs
3. Address issues
4. Complete rollout
5. Decommission old architecture

## Summary

This enhanced architecture provides:

✅ **HIPAA Compliance**: Encryption, access control, audit trails, BAA  
✅ **CMS Compliance**: 7-year retention, data integrity, disaster recovery  
✅ **Improved UX**: Offline-first, async processing, real-time notifications  
✅ **Scalability**: Service Bus handles message routing, auto-scaling functions  
✅ **Reliability**: Retry logic, dead letter queues, geo-redundancy  
✅ **Security**: Defense-in-depth, Azure AD, Managed Identity, private endpoints  
✅ **Observability**: Application Insights, Log Analytics, custom dashboards  
✅ **Cost-Effective**: Consumption-based pricing, ~$30/month for Azure services  

The architecture is production-ready, compliant, and provides a modern cloud-native experience for field agents while maintaining the security and control required for healthcare applications.
