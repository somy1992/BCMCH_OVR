using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BCMCHOVR.Helpers{

    public static class Encryption
    {
        public const string defaultPassKey = "bcmch#lovehope@";

        private const string initVector = "hopenheabcmchdev";
        private const int keysize = 256;
        private const string passPhrase = "BCMCH#678gdm@";

        public static string EncryptString(string plainText)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged
            {
                Mode = CipherMode.CBC
            };
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }

        //Decrypt
        public static string DecryptString(string cipherText)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged
            {
                Mode = CipherMode.CBC
            };
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }
    }

    public class AESEncryption
    {
        private readonly byte[] key =
    {
        85, 45, 114, 184, 27, 162, 37, 112, 222, 209, 241,218, 131, 236, 53, 255,123, 217, 19, 101, 24, 26,  24, 175, 144, 173, 53, 196, 29, 24, 26, 17
        };

        private readonly byte[] vector =
        {
        146, 64, 191, 111,121, 254, 112, 79, 32, 114, 22, 23, 30, 113, 119, 231
        };

        private readonly ICryptoTransform encryptor;
        private readonly ICryptoTransform decryptor;
        private readonly UTF8Encoding encoder;

        public AESEncryption()
        {
            using (var rm = new RijndaelManaged())
            {
                encryptor = rm.CreateEncryptor(key, vector);
                decryptor = rm.CreateDecryptor(key, vector);
            }

            encoder = new UTF8Encoding();
        }

        public string EncryptString(string stringValue)
        {
            var encrptedText = Convert.ToBase64String(Encrypt(encoder.GetBytes(stringValue)));
            return encrptedText.Base64UrlEncode();
        }

        public string DecryptString(string encryptedString)
        {
            return encoder.GetString(Decrypt(Convert.FromBase64String(encryptedString.Base64UrlDecode())));
        }

        private byte[] Encrypt(byte[] buffer)
        {
            return Transform(buffer, encryptor);
        }

        private byte[] Decrypt(byte[] buffer)
        {
            return Transform(buffer, decryptor);
        }

        private byte[] Transform(byte[] buffer, ICryptoTransform transform)
        {
            using (var stream = new MemoryStream())
            {
                using (var cs = new CryptoStream(stream, transform, CryptoStreamMode.Write))
                {
                    cs.Write(buffer, 0, buffer.Length);
                }
                return stream.ToArray();
            }
        }
    }

    public static class CryptographyExtensions
    {
        /*------Healper Functions -------------------*/

        public static string Base64UrlEncode(this string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            var s = Convert.ToBase64String(bytes); // Regular base64 encoder
            s = s.Split('=')[0]; // Remove any trailing '='s
            s = s.Replace('+', '-'); // 62nd char of encoding
            s = s.Replace('/', '_'); // 63rd char of encoding
            return s;
        }

        public static string Base64UrlDecode(this string value)
        {
            if (string.IsNullOrEmpty(value)) { return ""; }
            var s = value;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding
            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0:
                    break; // No pad chars in this case
                case 2:
                    s += "==";
                    break; // Two pad chars
                case 3:
                    s += "=";
                    break; // One pad char
                default:
                    throw new Exception("Illegal base64 url string!");
            }
            var bytes = Convert.FromBase64String(s); // Standard base64 decoder
            return Encoding.UTF8.GetString(bytes);
        }
    }
}