namespace LocalizationProvider.Contracts;

public enum DateTimeFormat {
    DefaultDateTimePattern, // e.g. 1998-02-29 21:45:37.123456
    LongDateTimePattern, // e.g. 98-02-29 9:45:37 pm
    ShortDateTimePattern, // e.g. feb 2nd 21:45
    LongDatePattern, // e.g. 1998.02.29
    ShortDatePattern, // e.g.29/2
    LongTimePattern, // e.g. 9:45:37 pm
    ShortTimePattern, // e.g. 9:45 pm
}
