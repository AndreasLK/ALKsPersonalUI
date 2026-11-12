using System.Globalization;

namespace ALKsPersonalUI.Core;

public sealed class UIModifier
{
    private StyleBuilder _extraFrameStyles = new();
    private StyleBuilder _extraBodyStyles = new();
    private CssBuilder _cssBuilder = new();

    public static UIModifier Empty => new();
    public static UIModifier New => new();

    public string? WidthValue { get; private set; }
    public string? HeightValue { get; private set; }
    public string? MinWidthValue { get; private set; }
    public string? MinHeightValue { get; private set; }
    public string? PaddingValue { get; private set; }
    public string? PaddingVerticalValue { get; private set; }
    public string? PaddingHorizontalValue { get; private set; }
    public string? GapValue { get; private set; }
    public string? DisplayValue { get; private set; }
    public string? FlexDirectionValue { get; private set; }
    public string? AlignItemsValue { get; private set; }
    public string? JustifyContentValue { get; private set; }
    public string? OverflowValue { get; private set; }
    public string? BackgroundValue { get; private set; }
    public string? HeaderBackgroundValue { get; private set; }
    public string? BorderColorValue { get; private set; }
    public string? BorderRadiusValue { get; private set; }
    public string? BaseColorValue { get; private set; }
    public string? TextColorValue { get; private set; }
    public string? AccentColorValue { get; private set; }
    public string? DepthValue { get; private set; }
    public string? AnimationSpeedValue { get; private set; }
    public string? FontSizeValue { get; private set; }
    public string? ThicknessValue { get; private set; }
    public string? PressScaleValue { get; private set; }

    public UIModifier Clone()
    {
        var copy = (UIModifier)MemberwiseClone();
        copy._extraFrameStyles = new StyleBuilder().AddRaw(_extraFrameStyles.Build());
        copy._extraBodyStyles = new StyleBuilder().AddRaw(_extraBodyStyles.Build());
        copy._cssBuilder = new CssBuilder().AddRaw(_cssBuilder.Build());
        return copy;
    }

    public UIModifier Width(string width)
    {
        WidthValue = width;
        return this;
    }

    public UIModifier Width(double pixels) => Width(FormatPixels(pixels));

    public UIModifier Height(string height)
    {
        HeightValue = height;
        return this;
    }

    public UIModifier Height(double pixels) => Height(FormatPixels(pixels));

    public UIModifier Size(string uniformSize) => Width(uniformSize).Height(uniformSize);
    public UIModifier Size(double uniformPixels) => Size(FormatPixels(uniformPixels));
    public UIModifier Size(string width, string height) => Width(width).Height(height);
    public UIModifier Size(double widthPixels, double heightPixels) => Width(widthPixels).Height(heightPixels);

    public UIModifier MinWidth(string minWidth)
    {
        MinWidthValue = minWidth;
        return this;
    }

    public UIModifier MinWidth(double pixels) => MinWidth(FormatPixels(pixels));

    public UIModifier MinHeight(string minHeight)
    {
        MinHeightValue = minHeight;
        return this;
    }

    public UIModifier MinHeight(double pixels) => MinHeight(FormatPixels(pixels));

    public UIModifier MaxWidth(string maxWidth) => FrameStyle("max-width", maxWidth);
    public UIModifier MaxWidth(double pixels) => MaxWidth(FormatPixels(pixels));
    public UIModifier MaxHeight(string maxHeight) => FrameStyle("max-height", maxHeight);
    public UIModifier MaxHeight(double pixels) => MaxHeight(FormatPixels(pixels));

    public UIModifier FillMaxWidth() => Width("100%");
    public UIModifier FillMaxHeight() => Height("100%");
    public UIModifier FillMaxSize() => Width("100%").Height("100%");
    public UIModifier WrapContentWidth() => Width("fit-content");
    public UIModifier WrapContentHeight() => Height("fit-content");
    public UIModifier WrapContentSize() => Width("fit-content").Height("fit-content");
    public UIModifier Weight(double flexWeight = 1.0) => FlexGrow(flexWeight).FlexShrink(1).FrameStyle("flex-basis", "0%");
    public UIModifier AspectRatio(string ratio) => FrameStyle("aspect-ratio", ratio);
    public UIModifier AspectRatio(double widthRatio, double heightRatio) =>
        AspectRatio($"{GeometryMath.FormatInvariant(widthRatio)} / {GeometryMath.FormatInvariant(heightRatio)}");

    public UIModifier Padding(string padding)
    {
        PaddingValue = padding;
        var segments = padding.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        PaddingVerticalValue = segments.Length > 0 ? segments[0] : padding;
        PaddingHorizontalValue = segments.Length > 1 ? segments[1] : PaddingVerticalValue;
        return this;
    }

