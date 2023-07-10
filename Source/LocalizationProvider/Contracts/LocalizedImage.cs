namespace LocalizationProvider.Contracts;

public record LocalizedImage(string Key, LocalizedText? Label, byte[] Bytes);
