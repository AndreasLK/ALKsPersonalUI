namespace ALKsPersonalUI.Core;

public sealed class StyleBuilder
{
    private readonly List<string> _cssDeclarations = new();

    public StyleBuilder Add(string propertyName, string? propertyValue)
    {
        if (string.IsNullOrWhiteSpace(propertyName) || string.IsNullOrWhiteSpace(propertyValue))
            return this;

        var cleanName = propertyName.Trim();
        var cleanValue = propertyValue.Trim().TrimEnd(';');
        _cssDeclarations.Add($"{cleanName}: {cleanValue}");
        return this;
    }

    public StyleBuilder AddIf(bool condition, string propertyName, string? propertyValue)
    {
        if (!condition)
            return this;

        return Add(propertyName, propertyValue);
    }

    public StyleBuilder AddRaw(string? rawCssDeclarations)
    {
        if (string.IsNullOrWhiteSpace(rawCssDeclarations))
            return this;

        var trimmedDeclarations = rawCssDeclarations.Trim().TrimEnd(';');
        if (string.IsNullOrWhiteSpace(trimmedDeclarations))
            return this;

        _cssDeclarations.Add(trimmedDeclarations);
        return this;
    }

    public StyleBuilder AddRawIf(bool condition, string? rawCssDeclarations)
    {
        if (!condition)
            return this;

        return AddRaw(rawCssDeclarations);
    }

    public string Build() => string.Join("; ", _cssDeclarations);
}

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