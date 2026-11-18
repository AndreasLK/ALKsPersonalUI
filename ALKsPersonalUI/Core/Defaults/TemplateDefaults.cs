namespace ALKsPersonalUI.Core;

public static class TemplateDefaults
{
    public static class AspectRatioFrame
    {
        public const int DefaultAspectWidth = 16;
    }

    public static class TemplateSlot
    {
        public const string DefaultSlotName = "Slot";
        public const string DefaultPlaceholderText = "Drop or select content";
        public const string DefaultMinHeight = "5rem";
        public const string DefaultBorderWidth = "2px";
        public const string DefaultBorderStyle = "dashed";
        public const string DefaultPadding = "0.75rem";
    }

    public static class FrameBox
    {
        public const string DefaultPadding = "1rem";
        public const string DefaultBorderWidth = "2px";
        public const string DefaultBorderStyle = "solid";
        public const string DefaultBorderRadius = "var(--radius-md, 0.75rem)";
        public const string DefaultBorderImageSlice = "1";
    }
}
