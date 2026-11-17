using System.Text;

namespace ALKsPersonalUI.Inputs;

public enum KeyboardLayout
{
    Nordic,   // QWERTY with AE, O-slash, A-ring
    QwertyUS, // Standard US QWERTY
    QwertzDE, // German QWERTZ
    AzertyFR  // French AZERTY
}

public sealed record VirtualKeyEventArgs(
    string Key,
    string Code,
    bool Shift,
    bool Ctrl,
    bool Alt,
    bool IsPrintable);

public sealed class KeyboardSetup
{
    public KeyboardLayout LayoutValue { get; private set; } = KeyboardLayout.Nordic;
    public bool IncludeLetters { get; private set; } = true;
    public bool IncludeNumbers { get; private set; }
    public bool IncludeSpecialCharacters { get; private set; }
    public bool IncludeFunctionKeys { get; private set; }
    public bool IncludeSystemKeys { get; private set; }            // PrtSc, ScrLk, Pause
    public bool IncludeNavigationCluster { get; private set; }     // Ins, Home, PgUp, Del, End, PgDn
    public bool IncludeArrowKeys { get; private set; }             // Inverted-T Arrows
    public bool IncludeNumpad { get; private set; }                // Right-hand Numpad
    public bool IncludeDesktopModifierKeys { get; private set; }   // Tab, Ctrl, Alt, AltGr (NEVER Windows key)

    public static KeyboardSetup New => new();

    public static KeyboardSetup LettersOnly => new();

    public static KeyboardSetup TouchStandard => new KeyboardSetup()
        .WithNumbers()
        .WithSpecialCharacters();

    public static KeyboardSetup Tenkeyless => new KeyboardSetup()
        .WithNumbers()
        .WithSpecialCharacters()
        .WithFunctionKeys()
        .WithDesktopModifierKeys()
        .WithSystemKeys()
        .WithNavigationCluster()
        .WithArrowKeys();

    public static KeyboardSetup FullMacro => Tenkeyless.Clone().WithNumpad();

    public KeyboardSetup Clone() => (KeyboardSetup)MemberwiseClone();

    public KeyboardSetup Layout(KeyboardLayout layout)
    {
        LayoutValue = layout;
        return this;
    }

    public KeyboardSetup WithLetters(bool enabled = true)
    {
        IncludeLetters = enabled;
        return this;
    }

    public KeyboardSetup WithoutLetters() => WithLetters(false);

    public KeyboardSetup WithNumbers(bool enabled = true)
    {
        IncludeNumbers = enabled;
        return this;
    }

    public KeyboardSetup WithoutNumbers() => WithNumbers(false);

    public KeyboardSetup WithSpecialCharacters(bool enabled = true)
    {
        IncludeSpecialCharacters = enabled;
        return this;
    }

    public KeyboardSetup WithoutSpecialCharacters() => WithSpecialCharacters(false);

    public KeyboardSetup WithFunctionKeys(bool enabled = true)
    {
        IncludeFunctionKeys = enabled;
        return this;
    }

    public KeyboardSetup WithoutFunctionKeys() => WithFunctionKeys(false);

    public KeyboardSetup WithSystemKeys(bool enabled = true)
    {
        IncludeSystemKeys = enabled;
        return this;
    }

    public KeyboardSetup WithoutSystemKeys() => WithSystemKeys(false);

    public KeyboardSetup WithNavigationCluster(bool enabled = true)
    {
        IncludeNavigationCluster = enabled;
        return this;
    }

    public KeyboardSetup WithoutNavigationCluster() => WithNavigationCluster(false);

    public KeyboardSetup WithArrowKeys(bool enabled = true)
    {
        IncludeArrowKeys = enabled;
        return this;
    }

    public KeyboardSetup WithoutArrowKeys() => WithArrowKeys(false);

    public KeyboardSetup WithNumpad(bool enabled = true)
    {
        IncludeNumpad = enabled;
        return this;
    }

    public KeyboardSetup WithoutNumpad() => WithNumpad(false);

    public KeyboardSetup WithDesktopModifierKeys(bool enabled = true)
    {
        IncludeDesktopModifierKeys = enabled;
        return this;
    }

    public KeyboardSetup WithoutDesktopModifierKeys() => WithDesktopModifierKeys(false);

    public string ToCodeSnippet()
    {
        var codeSnippetBuilder = new StringBuilder($"{nameof(KeyboardSetup)}.{nameof(New)}");
        codeSnippetBuilder.Append($".{nameof(Layout)}({nameof(KeyboardLayout)}.{LayoutValue})");

        if (!IncludeLetters) codeSnippetBuilder.Append($".{nameof(WithoutLetters)}()");
        if (IncludeNumbers) codeSnippetBuilder.Append($".{nameof(WithNumbers)}()");
        if (IncludeSpecialCharacters) codeSnippetBuilder.Append($".{nameof(WithSpecialCharacters)}()");
        if (IncludeFunctionKeys) codeSnippetBuilder.Append($".{nameof(WithFunctionKeys)}()");
        if (IncludeDesktopModifierKeys) codeSnippetBuilder.Append($".{nameof(WithDesktopModifierKeys)}()");
        if (IncludeSystemKeys) codeSnippetBuilder.Append($".{nameof(WithSystemKeys)}()");
        if (IncludeNavigationCluster) codeSnippetBuilder.Append($".{nameof(WithNavigationCluster)}()");
        if (IncludeArrowKeys) codeSnippetBuilder.Append($".{nameof(WithArrowKeys)}()");
        if (IncludeNumpad) codeSnippetBuilder.Append($".{nameof(WithNumpad)}()");

        return codeSnippetBuilder.ToString();
    }
}