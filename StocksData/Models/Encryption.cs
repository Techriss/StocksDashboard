using System.Security.Cryptography;
using System.Text;

namespace StocksData.Models
{
    public static class Encryption
    {
        public static void Encrypt(ref string value, out byte[] valuecipher, out byte[] valueentropy)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            var entropy = new byte[20];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(entropy);
            }

            var cipher = ProtectedData.Protect(bytes, entropy, DataProtectionScope.LocalMachine);

            valuecipher = cipher;
            valueentropy = entropy;
        }

        public static string Decrypt(byte[] cipher, byte[] entropy)
        {
            var data = ProtectedData.Unprotect(cipher, entropy, DataProtectionScope.LocalMachine);
            return Encoding.UTF8.GetString(data);
        }
    }
}