using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Common.Behaviours;

namespace Portfolio.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // MediatR — scans all handlers, commands, queries in this assembly
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);

            // Register pipeline behaviours in order: Logging → Validation → Handler
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        });

        // FluentValidation — scans all validators in this assembly
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
