using Application = LocalizationProvider.PostgreSql.Schema.Application;
using DomainApplication = LocalizationProvider.Contracts.Application;

namespace LocalizationProvider.PostgreSql.Models;

internal static class Mapper {
    public static TDomainModel Map<TEntity, TDomainModel>(this TEntity input)
        where TEntity : Resource
        where TDomainModel : class, ILocalizedResource
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
        => input switch {
            Text r => (r.MapTo() as TDomainModel)!,
            List r => (r.MapTo() as TDomainModel)!,
            Image r => (r.MapTo() as TDomainModel)!,
        };
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

    public static TEntity Map<TDomainModel, TEntity>(this TDomainModel input, Guid applicationId, string culture, Func<LocalizedText, Text> getOrAddText)
        where TDomainModel : class, ILocalizedResource
        where TEntity : Resource
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
        => input switch {
            LocalizedText r => (r.MapTo(applicationId, culture) as TEntity)!,
            LocalizedList r => (r.MapTo(applicationId, culture, getOrAddText) as TEntity)!,
            LocalizedImage r => (r.MapTo(applicationId, culture) as TEntity)!,
        };
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

    public static void Update<TEntity, TDomainModel>(this TEntity target, TDomainModel input, Func<LocalizedText, Text> getOrAddText)
        where TEntity : Resource
        where TDomainModel : class, ILocalizedResource {
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

    public static DomainApplication MapTo(this Application input)
        => new() {
            Id = input.Id,
            Name = input.Name,
            DefaultCulture = input.DefaultCulture,
            AvailableCultures = input.AvailableCultures,
            Texts = input.Texts.Select(MapTo).ToHashSet(),
            Lists = input.Lists.Select(MapTo).ToHashSet(),
            Images = input.Images.Select(MapTo).ToHashSet(),
        };

    public static Application MapTo(this DomainApplication input)
        => new() {
            Id = input.Id,
            Name = input.Name,
            DefaultCulture = input.DefaultCulture,
            AvailableCultures = input.AvailableCultures,
        };

    public static void UpdateFrom(this Application target, DomainApplication input) {
        target.Id = input.Id;
        target.Name = input.Name;
        target.DefaultCulture = input.DefaultCulture;
        target.AvailableCultures = input.AvailableCultures;
    }

    public static void UpdateFrom(this DomainApplication target, Application input) {
        target.Id = input.Id;
        target.Name = input.Name;
        target.DefaultCulture = input.DefaultCulture;
        target.AvailableCultures = input.AvailableCultures;
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
