using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.OpenApi;
using quantity_move_api.Services;
using quantity_move_api.Services.Stock;
using quantity_move_api.Services.Quantity;
using quantity_move_api.Middleware;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
var environment = builder.Environment.EnvironmentName;
var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
var processId = Environment.ProcessId;

var loggerConfig = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithProperty("Application", "quantity-move-api")
    .Enrich.WithProperty("Version", version)
    .Enrich.WithProperty("ProcessId", processId);

// Configure console sink based on environment
if (environment == "Development")
{
    loggerConfig.WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
}
else
{
    loggerConfig.WriteTo.Console(new CompactJsonFormatter());
}

Log.Logger = loggerConfig
    .WriteTo.File(
        path: "logs/app-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        fileSizeLimitBytes: 100_000_000, // 100MB
        rollOnFileSizeLimit: true,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
        shared: true
    )
    .WriteTo.File(
        path: "logs/app-structured-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        fileSizeLimitBytes: 100_000_000,
        rollOnFileSizeLimit: true,
        formatter: new CompactJsonFormatter(),
        shared: true
    )
    .WriteTo.File(
        path: "logs/errors-.log",
        restrictedToMinimumLevel: LogEventLevel.Warning,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        fileSizeLimitBytes: 100_000_000,
        rollOnFileSizeLimit: true,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
        shared: true
    )
    .CreateLogger();

// Use Serilog for logging
builder.Host.UseSerilog();

// Validate critical configuration
ValidateConfiguration(builder.Configuration);

// Add services to the container
builder.Services.AddControllers();

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Note: API versioning is configured but controllers currently use unversioned routes
// To enable versioning, update controller routes to: [Route("api/v{version:apiVersion}/stock")]
// For now, versioning infrastructure is in place for future use

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{

    // Add JWT Bearer authentication to Swagger
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter your token in the text input below.\n\nExample: \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });
});

// Add health checks with database connection check
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddSqlServer(
        connectionString ?? throw new InvalidOperationException("Connection string is not configured"),
        name: "database",
        tags: new[] { "db", "sql", "sqlserver" },
        timeout: TimeSpan.FromSeconds(15));

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
                ?? new[] { "http://localhost:3000", "https://localhost:3000" })
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Register core services
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<quantity_move_api.Services.Query.IQueryService, quantity_move_api.Services.Query.QueryService>();

// Register repositories
builder.Services.AddScoped<quantity_move_api.Repositories.ILotLocationRepository, quantity_move_api.Repositories.LotLocationRepository>();
builder.Services.AddScoped<quantity_move_api.Repositories.IItemRepository, quantity_move_api.Repositories.ItemRepository>();
builder.Services.AddScoped<quantity_move_api.Repositories.ILocationRepository, quantity_move_api.Repositories.LocationRepository>();

// Register stock services
builder.Services.AddScoped<IStockQueryService, StockQueryService>();

// Register FIFO service (must be registered before QuantityMoveService which depends on it)
builder.Services.AddScoped<quantity_move_api.Services.Fifo.IFifoService, quantity_move_api.Services.Fifo.FifoService>();

// Register quantity services
builder.Services.AddScoped<IQuantityMoveService, QuantityMoveService>();
builder.Services.AddScoped<IQuantityValidationService, QuantityValidationService>();

// Register metrics service as singleton (in-memory metrics)
builder.Services.AddSingleton<IMetricsService, MetricsService>();

var app = builder.Build();

// Configure application-level exception handler
// This catches exceptions that occur before or outside the ExceptionHandlingMiddleware
// Examples: model binding errors, middleware exceptions, pipeline setup errors
// Must be placed early in the pipeline to catch exceptions from all subsequent middleware
app.UseExceptionHandler("/api/error");

// Configure the HTTP request pipeline
var swaggerEnabled = builder.Configuration.GetValue<bool>("Swagger:Enabled", false);
if (app.Environment.IsDevelopment() || swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS must be before authentication/authorization
app.UseCors();

// Request logging middleware (should be early in pipeline)
app.UseMiddleware<RequestLoggingMiddleware>();

// Global exception handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Note: Health check endpoints are handled by HealthController
// The built-in MapHealthChecks middleware was removed to avoid route conflicts
// HealthController provides custom health endpoints with metrics and detailed information

app.MapControllers();

try
{
    Log.Information("Starting Quantity Move API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Make Program class accessible for integration tests
public partial class Program 
{
    /// <summary>
    /// Validates critical configuration values at startup.
    /// Throws InvalidOperationException if required configuration is missing.
    /// </summary>
    private static void ValidateConfiguration(IConfiguration configuration)
    {
        // Validate database connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Required configuration 'ConnectionStrings:DefaultConnection' is missing or empty.");
        }

        // Validate JWT settings
        var jwtSettings = configuration.GetSection("Jwt");
        var secretKey = jwtSettings["SecretKey"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException(
                "Required configuration 'Jwt:SecretKey' is missing or empty.");
        }

        var issuer = jwtSettings["Issuer"];
        if (string.IsNullOrEmpty(issuer))
        {
            throw new InvalidOperationException(
                "Required configuration 'Jwt:Issuer' is missing or empty.");
        }

        var audience = jwtSettings["Audience"];
        if (string.IsNullOrEmpty(audience))
        {
            throw new InvalidOperationException(
                "Required configuration 'Jwt:Audience' is missing or empty.");
        }

        // Validate required stored procedure names
        var storedProcedures = configuration.GetSection("StoredProcedures");
        var moveQuantityProcedure = storedProcedures["MoveQuantity"];
        if (string.IsNullOrEmpty(moveQuantityProcedure))
        {
            throw new InvalidOperationException(
                "Required configuration 'StoredProcedures:MoveQuantity' is missing or empty.");
        }

        var validateStockProcedure = storedProcedures["ValidateStock"];
        if (string.IsNullOrEmpty(validateStockProcedure))
        {
            throw new InvalidOperationException(
                "Required configuration 'StoredProcedures:ValidateStock' is missing or empty.");
        }
    }
}
