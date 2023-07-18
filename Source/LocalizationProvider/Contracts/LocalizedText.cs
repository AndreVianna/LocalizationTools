namespace LocalizationProvider.Contracts;

public record LocalizedText(string Key, string? Value)
    : ILocalizedResource<LocalizedText> {
    public static ResourceType Type => ResourceType.Text;
}
