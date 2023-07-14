using DomainApplication = LocalizationProvider.Contracts.Application;
using Application = LocalizationProvider.PostgreSql.Schema.Application;

namespace LocalizationProvider.PostgreSql.Models;

internal static class Mapper {
    public static TDomainModel MapTo<TEntity, TDomainModel>(this TEntity input)
        where TDomainModel : class
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
        => input switch {
            Application r => (r.MapTo() as TDomainModel)!,
            Text r => (r.MapTo() as TDomainModel)!,
            List r => (r.MapTo() as TDomainModel)!,
            Image r => (r.MapTo() as TDomainModel)!,
        };
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

    public static TEntity MapTo<TDomainModel, TEntity>(this TDomainModel input, Guid applicationId, string culture, Func<LocalizedText, Text> getOrAddText)
        where TEntity : class
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
        => input switch {
            DomainApplication r => (r.MapTo() as TEntity)!,
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

    private static DomainApplication MapTo(this Application input)
        => new() {
            Id = input.Id,
            Name = input.Name,
            DefaultCulture = input.DefaultCulture,
            AvailableCultures = input.AvailableCultures,
            Texts = input.Texts.Select(MapTo).ToHashSet(),
            Lists = input.Lists.Select(MapTo).ToHashSet(),
            Images = input.Images.Select(MapTo).ToHashSet(),
        };

    private static LocalizedText MapTo(this Text input)
        => new(input.Key, input.Value);

    private static LocalizedList MapTo(this List input)
        => new(input.Key, input.MapToItems());

    private static LocalizedText[] MapToItems(this List input)
        => input.Items.Select(i => i.Text!.MapTo()).ToArray();

    private static LocalizedImage MapTo(this Image input)
        => new(input.Key, input.Bytes);

    private static Application MapTo(this DomainApplication input)
        => new() {
            Id = input.Id,
            Name = input.Name,
            DefaultCulture = input.DefaultCulture,
            AvailableCultures = input.AvailableCultures,
        };

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

    private static ICollection<ListItem> MapToListItems(this LocalizedList input, List list, Func<LocalizedText, Text> getOrAddText)
        => input.Items
                .Select(getOrAddText)
                .Select((item, index) => item.MapTo(list, index))
                .ToHashSet();

    private static void UpdateFrom(this Text text, LocalizedText input)
        => text.Value = input.Value;

    private static void UpdateFrom(this List list, LocalizedList input, Func<LocalizedText, Text> getOrAddText)
        => list.Items = input.Items
                             .Select((item, index) => getOrAddText(item).MapTo(list, index))
                             .ToHashSet();

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
