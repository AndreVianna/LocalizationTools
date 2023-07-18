namespace LocalizationProvider.Contracts;

public interface IListLocalizer : ILocalizer {
    string[] this[string listKey] { get; }
    string this[string listKey, string itemKey] { get; }
}
