namespace LocalizationProvider;

public interface IImageLocalizer : ILocalizer
{
    Stream? this[string name] { get; }
}
