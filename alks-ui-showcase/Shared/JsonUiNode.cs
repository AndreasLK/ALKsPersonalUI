using System.Text.Json;

namespace ALKsPersonalUI.Showcase.Shared;

public sealed class JsonUiNode
{
    public string Type { get; set; } = string.Empty;
    public Dictionary<string, string>? Props { get; set; }
    public string? Text { get; set; }
    public List<JsonUiNode>? Children { get; set; }

    public string GetProp(string key, string fallback = "") =>
        Props is not null && Props.TryGetValue(key, out var value) ? value : fallback;

    public bool GetBool(string key, bool fallback = false) =>
        Props is not null && Props.TryGetValue(key, out var value) && bool.TryParse(value, out var parsed)
            ? parsed
            : fallback;

    public int GetInt(string key, int fallback = 0) =>
        Props is not null && Props.TryGetValue(key, out var value) && int.TryParse(value, out var parsed)
            ? parsed
            : fallback;

    public double GetDouble(string key, double fallback = 0) =>
        Props is not null && Props.TryGetValue(key, out var value) && double.TryParse(value, out var parsed)
            ? parsed
            : fallback;

    public string ToJson(bool indented = true) =>
        JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = indented });

    public static JsonUiNode? FromJson(string json) =>
        JsonSerializer.Deserialize<JsonUiNode>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
}

