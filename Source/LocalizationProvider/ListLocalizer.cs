using LocalizationProvider.Contracts;

namespace LocalizationProvider;

internal sealed class ListLocalizer
    : Localizer<ListLocalizer>,
    IListLocalizer {
    internal ListLocalizer(ILocalizationReader reader, ILogger<ListLocalizer> logger)
        : base(reader, logger) { }

    public LocalizedList? GetLocalizedList(string lisKey)
        => GetResourceOrDefault(lisKey, List, rdr
                => rdr.FindList(lisKey));

    public string[] this[string listKey]
        => GetLocalizedList(listKey)?
          .Items
          .Select(i => i.Value ?? i.Key)
          .ToArray() ?? Array.Empty<string>();

    public string this[string listKey, string itemKey]
        => itemKey == Keys.ListLabel
            ? GetListLabel(listKey)
            : GetListItemByKey(listKey, itemKey);

    private string GetListLabel(string listKey) {
        var list = GetLocalizedList(listKey);
        var label = list?.Label;
        return label?.Value ?? label?.Key ?? listKey;
    }

    private string GetListItemByKey(string listKey, string itemKey)
        => GetLocalizedList(listKey)?
                .Items
                .FirstOrDefault(i => i.Key == itemKey)?
                .Value ?? itemKey;
}
