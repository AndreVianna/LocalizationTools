namespace LocalizationProvider.Contracts;

public interface IImageLocalizer : ITypedLocalizer {
    byte[]? this[string imageKey] { get; }
}
