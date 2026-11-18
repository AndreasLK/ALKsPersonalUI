using ALKsPersonalUI.Core;
using ALKsPersonalUI.Overlays;
using ALKsPersonalUI.Tests.TestInfrastructure;
using ALKsPersonalUI.Windows;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace ALKsPersonalUI.Tests.Components;

public class WindowAndOverlayTests : PersonalUiTestContext
{
    [Fact]
    public void WindowAndTitledWindow_RenderModifierAndTitleImagePlacements()
    {
        var windowCut = Render<Window>(parameters => parameters
            .Add(window => window.Modifier, UIModifier.New.BackgroundImage("/bg/grid.png"))
            .AddChildContent("<div>Inner Content</div>"));

        Assert.Contains("--_w-bg-image: url('/bg/grid.png')", windowCut.Find(".window").GetAttribute("style"));
        Assert.Contains("Inner Content", windowCut.Markup);

        var titledCut = Render<TitledWindow>(parameters => parameters
            .Add(window => window.Title, "Quiz Station")
            .Add(window => window.TitleImageSource, "/icons/recycle.png")
            .Add(window => window.TitleImagePlacement, ImagePlacement.Start)
            .Add(window => window.Modifier, UIModifier.New.HeaderBackgroundImage("/bg/header.png"))
            .AddChildContent("<div>Body</div>"));

        Assert.Equal("/icons/recycle.png", titledCut.Find("img.title-image").GetAttribute("src"));
        Assert.Equal("Quiz Station", titledCut.Find(".title-text").TextContent);
        Assert.Contains("--_tw-header-bg-image: url('/bg/header.png')", titledCut.Find(".title-bar").GetAttribute("style"));
    }

    [Fact]
    public void TitledWindow_WhenTitleImagePlacementIsBackground_SetsHeaderBackgroundImage()
    {
        var cut = Render<TitledWindow>(parameters => parameters
            .Add(window => window.Title, "Banner Header")
            .Add(window => window.TitleImageSource, "/bg/banner.jpg")
            .Add(window => window.TitleImagePlacement, ImagePlacement.Background));

        Assert.Empty(cut.FindAll("img.title-image"));
        Assert.Contains("--_tw-header-bg-image: url('/bg/banner.jpg')", cut.Find(".title-bar").GetAttribute("style"));
    }

    [Fact]
    public void Modal_ForwardsTitleImageAndRespectsBackdropClickAndEscapeClosing()
    {
        var isOpen = true;
        var cut = Render<Modal>(parameters => parameters
            .Add(modal => modal.IsOpen, isOpen)
            .Add(modal => modal.IsOpenChanged, newValue => isOpen = newValue)
            .Add(modal => modal.Title, "Confirm")
            .Add(modal => modal.TitleImageSource, "/icons/warn.png")
            .Add(modal => modal.CloseOnBackdropClick, false)
            .AddChildContent("<p>Are you sure?</p>"));

        Assert.Equal("/icons/warn.png", cut.Find("img.title-image").GetAttribute("src"));

        cut.Find(".modal-backdrop").Click();
        Assert.True(isOpen);

        cut.Find(".modal-backdrop").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        Assert.False(isOpen);
    }

    [Fact]
    public void BottomSheet_ClosesOnBackdropClickAndEscapeKey()
    {
        var isOpen = true;
        var cut = Render<BottomSheet>(parameters => parameters
            .Add(sheet => sheet.IsOpen, isOpen)
            .Add(sheet => sheet.IsOpenChanged, newValue => isOpen = newValue)
            .Add(sheet => sheet.Title, "Options")
            .Add(sheet => sheet.CloseOnBackdropClick, true));

        cut.Find(".sheet-backdrop").Click();
        Assert.False(isOpen);

        isOpen = true;
        var secondCut = Render<BottomSheet>(parameters => parameters
            .Add(sheet => sheet.IsOpen, isOpen)
            .Add(sheet => sheet.IsOpenChanged, newValue => isOpen = newValue));

        secondCut.Find(".sheet-backdrop").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        Assert.False(isOpen);
    }

    [Fact]
    public void Tooltip_TogglesTouchOpenAndOmitsBubbleWhenTextIsBlank()
    {
        var cut = Render<Tooltip>(parameters => parameters
            .Add(tooltip => tooltip.Text, "Helpful hint")
            .Add(tooltip => tooltip.Position, Tooltip.TooltipPosition.Bottom)
            .AddChildContent("<span>Hover me</span>"));

        Assert.Contains("pos-bottom", cut.Find(".ui-tooltip-wrapper").ClassName);
        Assert.Equal("Helpful hint", cut.Find(".tooltip-text").TextContent);

        cut.Find(".ui-tooltip-wrapper").Click();
        Assert.Contains("touch-open", cut.Find(".ui-tooltip-wrapper").ClassName);

        cut.Find(".tooltip-touch-backdrop").Click();
        Assert.DoesNotContain("touch-open", cut.Find(".ui-tooltip-wrapper").ClassName);

        var blankCut = Render<Tooltip>(parameters => parameters
            .Add(tooltip => tooltip.Text, "   ")
            .AddChildContent("<span>No bubble</span>"));

        Assert.Empty(blankCut.FindAll(".tooltip-bubble"));
    }

    [Fact]
    public void ToastContainer_RendersActiveToastsAndDismissesOnCloseClick()
    {
        var toastService = Services.GetRequiredService<ToastService>();
        toastService.Show("Saved offline!", ToastVariant.Success, durationMilliseconds: 10000);

        var cut = Render<ToastContainer>();
        Assert.Contains("Saved offline!", cut.Find(".toast-inner").TextContent);

        cut.Find(".toast-inner button").Click();
        Assert.Empty(cut.FindAll(".toast-item"));
    }
}

