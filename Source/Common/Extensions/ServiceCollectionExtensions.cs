namespace System.Extensions;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddDefaultSystemComponents(this IServiceCollection services) {
        services.AddSingleton<DateTimeProvider>();
        services.AddSingleton<FileSystemProvider>();
        return services;
    }
}
