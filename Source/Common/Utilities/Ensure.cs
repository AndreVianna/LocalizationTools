using System.Collections;

namespace System.Utilities;

public static class Ensure {
    [return: NotNull]
    public static TArgument IsNotNull<TArgument>(TArgument? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        => argument is null
            ? throw new ArgumentNullException(paramName, GetInvertedErrorMessage(MustBeNull, paramName))
            : argument;

    [return: NotNull]
    public static TArgument IsOfType<TArgument>(object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        => IsNotNull(argument, paramName) is not TArgument result
            ? throw new ArgumentException(string.Format(MustBeOfType, paramName, typeof(TArgument).Name, argument!.GetType().Name), paramName)
            : result;

    [return: NotNull]
    public static TArgument IsNotNullOrEmpty<TArgument>(TArgument? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable {
        argument = IsNotNull(argument, paramName);
        return argument switch {
            string { Length: 0 }
                => throw new ArgumentException(GetInvertedErrorMessage(MustBeEmpty, paramName), paramName),
            string => argument,
            ICollection { Count: 0 }
                => throw new ArgumentException(GetInvertedErrorMessage(MustBeEmpty, paramName), paramName),
            _ => argument,
        };
    }

    public static string IsNotNullOrWhiteSpace(string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null) {
        argument = IsNotNull(argument, paramName);
        return argument.Trim().Length == 0
            ? throw new ArgumentException(GetInvertedErrorMessage(MustBeEmptyOrWhitespace, paramName), paramName)
            : argument;
    }

    [return: NotNull]
    public static TArgument IsNotNullAndDoesNotHaveNull<TArgument>(TArgument? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable {
        argument = IsNotNull(argument, paramName);
        return argument switch {
            IEnumerable collection when collection.Cast<object?>().Any(item => item is null)
                => throw new ArgumentException(GetInvertedErrorMessage(MustContainNull, paramName), paramName),
            _ => argument
        };
    }

    [return: NotNull]
    [SuppressMessage("Style", "IDE0200:Remove unnecessary lambda expression", Justification = "<Pending>")]
    public static TArgument IsNotNullAndDoesNotHaveNullOrEmpty<TArgument>(TArgument? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable<string?> {
        argument = IsNotNull(argument, paramName);
        return argument switch {
            // ReSharper disable once ConvertClosureToMethodGroup - it messes with code coverage
            IEnumerable<string?> collection when collection.Any(i => string.IsNullOrEmpty(i))
                => throw new ArgumentException(GetInvertedErrorMessage(MustContainNullOrEmpty, paramName), paramName),
            _ => argument,
        };
    }

    [return: NotNull]
    [SuppressMessage("Style", "IDE0200:Remove unnecessary lambda expression", Justification = "<Pending>")]
    public static TArgument IsNotNullAndDoesNotHaveNullOrWhiteSpace<TArgument>(TArgument? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable<string?> {
        argument = IsNotNull(argument, paramName);
        return argument switch {
            // ReSharper disable once ConvertClosureToMethodGroup - it messes with code coverage
            IEnumerable<string?> collection when collection.Any(i => string.IsNullOrWhiteSpace(i))
                => throw new ArgumentException(GetInvertedErrorMessage(MustContainNullOrWhitespace, paramName), paramName),
            _ => argument,
        };
    }

    [return: NotNull]
    public static TArgument IsNotNullOrEmptyAndDoesNotHaveNull<TArgument>(TArgument? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable {
        argument = IsNotNullOrEmpty(argument, paramName);
        return argument switch {
            IEnumerable collection when collection.Cast<object?>().Any(x => x is null)
                => throw new ArgumentException(GetInvertedErrorMessage(MustContainNull, paramName), paramName),
            _ => argument,
        };
    }

    [return: NotNull]
    public static TArgument IsNotNullOrEmptyAndDoesNotHaveNullOrEmpty<TArgument>(TArgument? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable<string?> {
        argument = IsNotNullOrEmpty(argument, paramName);
        return argument switch {
            IEnumerable<string?> collection when collection.Any(string.IsNullOrEmpty)
                => throw new ArgumentException(GetInvertedErrorMessage(MustContainNullOrEmpty, paramName), paramName),
            _ => argument,
        };
    }

    [return: NotNull]
    public static TArgument IsNotNullOrEmptyAndDoesNotHaveNullOrWhiteSpace<TArgument>(TArgument? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable<string?> {
        argument = IsNotNullOrEmpty(argument, paramName);
        return argument switch {
            IEnumerable<string?> collection when collection.Any(string.IsNullOrWhiteSpace)
                => throw new ArgumentException(GetInvertedErrorMessage(MustContainNullOrWhitespace, paramName), paramName),
            _ => argument,
        };
    }

    [return: NotNull]
    public static TItem ArgumentExistsAndIsOfType<TItem>(string methodName, IReadOnlyList<object?> arguments, uint argumentIndex, [CallerArgumentExpression(nameof(arguments))] string? paramName = null)
        => argumentIndex >= arguments.Count
            ? throw new ArgumentException($"Invalid number of arguments for '{methodName}'. Missing argument {argumentIndex}.", paramName)
            : arguments[(int)argumentIndex] is TItem value
                    ? value
                    : throw new ArgumentException($"Invalid type of {paramName}[{argumentIndex}] of '{methodName}'. Expected: {typeof(TItem).Name}. Found: {arguments[(int)argumentIndex]!.GetType().Name}.", $"{paramName}[{argumentIndex}]");

    public static TItem[] ArgumentsAreAllOfType<TItem>(string methodName, IReadOnlyList<object?> arguments, [CallerArgumentExpression(nameof(arguments))] string? paramName = null) {
        var list = IsNotNullOrEmptyAndDoesNotHaveNull(arguments, paramName);
        for (var index = 0; index < list.Count; index++)
            ArgumentExistsAndIsOfType<TItem>(methodName, arguments, (uint)index, paramName);

        return list.Cast<TItem>().ToArray();
    }

    public static TItem? ArgumentExistsAndIsOfTypeOrDefault<TItem>(string methodName, IReadOnlyList<object?> arguments, uint argumentIndex, [CallerArgumentExpression(nameof(arguments))] string? paramName = null)
        => argumentIndex >= arguments.Count
            ? throw new ArgumentException($"Invalid number of arguments for '{methodName}'. Missing argument {argumentIndex}.", paramName)
            : arguments[(int)argumentIndex] switch {
                null => default,
                TItem value => value,
                _
                => throw new ArgumentException($"Invalid type of {paramName}[{argumentIndex}] of '{methodName}'. Expected: {typeof(TItem).Name}. Found: {arguments[(int)argumentIndex]!.GetType().Name}.", $"{paramName}[{argumentIndex}]")
            };

    public static TItem?[] ArgumentsAreAllOfTypeOrDefault<TItem>(string methodName, IReadOnlyList<object?> arguments, [CallerArgumentExpression(nameof(arguments))] string? paramName = null) {
        var list = IsNotNullOrEmpty(arguments, paramName);
        for (var index = 0; index < list.Count; index++)
            ArgumentExistsAndIsOfTypeOrDefault<TItem>(methodName, arguments, (uint)index, paramName);

        return list.Select(i => i is null ? default : (TItem)i).ToArray();
    }
}
