using System.Text.Json;
using System.Text.Json.Serialization;

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

    public string ApplicationBackgroundColor { get; set; } = ThemeDefaults.DarkDefault.ApplicationBackgroundColor;
    public string SurfaceBackgroundColor { get; set; } = ThemeDefaults.DarkDefault.SurfaceBackgroundColor;
    public string ElevatedBackgroundColor { get; set; } = ThemeDefaults.DarkDefault.ElevatedBackgroundColor;
    public string HeaderBackgroundColor { get; set; } = ThemeDefaults.DarkDefault.HeaderBackgroundColor;
    public string BorderColor { get; set; } = ThemeDefaults.DarkDefault.BorderColor;

    public string TextColor { get; set; } = ThemeDefaults.DarkDefault.TextColor;
    public string MutedTextColor { get; set; } = ThemeDefaults.DarkDefault.MutedTextColor;
    public string InverseTextColor { get; set; } = ThemeDefaults.DarkDefault.InverseTextColor;

    public string AccentColor { get; set; } = ThemeDefaults.DarkDefault.AccentColor;
    public string ActionColor { get; set; } = ThemeDefaults.DarkDefault.ActionColor;
    public string SuccessColor { get; set; } = ThemeDefaults.DarkDefault.SuccessColor;
    public string WarningColor { get; set; } = ThemeDefaults.DarkDefault.WarningColor;
    public string DangerColor { get; set; } = ThemeDefaults.DarkDefault.DangerColor;

    public string SmallBorderRadius { get; set; } = ThemeDefaults.DarkDefault.SmallBorderRadius;
    public string MediumBorderRadius { get; set; } = ThemeDefaults.DarkDefault.MediumBorderRadius;
    public string LargeBorderRadius { get; set; } = ThemeDefaults.DarkDefault.LargeBorderRadius;
    public string ButtonExtrusionDepth { get; set; } = ThemeDefaults.DarkDefault.ButtonExtrusionDepth;
    public string PressAnimationDuration { get; set; } = ThemeDefaults.DarkDefault.PressAnimationDuration;

    public string InwardPressScaleFactor { get; set; } = ThemeDefaults.DarkDefault.InwardPressScaleFactor;
    public string MinimumTouchTargetSize { get; set; } = ThemeDefaults.DarkDefault.MinimumTouchTargetSize;
    public string FontFamily { get; set; } = "sans-serif";
    public ButtonPressEffect DefaultPressEffect { get; set; } = ButtonPressEffect.Auto;
    public TouchInputMode DefaultTouchInputMode { get; set; } = TouchInputMode.None;

    private Dictionary<string, string> _customCssVariables = new();

    public static UITheme FromSeed(
        string name,
        string accentColor,
        string applicationBackgroundColor = "#121316",
        string? actionColor = null,
        bool isDarkTheme = true,
        ButtonPressEffect defaultPressEffect = ButtonPressEffect.Auto)
    {
        var contrastMixColor = isDarkTheme ? "white" : "black";
        return new UITheme
        {
            Name = name,
            ApplicationBackgroundColor = applicationBackgroundColor,
            HeaderBackgroundColor = $"color-mix(in srgb, {applicationBackgroundColor}, {contrastMixColor} 4%)",
            ElevatedBackgroundColor = $"color-mix(in srgb, {applicationBackgroundColor}, {contrastMixColor} 8%)",
            SurfaceBackgroundColor = $"color-mix(in srgb, {applicationBackgroundColor}, {contrastMixColor} 12%)",
            BorderColor = $"color-mix(in srgb, {applicationBackgroundColor}, {contrastMixColor} 20%)",
            TextColor = isDarkTheme ? "#ffffff" : "#0f172a",
            MutedTextColor = $"color-mix(in srgb, {(isDarkTheme ? "#ffffff" : "#0f172a")}, transparent 38%)",
            InverseTextColor = isDarkTheme ? "#111111" : "#ffffff",
            AccentColor = accentColor,
            ActionColor = actionColor ?? accentColor,
            DefaultPressEffect = defaultPressEffect
        };
    }

    public UITheme WithCustomVariable(string cssVariableName, string variableValue)
    {
        var normalizedCssVariableName = cssVariableName.StartsWith("--") ? cssVariableName : $"--{cssVariableName}";
        _customCssVariables[normalizedCssVariableName] = variableValue;
        return this;
    }

    public string ToCssVariables()
    {
        var styleBuilder = new StyleBuilder()
            .Add("--ui-bg-app", ApplicationBackgroundColor)
            .Add("--ui-bg-surface", SurfaceBackgroundColor)
            .Add("--ui-bg-elevated", ElevatedBackgroundColor)
            .Add("--ui-bg-header", HeaderBackgroundColor)
            .Add("--ui-border", BorderColor)
            .Add("--ui-text", TextColor)
            .Add("--ui-text-muted", MutedTextColor)
            .Add("--ui-text-inverse", InverseTextColor)
            .Add("--ui-accent", AccentColor)
            .Add("--ui-action", ActionColor)
            .Add("--ui-success", SuccessColor)
            .Add("--ui-warning", WarningColor)
            .Add("--ui-danger", DangerColor)
            .Add("--ui-radius-sm", SmallBorderRadius)
            .Add("--ui-radius-md", MediumBorderRadius)
            .Add("--ui-radius-lg", LargeBorderRadius)
            .Add("--ui-radius-pill", ThemeDefaults.DarkDefault.PillBorderRadius)
            .Add("--ui-depth", ButtonExtrusionDepth)
            .Add("--ui-anim-speed", PressAnimationDuration)
            .Add("--ui-press-scale", InwardPressScaleFactor)
            .Add("--ui-touch-target", MinimumTouchTargetSize)
            .Add("--ui-font-family", FontFamily);

        foreach (var (variableName, variableValue) in _customCssVariables)
            styleBuilder.Add(variableName, variableValue);

        return styleBuilder.Build();
    }

    public UITheme Clone()
    {
        var clonedTheme = (UITheme)MemberwiseClone();
        clonedTheme._customCssVariables = new Dictionary<string, string>(_customCssVariables);
        return clonedTheme;
    }

    public string ToJson(bool indented = true)
    {
        var options = CreateJsonOptions(indented);
        return JsonSerializer.Serialize(this, options);
    }

    public static UITheme FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new UITheme();
        }

        try
        {
            var options = CreateJsonOptions(indented: false);
            var parsed = JsonSerializer.Deserialize<UITheme>(json, options);
            return parsed ?? new UITheme();
        }
        catch (JsonException)
        {
            return new UITheme();
        }
    }

    private static JsonSerializerOptions CreateJsonOptions(bool indented)
    {
        return new JsonSerializerOptions
        {
            WriteIndented = indented,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }
}