    public UIModifier Padding(double uniformPixels) => Padding(FormatPixels(uniformPixels));

    public UIModifier Padding(string vertical, string horizontal)
    {
        PaddingVerticalValue = vertical;
        PaddingHorizontalValue = horizontal;
        PaddingValue = $"{vertical} {horizontal}";
        return this;
    }

    public UIModifier Padding(double verticalPixels, double horizontalPixels) =>
        Padding(FormatPixels(verticalPixels), FormatPixels(horizontalPixels));

    public UIModifier Padding(string top, string right, string bottom, string left)
    {
        PaddingVerticalValue = top;
        PaddingHorizontalValue = right;
        PaddingValue = $"{top} {right} {bottom} {left}";
        return this;
    }

    public UIModifier Padding(double topPx, double rightPx, double bottomPx, double leftPx) =>
        Padding(FormatPixels(topPx), FormatPixels(rightPx), FormatPixels(bottomPx), FormatPixels(leftPx));

    public UIModifier Margin(string margin) => FrameStyle("margin", margin);
    public UIModifier Margin(double uniformPixels) => Margin(FormatPixels(uniformPixels));
    public UIModifier Margin(string vertical, string horizontal) => FrameStyle("margin", $"{vertical} {horizontal}");
    public UIModifier Margin(double verticalPx, double horizontalPx) => Margin(FormatPixels(verticalPx), FormatPixels(horizontalPx));
    public UIModifier Margin(string top, string right, string bottom, string left) => FrameStyle("margin", $"{top} {right} {bottom} {left}");
    public UIModifier Margin(double topPx, double rightPx, double bottomPx, double leftPx) =>
        Margin(FormatPixels(topPx), FormatPixels(rightPx), FormatPixels(bottomPx), FormatPixels(leftPx));

    public UIModifier Display(string display)
    {
        DisplayValue = display;
        return this;
    }

    public UIModifier FlexDirection(string direction)
    {
        FlexDirectionValue = direction;
        return this;
    }

    public UIModifier Row() => Display("flex").FlexDirection("row");
    public UIModifier Column() => Display("flex").FlexDirection("column");

    public UIModifier AlignItems(string alignItems)
    {
        AlignItemsValue = alignItems;
        return this;
    }

    public UIModifier AlignItems(HorizontalAlignment alignment) => AlignItems(LayoutDefaults.ToCss(alignment));
    public UIModifier AlignItems(VerticalAlignment alignment) => AlignItems(LayoutDefaults.ToCss(alignment));

    public UIModifier JustifyContent(string justifyContent)
    {
        JustifyContentValue = justifyContent;
        return this;
    }

    public UIModifier JustifyContent(Arrangement arrangement) => JustifyContent(LayoutDefaults.ToCss(arrangement));

    public UIModifier AlignBox(BoxAlignment alignment)
    {
        var (alignItems, justifyContent) = LayoutDefaults.ToCss(alignment);
        return AlignItems(alignItems).JustifyContent(justifyContent);
    }

    public UIModifier CenterContent() => AlignItems("center").JustifyContent("center");

    public UIModifier Gap(string gap)
    {
        GapValue = gap;
        return this;
    }

    public UIModifier Gap(double pixels) => Gap(FormatPixels(pixels));

    public UIModifier FlexWrap(string wrap) => BodyStyle("flex-wrap", wrap);
    public UIModifier Wrap() => FlexWrap("wrap");
    public UIModifier FlexGrow(double growFactor) => FrameStyle("flex-grow", growFactor.ToString(CultureInfo.InvariantCulture));
    public UIModifier FlexShrink(double shrinkFactor) => FrameStyle("flex-shrink", shrinkFactor.ToString(CultureInfo.InvariantCulture));
    public UIModifier AlignSelf(string alignSelf) => FrameStyle("align-self", alignSelf);
    public UIModifier AlignSelf(HorizontalAlignment alignment) => AlignSelf(LayoutDefaults.ToCss(alignment));
    public UIModifier AlignSelf(VerticalAlignment alignment) => AlignSelf(LayoutDefaults.ToCss(alignment));

    public UIModifier Background(string backgroundColor)
    {
        BackgroundValue = backgroundColor;
        return this;
    }

    public UIModifier HeaderBackground(string headerBackgroundColor)
    {
        HeaderBackgroundValue = headerBackgroundColor;
        return this;
    }

    public UIModifier BorderColor(string borderColor)
    {
        BorderColorValue = borderColor;
        return this;
    }

