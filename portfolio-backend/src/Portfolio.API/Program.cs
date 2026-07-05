using System.Threading.RateLimiting;
using Portfolio.API.Extensions;
using Portfolio.API.Middleware;
using Portfolio.Application;
using Portfolio.Infrastructure;
using Portfolio.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog ───────────────────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/portfolio-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ── Application + Infrastructure ──────────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ── Controllers + JSON ────────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;

        opts.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// ── Swagger ───────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Portfolio API",
        Version = "v1",
        Description = "Backend API for the portfolio website."
    });
});

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("PortfolioPolicy", policy =>
    {
        var origins = builder.Configuration
            .GetSection("AllowedOrigins")
            .Get<string[]>() ?? ["http://localhost:4200"];

        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ── Rate Limiter ──────────────────────────────────────────────────────────────
builder.Services.AddRateLimiter(opts =>
{
    opts.AddPolicy("contact", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    opts.AddPolicy("chat", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 30,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 2
            }));

    opts.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// ── Health Checks ─────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks();

var app = builder.Build();

// ── Middleware ────────────────────────────────────────────────────────────────
app.UseSerilogRequestLogging();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Portfolio API v1");

        // Opens Swagger at https://localhost:xxxx/
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseCors("PortfolioPolicy");

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

// ── Database Seeding ──────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbCtx = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        await DatabaseSeeder.SeedAsync(dbCtx, logger);
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Database seeding failed.");
    }
}

app.Run();