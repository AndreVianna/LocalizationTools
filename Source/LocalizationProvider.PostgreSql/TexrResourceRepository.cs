namespace LocalizationProvider.PostgreSql;

public sealed partial class PostgreSqlLocalizationRepository {
    public LocalizedText? FindText(string textKey)
        => GetOrDefault<Text, LocalizedText>(textKey);

    public void SetText(LocalizedText input)
        => AddOrUpdate<Text, LocalizedText>(input);
    private Text GetUpdatedText(LocalizedText input) {
        var text = LoadForUpdate<Text>(input.Key);
        if (text is not null) {
            if (text.Value == input.Value) {
                return text;
            }

            text.Update(input, null!);
            return text;
        }

        text = input.Map<LocalizedText, Text>(_application.Id, _culture, null!);
        _dbContext.Texts.Add(text);
        return text;
    }
}
