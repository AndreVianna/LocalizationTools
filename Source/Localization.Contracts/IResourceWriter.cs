namespace Localization.Contracts;

public interface IResourceWriter {
    static abstract IResourceWriter CreateFor(Guid applicationId, IServiceProvider services);
    IResourceWriter For(string culture);
    void SetText(string key, string value);
    void SetListItems(string listKey, (string key, string value)[] items);
    void SetListItem(string listKey, uint index, string key, string value);
    void SetImage(string key, byte[] bytes);
    void SetDateTimeFormat(DateTimeFormat format, string pattern);
}