namespace ALKsPersonalUI.Core;

public sealed class CssBuilder
{
    private readonly List<string> _cssClassNames = new();

    public CssBuilder Add(string? className, bool when = true)
    {
        if (!when || string.IsNullOrWhiteSpace(className))
            return this;

        _cssClassNames.Add(className.Trim());
        return this;
    }

    public CssBuilder AddRaw(string? rawClassNames) => Add(rawClassNames);

    public string Build() => string.Join(" ", _cssClassNames);
}

