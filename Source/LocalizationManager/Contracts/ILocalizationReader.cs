namespace LocalizationManager.Contracts;

public interface ILocalizationReader {
    string GetText(string textKey);
    string GetDateTimeFormat(string formatKey);
    string GetNumberFormat(string formatKey);

    string[] GetLists();
    string[] GetListItems(string listKey);
    string GetListItem(string listKey, uint index);

    byte[]? GetImageOrDefault(string imageKey);
}