    public UIModifier BorderRadius(string borderRadius)
    {
        BorderRadiusValue = borderRadius;
        return this;
    }

    public UIModifier BorderRadius(double pixels) => BorderRadius(FormatPixels(pixels));
    public UIModifier ClipRounded(double pixels) => BorderRadius(pixels).Overflow("hidden");
    public UIModifier ClipCircle() => BorderRadius("9999px").Overflow("hidden");

    public UIModifier Border(string borderShorthand) => FrameStyle("border", borderShorthand);
    public UIModifier Border(string thickness, string borderColor) => FrameStyle("border", $"{thickness} solid {borderColor}").BorderColor(borderColor);
    public UIModifier Border(double thicknessPixels, string borderColor) => Border(FormatPixels(thicknessPixels), borderColor);
    public UIModifier BoxShadow(string shadow) => FrameStyle("box-shadow", shadow);
    public UIModifier Opacity(double opacity) => FrameStyle("opacity", opacity.ToString(CultureInfo.InvariantCulture));

    public UIModifier BaseColor(string baseColor)
    {
        BaseColorValue = baseColor;
        return this;
    }

    public UIModifier TextColor(string textColor)
    {
        TextColorValue = textColor;
        return this;
    }

    public UIModifier AccentColor(string accentColor)
    {
        AccentColorValue = accentColor;
        return this;
    }

    public UIModifier Depth(string depth)
    {
        DepthValue = depth;
        return this;
    }

    public UIModifier Depth(double pixels) => Depth(FormatPixels(pixels));

    public UIModifier AnimationSpeed(string speed)
    {
        AnimationSpeedValue = speed;
        return this;
    }

    public UIModifier FontSize(string fontSize)
    {
        FontSizeValue = fontSize;
        return this;
    }

    public UIModifier FontSize(double pixels) => FontSize(FormatPixels(pixels));
    public UIModifier FontWeight(string fontWeight) => FrameStyle("font-weight", fontWeight);
    public UIModifier FontWeight(FontWeight weight) => FontWeight(LayoutDefaults.ToCss(weight));

    public UIModifier Thickness(string thickness)
    {
        ThicknessValue = thickness;
        return this;
    }

    public UIModifier Thickness(double pixels) => Thickness(FormatPixels(pixels));

    public UIModifier Overflow(string overflow)
    {
        OverflowValue = overflow;
        return this;
    }

    public UIModifier OverflowX(string overflowX) => BodyStyle("overflow-x", overflowX);
    public UIModifier OverflowY(string overflowY) => BodyStyle("overflow-y", overflowY);
    public UIModifier VerticalScroll() => OverflowY("auto");
    public UIModifier HorizontalScroll() => OverflowX("auto");
    public UIModifier Position(string position) => FrameStyle("position", position);
    public UIModifier ZIndex(int zIndex) => FrameStyle("z-index", zIndex.ToString(CultureInfo.InvariantCulture));
    public UIModifier Cursor(string cursor) => FrameStyle("cursor", cursor);
    public UIModifier Clickable() => Cursor("pointer").FrameStyle("user-select", "none");

    public UIModifier FrameStyle(string propertyName, string? propertyValue)
    {
        _extraFrameStyles.Add(propertyName, propertyValue);
        return this;
    }

    public UIModifier BodyStyle(string propertyName, string? propertyValue)
    {
        _extraBodyStyles.Add(propertyName, propertyValue);
        return this;
    }

    public UIModifier Style(string propertyName, string? propertyValue) =>
        FrameStyle(propertyName, propertyValue);

    public UIModifier RawStyle(string? rawCss)
    {
        _extraFrameStyles.AddRaw(rawCss);
        return this;
    }

    public UIModifier RawBodyStyle(string? rawCss)
    {
        _extraBodyStyles.AddRaw(rawCss);
        return this;
    }

    public UIModifier Class(string? className, bool when = true)
    {
        _cssBuilder.Add(className, when);
        return this;
    }

    public UIModifier PressScale(double scaleFactor)
    {
        PressScaleValue = GeometryMath.FormatInvariant(scaleFactor);
        return this;
    }

