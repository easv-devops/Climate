using System.Security.Cryptography;
using System.Text;


namespace api.security;

public class CredentialService
{
    public string GenerateSalt()
    {
        var bytes = new byte[128 / 8];
        using var keyGenerator = RandomNumberGenerator.Create();
        keyGenerator.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    public string Hash(string password, string salt)
    {
        try
        {
            var bytes = Encoding.UTF8.GetBytes(password + salt);
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        catch (Exception e)
        {
            //todo should be Logged and caught in a global exception handler. 
            throw new InvalidOperationException("Failed to hash password");
        }
    }
}