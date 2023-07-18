namespace LocalizationProvider.Contracts;

public interface ILocalizerFactory {
    TLocalizer Create<TLocalizer>(string culture)
        where TLocalizer : class, ITypedLocalizer;
}
