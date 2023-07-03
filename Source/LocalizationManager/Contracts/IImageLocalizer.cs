namespace LocalizationManager.Contracts;

public interface IImageLocalizer : ILocalizer {
    byte[]? this[string name] { get; }
    Stream? GetAsStream(string name);
}
