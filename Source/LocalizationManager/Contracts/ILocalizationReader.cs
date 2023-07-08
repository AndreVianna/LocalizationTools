using LocalizationManager.Models;

namespace LocalizationManager.Contracts;

public interface ILocalizationReader {
    LocalizedText? FindText(string textKey);
    LocalizedList? FindList(string listKey);
    LocalizedImage? FindImage(string imageKey);
}
