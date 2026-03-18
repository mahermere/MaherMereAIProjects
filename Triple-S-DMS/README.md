# Hyland Document Management Service

A C# .NET RESTful Web API microservice for managing documents in a Hyland-compatible format.

## Features

- RESTful API endpoints for document CRUD operations
- Document metadata management with flexible key-value pairs
- Document search and filtering capabilities
- Document archiving and status management
- File content download functionality
- In-memory storage (suitable for development/demo)
- Dependency injection configured
- OpenAPI/Swagger documentation (in development mode)

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/documents` | Get all active documents |
| GET | `/api/documents/{id}` | Get a specific document by ID |
| POST | `/api/documents` | Create a new document |
| PUT | `/api/documents/{id}` | Update document metadata |
| DELETE | `/api/documents/{id}` | Delete a document (soft delete) |
| POST | `/api/documents/search` | Search documents by criteria |
| POST | `/api/documents/{id}/archive` | Archive a document |
| GET | `/api/documents/{id}/content` | Download document content |

## Document Model

Each document contains:
- **Basic Info**: ID, AutoName (read-only), FileName, FileType, ContentType
- **File Details**: FileSizeBytes, FilePath
- **Metadata**: Flexible key-value pairs for custom properties
- **Audit Info**: UploadedAt, ModifiedAt, UploadedBy
- **Status**: Active, Archived, Deleted, InReview, Draft

## Running the Application

### Prerequisites
- .NET 9.0 SDK

### Build and Run
```bash
# Build the project
dotnet build Triple-S-DMS.csproj

# Run the application
dotnet run --project Triple-S-DMS.csproj
```

The application will start and listen on:
- HTTP: http://localhost:5214

### Using VS Code Tasks
- **Build:** `Ctrl+Shift+P` → "Tasks: Run Task" → "build"
- **Run:** `Ctrl+Shift+P` → "Tasks: Run Task" → "run"

## Testing the API

### Sample Requests

**Get all documents:**
```http
GET http://localhost:5214/api/documents
```

**Create a new document:**
```http
POST http://localhost:5214/api/documents
Content-Type: application/json

{
    "fileName": "contract_2024.pdf",
    "fileType": "PDF",
    "fileSizeBytes": 1048576,
    "contentType": "application/pdf",
    "description": "Annual service contract",
    "uploadedBy": "john.doe",
    "metadata": {
        "Department": "Legal",
        "Category": "Contract",
        "Year": "2024"
    }
}
```

**Search documents:**
```http
POST http://localhost:5214/api/documents/search
Content-Type: application/json

{
    "fileType": "PDF",
    "uploadedBy": "john.doe",
    "metadata": {
        "Department": "Legal"
    }
}
```

**Update document metadata:**
```http
PUT http://localhost:5214/api/documents/1
Content-Type: application/json

{
    "metadata": {
        "Status": "Reviewed",
        "ApprovedBy": "jane.smith"
    }
}
```

**Archive a document:**
```http
POST http://localhost:5214/api/documents/1/archive
```

**Download document content:**
```http
GET http://localhost:5214/api/documents/1/content
```

## Project Structure

```
├── Controllers/
│   └── DocumentsController.cs  # REST API endpoints for documents
├── Models/
│   └── Document.cs             # Document models and DTOs
├── Services/
│   └── DocumentService.cs      # Business logic and data access
├── Properties/
│   └── launchSettings.json     # Development settings
├── Program.cs                  # Application entry point
└── WebApiService.csproj        # Project configuration
```

## Technologies Used

- ASP.NET Core 9.0
- C# 12
- OpenAPI/Swagger
- Dependency Injection

## Hyland Integration Notes

This microservice provides a foundation for Hyland document management integration:
- **Document Metadata**: Flexible metadata structure supports Hyland's custom property requirements
- **File Management**: Supports various file types common in document management
- **Status Workflow**: Document status enum supports typical document lifecycle
- **Search Capabilities**: Advanced search supports metadata-based queries
- **Audit Trail**: Tracks document creation, modification, and user activities

## Development

The application uses in-memory storage for development. In a production environment, you would typically:
- Replace the `DocumentService` with a database-backed implementation using Entity Framework Core
- Integrate with actual file storage (Azure Blob Storage, AWS S3, or local file system)
- Add authentication and authorization
- Implement actual Hyland OnBase integration
- Add file upload/download functionality with proper streaming