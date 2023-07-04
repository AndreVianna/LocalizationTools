using System.Runtime.Serialization;

using DateTimeFormat = System.Runtime.Serialization.DateTimeFormat;

namespace LocalizationManager.Models;

public record struct LocalizedDateTimeFormat(DateTimeFormat Key, string Pattern);