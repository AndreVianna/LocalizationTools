namespace LocalizationProvider.Contracts;

public interface IResourceRepository : IResourceReader {
    void SetText(LocalizedText input);
    void SetList(LocalizedList input);
    void SetImage(LocalizedImage input);
}
