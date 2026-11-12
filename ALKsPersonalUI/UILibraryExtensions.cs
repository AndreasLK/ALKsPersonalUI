using ALKsPersonalUI.Core;
using ALKsPersonalUI.Overlays;
using Microsoft.Extensions.DependencyInjection;

namespace ALKsPersonalUI;

public sealed class PersonalUIOptions
{
    internal Dictionary<string, Func<UITheme>> CustomPresets { get; } = new(StringComparer.OrdinalIgnoreCase);
    public string? InitialPresetName { get; set; }
    public UITheme? InitialCustomTheme { get; set; }
    public ButtonPressEffect? DefaultPressEffect { get; set; }
    public TouchInputMode? DefaultTouchInputMode { get; set; }

    public PersonalUIOptions AddThemePreset(string name, UITheme theme)
    {
        CustomPresets[name] = theme.Clone;
        return this;
    }

    public PersonalUIOptions AddSeedTheme(
        string name,
        string accentColor,
        string appBackground = "#121316",
        string? actionColor = null,
        bool isDark = true,
        ButtonPressEffect pressEffect = ButtonPressEffect.Auto)
    {
        CustomPresets[name] = () => UITheme.FromSeed(name, accentColor, appBackground, actionColor, isDark, pressEffect);
        return this;
    }
}

public static class UILibraryExtensions
{
    public static IServiceCollection AddPersonalUILibrary(
        this IServiceCollection services,
        Action<PersonalUIOptions>? configure = null)
    {
        var options = new PersonalUIOptions();
        configure?.Invoke(options);

        services.AddScoped<ToastService>();
        services.AddScoped<UIThemeService>(_ =>
        {
            var themeService = new UIThemeService();
            foreach (var (name, factory) in options.CustomPresets)
                themeService.RegisterPreset(name, factory);

            if (options.InitialCustomTheme is not null)
                themeService.SetTheme(options.InitialCustomTheme);
            else if (!string.IsNullOrWhiteSpace(options.InitialPresetName))
                themeService.ApplyPreset(options.InitialPresetName);

            if (options.DefaultPressEffect.HasValue)
                themeService.Current.DefaultPressEffect = options.DefaultPressEffect.Value;

            if (options.DefaultTouchInputMode.HasValue)
                themeService.Current.DefaultTouchInputMode = options.DefaultTouchInputMode.Value;

            return themeService;
        });

        return services;
    }
}