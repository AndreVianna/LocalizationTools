using LocalizationManager.Contracts;

namespace LocalizationManager;

public sealed class ListLocalizer : IOptionsLocalizer
{
    private readonly IResourceReader _reader;
    private readonly string _culture;

    public ListLocalizer(IResourceReader reader, string culture)
    {
        _reader = reader;
        _culture = culture;
    }

    public string? this[string listId, uint index]
        => _reader.For(_culture).GetListItemOrDefault(listId, index);

    public string[] this[string listId]
        => _reader.For(_culture).GetListItemsOrDefault(listId) ?? Array.Empty<string>();

    public string[] GetLists()
        => _reader.For(_culture).GetLists();
}
