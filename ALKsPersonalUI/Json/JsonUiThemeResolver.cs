using ALKsPersonalUI.Core;

namespace ALKsPersonalUI.Json;

/// <summary>
/// Resolves a <see cref="JsonUiDocument"/>'s theme preset and overrides into scoped CSS variable declarations.
/// </summary>
public static class JsonUiThemeResolver
{
    /// <summary>
    /// Resolves a document's theme configuration into inline CSS variables without mutating global <see cref="UIThemeService"/>.
    /// </summary>
    public static string ResolveScopedCssVariables(JsonUiDocument? document)
    {
        if (document is null)
        {
            return string.Empty;
        }

        var hasPreset = !string.IsNullOrWhiteSpace(document.ThemePreset);
        var hasOverrides = document.ThemeOverrides is { Count: > 0 };
        if (!hasPreset && !hasOverrides)
        {
            return string.Empty;
        }

        var theme = CreateBaseTheme(document.ThemePreset);
        ApplyOverrides(theme, document.ThemeOverrides);
        return theme.ToCssVariables();
    }

    private static UITheme CreateBaseTheme(string? presetName)
    {
        if (string.IsNullOrWhiteSpace(presetName))
        {
            return new UITheme();
        }

        // Resolve built-in presets through an isolated UIThemeService instance so DI state is never mutated.
        var isolatedService = new UIThemeService();
        isolatedService.ApplyPreset(presetName.Trim());
        return isolatedService.Current.Clone();
    }

    private static void ApplyOverrides(UITheme theme, IReadOnlyDictionary<string, string>? overrides)
    {
        if (overrides is null)
        {
            return;
        }

        foreach (var (key, rawValue) in overrides)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(rawValue))
            {
                continue;
            }

            var normalizedKey = key.Trim().ToLowerInvariant();
            var trimmedValue = rawValue.Trim();

            if (normalizedKey is "fontfamily" or "--ui-font-family")
            {
                if (!JsonUiSanitizer.IsSafeFontFamily(trimmedValue))
                {
                    continue;
                }

                theme.FontFamily = trimmedValue;
                continue;
            }

            if (!JsonUiSanitizer.IsSafeCssValue(trimmedValue))
            {
                continue;
            }

            ApplyThemeProperty(theme, normalizedKey, trimmedValue);
        }
    }

    private static void ApplyThemeProperty(UITheme theme, string normalizedKey, string value)
    {
        switch (normalizedKey)
        {
            case "applicationbackgroundcolor": theme.ApplicationBackgroundColor = value; break;
            case "surfacebackgroundcolor": theme.SurfaceBackgroundColor = value; break;
            case "elevatedbackgroundcolor": theme.ElevatedBackgroundColor = value; break;
            case "headerbackgroundcolor": theme.HeaderBackgroundColor = value; break;
            case "bordercolor": theme.BorderColor = value; break;
            case "textcolor": theme.TextColor = value; break;
            case "mutedtextcolor": theme.MutedTextColor = value; break;
            case "inversetextcolor": theme.InverseTextColor = value; break;
            case "accentcolor": theme.AccentColor = value; break;
            case "actioncolor": theme.ActionColor = value; break;
            case "successcolor": theme.SuccessColor = value; break;
            case "warningcolor": theme.WarningColor = value; break;
            case "dangercolor": theme.DangerColor = value; break;
            case "smallborderradius": theme.SmallBorderRadius = value; break;
            case "mediumborderradius": theme.MediumBorderRadius = value; break;
            case "largeborderradius": theme.LargeBorderRadius = value; break;
            default:
                if (JsonUiSanitizer.IsSafeCssVariableName(normalizedKey))
                {
                    theme.WithCustomVariable(normalizedKey, value);
                }
                break;
        }
    }
}
