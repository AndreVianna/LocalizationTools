namespace System.Utilities;

public static class Create {
    public static T Instance<T>(params object?[]? args)
        where T : class
        => (T)Activator.CreateInstance(typeof(T), args)!;
}
