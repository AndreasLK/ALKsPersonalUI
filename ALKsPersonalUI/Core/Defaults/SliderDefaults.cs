namespace ALKsPersonalUI.Core;

public static class SliderDefaults
{
    public const double DefaultMinValue = 0.0;
    public const double DefaultMaxValue = 100.0;
    public const double DefaultStep = 10.0;
    public const double DefaultSelectedValue = 0.0;
    public const int DefaultDecimalPlaces = 0;
    public const double FineKeyboardStepRatio = 0.01;

    public static class RotaryDial
    {
        public const string DefaultSize = "250px";
        public const int DefaultMaxAngleDegrees = 180;
        public const string SvgViewBox = "0 0 100 100";
        public const double SvgCenterCoordinate = 50.0;
        public const double TickOuterY = 0.0;
        public const double TickInnerY = 15.0;
        public const double TickStrokeWidth = 1.0;
    }

    public static class LinearRange
    {
        public const string DefaultWidth = "260px";
        public const double DefaultTrackWidthPx = 260.0;
        public const string SvgViewBox = "0 0 100 12";
        public const double TickTopY = 2.0;
        public const double TickBottomY = 10.0;
        public const double TickStrokeWidth = 0.6;
        public const string DefaultTickColor = "var(--slider-tick-color, #52525b)";
    }
}