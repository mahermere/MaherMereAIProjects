# Triple-S MAUI AEP Application Architecture

## System Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                            MOBILE APPLICATION                                │
│                          (.NET MAUI - Android/iOS)                          │
│                                                                             │
│  ┌────────────────────┐  ┌────────────────────┐  ┌────────────────────┐  │
│  │   Agent Login      │  │   SOA Wizard       │  │ Enrollment Wizard  │  │
│  │   - Authentication │  │   - 4 Steps        │  │   - 9 Steps        │  │
│  │   - Session Mgmt   │  │   - Signatures     │  │   - Signatures     │  │
│  └────────────────────┘  └────────────────────┘  └────────────────────┘  │
│                                                                             │
│  ┌────────────────────────────────────────────────────────────────────┐   │
│  │                      LOCAL SERVICES                                 │   │
│  │  • PdfService - Generate/Fill PDF Templates                        │   │
│  │  • SignaturePadView - Capture Signatures                          │   │
│  │  • SOAService - Track SOA Records (CSV)                           │   │
│  │  • EnrollmentService - Track Enrollments (CSV)                    │   │
│  │  • AgentSessionService - Session Management                       │   │
│  │  • LanguageService - English/Spanish Toggle                       │   │
│  └────────────────────────────────────────────────────────────────────┘   │
│                                                                             │
│  ┌────────────────────────────────────────────────────────────────────┐   │
│  │                      DMS SERVICE                                    │   │
│  │  • DMSService - Upload to Hyland OnBase                           │   │
│  │  • OnBaseAuthenticationService - Authentication                    │   │
│  └────────────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────────────┘
                                     │
                                     │ HTTPS
                                     │ (JSON/REST)
                                     ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                         AZURE API MANAGEMENT                                 │
