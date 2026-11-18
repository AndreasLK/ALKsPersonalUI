using System.Globalization;
using ALKsPersonalUI.Core;
using Microsoft.AspNetCore.Components.Web;

namespace ALKsPersonalUI.Tests.Core;

public class GeometryAndLayoutDefaultsTests
{
    [Theory]
    [InlineData(Math.PI, 180.0)]
    [InlineData(Math.PI / 2.0, 90.0)]
    [InlineData(0.0, 0.0)]
    public void GeometryMath_RadiansToDegrees_ConvertsAccurately(double radians, double expectedDegrees)
    {
        var actualDegrees = GeometryMath.RadiansToDegrees(radians);

        Assert.Equal(expectedDegrees, actualDegrees, precision: 6);
    }

    [Theory]
    [InlineData(0.0, -10.0, 0.0)]
    [InlineData(10.0, 0.0, 90.0)]
    [InlineData(-10.0, 0.0, -90.0)]
    [InlineData(0.0, 10.0, 180.0)]
    public void GeometryMath_CalculatePointerAngleInDegrees_MapsQuadrantsCorrectly(
        double deltaX,
        double deltaY,
        double expectedDegrees)
    {
        var actualDegrees = GeometryMath.CalculatePointerAngleInDegrees(deltaX, deltaY);

        Assert.Equal(expectedDegrees, actualDegrees, precision: 6);
    }

    [Theory]
    [InlineData(3.0, 4.0, 5.0)]
    [InlineData(-3.0, -4.0, 5.0)]
    [InlineData(0.0, 0.0, 0.0)]
    public void GeometryMath_CalculateEuclideanDistance_ComputesPositiveLength(
        double deltaX,
        double deltaY,
        double expectedDistance)
    {
        var actualDistance = GeometryMath.CalculateEuclideanDistance(deltaX, deltaY);

        Assert.Equal(expectedDistance, actualDistance, precision: 6);
    }

