using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 站点支付方式(显示)
    /// </summary>
    public class SitePaymentsDto: SitePaymentsEditDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "创建人")]
        public string? AddBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; }


        /// <summary>
        /// 站点信息
        /// </summary>
        public SitesDto? Site { get; set; }

        /// <summary>
        /// 支付方式信息
        /// </summary>
        public PaymentsDto? Payment { get; set; }
    }

    /// <summary>
    /// 站点支付方式(编辑)
    /// </summary>
    public class SitePaymentsEditDto
    {
        /// <summary>
        /// 所属站点ID
        /// </summary>
        [Display(Name = "所属站点")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int SiteId { get; set; }

        /// <summary>
        /// 支付方式ID
        /// </summary>
        [Display(Name = "支付方式")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int PaymentId { get; set; }

        /// <summary>
        /// 接口类型
        /// 线下付款：cash
        /// 余额支付：balance
        /// 支付宝：pc(电脑)|h5(手机)|app
        /// 微信：native(扫码)|h5(手机)|mp(小程序)|jsapi(公众号)|app
        /// </summary>
        [Display(Name = "接口类型")]
        [Required(ErrorMessage = "{0}不可为空")]
        [StringLength(128)]
        public string? Type { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题")]
        [Required(ErrorMessage = "{0}不可为空")]
        [StringLength(128)]
        public string? Title { get; set; }

        /// <summary>
        /// 通讯密钥1
        /// </summary>
        [Display(Name = "通讯密钥1")]
        public string? Key1 { get; set; }

        /// <summary>
        /// 通讯密钥2
        /// </summary>
        [Display(Name = "通讯密钥2")]
        public string? Key2 { get; set; }

        /// <summary>
        /// 通讯密钥3
        /// </summary>
        [Display(Name = "通讯密钥3")]
        public string? Key3 { get; set; }

        /// <summary>
        /// 通讯密钥4
        /// </summary>
        [Display(Name = "通讯密钥4")]
        public string? Key4 { get; set; }

        /// <summary>
        /// 通讯密钥5
        /// </summary>
        [Display(Name = "通讯密钥5")]
        public string? Key5 { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [Display(Name = "排序数字")]
        public int SortId { get; set; } = 99;
    }

    /// <summary>
    /// 站点支付方式(前端)
    /// </summary>
    public class SitePaymentsClientDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 所属站点ID
        /// </summary>
        [Display(Name = "所属站点")]
        public int SiteId { get; set; }

        /// <summary>
        /// 支付方式ID
        /// </summary>
        [Display(Name = "支付方式")]
        public int PaymentId { get; set; }

        /// <summary>
        /// 接口类型(0线下1余额2微信3支付宝)
        /// </summary>
        [Display(Name = "收款类型")]
        public byte PaymentType { get; set; }

        /// <summary>
        /// 接口类型
        /// 线下付款：cash
        /// 余额支付：balance
        /// 支付宝：pc(电脑)|h5(手机)|app
        /// 微信：native(扫码)|h5(手机)|mp(小程序)|jsapi(公众号)|app
        /// </summary>
        [Display(Name = "接口类型")]
        public string? Type { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题")]
        public string? Title { get; set; }

        /// <summary>
        /// 显示图片
        /// </summary>
        [Display(Name = "显示图片")]
        public string? ImgUrl { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        public string? Remark { get; set; }

        /// <summary>
        /// 支付接口页面
        /// </summary>
        [Display(Name = "支付接口页面")]
        public string? PayUrl { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [Display(Name = "排序数字")]
        public int SortId { get; set; } = 99;

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; }
    }
}
