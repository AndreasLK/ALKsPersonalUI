using ALKsPersonalUI.Core;
using ALKsPersonalUI.Indicators;
using ALKsPersonalUI.Layout;
using ALKsPersonalUI.Tests.TestInfrastructure;

namespace ALKsPersonalUI.Tests.Components;

public class ImageTextAndPillTests : PersonalUiTestContext
{
    [Fact]
    public void Image_WithValidSourceCaptionAndChildContent_RendersImageOverlayAndCaption()
    {
        var cut = Render<Image>(parameters => parameters
            .Add(image => image.Source, "/images/mars.png")
            .Add(image => image.Alt, "Red planet")
            .Add(image => image.Fit, ImageFit.Contain)
            .Add(image => image.Caption, "Mars surface")
            .AddChildContent("<span>Overlay Badge</span>"));

        var imageElement = cut.Find("img.ui-image-element");
        Assert.Equal("/images/mars.png", imageElement.GetAttribute("src"));
        Assert.Equal("Red planet", imageElement.GetAttribute("alt"));
        Assert.Contains("object-fit: contain", imageElement.GetAttribute("style"));
        Assert.Equal("Mars surface", cut.Find("figcaption.ui-image-caption").TextContent);
        Assert.Contains("Overlay Badge", cut.Find(".ui-image-overlay").InnerHtml);
    }

    [Fact]
    public void Image_WhenSourceMissingOrLoadFails_RendersFallbackAndRecoversWhenSourceChanges()
    {
        var missingCut = Render<Image>(parameters => parameters
            .Add(image => image.Source, "")
            .Add(image => image.FallbackText, "No photo selected"));

        Assert.Empty(missingCut.FindAll("img.ui-image-element"));
        Assert.Equal("No photo selected", missingCut.Find(".ui-image-fallback").TextContent);

        var recoveryCut = Render<Image>(parameters => parameters
            .Add(image => image.Source, "/broken.png")
            .Add(image => image.FallbackText, "Unavailable"));

        recoveryCut.Find("img.ui-image-element").TriggerEvent("onerror", EventArgs.Empty);
        Assert.Equal("Unavailable", recoveryCut.Find(".ui-image-fallback").TextContent);

        recoveryCut.Render(parameters => parameters
            .Add(image => image.Source, "/valid.png")
            .Add(image => image.FallbackText, "Unavailable"));

        Assert.Empty(recoveryCut.FindAll(".ui-image-fallback"));
        Assert.Equal("/valid.png", recoveryCut.Find("img.ui-image-element").GetAttribute("src"));
    }

    [Fact]
    public void Text_RendersVariantsLineClampAndInlineOrBackgroundImages()
    {
        var inlineCut = Render<Text>(parameters => parameters
            .Add(text => text.Variant, TypographyVariant.H2)
            .Add(text => text.Value, "Waste Quiz")
            .Add(text => text.Muted, true)
            .Add(text => text.MaxLines, 2)
            .Add(text => text.ImageSource, "/icons/leaf.png")
            .Add(text => text.ImagePlacement, ImagePlacement.End));

        var spanElement = inlineCut.Find("span.ui-text");
        var inlineImage = inlineCut.Find("img.ui-text-image");

        Assert.Contains("variant-h2", spanElement.ClassName);
        Assert.Contains("with-inline-image", spanElement.ClassName);
        Assert.Contains("line-clamp", spanElement.ClassName);
        Assert.Contains("--_txt-dir: row-reverse", spanElement.GetAttribute("style"));
        Assert.Equal("/icons/leaf.png", inlineImage.GetAttribute("src"));
        Assert.Contains("Waste Quiz", spanElement.TextContent);

        var replacedCut = Render<Text>(parameters => parameters
            .Add(text => text.Value, "Hidden")
            .Add(text => text.ImageSource, "/icons/only-icon.png")
            .Add(text => text.ImagePlacement, ImagePlacement.ReplaceText));

        Assert.DoesNotContain("Hidden", replacedCut.Find("span.ui-text").TextContent);
        Assert.Equal("Hidden", replacedCut.Find("img.ui-text-image").GetAttribute("alt"));
    }

    [Fact]
    public void Pill_RendersSpanWhenStaticAndButtonWhenInteractiveWithImageSupport()
    {
        var clickCount = 0;

        var staticCut = Render<Pill>(parameters => parameters
            .Add(pill => pill.Text, "Static Badge")
            .Add(pill => pill.Variant, Pill.PillVariant.Success)
            .Add(pill => pill.ImageSource, "/icons/check.png"));

        Assert.NotEmpty(staticCut.FindAll("span.ui-pill"));
        Assert.Empty(staticCut.FindAll("button.ui-pill"));
        Assert.Equal("/icons/check.png", staticCut.Find("img.ui-pill-image").GetAttribute("src"));

        var interactiveCut = Render<Pill>(parameters => parameters
            .Add(pill => pill.Text, "Filter")
            .Add(pill => pill.Active, true)
            .Add(pill => pill.ImageSource, "/icons/filter.png")
            .Add(pill => pill.ImagePlacement, ImagePlacement.ReplaceText)
            .Add(pill => pill.OnClick, () => clickCount++));

        var buttonElement = interactiveCut.Find("button.ui-pill");
        Assert.Contains("active", buttonElement.ClassName);
        Assert.DoesNotContain("Filter", buttonElement.TextContent);

        buttonElement.Click();
        Assert.Equal(1, clickCount);
    }

    [Fact]
    public void Pill_WhenDisabled_DoesNotInvokeOnClick()
    {
        var clickCount = 0;
        var cut = Render<Pill>(parameters => parameters
            .Add(pill => pill.Text, "Disabled Pill")
            .Add(pill => pill.Disabled, true)
            .Add(pill => pill.OnClick, () => clickCount++));

        cut.Find("button.ui-pill").Click();
        Assert.Equal(0, clickCount);
    }
}

