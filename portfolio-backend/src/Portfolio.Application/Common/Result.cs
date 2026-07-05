namespace Portfolio.Application.Common;

/// <summary>
/// Discriminated union result type.
/// Handlers return Result&lt;T&gt; — the API layer maps to HTTP status codes.
/// </summary>
public sealed class Result<T>
{
    public bool    IsSuccess { get; }
    public T?      Value     { get; }
    public string? Error     { get; }
    public string? ErrorCode { get; }

    private Result(bool isSuccess, T? value, string? error, string? errorCode)
    {
        IsSuccess = isSuccess;
        Value     = value;
        Error     = error;
        ErrorCode = errorCode;
    }

    public static Result<T> Success(T value)                         => new(true,  value, null,  null);
    public static Result<T> Failure(string error, string? code = null) => new(false, default, error, code);

    public static implicit operator Result<T>(T value) => Success(value);
}

/// <summary>Non-generic result for commands that return no value.</summary>
public sealed class Result
{
    public bool    IsSuccess { get; }
    public string? Error     { get; }
    public string? ErrorCode { get; }

    private Result(bool isSuccess, string? error, string? errorCode)
    {
        IsSuccess = isSuccess;
        Error     = error;
        ErrorCode = errorCode;
    }

    public static Result Success()                                      => new(true,  null,  null);
    public static Result Failure(string error, string? code = null)    => new(false, error, code);
}
