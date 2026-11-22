using ALKsPersonalUI.Buttons;
using ALKsPersonalUI.Core;
using ALKsPersonalUI.Tests.TestInfrastructure;
using Microsoft.AspNetCore.Components.Web;

namespace ALKsPersonalUI.Tests.Components;

public class ActionButtonTests : PersonalUiTestContext
{
    [Fact]
    public void ActionButton_RendersVariantShapeSizeAndText()
    {
        var cut = Render<ActionButton>(parameters => parameters
            .Add(button => button.Text, "Confirm Order")
            .Add(button => button.Variant, ActionButton.ButtonVariant.Outline)
            .Add(button => button.Shape, ActionButton.ButtonShape.Pill)
            .Add(button => button.Size, ActionButton.ButtonSize.Large));

        var buttonElement = cut.Find("button.action-btn");
        Assert.Contains("variant-outline", buttonElement.ClassName);
        Assert.Contains("shape-pill", buttonElement.ClassName);
        Assert.Contains("Confirm Order", buttonElement.TextContent);
    }

    [Theory]
    [InlineData(ImagePlacement.Start, "row")]
    [InlineData(ImagePlacement.End, "row-reverse")]
    [InlineData(ImagePlacement.Top, "column")]
    [InlineData(ImagePlacement.Bottom, "column-reverse")]
    public void ActionButton_InlineImagePlacements_RenderImageAndTextWithExpectedFlexDirection(
        ImagePlacement placement,
        string expectedDirection)
    {
        var cut = Render<ActionButton>(parameters => parameters
            .Add(button => button.Text, "Battery")
            .Add(button => button.ImageSource, "/images/battery.png")
            .Add(button => button.ImageAlt, "Battery icon")
            .Add(button => button.ImageSize, "32px")
            .Add(button => button.ImagePlacement, placement));

        var buttonElement = cut.Find("button.action-btn");
        var imageElement = cut.Find("img.btn-image");

        Assert.Equal("/images/battery.png", imageElement.GetAttribute("src"));
        Assert.Equal("Battery icon", imageElement.GetAttribute("alt"));
        Assert.Contains($"--_btn-content-dir: {expectedDirection}", buttonElement.GetAttribute("style"));
        Assert.Contains("--_btn-img-size: 32px", buttonElement.GetAttribute("style"));
        Assert.Contains("Battery", cut.Find(".btn-content").TextContent);
    }

    [Fact]
    public void ActionButton_ReplaceTextPlacement_OmitsTextAndFillsImageSizeWhenUnset()
    {
        var cut = Render<ActionButton>(parameters => parameters
            .Add(button => button.Text, "Hidden Text")
            .Add(button => button.ImageSource, "/images/logo.png")
            .Add(button => button.ImagePlacement, ImagePlacement.ReplaceText));

        var buttonElement = cut.Find("button.action-btn");
        var imageElement = cut.Find("img.btn-image");

        Assert.Contains("has-replaced-text", buttonElement.ClassName);
        Assert.Contains("--_btn-img-size: 100%", buttonElement.GetAttribute("style"));
        Assert.Equal("Hidden Text", imageElement.GetAttribute("alt"));
        Assert.DoesNotContain("Hidden Text", cut.Find(".btn-content").TextContent);
    }

    [Fact]
    public void ActionButton_BackgroundPlacementAndModifierBackgroundImage_SetBackgroundCssVariables()
    {
        var directBackgroundCut = Render<ActionButton>(parameters => parameters
            .Add(button => button.Text, "Bg Button")
            .Add(button => button.ImageSource, "/images/wood.png")
            .Add(button => button.ImageFit, ImageFit.Contain)
            .Add(button => button.ImagePlacement, ImagePlacement.Background));

        var modifierBackgroundCut = Render<ActionButton>(parameters => parameters
            .Add(button => button.Text, "Modifier Bg")
            .Add(button => button.Modifier, UIModifier.New.BackgroundImage("/images/stone.png", ImageFit.Fill)));

        Assert.Empty(directBackgroundCut.FindAll("img.btn-image"));
        Assert.Contains("--_btn-bg-image: url('/images/wood.png')", directBackgroundCut.Find("button").GetAttribute("style"));
        Assert.Contains("--_btn-bg-size: contain", directBackgroundCut.Find("button").GetAttribute("style"));
        Assert.Contains("Bg Button", directBackgroundCut.Find("button").TextContent);

        Assert.Contains("--_btn-bg-image: url('/images/stone.png')", modifierBackgroundCut.Find("button").GetAttribute("style"));
        Assert.Contains("--_btn-bg-size: fill", modifierBackgroundCut.Find("button").GetAttribute("style"));
    }

