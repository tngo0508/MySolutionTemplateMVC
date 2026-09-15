using Refit;
using Serilog;
using Company.App.Shared.Constants;
using Company.App.Shared.Contracts;
#if (IndividualAuth)
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Company.App.Data;
#elif (WindowsAuth)
using Microsoft.AspNetCore.Authentication.Negotiate;
#endif

// 1. Bootstrap early logging to catch startup errors
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting {AppName} (v{Version})...", AppVersion.ApplicationName, AppVersion.Current);

    var builder = WebApplication.CreateBuilder(args);

    // 2. Wire up Serilog from appsettings.json
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

#if (IndividualAuth)
    // 3a. Register EF Core DbContext & ASP.NET Core Identity (when --auth Individual is selected)
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));

    builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
        .AddEntityFrameworkStores<AppDbContext>();

    builder.Services.AddRazorPages();
#elif (WindowsAuth)
    // 3b. Register Windows Authentication (when --auth Windows is selected)
    builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
        .AddNegotiate();

    builder.Services.AddAuthorization(options =>
    {
        // Require authenticated Windows users across all endpoints by default
        options.FallbackPolicy = options.DefaultPolicy;
    });
#endif

    // 4. Add MVC Controllers and Views
    builder.Services.AddControllersWithViews();

    // 5. Register Health Checks
    builder.Services.AddHealthChecks();

    // 6. Register Refit Client with Standard HTTP Resilience Pipeline
    var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7100";

    builder.Services.AddRefitClient<IItemsApi>()
        .ConfigureHttpClient(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(15);
        })
        // Enables Microsoft.Extensions.Http.Resilience (retries with exponential jitter, circuit breaker, rate limiter)
        .AddStandardResilienceHandler();

    var app = builder.Build();

    // 7. Enable Serilog HTTP request logging
    app.UseSerilogRequestLogging();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();

#if (IndividualAuth || WindowsAuth)
    app.UseAuthentication();
#endif
    app.UseAuthorization();
    app.MapStaticAssets();

    // 8. Health Check endpoint
    app.MapHealthChecks("/health")
       .WithName("HealthCheck")
       .WithTags("System");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

#if (IndividualAuth)
    app.MapRazorPages();

    if (app.Environment.IsDevelopment())
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Could not automatically initialize Identity database on startup. Verify database connection string.");
        }
    }
#endif

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
