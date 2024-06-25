using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Common.Extensions
{
    /// <summary>
    /// 枚举扩展类
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取枚举的Display名称
        /// </summary>
        public static string? DisplayName(this Enum value)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                var t = value.GetType().GetFields().SingleOrDefault(w => w.Name == value.ToString())
                    ?.CustomAttributes.SingleOrDefault(w => w.AttributeType == typeof(DisplayAttribute));
                if (t != null)
                {
                    return t?.NamedArguments[0].TypedValue.Value?.ToString();
                }
                else
                {
                    return value?.ToString();
                }
            }
        }
    }
}
