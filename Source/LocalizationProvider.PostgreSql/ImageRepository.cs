namespace LocalizationProvider.PostgreSql;

internal partial class PostgreSqlLocalizationRepository {
    public LocalizedImage? FindImageByKey(string imageKey)
        => GetOrDefault<Image, LocalizedImage>(imageKey);

    public void AddOrUpdateImage(LocalizedImage input)
        => AddOrUpdate<Image, LocalizedImage>(input);
}
