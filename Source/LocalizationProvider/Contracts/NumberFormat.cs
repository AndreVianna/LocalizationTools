namespace LocalizationManager.Contracts;

public enum NumberFormat {
    DefaultNumberPattern, // e.g.-12.345
    CurrencyPattern, // e.g.-$12.34
    PercentPattern, // e.g.1234.5% Important: The original value is multiplied by 100.
    ExponentialPattern, // e.g.-1.2345e+002
}
