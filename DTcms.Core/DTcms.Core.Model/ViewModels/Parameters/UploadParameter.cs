using System;
using System.Collections.Generic;
using System.Text;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 文件上传参数
    /// </summary>
    public class UploadParameter
    {
        /// <summary>
        /// 是否生成水印
        /// </summary>
        public byte? Water { get; set; }

        /// <summary>
        /// 是否生成缩略图
        /// </summary>
        public byte? Thumb { get; set; }

        /// <summary>
        /// 缩略图宽度
        /// </summary>
        public int? TWidth { get; set; } = null;

        /// <summary>
        /// 缩略图高度
        /// </summary>
        public int? THeight { get; set; } = null;
    }
}
