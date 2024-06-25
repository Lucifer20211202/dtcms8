using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace DTcms.Core.Common.Helpers
{
    /// <summary>
    /// 微信解密帮助类
    /// </summary>
    public class WeChatHelper
    {
        /// <summary>
        /// 加密微信数据
        /// </summary>
        /// <param name="data">数据明文</param>
        /// <param name="encryptIv">iv向量</param>
        /// <param name="sessionKey">调用 wx auth.code2Session 来获得</param>
        public static string EncryptWeChatData(string data, string encryptIv, string sessionKey)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] keyBytes = Convert.FromBase64String(sessionKey);
            byte[] ivBytes = Convert.FromBase64String(encryptIv);

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = keyBytes;
            aesAlg.IV = ivBytes;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msEncrypt = new();
            using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                csEncrypt.Write(dataBytes, 0, dataBytes.Length);
                csEncrypt.FlushFinalBlock();
            }
            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        /// <summary>
        /// 解密微信数据
        /// </summary>
        /// <param name="encryptedData">加密的数据</param>
        /// <param name="encryptIv">iv向量</param>
        /// <param name="sessionKey">调用 wx auth.code2Session 来获得</param>
        public static T? Decrypt<T>(string encryptedData, string encryptIv, string sessionKey)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
            byte[] keyBytes = Convert.FromBase64String(sessionKey);
            byte[] ivBytes = Convert.FromBase64String(encryptIv);

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = keyBytes;
            aesAlg.IV = ivBytes;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new(encryptedBytes);
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            var jsonString = srDecrypt.ReadToEnd();

            return JsonConvert.DeserializeObject<T>(jsonString);
        }

    }
}
