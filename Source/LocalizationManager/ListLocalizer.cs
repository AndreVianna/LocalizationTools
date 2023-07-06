using LocalizationManager.Contracts;

namespace LocalizationManager;

internal sealed class ListLocalizer
    : Localizer<ListLocalizer>,
    IListLocalizer {
    internal ListLocalizer(ILocalizationProvider provider, string culture, ILogger<ListLocalizer> logger)
        : base(provider, culture, logger) { }

    public string this[string listKey, uint index]
        => GetResource(listKey, ResourceType.List, rdr => rdr.GetListItem(listKey, index))!;

    public string[] this[string listKey]
        => GetResource(listKey, ResourceType.List, rdr => rdr.GetListItems(listKey)) ?? Array.Empty<string>();

    public string[] GetLists()
        => GetResource(Keys.AllListsKey, ResourceType.List, rdr => rdr.GetLists()) ?? Array.Empty<string>();
}
