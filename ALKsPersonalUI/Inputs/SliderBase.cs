using ALKsPersonalUI.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace ALKsPersonalUI.Inputs;

public abstract class SliderBase : UIComponentBase
{
    [Parameter] public double MinValue { get; set; } = SliderDefaults.DefaultMinValue;
    [Parameter] public double MaxValue { get; set; } = SliderDefaults.DefaultMaxValue;
    [Parameter] public double Step { get; set; } = SliderDefaults.DefaultStep;
    [Parameter] public double SelectedValue { get; set; } = SliderDefaults.DefaultSelectedValue;
    [Parameter] public EventCallback<double> SelectedValueChanged { get; set; }
    [Parameter] public string SelectionUnit { get; set; } = string.Empty;
    [Parameter] public bool SnapToTicks { get; set; }
    [Parameter] public int? DecimalPlaces { get; set; } = SliderDefaults.DefaultDecimalPlaces;
    [Parameter] public bool ShowSelectedValue { get; set; } = true;
    [Parameter] public RenderFragment<double>? TickTemplate { get; set; }

    protected bool IsDragging { get; set; }
    protected bool HasValidRange => MaxValue > MinValue;
    protected double ValueRange => Math.Max(0.0, MaxValue - MinValue);
    protected int TotalTicks => Step > 0.0 ? (int)Math.Floor(ValueRange / Step) : 0;

    protected double ValuePercentage
    {
        get
        {
            if (!HasValidRange)
                return GeometryMath.NormalizedMin;

            var clampedSelection = Math.Clamp(SelectedValue, MinValue, MaxValue);
            return (clampedSelection - MinValue) / ValueRange;
        }
    }

    protected double KeyboardAndWheelIncrement =>
        SnapToTicks ? Step : ValueRange * SliderDefaults.FineKeyboardStepRatio;

    protected abstract Task UpdateValueFromPointer(PointerEventArgs pointerEvent);

    protected async Task HandlePointerDown(PointerEventArgs pointerEvent)
    {
        IsDragging = true;
        await UpdateValueFromPointer(pointerEvent);
    }

    protected async Task HandlePointerMove(PointerEventArgs pointerEvent)
    {
        if (!IsDragging)
            return;

        await UpdateValueFromPointer(pointerEvent);
    }

    protected void HandlePointerUp(PointerEventArgs pointerEvent) =>
        IsDragging = false;

    protected async Task HandleKeyDown(KeyboardEventArgs keyboardEvent)
    {
        if (KeyboardKeys.IsIncrementKey(keyboardEvent))
            await UpdateSelectedValue(SelectedValue + KeyboardAndWheelIncrement);

        if (KeyboardKeys.IsDecrementKey(keyboardEvent))
            await UpdateSelectedValue(SelectedValue - KeyboardAndWheelIncrement);
    }

    protected async Task HandleMouseWheel(WheelEventArgs wheelEvent)
    {
        if (wheelEvent.DeltaY < 0)
            await UpdateSelectedValue(SelectedValue + KeyboardAndWheelIncrement);

        if (wheelEvent.DeltaY > 0)
            await UpdateSelectedValue(SelectedValue - KeyboardAndWheelIncrement);
    }

    protected async Task UpdateSelectedValue(double targetValue)
    {
        var clampedValue = Math.Clamp(targetValue, MinValue, MaxValue);
        var roundedOrSnappedValue = ApplySnapAndPrecision(clampedValue);

        if (roundedOrSnappedValue == SelectedValue)
            return;

        SelectedValue = roundedOrSnappedValue;
        await SelectedValueChanged.InvokeAsync(SelectedValue);
    }

    protected double ApplySnapAndPrecision(double rawValue)
    {
        var shouldSnapToStep = SnapToTicks && Step > 0.0;
        if (shouldSnapToStep)
        {
            var nearestStepCountFromMin = Math.Round((rawValue - MinValue) / Step);
            var snappedValue = MinValue + (nearestStepCountFromMin * Step);
            return Math.Clamp(snappedValue, MinValue, MaxValue);
        }

        if (DecimalPlaces.HasValue)
            return Math.Round(rawValue, DecimalPlaces.Value);

        return rawValue;
    }
}