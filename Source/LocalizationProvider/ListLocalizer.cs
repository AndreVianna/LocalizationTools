using static LocalizationProvider.Models.ResourceType;

namespace LocalizationProvider;

internal sealed class ListLocalizer
    : Localizer<ListLocalizer>,
    IListLocalizer {
    internal ListLocalizer(ILocalizationReader reader, ILogger<ListLocalizer> logger)
        : base(reader, logger) { }

    public LocalizedList? GetLocalizedList(string lisKey)
        => GetResourceOrDefault(lisKey, List, rdr => rdr.FindList(lisKey));

    public string[] this[string listKey]
        => GetLocalizedList(listKey)?
          .Items
          .Select(i => i.Value ?? i.Key)
          .ToArray() ?? Array.Empty<string>();

    public string this[string listKey, string itemKey]
        => GetListItem(listKey, itemKey);

    private string GetListItem(string listKey, string itemKey) {
        var s = GetLocalizedList(listKey)?
            .Items
            .FirstOrDefault(i => i.Key == itemKey);

        var v = s?.Value;
        if (v is not null) {
            return v;
        }

        return itemKey;
    }
}
