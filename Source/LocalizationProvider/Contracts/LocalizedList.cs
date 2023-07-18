namespace LocalizationProvider.Contracts;

public record LocalizedList(string Key, LocalizedText[] Items)
    : ILocalizedResource<LocalizedList> {
    public static ResourceType Type => ResourceType.List;
}
