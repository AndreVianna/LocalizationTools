namespace LocalizationManager.PostgreSql;

internal sealed class LocalizationProvider : ILocalizationProvider, IDisposable {
    private readonly ILocalizationManager _handler;

    private LocalizationProvider(Guid applicationId, IServiceProvider serviceProvider) {
        _handler = LocalizationManager.CreateFor(applicationId, serviceProvider);
    }

    private bool _isDisposed;
    public void Dispose() {
        if (_isDisposed) return;
        ((IDisposable)_handler).Dispose();
        _isDisposed = true;
    }

    public static ILocalizationProvider CreateFor(Guid applicationId, IServiceProvider serviceProvider)
        => new LocalizationProvider(applicationId, serviceProvider);

    public ILocalizationReader For(string culture) => _handler.For(culture);

    public string GetDateTimeFormat(DateTimeFormat format)
        => _handler.GetDateTimeFormat(format);

    public string GetNumberFormat(int decimalPlaces = 0, int integerDigits = 1)
        => _handler.GetNumberFormat(decimalPlaces, integerDigits);

    public byte[]? GetImageOrDefault(string imageKey)
        => _handler.GetImageOrDefault(imageKey);

    public string[] GetLists()
        => _handler.GetLists();

    public string[]? GetListItemsOrDefault(string listKey)
        => _handler.GetListItemsOrDefault(listKey);

    public string? GetListItemOrDefault(string listKey, uint index)
        => _handler.GetListItemOrDefault(listKey, index);

    public string? GetTextOrDefault(string textKey)
        => _handler.GetTextOrDefault(textKey);
}
