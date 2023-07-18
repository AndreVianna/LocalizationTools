using static LocalizationProvider.Models.ResourceType;

namespace LocalizationProvider;

internal class ImageResourceHandler
    : ResourceHandler<ImageResourceHandler>
    , IImageResourceHandler {
    internal ImageResourceHandler(IResourceRepository repository, ILogger<ImageResourceHandler> logger)
        : base(repository, logger) { }

    public LocalizedImage? Get(string imageKey)
        => GetResourceOrDefault(imageKey, rdr => rdr.FindImageByKey(imageKey));

    public void Set(LocalizedImage resource)
        => SetResource(resource, wtr => wtr.AddOrUpdateImage(resource));
}
