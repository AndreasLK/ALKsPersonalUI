using Microsoft.AspNetCore.Components.Web;

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