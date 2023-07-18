namespace LocalizationProvider.Contracts;

public interface IImageRepository {
    LocalizedImage? FindImageByKey(string imageKey);
    void AddOrUpdateImage(LocalizedImage input);
}