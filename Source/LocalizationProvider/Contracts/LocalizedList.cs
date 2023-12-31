﻿namespace LocalizationProvider.Contracts;

public record LocalizedList(string Key, LocalizedText[] Items)
    : ILocalizedResource<LocalizedList> {
}
