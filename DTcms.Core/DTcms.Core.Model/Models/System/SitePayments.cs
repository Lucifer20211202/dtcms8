using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 站点支付方式
    /// </summary>
    public class SitePayments
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        /// 接口类型
        /// 线下付款：cash
        /// 余额支付：balance
        /// 支付宝：pc(电脑)|h5(手机)|app
        /// 微信：native(扫码)|h5(手机)|mp(小程序)|jsapi(公众号)|app
        /// </summary>
        [Display(Name = "接口类型")]
        [StringLength(30)]
        public string? Type { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题")]
        [StringLength(128)]
        public string? Title { get; set; }

        /// <summary>
        /// 通讯密钥1
        /// </summary>
        [Display(Name = "通讯密钥1")]
        [StringLength(1024)]
        public string? Key1 { get; set; }

        /// <summary>
        /// 通讯密钥2
        /// </summary>
        [Display(Name = "通讯密钥2")]
        [StringLength(1024)]
        public string? Key2 { get; set; }

        /// <summary>
        /// 通讯密钥3
        /// </summary>
        [Display(Name = "通讯密钥3")]
        [StringLength(1024)]
        public string? Key3 { get; set; }

        /// <summary>
        /// 通讯密钥3
        /// </summary>
        [Display(Name = "通讯密钥4")]
        [StringLength(1024)]
        public string? Key4 { get; set; }

        /// <summary>
        /// 通讯密钥5
        /// </summary>
        [Display(Name = "通讯密钥5")]
        [StringLength(1024)]
        public string? Key5 { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [Display(Name = "排序数字")]
        public int SortId { get; set; } = 99;

        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "创建人")]
        [StringLength(30)]
        public string? AddBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; } = DateTime.Now;


        /// <summary>
        /// 站点信息
        /// </summary>
        [ForeignKey("SiteId")]
        public Sites? Site { get; set; }

        /// <summary>
        /// 支付方式信息
        /// </summary>
        [ForeignKey("PaymentId")]
        public Payments? Payment { get; set; }
    }
}
