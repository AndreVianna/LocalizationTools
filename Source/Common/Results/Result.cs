using static System.Utilities.Ensure;

namespace System.Results;

public record Result : IResult {
    protected Result(IEnumerable<ValidationError>? errors = null) {
        Errors = errors?.ToList() ?? new List<ValidationError>();
    }

    public static Result Success() => new();
    public static Result Invalid(string message, string source, params object?[] args) => Invalid(new ValidationError(message, source, args));
    public static Result Invalid(Result result) => new(result.Errors);

    public static Result<TValue> Success<TValue>(TValue? value) => new(value);
    public static Result<TValue> Invalid<TValue>(TValue? value, string message, string source) => Invalid(value, new ValidationError(message, source));
    public static Result<TValue> Invalid<TValue>(TValue? value, ValidationError error) => Invalid(value, new[] { error });
    public static Result<TValue> Invalid<TValue>(TValue? value, IEnumerable<ValidationError> errors) => new(value, IsNotNullAndDoesNotHaveNull(errors));

    public IList<ValidationError> Errors { get; protected init; } = new List<ValidationError>();
    public bool IsInvalid => Errors.Count != 0;

    public virtual bool IsSuccess => !IsInvalid;

    public static implicit operator Result(List<ValidationError> errors) => new(errors.AsEnumerable());
    public static implicit operator Result(ValidationError[] errors) => new(errors.AsEnumerable());
    public static implicit operator Result(ValidationError error) => new(new[] { error }.AsEnumerable());

    public static Result operator +(Result left, Result right) {
        left.Errors.MergeWith(right.Errors.Distinct());
        return left;
    }

    public virtual bool Equals(Result? other)
        => other is not null
        && Errors.SequenceEqual(other.Errors);

    public override int GetHashCode()
        => Errors.Aggregate(Array.Empty<ValidationError>().GetHashCode(), HashCode.Combine);
}

public record Result<TValue> : Result {
    public Result(TValue? value = default, IEnumerable<ValidationError>? errors = null)
        : base(errors) {
        Value = value;
    }
    public TValue? Value { get; }

    public bool IsNull => !IsInvalid && Value is null;
    public bool IsNotNull => !IsInvalid && Value is not null;

    public static implicit operator Result<TValue>(TValue? value) => new(value);

    public static Result<TValue> operator +(Result<TValue> left, Result right) {
        left.Errors.MergeWith(right.Errors.Distinct());
        return left;
    }

    public Result<TOutput> MapTo<TOutput>(Func<TValue?, TOutput?> map)
        => new(map(Value), Errors);
}
