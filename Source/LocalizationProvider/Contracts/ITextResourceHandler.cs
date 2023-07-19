namespace LocalizationProvider.Contracts;

public interface ITextResourceHandler {
    LocalizedText? Get(string textKey);
    void Set(LocalizedText resource);
}
