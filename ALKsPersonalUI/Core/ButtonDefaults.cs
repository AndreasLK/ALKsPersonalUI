namespace ALKsPersonalUI.Core;

public static class ButtonDefaults
{
    public const string DefaultWidth = "fit-content";
    public const string DefaultHeight = "fit-content";
    public const string DefaultPadding = "0.75rem 1.5rem";
    public const string DefaultFontSize = "0.95rem";
    public const string DefaultBaseColor = "var(--ui-action)";
    public const string DefaultTextColor = "var(--ui-text-inverse)";
    public const string DefaultDepth = "var(--ui-depth)";
    public const string DefaultAnimationSpeed = "var(--ui-anim-speed)";

    public static readonly (string VerticalPadding, string HorizontalPadding, string FontSize) SmallMetrics =
        ("0.5rem", "1.05rem", "0.85rem");

    public static readonly (string VerticalPadding, string HorizontalPadding, string FontSize) MediumMetrics =
        ("0.7rem", "1.4rem", "0.95rem");

    public static readonly (string VerticalPadding, string HorizontalPadding, string FontSize) LargeMetrics =
        ("0.9rem", "2rem", "1.1rem");

    public static class Close
    {
        public const string DefaultAriaLabel = "Close";
        public const string BaseColor = "var(--ui-text)";
        public const string Width = "auto";
        public const string Padding = "0.35rem 0.65rem";
        public const string FontSize = "0.85rem";
        public const string IconSymbol = "\u2715";
    }
}