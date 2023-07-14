using DomainApplication = LocalizationProvider.Contracts.Application;
using Application = LocalizationProvider.PostgreSql.Schema.Application;

namespace LocalizationProvider.PostgreSql;

public sealed partial class PostgreSqlLocalizationProvider {
    public DomainApplication? FindApplicationById(Guid id)
        => _dbContext.Applications
                     .AsNoTracking()
                     .FirstOrDefault(i => i.Id == id)?
                     .MapTo<Application, DomainApplication>();
}
