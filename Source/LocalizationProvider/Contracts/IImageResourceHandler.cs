namespace LocalizationProvider.Contracts;

public interface IImageResourceHandler {
    LocalizedImage? GetLocalizedImage(string imageKey);
}
