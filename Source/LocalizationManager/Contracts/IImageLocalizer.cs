using LocalizationManager.Models;

namespace LocalizationManager.Contracts;

public interface IImageLocalizer : ILocalizer {
    LocalizedImage? GetLocalizedImage(string imageKey);
    byte[]? this[string imageKey] { get; }
}
