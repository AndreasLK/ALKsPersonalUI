using Microsoft.AspNetCore.Components;

namespace ALKsPersonalUI.Json;

/// <summary>
/// Carries runtime state, data sources, and event dispatchers into decomposed JSON UI sub-renderers.
/// </summary>
/// <param name="State">The active in-memory session state store.</param>
/// <param name="DataSources">Host-supplied tabular or list data keyed by data source name.</param>
/// <param name="DispatchEventAsync">Dispatches a declarative node event sequence.</param>
/// <param name="TriggerLegacyActionAsync">Invokes the backwards-compatible <c>OnAction</c> string callback.</param>
/// <param name="NotifyStateChanged">Requests a re-render of the root <see cref="JsonUiRenderer"/>.</param>
/// <param name="RenderChild">Recursively renders a child node at the specified tree depth.</param>
public sealed record JsonUiRenderContext(
    JsonUiSessionState State,
    IReadOnlyDictionary<string, IReadOnlyList<IReadOnlyDictionary<string, object?>>>? DataSources,
    Func<JsonUiNode, string, string?, Task> DispatchEventAsync,
    Func<string, Task> TriggerLegacyActionAsync,
    Action NotifyStateChanged,
    Func<JsonUiNode, int, RenderFragment> RenderChild);
