namespace LocalizationManager.Contracts;

public static class Keys
{
    public const string NumberPatternKey = $"{nameof(Keys)}.{nameof(NumberPatternKey)}";
    public const string AllListsKey = $"{nameof(Keys)}.{nameof(AllListsKey)}";
    public static string GetDateTimeFormatKey(DateTimeFormat format) => $"{nameof(Keys)}.{format}Key";
}
