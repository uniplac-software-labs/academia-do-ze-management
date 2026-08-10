// Nome: [Pedro Henrique dos Santos]

using System.Collections.Generic;
using System.Linq;

namespace AcademiaDoZe.Domain.Common;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyCollection<Notification> Notifications { get; }

    protected Result(bool isSuccess, IEnumerable<Notification>? notifications = null)
    {
        IsSuccess = isSuccess;
        Notifications = notifications?.ToList().AsReadOnly() ?? new List<Notification>().AsReadOnly();
    }

    public static Result Success() => new(true);
    public static Result Failure(IEnumerable<Notification> notifications) => new(false, notifications);
    public static Result Failure(string key, string message) => new(false, new[] { new Notification(key, message) });

    public static Result<TValue> Success<TValue>(TValue value) => Result<TValue>.Success(value);
    public static Result<TValue> Failure<TValue>(IEnumerable<Notification> notifications) => Result<TValue>.Failure(notifications);
    public static Result<TValue> Failure<TValue>(string key, string message) => Result<TValue>.Failure(key, message);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Não é possível acessar o valor de um resultado com falha.");

    private Result(TValue? value, bool isSuccess, IEnumerable<Notification>? notifications = null)
        : base(isSuccess, notifications)
    {
        _value = value;
    }

    public static Result<TValue> Success(TValue value) => new(value, true);
    public static new Result<TValue> Failure(IEnumerable<Notification> notifications) => new(default, false, notifications);
    public static new Result<TValue> Failure(string key, string message) => new(default, false, new[] { new Notification(key, message) });
}