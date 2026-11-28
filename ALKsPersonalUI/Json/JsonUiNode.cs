using System.Globalization;
using System.Text.Json;

namespace ALKsPersonalUI.Json;

/// <summary>
/// Serializable UI node representing a component, its properties, styling, conditions, and declarative events.
/// </summary>
public sealed class JsonUiNode
{
    /// <summary>
    /// Gets or sets an optional stable identifier used for keying stateful subtrees.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the allow-listed component type name (for example, <c>Column</c> or <c>ActionButton</c>).
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets component-specific scalar properties.
    /// </summary>
    public Dictionary<string, string>? Props { get; set; }

    /// <summary>
    /// Gets or sets optional inline text content for display components.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets an optional session state key bound two-way to an interactive component.
    /// </summary>
    public string? BindKey { get; set; }

    /// <summary>
    /// Gets or sets optional allow-listed CSS style overrides mapped onto <c>UIModifier</c>.
    /// </summary>
    public Dictionary<string, string>? Style { get; set; }

    /// <summary>
    /// Gets or sets an optional condition controlling whether this node is rendered.
    /// </summary>
    public JsonUiCondition? VisibleWhen { get; set; }

    /// <summary>
    /// Gets or sets an optional condition controlling whether an interactive node is disabled.
    /// </summary>
    public JsonUiCondition? DisabledWhen { get; set; }

    /// <summary>
    /// Gets or sets declarative action sequences keyed by event name (for example, <c>click</c> or <c>submit</c>).
    /// </summary>
    public Dictionary<string, List<JsonUiAction>>? Events { get; set; }

    /// <summary>
    /// Gets or sets named child subtrees for multi-slot containers such as <c>KioskShell</c>.
    /// </summary>
    public Dictionary<string, JsonUiNode>? Slots { get; set; }

    /// <summary>
    /// Gets or sets nested child nodes rendered inside this node.
    /// </summary>
    public List<JsonUiNode>? Children { get; set; }

    /// <summary>
    /// Reads a string property from <see cref="Props"/> or returns <paramref name="fallback"/> when absent.
    /// </summary>
    public string GetProp(string key, string fallback = "") =>
        Props is not null && Props.TryGetValue(key, out var value) && value is not null ? value : fallback;

    /// <summary>
    /// Reads a boolean property from <see cref="Props"/> or returns <paramref name="fallback"/> when absent or invalid.
    /// </summary>
    public bool GetBool(string key, bool fallback = false) =>
        Props is not null && Props.TryGetValue(key, out var value) && bool.TryParse(value, out var parsed)
            ? parsed
            : fallback;

    /// <summary>
    /// Reads an integer property from <see cref="Props"/> using invariant culture or returns <paramref name="fallback"/>.
    /// </summary>
    public int GetInt(string key, int fallback = 0) =>
        Props is not null && Props.TryGetValue(key, out var value)
        && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : fallback;

    /// <summary>
    /// Reads a double property from <see cref="Props"/> using invariant culture or returns <paramref name="fallback"/>.
    /// </summary>
    public double GetDouble(string key, double fallback = 0) =>
        Props is not null && Props.TryGetValue(key, out var value)
        && double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : fallback;

    /// <summary>
    /// Serializes this node tree into JSON.
    /// </summary>
    public string ToJson(bool indented = true) =>
        JsonSerializer.Serialize(this, JsonUiDefaults.CreateWriteOptions(indented));

    /// <summary>
    /// Deserializes a <see cref="JsonUiNode"/> from JSON, coalescing null <see cref="Type"/> values.
    /// </summary>
    public static JsonUiNode? FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        var node = JsonSerializer.Deserialize<JsonUiNode>(json, JsonUiDefaults.ReadOptions);
        if (node is not null)
        {
            // Guard against explicit JSON `"type": null` bypassing the C# property initializer.
            node.Type ??= string.Empty;
        }

        return node;
    }
}
