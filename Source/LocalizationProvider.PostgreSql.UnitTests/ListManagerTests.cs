namespace LocalizationProvider.PostgreSql;

public sealed partial class PostgreSqlResourceRepositoryTests {
    [Fact]
    public void FindListByKey_ReturnsNull_WhenListNotFound() {
        // Act
        var result = _repository.FindListByKey("SomeList");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void FindListByKey_ReturnsListItems_WhenListExists() {
        // Arrange
        SeedList("list_key", 2);

        // Act
        var result = _repository.FindListByKey("list_key");

        // Assert
        var subject = result.Should().BeOfType<LocalizedList>().Subject;
        subject.Items.Should().HaveCount(2);
    }

    [Fact]
    public void AddOrUpdateList_AddsLocalizedList() {
        const string key = "newList_key";
        var input = new LocalizedList(key,
                                      new[] {
                                          new LocalizedText("item1_key", "Item 1"),
                                          new LocalizedText("item2_key", "Item 2"),
                                      });
        // Act
        _repository.AddOrUpdateList(input);

        // Assert
        _repository.FindListByKey(key).Should().NotBeNull();
    }

    [Fact]
    public void AddOrUpdateList_WhenListExists_UpdatesListValue() {
        // Arrange
        const string key = "oldList_key";
        SeedList(key, 2);
        SeedText("item3_key", "Item 3");
        var input = new LocalizedList(key, new[] {
            new LocalizedText("item1_key", "Item 1"),
            new LocalizedText("item2_key", "Item 20"),
            new LocalizedText("item3_key", "Item 3"),
            new LocalizedText("item4_key", "Item 4")
        });

        // Act
        _repository.AddOrUpdateList(input);

        // Assert
        var result = _repository.FindListByKey(key);
        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(4);
    }

    private void SeedList(string key, int itemCount) {
        _dbContext.Lists
                  .Add(new() {
                      Key = key,
                      ApplicationId = _application.Id,
                      Culture = _application.DefaultCulture,
                      Items = Enumerable
                              .Range(0, itemCount)
                              .Select(i => new ListItem {
                                  Index = i,
                                  Text = new() {
                                      ApplicationId = _application.Id,
                                      Culture = _application.DefaultCulture,
                                      Key = $"item{i + 1}_key",
                                      Value = $"Item {i + 1}"
                                  },
                              }).ToArray(),
                  });
        _dbContext.SaveChanges();
    }
}
