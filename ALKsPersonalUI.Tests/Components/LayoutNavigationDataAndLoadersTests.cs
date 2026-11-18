using ALKsPersonalUI.Core;
using ALKsPersonalUI.Data;
using ALKsPersonalUI.Inputs;
using ALKsPersonalUI.Layout;
using ALKsPersonalUI.Loaders;
using ALKsPersonalUI.Loaders.Planets;
using ALKsPersonalUI.Navigation;
using ALKsPersonalUI.Tests.TestInfrastructure;

namespace ALKsPersonalUI.Tests.Components;

public class LayoutNavigationDataAndLoadersTests : PersonalUiTestContext
{
    private sealed record WasteItem(string Name, string Category, int Points);

    [Fact]
    public void LayoutPrimitives_BoxRowColumnGridDividerSpacerAndScaffold_RenderExpectedStyles()
    {
        var boxCut = Render<Box>(parameters => parameters
            .Add(box => box.ContentAlignment, BoxAlignment.BottomEnd)
            .Add(box => box.Modifier, UIModifier.New.BackgroundImage("/bg/box.png")));
        Assert.Contains("align-items: flex-end", boxCut.Find(".ui-box").GetAttribute("style"));
        Assert.Contains("background-image: url('/bg/box.png')", boxCut.Find(".ui-box").GetAttribute("style"));

        var rowCut = Render<Row>(parameters => parameters
            .Add(row => row.HorizontalArrangement, Arrangement.SpaceBetween)
            .Add(row => row.Wrap, true));
        Assert.Contains("justify-content: space-between", rowCut.Find(".ui-row").GetAttribute("style"));
        Assert.Contains("flex-wrap: wrap", rowCut.Find(".ui-row").GetAttribute("style"));

        var columnCut = Render<Column>(parameters => parameters
            .Add(column => column.HorizontalAlignment, HorizontalAlignment.Center));
        Assert.Contains("align-items: center", columnCut.Find(".ui-column").GetAttribute("style"));

        var gridCut = Render<Grid>(parameters => parameters
            .Add(grid => grid.Columns, 3));
        Assert.Contains("repeat(3, minmax(0, 1fr))", gridCut.Find(".ui-grid").GetAttribute("style"));

        var dividerCut = Render<Divider>(parameters => parameters
            .Add(divider => divider.Vertical, true));
        Assert.Equal("vertical", dividerCut.Find(".ui-divider").GetAttribute("aria-orientation"));

        var flexSpacerCut = Render<Spacer>();
        var fixedSpacerCut = Render<Spacer>(parameters => parameters
            .Add(spacer => spacer.Modifier, UIModifier.New.Width(24)));
        Assert.Contains("flex-grow: 1", flexSpacerCut.Find(".ui-spacer").GetAttribute("style"));
        Assert.DoesNotContain("flex-grow: 1", fixedSpacerCut.Find(".ui-spacer").GetAttribute("style"));

        var scaffoldCut = Render<Scaffold>(parameters => parameters
            .Add(scaffold => scaffold.EnableToasts, false)
            .AddChildContent("<p>Main Screen</p>"));
        Assert.Contains("--ui-bg-app", scaffoldCut.Find(".ui-scaffold").GetAttribute("style"));
        Assert.Empty(scaffoldCut.FindAll(".ui-toast-container"));
    }

