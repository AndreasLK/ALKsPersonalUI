namespace ALKsPersonalUI.Json;

/// <summary>
/// Compile-time allow-listed declarative actions that a JSON UI node can trigger.
/// </summary>
public enum JsonUiActionType
{
    /// <summary>
    /// Assigns a literal or interpolated value to a session state key.
    /// </summary>
    SetState,

    /// <summary>
    /// Adds a numeric delta to a session state key, initializing from zero if missing.
    /// </summary>
    IncrementState,

    /// <summary>
    /// Flips a boolean session state key between true and false.
    /// </summary>
    ToggleState,

    /// <summary>
    /// Switches the active view in the current document.
    /// </summary>
    Navigate,

    /// <summary>
    /// Restores initial session state, resets view navigation, and increments the session generation.
    /// </summary>
    ResetSession,

    /// <summary>
    /// Records the current UTC timestamp under the specified state key.
    /// </summary>
    MarkTime,

    /// <summary>
    /// Computes whole elapsed seconds since a marked timestamp key and stores it into a target state key.
    /// </summary>
    ComputeElapsedSeconds,

    /// <summary>
    /// Emits a named command with resolved payload values to the hosting application.
    /// </summary>
    HostCommand
}
