using System;
using System.Security.Cryptography;
using System.Text;

namespace ViewUI.Admin.IdentityServer.Helpers
{
    public static class StringEncryptExtensions
    {
        public static string SHA256Encrypt(this string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }

            return builder.ToString();
        }
    }
}
