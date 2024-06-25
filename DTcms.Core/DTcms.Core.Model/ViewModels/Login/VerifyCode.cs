using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 验证码
    /// </summary>
    public class VerifyCode
    {
        /// <summary>
        /// 验证码密钥
        /// </summary>
        [Display(Name = "验证密钥")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string? CodeKey { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [Display(Name = "验证码")]
        [Required(ErrorMessage = "{0}不能为空")]
        [MinLength(4, ErrorMessage = "{0}至少{1}位字符")]
        public string? CodeValue { get; set; }
    }
}
