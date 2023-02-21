using BCrypt.Net;

namespace API.Security
{
    public class HashProvider
    {
        public static bool CheckHash(string target, string hash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(target, hash, HashType.SHA512);
        }
        public static string MakeHash(string target)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(target, HashType.SHA512);
        }
    }
}
