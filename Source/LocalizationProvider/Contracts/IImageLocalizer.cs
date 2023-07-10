namespace LocalizationProvider.Contracts;

public interface IImageLocalizer : ILocalizer {
    LocalizedImage? GetLocalizedImage(string imageKey);
    byte[]? this[string imageKey] { get; }
}
