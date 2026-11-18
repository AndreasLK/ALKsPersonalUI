# ALKsPersonalUI

## Tech Stack & Hard Constraints

- **Languages & Frameworks:** C#, Razor / Blazor (`Microsoft.NET.Sdk.Razor` multi-targeting `net8.0;net9.0;net10.0`, and `Microsoft.NET.Sdk.BlazorWebAssembly` on `net10.0`).
- **No JavaScript:** Do not use JavaScript (`.js`, inline `<script>`, or JS interop) in this project.
- **Native Alternatives:** Prefer pure C# / Blazor, CSS, and HTML solutions over JavaScript.

## Solution & Project Map

- `ALKsPersonalUI.slnx` — Root solution file referencing `ALKsPersonalUI`, `ALKsPersonalUI.Showcase`, and `ALKsPersonalUI.Tests`.
- `ALKsPersonalUI/` (`ALKsPersonalUI.csproj`) — Razor Class Library for tactile dark-mode & touch-ready Blazor UI components.
- `alks-ui-showcase/` (`ALKsPersonalUI.Showcase.csproj`) — Blazor WebAssembly (`net10.0`) showcase & documentation app.
- `ALKsPersonalUI.Tests/` (`ALKsPersonalUI.Tests.csproj`) — bUnit & xUnit test project.

## Repository File Tree

