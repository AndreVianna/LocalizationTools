namespace LocalizationProvider.Contracts;

public interface ILocalizedResourceProvider {
    static abstract ILocalizedResourceProvider Create(IServiceProvider services, string applicationId);

    string? GetLocalizedTextOrDefault(string culture, string text);
    string[] GetLocalizedList(string culture, string listId);
    string? GetLocalizedOptionOrDefault(string culture, string listId, uint index);
    Stream? GetLocalizedImageOrDefault(string culture, string name);
    string GetDateTimeFormat(string culture, DateTimeFormat dateTimeFormat);
    string GetNumberFormat(string culture, int integerDigits, int decimalPlaces);
}

public interface ILocalizedResourceProvider<TOptions>
    : ILocalizedResourceProvider where TOptions : LocalizationOptions {

    LocalizationOptions Options { get; init; }
}
