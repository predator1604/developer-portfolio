using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Domain.Interfaces;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Infrastructure.Persistence.Repositories;
using Portfolio.Infrastructure.Services.AI;
using Portfolio.Infrastructure.Services.Email;

namespace Portfolio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // ── MongoDB ───────────────────────────────────────────────────────────
        services.Configure<MongoDbSettings>(config.GetSection("MongoDB"));

        var mongoSettings = config.GetSection("MongoDB").Get<MongoDbSettings>()!;

        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoSettings.ConnectionString));

        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<MongoDbSettings>>();
            return new MongoDbContext(client, settings.Value);
        });

        // ── Repositories ──────────────────────────────────────────────────────
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        services.AddScoped<IChatSessionRepository, ChatSessionRepository>();

        // ── Email ─────────────────────────────────────────────────────────────
        services.Configure<SmtpSettings>(config.GetSection("Smtp"));
        services.AddTransient<IEmailService, SmtpEmailService>();

        // ── AI ────────────────────────────────────────────────────────────────
        services.Configure<AiSettings>(config.GetSection("AI"));
        services.AddTransient<IAiChatService, SemanticKernelChatService>();

        return services;
    }
}
