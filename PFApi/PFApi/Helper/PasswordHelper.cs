using System.Security.Cryptography;

public static class PasswordHelper
{
    public static string HashPassword(string password)
    {
        // Generate a randomic salt
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[16];
        rng.GetBytes(salt);

        // Apply Password-Based Key Derivation Function 2 with 100000 iteractions
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);

        // Salvo insieme salt + hash, in Base64
        var hashBytes = new byte[48]; // 16 salt + 32 hash
        Buffer.BlockCopy(salt, 0, hashBytes, 0, 16);
        Buffer.BlockCopy(hash, 0, hashBytes, 16, 32);

        return Convert.ToBase64String(hashBytes);
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        var hashBytes = Convert.FromBase64String(storedHash);

        // Extract the salt
        var salt = new byte[16];
        Buffer.BlockCopy(hashBytes, 0, salt, 0, 16);

        // Extract the hash saved
        var storedHashBytes = new byte[32];
        Buffer.BlockCopy(hashBytes, 16, storedHashBytes, 0, 32);

        // Regenerate the hash with the same salt
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);

        // Byte-by-byte comparison in fixed time
        return CryptographicOperations.FixedTimeEquals(hash, storedHashBytes);
    }
}
