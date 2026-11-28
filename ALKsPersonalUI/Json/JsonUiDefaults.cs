using System.Text.Json;
using System.Text.Json.Serialization;

namespace ALKsPersonalUI.Json;

/// <summary>
/// Centralizes compile-time constants, event names, URL allow-lists, and serialization options for the JSON UI runtime.
/// </summary>
public static class JsonUiDefaults
{
    /// <summary>
    /// Maximum JSON nesting depth permitted during deserialization to guard against stack-overflow payloads.
    /// </summary>
    public const int MaxJsonDepth = 32;

    /// <summary>
    /// Maximum node tree depth permitted during recursive rendering.
    /// </summary>
    public const int MaxRenderDepth = 24;

    /// <summary>
    /// Minimum column count allowed for a rendered grid.
    /// </summary>
    public const int MinGridColumns = 1;

    /// <summary>
    /// Default fallback view key when a document specifies a <c>Root</c> without named <c>Views</c>.
    /// </summary>
    public const string DefaultRootViewKey = "root";

    /// <summary>
    /// Opening delimiter for single-pass session-state interpolation tokens.
    /// </summary>
    public const string InterpolationOpenToken = "{{";

    /// <summary>
    /// Closing delimiter for single-pass session-state interpolation tokens.
    /// </summary>
    public const string InterpolationCloseToken = "}}";

    /// <summary>
    /// Ephemeral interpolation token representing the value emitted by an input submission or change event.
    /// </summary>
    public const string EventValueToken = "$value";

    /// <summary>
    /// Standard event key for click interactions.
    /// </summary>
    public const string EventClick = "click";

    /// <summary>
    /// Standard event key for keyboard or form submissions.
    /// </summary>
    public const string EventSubmit = "submit";

    /// <summary>
    /// Standard event key for countdown completion.
    /// </summary>
    public const string EventCompleted = "completed";

    /// <summary>
    /// Standard event key for kiosk idle timeout notifications.
    /// </summary>
    public const string EventIdleTimeout = "idleTimeout";

    /// <summary>
    /// Standard event key for value changes on bound inputs.
    /// </summary>
    public const string EventChange = "change";

    /// <summary>
    /// Allowed HTTPS scheme prefix for external image URLs.
    /// </summary>
    public const string HttpsSchemePrefix = "https://";

    /// <summary>
    /// Allowed single-leading-slash prefix for relative local asset URLs.
    /// </summary>
    public const string RelativePathPrefix = "/";

    /// <summary>
    /// Disallowed protocol-relative prefix that mimics a relative path.
    /// </summary>
    public const string ProtocolRelativePrefix = "//";

    /// <summary>
    /// Default gap applied to column containers when unspecified.
    /// </summary>
    public const string DefaultColumnGap = "1rem";

    /// <summary>
    /// Default gap applied to row containers when unspecified.
    /// </summary>
    public const string DefaultRowGap = "0.75rem";

    /// <summary>
    /// Default border color for frame boxes when unspecified.
    /// </summary>
    public const string DefaultFrameBorderColor = "var(--ui-accent)";

    /// <summary>
    /// Default padding for frame boxes when unspecified.
    /// </summary>
    public const string DefaultFramePadding = "1rem";

    /// <summary>
    /// Default aspect width for aspect-ratio frames.
    /// </summary>
    public const int DefaultAspectWidth = 16;

    /// <summary>
    /// Default aspect height for aspect-ratio frames.
    /// </summary>
    public const int DefaultAspectHeight = 9;

    /// <summary>
    /// Default progress bar value when unspecified.
    /// </summary>
    public const double DefaultProgressValue = 60;

    /// <summary>
    /// Default progress bar maximum value when unspecified.
    /// </summary>
    public const double DefaultProgressMax = 100;

    /// <summary>
    /// Default countdown duration in seconds when unspecified.
    /// </summary>
    public const int DefaultCountdownSeconds = 30;

    /// <summary>
    /// Default row count for multi-line text areas.
    /// </summary>
    public const int DefaultTextAreaRows = 3;

    /// <summary>
    /// Shared <see cref="JsonSerializerOptions"/> for reading JSON UI documents and nodes.
    /// </summary>
    public static JsonSerializerOptions ReadOptions { get; } = new()
    {
        PropertyNameCaseInsensitive = true,
        MaxDepth = MaxJsonDepth,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) }
    };

    /// <summary>
    /// Shared <see cref="JsonSerializerOptions"/> for writing JSON UI documents and nodes.
    /// </summary>
    public static JsonSerializerOptions CreateWriteOptions(bool isIndented) => new()
    {
        WriteIndented = isIndented,
        MaxDepth = MaxJsonDepth,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) }
    };
}
