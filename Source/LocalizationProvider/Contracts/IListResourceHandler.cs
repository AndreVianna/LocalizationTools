namespace LocalizationProvider.Contracts;

public interface IListResourceHandler {
    LocalizedList? GetLocalizedList(string listKey);
}
