namespace LocalizationProvider;

public enum DateTimeType
{
    Custom,
    PreciseDateTime, // e.g. 1998-02-29 21:45:37.123456
    LongDateTime, // e.g. 98-02-29 9:45:37 pm
    ShortDateTime, // e.g. feb 2nd 21:45
    LongDate, // e.g. 1998.02.29
    ShortDate, // e.g.29/2
    LongTime, // e.g. 9:45:37 pm
    ShortTime, // e.g. 9:45 pm
}
