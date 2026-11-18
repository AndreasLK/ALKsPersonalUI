using ALKsPersonalUI.Core;
using ALKsPersonalUI.Inputs;
using ALKsPersonalUI.Tests.TestInfrastructure;
using Microsoft.AspNetCore.Components.Web;

namespace ALKsPersonalUI.Tests.Components;

public class InputComponentsTests : PersonalUiTestContext
{
    [Fact]
    public void Checkbox_SupportsToggleSwitchDisabledGuardAndLabelImage()
    {
        var isChecked = false;
        var cut = Render<Checkbox>(parameters => parameters
            .Add(checkbox => checkbox.Checked, isChecked)
            .Add(checkbox => checkbox.CheckedChanged, value => isChecked = value)
            .Add(checkbox => checkbox.RenderAsToggleSwitch, true)
            .Add(checkbox => checkbox.Label, "Oat Milk")
            .Add(checkbox => checkbox.LabelImageSource, "/icons/oat.png"));

        Assert.NotEmpty(cut.FindAll(".switch-track"));
        Assert.Equal("/icons/oat.png", cut.Find("img.checkbox-label-image").GetAttribute("src"));

        cut.Find("input.checkbox-native").Change(true);
        Assert.True(isChecked);

        var disabledCut = Render<Checkbox>(parameters => parameters
            .Add(checkbox => checkbox.Checked, false)
            .Add(checkbox => checkbox.CheckedChanged, value => isChecked = value)
            .Add(checkbox => checkbox.Disabled, true));

        disabledCut.Find("input.checkbox-native").Change(false);
        Assert.True(isChecked);
    }

    [Fact]
    public void TextInput_TruncatesMaxLengthTogglesPasswordAndHandlesThrowingValidationRule()
    {
        string? latestText = null;
        var latestSecret = new SecretString(null);
        var rules = new List<ValidationRule>
        {
            new(value => value.Length >= 4, "Minimum 4 chars"),
            new(_ => throw new InvalidOperationException("Broken rule"), "Rule failed safely")
        };

        var cut = Render<TextInput>(parameters => parameters
            .Add(input => input.Label, "Passcode")
            .Add(input => input.IsPassword, true)
            .Add(input => input.MaxLength, 5)
            .Add(input => input.ValidationRules, rules)
            .Add(input => input.ValueChanged, value => latestText = value)
            .Add(input => input.SecretValueChanged, secret => latestSecret = secret));

        Assert.Equal("password", cut.Find("input.custom-input").GetAttribute("type"));

        cut.Find("input.custom-input").Input("1234567");
        Assert.Equal("12345", latestText);
        Assert.True(latestSecret.IsEmpty);
        Assert.Contains("Rule failed safely", cut.Find(".input-error").TextContent);

        cut.Find("button.password-toggle").Click();
        Assert.Equal("text", cut.Find("input.custom-input").GetAttribute("type"));

        cut.Find("button[aria-label='Clear text']").Click();
        Assert.Equal(string.Empty, latestText);
    }

    [Fact]
    public void NumberInput_ClampsBoundsParsesCommaDecimalsAndDisablesStepButtonsAtLimits()
    {
        var currentValue = 2.0;
        var cut = Render<NumberInput<double>>(parameters => parameters
            .Add(input => input.Value, currentValue)
            .Add(input => input.ValueChanged, value => currentValue = value)
            .Add(input => input.Min, 1.0)
            .Add(input => input.Max, 5.0)
            .Add(input => input.Step, 1.5));

        cut.Find("button[aria-label='Increase value']").Click();
        Assert.Equal(3.5, currentValue);

        cut.Find("input.number-native").Change("4,25");
        Assert.Equal(4.25, currentValue);

        cut.Find("input.number-native").Change("99");
        Assert.Equal(5.0, currentValue);
        Assert.True(cut.Find("button[aria-label='Increase value']").HasAttribute("disabled"));
    }

