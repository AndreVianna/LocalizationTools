using DomainApplication = LocalizationProvider.Contracts.Application;

namespace LocalizationProvider.PostgreSql;

internal partial class PostgreSqlLocalizationRepository {
    public DomainApplication? FindApplicationById(Guid id)
        => _dbContext.Applications
                     .AsNoTracking()
                     .FirstOrDefault(i => i.Id == id)?
                     .MapTo();

    public IEnumerable<DomainApplication> ListApplications()
        => _dbContext.Applications
                     .AsNoTracking()
                     .Select(i => i.MapTo())
                     .ToArray();

    public bool AddApplication(DomainApplication application) {
        var entity = _dbContext.Applications
                               .FirstOrDefault(i => i.Id == application.Id);
        if (entity != null) return false;
        entity = application.MapTo();
        _dbContext.Applications.Add(entity);
        _dbContext.SaveChanges();
        application.UpdateFrom(entity);
        return true;
    }

    public bool UpdateApplication(DomainApplication application) {
        var entity = _dbContext.Applications
                  .FirstOrDefault(i => i.Id == application.Id);
        if (entity == null) return false;
        entity.UpdateFrom(application);
        _dbContext.SaveChanges();
        application.UpdateFrom(entity);
        return true;
    }

    public void RemoveApplication(Guid id) {
        var entity = _dbContext.Applications
            .FirstOrDefault(i => i.Id == id);
        if (entity == null) return;
        _dbContext.Applications.Remove(entity);
        _dbContext.SaveChanges();
    }
}
