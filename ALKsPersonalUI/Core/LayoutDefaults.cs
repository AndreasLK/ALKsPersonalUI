namespace ALKsPersonalUI.Core;

public static class LayoutDefaults
{
    public const string DefaultDisplay = "flex";
    public const string GridDisplay = "grid";
    public const string DefaultGap = "0.5rem";

    public static class Divider
    {
        public const string DefaultThickness = "1px";
        public const string DefaultColor = "var(--ui-border)";
        public const string FullLength = "100%";
    }

    public static class Grid
    {
        public const int DefaultColumns = 0;
        public const string DefaultMinColumnWidth = "220px";
        public const string DefaultGap = "0.75rem";
    }

    public static class Text
    {
        public const string DefaultColor = "var(--ui-text)";
        public const string MutedColor = "var(--ui-text-muted)";
        public const string DisplaySize = "2.25rem";
        public const string H1Size = "1.75rem";
        public const string H2Size = "1.4rem";
        public const string H3Size = "1.15rem";
        public const string TitleSize = "1.05rem";
        public const string SubtitleSize = "0.95rem";
        public const string BodySize = "0.92rem";
        public const string CaptionSize = "0.78rem";
        public const string MonoSize = "0.85rem";
    }

    public static string ToCss(HorizontalAlignment alignment) => alignment switch
    {
        HorizontalAlignment.Start => "flex-start",
        HorizontalAlignment.Center => "center",
        HorizontalAlignment.End => "flex-end",
        _ => "stretch"
    };

    public static string ToCss(VerticalAlignment alignment) => alignment switch
    {
        VerticalAlignment.Top => "flex-start",
        VerticalAlignment.Center => "center",
        VerticalAlignment.Bottom => "flex-end",
        _ => "stretch"
    };

    public static string ToCss(Arrangement arrangement) => arrangement switch
    {
        Arrangement.Start => "flex-start",
        Arrangement.Center => "center",
        Arrangement.End => "flex-end",
        Arrangement.SpaceBetween => "space-between",
        Arrangement.SpaceAround => "space-around",
        Arrangement.SpaceEvenly => "space-evenly",
        _ => "flex-start"
    };

    public static (string AlignItems, string JustifyContent) ToCss(BoxAlignment alignment) => alignment switch
    {
        BoxAlignment.TopStart => ("flex-start", "flex-start"),
        BoxAlignment.TopCenter => ("flex-start", "center"),
        BoxAlignment.TopEnd => ("flex-start", "flex-end"),
        BoxAlignment.CenterStart => ("center", "flex-start"),
        BoxAlignment.Center => ("center", "center"),
        BoxAlignment.CenterEnd => ("center", "flex-end"),
        BoxAlignment.BottomStart => ("flex-end", "flex-start"),
        BoxAlignment.BottomCenter => ("flex-end", "center"),
        BoxAlignment.BottomEnd => ("flex-end", "flex-end"),
        _ => ("center", "center")
    };

    public static string ToCss(FontWeight weight) => weight switch
    {
        FontWeight.Light => "300",
        FontWeight.Regular => "400",
        FontWeight.Medium => "500",
        FontWeight.SemiBold => "600",
        FontWeight.Bold => "700",
        _ => "400"
    };

    public static string ToCss(TextAlignment alignment) => alignment switch
    {
        TextAlignment.Center => "center",
        TextAlignment.End => "right",
        TextAlignment.Justify => "justify",
        _ => "left"
    };
}