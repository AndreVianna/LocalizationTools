namespace LocalizationProvider.Contracts;

public interface ITypedLocalizer : ILocalizer {
    static abstract ResourceType Type { get; }
}
