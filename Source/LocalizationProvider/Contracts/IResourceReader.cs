namespace LocalizationProvider.Contracts;

public interface IResourceReader {
    LocalizedText? FindText(string textKey);
    LocalizedList? FindList(string listKey);
    LocalizedImage? FindImage(string imageKey);
}
