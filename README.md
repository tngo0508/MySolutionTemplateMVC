# ASP.NET Core MVC and Web API Solution Template

A modern, production-ready .NET solution template featuring an ASP.NET Core MVC front-end, a Web API backend with Scalar OpenAPI documentation, Entity Framework Core data access, Refit type-safe HTTP client with resilience handlers, Serilog structured logging, and configurable authentication options.

The solution uses modern .NET features including XML-based Solution format (`.slnx`), Central Package Management (`Directory.Packages.props`), and a clean `src/` directory layout.

---

## Features

- **Modern Solution Architecture**: Built with `.slnx` solution format and Central Package Management (`Directory.Packages.props`).
- **Clean Project Separation**:
  - `src/{Name}.Shared`: Shared contracts, DTOs (`ItemDto`), Refit client interface (`IItemsApi`), custom exceptions (`ApiException`), and shared constants (`AppVersion`).
  - `src/{Name}.Data`: Entity Framework Core `AppDbContext`, entities (`Item`), connection resiliency with retry execution strategies.
  - `src/{Name}.ApiService`: ASP.NET Core Web API with Scalar interactive API reference UI (`/scalar/v1`), OpenAPI document generator (`/openapi/v1.json`), Serilog logging, health checks (`/health`), RFC 7807 `ProblemDetails` error handling, and development database auto-creation and seeding.
  - `src/{Name}.Web`: ASP.NET Core MVC application consuming the API via Refit with standard resilience policies (jitter retry, circuit breaker, rate limiting) from `Microsoft.Extensions.Http.Resilience`, error handling for API exceptions, and navigation login partials.
- **Client-Side Libraries (LibMan)**: Pre-configured Library Manager with Bootstrap 5, Chart.js, DataTables (with BS5 theme), Leaflet, Select2 (with BS5 theme), and jQuery.
- **Configurable Authentication Options**: Out-of-the-box support for `None`, `Individual` (ASP.NET Core Identity & EF Core stores), and `Windows` (Negotiate) authentication.
- **Standardized Developer Experience**: Consistent launch ports out-of-the-box (`7100` for ApiService, `7200` for Web) aligned with Refit client settings.

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
    │   ├── Exceptions/
    │   │   └── ApiException.cs
    │   └── {Name}.Shared.csproj
    ├── {Name}.Data/
    │   ├── Entities/
    │   │   └── Item.cs
    │   ├── AppDbContext.cs
    │   └── {Name}.Data.csproj
    ├── {Name}.ApiService/
    │   ├── Controllers/
    │   │   └── ItemsController.cs
    │   ├── Properties/
    │   │   └── launchSettings.json
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
        ├── Properties/
        │   └── launchSettings.json
        ├── Views/
        │   ├── Home/
        │   │   ├── Index.cshtml
        │   │   └── Privacy.cshtml
        │   ├── Items/
        │   │   └── Index.cshtml
        │   └── Shared/
        │       ├── _Layout.cshtml
        │       ├── _LoginPartial.cshtml (included with Individual or Windows auth)
        │       ├��─ _ValidationScriptsPartial.cshtml
        │       └── Error.cshtml
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

You should see **ASP.NET Core MVC and Web API Solution** with short names `mvcapi` and `mvc-api`.

---

## Creating a New Project

### Basic Usage

Create a new solution with a custom name:

```bash
dotnet new mvc-api -n MyCompany.App -o MyCompany.App
```

### Template Options

| Parameter | Short Form | Choices | Default | Description |
|---|---|---|---|---|
| `--auth` | `-a` | `None`, `Individual`, `Windows` | `None` | The type of authentication to configure for the solution. |

### Authentication Options

#### 1. No Authentication (Default)
Generates a clean project without authentication overhead:
```bash
dotnet new mvc-api -n MyCompany.App -o MyCompany.App --auth None
```

#### 2. Individual Authentication (ASP.NET Core Identity & EF Core)
Configures ASP.NET Core Identity, Identity UI Razor Pages (`/Identity/Account/Login`, `/Identity/Account/Register`), EF Core Identity stores, and login partial navigation:
```bash
dotnet new mvc-api -n MyCompany.App -o MyCompany.App --auth Individual
```

#### 3. Windows Authentication
Configures Windows Negotiate authentication scheme, fallback authorization policy, and authenticated user display (`Hello <Username>!`) on the navbar:
```bash
dotnet new mvc-api -n MyCompany.App -o MyCompany.App --auth Windows
```

---

## Configuration & Database

### Database Connection Strings
Both `ApiService` and `Web` (when using `Individual` authentication) are configured with LocalDB connection strings in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyCompany.AppDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

- In Development mode, `EnsureCreated()` automatically provisions the database schema and seeds initial sample items on startup.

### Refit API Settings
`Web/appsettings.json` points to the `ApiService` default HTTPS launch URL:

```json
"ApiSettings": {
  "BaseUrl": "https://localhost:7100"
}
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

Default URLs:
- HTTPS: `https://localhost:7100`
- HTTP: `http://localhost:5100`

Key Endpoints:
- **Scalar API Reference UI**: `https://localhost:7100/scalar/v1`
- **OpenAPI Specification**: `https://localhost:7100/openapi/v1.json`
- **Health Checks**: `https://localhost:7100/health`
- **Version Endpoint**: `https://localhost:7100/`

### 3. Run MVC Web Application
In a separate terminal, run the Web application:

```bash
dotnet run --project src/MyCompany.App.Web
```

Default URLs:
- HTTPS: `https://localhost:7200`
- HTTP: `http://localhost:5200`

Key Views:
- **Home UI**: `https://localhost:7200`
- **Items Management Dashboard**: `https://localhost:7200/Items` (consumes `IItemsApi` via Refit with resilience handlers)
- **Identity Pages** (with `--auth Individual`): `https://localhost:7200/Identity/Account/Login`

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
