using System;
using System.Collections.Generic;
using System.Text;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 上传设置(公共)
    /// </summary>
    public class UploadConfigDto
    {
        /// <summary>
        /// 附件上传目录
        /// </summary>
        public string FilePath { get; set; } = "upload";

        /// <summary>
        /// 附件保存方式
        /// </summary>
        public int FileSave { get; set; } = 2;

        /// <summary>
        /// 编辑器远程图片上传
        /// </summary>
        public int FileRemote { get; set; } = 0;

        /// <summary>
        /// 附件上传类型
        /// </summary>
        public string FileExtension { get; set; } = "gif,jpg,jpeg,png,bmp,webp,rar,zip,doc,xls,txt";

        /// <summary>
        /// 视频上传类型
        /// </summary>
        public string VideoExtension { get; set; } = "flv,mp3,mp4,avi";

        /// <summary>
        /// 文件上传大小
        /// </summary>
        public int AttachSize { get; set; } = 51200;

        /// <summary>
        /// 视频上传大小
        /// </summary>
        public int VideoSize { get; set; } = 102400;

        /// <summary>
        /// 图片上传大小
        /// </summary>
        public int ImgSize { get; set; } = 10240;

        /// <summary>
        /// 图片最大高度(像素)
        /// </summary>
        public int ImgMaxHeight { get; set; } = 1600;

        /// <summary>
        /// 图片最大宽度(像素)
        /// </summary>
        public int ImgMaxWidth { get; set; } = 1600;

        /// <summary>
        /// 生成缩略图高度(像素)
        /// </summary>
        public int ThumbnailHeight { get; set; } = 300;

        /// <summary>
        /// 生成缩略图宽度(像素)
        /// </summary>
        public int ThumbnailWidth { get; set; } = 300;

        /// <summary>
        /// 缩略图生成方式
        /// </summary>
        public string ThumbnailMode { get; set; } = "Cut";

        /// <summary>
        /// 图片水印类型
        /// </summary>
        public int WatermarkType { get; set; } = 2;

        /// <summary>
        /// 图片水印位置
        /// </summary>
        public int WatermarkPosition { get; set; } = 9;

        /// <summary>
        /// 图片生成质量
        /// </summary>
        public int WatermarkImgQuality { get; set; } = 80;

        /// <summary>
        /// 图片水印文件
        /// </summary>
        public string WatermarkPic { get; set; } = "watermark.png";

        /// <summary>
        /// 水印透明度
        /// </summary>
        public int WatermarkTransparency { get; set; } = 5;

        /// <summary>
        /// 水印文字
        /// </summary>
        public string WatermarkText { get; set; } = "动力启航";

        /// <summary>
        /// 文字字体
        /// </summary>
        public string WatermarkFont { get; set; } = "Tahoma";

        /// <summary>
        /// 文字大小(像素)
        /// </summary>
        public int WatermarkFontSize { get; set; } = 12;
    }
}
