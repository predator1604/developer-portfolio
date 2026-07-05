using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Portfolio.Application.Common.Behaviours;

// ── Validation Behaviour ──────────────────────────────────────────────────────

/// <summary>
/// Intercepts every IRequest before it reaches the handler.
/// Runs all registered FluentValidation validators.
/// Returns a Result failure instead of throwing exceptions.
/// </summary>
public sealed class ValidationBehaviour<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (!validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0) return await next();

        var errors = string.Join("; ", failures.Select(f => f.ErrorMessage));
        logger.LogWarning("Validation failed for {Request}: {Errors}", typeof(TRequest).Name, errors);

        // Construct a failure Result dynamically so all Result<T> types work
        var resultType = typeof(TResponse);
        if (resultType == typeof(Result))
            return (TResponse)(object)Result.Failure(errors, "VALIDATION_ERROR");

        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var innerType = resultType.GetGenericArguments()[0];
            var failureMethod = typeof(Result<>)
                .MakeGenericType(innerType)
                .GetMethod(nameof(Result<object>.Failure),
                    new[] { typeof(string), typeof(string) })!;
            return (TResponse)failureMethod.Invoke(null, new object?[] { errors, "VALIDATION_ERROR" })!;
        }

        throw new ValidationException(failures);
    }
}

// ── Logging Behaviour ─────────────────────────────────────────────────────────

/// <summary>
/// Logs every request entering and leaving the MediatR pipeline.
/// Measures execution time and warns on slow queries (>500ms).
/// </summary>
public sealed class LoggingBehaviour<TRequest, TResponse>(
    ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private const int SlowRequestThresholdMs = 500;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var name  = typeof(TRequest).Name;
        var start = DateTime.UtcNow;

        logger.LogInformation("Handling {Request}", name);

        var response = await next();

        var elapsed = (DateTime.UtcNow - start).TotalMilliseconds;

        if (elapsed > SlowRequestThresholdMs)
            logger.LogWarning("Slow request: {Request} took {Elapsed}ms", name, elapsed);
        else
            logger.LogInformation("Handled {Request} in {Elapsed}ms", name, elapsed);

        return response;
    }
}
