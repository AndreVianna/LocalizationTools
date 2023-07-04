namespace LocalizationManager.Contracts;

public interface ILocalizerFactory
{
    ITextLocalizer CreateTextLocalizer(string culture);
    IListLocalizer CreateListLocalizer(string culture);
    IImageLocalizer CreateImageLocalizer(string culture);
}
