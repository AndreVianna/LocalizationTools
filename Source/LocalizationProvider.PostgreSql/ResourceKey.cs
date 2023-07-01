namespace LocalizationProvider.PostgreSql;

public record struct ResourceKey(string ApplicationId, string Culture, string ResourceId);
