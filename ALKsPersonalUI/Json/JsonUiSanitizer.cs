using System.Text.RegularExpressions;

namespace ALKsPersonalUI.Json;

/// <summary>
/// Provides compile-time OWASP allow-list validation for URLs and CSS property values in JSON UI documents.
/// </summary>
public static partial class JsonUiSanitizer
{
    private static readonly string[] AllowedDataImagePrefixes =
    [
        "data:image/png;base64,",
        "data:image/jpeg;base64,",
        "data:image/jpg;base64,",
        "data:image/gif;base64,",
        "data:image/webp;base64,"
    ];

    private static readonly Regex Base64PayloadPattern =
        new("^[A-Za-z0-9+/=]+$", RegexOptions.Compiled);

    private static readonly Regex SafeCssValuePattern =
        new(@"^[A-Za-z0-9\s\.\,\#\%\(\)\-\+\/]+$", RegexOptions.Compiled);

    private static readonly Regex SafeCssVariableNamePattern =
        new(@"^--[A-Za-z0-9\-]+$", RegexOptions.Compiled);

    private static readonly Regex UnquotedFontTokenPattern =
        new(@"^[A-Za-z0-9][A-Za-z0-9 \-]*$", RegexOptions.Compiled);

    private static readonly Regex SingleQuotedFontTokenPattern =
        new(@"^'[A-Za-z0-9 \-]+'$", RegexOptions.Compiled);

    private static readonly Regex SafeCssVarRefPattern =
        new(@"^var\(--[A-Za-z0-9\-]+\)$", RegexOptions.Compiled);

    /// <summary>
    /// Returns the trimmed URL if it matches an allow-listed safe image scheme; otherwise returns an empty string.
    /// </summary>
    public static string SanitizeImageUrl(string? rawUrl)
    {
        if (string.IsNullOrWhiteSpace(rawUrl))
        {
            return string.Empty;
        }

        var trimmed = rawUrl.Trim();
        // Check data:image/...;base64,... first because its MIME header legitimately contains ';'.
        if (IsSafeDataImageUri(trimmed))
        {
            return trimmed;
        }

        if (ContainsUnsafeControlOrCssBreakoutChars(trimmed))
        {
            return string.Empty;
        }

        if (IsSafeHttpsUrl(trimmed) || IsSafeRelativePath(trimmed))
        {
            return trimmed;
        }

        return string.Empty;
    }

    /// <summary>
    /// Validates a CSS property value against an allow-list to block inline style injection.
    /// </summary>
    public static bool IsSafeCssValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var trimmed = value.Trim();
        // Reject CSS functions that can trigger network requests or script evaluation.
        if (trimmed.Contains("url(", StringComparison.OrdinalIgnoreCase)
            || trimmed.Contains("expression(", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return SafeCssValuePattern.IsMatch(trimmed);
    }

    /// <summary>
    /// Validates a CSS custom property name (for example, <c>--ui-accent</c>) against a strict identifier allow-list.
    /// </summary>
    public static bool IsSafeCssVariableName(string? name) =>
        !string.IsNullOrWhiteSpace(name) && SafeCssVariableNamePattern.IsMatch(name.Trim());

    /// <summary>
    /// Validates a CSS <c>font-family</c> list, allowing unquoted names, balanced single-quoted font names, and <c>var(--...)</c> tokens without newlines or breakout characters.
    /// </summary>
    public static bool IsSafeFontFamily(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var trimmed = value.Trim();
        if (trimmed.Contains('\r') || trimmed.Contains('\n'))
        {
            return false;
        }

        var tokens = trimmed.Split(',', StringSplitOptions.TrimEntries);
        if (tokens.Length == 0)
        {
            return false;
        }

        foreach (var token in tokens)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            if (!UnquotedFontTokenPattern.IsMatch(token)
                && !SingleQuotedFontTokenPattern.IsMatch(token)
                && !SafeCssVarRefPattern.IsMatch(token))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsSafeHttpsUrl(string url) =>
        url.StartsWith(JsonUiDefaults.HttpsSchemePrefix, StringComparison.OrdinalIgnoreCase);

    private static bool IsSafeRelativePath(string url) =>
        url.StartsWith(JsonUiDefaults.RelativePathPrefix, StringComparison.Ordinal)
        && !url.StartsWith(JsonUiDefaults.ProtocolRelativePrefix, StringComparison.Ordinal)
        && !url.StartsWith("/\\", StringComparison.Ordinal);

    private static bool IsSafeDataImageUri(string url)
    {
        foreach (var prefix in AllowedDataImagePrefixes)
        {
            if (!url.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var payload = url[prefix.Length..];
            return payload.Length > 0 && Base64PayloadPattern.IsMatch(payload);
        }

        return false;
    }

    private static bool ContainsUnsafeControlOrCssBreakoutChars(string value)
    {
        foreach (var ch in value)
        {
            if (char.IsControl(ch) || ch is '\'' or '"' or '(' or ')' or ';' or '\\' or '<' or '>')
            {
                return true;
            }
        }

        return false;
    }
}
