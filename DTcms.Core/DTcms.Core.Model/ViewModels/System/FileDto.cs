using System;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 文件上传返回的实体
    /// </summary>
    public class FileDto
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string? FileName { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string? FilePath { get; set; }
        /// <summary>
        /// 缩略图路径
        /// </summary>
        public List<string>? ThumbPath { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string? FileExt { get; set; }
    }
}
