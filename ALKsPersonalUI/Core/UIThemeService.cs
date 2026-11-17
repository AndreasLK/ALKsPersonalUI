namespace ALKsPersonalUI.Core;

public sealed class UIThemeService
{
    public event Action? OnChange;

    private readonly Dictionary<string, Func<UITheme>> _registeredPresets = new(StringComparer.OrdinalIgnoreCase);

    public UITheme Current { get; private set; } = new();

    public IReadOnlyCollection<string> RegisteredPresets => _registeredPresets.Keys;

    public UIThemeService()
    {
        RegisterPreset(ThemeDefaults.PresetNames.Default, () => new UITheme { Name = ThemeDefaults.PresetNames.Default });
        RegisterPreset(ThemeDefaults.PresetNames.Cyberpunk, CreateCyberpunkTheme);
        RegisterPreset(ThemeDefaults.PresetNames.Emerald, CreateEmeraldTheme);
        RegisterPreset(ThemeDefaults.PresetNames.NordicLight, CreateNordicLightTheme);
    }

    public void RegisterPreset(string presetName, Func<UITheme> themeFactory)
    {
        _registeredPresets[presetName] = () =>
        {
            var theme = themeFactory();
            theme.Name = presetName;
            return theme;
        };
    }

    public void RegisterPreset(string presetName, UITheme themeInstance) =>
        RegisterPreset(presetName, themeInstance.Clone);

    public void SetTheme(UITheme customTheme)
    {
        Current = customTheme.Clone();
        NotifyThemeChanged();
    }

    public void Update(Action<UITheme> applyThemeChanges)
    {
        applyThemeChanges(Current);
        NotifyThemeChanged();
    }

    public void ApplyPreset(string presetName)
    {
        if (_registeredPresets.TryGetValue(presetName, out var themeFactory))
        {
            Current = themeFactory();
            NotifyThemeChanged();
            return;
        }

        Current = new UITheme { Name = ThemeDefaults.PresetNames.Default };
        NotifyThemeChanged();
    }

    private static UITheme CreateCyberpunkTheme() => new()
    {
        Name = ThemeDefaults.PresetNames.Cyberpunk,
        ApplicationBackgroundColor = ThemeDefaults.Cyberpunk.ApplicationBackgroundColor,
        SurfaceBackgroundColor = ThemeDefaults.Cyberpunk.SurfaceBackgroundColor,
        ElevatedBackgroundColor = ThemeDefaults.Cyberpunk.ElevatedBackgroundColor,
        HeaderBackgroundColor = ThemeDefaults.Cyberpunk.HeaderBackgroundColor,
        BorderColor = ThemeDefaults.Cyberpunk.BorderColor,
        AccentColor = ThemeDefaults.Cyberpunk.AccentColor,
        ActionColor = ThemeDefaults.Cyberpunk.ActionColor,
        MediumBorderRadius = ThemeDefaults.Cyberpunk.MediumBorderRadius,
        LargeBorderRadius = ThemeDefaults.Cyberpunk.LargeBorderRadius,
        ButtonExtrusionDepth = ThemeDefaults.Cyberpunk.ButtonExtrusionDepth
    };

    private static UITheme CreateEmeraldTheme() => new()
    {
        Name = ThemeDefaults.PresetNames.Emerald,
        ApplicationBackgroundColor = ThemeDefaults.Emerald.ApplicationBackgroundColor,
        SurfaceBackgroundColor = ThemeDefaults.Emerald.SurfaceBackgroundColor,
        ElevatedBackgroundColor = ThemeDefaults.Emerald.ElevatedBackgroundColor,
        HeaderBackgroundColor = ThemeDefaults.Emerald.HeaderBackgroundColor,
        BorderColor = ThemeDefaults.Emerald.BorderColor,
        AccentColor = ThemeDefaults.Emerald.AccentColor,
        ActionColor = ThemeDefaults.Emerald.ActionColor,
        MediumBorderRadius = ThemeDefaults.Emerald.MediumBorderRadius,
        LargeBorderRadius = ThemeDefaults.Emerald.LargeBorderRadius,
        ButtonExtrusionDepth = ThemeDefaults.Emerald.ButtonExtrusionDepth
    };

    private static UITheme CreateNordicLightTheme() => new()
    {
        Name = ThemeDefaults.PresetNames.NordicLight,
        ApplicationBackgroundColor = ThemeDefaults.NordicLight.ApplicationBackgroundColor,
        SurfaceBackgroundColor = ThemeDefaults.NordicLight.SurfaceBackgroundColor,
        ElevatedBackgroundColor = ThemeDefaults.NordicLight.ElevatedBackgroundColor,
        HeaderBackgroundColor = ThemeDefaults.NordicLight.HeaderBackgroundColor,
        BorderColor = ThemeDefaults.NordicLight.BorderColor,
        TextColor = ThemeDefaults.NordicLight.TextColor,
        MutedTextColor = ThemeDefaults.NordicLight.MutedTextColor,
        InverseTextColor = ThemeDefaults.NordicLight.InverseTextColor,
        AccentColor = ThemeDefaults.NordicLight.AccentColor,
        ActionColor = ThemeDefaults.NordicLight.ActionColor
    };

    private void NotifyThemeChanged() => OnChange?.Invoke();
}