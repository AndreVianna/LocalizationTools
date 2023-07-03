using Localization.PostgreSql.Models;

namespace Localization.PostgreSql;

public sealed class DatabaseLocalizationManager : IResourceWriter, IDisposable {
    private readonly Application _application;
    private string _culture;

    private readonly ResourceDbContext _dbContext;
    private static readonly ConcurrentDictionary<Guid, Application> _applications = new();
    private bool _isDisposed;

    private DatabaseLocalizationManager(Guid applicationId, IServiceProvider serviceProvider) {
        _dbContext = serviceProvider.GetRequiredService<ResourceDbContext>();
        _application = _applications.GetOrAdd(applicationId, id
            => _dbContext.Applications.FirstOrDefault(a => a.Id == id)
            ?? throw new InvalidOperationException($"Application with id '{id}' not found."));
        _culture = _application.DefaultCulture;
    }

    public void Dispose() {
        if (_isDisposed) return;
        _dbContext.Dispose();
        _isDisposed = true;
    }

    public static IResourceWriter CreateFor(Guid applicationId, IServiceProvider serviceProvider)
        => new DatabaseLocalizationManager(applicationId, serviceProvider);

    public IResourceWriter For(string culture) {
        if (!_application.AvailableCultures.Split(',').Contains(culture)) {
            throw new InvalidOperationException($"Culture '{culture}' is not available for application '{_application.Name}'.");
        }

        _culture = culture;
        return this;
    }

    public void SetText(string key, string value) {
        var text = _dbContext
                  .Texts
                  .FirstOrDefault(t => t.ApplicationId == _application.Id
                                    && t.Culture == _culture
                                    && t.Key == key)
                ?? new Text {
                       ApplicationId = _application.Id,
                       Culture = _culture,
                       Key = key,
                       Value = value
                   };
            _dbContext.Texts.Add(text);
        text.Value = value;

        _dbContext.SaveChanges();
    }

    public void SetListItems(string listKey, (string key, string value)[] items) {
        var list = _dbContext
                  .Lists
                  .Include(l => l.ListItems)
                  .FirstOrDefault(l => l.ApplicationId == _application.Id
                                    && l.Culture == _culture
                                    && l.Key == listKey)
                ?? throw new InvalidOperationException($"List '{listKey}' not found.");

        list.ListItems.Clear();
        for (var i = 0; i < items.Length; i++) {
            var (key, value) = items[i];
            var text = _dbContext.Texts
                                 .FirstOrDefault(l => l.ApplicationId == _application.Id
                                                   && l.Culture == _culture
                                                   && l.Key == key)
                    ?? new Text {
                           ApplicationId = _application.Id,
                           Culture = _culture,
                           Key = key,
                           Value = value,
                       };
            list.ListItems.Add(new ListItem {
                Index = i,
                ListId = list.Id,
                Item = text,
            });
        }

        _dbContext.SaveChanges();
    }

    public void SetListItem(string listKey, uint index, string key, string value) => throw new NotImplementedException();
    public void SetImage(string key, byte[] bytes) => throw new NotImplementedException();
    public void SetDateTimeFormat(DateTimeFormat format, string pattern) => throw new NotImplementedException();
}
