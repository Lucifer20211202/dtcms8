using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 系统设置
    /// </summary>
    public class SysConfigDto
    {
        #region 基本设置======================================
        /// <summary>
        /// 主站名称
        /// </summary>
        [Display(Name = "网站名称")]
        [Required(ErrorMessage = "{0}不可为空")]
        public string? WebName { get; set; }

        /// <summary>
        /// 主站LOGO
        /// </summary>
        [Display(Name = "主站LOGO")]
        public string? WebLogo { get; set; }

        /// <summary>
        /// 主站域名
        /// </summary>
        [Display(Name = "主站域名")]
        [Required(ErrorMessage = "{0}不可为空")]
        public string WebUrl { get; set; } = "http://demo.dtcms.net";

        /// <summary>
        /// 系统版本号
        /// </summary>
        [Display(Name = "系统版本号")]
        [Required(ErrorMessage = "{0}不可为空")]
        public string WebVersion { get; set; } = "VCore 8.0";

        /// <summary>
        /// 公司名称
        /// </summary>
        public string? WebCompany { get; set; }

        /// <summary>
        /// 通讯地址
        /// </summary>
        public string? WebAddress { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string? WebTel { get; set; }

        /// <summary>
        /// 传真号码
        /// </summary>
        public string? WebFax { get; set; }

        /// <summary>
        /// 管理员邮箱
        /// </summary>
        public string? WebMail { get; set; }

        /// <summary>
        /// 网站备案号
        /// </summary>
        public string? WebCrod { get; set; }

        /// <summary>
        /// 是否关闭网站
        /// </summary>
        public byte WebStatus { get; set; } = 0;

        /// <summary>
        /// 关闭原因描述
        /// </summary>
        public string? WebCloseReason { get; set; }
        #endregion

        #region 短信平台设置==================================
        /// <summary>
        /// 短信服务商
        /// 1.阿里云 2.腾讯云
        /// </summary>
        public byte SmsProvider { get; set; } = 1;

        /// <summary>
        /// 短信AccessKey ID
        /// </summary>
        public string? SmsSecretId { get; set; }

        /// <summary>
        /// 短信AccessKey Secret
        /// </summary>
        public string? SmsSecretKey { get; set; }

        /// <summary>
        /// 短信SDKAPPID
        /// </summary>
        public string? SmsAppId { get; set; }

        /// <summary>
        /// 短信签名
        /// </summary>
        public string? SmsSignTxt { get; set; }
        #endregion

        #region 邮件发送设置==================================
        /// <summary>
        /// STMP服务器
        /// </summary>
        public string? EmailSmtp { get; set; }

        /// <summary>
        /// 是否启用SSL加密连接
        /// </summary>
        public byte EmailSSL { get; set; }

        /// <summary>
        /// SMTP端口
        /// </summary>
        public int? EmailPort { get; set; }

        /// <summary>
        /// 发件人地址
        /// </summary>
        public string? EmailFrom { get; set; }

        /// <summary>
        /// 邮箱账号
        /// </summary>
        public string? EmailUserName { get; set; }

        /// <summary>
        /// 邮箱密码
        /// </summary>
        public string? EmailPassword { get; set; }

        /// <summary>
        /// 发件人昵称
        /// </summary>
        public string? EmailNickname { get; set; }
        #endregion

        #region 文件上传设置==================================
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
        #endregion

        #region 存储设置======================================
        /// <summary>
        /// 文件服务器(本地localhost)
        /// </summary>
        public string FileServer { get; set; } = "localhost";
        #endregion
    }

    /// <summary>
    /// 系统设置(公共)
    /// </summary>
    public class SysConfigClientDto
    {
        /// <summary>
        /// 网站名称
        /// </summary>
        [Display(Name = "网站名称")]
        [Required(ErrorMessage = "{0}不可为空")]
        public string? WebName { get; set; }

        /// <summary>
        /// 主站LOGO
        /// </summary>
        [Display(Name = "主站LOGO")]
        public string? WebLogo { get; set; }

        /// <summary>
        /// 主站域名
        /// </summary>
        [Display(Name = "主站域名")]
        [Required(ErrorMessage = "{0}不可为空")]
        public string WebUrl { get; set; } = "http://demo.dtcms.net";

        /// <summary>
        /// 系统版本号
        /// </summary>
        [Display(Name = "系统版本号")]
        [Required(ErrorMessage = "{0}不可为空")]
        public string WebVersion { get; set; } = "VCore 8.0";

        /// <summary>
        /// 公司名称
        /// </summary>
        public string? WebCompany { get; set; }

        /// <summary>
        /// 通讯地址
        /// </summary>
        public string? WebAddress { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string? WebTel { get; set; }

        /// <summary>
        /// 传真号码
        /// </summary>
        public string? WebFax { get; set; }

        /// <summary>
        /// 管理员邮箱
        /// </summary>
        public string? WebMail { get; set; }

        /// <summary>
        /// 网站备案号
        /// </summary>
        public string? WebCrod { get; set; }

        /// <summary>
        /// 是否关闭网站
        /// </summary>
        public byte WebStatus { get; set; } = 0;

        /// <summary>
        /// 关闭原因描述
        /// </summary>
        public string? WebCloseReason { get; set; }
    }
}
