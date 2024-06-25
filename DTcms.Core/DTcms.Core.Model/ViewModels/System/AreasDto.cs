using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 省市区(显示)
    /// </summary>
    public class AreasDto : AreasEditDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public int Id { get; set; }


        /// <summary>
        /// 子地区列表
        /// </summary>
        public List<AreasDto> Children { get; set; } = [];
    }

    /// <summary>
    /// 省市区(编辑)
    /// </summary>
    public class AreasEditDto
    {
        /// <summary>
        /// 父级地区ID
        /// </summary>
        [Display(Name = "父级地区")]
        public int ParentId { get; set; } = 0;

        /// <summary>
        /// 地区名称
        /// </summary>
        [Display(Name = "地区名称")]
        [Required(ErrorMessage = "{0}不可为空")]
        [StringLength(128)]
        public string? Title { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [Display(Name = "排序数字")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int SortId { get; set; } = 99;
    }

    /// <summary>
    /// 省市区(导入)
    /// </summary>
    public class AreasImportDto
    {
        /// <summary>
        /// 地区名称
        /// </summary>
        [Display(Name = "地区名称")]
        [Required(ErrorMessage = "{0}不可为空")]
        [StringLength(128)]
        public string? Name { get; set; }


        /// <summary>
        /// 子地区列表
        /// </summary>
        public List<AreasImportDto> Children { get; set; } = new List<AreasImportDto>();
    }

    /// <summary>
    /// 省市区(导入)
    /// </summary>
    public class AreasImportEditDto
    {
        /// <summary>
        /// 地区数据
        /// </summary>
        [Display(Name = "地区数据")]
        [Required(ErrorMessage = "{0}不可为空")]
        public string? JsonData { get; set; }
    }
}
