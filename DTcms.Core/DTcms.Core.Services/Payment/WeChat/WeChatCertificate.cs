using DTcms.Core.Common.Helpers;
using DTcms.Core.Model.WeChat;
using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DTcms.Core.Services.WeChat
{
    /// <summary>
    /// 微信平台证书获取(回调验签)
    /// </summary>
    public class WeChatCertificate
    {
        private readonly ConcurrentDictionary<string, WeChatPayCertificateDto> _certs = new();

        /// <summary>
        /// 获取微信支付平台证书信息(指定证书序列号)。
        /// </summary>
        /// <param name="wxAccount">账户信息</param>
        /// <param name="serial">证书序列号</param>
        /// <returns>微信支付平台证书信息</returns>
        public async Task<WeChatPayCertificateDto> GetCertificateAsync(WeChatPayAccountDto wxAccount, string serial)
        {
            // 如果证书序列号已缓存，则直接使用缓存的证书
            if (_certs.TryGetValue(serial, out var platformCert))
            {
                return platformCert;
            }
            if (wxAccount.CertPath == null || wxAccount.MchId == null)
            {
                throw new ResponseException($"商户号和证书路径不可为空");
            }

            // 否则重新下载新的微信支付平台证书并更新缓存
            var url = WeChatPayConfig.CertificateUrl;
            var auth = BuildAuth(url, "GET", null, wxAccount.MchId, wxAccount.CertPath, wxAccount.MchId);
            ICollection<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization", $"WECHATPAY2-SHA256-RSA2048 {auth}"),
                new KeyValuePair<string, string>("User-Agent", "Unknown"),
                new KeyValuePair<string, string>("Accept", "application/json")
            };
            var (statusCode, reHeaders, reBody) = await RequestHelper.GetAsync(url, headers);
            //throw new ResponseException($"Download certificates failed!Code：{statusCode},Head：{reHeaders},Body：{reBody},Auth：{auth}");
            if (statusCode == 200)
            {
                var certList = reBody.ToObject<CertificateList>();
                if (certList == null)
                {
                    throw new ResponseException($"微信平台上没有找到对应的证书");
                }
                foreach (var certificate in certList.Data)
                {
                    if (certificate.SerialNo == null || certificate.EffectiveTime == null || certificate.ExpireTime == null)
                    {
                        continue;
                    }
                    // 若证书序列号未被缓存，解密证书并加入缓存
                    if (!_certs.ContainsKey(certificate.SerialNo))
                    {
                        switch (certificate?.EncryptCertificate?.Algorithm)
                        {
                            case nameof(AEADAES256GCM):
                                {
                                    var certStr = AEADAES256GCM.Decrypt(certificate.EncryptCertificate.Nonce,
                                        certificate.EncryptCertificate.Ciphertext, certificate.EncryptCertificate.AssociatedData, wxAccount.Apiv3Key);

                                    var cert = new WeChatPayCertificateDto
                                    {
                                        SerialNo = certificate.SerialNo,
                                        EffectiveTime = DateTime.Parse(certificate.EffectiveTime),
                                        ExpireTime = DateTime.Parse(certificate.ExpireTime),
                                        Certificate = new X509Certificate2(Encoding.ASCII.GetBytes(certStr))
                                    };

                                    _certs.TryAdd(certificate.SerialNo, cert);
                                }
                                break;
                            default:
                                throw new ResponseException($"Unknown algorithm: {certificate?.EncryptCertificate?.Algorithm}");
                        }
                    }
                }
            }
            else
            {
                throw new ResponseException($"Download certificates failed!Code：{statusCode},Body：{reBody},Auth：{auth}");
            }

            // 重新从缓存获取证书
            if (_certs.TryGetValue(serial, out platformCert))
            {
                return platformCert;
            }
            else
            {
                throw new ResponseException("Certificate not found!");
            }
        }

        /// <summary>
        /// 生成头部Authorization
        /// </summary>
        private string BuildAuth(string url, string method, string? body, string? mchId, string certPath, string certPwd)
        {
            var uri = new Uri(url).PathAndQuery;
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var nonce = Guid.NewGuid().ToString("N");
            var message = $"{method}\n{uri}\n{timestamp}\n{nonce}\n{body}\n";
            var certificate2 = new X509Certificate2(certPath, certPwd, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            var signature = SHA256WithRSA.Sign(certificate2.GetRSAPrivateKey(), message);
            return $"mchid=\"{mchId}\",nonce_str=\"{nonce}\",timestamp=\"{timestamp}\",serial_no=\"{certificate2.GetSerialNumberString()}\",signature=\"{signature}\"";
        }
    }
}
