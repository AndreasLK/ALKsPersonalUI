using ALKsPersonalUI.Core;
using System.Security.Cryptography;
using System.Text;

namespace ALKsPersonalUI.Inputs;

public readonly struct SecretString(string? rawPlaintextValue)
{
    private readonly string _plaintextValue = rawPlaintextValue ?? string.Empty;

    public bool IsEmpty => string.IsNullOrEmpty(_plaintextValue);

    public override string ToString() => InputDefaults.TextInput.RedactedTextPlaceholder;

    public string Hash(string salt, Func<string, string, string>? customHasher = null)
    {
        if (IsEmpty)
            return string.Empty;

        if (customHasher is not null)
            return customHasher(_plaintextValue, salt);

        var saltBytes = Encoding.UTF8.GetBytes(salt);
        var secretBytes = Encoding.UTF8.GetBytes(_plaintextValue);
        return Convert.ToHexString(HMACSHA256.HashData(saltBytes, secretBytes));
    }
}