namespace LocalizationManager.Contracts;

public interface ILocalizerFactory {
    ITextLocalizer CreateStringLocalizer(string culture);
    IListLocalizer CreateListLocalizer(string culture);
    IImageLocalizer CreateImageLocalizer(string culture);
}
