using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace JWTApi.Infrastructure.Middleware
{


    public static class AesEncryption
    {
        private static readonly int KeySize = 256;
        private static readonly int BlockSize = 128;
        private static readonly int IvSize = 16; // 128 bits

        public static byte[] Encrypt(string plainText, string base64Key, out string ivBase64)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(base64Key);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // تولید IV تصادفی
                aes.GenerateIV();
                ivBase64 = Convert.ToBase64String(aes.IV);

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                        cs.Write(plainBytes, 0, plainBytes.Length);
                        cs.FlushFinalBlock();
                    }

                    return ms.ToArray();
                }
            }
        }

        public static string Decrypt(byte[] cipherText, string base64Key, string ivBase64)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(base64Key);
                aes.IV = Convert.FromBase64String(ivBase64);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(cipherText))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        // تولید کلید جدید (فقط برای راه‌اندازی اولیه)
        public static string GenerateKey()
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.GenerateKey();
                return Convert.ToBase64String(aes.Key);
            }
        }
    }
}
