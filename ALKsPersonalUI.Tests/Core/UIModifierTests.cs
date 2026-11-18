using ALKsPersonalUI.Core;

namespace ALKsPersonalUI.Tests.Core;

public class UIModifierTests
{
    [Fact]
    public void UIModifier_SizingPaddingAndTactileMethods_PopulateExpectedValues()
    {
        var modifier = UIModifier.New
            .Width(320)
            .Height("100%")
            .MinWidth(120)
            .MinHeight(48)
            .Padding(12, 24)
            .Gap(10)
            .BaseColor("#22c55e")
            .TextColor("#111111")
            .AccentColor("#3b82f6")
            .ButtonExtrusionDepth(4)
            .PressAnimationDuration("0.08s")
            .StrokeThickness(3)
            .InwardPressScaleFactor(0.91);

        Assert.Equal("320px", modifier.WidthValue);
        Assert.Equal("100%", modifier.HeightValue);
        Assert.Equal("120px", modifier.MinWidthValue);
        Assert.Equal("48px", modifier.MinHeightValue);
        Assert.Equal("12px 24px", modifier.PaddingValue);
        Assert.Equal("12px", modifier.PaddingVerticalValue);
        Assert.Equal("24px", modifier.PaddingHorizontalValue);
        Assert.Equal("10px", modifier.GapValue);
        Assert.Equal("#22c55e", modifier.BaseColorValue);
        Assert.Equal("#111111", modifier.TextColorValue);
        Assert.Equal("#3b82f6", modifier.AccentColorValue);
        Assert.Equal("4px", modifier.ButtonExtrusionDepthValue);
        Assert.Equal("0.08s", modifier.PressAnimationDurationValue);
        Assert.Equal("3px", modifier.StrokeThicknessValue);
        Assert.Equal("0.91", modifier.InwardPressScaleFactorValue);
    }

    [Fact]
    public void UIModifier_PaddingString_ParsesSingleAndMultiTokenStrings()
    {
        var singleTokenModifier = UIModifier.New.Padding("16px");
        var multiTokenModifier = UIModifier.New.Padding("8px 20px");

        Assert.Equal("16px", singleTokenModifier.PaddingVerticalValue);
        Assert.Equal("16px", singleTokenModifier.PaddingHorizontalValue);

        Assert.Equal("8px", multiTokenModifier.PaddingVerticalValue);
        Assert.Equal("20px", multiTokenModifier.PaddingHorizontalValue);
    }

    [Fact]
    public void UIModifier_BackgroundImageAndHeaderBackgroundImage_OutputCssInBuilders()
    {
        var modifier = UIModifier.New
            .BackgroundImage("/assets/wood.png", ImageFit.Contain, "top left", "repeat-x")
            .HeaderBackgroundImage("/assets/banner.jpg", ImageFit.Fill, "bottom");

        var windowStyle = modifier.BuildWindowStyle();
        var titleBarStyle = modifier.BuildTitleBarStyle();
        var elementStyle = modifier.BuildStyle();

        Assert.Contains("--_w-bg-image: url('/assets/wood.png')", windowStyle);
        Assert.Contains("--_w-bg-size: contain", windowStyle);
        Assert.Contains("--_w-bg-pos: top left", windowStyle);
        Assert.Contains("--_w-bg-repeat: repeat-x", windowStyle);

        Assert.Contains("--_tw-header-bg-image: url('/assets/banner.jpg')", titleBarStyle);
        Assert.Contains("--_tw-header-bg-size: fill", titleBarStyle);
        Assert.Contains("--_tw-header-bg-pos: bottom", titleBarStyle);

        Assert.Contains("background-image: url('/assets/wood.png')", elementStyle);
        Assert.Contains("background-size: contain", elementStyle);
        Assert.Contains("background-position: top left", elementStyle);
        Assert.Contains("background-repeat: repeat-x", elementStyle);
    }

    [Fact]
    public void UIModifier_ForTitledWindowFrame_CopiesBackgroundImageAndRoutesPaddingToZero()
    {
        var modifier = UIModifier.New
            .Width(400)
            .Padding(20)
            .Gap(16)
            .BackgroundImage("/assets/card-bg.png", ImageFit.ScaleDown)
            .Class("custom-card");

        var frameModifier = modifier.ForTitledWindowFrame();

        Assert.Equal("400px", frameModifier.WidthValue);
        Assert.Equal("0", frameModifier.PaddingValue);
        Assert.Equal("0", frameModifier.GapValue);
        Assert.Equal("/assets/card-bg.png", frameModifier.BackgroundImageSourceValue);
        Assert.Equal("scale-down", frameModifier.BackgroundSizeValue);
        Assert.Contains("custom-card", frameModifier.BuildClass());
    }

    [Fact]
    public void UIModifier_Clone_DeepCopiesFrameBodyAndClassBuildersIndependently()
    {
        var originalModifier = UIModifier.New
            .FrameStyle("border-style", "dashed")
            .BodyStyle("flex-wrap", "wrap")
            .Class("base-class");

        var clonedModifier = originalModifier.Clone();
        clonedModifier.FrameStyle("opacity", "0.75");
        clonedModifier.BodyStyle("overflow-y", "scroll");
        clonedModifier.Class("cloned-only");

        Assert.DoesNotContain("opacity", originalModifier.BuildFrameExtras());
        Assert.DoesNotContain("overflow-y", originalModifier.BuildTitledWindowBodyStyle());
        Assert.DoesNotContain("cloned-only", originalModifier.BuildClass());

        Assert.Contains("opacity: 0.75", clonedModifier.BuildFrameExtras());
        Assert.Contains("overflow-y: scroll", clonedModifier.BuildTitledWindowBodyStyle());
        Assert.Contains("cloned-only", clonedModifier.BuildClass());
    }
}

