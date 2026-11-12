namespace ALKsPersonalUI.Core;

public static class LoaderDefaults
{
    public static class SimpleSpinner
    {
        public const string DefaultSize = "1.2em";
        public const string DefaultThickness = "3px";
        public const string DefaultColor = "currentColor";
        public const string DefaultSpeed = "0.8s";
    }

    public static class Orbital
    {
        public const string DefaultLoadingText = "Loading...";
        public const double DefaultScale = 1.0;
        public const int DefaultSystemSizePixels = 90;
        public const double DefaultOrbitTiltDegrees = 80.0;
        public const int DefaultSatelliteCount = 9;
        public const int DefaultSatelliteSizePixels = 3;
        public const double DefaultOrbitDurationSeconds = 4.0;
        public const double DefaultPrecessionDurationSeconds = 45.0;
        public const string DefaultBaseColor = "#fff099";
        public const string DefaultBlueshiftColor = "#00ffff";
        public const string DefaultRedshiftColor = "#ff0000";
        public const int DefaultShiftIntensityPercent = 25;
        public const string ResponsiveSystemSize = "66.6cqmin";
        public const string ResponsiveSatelliteSize = "2.2cqmin";
    }

    public static class Mars
    {
        public const double DefaultScale = 1.0;
        public const double DefaultPlanetSizePixels = 40.0;
        public const string ResponsivePlanetSize = "30cqmin";
        public const double DefaultDayDurationSeconds = 24.0;
        public const int DefaultMinFeatures = 10;
        public const int DefaultMaxFeatures = 15;
        public const double DefaultColonyProbability = 0.25;
        public const double DefaultColonySizePercent = 5.0;
        public const double DefaultMinCraterSizePercent = 10.0;
        public const double DefaultMaxCraterSizePercent = 25.0;
        public const double DefaultWindowLightSizePercent = 5.0;

        public const int CandidateGenerationPoolSize = 50;
        public const double MinimumFeatureSeparationPercent = 5.0;
        public const double PlanetVirtualRadiusPercent = 50.0;
        public const double ColonyMinRadialFraction = 0.4;
        public const double ColonyRadialSpanFraction = 0.4;
        public const double CraterMaxRadialFraction = 0.85;

        public const string DefaultPlanetColor = "#e66a3d";
        public const string DefaultNightShadowColor = "rgba(5, 5, 15, 0.95)";
        public const string DefaultCraterColor = "rgba(130, 30, 0, 0.35)";
        public const string DefaultCraterRimColor = "rgba(255, 255, 255, 0.15)";
        public const string DefaultBuildingDayColor = "var(--ui-text)";
        public const string DefaultBuildingNightColor = "#2a2a35";
        public const string DefaultWindowLightColor = "var(--ui-action)";
        public const string DefaultWindowGlowColor = "#ffae00";
    }
}