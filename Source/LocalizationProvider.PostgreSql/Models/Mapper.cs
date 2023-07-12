namespace LocalizationProvider.PostgreSql.Models;

internal static class Mapper {
    public static TDomainModel MapTo<TEntity, TDomainModel>(this TEntity input)
        where TDomainModel : class
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
        => input switch {
            Text r => (r.MapTo() as TDomainModel)!,
            List r => (r.MapTo() as TDomainModel)!,
            Image r => (r.MapTo() as TDomainModel)!,
        };
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

    public static TEntity MapTo<TDomainModel, TEntity>(this TDomainModel input, Guid applicationId, string culture, Func<LocalizedText, Text> getOrAddText)
        where TEntity : class
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
        => input switch {
            LocalizedText r => (r.MapTo(applicationId, culture) as TEntity)!,
            LocalizedList r => (r.MapTo(applicationId, culture, getOrAddText) as TEntity)!,
            LocalizedImage r => (r.MapTo(applicationId, culture) as TEntity)!,
        };
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

    public static void UpdateFrom<TEntity, TDomainModel>(this TEntity target, TDomainModel input, Func<LocalizedText, Text> getOrAddText)
        where TEntity : Resource
        where TDomainModel : ILocalizedResource {

        switch (target) {
            case Text r when input is LocalizedText lti:
                r.UpdateFrom(lti);
                return;
            case List r when input is LocalizedList lli:
                r.UpdateFrom(lli, getOrAddText);
                return;
            case Image r when input is LocalizedImage lii:
                r.UpdateFrom(lii);
                return;
        }
    }

    private static LocalizedText MapTo(this Text input)
        => new(input.Key, input.Value);

    private static LocalizedList MapTo(this List input)
        => new(input.Key, input.MapToItems());

    private static LocalizedText[] MapToItems(this List input)
        => input.Items.Select(i => i.Text!.MapTo()).ToArray();

    private static LocalizedImage MapTo(this Image input)
        => new(input.Key, input.Bytes);

    private static Text MapTo(this LocalizedText input, Guid applicationId, string culture)
        => new() {
            ApplicationId = applicationId,
            Culture = culture,
            Key = input.Key,
            Value = input.Value,
        };

    private static List MapTo(this LocalizedList input, Guid applicationId, string culture, Func<LocalizedText, Text> getOrAddText) {
        var list = new List {
            ApplicationId = applicationId,
            Culture = culture,
            Key = input.Key,
        };
        list.Items = input.MapToListItems(list, getOrAddText);
        return list;
    }

    private static Image MapTo(this LocalizedImage input, Guid applicationId, string culture)
        => new() {
            ApplicationId = applicationId,
            Culture = culture,
            Key = input.Key,
            Bytes = input.Bytes,
        };

    private static List<ListItem> MapToListItems(this LocalizedList input, List list, Func<LocalizedText, Text> getOrAddText)
        => input.Items
                .Select(getOrAddText)
                .Select((item, index) => item.MapTo(list, index))
                .ToList();

    private static void UpdateFrom(this Text text, LocalizedText input)
        => text.Value = input.Value;

    private static void UpdateFrom(this List list, LocalizedList input, Func<LocalizedText, Text> getOrAddText) {
        list.Items.Clear();
        for (var index = 0; index < input.Items.Length; index++) {
            var item = getOrAddText(input.Items[index]);
            list.Items.Add(item.MapTo(list, index));
        }
    }

    private static ListItem MapTo(this Text input, List list, int index)
        => new() {
            ListId = list.Id,
            List = list.Id == 0 ? list : null,
            Index = index,
            TextId = input.Id,
        };

    private static void UpdateFrom(this Image image, LocalizedImage input)
        => image.Bytes = input.Bytes;
}
