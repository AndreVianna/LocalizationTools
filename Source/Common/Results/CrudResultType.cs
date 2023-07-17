namespace System.Results;

public enum CrudResultType {
    Invalid = 0, // request validation failed.
    Success = 1, // operation succeeded.
    NotFound = 2, // operation failed because the resource was not found.
    Conflict = 3, // operation failed because the resource already exists.
}
