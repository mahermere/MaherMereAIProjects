<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

## Project Overview

This is a C# .NET 9.0 RESTful Web API microservice for Hyland document management. The project follows standard ASP.NET Core patterns with:

- **Controllers**: REST API endpoints in `/Controllers/DocumentsController.cs`
- **Models**: Document models and DTOs in `/Models/Document.cs`
- **Services**: Business logic and data access in `/Services/DocumentService.cs`
- **Program.cs**: Application configuration and dependency injection setup

## Key Features

- RESTful CRUD operations for document management
- Document metadata management with flexible key-value pairs
- Document search and filtering capabilities
- Document status workflow (Active, Archived, Deleted, InReview, Draft)
- File content download functionality
- In-memory data storage (suitable for development/demo)
- OpenAPI/Swagger integration for API documentation
- Dependency injection configured for services
- Async/await patterns throughout

## Development Guidelines

- Follow RESTful conventions for new endpoints
- Use async/await for all service operations
- Implement proper error handling and HTTP status codes
- Add XML documentation comments for API methods
- Use dependency injection for service registration
- Support flexible metadata through Dictionary<string, string>
- Implement proper document lifecycle management
- Follow Hyland document management best practices

## Available Tasks

- **build**: Build the project using `dotnet build`
- **run**: Run the development server (listening on http://localhost:5214)

## API Endpoints

The service provides document management operations at `/api/documents`:
- GET (all active documents)
- GET by ID
- POST (create new document)
- PUT (update document metadata)
- DELETE (soft delete document)
- POST /search (search with criteria)
- POST /{id}/archive (archive document)
- GET /{id}/content (download content)

## Document Model Structure

- **Basic Info**: ID, AutoName (read-only), FileName, FileType, ContentType
- **File Details**: FileSizeBytes, FilePath
- **Metadata**: Flexible key-value pairs for custom properties
- **Audit Info**: UploadedAt, ModifiedAt, UploadedBy
- **Status**: Enum for document lifecycle management

## Integration Notes

Designed for Hyland document management system integration:
- Supports flexible metadata for custom properties
- Document status workflow for lifecycle management
- Search capabilities for metadata-based queries
- Audit trail for document activities

All endpoints return appropriate HTTP status codes and JSON responses.