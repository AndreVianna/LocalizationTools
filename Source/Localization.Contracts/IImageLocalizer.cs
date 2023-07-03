namespace Localization.Contracts;

public interface IImageLocalizer : ILocalizer {
    Stream? this[string name] { get; }
}
