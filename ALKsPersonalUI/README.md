# ALK's Personal UI v3.3.0

A tactile, auto-adaptive desktop and touch kiosk component library for Blazor. Features 3D tactile buttons, touch-focused input handlers, on-screen keyboards, numeric keypads, bottom sheets, sliders, windows, modals, toasts, segmented controls, and procedural planetary loaders.

## 1. Installation

```bash
dotnet add package ALKsPersonalUI --version 3.3.0
```


## 2. Register Services (`Program.cs`)

Configure default button press physics, automatic touch input overlays, or register custom seed themes:

```csharp
using ALKsPersonalUI;
using ALKsPersonalUI.Core;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddPersonalUILibrary(options =>
{
    // Auto-selects 3D downward press on mouse pointer and inward scale on touch pointer
    options.DefaultPressEffect = ButtonPressEffect.Auto;

    // Automatically opens on-screen keyboard/numpad when inputs are tapped by touch
    options.DefaultTouchInputMode = TouchInputMode.Auto;

    // Register a seed theme which computes surface, header, and border colors automatically
    options.AddSeedTheme(
        name: "CustomTheme",
        accentColor: "#f97316",
        appBackground: "#16100c",
        isDark: true);
    
    options.InitialPresetName = "CustomTheme";
});
```

## 3. Add Stylesheet (`App.razor` or `index.html`)

Include the stylesheet and tap-highlight reset in the `<head>` section:

```html
<link rel="stylesheet" href="_content/ALKsPersonalUI/personal-ui.css" />
```

## 4. Add Namespace Imports (`_Imports.razor`)

Add the component namespaces to `_Imports.razor`:

```razor
@using ALKsPersonalUI
@using ALKsPersonalUI.Buttons
@using ALKsPersonalUI.Core
@using ALKsPersonalUI.Data
@using ALKsPersonalUI.Indicators
@using ALKsPersonalUI.Inputs
@using ALKsPersonalUI.Layout
@using ALKsPersonalUI.Loaders
@using ALKsPersonalUI.Loaders.Planets
@using ALKsPersonalUI.Navigation
@using ALKsPersonalUI.Overlays
@using ALKsPersonalUI.Windows
```

## 5. Configure Layout (`MainLayout.razor`)

Wrap the application content inside `<Scaffold>` to inject theme CSS variables and enable toast notifications:

```razor
@inherits LayoutComponentBase

<Scaffold Modifier="@(UIModifier.New.AlignItems(HorizontalAlignment.Center).Padding(24))">
    <TopBar>
        <Navbar Title="Application Title" BrandHref="/" />
    </TopBar>
    <ChildContent>
        <Column Modifier="@(UIModifier.New.FillMaxWidth().MaxWidth(1280))" Gap="1.5rem">
            @Body
        </Column>
    </ChildContent>
</Scaffold>
```

## 6. Version 2.0 Feature Reference

* **Auto-Adaptive Buttons (`<ActionButton>`):** Automatically switches between 3D downward translation (`Down3D`) on fine mouse pointers and inward scaling (`InwardScale`) on coarse touch pointers. Supports `HoldDurationMs` for hold-to-confirm actions and `AutoRepeat` for continuous value stepping.
* **Modular Virtual Keyboard (`<ScreenKeyboard>`):** Kiosk-safe virtual keyboard with a fluent builder API (`KeyboardSetup`). Features individual zone toggles for letters, numbers, special characters, function keys (`F1`-`F12`), system keys, navigation clusters, arrow keys, and numpads. Excludes the Windows/Meta key by design.
* **Touch Numpad (`<TouchNumpad<TValue>>`):** Generic `INumber<T>` numeric keypad supporting decimals, sign toggles, quick-add steps, and PIN entry mode (`IsPinMode`).
* **BottomSheet (`<BottomSheet>`):** Slide-up container panel optimized for touch reachability.
* **Segmented Controls (`<SegmentedControl<TItem>>`):** Touch-oriented single-select button bar for discrete options.
* **Seed Theme Generator (`UITheme.FromSeed(...)`):** Generates a complete color palette from an accent color and background color using CSS `color-mix()`.

## 7. Documentation & Showcase

Interactive documentation, live component workbenches, and code generators are hosted at:
[UILibrary.maleneogandreas.dk](https://UILibrary.maleneogandreas.dk)