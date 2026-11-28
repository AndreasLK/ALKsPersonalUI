using ALKsPersonalUI.Core;

namespace ALKsPersonalUI.Overlays;

public enum ToastVariant { Info, Success, Warning, Danger }

public record ToastMessage(Guid Id, string Text, ToastVariant Variant);

public class ToastService
{
    public event Action? OnChange;
    private readonly List<ToastMessage> _activeToasts = new();
    private readonly object _syncLock = new();

    public IReadOnlyList<ToastMessage> GetToasts()
    {
        lock (_syncLock)
        {
            return _activeToasts.ToArray();
        }
    }

    public void ShowInfo(string messageText) => Show(messageText, ToastVariant.Info);
    public void ShowSuccess(string messageText) => Show(messageText, ToastVariant.Success);
    public void ShowWarning(string messageText) => Show(messageText, ToastVariant.Warning);
    public void ShowDanger(string messageText) => Show(messageText, ToastVariant.Danger);

    public void Show(
        string messageText,
        ToastVariant variant = ToastVariant.Info,
        int durationMilliseconds = OverlayDefaults.Toast.DefaultDurationMilliseconds)
    {
        var newToast = new ToastMessage(Guid.NewGuid(), messageText, variant);
        lock (_syncLock)
        {
            _activeToasts.Add(newToast);
        }

        NotifyToastsChanged();
        _ = DismissAfterDelayAsync(newToast.Id, durationMilliseconds);
    }

    public void Remove(Guid toastId)
    {
        bool wasAnyToastRemoved;
        lock (_syncLock)
        {
            wasAnyToastRemoved = _activeToasts.RemoveAll(existingToast => existingToast.Id == toastId) > 0;
        }

        if (wasAnyToastRemoved)
            NotifyToastsChanged();
    }

    private async Task DismissAfterDelayAsync(Guid toastId, int durationMilliseconds)
    {
        await Task.Delay(durationMilliseconds);
        Remove(toastId);
    }

    private void NotifyToastsChanged() => OnChange?.Invoke();
}