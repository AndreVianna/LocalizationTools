namespace Localization.PostgreSql.Models;

internal record struct ResourceKey(Guid ApplicationId, string Culture, string ResourceId, uint? Index = null);
