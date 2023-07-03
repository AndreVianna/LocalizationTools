namespace LocalizationManager.Contracts;

public interface IOptionsLocalizer : ILocalizer {
    string[] this[string listId] { get; }
    string? this[string listId, uint index] { get; }
    string[] GetLists();
}
