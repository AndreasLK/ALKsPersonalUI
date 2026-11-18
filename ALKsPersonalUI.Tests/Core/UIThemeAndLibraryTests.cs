using ALKsPersonalUI.Core;
using ALKsPersonalUI.Overlays;
using Microsoft.Extensions.DependencyInjection;

namespace ALKsPersonalUI.Tests.Core;

public class UIThemeAndLibraryTests
{
    [Fact]
    public void UITheme_FromSeed_GeneratesDarkAndLightPalettesWithActionFallback()
    {
        var darkTheme = UITheme.FromSeed("Forest", "#10b981", "#05100a", isDarkTheme: true);
        var lightTheme = UITheme.FromSeed("Paper", "#2563eb", "#f8fafc", actionColor: "#f97316", isDarkTheme: false);

        Assert.Equal("#10b981", darkTheme.AccentColor);
        Assert.Equal("#10b981", darkTheme.ActionColor);
        Assert.Equal("#ffffff", darkTheme.TextColor);
        Assert.Contains("white 4%", darkTheme.HeaderBackgroundColor);

        Assert.Equal("#f97316", lightTheme.ActionColor);
        Assert.Equal("#0f172a", lightTheme.TextColor);
        Assert.Contains("black 4%", lightTheme.HeaderBackgroundColor);
    }

    [Fact]
    public void UITheme_WithCustomVariable_NormalizesLeadingDashesAndOutputsCssVariables()
    {
        var theme = new UITheme()
            .WithCustomVariable("kiosk-glow", "0 0 12px cyan")
            .WithCustomVariable("--brand-font", "sans-serif");

        var cssVariables = theme.ToCssVariables();

        Assert.Contains("--kiosk-glow: 0 0 12px cyan", cssVariables);
        Assert.Contains("--brand-font: sans-serif", cssVariables);
    }

    [Fact]
    public void UITheme_Clone_CreatesIndependentCustomVariableDictionary()
    {
        var originalTheme = new UITheme().WithCustomVariable("--token-a", "1px");
        var clonedTheme = originalTheme.Clone();
        clonedTheme.WithCustomVariable("--token-b", "2px");

        Assert.DoesNotContain("--token-b", originalTheme.ToCssVariables());
        Assert.Contains("--token-b: 2px", clonedTheme.ToCssVariables());
    }

    [Fact]
    public void UIThemeService_ApplyPreset_IsCaseInsensitiveAndFallsBackToDefaultForUnknownPreset()
    {
        var themeService = new UIThemeService();
        var changeNotificationCount = 0;
        themeService.OnChange += () => changeNotificationCount++;

        themeService.ApplyPreset("cyberpunk");
        Assert.Equal(ThemeDefaults.PresetNames.Cyberpunk, themeService.Current.Name);
        Assert.Equal(1, changeNotificationCount);

        themeService.ApplyPreset("NonExistentPreset");
        Assert.Equal(ThemeDefaults.PresetNames.Default, themeService.Current.Name);
        Assert.Equal(2, changeNotificationCount);
    }

    [Fact]
    public void UIThemeService_RegisterPresetAndSetTheme_ApplyNamesAndNotifyListeners()
    {
        var themeService = new UIThemeService();
        var customTheme = UITheme.FromSeed("Custom", "#ec4899");
        var notificationCount = 0;
        themeService.OnChange += () => notificationCount++;

        themeService.RegisterPreset("MuseumPink", customTheme);
        themeService.ApplyPreset("MuseumPink");

        Assert.Equal("MuseumPink", themeService.Current.Name);
        Assert.Equal("#ec4899", themeService.Current.AccentColor);
        Assert.Equal(1, notificationCount);

        themeService.SetTheme(UITheme.FromSeed("Manual", "#22c55e"));
        Assert.Equal("Manual", themeService.Current.Name);
        Assert.Equal(2, notificationCount);
    }

    [Fact]
    public void AddPersonalUILibrary_ConfiguresSeedThemesAndDefaultInteractionOptions()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddPersonalUILibrary(options =>
        {
            options.AddSeedTheme("EcoGreen", "#22c55e", "#07140d");
            options.InitialPresetName = "EcoGreen";
            options.DefaultPressEffect = ButtonPressEffect.ConcaveInset;
            options.DefaultTouchInputMode = TouchInputMode.Always;
        });

        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var themeService = serviceProvider.GetRequiredService<UIThemeService>();
        var toastService = serviceProvider.GetRequiredService<ToastService>();

        Assert.NotNull(toastService);
        Assert.Contains("EcoGreen", themeService.RegisteredPresets);
        Assert.Equal("EcoGreen", themeService.Current.Name);
        Assert.Equal(ButtonPressEffect.ConcaveInset, themeService.Current.DefaultPressEffect);
        Assert.Equal(TouchInputMode.Always, themeService.Current.DefaultTouchInputMode);
    }
}

