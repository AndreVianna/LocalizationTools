namespace LocalizationProvider.Contracts;

public interface IApplicationRepository {
    Application? FindApplicationById(Guid id);
    IEnumerable<Application> ListApplications();
    bool AddApplication(Application application);
    bool UpdateApplication(Application application);
    void RemoveApplication(Guid id);
}
