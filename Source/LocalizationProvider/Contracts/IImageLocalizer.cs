namespace LocalizationProvider.Contracts;

public interface IImageLocalizer : ILocalizer {
    Stream? this[string name] { get; }
}
