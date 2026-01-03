using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using quantity_move_api.Services;
using quantity_move_api.Services.Stock;
using quantity_move_api.Services.Quantity;
using quantity_move_api.Middleware;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddSwaggerGen();

// Add health checks with database connection check
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy())
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

// Register quantity services
builder.Services.AddScoped<IQuantityMoveService, QuantityMoveService>();
builder.Services.AddScoped<IQuantityValidationService, QuantityValidationService>();

var app = builder.Build();

// Configure path base for /api
app.UsePathBase(new PathString("/api"));

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS must be before authentication/authorization
app.UseCors();

// Global exception handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Map health check endpoints (before controllers to allow unauthenticated access)
app.MapHealthChecks("/api/health/ready");

app.MapControllers();

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
