namespace LocalizationProvider;

internal sealed class TextResourceHandler
    : ResourceHandler<TextResourceHandler>
    , ITextResourceHandler {
    internal TextResourceHandler(IResourceRepository repository, ILogger<TextResourceHandler> logger)
        : base(repository, logger) { }

    public LocalizedText? Get(string textKey)
        => GetResourceOrDefault(textKey, rdr => rdr.FindTextByKey(textKey));

    public void Set(LocalizedText resource)
        => SetResource(resource, wtr => wtr.AddOrUpdateText(resource));
}
