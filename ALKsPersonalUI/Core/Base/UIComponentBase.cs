using Microsoft.AspNetCore.Components;

namespace ALKsPersonalUI.Core;

public abstract class UIComponentBase : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public UIModifier? Modifier { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    protected UIModifier ModifierOrEmpty => Modifier ?? UIModifier.Empty;
    protected string? ModifierStyle => Modifier?.BuildStyle();
    protected string? ModifierClass => Modifier?.BuildClass();
}