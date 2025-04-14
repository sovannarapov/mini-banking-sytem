using System.Security.Cryptography;

namespace Application.Services;

public static class AccountNumberGenerator
{
    private const int AccountNumberLength = 10;

    public static string Generate()
    {
        return string.Create<object?>(AccountNumberLength, null, (span, _) =>
        {
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = (char)(RandomNumberGenerator.GetInt32(0, 10) + '0');
            }
        });
    }
}
