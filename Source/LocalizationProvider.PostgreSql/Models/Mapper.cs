namespace LocalizationProvider.PostgreSql.Models;

internal static class Mapper {
    public static TOutput MapTo<TInput, TOutput>(this TInput input)
        where TInput : Resource
        where TOutput : class
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
        => input switch {
            Text r => (r.MapTo() as TOutput)!,
            List r => (r.MapTo() as TOutput)!,
            Image r => (r.MapTo() as TOutput)!,
        };
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

    private static LocalizedText MapTo(this Text input)
        => new(input.Key, input.Value);

    private static LocalizedList MapTo(this List input)
        => new(input.MapToLabel(), input.MapToItems());

    private static LocalizedText MapToLabel(this List input)
        => new(input.Key, input.Label);

    private static LocalizedText[] MapToItems(this List input)
        => input.Items.Select(MapToText).ToArray();

    private static LocalizedText MapToText(this ListItem input)
        => new(input.Key, input.Value);

    private static LocalizedImage MapTo(this Image input)
        => new(input.MapToLabel(), input.Bytes);

    private static LocalizedText MapToLabel(this Image input)
        => new(input.Key, input.Label);
}
