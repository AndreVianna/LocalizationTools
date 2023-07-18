namespace LocalizationProvider.Contracts;

public interface ITextResourceHandler {
    LocalizedText? Get(string textKey);
}
