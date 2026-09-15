# ASP.NET Core MVC & API Solution Template

A modern, production-ready .NET solution template featuring an ASP.NET Core MVC front-end, a Web API backend with Scalar OpenAPI documentation, Entity Framework Core data access, Refit HTTP client with resilience handlers, Serilog structured logging, and configurable authentication options.

The solution uses modern .NET features including XML-based Solution format (`.slnx`), Central Package Management (`Directory.Packages.props`), and a clean `src/` directory layout.

---

## Features

- **Modern Solution Architecture**: Built with `.slnx` solution format and Central Package Management (`Directory.Packages.props`).
- **Clean Project Separation**:
  - `src/{Name}.Shared`: Shared contracts, DTOs (`ItemDto`), Refit client interface (`IItemsApi`), and shared constants (`AppVersion`).
  - `src/{Name}.Data`: Entity Framework Core `DbContext`, entities, and connection resiliency with retry policies.
  - `src/{Name}.ApiService`: ASP.NET Core Web API with Scalar interactive API reference UI (`/scalar/v1`), OpenAPI document generator (`/openapi/v1.json`), Serilog logging, health checks (`/health`), and RFC 7807 `ProblemDetails` error handling.
  - `src/{Name}.Web`: ASP.NET Core MVC application consuming the API via Refit with standard resilience policies (jitter retry, circuit breaker, rate limiting) from `Microsoft.Extensions.Http.Resilience`.
- **Client-Side Libraries (LibMan)**: Pre-configured Library Manager with Bootstrap 5, Chart.js, DataTables (with BS5 theme), Leaflet, Select2, and jQuery.
- **Configurable Authentication Options**: Out-of-the-box support for `None`, `Individual` (ASP.NET Core Identity & EF Core stores), and `Windows` (Negotiate) authentication.

---

## Solution Structure

When instantiated, the template generates the following layout:

```text
{Name}/
├── {Name}.slnx
├── Directory.Packages.props
└── src/
    ├── {Name}.Shared/
    │   ├── Constants/
    │   │   └── AppVersion.cs
    │   ├── Contracts/
    │   │   └── IItemsApi.cs
    │   ├── DTOs/
    │   │   └── ItemDto.cs
    │   └── {Name}.Shared.csproj
    ├── {Name}.Data/
    │   ├── Entities/
    │   │   └── Item.cs
    │   ├── AppDbContext.cs
    │   └── {Name}.Data.csproj
    ├── {Name}.ApiService/
    │   ├── Controllers/
    │   │   └── ItemsController.cs
    │   ├── appsettings.json
    │   ├── appsettings.Development.json
    │   ├── Program.cs
    │   └── {Name}.ApiService.csproj
    └── {Name}.Web/
        ├── Controllers/
        │   ├── HomeController.cs
        │   └── ItemsController.cs
        ├── Models/
        │   └── ErrorViewModel.cs
        ├── Views/
        │   ├── Home/
        │   ├── Items/
        │   │   └── Index.cshtml
        │   └── Shared/
        │       ├── _Layout.cshtml
        │       └── _LoginPartial.cshtml (included with Individual auth)
        ├── wwwroot/
        │   ├── css/
        │   ├── js/
        │   └── lib/
        │       ├── bootstrap/
        │       ├── chartjs/
        │       ├── datatables/
        │       ├── datatables-bs5/
        │       ├── jquery/
        │       ├── leaflet/
        │       ├── select2/
        │       └── select2-bootstrap-5-theme/
        ├── libman.json
        ├── appsettings.json
        ├── appsettings.Development.json
        ├── Program.cs
        └── {Name}.Web.csproj
```

---

## Installation

Install the template from the local repository directory:

```bash
dotnet new install <path-to-template-folder>
```

For example, if you are currently in the template directory:

```bash
dotnet new install .
```

To verify the template is installed, run:

```bash
dotnet new list --tag solution
```

You should see **ASP.NET Core MVC & API Solution with EF Core, Serilog, Scalar UI, Refit & Auth Options (.NET 10)** with short name `mvc-api`.

---

## Creating a New Project

### Basic Usage

Create a new solution with a custom name:

```bash
dotnet new mvc-api -n MyCompany.App -o MyCompany.App
```

### Authentication Options (`--auth` / `-a`)

The template supports three authentication configurations via the `--auth` parameter:

#### 1. No Authentication (Default)
```bash
dotnet new mvc-api -n MyCompany.App -o MyCompany.App --auth None
```

#### 2. Individual Authentication (ASP.NET Core Identity & EF Core)
Configures ASP.NET Core Identity, Identity UI Razor Pages, and login partials:
```bash
dotnet new mvc-api -n MyCompany.App -o MyCompany.App --auth Individual
```

#### 3. Windows Authentication
Configures Windows Negotiate authentication scheme and fallback authorization policy:
```bash
dotnet new mvc-api -n MyCompany.App -o MyCompany.App --auth Windows
```

---

## Building and Running

### 1. Build Solution
Navigate to your generated solution folder and build using the `.slnx` file:

```bash
dotnet build MyCompany.App.slnx
```

### 2. Run API Service
```bash
dotnet run --project src/MyCompany.App.ApiService
```
- **Scalar API Reference UI**: Navigate to `https://localhost:<port>/scalar/v1`
- **OpenAPI Specification**: `https://localhost:<port>/openapi/v1.json`
- **Health Checks**: `https://localhost:<port>/health`
- **Version Endpoint**: `https://localhost:<port>/`

### 3. Run MVC Web Application
```bash
dotnet run --project src/MyCompany.App.Web
```
- **Web UI**: Navigate to `https://localhost:<port>`
- **Items Management**: Access MVC views consuming `IItemsApi` via Refit.

---

## Managing Client-Side Libraries

Client-side libraries in `{Name}.Web` are managed using [Library Manager (LibMan)](https://learn.microsoft.com/aspnet/core/client-side/libman/). To restore or update client libraries:

```bash
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
libman restore --cwd src/MyCompany.App.Web
```

---

## Uninstalling the Template

To uninstall the template from your local `dotnet` environment:

```bash
dotnet new uninstall <path-to-template-folder>
```
