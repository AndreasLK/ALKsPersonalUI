namespace ALKsPersonalUI.Core;

public static class NavigationDefaults
{
    public static class BurgerMenu
    {
        public const string DefaultTitle = "Menu";
        public const string DefaultButtonColor = "var(--ui-accent)";
        public const string DefaultBackgroundColor = "var(--ui-bg-elevated)";
        public const string DefaultHeaderBackground = "var(--ui-bg-app)";
        public const string TriggerButtonWidth = "2.6rem";
        public const string TriggerButtonPadding = "0.45rem 0";
        public const string DrawerWidth = "min(280px, 82vw)";
        public const string DrawerHeight = "100vh";
        public const string DrawerBorderRadius = "0";
        public const string DrawerPadding = "1rem";
        public const string OpenIconSymbol = "\u2630";
        public const string CloseIconSymbol = "\u2715";
    }

    public static class Navbar
    {
        public const string DefaultTitle = "My App";
        public const string DefaultBrandHref = "/";
        public const string Width = "100%";
        public const string Height = "fit-content";
        public const string MinHeight = "3.5rem";
        public const string Padding = "0.5rem 1rem";
        public const string DefaultBackgroundColor = "var(--ui-bg-elevated)";
        public const string DefaultBorderColor = "var(--ui-border)";
        public const string DefaultBorderRadius = "var(--ui-radius-lg)";
        public const string DefaultAccentColor = "var(--ui-accent)";
    }

    public static class Tabs
    {
        public const string DefaultAccentColor = "var(--ui-accent)";
        public const string InactiveTabColor = "var(--ui-text)";
        public const string DefaultBarBackground = "var(--ui-bg-header)";
        public const string DefaultBorderColor = "var(--ui-border)";
        public const string DefaultBorderRadius = "var(--ui-radius-md)";
        public const string BarWidth = "100%";
        public const string BarHeight = "fit-content";
        public const string BarMinHeight = "0";
        public const string BarPadding = "0.35rem 0.5rem";
        public const string TabButtonWidth = "auto";
        public const string TabButtonPadding = "0.35rem 0.75rem";
        public const string TabButtonFontSize = "0.85rem";
        public const string TrailSeparatorSymbol = "/";
    }

    public static class Breadcrumbs
    {
        public const string DefaultSeparator = ">";
        public const string DefaultAriaLabel = "Breadcrumb navigation";
    }

    public static class PageCarousel
    {
        public const string PreviousLabel = "Previous";
        public const string NextLabel = "Next";
        public const string DefaultAriaLabel = "Page carousel";
    }
}