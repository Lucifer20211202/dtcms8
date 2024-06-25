using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace DTcms.Core.Common.Helpers
{
    /// <summary>
    /// 有关二维码的帮助类
    /// </summary>
    public class QRCodeHelper
    {
        /// <summary>
        /// 根据字符串生成二维码
        /// </summary>
        public static Bitmap GenerateQRCode(string str)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(str, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new(qrCodeData);

            return qrCode.GetGraphic(20);
        }

        /// <summary>
        /// 根据字符串生成二维码
        /// </summary>
        public static Bitmap GenerateQRCode(string str, int pixel)
        {
            QRCodeGenerator qrGenerator = new();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(str, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new(qrCodeData);

            return qrCode.GetGraphic(pixel);
        }

        /// <summary>
        /// 将图片转成二进制流
        /// </summary>
        public static byte[] BitmapConvertToByte(Bitmap image, ImageFormat format)
        {
            MemoryStream ms = new();
            image.Save(ms, format);
            var bytes = ms.GetBuffer();
            ms.Close();
            return bytes;
        }

        /// <summary>
        /// 将图片转换成Base64编码的字符串
        /// </summary>
        public static string BitmapConvertToString(Bitmap image)
        {
            MemoryStream ms = new();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] arr = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(arr, 0, (int)ms.Length);
            ms.Close();
            return Convert.ToBase64String(arr);
        }

        /// <summary>
        /// 根据字符串生成Base64二维码字符串
        /// </summary>
        public static string GenerateToString(string str)
        {
            var image = GenerateQRCode(str);
            return BitmapConvertToString(image);
        }
    }
}
