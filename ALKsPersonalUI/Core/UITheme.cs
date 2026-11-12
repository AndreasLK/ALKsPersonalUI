namespace ALKsPersonalUI.Core;

public enum ButtonPressEffect
{
    Auto,         // Automatically uses Down3D for mouse and InwardScale for touch
    Down3D,       // Classic desktop 3D downward press
    InwardScale,  // Touch inward squish
    ConcaveInset  // Touch inward squish + deep inner shadow
}

public enum TouchInputMode
{
    None,   // Standard input behavior (uses physical or OS keyboard)
    Auto,   // Automatically opens built-in touch keyboard/numpad when tapped with a finger
    Always  // Always opens built-in touch keyboard/numpad on click or tap
}

public sealed class UITheme
{
    public string Name { get; set; } = ThemeDefaults.PresetNames.Default;

    public string AppBg { get; set; } = ThemeDefaults.DarkDefault.AppBackground;
    public string SurfaceBg { get; set; } = ThemeDefaults.DarkDefault.SurfaceBackground;
    public string ElevatedBg { get; set; } = ThemeDefaults.DarkDefault.ElevatedBackground;
    public string HeaderBg { get; set; } = ThemeDefaults.DarkDefault.HeaderBackground;
    public string BorderColor { get; set; } = ThemeDefaults.DarkDefault.BorderColor;

    public string TextColor { get; set; } = ThemeDefaults.DarkDefault.TextColor;
    public string TextMuted { get; set; } = ThemeDefaults.DarkDefault.TextMuted;
    public string TextInverse { get; set; } = ThemeDefaults.DarkDefault.TextInverse;

    public string AccentColor { get; set; } = ThemeDefaults.DarkDefault.AccentColor;
    public string ActionColor { get; set; } = ThemeDefaults.DarkDefault.ActionColor;
    public string SuccessColor { get; set; } = ThemeDefaults.DarkDefault.SuccessColor;
    public string WarningColor { get; set; } = ThemeDefaults.DarkDefault.WarningColor;
    public string DangerColor { get; set; } = ThemeDefaults.DarkDefault.DangerColor;

    public string RadiusSm { get; set; } = ThemeDefaults.DarkDefault.SmallRadius;
    public string RadiusMd { get; set; } = ThemeDefaults.DarkDefault.MediumRadius;
    public string RadiusLg { get; set; } = ThemeDefaults.DarkDefault.LargeRadius;
    public string Depth { get; set; } = ThemeDefaults.DarkDefault.TactileDepth;
    public string AnimSpeed { get; set; } = ThemeDefaults.DarkDefault.AnimationSpeed;

    public string PressScale { get; set; } = "0.93";
    public string TouchTargetMin { get; set; } = "2.75rem";
    public ButtonPressEffect DefaultPressEffect { get; set; } = ButtonPressEffect.Auto;
    public TouchInputMode DefaultTouchInputMode { get; set; } = TouchInputMode.None;

    private Dictionary<string, string> _customVariables = new();

    public static UITheme FromSeed(
        string name,
        string accentColor,
        string appBackground = "#121316",
        string? actionColor = null,
        bool isDark = true,
        ButtonPressEffect pressEffect = ButtonPressEffect.Auto)
    {
        var mixTarget = isDark ? "white" : "black";
        return new UITheme
        {
            Name = name,
            AppBg = appBackground,
            HeaderBg = $"color-mix(in srgb, {appBackground}, {mixTarget} 4%)",
            ElevatedBg = $"color-mix(in srgb, {appBackground}, {mixTarget} 8%)",
            SurfaceBg = $"color-mix(in srgb, {appBackground}, {mixTarget} 12%)",
            BorderColor = $"color-mix(in srgb, {appBackground}, {mixTarget} 20%)",
            TextColor = isDark ? "#ffffff" : "#0f172a",
            TextMuted = $"color-mix(in srgb, {(isDark ? "#ffffff" : "#0f172a")}, transparent 38%)",
            TextInverse = isDark ? "#111111" : "#ffffff",
            AccentColor = accentColor,
            ActionColor = actionColor ?? accentColor,
            DefaultPressEffect = pressEffect
        };
    }

    public UITheme WithCustomVariable(string cssVariableName, string value)
    {
        var formattedName = cssVariableName.StartsWith("--") ? cssVariableName : $"--{cssVariableName}";
        _customVariables[formattedName] = value;
        return this;
    }

    public string ToCssVariables()
    {
        var builder = new StyleBuilder()
            .Add("--ui-bg-app", AppBg)
            .Add("--ui-bg-surface", SurfaceBg)
            .Add("--ui-bg-elevated", ElevatedBg)
            .Add("--ui-bg-header", HeaderBg)
            .Add("--ui-border", BorderColor)
            .Add("--ui-text", TextColor)
            .Add("--ui-text-muted", TextMuted)
            .Add("--ui-text-inverse", TextInverse)
            .Add("--ui-accent", AccentColor)
            .Add("--ui-action", ActionColor)
            .Add("--ui-success", SuccessColor)
            .Add("--ui-warning", WarningColor)
            .Add("--ui-danger", DangerColor)
            .Add("--ui-radius-sm", RadiusSm)
            .Add("--ui-radius-md", RadiusMd)
            .Add("--ui-radius-lg", RadiusLg)
            .Add("--ui-radius-pill", ThemeDefaults.DarkDefault.PillRadius)
            .Add("--ui-depth", Depth)
            .Add("--ui-anim-speed", AnimSpeed)
            .Add("--ui-press-scale", PressScale)
            .Add("--ui-touch-target", TouchTargetMin);

        foreach (var (key, val) in _customVariables)
            builder.Add(key, val);

        return builder.Build();
    }

    public UITheme Clone()
    {
        var copy = (UITheme)MemberwiseClone();
        copy._customVariables = new Dictionary<string, string>(_customVariables);
        return copy;
    }
}