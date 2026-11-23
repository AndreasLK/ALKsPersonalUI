using ALKsPersonalUI.Core;
using ALKsPersonalUI.Inputs;
using ALKsPersonalUI.Overlays;

namespace ALKsPersonalUI.Tests.Core;

public class SecretStringKeyboardAndToastTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("SuperSecret123!")]
    public void SecretString_ToString_AlwaysReturnsRedactedPlaceholder(string? rawSecret)
    {
        var secret = new SecretString(rawSecret);

        Assert.Equal(InputDefaults.TextInput.RedactedTextPlaceholder, secret.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void SecretString_Hash_ReturnsEmptyStringWhenSecretIsEmpty(string? rawSecret)
    {
        var secret = new SecretString(rawSecret);

        Assert.True(secret.IsEmpty);
        Assert.Equal(string.Empty, secret.Hash("my-salt"));
    }

    [Fact]
    public void SecretString_Hash_ComputesDeterministicHmacAndSupportsCustomHasher()
    {
        var secret = new SecretString("kiosk-pin-42");

        var firstHash = secret.Hash("salt-value");
        var secondHash = secret.Hash("salt-value");
        var customHash = secret.Hash("salt-value", (value, salt) => $"{salt}:{value.Length}");

        Assert.False(secret.IsEmpty);
        Assert.Equal(firstHash, secondHash);
        Assert.Equal(64, firstHash.Length);
        Assert.Equal("salt-value:12", customHash);
    }

    [Fact]
    public void KeyboardSetup_PresetsCloneAndToCodeSnippet_ProduceExpectedConfiguration()
    {
        var touchStandard = KeyboardSetup.TouchStandard;
        var fullMacro = KeyboardSetup.FullMacro;
        var customSetup = KeyboardSetup.New
            .Layout(KeyboardLayout.QwertzDE)
            .WithoutLetters()
            .WithNumbers()
            .WithArrowKeys();

        var clonedMacro = fullMacro.Clone().WithoutNumpad();

        Assert.True(touchStandard.IncludeNumbers);
        Assert.True(touchStandard.IncludeSpecialCharacters);
        Assert.False(touchStandard.IncludeFunctionKeys);

        Assert.True(fullMacro.IncludeNumpad);
        Assert.False(clonedMacro.IncludeNumpad);

        var codeSnippet = customSetup.ToCodeSnippet();
        Assert.Contains("KeyboardLayout.QwertzDE", codeSnippet);
        Assert.Contains(".WithoutLetters()", codeSnippet);
        Assert.Contains(".WithNumbers()", codeSnippet);
        Assert.Contains(".WithArrowKeys()", codeSnippet);
    }

    [Fact]
    public void ToastService_ShowAndRemove_ManagesActiveToastsAndIgnoresUnknownIds()
    {
        var toastService = new ToastService();
        var changeCount = 0;
        toastService.OnChange += () => changeCount++;

        toastService.ShowInfo("Info message");
        toastService.ShowSuccess("Success message");
        toastService.ShowWarning("Warning message");
        toastService.ShowDanger("Danger message");

        Assert.Equal(4, toastService.GetToasts().Count);
        Assert.Equal(4, changeCount);

        toastService.Remove(Guid.NewGuid());
        Assert.Equal(4, changeCount);

        var firstToastId = toastService.GetToasts()[0].Id;
        toastService.Remove(firstToastId);

        Assert.Equal(3, toastService.GetToasts().Count);
        Assert.Equal(5, changeCount);
    }

    /// <summary>
    /// Verifies that ToastService automatically removes a toast after its configured duration elapses.
    /// </summary>
    [Fact]
    public async Task ToastService_AutoDismissesToastAfterConfiguredDuration()
    {
        const int toastDurationMilliseconds = 40;
        const int autoDismissTimeoutSeconds = 3;

        var toastService = new ToastService();
        var dismissedSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        toastService.OnChange += () =>
        {
            if (toastService.GetToasts().Count == 0)
            {
                dismissedSource.TrySetResult();
            }
        };

        toastService.Show("Brief notice", ToastVariant.Info, durationMilliseconds: toastDurationMilliseconds);
        Assert.Single(toastService.GetToasts());

        // Wait on the OnChange dismissal signal instead of a fixed sleep that can flake under CI threadpool load.
        await dismissedSource.Task.WaitAsync(TimeSpan.FromSeconds(autoDismissTimeoutSeconds));

        Assert.Empty(toastService.GetToasts());
    }
}

