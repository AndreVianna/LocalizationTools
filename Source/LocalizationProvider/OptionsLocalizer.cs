namespace LocalizationProvider;

public sealed class OptionsLocalizer : IOptionsLocalizer
{
    private readonly ILocalizedResourceProvider _provider;
    private readonly string _culture;

    public OptionsLocalizer(ILocalizedResourceProvider provider, string culture)
    {
        _provider = provider;
        _culture = culture;
    }

    public string? this[string category, uint index]
        => _provider.GetLocalizedOptionOrDefault(_culture, category, index);

    public string[] this[string category]
        => _provider.GetLocalizedOptions(_culture, category);

    public string[] GetCategories() => throw new NotImplementedException();
}

