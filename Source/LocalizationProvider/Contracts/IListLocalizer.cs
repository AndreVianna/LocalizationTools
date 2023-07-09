namespace LocalizationManager.Contracts;

public interface IListLocalizer : ILocalizer {
    LocalizedList? GetLocalizedList(string listKey);
    string[] this[string listKey] { get; }
    string this[string listKey, string itemKey] { get; }
}
