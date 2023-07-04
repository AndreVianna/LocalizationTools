namespace LocalizationManager;

public sealed class ListLocalizer : IListLocalizer
{
    private readonly ILocalizationReader _reader;

    public ListLocalizer(ILocalizationProvider provider, string culture)
    {
        _reader = provider.For(culture);
    }

    public string? this[string listId, uint index]
        => _reader.GetListItemOrDefault(listId, index);

    public string[] this[string listId]
        => _reader.GetListItemsOrDefault(listId) ?? Array.Empty<string>();

    public string[] GetLists()
        => _reader.GetLists();
}