    [Fact]
    public void TouchNumpad_SupportsDigitsDecimalsToggleSignBackspaceAndPinLimit()
    {
        var numericValue = 0.0;
        var cut = Render<TouchNumpad<double>>(parameters => parameters
            .Add(pad => pad.Value, numericValue)
            .Add(pad => pad.ValueChanged, value => numericValue = value)
            .Add(pad => pad.AllowDecimal, true));

        cut.FindAll("button.action-btn").First(btn => btn.TextContent.Trim() == "4").Click();
        cut.FindAll("button.action-btn").First(btn => btn.TextContent.Trim() == ".").Click();
        cut.FindAll("button.action-btn").First(btn => btn.TextContent.Trim() == "5").Click();
        Assert.Equal(4.5, numericValue);

        cut.Find("button[aria-label='Backspace']").Click();
        Assert.Equal(4.0, numericValue);

        var pinValue = 0;
        var pinCut = Render<TouchNumpad<int>>(parameters => parameters
            .Add(pad => pad.Value, pinValue)
            .Add(pad => pad.ValueChanged, value => pinValue = value)
            .Add(pad => pad.IsPinMode, true)
            .Add(pad => pad.MaxPinLength, 2));

        var keyOne = pinCut.FindAll("button.action-btn").First(btn => btn.TextContent.Trim() == "1");
        keyOne.Click();
        keyOne.Click();
        keyOne.Click();

        Assert.Equal(11, pinValue);
        Assert.Equal("\u25CF\u25CF", pinCut.Find(".numpad-value").TextContent);
    }

    [Fact]
    public void SegmentedControlAndDropdown_SelectItemsAndRenderItemImages()
    {
        string? selectedSegment = "Plastic";
        var segmentCut = Render<SegmentedControl<string>>(parameters => parameters
            .Add(control => control.Items, ["Plastic", "Metal"])
            .Add(control => control.Value, selectedSegment)
            .Add(control => control.ValueChanged, value => selectedSegment = value)
            .Add(control => control.ItemImageSourceSelector, item => $"/icons/{item.ToLowerInvariant()}.png"));

        Assert.Equal(2, segmentCut.FindAll("img.btn-image").Count);
        segmentCut.FindAll("button.action-btn")[1].Click();
        Assert.Equal("Metal", selectedSegment);

        string? selectedOption = "Plastic";
        var dropdownCut = Render<Dropdown<string>>(parameters => parameters
            .Add(dropdown => dropdown.Items, ["Plastic", "Glass", "Paper"])
            .Add(dropdown => dropdown.Value, selectedOption)
            .Add(dropdown => dropdown.ValueChanged, value => selectedOption = value)
            .Add(dropdown => dropdown.ItemImageSourceSelector, item => $"/icons/{item.ToLowerInvariant()}.png"));

        Assert.Equal("/icons/plastic.png", dropdownCut.Find("button.select-trigger img.select-item-image").GetAttribute("src"));

        dropdownCut.Find("button.select-trigger").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        Assert.Equal("Glass", selectedOption);

        dropdownCut.Find("button.select-trigger").Click();
        Assert.NotEmpty(dropdownCut.FindAll("ul.select-menu li.select-option"));

        dropdownCut.Find("button.select-trigger").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        Assert.Empty(dropdownCut.FindAll("ul.select-menu"));
    }

    [Fact]
    public void RangeSliderAndRadioSlider_HandleKeyboardWheelAndInvalidRangeSafely()
    {
        var rangeValue = 10.0;
        var rangeCut = Render<RangeSlider>(parameters => parameters
            .Add(slider => slider.MinValue, 0.0)
            .Add(slider => slider.MaxValue, 100.0)
            .Add(slider => slider.Step, 5.0)
            .Add(slider => slider.SnapToTicks, true)
            .Add(slider => slider.SelectedValue, rangeValue)
            .Add(slider => slider.SelectedValueChanged, value => rangeValue = value));

        rangeCut.Find("button[role='slider']").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        Assert.Equal(15.0, rangeValue);

        rangeCut.Find("button[role='slider']").Wheel(new WheelEventArgs { DeltaY = 120 });
        Assert.Equal(10.0, rangeValue);

        var invalidRadioCut = Render<RadioSlider>(parameters => parameters
            .Add(slider => slider.MinValue, 50.0)
            .Add(slider => slider.MaxValue, 50.0)
            .Add(slider => slider.SelectedValue, 50.0));

        invalidRadioCut.Find("button[role='slider']").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        Assert.Equal("50", invalidRadioCut.Find("button[role='slider']").GetAttribute("aria-valuenow"));
        Assert.Contains("--_rs-rotation: -90deg", invalidRadioCut.Find(".dial-indicator").GetAttribute("style"));
    }
}
