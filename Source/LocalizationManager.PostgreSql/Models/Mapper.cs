namespace LocalizationManager.PostgreSql.Models;

internal static class Mapper {
    public static LocalizedImage MapToLocalizedImage(this Image input)
        => new(input.MapToLabel(), input.Bytes);

    public static LocalizedList MapToLocalizedList(this List input)
        => new(input.MapToLabel(), input.MapToItems());

    public static LocalizedText MapToLocalizedText(this Text input)
        => new(input.Key, input.Value);

    public static LocalizedText MapToLabel(this Image input)
        => new(input.Key, input.Label);

    public static LocalizedText MapToLabel(this List input)
        => new(input.Key, input.Label);

    public static LocalizedText[] MapToItems(this List input)
        => input.Items.Select(MapToListItem).ToArray();

    public static LocalizedText MapToListItem(this ListItem input)
        => new(input.Key, input.Value);
}
