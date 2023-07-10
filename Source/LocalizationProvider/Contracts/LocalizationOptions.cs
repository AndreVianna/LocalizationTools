namespace LocalizationProvider.Contracts;

public record LocalizationOptions {
    [Required]
    public required Guid ApplicationId { get; init; }
};
