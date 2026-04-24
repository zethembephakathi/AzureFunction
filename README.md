# ABC_Retail Azure Function Application

## Project Overview

**ABC_Retail** is a comprehensive serverless retail management application built on **Azure Functions** and **ASP.NET Core**. It combines Azure's powerful cloud services with a multi-layered architecture to provide a robust platform for retail operations, inventory management, and order processing.

This is an enterprise-grade application that leverages Azure's serverless computing capabilities to handle retail business logic efficiently and cost-effectively.

## Technology Stack

- **Backend Framework**: ASP.NET Core (C#)
- **Serverless Compute**: Azure Functions
- **Storage Solutions**:
  - Azure Table Storage (for NoSQL data)
  - Azure Blob Storage (for file storage)
  - Azure Queue Storage (for asynchronous processing)
  - Azure File Share (for log file management)
- **Database**: SQL Server (via Entity Framework Core)
- **Language**: C# (60.1%), HTML (37.6%), CSS (2.1%), JavaScript (0.2%)

## Architecture & Components

### Core Projects

The solution consists of 5 integrated projects:

#### 1. **ABC_Retail.Functions**
Azure Functions project containing serverless functions for handling various retail operations.

**Key Functions:**
- **Function1.cs** - HTTP trigger function (GET/POST requests)
- **QueueFunction.cs** - Queue trigger processor for async message handling
- **WriteToBlobFunction.cs** - Handles blob storage operations
- **StoreToTableFunction.cs** - Manages table storage operations  
- **SendToAzureFilesFunction.cs** - File share integration for logging and file management

**Configuration:** Uses dependency injection with TableStorage and Entity Framework DbContext.

#### 2. **ABC_Retail** (Main Web Application)
ASP.NET Core MVC web application providing the user interface and API endpoints.

**Features:**
- Controllers for managing retail operations
- Razor Pages for server-side rendering
- Views for UI presentation (HTML/CSS/JavaScript)
- Integration with all Azure storage services via dependency injection

**Services Registered:**
- TableStorage - NoSQL data management
- BlobStorage - File storage operations
- QueueStorage - Asynchronous messaging
- LogFileService - Logging and file tracking

#### 3. **ABC_Retail.Services**
Shared service layer containing Azure storage abstractions.

**Service Classes:**
- **TableStorage.cs** - Azure Table Storage client wrapper
- **BlobStorage.cs** - Azure Blob Storage client wrapper
- **QueueStorage.cs** - Azure Queue Storage client wrapper
- **LogFileService.cs** - Azure File Share logging service

#### 4. **ABC_Retail.Models**
Data models and entity definitions used across the application.

#### 5. **AbcRetail_Data** (Data Layer)
Entity Framework Core DbContext and data access layer for SQL Server operations.

## Key Features

✅ **Serverless Architecture** - No infrastructure management; automatic scaling  
✅ **Multi-Storage Integration** - Works with Table, Blob, Queue, and File Storage  
✅ **Real-Time Processing** - Queue-based async processing for orders  
✅ **Web & API Interface** - Both MVC web UI and API endpoints  
✅ **Scalable Design** - Handles varying retail operation volumes  
✅ **Logging & Monitoring** - File-based logging via Azure File Share  
✅ **Cloud-Native** - Built specifically for Azure cloud platform

## Project Structure

```
AzureFunction/
├── README.md
└── ABC_Retail/
    ├── ABC_Retail.sln                 (Visual Studio Solution)
    ├── ABC_Retail/                    (Web App - MVC/Razor Pages)
    │   ├── Controllers/               (Route handlers)
    │   ├── Views/                     (HTML/CSS views)
    │   ├── wwwroot/                   (Static assets)
    │   ├── Program.cs                 (Dependency injection setup)
    │   └── appsettings.json           (Configuration)
    ├── ABC_Retail.Functions/          (Azure Functions)
    │   ├── Function1.cs               (HTTP trigger)
    │   ├── Functions/
    │   │   ├── QueueFunction.cs       (Queue trigger)
    │   │   ├── WriteToBlobFunction.cs (Blob storage)
    │   │   ├── StoreToTableFunction.cs (Table storage)
    │   │   └── SendToAzureFilesFunction.cs (File share)
    │   ├── Program.cs                 (Functions setup)
    │   └── host.json                  (Functions configuration)
    ├── ABC_Retail.Services/           (Service layer)
    │   └── Services/
    │       ├── TableStorage.cs
    │       ├── BlobStorage.cs
    │       ├── QueueStorage.cs
    │       └── LogFileService.cs
    ├── ABC_Retail.Models/             (Data models)
    └── AbcRetail_Data/                (Data layer & DbContext)
```

## Getting Started

### Prerequisites
- .NET 6.0+ SDK
- Azure Account with active subscription
- Visual Studio 2022 or VS Code
- Azure Functions Core Tools (v4+)
- Azure Storage Emulator (Azurite) for local development

### Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/zethembephakathi/AzureFunction.git
   cd AzureFunction
   ```

2. **Navigate to the project:**
   ```bash
   cd ABC_Retail/ABC_Retail
   ```

3. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

4. **Configure Azure Settings:**
   Create or update `appsettings.json` with your Azure connection strings:
   ```json
   {
     "AzureStorage:ConnectionString": "YOUR_STORAGE_CONNECTION_STRING",
     "AzureFileStorage:ConnectionString": "YOUR_FILE_STORAGE_CONNECTION_STRING",
     "AzureFileStorage:ShareName": "YOUR_SHARE_NAME"
   }
   ```

5. **For Azure Functions, set environment variables:**
   ```bash
   export AzureWebJobsStorage="YOUR_STORAGE_CONNECTION_STRING"
   export SqlConnectionString="YOUR_SQL_CONNECTION_STRING"
   ```

### Running Locally

**Web Application:**
```bash
cd ABC_Retail
dotnet run
```
Access at: `https://localhost:5001`

**Azure Functions:**
```bash
cd ABC_Retail.Functions
func start
```
Functions available at: `http://localhost:7071`

### Deploying to Azure

1. **Publish Web App:**
   ```bash
   dotnet publish -c Release
   ```

2. **Deploy Azure Functions:**
   ```bash
   func azure functionapp publish <YOUR_FUNCTION_APP_NAME>
   ```

## API Endpoints

- **Function1** - `POST/GET http://localhost:7071/api/Function1`
  - Welcome endpoint for HTTP requests

- **QueueProcessor** - Triggered by messages in `orders` queue
  - Processes retail orders from queue

- **WriteToBlobFunction** - Blob storage operations
- **StoreToTableFunction** - Table storage operations
- **SendToAzureFilesFunction** - File share logging

## Configuration

Key configuration files:
- `appsettings.json` - Application settings (connection strings, Azure services)
- `appsettings.Development.json` - Development-specific settings
- `host.json` - Azure Functions runtime configuration

## Development Notes

- The project uses Entity Framework Core for SQL operations
- Dependency Injection is configured in `Program.cs` for both web and functions
- Azure Storage abstractions (BlobStorage, TableStorage, QueueStorage) provide clean interfaces
- Local development can use Azurite for Azure Storage emulation

## Dependencies

Key NuGet packages:
- Microsoft.Azure.Functions.Worker
- Microsoft.EntityFrameworkCore
- Azure.Storage.Blobs
- Azure.Storage.Queues
- Azure.Storage.Files.Shares
