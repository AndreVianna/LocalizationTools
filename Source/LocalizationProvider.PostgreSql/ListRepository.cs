namespace LocalizationProvider.PostgreSql;

internal partial class PostgreSqlLocalizationRepository {
    public LocalizedList? FindListByKey(string listKey)
        => GetOrDefault<List, LocalizedList>(listKey);

    public void AddOrUpdateList(LocalizedList input)
        => AddOrUpdate<List, LocalizedList>(input);
}
