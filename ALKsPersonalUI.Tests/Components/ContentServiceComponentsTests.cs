using ALKsPersonalUI.Data;
using ALKsPersonalUI.Indicators;
using ALKsPersonalUI.Inputs;
using ALKsPersonalUI.Loaders;
using ALKsPersonalUI.Loaders.Planets;
using ALKsPersonalUI.Navigation;
using ALKsPersonalUI.Templates;
using ALKsPersonalUI.Tests.TestInfrastructure;
using Bunit;

namespace ALKsPersonalUI.Tests.Components;

public class ContentServiceComponentsTests : PersonalUiTestContext
{
    [Fact]
    public void TemplateSlot_RendersEditorHeaderInEditorModeAndOnlyChildInScreenMode()
    {
        string? selected = null;
        var editorCut = Render<TemplateSlot>(p => p
            .Add(x => x.SlotName, "Header")
            .Add(x => x.IsEditorMode, true)
            .Add(x => x.IsFilled, true)
            .Add(x => x.OnSelectSlot, name => selected = name)
            .AddChildContent("<p>Title</p>"));

        editorCut.Find(".template-slot").Click();
        Assert.Equal("Header", selected);
        Assert.Contains("Filled", editorCut.Markup);

        var screenCut = Render<TemplateSlot>(p => p
            .Add(x => x.SlotName, "Header")
            .Add(x => x.IsEditorMode, false)
            .AddChildContent("<p>Title</p>"));

        Assert.Empty(screenCut.FindAll(".template-slot"));
        Assert.Contains("<p>Title</p>", screenCut.Markup);
    }

    [Fact]
    public void FrameBox_RendersChildContentAndBorderImageStyle()
    {
        var cut = Render<FrameBox>(p => p
            .Add(x => x.BorderImageSource, "/frames/gold.png")
            .AddChildContent("<span>Framed</span>"));

        Assert.Contains("frame-box-image", cut.Find(".frame-box").ClassList);
        Assert.Contains("Framed", cut.Markup);
    }

    [Fact]
    public void FileUpload_RendersInputFileWithAcceptedExtensions()
    {
        var cut = Render<FileUpload>(p => p
            .Add(x => x.AllowMultiple, true)
            .Add(x => x.AcceptedExtensions, ".png,.jpg"));

        var input = cut.Find("input[type='file']");
        Assert.Equal(".png,.jpg", input.GetAttribute("accept"));
        Assert.True(input.HasAttribute("multiple"));
    }

    /// <summary>
    /// Verifies that ImageUpload renders the native file input inside the clickable label zone.
    /// </summary>
    [Fact]
    public void ImageUpload_RendersInputFileInsideClickableZone()
    {
        var cut = Render<ImageUpload>(p => p
            .Add(x => x.AllowMultiple, true)
            .Add(x => x.AcceptedExtensions, ".png,.webp")
            .Add(x => x.PromptText, "Choose artwork"));

        var zoneInput = cut.Find("label.image-upload-zone input[type='file'].image-upload-input");
        Assert.Equal(".png,.webp", zoneInput.GetAttribute("accept"));
        Assert.True(zoneInput.HasAttribute("multiple"));
        Assert.Contains("Choose artwork", cut.Find(".image-upload-prompt").TextContent);
    }

    [Fact]
    public void SearchInput_UpdatesValueAndClearsWithCloseButton()
    {
        string? current = "mars";
        var cut = Render<SearchInput>(p => p
            .Add(x => x.Value, current)
            .Add(x => x.ValueChanged, v => current = v));

        cut.Find("input[type='search']").Input("olympus");
        Assert.Equal("olympus", current);

        cut.Find("button[aria-label='Clear search']").Click();
        Assert.Equal(string.Empty, current);
    }

    [Fact]
    public void TextArea_UpdatesValueOnInput()
    {
        string? current = null;
        var cut = Render<TextArea>(p => p
            .Add(x => x.Label, "Description")
            .Add(x => x.ValueChanged, v => current = v));

        cut.Find("textarea").Input("Exhibition notes");
        Assert.Equal("Exhibition notes", current);
    }

