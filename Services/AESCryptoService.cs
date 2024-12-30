using System.Security.Cryptography;
using System.Text;

namespace API_CredentialManager.Services
{
    public class AESCryptoService
    {
        private readonly byte[] _key; // 256-bit key
        private readonly byte[] _iv;  // 128-bit initialization vector (IV)

        // Constructor to initialize AES Key and IV
        public AESCryptoService(string key)
        {
            if (key.Length != 32) // Ensure the key is 256 bits
                throw new ArgumentException("Key must be 32 characters (256 bits).");

            _key = Encoding.UTF8.GetBytes(key);
            _iv = GenerateIV(); // IV can be generated or fixed, depending on the use case
        }

        // Generate a new random IV
        private byte[] GenerateIV()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] iv = new byte[16]; // AES uses a 128-bit IV
                rng.GetBytes(iv);
                return iv;
            }
        }

        // Encrypt a plaintext string
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));

            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    // Write the IV at the start of the encrypted payload
                    ms.Write(_iv, 0, _iv.Length);

                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        // Decrypt an encrypted string
        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException(nameof(cipherText));

            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Read the IV from the start of the payload
                byte[] iv = new byte[16];
                Array.Copy(cipherBytes, 0, iv, 0, iv.Length);
                aes.IV = iv;

                // Get the encrypted data (excluding the IV)
                byte[] encryptedBytes = new byte[cipherBytes.Length - iv.Length];
                Array.Copy(cipherBytes, iv.Length, encryptedBytes, 0, encryptedBytes.Length);

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(encryptedBytes))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
