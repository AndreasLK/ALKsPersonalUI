namespace ALKsPersonalUI.Json;

/// <summary>
/// Represents an external side-effect command emitted by the JSON UI runtime to the host application.
/// </summary>
/// <param name="Command">The command name declared in the JSON document.</param>
/// <param name="Payload">Resolved session state values requested by the command.</param>
public sealed record JsonUiHostCommand(string Command, IReadOnlyDictionary<string, object?> Payload);
