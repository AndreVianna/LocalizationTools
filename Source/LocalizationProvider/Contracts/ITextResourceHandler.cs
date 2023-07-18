namespace LocalizationProvider.Contracts;

public interface ITextResourceHandler {
    LocalizedText? GetLocalizedText(string textKey);
}
