namespace LocalizationManager.Contracts;

public interface IResourceReader {
    static abstract IResourceReader CreateFor(Guid applicationId, IServiceProvider services);
    IResourceReader For(string culture);

    string? GetTextOrDefault(string textKey);
    string[] GetLists();
    string[]? GetListItemsOrDefault(string listKey);
    string? GetListItemOrDefault(string listKey, uint index);
    byte[]? GetImageOrDefault(string imageKey);
    string GetDateTimeFormat(DateTimeFormat format);
    string GetNumberFormat(int decimalPlaces = 0, int integerDigits = 1);
}