    [Fact]
    public void GeometryMath_FormatInvariant_UsesDotDecimalSeparatorUnderCommaCulture()
    {
        var previousCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("da-DK");

        try
        {
            var formattedValue = GeometryMath.FormatInvariant(12.5);
            Assert.Equal("12.5", formattedValue);
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }

    [Fact]
    public void KeyboardKeys_RecognizesEscapeIncrementAndDecrementKeys()
    {
        Assert.True(KeyboardKeys.IsEscape(new KeyboardEventArgs { Key = "Escape" }));
        Assert.False(KeyboardKeys.IsEscape(new KeyboardEventArgs { Key = "Enter" }));

        Assert.True(KeyboardKeys.IsIncrementKey(new KeyboardEventArgs { Key = "ArrowRight" }));
        Assert.True(KeyboardKeys.IsIncrementKey(new KeyboardEventArgs { Key = "ArrowUp" }));
        Assert.False(KeyboardKeys.IsIncrementKey(new KeyboardEventArgs { Key = "ArrowLeft" }));

        Assert.True(KeyboardKeys.IsDecrementKey(new KeyboardEventArgs { Key = "ArrowLeft" }));
        Assert.True(KeyboardKeys.IsDecrementKey(new KeyboardEventArgs { Key = "ArrowDown" }));
        Assert.False(KeyboardKeys.IsDecrementKey(new KeyboardEventArgs { Key = "ArrowUp" }));
    }

    [Theory]
    [InlineData(HorizontalAlignment.Start, "flex-start")]
    [InlineData(HorizontalAlignment.Center, "center")]
    [InlineData(HorizontalAlignment.End, "flex-end")]
    [InlineData(HorizontalAlignment.Stretch, "stretch")]
    public void LayoutDefaults_ToCssHorizontalAlignment_ReturnsExpectedFlexKeyword(
        HorizontalAlignment alignment,
        string expectedCss)
    {
        Assert.Equal(expectedCss, LayoutDefaults.ToCss(alignment));
    }

    [Theory]
    [InlineData(VerticalAlignment.Top, "flex-start")]
    [InlineData(VerticalAlignment.Center, "center")]
    [InlineData(VerticalAlignment.Bottom, "flex-end")]
    [InlineData(VerticalAlignment.Stretch, "stretch")]
    public void LayoutDefaults_ToCssVerticalAlignment_ReturnsExpectedFlexKeyword(
        VerticalAlignment alignment,
        string expectedCss)
    {
        Assert.Equal(expectedCss, LayoutDefaults.ToCss(alignment));
    }

    [Theory]
    [InlineData(Arrangement.Start, "flex-start")]
    [InlineData(Arrangement.Center, "center")]
    [InlineData(Arrangement.End, "flex-end")]
    [InlineData(Arrangement.SpaceBetween, "space-between")]
    [InlineData(Arrangement.SpaceAround, "space-around")]
    [InlineData(Arrangement.SpaceEvenly, "space-evenly")]
    public void LayoutDefaults_ToCssArrangement_ReturnsExpectedJustifyKeyword(
        Arrangement arrangement,
        string expectedCss)
    {
        Assert.Equal(expectedCss, LayoutDefaults.ToCss(arrangement));
    }

    [Theory]
    [InlineData(BoxAlignment.TopStart, "flex-start", "flex-start")]
    [InlineData(BoxAlignment.TopCenter, "flex-start", "center")]
    [InlineData(BoxAlignment.TopEnd, "flex-start", "flex-end")]
    [InlineData(BoxAlignment.CenterStart, "center", "flex-start")]
    [InlineData(BoxAlignment.Center, "center", "center")]
    [InlineData(BoxAlignment.CenterEnd, "center", "flex-end")]
    [InlineData(BoxAlignment.BottomStart, "flex-end", "flex-start")]
    [InlineData(BoxAlignment.BottomCenter, "flex-end", "center")]
    [InlineData(BoxAlignment.BottomEnd, "flex-end", "flex-end")]
    public void LayoutDefaults_ToCssBoxAlignment_ReturnsAlignAndJustifyPair(
        BoxAlignment alignment,
        string expectedAlignItems,
        string expectedJustifyContent)
    {
        var (alignItems, justifyContent) = LayoutDefaults.ToCss(alignment);

        Assert.Equal(expectedAlignItems, alignItems);
        Assert.Equal(expectedJustifyContent, justifyContent);
    }

    [Theory]
    [InlineData(FontWeight.Light, "300")]
    [InlineData(FontWeight.Regular, "400")]
    [InlineData(FontWeight.Medium, "500")]
    [InlineData(FontWeight.SemiBold, "600")]
    [InlineData(FontWeight.Bold, "700")]
    public void LayoutDefaults_ToCssFontWeight_ReturnsExpectedNumericWeight(FontWeight weight, string expectedCss)
    {
        Assert.Equal(expectedCss, LayoutDefaults.ToCss(weight));
    }

    [Theory]
    [InlineData(TextAlignment.Start, "left")]
    [InlineData(TextAlignment.Center, "center")]
    [InlineData(TextAlignment.End, "right")]
    [InlineData(TextAlignment.Justify, "justify")]
    public void LayoutDefaults_ToCssTextAlignment_ReturnsExpectedCssAlignment(TextAlignment alignment, string expectedCss)
    {
        Assert.Equal(expectedCss, LayoutDefaults.ToCss(alignment));
    }

    [Theory]
    [InlineData(ImageFit.Cover, "cover")]
    [InlineData(ImageFit.Contain, "contain")]
    [InlineData(ImageFit.Fill, "fill")]
    [InlineData(ImageFit.ScaleDown, "scale-down")]
    [InlineData(ImageFit.None, "none")]
    public void LayoutDefaults_ToCssImageFit_ReturnsExpectedCssKeyword(ImageFit imageFit, string expectedCss)
    {
        Assert.Equal(expectedCss, LayoutDefaults.ToCss(imageFit));
    }

    [Theory]
    [InlineData(ImagePlacement.Start, "row")]
    [InlineData(ImagePlacement.End, "row-reverse")]
    [InlineData(ImagePlacement.Top, "column")]
    [InlineData(ImagePlacement.Bottom, "column-reverse")]
    [InlineData(ImagePlacement.ReplaceText, "row")]
    [InlineData(ImagePlacement.Background, "row")]
    public void LayoutDefaults_ToFlexDirectionCss_ReturnsExpectedFlexDirection(
        ImagePlacement placement,
        string expectedDirection)
    {
        Assert.Equal(expectedDirection, LayoutDefaults.ToFlexDirectionCss(placement));
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("   ", null)]
    [InlineData("/images/battery.png", "url('/images/battery.png')")]
    [InlineData("url('/images/battery.png')", "url('/images/battery.png')")]
    [InlineData("URL('/images/battery.png')", "URL('/images/battery.png')")]
    public void LayoutDefaults_FormatCssUrl_HandlesEmptyAndAlreadyWrappedUrlsSafely(
        string? rawSource,
        string? expectedCssUrl)
    {
        Assert.Equal(expectedCssUrl, LayoutDefaults.FormatCssUrl(rawSource));
    }
}

