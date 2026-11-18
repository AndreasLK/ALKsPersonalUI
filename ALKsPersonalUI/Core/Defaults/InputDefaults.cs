namespace ALKsPersonalUI.Core;

public static class InputDefaults
{
    public static class Checkbox
    {
        public const string DefaultSize = "1.35rem";
        public const string DefaultAccentColor = "var(--ui-accent, #3b82f6)";
    }

    public static class Dropdown
    {
        public const string DefaultPlaceholder = "Select...";
        public const string DefaultWidth = "auto";
        public const string ChevronSymbol = "\u25BE";
        public const string SelectedCheckmarkSymbol = "\u2713";
        public const int NoSelectionIndex = -1;
        public const int NextItemStep = 1;
        public const int PreviousItemStep = -1;
    }

    public static class NumberInput
    {
        public const string ContainerWidth = "fit-content";
        public const string ContainerMinHeight = "0";
        public const string ContainerPadding = "0.25rem";
        public const string ContainerBorderRadius = "12px";
        public const string StepButtonWidth = "2.2rem";
        public const string StepButtonPadding = "0.3rem 0";
        public const string StepButtonExtrusionDepth = "2px";
        public const string DefaultButtonColor = "#3b82f6";
        public const string DefaultButtonTextColor = "#ffffff";
        public const string DecrementSymbol = "\u2212";
        public const string IncrementSymbol = "+";
        public const string KeypadSymbol = "\u2328";
    }

    public static class TextInput
    {
        public const int DefaultMaxLength = 256;
        public const string DefaultContainerMinimumDimensions = "auto";
        public const string ContainerPadding = "0.5rem 0.75rem";
        public const string RedactedTextPlaceholder = "[REDACTED]";
        public const string ClearSymbol = "\u2715";
        public const string KeyboardSymbol = "\u2328";
    }

    public static class ScreenKeyboard
    {
        public const int DefaultMaxLength = TextInput.DefaultMaxLength;
    }

    public static class RadioSlider
    {
        public const double DefaultDialSizePx = 250.0;
    }

    public static class Symbols
    {
        public const string Backspace = "\u232B";
        public const string ShiftUp = "\u21E7";
        public const string EnterReturn = "\u21B5";
        public const string TabRight = "\u21E5";
        public const string Checkmark = "\u2713";
        public const string PlusMinus = "\u00B1";
        public const string BulletMask = "\u25CF";
        public const string ArrowUp = "\u2191";
        public const string ArrowLeft = "\u2190";
        public const string ArrowDown = "\u2193";
        public const string ArrowRight = "\u2192";
    }

    public static class FileUpload
    {
        public const long DefaultMaxFileSizeBytes = 10 * 1024 * 1024;
        public const int DefaultMaxAllowedFiles = 20;
        public const string DefaultAcceptedExtensions = ".png,.jpg,.jpeg,.webp,.gif,.svg";
        public const string DefaultPromptText = "Tap to choose files";
    }

    public static class SearchInput
    {
        public const string DefaultPlaceholder = "Search...";
        public const string DefaultAriaLabel = "Search";
    }

    public static class TextArea
    {
        public const int DefaultRows = 4;
        public const int DefaultMaxLength = 2000;
        public const string DefaultPlaceholder = "Enter text...";
    }
}