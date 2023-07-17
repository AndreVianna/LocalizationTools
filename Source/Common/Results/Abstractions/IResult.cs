namespace System.Results.Abstractions;

public interface IResult {
    bool IsSuccess { get; }
    IList<ValidationError> Errors { get; }
}
