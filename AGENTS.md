# ALKsPersonalUI

## Tech Stack & Hard Constraints

- **Languages & Frameworks:** C#, Razor / Blazor (`Microsoft.NET.Sdk.Razor` multi-targeting `net8.0;net9.0;net10.0`, and `Microsoft.NET.Sdk.BlazorWebAssembly` on `net10.0`).
- **No JavaScript:** Do not use JavaScript (`.js`, inline `<script>`, or JS interop) in this project.
- **Native Alternatives:** Prefer pure C# / Blazor, CSS, and HTML solutions over JavaScript.

## Solution & Project Map

- `ALKsPersonalUI.slnx` — Root solution file referencing `ALKsPersonalUI` and `ALKsPersonalUI.Showcase`.
- `ALKsPersonalUI/` (`ALKsPersonalUI.csproj`) — Razor Class Library for tactile dark-mode & touch-ready Blazor UI components:
  - `Buttons/` — Button components
  - `Core/` — Core library types and utilities
  - `Data/` — Data display components
  - `Indicators/` — Status/indicator components
  - `Inputs/` — Touch keyboard, touch numpad, validation rules, and input controls
  - `Layout/` — `Box`, `Column`, `Divider`, `Grid`, `Row`, `Scaffold`, `Spacer`, `Text`
  - `Loaders/` — `OrbitalLoader`, `SimpleSpinner`, and `Planets/` (`Mars`)
  - `Navigation/` — `BurgerMenu`, `Navbar`, `TabPanel`, `Tabs`
  - `Overlays/` — `BottomSheet`, `Modal`, `ToastContainer`, `ToastService`, `Tooltip`
  - `Windows/` — `Window`, `TitledWindow`
- `alks-ui-showcase/` (`ALKsPersonalUI.Showcase.csproj`) — Blazor WebAssembly (`net10.0`) showcase & documentation app (`Program.cs`, `App.razor`, `Layout/MainLayout.razor`, `Pages/`).
- `ALKsPersonalUI.Tests/` — Test directory.

## Ignore Rules

Never scan or index the following directories:
- `bin/`
- `obj/`
- `.vs/`
- `.git/`
- `node_modules/`
- `wwwroot/`

