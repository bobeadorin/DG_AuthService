using System.Security.Cryptography;
using System.Text;

namespace AuthService.Helpers
{

    public static class Hashing
    {
        public static string toSHA256(string password)
        {
            using var sha256 = SHA256.Create();

            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            var sb = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