    public string BuildWindowStyle() => new StyleBuilder()
        .Add("--_w-radius", BorderRadiusValue ?? WindowDefaults.DefaultBorderRadius)
        .Add("--_w-bg", BackgroundValue ?? WindowDefaults.DefaultBackground)
        .Add("--_w-border", BorderColorValue ?? WindowDefaults.DefaultBorderColor)
        .Add("--_w-display", DisplayValue ?? WindowDefaults.DefaultDisplay)
        .Add("--_w-flex-dir", FlexDirectionValue ?? WindowDefaults.DefaultFlexDirection)
        .Add("--_w-align", AlignItemsValue ?? WindowDefaults.DefaultAlignItems)
        .Add("--_w-justify", JustifyContentValue ?? WindowDefaults.DefaultJustifyContent)
        .Add("--_w-gap", GapValue ?? WindowDefaults.DefaultGap)
        .Add("--_w-padding", PaddingValue ?? WindowDefaults.DefaultPadding)
        .Add("--_w-width", WidthValue ?? WindowDefaults.DefaultWidth)
        .Add("--_w-height", HeightValue ?? WindowDefaults.DefaultHeight)
        .Add("--_w-min-width", MinWidthValue ?? WindowDefaults.DefaultMinWidth)
        .Add("--_w-min-height", MinHeightValue ?? WindowDefaults.DefaultMinHeight)
        .Add("--_w-overflow", OverflowValue ?? WindowDefaults.DefaultOverflow)
        .AddRaw(_extraFrameStyles.Build())
        .AddRaw(_extraBodyStyles.Build())
        .Build();

    public UIModifier ForTitledWindowFrame(string? defaultWidth = null) => new UIModifier()
        .Width(WidthValue ?? defaultWidth ?? WindowDefaults.DefaultWidth)
        .Height(HeightValue ?? WindowDefaults.DefaultHeight)
        .MinWidth(MinWidthValue ?? WindowDefaults.DefaultMinWidth)
        .MinHeight(MinHeightValue ?? WindowDefaults.DefaultMinHeight)
        .BorderRadius(BorderRadiusValue ?? WindowDefaults.DefaultBorderRadius)
        .Background(BackgroundValue ?? WindowDefaults.DefaultBackground)
        .BorderColor(BorderColorValue ?? WindowDefaults.DefaultBorderColor)
        .Padding(WindowDefaults.ZeroPadding)
        .Gap(WindowDefaults.ZeroPadding)
        .Column()
        .AlignItems("stretch")
        .JustifyContent("flex-start")
        .Overflow("hidden")
        .RawStyle(_extraFrameStyles.Build())
        .Class(_cssBuilder.Build());

    public string BuildTitleBarStyle(string? fallbackHeaderBackground = null) => new StyleBuilder()
        .Add("--_tw-header-bg", HeaderBackgroundValue ?? fallbackHeaderBackground ?? WindowDefaults.DefaultHeaderBackground)
        .Add("--_tw-border", BorderColorValue ?? WindowDefaults.DefaultBorderColor)
        .Add("--_tw-radius", BorderRadiusValue ?? WindowDefaults.DefaultBorderRadius)
        .Build();

    public string BuildTitledWindowBodyStyle(string? fallbackPadding = null) => new StyleBuilder()
        .Add("--_w-body-padding", PaddingValue ?? fallbackPadding ?? WindowDefaults.DefaultPadding)
        .Add("--_w-body-display", DisplayValue ?? WindowDefaults.DefaultDisplay)
        .Add("--_w-body-flex-dir", FlexDirectionValue ?? WindowDefaults.DefaultFlexDirection)
        .Add("--_w-body-align", AlignItemsValue ?? WindowDefaults.DefaultAlignItems)
        .Add("--_w-body-justify", JustifyContentValue ?? WindowDefaults.DefaultJustifyContent)
        .Add("--_w-body-gap", GapValue ?? WindowDefaults.DefaultGap)
        .Add("--_w-body-overflow", OverflowValue ?? WindowDefaults.DefaultOverflow)
        .AddRaw(_extraBodyStyles.Build())
        .Build();

    public string BuildStyle() => new StyleBuilder()
        .Add("width", WidthValue)
        .Add("height", HeightValue)
        .Add("min-width", MinWidthValue)
        .Add("min-height", MinHeightValue)
        .Add("padding", PaddingValue)
        .Add("display", DisplayValue)
        .Add("flex-direction", FlexDirectionValue)
        .Add("align-items", AlignItemsValue)
        .Add("justify-content", JustifyContentValue)
        .Add("gap", GapValue)
        .Add("overflow", OverflowValue)
        .Add("background-color", BackgroundValue)
        .Add("border-color", BorderColorValue)
        .Add("border-radius", BorderRadiusValue)
        .Add("color", TextColorValue)
        .Add("font-size", FontSizeValue)
        .AddRaw(_extraFrameStyles.Build())
        .AddRaw(_extraBodyStyles.Build())
        .Build();

    public string BuildFrameExtras() => _extraFrameStyles.Build();
    public string BuildClass() => _cssBuilder.Build();

    private static string FormatPixels(double pixels) =>
        $"{GeometryMath.FormatInvariant(pixels)}px";
}