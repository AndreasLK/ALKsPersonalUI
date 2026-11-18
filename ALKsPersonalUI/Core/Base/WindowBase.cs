namespace ALKsPersonalUI.Core;

public abstract class WindowBase : UIComponentBase
{
    protected string BuildWindowCssClasses() => new CssBuilder()
        .Add("window")
        .AddRaw(ModifierClass)
        .Build();

    protected string BuildWindowVariables() =>
        ModifierOrEmpty.BuildWindowStyle();
}