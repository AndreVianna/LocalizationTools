namespace LocalizationManager;

internal sealed class ListLocalizer
    : Localizer<ListLocalizer>,
    IListLocalizer
{
    internal ListLocalizer(ILocalizationProvider provider, string culture, ILogger<ListLocalizer> logger)
        : base(provider, culture, logger)
    { }
    
    public string? this[string listId, uint index]
        => GetLocalizedResource(listId, LocalizerType.List, string.Empty , rdr => rdr.GetListItemOrDefault(listId, index))!;

    public string[] this[string listId]
        => GetLocalizedResource(listId, LocalizerType.List, Array.Empty<string>(), rdr => rdr.GetListItemsOrDefault(listId))!;

    public string[] GetLists()
        => GetLocalizedResource(Keys.AllListsKey, LocalizerType.List, Array.Empty<string>(), rdr => rdr.GetLists());
}
