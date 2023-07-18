namespace LocalizationProvider.Contracts;

public interface IImageResourceHandler {
    LocalizedImage? Get(string imageKey);
}
