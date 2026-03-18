# DMS Integration Architecture Diagram

## System Overview
```
┌─────────────────────────────────────────────────────────────────┐
│                    TRIPLE-S AGENT PORTAL                         │
│                     (.NET MAUI Windows App)                      │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌────────────────────────────────────────────────────────┐    │
│  │                    DASHBOARD                            │    │
│  │                                                          │    │
│  │  ┌──────────────────────────────────────────────┐      │    │
│  │  │  SOA List                                     │      │    │
│  │  │  ┌────────────────────────────────────┐      │      │    │
│  │  │  │ SOA-123 - John Smith    [⬆] / [✓]│      │      │    │
│  │  │  │ SOA-456 - Maria Garcia  [⬆] / [✓]│      │      │    │
│  │  │  └────────────────────────────────────┘      │      │    │
│  │  └──────────────────────────────────────────────┘      │    │
│  │                                                          │    │
│  │  ┌──────────────────────────────────────────────┐      │    │
│  │  │  Enrollment List                              │      │    │
│  │  │  ┌────────────────────────────────────┐      │      │    │
│  │  │  │ ENR-789 - Jane Doe      [⬆] / [✓]│      │      │    │
│  │  │  │ ENR-101 - Carlos Lopez  [⬆] / [✓]│      │      │    │
│  │  │  └────────────────────────────────────┘      │      │    │
│  │  └──────────────────────────────────────────────┘      │    │
│  └────────────────────────────────────────────────────────┘    │
│                            │                                     │
│                            │ User clicks Upload (⬆)             │
│                            ▼                                     │
│  ┌────────────────────────────────────────────────────────┐    │
│  │               DMS SERVICE                               │    │
│  │           (Services/DMSService.cs)                      │    │
│  │                                                          │    │
│  │  1. Read PDF file from disk                            │    │
│  │  2. Convert to Base64 string                           │    │
│  │  3. Build JSON request with metadata                   │    │
│  │  4. HTTP POST to DMS endpoint                          │    │
│  │  5. Parse response                                      │    │
│  │  6. Update UI and CSV                                   │    │
│  └────────────────────────────────────────────────────────┘    │
│                            │                                     │
└────────────────────────────┼─────────────────────────────────────┘
                             │
                             │ HTTPS POST
                             │ Content-Type: application/json
                             │ X-API-Key: [key]
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    TRIPLE-S DMS API                              │
│            https://localhost:44304/api/document/upload           │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Receives JSON Request:                                         │
│  {                                                               │
│    "documentType": "Enrollment",                                │
│    "fileName": "Enrollment_20250115.pdf",                       │
│    "fileContent": "JVBERi0xLjQK...",  ← Base64 PDF             │
│    "metadata": {                                                │
│      "enrollmentNumber": "ENR-123",                             │
│      "beneficiaryFirstName": "John",                            │
│      "beneficiaryLastName": "Smith",                            │
│      ...                                                         │
│    }                                                             │
│  }                                                               │
│                                                                  │
│  Returns JSON Response:                                         │
│  {                                                               │
│    "success": true,                                             │
│    "documentId": "DOC-2025-01234",                              │
│    "message": "Document uploaded successfully",                 │
│    "timestamp": "2025-01-15T12:34:56Z"                          │
│  }                                                               │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

## Upload Flow Sequence
```
User Action              →  Dashboard           →  DMS Service        →  DMS API
───────────────────────────────────────────────────────────────────────────────
                                                                              
1. Click Upload [⬆]    →                                                     
                                                                              
2.                      →  OnUploadClicked()                                 
                                                                              
3.                      →  Get record from CSV                               
                                                                              
4.                      →  Validate file exists                              
                                                                              
5.                      →  Call DMSService                                   
                           .UploadAsync()                                    
                                                                              
6.                                          →  Read PDF file                 
                                                                              
7.                                          →  Convert to Base64             
                                                                              
8.                                          →  Build JSON request            
                                                                              
9.                                          →  HTTP POST  ───────────→       
                                                                              
10.                                                             Process       
                                                                request       
                                                                              
11.                                                             Store         
                                                                document      
                                                                              
12.                                         ←  JSON Response  ←──────────    
                                                                              
13.                     ←  Return response                                    
                                                                              
14. Update UI (✓)      ←                                                     
                                                                              
15. Update CSV         ←                                                     
                                                                              
16. Show success msg   ←                                                     
```

## Data Flow
```
┌──────────────────┐
│   PDF File       │
│  (Local Disk)    │
└────────┬─────────┘
         │
         │ Read file
         ▼
┌──────────────────┐
│   Byte Array     │
│  (Memory)        │
└────────┬─────────┘
         │
         │ Convert
         ▼
┌──────────────────┐
│  Base64 String   │
│  "JVBERi0x..."   │
└────────┬─────────┘
         │
         │ Add to JSON
         ▼
