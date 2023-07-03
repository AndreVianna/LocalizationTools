namespace LocalizationManager.PostgreSql;

public sealed class ResourceWriter : IResourceWriter, IDisposable {
    private readonly Application _application;
    private string _culture;

    private readonly ResourceDbContext _dbContext;
    private static readonly ConcurrentDictionary<Guid, Application> _applications = new();
    private bool _isDisposed;

    private ResourceWriter(Guid applicationId, IServiceProvider serviceProvider) {
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
        => new ResourceWriter(applicationId, serviceProvider);

    public IResourceWriter For(string culture) {
        if (!_application.AvailableCultures.Split(',').Contains(culture)) {
            throw new InvalidOperationException($"Culture '{culture}' is not available for application '{_application.Name}'.");
        }

        _culture = culture;
        return this;
    }

    public void SetText(LocalizedText input) {
        AddOrUpdateText(input);
        _dbContext.SaveChanges();
    }

    public void SetList(LocalizedList input) {
        AddOrUpdateList(input);
        _dbContext.SaveChanges();
    }

    public void SetImage(LocalizedImage input) {
        AddOrUpdateImage(input);
        _dbContext.SaveChanges();
    }

    public void SetDateTimeFormat(LocalizedDateTimeFormat format) {
        AddOrUpdateText(new LocalizedText($"{nameof(DateTimeFormat)}.{format.Key}", format.Pattern));
        _dbContext.SaveChanges();
    }

    private void AddOrUpdateText(LocalizedText input) {
        var text = _dbContext
                  .Texts
                  .FirstOrDefault(t => t.ApplicationId == _application.Id
                                    && t.Culture == _culture
                                    && t.Key == input.Key);
        if (text is null) {
            text = new Text {
                ApplicationId = _application.Id,
                Culture = _culture,
                Key = input.Key,
                Value = input.Value
            };
            _dbContext.Texts.Add(text);
        }
        else {
            text.Value = input.Value;
        }
    }

    private void AddOrUpdateList(LocalizedList input) {
        var list = _dbContext
                  .Lists
                  .Include(i => i.Items)
                  .FirstOrDefault(t => t.ApplicationId == _application.Id
                                    && t.Culture == _culture
                                    && t.Key == input.Key);
        if (list is null) {
            list = new List {
                ApplicationId = _application.Id,
                Culture = _culture,
                Key = input.Key,
                Name = input.Name
            };
            _dbContext.Lists.Add(list);
        }
        else {
            list.Name = input.Name;
        }

        list.Items.Clear();
        for (var i = 0; i < input.Items.Length; i++) {
            list.Items.Add(new ListItem {
                ListId = list.Id,
                List = list,
                Index = i,
                Value = input.Items[i],
            });
        }
    }

    private void AddOrUpdateImage(LocalizedImage input) {
        var image = _dbContext
                  .Images
                  .FirstOrDefault(t => t.ApplicationId == _application.Id
                                    && t.Culture == _culture
                                    && t.Key == input.Key);
        if (image is null) {
            image = new Image {
                ApplicationId = _application.Id,
                Culture = _culture,
                Key = input.Key,
                Label = input.Label,
                Bytes = input.Bytes
            };
            _dbContext.Images.Add(image);
        }
        else {
            image.Label = input.Label;
            image.Bytes = input.Bytes;
        }
    }
}
