using static System.Utilities.Ensure;

namespace System.Results;

public sealed record SignInResult : Result {
    public SignInResult(SignInResultType type, string? token = null, IEnumerable<ValidationError>? errors = null)
        : base(errors) {
        Type = IsInvalid ? SignInResultType.Invalid : type;
        Token = IsSuccess ? IsNotNull(token) : null; // only set token if success.
    }

    public string? Token { get; }
    public SignInResultType Type { get; private set; }
    public bool IsLocked => !IsInvalid && Type is SignInResultType.Locked;
    public bool IsBlocked => !IsInvalid && Type is SignInResultType.Blocked;
    public bool IsFailure => !IsInvalid && Type is SignInResultType.Failed;
    public bool RequiresConfirmation => !IsInvalid && Type is SignInResultType.ConfirmationRequired;
    public bool RequiresTwoFactor => !IsInvalid && Type is SignInResultType.TwoFactorRequired;
    public override bool IsSuccess => !IsInvalid && Type is SignInResultType.Success;

    public static SignInResult ConfirmationRequired(string token)
        => new(SignInResultType.ConfirmationRequired, token);
    public static SignInResult TwoFactorRequired(string token)
        => new(SignInResultType.TwoFactorRequired, token);
    public static SignInResult Success(string token)
        => new(SignInResultType.Success, token);
    public static new SignInResult Invalid(string message, string source, params object?[] args)
        => Invalid(new ValidationError(message, source, args));
    public static new SignInResult Invalid(Result result)
        => new(SignInResultType.Invalid, null, result.Errors);

    public static SignInResult Blocked() => new(SignInResultType.Blocked);
    public static SignInResult Locked() => new(SignInResultType.Locked);
    public static SignInResult Failure() => new(SignInResultType.Failed);

    public static implicit operator SignInResult(List<ValidationError> errors) => new(SignInResultType.Invalid, null, IsNotNullAndDoesNotHaveNull(errors));
    public static implicit operator SignInResult(ValidationError[] errors) => new(SignInResultType.Invalid, null, IsNotNullAndDoesNotHaveNull(errors));
    public static implicit operator SignInResult(ValidationError error) => new(SignInResultType.Invalid, null, new[] { error }.AsEnumerable());
    public static implicit operator SignInResult(string token) => new(SignInResultType.Success, token);
    public static implicit operator SignInResult(SignInResultType resultType)
        => resultType switch {
            SignInResultType.Invalid => throw new InvalidCastException("To make an invalid result assigned the errors directly. i.e. SingInResult result = \"[Token]\";"),
            SignInResultType.Success or SignInResultType.TwoFactorRequired => throw new InvalidCastException("To make a successful result assigned the token to it. i.e. SingInResult result = \"[Token]\";"),
            _ => new SignInResult(resultType)
        };

    public static SignInResult operator +(SignInResult left, Result right) {
        left.Errors.MergeWith(right.Errors.Distinct());
        left.Type = left.IsInvalid ? SignInResultType.Invalid : left.Type;
        return left;
    }

    public static bool operator ==(SignInResult left, SignInResultType right) => left.Type == right;
    public static bool operator !=(SignInResult left, SignInResultType right) => left.Type != right;
}