    [Fact]
    public void NavigationComponents_TabsNavbarAndBurgerMenu_RenderImagesAndSwitchState()
    {
        var navbarCut = Render<Navbar>(parameters => parameters
            .Add(navbar => navbar.Title, "Oekolariet")
            .Add(navbar => navbar.BrandImageSource, "/images/brand.png"));
        Assert.Equal("/images/brand.png", navbarCut.Find("img.brand-image").GetAttribute("src"));

        var burgerCut = Render<BurgerMenu>(parameters => parameters
            .Add(menu => menu.Title, "Menu")
            .AddChildContent("<a href='/home'>Home</a>"));
        burgerCut.Find("button.action-btn").Click();
        Assert.NotEmpty(burgerCut.FindAll(".drawer-backdrop"));

        var activeTabId = "tab1";
        var tabsCut = Render<Tabs>(parameters => parameters
            .Add(tabs => tabs.ActiveTabId, activeTabId)
            .Add(tabs => tabs.ActiveTabIdChanged, id => activeTabId = id)
            .Add(tabs => tabs.IsTrailMode, false)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<TabPanel>(0);
                builder.AddAttribute(1, nameof(TabPanel.Id), "tab1");
                builder.AddAttribute(2, nameof(TabPanel.Title), "Step 1");
                builder.AddAttribute(3, nameof(TabPanel.ImageSource), "/icons/one.png");
                builder.AddAttribute(4, nameof(TabPanel.ChildContent), (Microsoft.AspNetCore.Components.RenderFragment)(childBuilder => childBuilder.AddContent(5, "First Content")));
                builder.CloseComponent();

                builder.OpenComponent<TabPanel>(6);
                builder.AddAttribute(7, nameof(TabPanel.Id), "tab2");
                builder.AddAttribute(8, nameof(TabPanel.Title), "Step 2");
                builder.AddAttribute(9, nameof(TabPanel.ChildContent), (Microsoft.AspNetCore.Components.RenderFragment)(childBuilder => childBuilder.AddContent(10, "Second Content")));
                builder.CloseComponent();
            }));

        Assert.Equal("/icons/one.png", tabsCut.Find("img.btn-image").GetAttribute("src"));
        Assert.Contains("First Content", tabsCut.Find(".tabs-body").TextContent);

        tabsCut.FindAll("button[role='tab']")[1].Click();
        Assert.Equal("tab2", activeTabId);
        Assert.Contains("Second Content", tabsCut.Find(".tabs-body").TextContent);

        var trailTabsCut = Render<Tabs>(parameters => parameters
            .Add(tabs => tabs.ActiveTabId, "step1")
            .Add(tabs => tabs.IsTrailMode, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<TabPanel>(0);
                builder.AddAttribute(1, nameof(TabPanel.Id), "step1");
                builder.AddAttribute(2, nameof(TabPanel.Title), "Start");
                builder.CloseComponent();

                builder.OpenComponent<TabPanel>(3);
                builder.AddAttribute(4, nameof(TabPanel.Id), "step2");
                builder.AddAttribute(5, nameof(TabPanel.Title), "Next");
                builder.CloseComponent();
            }));

        Assert.Single(trailTabsCut.FindAll("button[role='tab']"));
        trailTabsCut.Render(parameters => parameters.Add(tabs => tabs.ActiveTabId, "step2"));
        Assert.Equal(2, trailTabsCut.FindAll("button[role='tab']").Count);
        Assert.Single(trailTabsCut.FindAll(".trail-separator"));
    }

    [Fact]
    public void DataTable_FiltersSortsAndPinsAlwaysIncludedRowsAtBottom()
    {
        var rows = new List<WasteItem>
        {
            new("Total", "Summary", 99),
            new("Battery", "Hazardous", 20),
            new("Aluminum Can", "Metal", 10)
        };
        var columns = new List<DataTable<WasteItem>.Column>
        {
            new("Name", item => item.Name, AlwaysIncludeValue: "Total"),
            new("Category", item => item.Category, ShowQuickFilters: true),
            new("Points", item => item.Points)
        };

        var cut = Render<DataTable<WasteItem>>(parameters => parameters
            .Add(table => table.Items, rows)
            .Add(table => table.Columns, columns));

        var initialRows = cut.FindAll("tbody tr");
        Assert.Equal(3, initialRows.Count);
        Assert.Contains("Total", initialRows[2].TextContent);

        cut.FindAll("button.sort-button")[0].Click();
        var sortedRows = cut.FindAll("tbody tr");
        Assert.Contains("Aluminum Can", sortedRows[0].TextContent);
        Assert.Contains("Battery", sortedRows[1].TextContent);
        Assert.Contains("Total", sortedRows[2].TextContent);

        cut.Find("input.custom-input").Input("Battery");
        var filteredRows = cut.FindAll("tbody tr");
        Assert.Equal(2, filteredRows.Count);
        Assert.Contains("Battery", filteredRows[0].TextContent);
        Assert.Contains("Total", filteredRows[1].TextContent);
    }

    [Fact]
    public void LoadersAndScreenKeyboard_RenderAndHandleKeyInputAndMaxLength()
    {
        var spinnerCut = Render<SimpleSpinner>(parameters => parameters
            .Add(spinner => spinner.Modifier, UIModifier.New.Width(40).StrokeThickness(5)));
        Assert.Contains("--loader-size: 40px", spinnerCut.Find(".circular-loader").GetAttribute("style"));

        var orbitalCut = Render<OrbitalLoader>(parameters => parameters
            .Add(loader => loader.SatelliteCount, 6)
            .Add(loader => loader.LoadingText, "Syncing")
            .AddChildContent<Mars>(marsParameters => marsParameters
                .Add(mars => mars.MinFeatures, 4)
                .Add(mars => mars.MaxFeatures, 6)));

        Assert.Equal(6, orbitalCut.FindAll(".sat-orbit").Count);
        Assert.Equal("Syncing", orbitalCut.Find(".spinner-text").TextContent);
        Assert.InRange(orbitalCut.FindAll(".planet > .crater, .planet > .building-base").Count, 1, 6);

        var typedValue = string.Empty;
        var keyboardCut = Render<ScreenKeyboard>(parameters => parameters
            .Add(keyboard => keyboard.Setup, KeyboardSetup.LettersOnly)
            .Add(keyboard => keyboard.MaxLength, 2)
            .Add(keyboard => keyboard.Value, typedValue)
            .Add(keyboard => keyboard.ValueChanged, value => typedValue = value ?? string.Empty));

        keyboardCut.FindAll("button.action-btn").First(button => string.Equals(button.TextContent.Trim(), "Q", StringComparison.OrdinalIgnoreCase)).Click();
        keyboardCut.FindAll("button.action-btn").First(button => string.Equals(button.TextContent.Trim(), "W", StringComparison.OrdinalIgnoreCase)).Click();
        keyboardCut.FindAll("button.action-btn").First(button => string.Equals(button.TextContent.Trim(), "E", StringComparison.OrdinalIgnoreCase)).Click();

        Assert.Equal("Qw", typedValue);

        keyboardCut.Find("button[aria-label='Backspace']").Click();
        Assert.Equal("Q", typedValue);
    }
}
