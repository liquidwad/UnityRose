using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using JsonFx.Json;
using System.IO;

namespace NetworkManager.Network
{
    static class Crypto
    {
        private const string AesKey = @"!QAZ2WSX#EDC4RFV";
        private const string AesIV = @"5TGB&YHN7UJM(IK<";
    
        public static void Decrypt(string message)
        {


            string plainText = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = UTF8Encoding.UTF8.GetBytes(AesKey);
                aesAlg.IV = UTF8Encoding.UTF8.GetBytes(AesIV);
                aesAlg.Padding = PaddingMode.None;
                aesAlg.Mode = CipherMode.CBC;

                Console.WriteLine(message);

                using (MemoryStream memoryDecrypt = new MemoryStream(UTF8Encoding.UTF8.GetBytes(message)))
                {
                    using (CryptoStream cryptStream = new CryptoStream(memoryDecrypt, aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV), CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptStream))
                        {
                            plainText = streamReader.ReadToEnd();
                        }
                    }
                }
            }

            Console.WriteLine(plainText);
        }
    }
}
