namespace LocalizationProvider.PostgreSql.Models;

public record struct ResourceKey(string ApplicationId, string Culture, string ResourceId, uint? Index = null);
