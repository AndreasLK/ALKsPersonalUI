using System.Globalization;
using System.Text;
using System.Text.Json;

namespace ALKsPersonalUI.Json;

/// <summary>
/// Scoped in-memory session state store for a running JSON UI experience.
/// </summary>
public sealed class JsonUiSessionState
{
    private readonly Dictionary<string, object?> _values = new(StringComparer.Ordinal);
    private readonly Dictionary<string, DateTimeOffset> _markedTimes = new(StringComparer.Ordinal);
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initializes a new <see cref="JsonUiSessionState"/> using an optional <see cref="TimeProvider"/>.
    /// </summary>
    public JsonUiSessionState(TimeProvider? timeProvider = null)
    {
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    /// <summary>
    /// Gets the currently active view key.
    /// </summary>
    public string ActiveView { get; private set; } = JsonUiDefaults.DefaultRootViewKey;

    /// <summary>
    /// Gets a monotonically increasing counter incremented on every session reset to force stateful child remounts.
    /// </summary>
    public int SessionGeneration { get; private set; }

    /// <summary>
    /// Resets all session values, marked timestamps, and active view from <paramref name="document"/>.
    /// </summary>
    public void Reset(JsonUiDocument? document)
    {
        _values.Clear();
        _markedTimes.Clear();
        SessionGeneration++;
        ActiveView = document?.ResolveInitialViewKey() ?? JsonUiDefaults.DefaultRootViewKey;

        if (document?.InitialState is null)
        {
            return;
        }

        foreach (var (key, element) in document.InitialState)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                _values[key] = NormalizeJsonElement(element);
            }
        }
    }

    /// <summary>
    /// Updates <see cref="ActiveView"/> to <paramref name="viewKey"/>.
    /// </summary>
    public void NavigateTo(string viewKey)
    {
        if (!string.IsNullOrWhiteSpace(viewKey))
        {
            ActiveView = viewKey;
        }
    }

    /// <summary>
    /// Attempts to retrieve the raw normalized state value for <paramref name="key"/>.
    /// </summary>
    public bool TryGetValue(string key, out object? value) =>
        _values.TryGetValue(key, out value);

    /// <summary>
    /// Returns the invariant string representation of <paramref name="key"/>, or <paramref name="fallback"/> when missing/null.
    /// </summary>
    public string GetString(string key, string fallback = "") =>
        _values.TryGetValue(key, out var value) && value is not null
            ? FormatValue(value)
            : fallback;

    /// <summary>
    /// Returns the boolean interpretation of <paramref name="key"/>, or <paramref name="fallback"/> when missing/invalid.
    /// </summary>
    public bool GetBool(string key, bool fallback = false)
    {
        if (!_values.TryGetValue(key, out var value) || value is null)
        {
            return fallback;
        }

        return value switch
        {
            bool flag => flag,
            double number => number != 0,
            string text when bool.TryParse(text, out var parsed) => parsed,
            _ => fallback
        };
    }

    /// <summary>
    /// Returns the numeric value of <paramref name="key"/>, or <paramref name="fallback"/> when missing/non-numeric.
    /// </summary>
    public double GetDouble(string key, double fallback = 0) =>
        _values.TryGetValue(key, out var value) && TryConvertToDouble(value, out var number)
            ? number
            : fallback;

    /// <summary>
    /// Returns the integer value of <paramref name="key"/>, or <paramref name="fallback"/> when missing/non-numeric.
    /// </summary>
    public int GetInt(string key, int fallback = 0) =>
        _values.TryGetValue(key, out var value) && TryConvertToDouble(value, out var number)
            ? (int)Math.Round(number, MidpointRounding.AwayFromZero)
            : fallback;

    /// <summary>
    /// Stores a normalized value under <paramref name="key"/>.
    /// </summary>
    public void SetValue(string key, object? value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        _values[key] = value is JsonElement element ? NormalizeJsonElement(element) : value;
    }

    /// <summary>
    /// Increments a numeric state key by <paramref name="delta"/>, initializing from zero if absent.
    /// </summary>
    public void Increment(string key, double delta)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        var current = GetDouble(key, 0);
        // Round to 6 decimal places to avoid IEEE-754 drift (e.g. 0.1 + 0.2) in equality checks.
        _values[key] = Math.Round(current + delta, 6, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Flips a boolean state key between true and false.
    /// </summary>
    public void Toggle(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        _values[key] = !GetBool(key);
    }

    /// <summary>
    /// Records the current UTC timestamp from <see cref="TimeProvider"/> under <paramref name="key"/>.
    /// </summary>
    public void MarkTime(string key)
    {
        if (!string.IsNullOrWhiteSpace(key))
        {
            _markedTimes[key] = _timeProvider.GetUtcNow();
        }
    }

    /// <summary>
    /// Computes whole elapsed seconds since <paramref name="sourceKey"/> was marked and writes it to <paramref name="targetKey"/>.
    /// </summary>
    public void ComputeElapsedSeconds(string sourceKey, string targetKey)
    {
        if (string.IsNullOrWhiteSpace(sourceKey) || string.IsNullOrWhiteSpace(targetKey))
        {
            return;
        }

        if (!_markedTimes.TryGetValue(sourceKey, out var startedAt))
        {
            _values[targetKey] = 0d;
            return;
        }

        var elapsed = Math.Max(0, (int)Math.Floor((_timeProvider.GetUtcNow() - startedAt).TotalSeconds));
        _values[targetKey] = (double)elapsed;
    }

    /// <summary>
    /// Performs single-pass <c>{{key}}</c> interpolation against session state without recursive expansion.
    /// </summary>
    public string Interpolate(string? template, string? eventValue = null)
    {
        if (string.IsNullOrEmpty(template) || !template.Contains(JsonUiDefaults.InterpolationOpenToken, StringComparison.Ordinal))
        {
            return template ?? string.Empty;
        }

        var builder = new StringBuilder(template.Length);
        var cursor = 0;

        while (cursor < template.Length)
        {
            var openIndex = template.IndexOf(JsonUiDefaults.InterpolationOpenToken, cursor, StringComparison.Ordinal);
            if (openIndex < 0)
            {
                builder.Append(template, cursor, template.Length - cursor);
                break;
            }

            builder.Append(template, cursor, openIndex - cursor);
            var tokenStart = openIndex + JsonUiDefaults.InterpolationOpenToken.Length;
            var closeIndex = template.IndexOf(JsonUiDefaults.InterpolationCloseToken, tokenStart, StringComparison.Ordinal);
            if (closeIndex < 0)
            {
                builder.Append(template, openIndex, template.Length - openIndex);
                break;
            }

            var tokenKey = template[tokenStart..closeIndex].Trim();
            builder.Append(ResolveTokenValue(tokenKey, eventValue));
            cursor = closeIndex + JsonUiDefaults.InterpolationCloseToken.Length;
        }

        return builder.ToString();
    }

    /// <summary>
    /// Converts a <see cref="JsonElement"/> into a canonical CLR primitive (<c>string</c>, <c>double</c>, <c>bool</c>, or <c>null</c>).
    /// </summary>
    public static object? NormalizeJsonElement(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.String => element.GetString(),
        JsonValueKind.Number when element.TryGetDouble(out var number) => number,
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Null or JsonValueKind.Undefined => null,
        _ => element.GetRawText()
    };

    /// <summary>
    /// Attempts to coerce a normalized value to a finite <see cref="double"/>.
    /// </summary>
    public static bool TryConvertToDouble(object? value, out double number)
    {
        switch (value)
        {
            case double d when !double.IsNaN(d) && !double.IsInfinity(d):
                number = d;
                return true;
            case int i:
                number = i;
                return true;
            case string text when double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
                && !double.IsNaN(parsed) && !double.IsInfinity(parsed):
                number = parsed;
                return true;
            default:
                number = 0;
                return false;
        }
    }

    /// <summary>
    /// Formats a normalized CLR value using invariant culture.
    /// </summary>
    public static string FormatValue(object? value) => value switch
    {
        null => string.Empty,
        bool flag => flag ? "true" : "false",
        double number => number.ToString(CultureInfo.InvariantCulture),
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString() ?? string.Empty
    };

    private string ResolveTokenValue(string tokenKey, string? eventValue)
    {
        if (string.Equals(tokenKey, JsonUiDefaults.EventValueToken, StringComparison.Ordinal))
        {
            return eventValue ?? string.Empty;
        }

        return GetString(tokenKey);
    }
}
