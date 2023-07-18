namespace LocalizationProvider;

internal sealed class ListResourceHandler
    : ResourceHandler<ListResourceHandler>
    , IListResourceHandler {
    internal ListResourceHandler(IResourceRepository repository, ILogger<ListResourceHandler> logger)
        : base(repository, logger) { }

    public LocalizedList? Get(string lisKey)
        => GetResourceOrDefault(lisKey, rdr => rdr.FindListByKey(lisKey));
}
