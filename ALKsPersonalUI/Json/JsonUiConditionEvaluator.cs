namespace ALKsPersonalUI.Json;

/// <summary>
/// Evaluates declarative <see cref="JsonUiCondition"/> rules against <see cref="JsonUiSessionState"/> using fail-closed semantics.
/// </summary>
public static class JsonUiConditionEvaluator
{
    /// <summary>
    /// Evaluates <paramref name="condition"/> against <paramref name="state"/>. Returns <c>true</c> when <paramref name="condition"/> is null.
    /// </summary>
    public static bool Evaluate(JsonUiCondition? condition, JsonUiSessionState state)
    {
        if (condition is null)
        {
            return true;
        }

        // Fail closed when a condition omits its target state key.
        if (string.IsNullOrWhiteSpace(condition.Key))
        {
            return false;
        }

        var hasValue = state.TryGetValue(condition.Key, out var actualValue);
        var expectedValue = condition.Value.HasValue
            ? JsonUiSessionState.NormalizeJsonElement(condition.Value.Value)
            : null;

        return condition.Operator switch
        {
            JsonUiConditionOperator.Equals => AreEqual(actualValue, expectedValue),
            JsonUiConditionOperator.NotEquals => !AreEqual(actualValue, expectedValue),
            JsonUiConditionOperator.GreaterThan => CompareNumbers(actualValue, expectedValue) > 0,
            JsonUiConditionOperator.LessThan => CompareNumbers(actualValue, expectedValue) < 0,
            JsonUiConditionOperator.IsEmpty => !hasValue || IsNullOrWhitespace(actualValue),
            JsonUiConditionOperator.IsNotEmpty => hasValue && !IsNullOrWhitespace(actualValue),
            JsonUiConditionOperator.IsTrue => IsTruthy(actualValue),
            JsonUiConditionOperator.IsFalse => !IsTruthy(actualValue),
            _ => false
        };
    }

    private static bool AreEqual(object? actual, object? expected)
    {
        if (actual is null && expected is null)
        {
            return true;
        }

        if (actual is null || expected is null)
        {
            return false;
        }

        if (JsonUiSessionState.TryConvertToDouble(actual, out var actualNum)
            && JsonUiSessionState.TryConvertToDouble(expected, out var expectedNum))
        {
            return Math.Abs(actualNum - expectedNum) < 0.000001;
        }

        return string.Equals(
            JsonUiSessionState.FormatValue(actual),
            JsonUiSessionState.FormatValue(expected),
            StringComparison.Ordinal);
    }

    private static int? CompareNumbers(object? actual, object? expected)
    {
        if (!JsonUiSessionState.TryConvertToDouble(actual, out var actualNum)
            || !JsonUiSessionState.TryConvertToDouble(expected, out var expectedNum))
        {
            return null;
        }

        return actualNum.CompareTo(expectedNum);
    }

    private static bool IsNullOrWhitespace(object? value) =>
        value is null || (value is string text && string.IsNullOrWhiteSpace(text));

    private static bool IsTruthy(object? value) => value switch
    {
        bool flag => flag,
        double number => number != 0,
        string text when bool.TryParse(text, out var parsed) => parsed,
        _ => false
    };
}
