using CurrencyExchange.Core.Services;
using System.Security.Cryptography;
using System.Text;

namespace CurrencyExchange.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));

            var builder = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));

            }
            return builder.ToString();
        }

        public bool VerifyHash(string password, string hash)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));

            var builder = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));

            }
            return builder.ToString().Equals(hash);
        }
    }
}