```text
ALKsPersonalUI/
├── ALKsPersonalUI.slnx
├── ALKsPersonalUI/                               # Razor Class Library (net8.0;net9.0;net10.0)
│   ├── ALKsPersonalUI.csproj
│   ├── _Imports.razor
│   ├── UILibraryExtensions.cs                    # DI registration for library services
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── Buttons/                                  # Button components
│   │   ├── ActionButton.razor
│   │   ├── ActionButton.razor.css
│   │   └── CloseButton.razor
│   ├── Core/                                     # Core library types, defaults, styling, and theming
│   │   ├── Base/
│   │   │   ├── UIComponentBase.cs
│   │   │   └── WindowBase.cs
│   │   ├── Defaults/
│   │   │   ├── ButtonDefaults.cs
│   │   │   ├── DataTableDefaults.cs
│   │   │   ├── IndicatorDefaults.cs
│   │   │   ├── InputDefaults.cs
│   │   │   ├── LayoutDefaults.cs
│   │   │   ├── LoaderDefaults.cs
│   │   │   ├── NavigationDefaults.cs
│   │   │   ├── OverlayDefaults.cs
│   │   │   ├── SliderDefaults.cs
│   │   │   ├── TemplateDefaults.cs
│   │   │   ├── ThemeDefaults.cs
│   │   │   └── WindowDefaults.cs
│   │   ├── Primitives/
│   │   │   ├── GeometryMath.cs
│   │   │   ├── KeyboardKeys.cs
│   │   │   └── LayoutEnums.cs
│   │   ├── Styling/
│   │   │   ├── CssBuilder.cs
│   │   │   ├── StyleBuilder.cs
│   │   │   └── UIModifier.cs
│   │   └── Theming/
│   │       ├── UITheme.cs
│   │       └── UIThemeService.cs
│   ├── Data/                                     # Data display components
│   │   ├── DataTable.razor
│   │   ├── DataTable.razor.css
│   │   ├── Leaderboard.razor
│   │   └── Leaderboard.razor.css
│   ├── Indicators/                               # Status & indicator components
│   │   ├── CountdownTimer.razor
│   │   ├── CountdownTimer.razor.css
│   │   ├── Pill.razor
│   │   ├── Pill.razor.css
│   │   ├── ProgressBar.razor
│   │   └── ProgressBar.razor.css
│   ├── Inputs/                                   # Touch keyboard, numpad, sliders, and input controls
│   │   ├── Checkbox.razor
│   │   ├── Checkbox.razor.css
│   │   ├── ChoiceCard.razor
│   │   ├── ChoiceCard.razor.css
│   │   ├── ChoiceCardState.cs
│   │   ├── Dropdown.razor
│   │   ├── Dropdown.razor.css
│   │   ├── KeyboardSetup.cs
│   │   ├── NumberInput.razor
│   │   ├── NumberInput.razor.css
│   │   ├── RadioSlider.razor
│   │   ├── RadioSlider.razor.css
│   │   ├── RangeSlider.razor
│   │   ├── RangeSlider.razor.css
│   │   ├── ScreenKeyboard.razor
│   │   ├── ScreenKeyboard.razor.css
│   │   ├── SecretString.cs
│   │   ├── SegmentedControl.razor
│   │   ├── SliderBase.cs
│   │   ├── TextInput.razor
│   │   ├── TextInput.razor.css
│   │   ├── TouchNumpad.razor
│   │   ├── TouchNumpad.razor.css
│   │   └── ValidationRule.cs
│   ├── Layout/                                   # Layout primitives
│   │   ├── Box.razor
│   │   ├── Column.razor
│   │   ├── Divider.razor
│   │   ├── Divider.razor.css
│   │   ├── Grid.razor
│   │   ├── Image.razor
│   │   ├── Image.razor.css
│   │   ├── Row.razor
│   │   ├── Scaffold.razor
│   │   ├── Scaffold.razor.css
│   │   ├── Spacer.razor
│   │   ├── Text.razor
│   │   └── Text.razor.css
│   ├── Loaders/                                  # Loading indicators & planets
│   │   ├── OrbitalLoader.razor
│   │   ├── OrbitalLoader.razor.css
│   │   ├── SimpleSpinner.razor
│   │   ├── SimpleSpinner.razor.css
│   │   └── Planets/
│   │       ├── Mars.razor
│   │       └── Mars.razor.css
│   ├── Navigation/                               # Navigation components
│   │   ├── BurgerMenu.razor
│   │   ├── BurgerMenu.razor.css
│   │   ├── Navbar.razor
│   │   ├── Navbar.razor.css
│   │   ├── TabPanel.razor
│   │   ├── Tabs.razor
│   │   └── Tabs.razor.css
│   ├── Overlays/                                 # Modals, sheets, toasts, and tooltips
│   │   ├── BottomSheet.razor
│   │   ├── BottomSheet.razor.css
│   │   ├── Modal.razor
│   │   ├── Modal.razor.css
│   │   ├── ToastContainer.razor
│   │   ├── ToastContainer.razor.css
│   │   ├── ToastService.cs
│   │   ├── Tooltip.razor
│   │   └── Tooltip.razor.css
│   ├── Templates/                                # Kiosk & page templates
│   │   ├── AspectRatioFrame.razor
│   │   ├── AspectRatioFrame.razor.css
│   │   ├── GameTemplate.razor
│   │   ├── GameTemplate.razor.css
│   │   ├── KioskShell.razor
│   │   ├── KioskShell.razor.css
│   │   ├── QuizTemplate.razor
│   │   └── QuizTemplate.razor.css
│   └── Windows/                                  # Window containers
│       ├── TitledWindow.razor
│       ├── TitledWindow.razor.css
│       ├── Window.razor
│       └── Window.razor.css
├── alks-ui-showcase/                             # Blazor WebAssembly showcase app (net10.0)
│   ├── ALKsPersonalUI.Showcase.csproj
│   ├── _Imports.razor
│   ├── App.razor
│   ├── Program.cs
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── Layout/
│   │   └── MainLayout.razor
│   ├── Pages/
│   │   ├── ApiReferencePage.razor
│   │   ├── ButtonsPage.razor
│   │   ├── IndicatorsPage.razor
│   │   ├── InputsPage.razor
│   │   ├── LayoutWindowsPage.razor
│   │   ├── LoadersPage.razor
│   │   ├── NotFound.razor
│   │   ├── OverlaysPage.razor
│   │   ├── PlaygroundPage.razor
│   │   ├── QuickstartPage.razor
│   │   ├── SlidersPage.razor
│   │   ├── TemplatesPage.razor
│   │   ├── ThemeGeneratorPage.razor
│   │   └── TouchKeyboardPage.razor
│   └── Shared/
│       └── ShowcaseStyles.cs
└── ALKsPersonalUI.Tests/                         # bUnit & xUnit test project
    ├── ALKsPersonalUI.Tests.csproj
    ├── Components/
    │   ├── ActionButtonTests.cs
    │   ├── ImageTextAndPillTests.cs
    │   ├── InputComponentsTests.cs
    │   ├── LayoutNavigationDataAndLoadersTests.cs
    │   ├── LeaderboardTests.cs
    │   └── WindowAndOverlayTests.cs
    ├── Core/
    │   ├── GeometryAndLayoutDefaultsTests.cs
    │   ├── SecretStringKeyboardAndToastTests.cs
    │   ├── StyleAndCssBuilderTests.cs
    │   ├── UIModifierTests.cs
    │   └── UIThemeAndLibraryTests.cs
    └── TestInfrastructure/
        └── PersonalUiTestContext.cs
```

## Ignore Rules

Never scan or index the following directories:
- `bin/`
- `obj/`
- `.vs/`
- `.git/`
- `.agents/`
- `node_modules/`
- `TestResults/`
- `wwwroot/`
