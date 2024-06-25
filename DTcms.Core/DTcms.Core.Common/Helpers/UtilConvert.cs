using System.Security.Cryptography;
using System.Text;

namespace DTcms.Core.Common.Helpers
{
    /// <summary>
    /// 通用工具类
    /// </summary>
    public static class UtilConvert
    {
        /// <summary>
        /// 继承HashAlgorithm
        /// </summary>
        public static string GetHash<T>(Stream stream) where T : HashAlgorithm
        {
            StringBuilder sb = new();

            var create = typeof(T).GetMethod("Create", new Type[] { });
            using (T? crypt = (T?)create?.Invoke(null, null))
            {
                if (crypt != null)
                {
                    byte[] hashBytes = crypt.ComputeHash(stream);
                    foreach (byte bt in hashBytes)
                    {
                        sb.Append(bt.ToString("x2"));
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 根据GUID获取16位字符串
        /// </summary>
        public static string GetGuidToString()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            string tempStr = string.Format("{0:x}", i - DateTime.Now.Ticks);
            if (tempStr.Length != 16)
            {
                tempStr += "0";
            }
            return tempStr;
        }

        /// <summary>
        /// 根据GUID获取19位数字
        /// </summary>
        public static string GetGuidToNumber()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0).ToString();
        }

        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        public static string Number(int Length)
        {
            return Number(Length, false);
        }

        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="Length">生成长度</param>
        /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
        public static string Number(int Length, bool Sleep)
        {
            if (Sleep)
                System.Threading.Thread.Sleep(3);
            string result = "";
            System.Random random = new();
            for (int i = 0; i < Length; i++)
            {
                result += random.Next(10).ToString();
            }
            return result;
        }

        /// <summary>
        /// 生成随机字母字符串(数字字母混和)
        /// </summary>
        /// <param name="codeCount">待生成的位数</param>
        public static string GetCheckCode(int codeCount)
        {
            string str = string.Empty;
            int rep = 0;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str += ch.ToString();
            }
            return str;
        }

        /// <summary>
        /// Object转Int
        /// </summary>
        public static int ObjToInt(this object thisValue)
        {
            int reval = 0;
            if (thisValue == null) return 0;
            if (thisValue != null && thisValue != DBNull.Value && int.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return reval;
        }

        /// <summary>
        /// Object转Int
        /// </summary>
        public static int ObjToInt(this object thisValue, int errorValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && int.TryParse(thisValue.ToString(), out int reval))
            {
                return reval;
            }
            return errorValue;
        }

        /// <summary>
        /// Object转Double
        /// </summary>
        public static double ObjToMoney(this object thisValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), out double reval))
            {
                return reval;
            }
            return 0;
        }

        /// <summary>
        /// Object转Double
        /// </summary>
        public static double ObjToMoney(this object thisValue, double errorValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), out double reval))
            {
                return reval;
            }
            return errorValue;
        }

        /// <summary>
        /// Object转String
        /// </summary>
        public static string ObjToString(this object thisValue)
        {
            return thisValue.ToString()?.Trim() ?? String.Empty;
        }

        /// <summary>
        /// Object转String
        /// </summary>
        public static string ObjToString(this object thisValue, string errorValue)
        {
            return thisValue.ToString()?.Trim()?? errorValue;
        }

        /// <summary>
        /// Object转Decimal
        /// </summary>
        public static Decimal ObjToDecimal(this object thisValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && decimal.TryParse(thisValue.ToString(), out decimal reval))
            {
                return reval;
            }
            return 0;
        }

        /// <summary>
        /// Object转Decimal
        /// </summary>
        public static Decimal ObjToDecimal(this object thisValue, decimal errorValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && decimal.TryParse(thisValue.ToString(), out decimal reval))
            {
                return reval;
            }
            return errorValue;
        }

        /// <summary>
        /// Object转DateTime
        /// </summary>
        public static DateTime ObjToDate(this object thisValue)
        {
            DateTime reval = DateTime.MinValue;
            if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out reval))
            {
                reval = Convert.ToDateTime(thisValue);
            }
            return reval;
        }

        /// <summary>
        /// Object转DateTime
        /// </summary>
        public static DateTime ObjToDate(this object thisValue, DateTime errorValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out DateTime reval))
            {
                return reval;
            }
            return errorValue;
        }

        /// <summary>
        /// Object转Bool
        /// </summary>
        public static bool ObjToBool(this object thisValue)
        {
            bool reval = false;
            if (thisValue != null && thisValue != DBNull.Value && bool.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return reval;
        }
    }
}