using System.Security.Cryptography;
using System.Text;

namespace Nexora.Application.Utils;

public static class PasswordHasher
{
    private const int SaltSize = 16; 
    private const int KeySize = 32; 
    private const int Iterations = 100000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public static string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string password, string hashString)
    {
        var parts = hashString.Split(':');
        if (parts.Length != 2) return false;

        try
        {
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] originalHash = Convert.FromBase64String(parts[1]);

            if (salt.Length != SaltSize || originalHash.Length != KeySize) return false;

            byte[] hashToVerify = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

            return CryptographicOperations.FixedTimeEquals(originalHash, hashToVerify);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
