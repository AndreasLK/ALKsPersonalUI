using ALKsPersonalUI.Core;

namespace ALKsPersonalUI.Tests.Core;

public class StyleAndCssBuilderTests
{
    [Fact]
    public void StyleBuilder_Add_TrimsWhitespaceAndTrailingSemicolons()
    {
        var styleResult = new StyleBuilder()
            .Add(" color ", " #ffffff;; ")
            .Add("margin", "10px;")
            .Build();

        Assert.Equal("color: #ffffff; margin: 10px", styleResult);
    }

    [Theory]
    [InlineData(null, "10px")]
    [InlineData("", "10px")]
    [InlineData("   ", "10px")]
    [InlineData("width", null)]
    [InlineData("width", "")]
    [InlineData("width", "   ")]
    public void StyleBuilder_Add_IgnoresNullOrWhitespacePropertyNamesAndValues(string? propertyName, string? propertyValue)
    {
        var styleResult = new StyleBuilder()
            .Add(propertyName!, propertyValue)
            .Build();

        Assert.Equal(string.Empty, styleResult);
    }

    [Fact]
    public void StyleBuilder_AddIf_AppliesDeclarationOnlyWhenConditionIsTrue()
    {
        var styleResult = new StyleBuilder()
            .AddIf(true, "display", "flex")
            .AddIf(false, "color", "red")
            .Build();

        Assert.Equal("display: flex", styleResult);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(" ;;; ")]
    public void StyleBuilder_AddRaw_IgnoresEmptyAndOnlySemicolonStrings(string? rawDeclarations)
    {
        var styleResult = new StyleBuilder()
            .AddRaw(rawDeclarations)
            .Build();

        Assert.Equal(string.Empty, styleResult);
    }

    [Fact]
    public void StyleBuilder_AddRawIf_AppliesRawDeclarationsOnlyWhenConditionIsTrue()
    {
        var styleResult = new StyleBuilder()
            .AddRawIf(true, "opacity: 0.5;")
            .AddRawIf(false, "z-index: 99;")
            .Build();

        Assert.Equal("opacity: 0.5", styleResult);
    }

    [Fact]
    public void CssBuilder_Add_JoinsValidTrimmedClassNamesAndIgnoresDisabledOrBlankEntries()
    {
        var cssClasses = new CssBuilder()
            .Add("  action-btn  ")
            .Add("active", when: true)
            .Add("disabled", when: false)
            .Add("   ", when: true)
            .Add(null, when: true)
            .AddRaw("  custom-extra  ")
            .Build();

        Assert.Equal("action-btn active custom-extra", cssClasses);
    }
}

