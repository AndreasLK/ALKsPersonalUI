namespace ALKsPersonalUI.Core;

public sealed class UIThemeService
{
    public event Action? OnChange;

    private readonly Dictionary<string, Func<UITheme>> _presets = new(StringComparer.OrdinalIgnoreCase);

    public UITheme Current { get; private set; } = new();

    public IReadOnlyCollection<string> RegisteredPresets => _presets.Keys;

    public UIThemeService()
    {
        RegisterPreset(ThemeDefaults.PresetNames.Default, () => new UITheme { Name = ThemeDefaults.PresetNames.Default });
        RegisterPreset(ThemeDefaults.PresetNames.Cyberpunk, CreateCyberpunkTheme);
        RegisterPreset(ThemeDefaults.PresetNames.Emerald, CreateEmeraldTheme);
        RegisterPreset(ThemeDefaults.PresetNames.NordicLight, CreateNordicLightTheme);
    }

    public void RegisterPreset(string presetName, Func<UITheme> factory)
    {
        _presets[presetName] = () =>
        {
            var theme = factory();
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
        if (_presets.TryGetValue(presetName, out var factory))
        {
            Current = factory();
            NotifyThemeChanged();
            return;
        }

        Current = new UITheme { Name = ThemeDefaults.PresetNames.Default };
        NotifyThemeChanged();
    }

    private static UITheme CreateCyberpunkTheme() => new()
    {
        Name = ThemeDefaults.PresetNames.Cyberpunk,
        AppBg = ThemeDefaults.Cyberpunk.AppBackground,
        SurfaceBg = ThemeDefaults.Cyberpunk.SurfaceBackground,
        ElevatedBg = ThemeDefaults.Cyberpunk.ElevatedBackground,
        HeaderBg = ThemeDefaults.Cyberpunk.HeaderBackground,
        BorderColor = ThemeDefaults.Cyberpunk.BorderColor,
        AccentColor = ThemeDefaults.Cyberpunk.AccentColor,
        ActionColor = ThemeDefaults.Cyberpunk.ActionColor,
        RadiusMd = ThemeDefaults.Cyberpunk.MediumRadius,
        RadiusLg = ThemeDefaults.Cyberpunk.LargeRadius,
        Depth = ThemeDefaults.Cyberpunk.TactileDepth
    };

    private static UITheme CreateEmeraldTheme() => new()
    {
        Name = ThemeDefaults.PresetNames.Emerald,
        AppBg = ThemeDefaults.Emerald.AppBackground,
        SurfaceBg = ThemeDefaults.Emerald.SurfaceBackground,
        ElevatedBg = ThemeDefaults.Emerald.ElevatedBackground,
        HeaderBg = ThemeDefaults.Emerald.HeaderBackground,
        BorderColor = ThemeDefaults.Emerald.BorderColor,
        AccentColor = ThemeDefaults.Emerald.AccentColor,
        ActionColor = ThemeDefaults.Emerald.ActionColor,
        RadiusMd = ThemeDefaults.Emerald.MediumRadius,
        RadiusLg = ThemeDefaults.Emerald.LargeRadius,
        Depth = ThemeDefaults.Emerald.TactileDepth
    };

    private static UITheme CreateNordicLightTheme() => new()
    {
        Name = ThemeDefaults.PresetNames.NordicLight,
        AppBg = ThemeDefaults.NordicLight.AppBackground,
        SurfaceBg = ThemeDefaults.NordicLight.SurfaceBackground,
        ElevatedBg = ThemeDefaults.NordicLight.ElevatedBackground,
        HeaderBg = ThemeDefaults.NordicLight.HeaderBackground,
        BorderColor = ThemeDefaults.NordicLight.BorderColor,
        TextColor = ThemeDefaults.NordicLight.TextColor,
        TextMuted = ThemeDefaults.NordicLight.TextMuted,
        TextInverse = ThemeDefaults.NordicLight.TextInverse,
        AccentColor = ThemeDefaults.NordicLight.AccentColor,
        ActionColor = ThemeDefaults.NordicLight.ActionColor
    };

    private void NotifyThemeChanged() => OnChange?.Invoke();
}