namespace ALKsPersonalUI.Json;

/// <summary>
/// Executes declarative <see cref="JsonUiAction"/> sequences sequentially against <see cref="JsonUiSessionState"/>.
/// </summary>
public static class JsonUiActionDispatcher
{
    /// <summary>
    /// Executes every action in <paramref name="actions"/> in order, resolving state tokens just-in-time per step.
    /// </summary>
    public static async Task DispatchAsync(
        IReadOnlyList<JsonUiAction>? actions,
        JsonUiSessionState state,
        JsonUiDocument? document,
        Func<JsonUiHostCommand, Task>? onHostCommand,
        string? eventValue = null)
    {
        if (actions is null || actions.Count == 0)
        {
            return;
        }

        foreach (var action in actions)
        {
            await ExecuteSingleAsync(action, state, document, onHostCommand, eventValue);
        }
    }

    private static async Task ExecuteSingleAsync(
        JsonUiAction action,
        JsonUiSessionState state,
        JsonUiDocument? document,
        Func<JsonUiHostCommand, Task>? onHostCommand,
        string? eventValue)
    {
        switch (action.Type)
        {
            case JsonUiActionType.SetState:
                ApplySetState(action, state, eventValue);
                break;

            case JsonUiActionType.IncrementState:
                if (!string.IsNullOrWhiteSpace(action.Key))
                {
                    state.Increment(action.Key, action.Delta);
                }
                break;

            case JsonUiActionType.ToggleState:
                if (!string.IsNullOrWhiteSpace(action.Key))
                {
                    state.Toggle(action.Key);
                }
                break;

            case JsonUiActionType.Navigate:
                ApplyNavigate(action, state, document);
                break;

            case JsonUiActionType.ResetSession:
                state.Reset(document);
                break;

            case JsonUiActionType.MarkTime:
                if (!string.IsNullOrWhiteSpace(action.Key))
                {
                    state.MarkTime(action.Key);
                }
                break;

            case JsonUiActionType.ComputeElapsedSeconds:
                if (!string.IsNullOrWhiteSpace(action.Key) && !string.IsNullOrWhiteSpace(action.Target))
                {
                    state.ComputeElapsedSeconds(action.Key, action.Target);
                }
                break;

            case JsonUiActionType.HostCommand:
                await DispatchHostCommandAsync(action, state, onHostCommand);
                break;
        }
    }

    private static void ApplySetState(JsonUiAction action, JsonUiSessionState state, string? eventValue)
    {
        if (string.IsNullOrWhiteSpace(action.Key))
        {
            return;
        }

        if (!action.Value.HasValue)
        {
            state.SetValue(action.Key, eventValue);
            return;
        }

        var normalized = JsonUiSessionState.NormalizeJsonElement(action.Value.Value);
        if (normalized is string template)
        {
            state.SetValue(action.Key, state.Interpolate(template, eventValue));
            return;
        }

        state.SetValue(action.Key, normalized);
    }

    private static void ApplyNavigate(JsonUiAction action, JsonUiSessionState state, JsonUiDocument? document)
    {
        if (string.IsNullOrWhiteSpace(action.Target))
        {
            return;
        }

        var resolvedTarget = state.Interpolate(action.Target);
        // Only navigate when the target view exists in the document (fail safe on unknown view keys).
        if (document?.Views is not null && document.Views.ContainsKey(resolvedTarget))
        {
            state.NavigateTo(resolvedTarget);
        }
    }

    private static async Task DispatchHostCommandAsync(
        JsonUiAction action,
        JsonUiSessionState state,
        Func<JsonUiHostCommand, Task>? onHostCommand)
    {
        if (onHostCommand is null || string.IsNullOrWhiteSpace(action.Target))
        {
            return;
        }

        var payload = new Dictionary<string, object?>(StringComparer.Ordinal);
        if (action.PayloadKeys is not null)
        {
            foreach (var key in action.PayloadKeys)
            {
                if (!string.IsNullOrWhiteSpace(key))
                {
                    payload[key] = state.TryGetValue(key, out var val) ? val : null;
                }
            }
        }

        await onHostCommand(new JsonUiHostCommand(state.Interpolate(action.Target), payload));
    }
}
