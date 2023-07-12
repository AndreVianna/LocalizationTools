namespace LocalizationProvider.Contracts;

public record LocalizedText(string Key, string? Value)
    : ILocalizedResource;
