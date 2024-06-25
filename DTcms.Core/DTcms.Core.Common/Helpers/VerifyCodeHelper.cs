using SkiaSharp;

namespace DTcms.Core.Common.Helpers
{
    /// <summary>
    /// 验证码帮助类
    /// </summary>
    public class VerifyCodeHelper
    {
        private static readonly string Letters = "1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,J,K,L,M,N,P,Q,R,S,T,U,V,W,X,Y,Z";

        /// <summary>
        /// 生成验证码图片
        /// </summary>
        public static MemoryStream Create(string captchaCode, int width = 0, int height = 30)
        {
            SKColor[] colors = { SKColors.Black, SKColors.Red, SKColors.DarkBlue, SKColors.Green, SKColors.Orange, SKColors.Brown, SKColors.DarkCyan, SKColors.Purple };
            string[] fonts = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial" };

            if (width == 0) width = captchaCode.Length * 20;

            using var surface = SKSurface.Create(new SKImageInfo(width, height));
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);

            var random = new Random();
            for (var i = 0; i < width * height * 0.1; i++)
            {
                var x = random.Next(width);
                var y = random.Next(height);
                var color = new SKColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                using var pointPaint = new SKPaint { Color = color };
                canvas.DrawPoint(x, y, pointPaint);
            }

            // 添加干扰线
            for (var i = 0; i < 2; i++)
            {
                var startX = random.Next(width);
                var startY = random.Next(height);
                var endX = random.Next(width);
                var endY = random.Next(height);
                var lineColor = new SKColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                using var linePaint = new SKPaint { Color = lineColor, StrokeWidth = 1.5f };
                canvas.DrawLine(startX, startY, endX, endY, linePaint);
            }

            for (var i = 0; i < captchaCode.Length; i++)
            {
                var cIndex = random.Next(colors.Length);
                var fIndex = random.Next(fonts.Length);
                var font = SKTypeface.FromFamilyName(fonts[fIndex]);
                var fontSize = height / 2;
                using var textPaint = new SKPaint
                {
                    Color = colors[cIndex],
                    Typeface = font,
                    TextSize = fontSize,
                    IsAntialias = true,
                    TextAlign = SKTextAlign.Left,
                    FakeBoldText = true
                };
                var bounds = new SKRect();
                textPaint.MeasureText(captchaCode[i].ToString(), ref bounds);
                var x = width / (captchaCode.Length + 2);
                var y = (height + bounds.Height) / 2;
                canvas.DrawText(captchaCode[i].ToString(), x + (x * i), y, textPaint);
            }

            using var image = surface.Snapshot();
            using var ms = new MemoryStream();
            image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(ms);
            return ms;
        }

        /// <summary>
        /// 生成验证码随机数
        /// </summary>
        public static string RandomCode(int codeLength = 4)
        {
            var array = Letters.Split(new[] { ',' });
            var random = new Random();
            var temp = -1;
            var captcheCode = string.Empty;
            for (int i = 0; i < codeLength; i++)
            {
                if (temp != -1)
                    random = new Random(i * temp * unchecked((int)DateTime.Now.Ticks));
                var index = random.Next(array.Length);
                if (temp != -1 && temp == index)
                    return RandomCode(codeLength);
                temp = index;
                captcheCode += array[index];
            }
            return captcheCode;
        }
    }
}
