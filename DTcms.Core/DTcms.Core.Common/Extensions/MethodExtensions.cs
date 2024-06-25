using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DTcms.Core.Common.Extensions
{
    /// <summary>
    /// 属性的扩展方法
    /// 作者：一些事情
    /// </summary>
    public static class MethodExtensions
    {
        /// <summary>
        /// 是否非空白字符
        /// </summary>
        public static bool IsNotEmpty(this string? @this)
        {
            return @this != "";
        }

        /// <summary>
        /// 是否非NULL或空白字符
        /// </summary>
        public static bool IsNotNullOrEmpty(this string? @this)
        {
            return !string.IsNullOrEmpty(@this);
        }

        /// <summary>
        /// 是否非NULL或空白字符
        /// </summary>
        public static Boolean IsNotNullOrWhiteSpace(this string? value)
        {
            return !String.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// 是否包含特性
        /// </summary>
        public static bool IsContains(this string? @this, string? value, StringComparison comparisonType)
        {
            if (value == null) return false;
            if (@this == null) return false;
            return @this.IndexOf(value, comparisonType) != -1;
        }

        /// <summary>
        /// 是否相似
        /// </summary>
        public static bool IsLike(this string @this, string pattern)
        {
            string regexPattern = "^" + Regex.Escape(pattern) + "$";
            regexPattern = regexPattern.Replace(@"\[!", "[^")
                .Replace(@"\[", "[")
                .Replace(@"\]", "]")
                .Replace(@"\?", ".")
                .Replace(@"\*", ".*")
                .Replace(@"\#", @"\d");
            return Regex.IsMatch(@this, regexPattern);
        }

        /// <summary>
        /// 是否整数
        /// </summary>
        public static bool IsNumeric(this string @this)
        {
            return !Regex.IsMatch(@this, "[^0-9]");
        }

        /// <summary>
        /// 是否日期
        /// </summary>
        public static bool IsValidDateTime(this object @this)
        {
            if (@this == null)
            {
                return true;
            }
            return DateTime.TryParse(@this.ToString(), out DateTime result);
        }

        /// <summary>
        /// 是否Decimal
        /// </summary>
        public static bool IsValidDecimal(this object @this)
        {
            if (@this == null)
            {
                return true;
            }
            return decimal.TryParse(@this.ToString(), out decimal result);
        }

        /// <summary>
        /// 是否16位整数
        /// </summary>
        public static bool IsValidInt16(this object @this)
        {
            if (@this == null)
            {
                return true;
            }
            return short.TryParse(@this.ToString(), out short result);
        }

        /// <summary>
        /// 是否32位整数
        /// </summary>
        public static bool IsValidInt32(this object @this)
        {
            if (@this == null)
            {
                return true;
            }
            return int.TryParse(@this.ToString(), out int result);
        }

        /// <summary>
        /// 是否64位整数
        /// </summary>
        public static bool IsValidInt64(this object @this)
        {
            if (@this == null)
            {
                return true;
            }
            return long.TryParse(@this.ToString(), out long result);
        }

        /// <summary>
        /// 是否不是数字
        /// </summary>
        public static Boolean IsNaN(this Double d)
        {
            return Double.IsNaN(d);
        }

        /// <summary>
        /// 转换成金额
        /// </summary>
        public static Decimal ToMoney(this Decimal @this)
        {
            return Math.Round(@this, 2);
        }

        /// <summary>
        /// 检查字符串是否在T属性中
        /// 字符串以逗号分隔
        /// </summary>
        public static bool IsPropertyExists<T>(this string? @this)
        {
            if (string.IsNullOrWhiteSpace(@this))
            {
                return true;
            }
            //逗号来分隔字段字符串
            var fieldsAfterSplit = @this.Split(',');
            foreach (var field in fieldsAfterSplit)
            {
                // 获得属性名称字符串
                var propertyName = field.Trim();
                var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                // 如果T中没找到对应的属性
                if (propertyInfo == null)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 字符串转换成枚举
        /// </summary>
        public static T ToEnum<T>(this string @this)
        {
            Type enumType = typeof(T);
            return (T)Enum.Parse(enumType, @this);
        }

        /// <summary>
        /// 字符串转换成IEnumerable
        /// 字符串以逗号分隔
        /// </summary>
        public static IEnumerable<T?>? ToIEnumerable<T>(this string? @this)
        {
            if (!@this.IsNotNullOrWhiteSpace())
            {
                return null;
            }
            Type targetType = typeof(T);
            var converter = TypeDescriptor.GetConverter(targetType);
            try
            {
                var values = @this?.Split(new[] { "," }, 
                    StringSplitOptions.RemoveEmptyEntries).Select(x => (T?)converter.ConvertTo(x.Trim(), targetType)).ToArray();
                return values;
            }
            catch
            {
                return null;
            }
        }

    }
}