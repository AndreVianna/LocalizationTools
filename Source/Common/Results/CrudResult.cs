using static System.Utilities.Ensure;

namespace System.Results;

public record CrudResult : Result {
    public CrudResult(CrudResultType type, IEnumerable<ValidationError>? errors = null)
        : base(errors) {
        Type = IsInvalid ? CrudResultType.Invalid : type;
    }

    public CrudResultType Type { get; protected set; }
    public override bool IsSuccess => !IsInvalid && Type is CrudResultType.Success;
    public bool IsNotFound => !IsInvalid
        ? Type is CrudResultType.NotFound
        : throw new InvalidOperationException("The result has validation errors. You must check for errors before checking if result is null.");
    public bool IsConflict => !IsInvalid
        ? Type is CrudResultType.Conflict
        : throw new InvalidOperationException("The result has validation errors. You must check for errors before checking if result has conflicts.");

    public static new CrudResult Success() => new(CrudResultType.Success);
    public static CrudResult NotFound() => new(CrudResultType.NotFound);
    public static CrudResult Conflict() => new(CrudResultType.Conflict);

    public static new CrudResult Invalid(string message, string source, params object?[] args) => Invalid(new ValidationError(message, source, args));
    public static new CrudResult Invalid(Result result) => new(CrudResultType.Invalid, result.Errors);

    public static new CrudResult<TValue> Success<TValue>(TValue? value) => new(CrudResultType.Success, value);
    public static CrudResult<TValue> NotFound<TValue>(TValue? value = default) => new(CrudResultType.NotFound, value);
    public static CrudResult<TValue> Conflict<TValue>(TValue? value) => new(CrudResultType.Conflict, value);
    public static new CrudResult<TValue> Invalid<TValue>(TValue? value, string message, string source) => Invalid(value, new ValidationError(message, source));
    public static new CrudResult<TValue> Invalid<TValue>(TValue? value, ValidationError error) => Invalid(value, new[] { error });
    public static new CrudResult<TValue> Invalid<TValue>(TValue? value, IEnumerable<ValidationError> errors) => new(CrudResultType.Invalid, value, IsNotNullAndDoesNotHaveNull(errors));

    public static implicit operator CrudResult(List<ValidationError> errors) => new(CrudResultType.Invalid, IsNotNullAndDoesNotHaveNull(errors));
    public static implicit operator CrudResult(ValidationError[] errors) => new(CrudResultType.Invalid, IsNotNullAndDoesNotHaveNull(errors));
    public static implicit operator CrudResult(ValidationError error) => new(CrudResultType.Invalid, new[] { error }.AsEnumerable());

    public static CrudResult operator +(CrudResult left, Result right) {
        left.Errors.MergeWith(right.Errors.Distinct());
        left.Type = left.IsInvalid ? CrudResultType.Invalid : left.Type;
        return left;
    }
}

public record CrudResult<TResult> : CrudResult {
    public CrudResult(CrudResultType type, TResult? value = default, IEnumerable<ValidationError>? errors = null)
        : base(type, errors) {
        Value = type != CrudResultType.NotFound ? IsNotNull(value) : value;
    }

    public TResult? Value { get; }

    public static implicit operator CrudResult<TResult>(TResult? value) => new(CrudResultType.Success, value);

    public static CrudResult<TResult> operator +(CrudResult<TResult> left, Result right) {
        left.Errors.MergeWith(right.Errors.Distinct());
        left.Type = left.IsInvalid ? CrudResultType.Invalid : left.Type;
        return left;
    }

    public CrudResult<TOutput> MapTo<TOutput>(Func<TResult?, TOutput?> map)
        => new(Type, map(Value), Errors);
}
