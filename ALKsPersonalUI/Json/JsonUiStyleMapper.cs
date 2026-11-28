using System.Globalization;
using ALKsPersonalUI.Core;

namespace ALKsPersonalUI.Json;

/// <summary>
/// Maps allow-listed, sanitized JSON style entries onto <see cref="UIModifier"/>.
/// </summary>
public static class JsonUiStyleMapper
{
    /// <summary>
    /// Converts a JSON style dictionary into a <see cref="UIModifier"/>, ignoring unsafe or unknown keys.
    /// </summary>
    public static UIModifier MapToModifier(IReadOnlyDictionary<string, string>? style)
    {
        var modifier = UIModifier.New;
        if (style is null || style.Count == 0)
        {
            return modifier;
        }

        foreach (var (key, rawValue) in style)
        {
            ApplyStyleEntry(modifier, key, rawValue);
        }

        return modifier;
    }

    private static void ApplyStyleEntry(UIModifier modifier, string key, string? rawValue)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(rawValue))
        {
            return;
        }

        var normalizedKey = key.Trim().ToLowerInvariant();
        if (normalizedKey is "backgroundimage")
        {
            var safeUrl = JsonUiSanitizer.SanitizeImageUrl(rawValue);
            if (!string.IsNullOrEmpty(safeUrl))
            {
                modifier.BackgroundImage(safeUrl);
            }

            return;
        }

        if (!JsonUiSanitizer.IsSafeCssValue(rawValue))
        {
            return;
        }

        var value = rawValue.Trim();
        ApplyValidatedCssProperty(modifier, normalizedKey, value);
    }

    private static void ApplyValidatedCssProperty(UIModifier modifier, string normalizedKey, string value)
    {
        switch (normalizedKey)
        {
            case "width": modifier.Width(value); break;
            case "height": modifier.Height(value); break;
            case "minwidth": modifier.MinWidth(value); break;
            case "maxwidth": modifier.MaxWidth(value); break;
            case "minheight": modifier.MinHeight(value); break;
            case "maxheight": modifier.MaxHeight(value); break;
            case "padding": modifier.Padding(value); break;
            case "margin": modifier.Margin(value); break;
            case "gap": modifier.Gap(value); break;
            case "backgroundcolor": modifier.Background(value); break;
            case "bordercolor": modifier.BorderColor(value); break;
            case "borderradius": modifier.BorderRadius(value); break;
            case "textcolor": modifier.TextColor(value); break;
            case "fontsize": modifier.FontSize(value); break;
            case "fontweight": modifier.FontWeight(value); break;
            case "alignitems": modifier.AlignItems(value); break;
            case "justifycontent": modifier.JustifyContent(value); break;
            case "alignself": modifier.AlignSelf(value); break;
            case "flexgrow":
                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var growFactor))
                {
                    modifier.FlexGrow(growFactor);
                }
                break;
            case "flexwrap": modifier.FlexWrap(value); break;
            case "border": modifier.Border(value); break;
        }
    }
}
