using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class SysConfig
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 配置类型
        /// </summary>
        [Display(Name = "配置类型")]
        [StringLength(30)]
        public string? Type { get; set; }

        /// <summary>
        /// Json格式数据
        /// </summary>
        [Display(Name = "Json格式数据")]
        public string? JsonData { get; set; }
    }
}