    [Fact]
    public void ActionButton_BlocksClicksWhenDisabledOrLoading()
    {
        var clickCount = 0;

        var disabledCut = Render<ActionButton>(parameters => parameters
            .Add(button => button.Text, "Disabled")
            .Add(button => button.IsDisabled, true)
            .Add(button => button.OnClick, () => clickCount++));

        var loadingCut = Render<ActionButton>(parameters => parameters
            .Add(button => button.Text, "Loading")
            .Add(button => button.IsLoading, true)
            .Add(button => button.OnClick, () => clickCount++));

        disabledCut.Find("button").Click();
        loadingCut.Find("button").Click();

        Assert.Equal(0, clickCount);
        Assert.NotEmpty(loadingCut.FindAll(".circular-loader"));
    }

    [Fact]
    public async Task ActionButton_AutoPressEffect_SwitchesBetweenTouchAndMouseClasses()
    {
        var cut = Render<ActionButton>(parameters => parameters
            .Add(button => button.Text, "Adaptive")
            .Add(button => button.PressEffect, ButtonPressEffect.Auto));

        var buttonElement = cut.Find("button");
        Assert.Contains("press-auto", buttonElement.ClassName);

        await buttonElement.TriggerEventAsync("onpointerdown", new PointerEventArgs { PointerType = "touch" });
        Assert.Contains("press-inwardscale", cut.Find("button").ClassName);

        await cut.Find("button").TriggerEventAsync("onpointerdown", new PointerEventArgs { PointerType = "mouse" });
        Assert.Contains("press-down3d", cut.Find("button").ClassName);
    }

    [Fact]
    public async Task ActionButton_HoldToConfirm_IgnoresRegularClickCancelsOnEarlyReleaseAndInvokesAfterDuration()
    {
        var confirmCount = 0;
        var cut = Render<ActionButton>(parameters => parameters
            .Add(button => button.Text, "Hold")
            .Add(button => button.HoldToConfirmDurationMilliseconds, 60)
            .Add(button => button.OnClick, () => confirmCount++));

        var buttonElement = cut.Find("button");
        buttonElement.Click();
        Assert.Equal(0, confirmCount);

        var pointerDownTask = buttonElement.TriggerEventAsync("onpointerdown", new PointerEventArgs());
        await buttonElement.TriggerEventAsync("onpointerup", new PointerEventArgs());
        await pointerDownTask;
        Assert.Equal(0, confirmCount);

        await cut.Find("button").TriggerEventAsync("onpointerdown", new PointerEventArgs());
        cut.WaitForAssertion(() => Assert.Equal(1, confirmCount), TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task ActionButton_HoldToConfirm_SupportsKeyboardHoldWithRepeatAndCancelsOnEarlyKeyUpOrBlur()
    {
        var confirmCount = 0;
        var cut = Render<ActionButton>(parameters => parameters
            .Add(button => button.Text, "Hold Keyboard")
            .Add(button => button.HoldToConfirmDurationMilliseconds, 60)
            .Add(button => button.OnClick, () => confirmCount++));

        var buttonElement = cut.Find("button");

        // Non-activation keys must not start a hold gesture.
        await buttonElement.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Tab" });
        Assert.DoesNotContain("is-holding", cut.Find("button").ClassName);

        // Releasing Space early cancels the pending hold confirmation.
        var earlySpaceTask = cut.Find("button").TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = " " });
        Assert.Contains("is-holding", cut.Find("button").ClassName);
        await cut.Find("button").TriggerEventAsync("onkeyup", new KeyboardEventArgs { Key = " " });
        await earlySpaceTask;
        Assert.Equal(0, confirmCount);
        Assert.DoesNotContain("is-holding", cut.Find("button").ClassName);

        // Losing focus mid-hold via Enter also cancels cleanly.
        var blurEnterTask = cut.Find("button").TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Enter" });
        await cut.Find("button").TriggerEventAsync("onblur", new FocusEventArgs());
        await blurEnterTask;
        Assert.Equal(0, confirmCount);

        // Key-repeat events while holding Space must not restart or block completion.
        var spaceHoldTask = cut.Find("button").TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = " " });
        await cut.Find("button").TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = " ", Repeat = true });
        await spaceHoldTask;
        cut.WaitForAssertion(() => Assert.Equal(1, confirmCount), TimeSpan.FromSeconds(1));
        await cut.Find("button").TriggerEventAsync("onkeyup", new KeyboardEventArgs { Key = " " });

        // Completing a full Enter hold confirms the action once.
        await cut.Find("button").TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Enter" });
        cut.WaitForAssertion(() => Assert.Equal(2, confirmCount), TimeSpan.FromSeconds(1));
    }
}
