using System.Security.Cryptography;
using System.Text;

namespace StocksData.Models
{
    /// <summary>
    /// Encyption class allowing to securely encrypt and decrypt information. Relies on the local machine's authentication. May be decrypted when given the earlier provided entropy and executing decryption on the same machine. Fully secure only when moved to a different machine or the earlier provided entropy is not accessible. Available only in the Windows Operating System.
    /// </summary>
    public static class Encryption
    {
        /// <summary>
        /// Method to encrypt given data for the machine its running on
        /// </summary>
        /// <param name="value">The data to be encrypted</param>
        /// <param name="valuecipher">The generated cipher</param>
        /// <param name="valueentropy">The entropy allowing for more secure encryption</param>
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

        /// <summary>
        /// Method to decrypt encrypted data on the machine it has been encrypted on when provided the cipher and the entropy.
        /// </summary>
        /// <param name="cipher">The cipher generated from the encryption</param>
        /// <param name="entropy">The entropy generated from the encryption making it more secure</param>
        /// <returns>The decrypted data stored in plain text</returns>
        public static string Decrypt(byte[] cipher, byte[] entropy)
        {
            var data = ProtectedData.Unprotect(cipher, entropy, DataProtectionScope.LocalMachine);
            return Encoding.UTF8.GetString(data);
        }
    }
}