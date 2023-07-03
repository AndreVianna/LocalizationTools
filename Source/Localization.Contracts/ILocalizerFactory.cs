namespace Localization.Contracts;

public interface ILocalizerFactory {
    ITextLocalizer CreateStringLocalizer(string culture);
    IOptionsLocalizer CreateOptionsLocalizer(string culture);
    IImageLocalizer CreateImageLocalizer(string culture);
}
