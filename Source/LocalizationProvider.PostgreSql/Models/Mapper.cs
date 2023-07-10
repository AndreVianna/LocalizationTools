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
        => new(input.Key, input.Label?.MapTo(), input.MapToItems());

    private static LocalizedText[] MapToItems(this List input)
        => input.Items.Select(i => i.Text.MapTo()).ToArray();

    private static LocalizedImage MapTo(this Image input)
        => new(input.Key, input.Label?.MapTo(), input.Bytes ?? Array.Empty<byte>());

    internal static Text MapTo(this LocalizedText input, Guid applicationId, string culture)
        => new() {
            ApplicationId = applicationId,
            Culture = culture,
            Key = input.Key,
        };

    internal static ListItem MapTo(this Text input, List list, int index)
        => new() {
            ListId = list.Id,
            List = list,
            Index = index,
            Text = input,
        };

    internal static Image MapTo(this LocalizedImage input, Guid applicationId, string culture)
        => new() {
            ApplicationId = applicationId,
            Culture = culture,
            Key = input.Key,
        };

    internal static List MapTo(this LocalizedList input, Guid applicationId, string culture)
        => new() {
            ApplicationId = applicationId,
            Culture = culture,
            Key = input.Key,
        };

    internal static void UpdateWith(this Text text, LocalizedText input)
        => text.Value = input.Value;

    internal static void UpdateWith(this List list, LocalizedList input, Func<LocalizedText?, Text?> getOrAdd) {
        var label = getOrAdd(input.Label);
        list.LabelId = label?.Id;
        list.Label = label;

        list.Items.Clear();
        for (var index = 0; index < input.Items.Length; index++) {
            var item = getOrAdd(input.Items[index])!;
            list.Items.Add(item.MapTo(list, index));
        }
    }

    internal static void UpdateWith(this Image image, LocalizedImage input, Func<LocalizedText?, Text?> getOrAdd) {
        var label = getOrAdd(input.Label);
        image.LabelId = label?.Id;
        image.Label = label;

        image.Bytes = input.Bytes;
    }
}