    [Fact]
    public void Breadcrumbs_InvokesOnItemClickedForNonLastItems()
    {
        Breadcrumbs.BreadcrumbItem? clicked = null;
        var items = new[]
        {
            new Breadcrumbs.BreadcrumbItem("1", "Albums"),
            new Breadcrumbs.BreadcrumbItem("2", "Mars")
        };

        var cut = Render<Breadcrumbs>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.OnItemClicked, item => clicked = item));

        cut.Find(".breadcrumbs-link").Click();
        Assert.Equal("Albums", clicked?.Label);
        Assert.Equal("Mars", cut.Find(".breadcrumbs-current").TextContent);
    }

    [Fact]
    public void MediaTile_TogglesSelectionInSelectionMode()
    {
        bool? selected = null;
        var cut = Render<MediaTile>(p => p
            .Add(x => x.Title, "Rover.png")
            .Add(x => x.IsSelectionMode, true)
            .Add(x => x.IsSelected, false)
            .Add(x => x.OnToggleSelect, v => selected = v));

        cut.Find(".media-tile").Click();
        Assert.True(selected);

        var selectedCut = Render<MediaTile>(p => p
            .Add(x => x.Title, "Rover.png")
            .Add(x => x.IsSelectionMode, true)
            .Add(x => x.IsSelected, true)
            .Add(x => x.OnToggleSelect, v => selected = v));

        selectedCut.Find(".media-tile").Click();
        Assert.False(selected);
    }

    [Fact]
    public void PageCarousel_StepsPagesAndNotifiesOnPageChanged()
    {
        int? page = null;
        var cut = Render<PageCarousel>(p => p
            .Add(x => x.ActivePageIndex, 0)
            .Add(x => x.PageCount, 3)
            .Add(x => x.OnPageChanged, idx => page = idx)
            .AddChildContent("<div>Page 1</div>"));

        Assert.Contains("1 / 3", cut.Find(".page-carousel-indicator").TextContent);
        cut.FindAll("button")[1].Click();
        Assert.Equal(1, page);
    }

    [Fact]
    public void PageCarousel_DisablesBoundaryButtonsAndTransitionsPages()
    {
        var changedPages = new List<int>();
        var cut = Render<PageCarousel>(p => p
            .Add(x => x.ActivePageIndex, 0)
            .Add(x => x.PageCount, 2)
            .Add(x => x.OnPageChanged, idx => changedPages.Add(idx))
            .AddChildContent("<div>Page content</div>"));

        var buttons = cut.FindAll("button");
        Assert.True(buttons[0].HasAttribute("disabled"));
        Assert.False(buttons[1].HasAttribute("disabled"));

        buttons[1].Click();
        Assert.Equal(new[] { 1 }, changedPages);
        Assert.Contains("2 / 2", cut.Find(".page-carousel-indicator").TextContent);

        buttons = cut.FindAll("button");
        Assert.False(buttons[0].HasAttribute("disabled"));
        Assert.True(buttons[1].HasAttribute("disabled"));

        buttons[0].Click();
        Assert.Equal(new[] { 1, 0 }, changedPages);
        Assert.Contains("1 / 2", cut.Find(".page-carousel-indicator").TextContent);
    }

    [Fact]
    public void OrbitalLoader_WithMarsChildContent_RendersPayloadStabilizerAndPlanet()
    {
        var cut = Render<OrbitalLoader>(p => p
            .AddChildContent<Mars>());

        Assert.NotEmpty(cut.FindAll(".payload-stabilizer"));
        Assert.NotEmpty(cut.FindAll(".planet"));
    }

    [Fact]
    public void CountdownTimer_AutoStartDefaultsToTrue()
    {
        var cut = Render<CountdownTimer>(p => p
            .Add(x => x.TotalSeconds, 10));

        Assert.True(cut.Instance.AutoStart);
    }

    [Fact]
    public void LockBanner_RendersWhenLockedAndHidesWhenUnlocked()
    {
        var unlockedCut = Render<LockBanner>(p => p.Add(x => x.IsLocked, false));
        Assert.Empty(unlockedCut.FindAll(".lock-banner"));

        bool requested = false;
        var lockedCut = Render<LockBanner>(p => p
            .Add(x => x.IsLocked, true)
            .Add(x => x.LockedByUser, "andre")
            .Add(x => x.OnRequestUnlock, () => requested = true));

        Assert.Contains("Locked by andre", lockedCut.Markup);
        lockedCut.Find("button").Click();
        Assert.True(requested);
    }
}
