namespace Localization.Contracts;

public interface IResourceReader {
    static abstract IResourceReader CreateFor(Guid applicationId, IServiceProvider services);
    IResourceReader For(string culture);

    string? GetTextOrDefault(string textId);
    string[] GetLists();
    string[]? GetListItemsOrDefault(string listId);
    string? GetListItemOrDefault(string listId, uint index);
    Stream? GetImageOrDefault(string imageId);
    string GetDateTimeFormat(DateTimeFormat dateTimeFormat);
    string GetNumberFormat(int decimalPlaces = 0, int integerDigits = 1);
}