namespace LocalizationProvider.Contracts;

public interface IListRepository {
    LocalizedList? FindListByKey(string listKey);
    void AddOrUpdateList(LocalizedList input);
}