using ALKsPersonalUI.Overlays;

namespace ALKsPersonalUI.Json;

/// <summary>
/// Executes declarative <see cref="JsonUiAction"/> sequences sequentially against <see cref="JsonUiSessionState"/>.
/// </summary>
public static class JsonUiActionDispatcher
{
    /// <summary>
    /// Executes every action in <paramref name="actions"/> in order, resolving state tokens just-in-time per step.
    /// </summary>
    public static Task DispatchAsync(
        IReadOnlyList<JsonUiAction>? actions,
        JsonUiSessionState state,
        JsonUiDocument? document,
        Func<JsonUiHostCommand, Task>? onHostCommand,
        string? eventValue = null) =>
        DispatchAsync(actions, state, document, onHostCommand, toastService: null, eventValue);

    /// <summary>
    /// Executes every action in <paramref name="actions"/> in order with optional toast notification support.
    /// </summary>
    public static async Task DispatchAsync(
        IReadOnlyList<JsonUiAction>? actions,
        JsonUiSessionState state,
        JsonUiDocument? document,
        Func<JsonUiHostCommand, Task>? onHostCommand,
        ToastService? toastService,
        string? eventValue = null)
    {
        if (actions is null || actions.Count == 0)
        {
            return;
        }

        // Track session generation so an awaiting HostCommand cannot continue mutating a reset session.
        var initialGeneration = state.SessionGeneration;

        foreach (var action in actions)
        {
            // Evaluate optional per-action condition against the latest session state.
            if (!JsonUiConditionEvaluator.Evaluate(action.When, state))
            {
                continue;
            }

            await ExecuteSingleAsync(action, state, document, onHostCommand, toastService, eventValue);

            if (action.Type == JsonUiActionType.ResetSession)
            {
                initialGeneration = state.SessionGeneration;
            }
            else if (state.SessionGeneration != initialGeneration)
            {
                return;
            }
        }
    }

    private static async Task ExecuteSingleAsync(
        JsonUiAction action,
        JsonUiSessionState state,
        JsonUiDocument? document,
        Func<JsonUiHostCommand, Task>? onHostCommand,
        ToastService? toastService,
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

            case JsonUiActionType.ShowToast:
                ApplyShowToast(action, state, toastService, eventValue);
                break;
        }
    }

    private static void ApplyShowToast(
        JsonUiAction action,
        JsonUiSessionState state,
        ToastService? toastService,
        string? eventValue)
    {
        if (toastService is null)
        {
            return;
        }

        var rawTemplate = !string.IsNullOrWhiteSpace(action.Message)
            ? action.Message
            : action.Value.HasValue
                ? JsonUiSessionState.FormatValue(JsonUiSessionState.NormalizeJsonElement(action.Value.Value))
                : string.Empty;

        var resolvedMessage = state.Interpolate(rawTemplate, eventValue);
        if (string.IsNullOrWhiteSpace(resolvedMessage))
        {
            return;
        }

        var variant = Enum.TryParse<ToastVariant>(action.Variant, ignoreCase: true, out var parsed)
            ? parsed
            : ToastVariant.Info;

        toastService.Show(resolvedMessage, variant);
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
