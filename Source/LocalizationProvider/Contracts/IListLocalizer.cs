namespace LocalizationProvider.Contracts;

public interface IListLocalizer : ITypedLocalizer {
    string[] this[string listKey] { get; }
    string this[string listKey, string itemKey] { get; }
}
