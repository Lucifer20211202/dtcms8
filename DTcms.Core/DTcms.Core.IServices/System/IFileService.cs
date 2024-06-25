using DTcms.Core.Model.ViewModels;
using Microsoft.AspNetCore.Http;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 文件上传接口
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// 文件分片上传方法
        /// </summary>
        /// <param name="file">IFormFile</param>
        /// <param name="fileExt">文件扩展名</param>
        /// <param name="isThumb">是否生成缩略图</param>
        /// <param name="isWater">是否水印</param>
        /// <param name="thumbWidth">缩略图宽</param>
        /// <param name="thumbHeight">缩略图高</param>
        Task<FileDto> SaveAsync(IFormFile file, string? fileExt, bool isThumb, bool isWater, int? thumbWidth = null, int? thumbHeight = null);

        /// <summary>
        /// 裁剪图片并保存
        /// </summary>
        Task<FileDto> CropAsync(string fileUri, int cropWidth, int cropHeight, int X, int Y);

        /// <summary>
        /// 保存远程文件到本地
        /// </summary>
        /// <param name="sourceUri">URI地址</param>
        /// <returns>上传后的路径</returns>
        Task<FileDto> RemoteAsync(string sourceUri);
    }
}
