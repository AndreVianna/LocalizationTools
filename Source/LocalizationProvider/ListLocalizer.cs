namespace LocalizationManager;

internal sealed class ListLocalizer
    : Localizer<ListLocalizer>,
    IListLocalizer {
    internal ListLocalizer(ILocalizationReader reader, ILogger<ListLocalizer> logger)
        : base(reader, logger) { }

    public LocalizedList? GetLocalizedList(string lisKey)
        => GetResourceOrDefault(lisKey, List, rdr
                => rdr.FindList(lisKey));

    public string this[string listKey, string itemKey]
        => itemKey == Keys.ListLabelKey
            ? GetListLabel(listKey)
            : GetListItemByKey(listKey, itemKey);

    public string[] this[string listKey]
        => GetLocalizedList(listKey)?
            .Items
            .Select(i => i.Value ?? i.Key)
            .ToArray() ?? Array.Empty<string>();

    private string GetListLabel(string listKey)
        => GetLocalizedList(listKey)?
            .Label.Value ?? listKey;

    private string GetListItemByKey(string listKey, string itemKey)
        => GetLocalizedList(listKey)?
                .Items
                .FirstOrDefault(i => i.Key == itemKey)?
                .Value ?? itemKey;
}
