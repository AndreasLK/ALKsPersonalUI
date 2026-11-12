namespace ALKsPersonalUI.Inputs;

public record ValidationRule(Func<string, bool> IsValid, string ErrorMessage);