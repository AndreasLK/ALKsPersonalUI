using System.Text.Json;
using ALKsPersonalUI.Json;
using Xunit;

namespace ALKsPersonalUI.Tests.Json;

/// <summary>
/// Unit tests for <see cref="JsonUiSanitizer"/>, <see cref="JsonUiSessionState"/>,
/// <see cref="JsonUiConditionEvaluator"/>, and <see cref="JsonUiThemeResolver"/>.
/// </summary>
public sealed class JsonUiRuntimeUnitTests
{
    private sealed class StubTimeProvider(DateTimeOffset initialUtc) : TimeProvider
    {
        public DateTimeOffset CurrentUtc { get; set; } = initialUtc;
        public override DateTimeOffset GetUtcNow() => CurrentUtc;
    }

    [Theory]
    [InlineData("https://example.com/museum.png", "https://example.com/museum.png")]
    [InlineData("/images/hero.webp", "/images/hero.webp")]
    [InlineData("data:image/png;base64,iVBORw0KGgo=", "data:image/png;base64,iVBORw0KGgo=")]
    [InlineData("javascript:alert(1)", "")]
    [InlineData("//evil.com/tracker.png", "")]
    [InlineData("data:image/svg+xml,<svg onload=alert(1)>", "")]
    [InlineData("https://x.com/a.png');position:fixed;inset:0;background:url('", "")]
    public void SanitizeImageUrl_EnforcesAllowListAndBlocksBreakouts(string input, string expected)
    {
        var actual = JsonUiSanitizer.SanitizeImageUrl(input);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("1.5rem", true)]
    [InlineData("#10B981", true)]
    [InlineData("var(--ui-accent)", true)]
    [InlineData("10px; position: fixed; inset: 0", false)]
    [InlineData("url(https://evil.com/leak)", false)]
    [InlineData("expression(alert(1))", false)]
    public void IsSafeCssValue_BlocksStyleInjection(string value, bool expectedSafe)
    {
        Assert.Equal(expectedSafe, JsonUiSanitizer.IsSafeCssValue(value));
    }

    [Fact]
    public void SessionState_Interpolate_IsSinglePassAndInvariant()
    {
        var state = new JsonUiSessionState();
        state.SetValue("secret", "TOP_SECRET");
        state.SetValue("player", "{{secret}}");
        state.SetValue("score", 3.5);

        var rendered = state.Interpolate("Player={{player}}, Score={{score}}, Missing={{none}}");

        Assert.Equal("Player={{secret}}, Score=3.5, Missing=", rendered);
    }

    [Fact]
    public void SessionState_MarkTimeAndComputeElapsedSeconds_UsesTimeProvider()
    {
        var clock = new StubTimeProvider(new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero));
        var state = new JsonUiSessionState(clock);

        state.MarkTime("quizStart");
        clock.CurrentUtc = clock.CurrentUtc.AddSeconds(42);
        state.ComputeElapsedSeconds("quizStart", "elapsedSeconds");

        Assert.Equal(42, state.GetInt("elapsedSeconds"));
    }

    [Fact]
    public void ConditionEvaluator_HandlesComparisonsAndFailsClosed()
    {
        var state = new JsonUiSessionState();
        state.SetValue("score", 20d);
        state.SetValue("name", "Ada");

        Assert.True(JsonUiConditionEvaluator.Evaluate(
            new JsonUiCondition
            {
                Key = "score",
                Operator = JsonUiConditionOperator.GreaterThan,
                Value = JsonDocument.Parse("10").RootElement
            },
            state));

        Assert.True(JsonUiConditionEvaluator.Evaluate(
            new JsonUiCondition
            {
                Key = "name",
                Operator = JsonUiConditionOperator.IsNotEmpty
            },
            state));

        Assert.False(JsonUiConditionEvaluator.Evaluate(
            new JsonUiCondition
            {
                Key = string.Empty,
                Operator = JsonUiConditionOperator.IsTrue
            },
            state));
    }

    [Fact]
    public void ThemeResolver_ResolvesPresetWithOverridesWithoutMutatingGlobalService()
    {
        var document = new JsonUiDocument
        {
            ThemePreset = "emerald",
            ThemeOverrides = new Dictionary<string, string>
            {
                ["accentColor"] = "#FF0055",
                ["borderColor"] = "red; position: fixed"
            }
        };

        var css = JsonUiThemeResolver.ResolveScopedCssVariables(document);

        Assert.Contains("--ui-accent: #FF0055", css);
        Assert.DoesNotContain("position: fixed", css);
    }
}
