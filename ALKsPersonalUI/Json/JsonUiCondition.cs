using System.Text.Json;

namespace ALKsPersonalUI.Json;

/// <summary>
/// Declarative condition evaluated against <see cref="JsonUiSessionState"/>.
/// </summary>
public sealed class JsonUiCondition
{
    /// <summary>
    /// Gets or sets the session state key to inspect.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the comparison operator applied to the state value.
    /// </summary>
    public JsonUiConditionOperator Operator { get; set; } = JsonUiConditionOperator.IsTrue;

    /// <summary>
    /// Gets or sets the expected comparison value when required by <see cref="Operator"/>.
    /// </summary>
    public JsonElement? Value { get; set; }
}
