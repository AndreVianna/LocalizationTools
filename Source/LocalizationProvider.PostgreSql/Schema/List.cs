﻿namespace LocalizationProvider.PostgreSql.Schema;

public class List : Resource {
    public IList<ListItem> Items { get; set; } = new List<ListItem>();
}
