using Microsoft.AspNetCore.Components.Web;
using System.Globalization;

namespace ALKsPersonalUI.Core;

public static class KeyboardKeys
{
    public const string Escape = "Escape";
    public const string ArrowUp = "ArrowUp";
    public const string ArrowDown = "ArrowDown";
    public const string ArrowLeft = "ArrowLeft";
    public const string ArrowRight = "ArrowRight";

    public static bool IsEscape(KeyboardEventArgs keyEvent) =>
        keyEvent.Key == Escape;

    public static bool IsIncrementKey(KeyboardEventArgs keyEvent) =>
        keyEvent.Key is ArrowRight or ArrowUp;

    public static bool IsDecrementKey(KeyboardEventArgs keyEvent) =>
        keyEvent.Key is ArrowLeft or ArrowDown;
}

public static class GeometryMath
{
    public const double HalfDivisor = 2.0;
    public const double FullPercentage = 100.0;
    public const double CenterPercentage = 50.0;
    public const double DegreesPerHalfCircle = 180.0;
    public const double FullCircleRadians = 2.0 * Math.PI;
    public const double NormalizedMin = 0.0;
    public const double NormalizedMax = 1.0;

    public static double RadiansToDegrees(double radians) =>
        radians * (DegreesPerHalfCircle / Math.PI);

    public static double CalculatePointerAngleInDegrees(double deltaXFromCenter, double deltaYFromCenter) =>
        RadiansToDegrees(Math.Atan2(deltaXFromCenter, -deltaYFromCenter));

    public static double CalculateEuclideanDistance(double horizontalDelta, double verticalDelta) =>
        Math.Sqrt((horizontalDelta * horizontalDelta) + (verticalDelta * verticalDelta));

    public static string FormatInvariant(double numericValue) =>
        numericValue.ToString(CultureInfo.InvariantCulture);
}