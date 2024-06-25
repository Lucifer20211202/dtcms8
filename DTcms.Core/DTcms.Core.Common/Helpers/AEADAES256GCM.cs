using System.Security.Cryptography;
using System.Text;

namespace DTcms.Core.Common.Helpers
{
    /// <summary>
    /// AEAD_AES_256_GCM算法
    /// </summary>
    public class AEADAES256GCM
    {
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="nonce">加密使用的随机串初始化向量</param>
        /// <param name="ciphertext">Base64编码后的密文</param>
        /// <param name="associatedData">附加数据包(可为空)</param>
        /// <param name="key">密钥</param>
        public static string Decrypt(string? nonce, string? ciphertext, string? associatedData, string? key)
        {
            if (string.IsNullOrEmpty(nonce))
            {
                throw new ArgumentNullException(nameof(nonce));
            }

            if (string.IsNullOrEmpty(ciphertext))
            {
                throw new ArgumentNullException(nameof(ciphertext));
            }

            if (string.IsNullOrEmpty(associatedData))
            {
                throw new ArgumentNullException(nameof(associatedData));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            using var aesGcm = new AesGcm(Encoding.UTF8.GetBytes(key), 256);
            var nonceBytes = Encoding.UTF8.GetBytes(nonce);
            var ciphertextWithTagBytes = Convert.FromBase64String(ciphertext); // ciphertext 实际包含了 tag，即尾部16字节
            var ciphertextBytes = ciphertextWithTagBytes[0..^16]; // 排除尾部16字节
            var tagBytes = ciphertextWithTagBytes[^16..]; // 获取尾部16字节
            var plaintextBytes = new byte[ciphertextBytes.Length];
            var associatedDataBytes = Encoding.UTF8.GetBytes(associatedData);
            aesGcm.Decrypt(nonceBytes, ciphertextBytes, tagBytes, plaintextBytes, associatedDataBytes);
            return Encoding.UTF8.GetString(plaintextBytes);
        }
    }
}
