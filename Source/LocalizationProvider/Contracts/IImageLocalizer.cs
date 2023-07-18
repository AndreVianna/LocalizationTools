namespace LocalizationProvider.Contracts;

public interface IImageLocalizer : ILocalizer {
    byte[]? this[string imageKey] { get; }
}
