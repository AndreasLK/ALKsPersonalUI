using ALKsPersonalUI.Data;
using Bunit;
using Xunit;

namespace ALKsPersonalUI.Tests.Components;

public class LeaderboardTests : BunitContext
{
    private sealed record PlayerScore(string Name, int Score, int Age);

    private static readonly List<Leaderboard<PlayerScore>.Column> DefaultColumns =
    [
        new("Player", item => item.Name),
        new("Score", item => item.Score),
        new("Age", item => item.Age)
    ];

    [Fact]
    public void RendersTopEntriesAndHighlightedEntryOutsideTop()
    {
        var entries = Enumerable.Range(1, 150)
            .Select(i => new PlayerScore($"Player {i}", 10000 - i * 10, 20 + (i % 30)))
            .ToList();
        var newEntry = entries[146]; // 147th entry

        var cut = Render<Leaderboard<PlayerScore>>(parameters => parameters
            .Add(p => p.Items, entries)
            .Add(p => p.Columns, DefaultColumns)
            .Add(p => p.TopCount, 3)
            .Add(p => p.HighlightedItem, newEntry));

        var rows = cut.FindAll("tbody tr");
        // 3 top rows + 1 separator row ("...") + 1 highlighted row (147.) = 5 rows
        Assert.Equal(5, rows.Count);
        Assert.Contains("1.", rows[0].TextContent);
        Assert.Contains("Player 1", rows[0].TextContent);
        Assert.Contains("...", rows[3].TextContent);
        Assert.Contains("147.", rows[4].TextContent);
        Assert.Contains("Player 147", rows[4].TextContent);
        Assert.Contains("leaderboard-row-highlighted", rows[4].ClassName);
    }

    [Fact]
    public void DoesNotRenderSeparatorWhenHighlightedEntryIsInsideTop()
    {
        var entries = new List<PlayerScore>
        {
            new("Alice", 500, 25),
            new("Bob", 400, 30),
            new("Charlie", 300, 22)
        };

        var cut = Render<Leaderboard<PlayerScore>>(parameters => parameters
            .Add(p => p.Items, entries)
            .Add(p => p.Columns, DefaultColumns)
            .Add(p => p.TopCount, 3)
            .Add(p => p.HighlightedItem, entries[1]));

        var rows = cut.FindAll("tbody tr");
        Assert.Equal(3, rows.Count);
        Assert.Empty(cut.FindAll(".leaderboard-separator-row"));
        Assert.Contains("leaderboard-row-highlighted", rows[1].ClassName);
    }
}

