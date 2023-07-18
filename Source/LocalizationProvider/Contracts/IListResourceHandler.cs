namespace LocalizationProvider.Contracts;

public interface IListResourceHandler {
    LocalizedList? Get(string listKey);
}
