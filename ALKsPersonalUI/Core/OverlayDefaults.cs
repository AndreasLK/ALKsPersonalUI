namespace ALKsPersonalUI.Core;

public static class OverlayDefaults
{
    public static class Modal
    {
        public const string DefaultTitle = "Dialog";
        public const string DefaultHeaderBackground = "var(--ui-bg-header)";
        public const string DefaultWidth = "min(400px, 92vw)";
        public const string DefaultPadding = "1rem";
        public const string CloseAriaLabel = "Close modal";
    }

    public static class Toast
    {
        public const int DefaultDurationMilliseconds = 3000;
        public const string Width = "fit-content";
        public const string Height = "fit-content";
        public const string MinHeight = "0";
        public const string Padding = "0.45rem 0.85rem";
        public const string BorderRadius = "var(--ui-radius-pill)";
        public const string BackgroundColor = "var(--ui-bg-elevated)";
        public const string DismissAriaLabel = "Dismiss notification";
        public const string InfoBorderColor = "var(--ui-accent)";
        public const string SuccessBorderColor = "var(--ui-success)";
        public const string WarningBorderColor = "var(--ui-warning)";
        public const string DangerBorderColor = "var(--ui-danger)";
    }

    public static class Tooltip
    {
        public const string TooltipElementIdPrefix = "tt-";
        public const string Width = "fit-content";
        public const string Height = "fit-content";
        public const string MinDimension = "0";
        public const string Padding = "0.35rem 0.65rem";
        public const string BorderRadius = "var(--ui-radius-md)";
        public const string DefaultBackgroundColor = "var(--ui-bg-app)";
        public const string DefaultBorderColor = "var(--ui-border)";
    }
}