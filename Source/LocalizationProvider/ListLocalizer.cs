namespace LocalizationProvider;

internal sealed class ListLocalizer : ITypedLocalizer {
    private readonly ListResourceHandler _handler;

    public static ResourceType Type => ResourceType.List;

    public ListLocalizer(ListResourceHandler handler) {
        _handler = handler;
    }

    public string[] this[string listKey]
        => _handler.Get(listKey)?
            .Items
            .Select(i => i.Value ?? i.Key)
            .ToArray() ?? Array.Empty<string>();

    public string this[string listKey, string itemKey]
        => GetListItem(listKey, itemKey);

    private string GetListItem(string listKey, string itemKey) {
        var list = _handler.Get(listKey);
        var item = list?.Items.FirstOrDefault(i => i.Key == itemKey);
        return item?.Value ?? itemKey;
    }
}
