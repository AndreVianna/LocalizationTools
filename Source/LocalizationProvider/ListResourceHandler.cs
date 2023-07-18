using static LocalizationProvider.Models.ResourceType;

namespace LocalizationProvider;

internal sealed class ListResourceHandler
    : Localizer<ListResourceHandler>
    , IListLocalizer
    , IListResourceHandler {
    internal ListResourceHandler(IResourceReader reader, ILogger<ListResourceHandler> logger)
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
        var list = GetLocalizedList(listKey);
        var item = list?.Items.FirstOrDefault(i => i.Key == itemKey);
        return item?.Value ?? itemKey;
    }
}
