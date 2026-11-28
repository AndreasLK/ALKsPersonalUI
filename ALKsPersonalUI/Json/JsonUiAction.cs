using System.Text.Json;

namespace ALKsPersonalUI.Json;

/// <summary>
/// Represents a single declarative action executed by the JSON UI runtime.
/// </summary>
public sealed class JsonUiAction
{
    /// <summary>
    /// Gets or sets the allow-listed action kind to execute.
    /// </summary>
    public JsonUiActionType Type { get; set; }

    /// <summary>
    /// Gets or sets the session state key or source timestamp key operated on by the action.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the literal or interpolated value used by <see cref="JsonUiActionType.SetState"/>.
    /// </summary>
    public JsonElement? Value { get; set; }

    /// <summary>
    /// Gets or sets the numeric delta used by <see cref="JsonUiActionType.IncrementState"/>.
    /// </summary>
    public double Delta { get; set; } = 1;

    /// <summary>
    /// Gets or sets the target view name, output state key, or host command name depending on <see cref="Type"/>.
    /// </summary>
    public string? Target { get; set; }

    /// <summary>
    /// Gets or sets the session state keys whose current values should be included in a host command payload.
    /// </summary>
    public List<string>? PayloadKeys { get; set; }

    /// <summary>
    /// Gets or sets the interpolated message text used by <see cref="JsonUiActionType.ShowToast"/>.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the optional toast severity variant (Info, Success, Warning, Danger) for <see cref="JsonUiActionType.ShowToast"/>.
    /// </summary>
    public string? Variant { get; set; }

    /// <summary>
    /// Gets or sets an optional condition evaluated against session state before executing this action.
    /// </summary>
    public JsonUiCondition? When { get; set; }
}
