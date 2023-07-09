namespace LocalizationManager.Contracts;

public interface ILocalizationHandler : ILocalizationReader {
    void SetText(LocalizedText input);
    void SetList(LocalizedList input);
    void SetImage(LocalizedImage input);
}
