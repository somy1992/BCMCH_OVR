using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
namespace BCMCHOVR.Helpers
{
    public class Cryptography
    {
        public class MD5Cryptography
        {
            public static string Encrypt(string toEncrypt, bool useHashing)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(toEncrypt);
                string s = "Huy^93$Nre";
                byte[] numArray;
                if (useHashing)
                {
                    MD5CryptoServiceProvider cryptoServiceProvider = new MD5CryptoServiceProvider();
                    numArray = cryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(s));
                    cryptoServiceProvider.Clear();
                }
                else
                    numArray = Encoding.UTF8.GetBytes(s);
                TripleDESCryptoServiceProvider cryptoServiceProvider1 = new TripleDESCryptoServiceProvider();
                cryptoServiceProvider1.Key = numArray;
                cryptoServiceProvider1.Mode = CipherMode.ECB;
                cryptoServiceProvider1.Padding = PaddingMode.PKCS7;
                byte[] inArray = cryptoServiceProvider1.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
                cryptoServiceProvider1.Clear();
                return Convert.ToBase64String(inArray, 0, inArray.Length);
            }

            public static string Decrypt(string cipherString, bool useHashing)
            {
                byte[] inputBuffer = Convert.FromBase64String(cipherString);
                string s = "Huy^93$Nre";
                byte[] numArray;
                if (useHashing)
                {
                    MD5CryptoServiceProvider crpProvider = new MD5CryptoServiceProvider();
                    numArray = crpProvider.ComputeHash(Encoding.UTF8.GetBytes(s));
                    crpProvider.Clear();
                }
                else
                    numArray = Encoding.UTF8.GetBytes(s);
                TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider
                {
                    Key = numArray,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };
                byte[] bytes = cryptoServiceProvider.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
                cryptoServiceProvider.Clear();
                return Encoding.UTF8.GetString(bytes);
            }

        }
    }
}
