namespace LocalizationProvider.PostgreSql;

public sealed partial class PostgreSqlLocalizationProvider {
    public LocalizedList? FindList(string listKey)
        => GetOrDefault<List, LocalizedList>(listKey);

    public void SetList(LocalizedList input)
        => AddOrUpdate<List, LocalizedList>(input);
}
