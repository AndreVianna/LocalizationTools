namespace LocalizationProvider.Contracts;

public interface IOptionsLocalizer : ILocalizer {
    string[] this[string category] { get; }
    string? this[string category, uint index] { get; }
    string[] GetCategories();
}
