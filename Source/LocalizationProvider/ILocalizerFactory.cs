namespace LocalizationProvider;

public interface ILocalizerFactory
{
    IStringLocalizer CreateStringLocalizer(string culture);
    IOptionsLocalizer CreateOptionsLocalizer(string culture);
    IImageLocalizer CreateImageLocalizer(string culture);
}
