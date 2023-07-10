﻿namespace LocalizationProvider.Contracts;

public static class Keys {
    public const string ListLabel = "(List.Label)";

    public const string AllListsKey = $"{nameof(Keys)}.{nameof(AllListsKey)}";

    public static string GetDateTimeFormatKey(DateTimeFormat dateTimeFormat) {
        var dtf = CultureInfo.InvariantCulture.DateTimeFormat;
        return dateTimeFormat switch {
            LongDateTimePattern => $"{dtf.LongDatePattern} {dtf.LongTimePattern}",
            ShortDateTimePattern => $"{dtf.ShortDatePattern} {dtf.ShortTimePattern}",
            LongDatePattern => dtf.LongDatePattern,
            ShortDatePattern => dtf.ShortDatePattern,
            LongTimePattern => dtf.LongTimePattern,
            ShortTimePattern => dtf.ShortTimePattern,
            _ => dtf.SortableDateTimePattern,
        };
    }

    public static string GetNumberFormatKey(NumberFormat numberFormat, int decimalPlaces = 2)
        => numberFormat switch {
            CurrencyPattern => $"c{decimalPlaces}",
            PercentPattern => $"p{decimalPlaces}",
            ExponentialPattern => $"e{decimalPlaces}",
            _ => $"n{decimalPlaces}",
        };
}
