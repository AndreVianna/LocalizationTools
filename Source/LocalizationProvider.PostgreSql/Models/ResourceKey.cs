namespace LocalizationProvider.PostgreSql.Models;

public record struct ResourceKey(Guid ApplicationId, string Culture, string ResourceId, uint? Index = null);
