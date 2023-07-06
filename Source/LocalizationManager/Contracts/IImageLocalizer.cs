namespace LocalizationManager.Contracts;

public interface IImageLocalizer : ILocalizer {
    byte[]? this[string imageId] { get; }
}
