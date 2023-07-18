namespace LocalizationProvider.PostgreSql;

public sealed partial class PostgreSqlLocalizationRepository {
    public LocalizedImage? FindImage(string imageKey)
        => GetOrDefault<Image, LocalizedImage>(imageKey);

    public void SetImage(LocalizedImage input)
        => AddOrUpdate<Image, LocalizedImage>(input);
}
