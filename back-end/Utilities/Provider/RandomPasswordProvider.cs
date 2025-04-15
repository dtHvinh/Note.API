using PasswordGenerator;

namespace back_end.Utilities.Provider;

public sealed class RandomPasswordProvider
{
    private const int MaxRecursionLevel = 10;

    public static string? GeneratePassword(int length)
    {
        int level = 0;

        try
        {
            return GeneratePasswordInternal(length, level);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static string GeneratePasswordInternal(int length, int recursionLevel)
    {
        if (recursionLevel > MaxRecursionLevel)
        {
            throw new InvalidOperationException("Seomthing wrong with the password generator");
        }

        var password = new Password()
        {
            Settings = new PasswordGeneratorSettings(true, true, true, true, length, 2, false),
        };

        var strPassword = password.Next();

        return strPassword == "Try Again" ? GeneratePasswordInternal(length, recursionLevel + 1) : strPassword;
    }
}
