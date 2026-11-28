namespace ALKsPersonalUI.Json;

/// <summary>
/// Compile-time allow-listed comparison operators for declarative visibility and disabled conditions.
/// </summary>
public enum JsonUiConditionOperator
{
    /// <summary>
    /// True when the state value equals the expected value.
    /// </summary>
    Equals,

    /// <summary>
    /// True when the state value does not equal the expected value.
    /// </summary>
    NotEquals,

    /// <summary>
    /// True when both the state value and expected value are numbers and state is greater.
    /// </summary>
    GreaterThan,

    /// <summary>
    /// True when both the state value and expected value are numbers and state is smaller.
    /// </summary>
    LessThan,

    /// <summary>
    /// True when the state key is missing, null, or whitespace-only string.
    /// </summary>
    IsEmpty,

    /// <summary>
    /// True when the state key has a non-null, non-whitespace value.
    /// </summary>
    IsNotEmpty,

    /// <summary>
    /// True when the state value represents boolean true.
    /// </summary>
    IsTrue,

    /// <summary>
    /// True when the state value is missing, null, or represents boolean false.
    /// </summary>
    IsFalse
}