│                                                                             │
│  ┌────────────────────────────────────────────────────────────────────┐   │
│  │  API Gateway                                                        │   │
│  │  • Rate Limiting                                                   │   │
│  │  • Authentication/Authorization                                    │   │
│  │  • Request/Response Transformation                                │   │
│  │  • Logging & Monitoring                                           │   │
│  └────────────────────────────────────────────────────────────────────┘   │
│                                                                             │
│  ┌────────────────────────────────────────────────────────────────────┐   │
│  │  Endpoints:                                                         │   │
│  │  • /api/dms/upload - Document Upload                              │   │
│  │  • /api/auth/login - Authentication                               │   │
│  │  • /api/documents/* - Document Operations                         │   │
│  └────────────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────────────┘
                                     │
                                     │ VPN Tunnel
                                     │ (Secure Connection)
                                     ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                         TRIPLE-S DATACENTER                                  │
│                          (Internal Network)                                  │
│                                                                             │
│  ┌────────────────────────────────────────────────────────────────────┐   │
│  │                    VPN GATEWAY                                      │   │
│  │  • Connection Termination                                          │   │
│  │  • Security & Firewall                                            │   │
│  └────────────────────────────────────────────────────────────────────┘   │
│                                     │                                       │
│                                     ▼                                       │
│  ┌────────────────────────────────────────────────────────────────────┐   │
│  │              TRIPLE-S DMS (HYLAND ONBASE)                          │   │
│  │                                                                     │   │
│  │  ┌──────────────────────────────────────────────────────────┐    │   │
│  │  │  Document Management                                       │    │   │
│  │  │  • Document Storage                                       │    │   │
│  │  │  • Indexing & Keywords                                    │    │   │
│  │  │  • Version Control                                        │    │   │
│  │  │  • Workflow Management                                    │    │   │
│  │  └──────────────────────────────────────────────────────────┘    │   │
│  │                                                                     │   │
│  │  ┌──────────────────────────────────────────────────────────┐    │   │
│  │  │  Document Types                                            │    │   │
│  │  │  • SOA Documents (Type: 123)                              │    │   │
│  │  │  • Enrollment Forms (Type: 456)                           │    │   │
│  │  └──────────────────────────────────────────────────────────┘    │   │
│  │                                                                     │   │
│  │  ┌──────────────────────────────────────────────────────────┐    │   │
│  │  │  Keywords (Metadata)                                       │    │   │
│  │  │  • DocumentID (Keyword: 1)                                │    │   │
│  │  │  • FirstName (Keyword: 2)                                 │    │   │
│  │  │  • LastName (Keyword: 3)                                  │    │   │
│  │  │  • SOANumber (Keyword: 4)                                 │    │   │
│  │  │  • EnrollmentNumber (Keyword: 5)                          │    │   │
│  │  │  • CreatedDate (Keyword: 6)                               │    │   │
│  │  │  • AgentID (Keyword: 7)                                   │    │   │
│  │  └──────────────────────────────────────────────────────────┘    │   │
│  └────────────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Data Flow: Document Upload Process

### 1. Agent Creates SOA/Enrollment
```
Mobile App
  │
  ├─► Agent Login (AgentSessionService)
  │
  ├─► Complete Wizard (SOA/Enrollment)
  │    ├─► Collect Beneficiary Info
  │    ├─► Capture Signatures
  │    └─► Generate PDF (PdfService)
  │
  └─► Save Locally (CSV + PDF)
```

### 2. Document Upload to DMS
```
Mobile App (DMSService)
  │
  ├─► Prepare Upload Request
  │    ├─► Base64 Encode PDF
  │    ├─► Prepare Metadata/Keywords
  │    └─► Create DMSUploadRequest
  │
  ├─► Send HTTPS POST Request
  │    └─► URL: {BaseUrl}/api/dms/upload
  │
  └─► Mobile App waits for response
       │
       ▼
Azure API Management
  │
  ├─► Validate Request
  ├─► Apply Policies
  ├─► Route through VPN
  │
  └─► Forward to DMS
       │
       ▼
VPN Gateway (Triple-S Datacenter)
  │
  ├─► Decrypt VPN Tunnel
  ├─► Validate Source
  │
  └─► Forward to Internal Network
       │
       ▼
Hyland OnBase (Triple-S DMS)
  │
  ├─► Authenticate Request
  ├─► Decode Base64 PDF
  ├─► Store Document
  ├─► Index with Keywords
  │
  └─► Return Document ID
       │
       ▼
Response travels back:
DMS → VPN Gateway → API Management → Mobile App
  │
  └─► Update Local Status (IsUploaded = true)
```

## Security Layers

```
┌──────────────────────────────────────────────────────────────┐
│ Layer 1: Mobile App Security                                 │
│  • Agent Authentication                                      │
│  • Session Management                                        │
│  • Local Data Encryption (if implemented)                   │
└──────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌──────────────────────────────────────────────────────────────┐
│ Layer 2: Transport Security (TLS/HTTPS)                      │
│  • All communication over HTTPS                              │
│  • Certificate Validation                                    │
└──────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌──────────────────────────────────────────────────────────────┐
│ Layer 3: Azure API Management                                │
│  • API Key/OAuth Validation                                  │
│  • Rate Limiting                                             │
│  • IP Whitelisting                                          │
└──────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌──────────────────────────────────────────────────────────────┐
│ Layer 4: VPN Tunnel                                          │
│  • Encrypted Channel                                         │
│  • Network Isolation                                         │
└──────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌──────────────────────────────────────────────────────────────┐
│ Layer 5: DMS Authentication                                  │
│  • OnBase User Credentials                                   │
│  • Document-Level Permissions                                │
└──────────────────────────────────────────────────────────────┘
```

## Technology Stack

### Mobile Application (.NET MAUI)
- **Framework**: .NET 9.0 / C# 13.0
- **Platforms**: Android, iOS, macOS, Windows
- **UI Components**:
  - `ContentPage` - Base pages
  - `SignaturePadView` - Custom signature capture
  - `Picker`, `Entry`, `DatePicker` - Form inputs
- **PDF Generation**: Syncfusion.Pdf
- **HTTP Client**: System.Net.Http.HttpClient
- **Local Storage**: CSV files in AppDataDirectory

### Azure API Management
- **API Gateway**: Azure APIM
- **Protocols**: HTTPS/REST
- **Authentication**: API Keys / OAuth 2.0
- **Policies**: 
  - Rate limiting
  - CORS
  - Request/Response transformation

### VPN Connection
- **Type**: Site-to-Site VPN
- **Protocol**: IPSec/IKEv2
- **Encryption**: AES-256

### Triple-S DMS (Hyland OnBase)
- **Platform**: Hyland OnBase
- **API**: Unity API / REST API
- **Document Storage**: SQL Server / File System
- **Authentication**: Windows Auth / OnBase Auth

## Component Interaction Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                    Mobile Application                        │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌──────────────┐         ┌──────────────┐                 │
│  │              │         │              │                 │
│  │  SOA Wizard  │────────▶│  PdfService  │                 │
│  │              │         │              │                 │
│  └──────────────┘         └──────┬───────┘                 │
│         │                        │                          │
│         │                        │ PDF Generated            │
│         │                        ▼                          │
│         │                 ┌──────────────┐                 │
│         │                 │              │                 │
│         └────────────────▶│  DMSService  │                 │
│                           │              │                 │
│                           └──────┬───────┘                 │
│                                  │                          │
└──────────────────────────────────┼──────────────────────────┘
                                   │
                                   │ HTTPS POST
                                   │ /api/dms/upload
                                   │
                                   ▼
                          ┌────────────────┐
                          │                │
                          │  API Gateway   │
                          │  (Azure APIM)  │
                          │                │
                          └────────┬───────┘
                                   │
                                   │ VPN Tunnel
                                   │
                                   ▼
                          ┌────────────────┐
                          │                │
                          │  Triple-S DMS  │
                          │  (OnBase)      │
                          │                │
                          └────────────────┘
```

## Key Configuration Points

### Mobile App Configuration (AppSettings.cs)
```csharp
public static class AppSettings
{
    public static string DMSBaseUrl = "https://api.triple-s.com";
    public static string DMSApiKey = "your-api-key";
    public static int SOADocumentTypeID = 123;
    public static int EnrollmentDocumentTypeID = 456;
}
```

### API Endpoint Structure
```
Base URL: https://api.triple-s.com

Endpoints:
  POST   /api/dms/upload          - Upload document
  GET    /api/dms/status/{id}     - Check upload status
  GET    /api/dms/document/{id}   - Retrieve document
  POST   /api/auth/login          - Agent authentication
```

### DMS Document Upload Payload
```json
{
  "DocumentTypeID": 123,
  "FileName": "SOA-2652-01002-8888.pdf",
  "FileData": "base64EncodedPdfContent...",
  "Keywords": [
    { "KeywordTypeID": 1, "Value": "SOA-2652-01002-8888" },
    { "KeywordTypeID": 2, "Value": "John" },
    { "KeywordTypeID": 3, "Value": "Doe" },
    { "KeywordTypeID": 4, "Value": "SOA-2652-01002-8888" },
    { "KeywordTypeID": 6, "Value": "2024-01-15" },
    { "KeywordTypeID": 7, "Value": "AGENT123" }
  ]
}
```

## Network Flow Timeline

```
Time    Mobile App          API Gateway         VPN Gateway         DMS (OnBase)
────────────────────────────────────────────────────────────────────────────────
T0      Generate PDF
T1      Encode Base64
T2      Prepare Request
T3      HTTPS POST ───────▶
T4                         Validate Request
T5                         Apply Policies
T6                         Route to VPN ─────▶
T7                                             Decrypt
T8                                             Validate
T9                                             Forward ───────────▶
T10                                                                Authenticate
T11                                                                Store Document
T12                                                                Index Keywords
T13                                                                Return Doc ID
T14                                            Return ◀───────────
T15                         Return ◀───────────
T16     Receive Response◀──
T17     Update CSV Status
        (IsUploaded=true)
```

## Deployment Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    DEVELOPMENT                               │
│  • Local Testing                                            │
│  • Visual Studio 2022                                       │
│  • Android Emulator / Physical Device                       │
└─────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    BUILD & PACKAGE                           │
│  • dotnet build -f net9.0-android                           │
│  • Generate APK/AAB                                         │
│  • Code Signing                                             │
└─────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    DISTRIBUTION                              │
│  • Internal Distribution (MDM)                              │
│  • Google Play Store (Enterprise)                           │
│  • iOS App Store (Enterprise)                               │
└─────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    PRODUCTION DEVICES                        │
│  • Field Agents' Android/iOS Devices                        │
│  • Corporate-Managed Devices                                │
└─────────────────────────────────────────────────────────────┘
```

## Offline/Online Mode Behavior

```
┌────────────────────────────────────────────────────────────┐
│                    OFFLINE MODE                             │
│  ✓ Create SOA/Enrollment                                   │
│  ✓ Capture Signatures                                      │
│  ✓ Generate PDF                                            │
│  ✓ Save to Local CSV                                       │
│  ✗ Upload to DMS (queued for later)                        │
└────────────────────────────────────────────────────────────┘
                         │
                         │ Network Available
                         ▼
┌────────────────────────────────────────────────────────────┐
│                    ONLINE MODE                              │
│  ✓ Upload Queued Documents                                 │
│  ✓ Update CSV Status (IsUploaded=true)                     │
│  ✓ Sync with DMS                                           │
└────────────────────────────────────────────────────────────┘
```

## Error Handling Flow

```
Upload Attempt
     │
     ├─► Network Error
     │    └─► Retry Logic (3 attempts)
     │         └─► Still Fails → Mark for Manual Review
     │
     ├─► API Error (4xx/5xx)
     │    └─► Log Error
     │         └─► Display User-Friendly Message
     │
     ├─► DMS Error
     │    └─► Parse Error Response
     │         └─► Determine if Retryable
     │              ├─► Yes → Queue for Retry
     │              └─► No → Mark as Failed
     │
     └─► Success
          └─► Update CSV (IsUploaded=true)
               └─► Show Success Message
```

## Monitoring & Logging

```
┌─────────────────────────────────────────────────────────────┐
│                  Mobile App Logging                          │
│  • System.Diagnostics.Debug.WriteLine()                     │
│  • Local Log Files (if enabled)                             │
│  • Crash Reporting (if integrated)                          │
└─────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│              Azure API Management Logging                    │
│  • Request/Response Logs                                    │
│  • Performance Metrics                                       │
│  • Error Tracking                                           │
│  • Azure Monitor Integration                                │
└─────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                  DMS Audit Logs                              │
│  • Document Access Logs                                     │
│  • Upload History                                           │
│  • User Activity                                            │
└─────────────────────────────────────────────────────────────┘
```

## Summary

This architecture provides:

✅ **Secure Communication**: Multi-layered security from mobile app to DMS  
✅ **Scalability**: Azure API Management can handle multiple agents  
✅ **Offline Support**: Documents created offline, uploaded when online  
✅ **Network Isolation**: VPN ensures DMS remains on internal network  
✅ **Audit Trail**: Comprehensive logging at every layer  
✅ **Flexibility**: Easy to add new document types or modify workflows  

The architecture leverages Azure's cloud capabilities while maintaining the security and control of an on-premises DMS system.
