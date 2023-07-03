using LocalizationManager.Models;

namespace LocalizationManager.Contracts;

public interface IResourceWriter {
    static abstract IResourceWriter CreateFor(Guid applicationId, IServiceProvider services);
    IResourceWriter For(string culture);
    void SetText(LocalizedText input);
    void SetList(LocalizedList input);
    void SetImage(LocalizedImage input);
    void SetDateTimeFormat(LocalizedDateTimeFormat input);
}