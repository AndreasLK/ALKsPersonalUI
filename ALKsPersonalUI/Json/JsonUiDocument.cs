using System.Text.Json;

namespace ALKsPersonalUI.Json;

/// <summary>
/// Self-contained declarative JSON UI experience package bundling theme, initial session state, and named views.
/// </summary>
public sealed class JsonUiDocument
{
    /// <summary>
    /// Gets or sets an optional experience identifier used to detect document swaps.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets an optional human-readable title for the experience.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets an optional built-in theme preset name (for example, <c>cyberpunk</c>, <c>emerald</c>, or <c>nordic-light</c>).
    /// </summary>
    public string? ThemePreset { get; set; }

    /// <summary>
    /// Gets or sets optional theme property or CSS variable overrides applied on top of <see cref="ThemePreset"/>.
    /// </summary>
    public Dictionary<string, string>? ThemeOverrides { get; set; }

    /// <summary>
    /// Gets or sets the initial session state values loaded at startup and restored on session reset.
    /// </summary>
    public Dictionary<string, JsonElement>? InitialState { get; set; }

    /// <summary>
    /// Gets or sets the name of the starting view inside <see cref="Views"/>.
    /// </summary>
    public string? InitialView { get; set; }

    /// <summary>
    /// Gets or sets named screen trees navigable via declarative <see cref="JsonUiActionType.Navigate"/> actions.
    /// </summary>
    public Dictionary<string, JsonUiNode>? Views { get; set; }

    /// <summary>
    /// Gets or sets an optional single root node when <see cref="Views"/> is not used.
    /// </summary>
    public JsonUiNode? Root { get; set; }

    /// <summary>
    /// Resolves the initial view key, falling back to the first declared view or the default root key.
    /// </summary>
    public string ResolveInitialViewKey()
    {
        if (!string.IsNullOrWhiteSpace(InitialView) && Views is not null && Views.ContainsKey(InitialView))
        {
            return InitialView;
        }

        if (Views is { Count: > 0 })
        {
            foreach (var key in Views.Keys)
            {
                return key;
            }
        }

        return JsonUiDefaults.DefaultRootViewKey;
    }

    /// <summary>
    /// Resolves the root <see cref="JsonUiNode"/> for the requested view key, or <see cref="Root"/> as fallback.
    /// </summary>
    public JsonUiNode? ResolveViewNode(string? viewKey)
    {
        if (!string.IsNullOrWhiteSpace(viewKey)
            && Views is not null
            && Views.TryGetValue(viewKey, out var viewNode))
        {
            return viewNode;
        }

        return Root;
    }

    /// <summary>
    /// Serializes this document into JSON.
    /// </summary>
    public string ToJson(bool isIndented = true) =>
        JsonSerializer.Serialize(this, JsonUiDefaults.CreateWriteOptions(isIndented));

    /// <summary>
    /// Deserializes a <see cref="JsonUiDocument"/> from JSON with depth protection.
    /// </summary>
    public static JsonUiDocument? FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return JsonSerializer.Deserialize<JsonUiDocument>(json, JsonUiDefaults.ReadOptions);
    }
}