┌──────────────────┐    ┌──────────────────┐
│  JSON Request    │    │   Metadata       │
│  {               │◄───┤   - Name         │
│    "fileContent" │    │   - Medicare #   │
│    "metadata"    │    │   - NPN          │
│  }               │    │   - Date         │
└────────┬─────────┘    └──────────────────┘
         │
         │ HTTP POST
         ▼
┌──────────────────┐
│   DMS Server     │
│   (Triple-S)     │
└────────┬─────────┘
         │
         │ Process & Store
         ▼
┌──────────────────┐
│  JSON Response   │
│  {               │
│    "success"     │
│    "documentId"  │
│  }               │
└──────────────────┘
```

## Component Dependencies
```
┌─────────────────────────────────────────────────┐
│             Agent Portal UI Layer                │
│  ┌───────────────────┐  ┌────────────────────┐ │
│  │ DashboardWindow   │  │ EnrollmentWizard   │ │
│  │   .xaml.cs        │  │   Page.xaml.cs     │ │
│  └─────────┬─────────┘  └─────────┬──────────┘ │
└────────────┼─────────────────────┼──────────────┘
             │                      │
             │ Uses                 │ Uses
             ▼                      ▼
┌─────────────────────────────────────────────────┐
│              Service Layer                       │
│  ┌──────────────────────────────────────────┐  │
│  │         DMSService.cs                     │  │
│  │  - UploadDocumentAsync()                 │  │
│  │  - UploadEnrollmentAsync()               │  │
│  │  - UploadSOAAsync()                      │  │
│  └────────┬─────────────────────────────────┘  │
└───────────┼──────────────────────────────────────┘
            │
            │ Uses
            ▼
┌─────────────────────────────────────────────────┐
│              Model Layer                         │
│  ┌──────────────────┐  ┌────────────────────┐  │
│  │ DMSUploadRequest │  │ DMSUploadResponse  │  │
│  │   .cs            │  │   .cs              │  │
│  └──────────────────┘  └────────────────────┘  │
│  ┌──────────────────┐                          │
│  │ DMSMetadata      │                          │
│  │   .cs            │                          │
│  └──────────────────┘                          │
└─────────────────────────────────────────────────┘
            │
            │ Uses
            ▼
┌─────────────────────────────────────────────────┐
│           Configuration Layer                    │
│  ┌──────────────────────────────────────────┐  │
│  │         AppSettings.cs                    │  │
│  │  - DMSEndpoint                           │  │
│  │  - DMSApiKey                             │  │
│  │  - DMSUploadTimeoutMinutes               │  │
│  └──────────────────────────────────────────┘  │
└─────────────────────────────────────────────────┘
```

## Error Handling Flow
```
┌────────────────┐
│  Upload Click  │
└───────┬────────┘
        │
        ▼
┌────────────────┐     No      ┌─────────────────┐
│ File Exists?   ├─────────────►│ Show Error:     │
└───────┬────────┘              │ File Not Found  │
        │ Yes                   └─────────────────┘
        ▼
┌────────────────┐     No      ┌─────────────────┐
│ Read Success?  ├─────────────►│ Show Error:     │
└───────┬────────┘              │ Read Failed     │
        │ Yes                   └─────────────────┘
        ▼
┌────────────────┐     No      ┌─────────────────┐
│ Convert Base64 ├─────────────►│ Show Error:     │
│   Success?     │              │ Encoding Failed │
└───────┬────────┘              └─────────────────┘
        │ Yes
        ▼
┌────────────────┐     No      ┌─────────────────┐
│ HTTP Success?  ├─────────────►│ Show Error:     │
└───────┬────────┘              │ Network Error   │
        │ Yes                   └─────────────────┘
        ▼
┌────────────────┐     No      ┌─────────────────┐
│ Response OK?   ├─────────────►│ Show Error:     │
└───────┬────────┘              │ DMS Error       │
        │ Yes                   └─────────────────┘
        ▼
┌────────────────┐
│  Update UI ✓   │
└───────┬────────┘
        │
        ▼
┌────────────────┐
│  Update CSV    │
└───────┬────────┘
        │
        ▼
┌────────────────┐
│ Show Success   │
│   Message      │
└────────────────┘
```

## Configuration States
```
Development                  QA                       Production
─────────────────────────────────────────────────────────────────
                                                                 
Endpoint:                   Endpoint:                Endpoint:   
localhost:44304             qa-dms.tripless.com      dms.tripless.com
                                                                 
API Key:                    API Key:                 API Key:    
None                        qa-api-key-12345         prod-key-67890
                                                                 
Timeout:                    Timeout:                 Timeout:    
5 minutes                   10 minutes               10 minutes  
                                                                 
Max File Size:              Max File Size:           Max File Size:
50 MB                       100 MB                   100 MB      
```

## Legend
```
[⬆]  = Upload button (not uploaded)
[✓]  = Checkmark (uploaded successfully)
─►   = Data flow direction
◄─   = Response flow direction
```

---

This diagram illustrates the complete DMS integration architecture, showing how components interact and how data flows through the system.
