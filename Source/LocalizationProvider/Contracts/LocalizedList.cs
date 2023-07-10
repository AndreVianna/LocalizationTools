namespace LocalizationProvider.Contracts;

public record LocalizedList(string Key, LocalizedText? Label, LocalizedText[] Items);
