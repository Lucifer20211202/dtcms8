using System;

namespace DTcms.Core.Common.Helpers
{
    /// <summary>
    /// HTML帮助类
    /// </summary>
    public static class HtmlHelper
    {
        /// <summary>
        /// 去除富文本中的HTML标签
        /// </summary>
        /// <param name="html">HTML字符串</param>
        /// <param name="length">截取长度</param>
        public static string ReplaceHtmlTag(string html, int length = 0)
        {
            string strText = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", "");
            strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");

            if (length > 0 && strText.Length > length)
                return strText.Substring(0, length);

            return strText;
        }

        /// <summary>
        /// 截取字符长度
        /// </summary>
        /// <param name="inputString">字符串</param>
        /// <param name="len">截取长度</param>
        public static string CutString(string? inputString, int length)
        {
            if (inputString == null) return string.Empty;
            inputString = ReplaceHtmlTag(inputString);
            string newString;
            if (inputString.Length <= length)
            {
                newString = inputString;
            }
            else
            {
                if (inputString.Length > 3)
                {
                    newString = inputString.Substring(0, length - 3) + "...";
                }
                else
                {
                    newString = inputString.Substring(0, length);
                }

            }
            return newString;
        }
    }
}
