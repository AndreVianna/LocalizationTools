using System.Runtime.Serialization;

namespace LocalizationManager.Models;

public record struct LocalizedDateTimeFormat(DateTimeFormat Key, string Pattern);