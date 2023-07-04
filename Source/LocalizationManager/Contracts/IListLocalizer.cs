namespace LocalizationManager.Contracts;

public interface IListLocalizer : ILocalizer
{
    string[] this[string listId] { get; }
    string? this[string listId, uint index] { get; }
    string[] GetLists();
}
