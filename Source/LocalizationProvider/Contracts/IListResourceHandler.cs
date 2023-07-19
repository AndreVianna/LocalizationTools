namespace LocalizationProvider.Contracts;

public interface IListResourceHandler {
    LocalizedList? Get(string listKey);
    void Set(LocalizedList resource);
}
