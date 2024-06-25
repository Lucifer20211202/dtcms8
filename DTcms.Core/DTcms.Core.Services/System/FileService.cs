using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Extensions;
using DTcms.Core.Common.Helpers;
using DTcms.Core.IServices;
using DTcms.Core.Model.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Net;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 文件上传接口实现
    /// </summary>
    public class FileService(IConfigService configService) : IFileService
    {
        private const int ChunkSize = 4 * 1024 * 1024; // 每个分片的大小
        private const int ImageMaxSize = 28 * 1024 * 1024; // 缩略图和水印最大值
        private readonly SemaphoreSlim _semaphore = new(1, 10); // 初始化信号量为1，表示只允许一个线程进入
        private readonly IConfigService _configService = configService;

        #region 公开的方法
        /// <summary>
        /// 文件分片上传方法
        /// </summary>
        /// <param name="file">IFormFile</param>
        /// <param name="fileExt">文件扩展名</param>
        /// <param name="isThumb">是否生成缩略图</param>
        /// <param name="isWater">是否水印</param>
        /// <param name="thumbWidth">缩略图宽</param>
        /// <param name="thumbHeight">缩略图高</param>
        public async Task<FileDto> SaveAsync(IFormFile file, string? fileExt, bool isThumb, bool isWater, int? thumbWidth = null, int? thumbHeight = null)
        {
            // 检查文件字节数组是否为NULL
            if (file.Length == 0)
            {
                throw new ResponseException("请选择要上传的文件");
            }
            //检查扩展名是否为Null
            if (fileExt == null)
            {
                throw new ResponseException("未知文件上传失败");
            }
            fileExt = fileExt.Trim('.'); //去掉.只保留扩展名
            // 取得站点配置信息
            var config = await GetConfigAsync();
            // 检查文件扩展名是否合法
            if (!CheckFileExt(config, fileExt))
            {
                throw new ResponseException($"不允许上传{fileExt}类型的文件");
            }
            // 检查文件大小是否合法
            if (!CheckFileSize(config, fileExt, file.Length))
            {
                throw new ResponseException($"文件超过限制的大小");
            }

            string newGuidName = Guid.NewGuid().ToString(); // 随机生成的Guid文件名
            string newFileName = $"{newGuidName}.{fileExt}"; // 生成新的文件名
            string newThumbnailFileName = $"{newGuidName}_thumb.{fileExt}"; // 生成缩略图文件名

            string upLoadPath = GetUpLoadPath(config); // 本地上传目录相对路径
            string fullUpLoadPath = FileHelper.GetWebPath(upLoadPath); // 本地上传目录的物理路径
            string newFilePath = $"{upLoadPath}{newFileName}"; // 本地上传后的路径
            string newThumbnailPath = $"{upLoadPath}{newThumbnailFileName}"; // 本地上传后的缩略图路径
            FileDto fileDto = new(); // 返回的文件信息对象
            byte[]? fileData = null; // 要存储的文件流
            fileDto.ThumbPath = [newFilePath]; // 预先赋值原图
            byte[]? thumbData = null; // 缩略图的文件流

            // 如果是图片且在28M以内则需要检查缩略图和水印
            if (file.ContentType.Contains("image") && file.Length <= ImageMaxSize)
            {
                //等待信号量变得可用
                await _semaphore.WaitAsync();

                try
                {
                    byte[] byteData;
                    // 读取文件流
                    using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        byteData = ms.ToArray(); // 转换成二进制
                    }
                    // 超设定最大宽高
                    if (config.ImgMaxHeight > 0 || config.ImgMaxWidth > 0)
                    {
                        fileData = ImageHelper.MakeThumbnail(byteData, fileExt, config.ImgMaxWidth, config.ImgMaxHeight);
                    }
                    // 缩略图
                    if (isThumb && config.ThumbnailWidth > 0 && config.ThumbnailHeight > 0)
                    {
                        thumbData = ImageHelper.MakeThumbnail(byteData, fileExt, thumbWidth ?? config.ThumbnailWidth, thumbHeight ?? config.ThumbnailHeight, config.ThumbnailMode);
                    }
                    // 水印
                    if (isWater && config.WatermarkType > 0)
                    {
                        switch (config.WatermarkType)
                        {
                            case 1:
                                fileData = ImageHelper.LetterWatermark(fileData ?? byteData, fileExt, config.WatermarkText, config.WatermarkPosition,
                                    config.WatermarkImgQuality, config.WatermarkFont, config.WatermarkFontSize);
                                break;
                            case 2:
                                fileData = ImageHelper.ImageWatermark(fileData ?? byteData, fileExt,
                                    FileHelper.GetWebPath(config.WatermarkPic), config.WatermarkPosition,
                                    config.WatermarkImgQuality, config.WatermarkTransparency);
                                break;
                        }
                    }
                }
                finally
                {
                    //释放信号量，允许其他线程进入
                    _semaphore.Release();
                }
            }

            //检查本地上传的物理路径是否存在，不存在则创建
            if (!Directory.Exists(fullUpLoadPath))
            {
                Directory.CreateDirectory(fullUpLoadPath);
            }
            // 保存主文件
            if (fileData != null)
            {
                // 普通上传
                FileHelper.SaveFile(fileData, fullUpLoadPath + newFileName);
            }
            else
            {
                // 分片上传
                using var fileStream = file.OpenReadStream();
                //计算分片数量及缓冲区
                int totalChunks = (int)Math.Ceiling((double)fileStream.Length / ChunkSize);
                byte[] buffer = new byte[ChunkSize];
                string[] chunkPaths = new string[totalChunks];
                //开始读取文件
                for (int i = 0; i < totalChunks; i++)
                {
                    int bytesRead = await fileStream.ReadAsync(buffer.AsMemory(0, ChunkSize));
                    string chunkFilePath = await FileHelper.UploadChunk(buffer, bytesRead, i, fullUpLoadPath, newGuidName);
                    chunkPaths[i] = chunkFilePath;
                }
                string mergedFilePath = Path.Combine(fullUpLoadPath, newFileName);
                await FileHelper.MergeChunks(chunkPaths, mergedFilePath);
            }
            // 赋值文件路径
            fileDto.FilePath = newFilePath;
            // 保存缩略图文件
            if (thumbData != null)
            {
                FileHelper.SaveFile(thumbData, fullUpLoadPath + newThumbnailFileName);
                // 赋值缩略图路径
                fileDto.ThumbPath = new List<string>() { newThumbnailPath };
            }

            //返回成功信息
            fileDto.FileName = file.FileName;
            fileDto.FileSize = file.Length;
            fileDto.FileExt = fileExt;
            return fileDto;
        }

        /// <summary>
        /// 裁剪图片并保存
        /// </summary>
        public async Task<FileDto> CropAsync(string fileUri, int cropWidth, int cropHeight, int X, int Y)
        {
            string fileExt = Path.GetExtension(fileUri).Trim('.'); //文件扩展名，不含“.”
            if (string.IsNullOrEmpty(fileExt) || !IsImage(fileExt))
            {
                throw new ResponseException($"该文件不是图片");
            }

            byte[]? byteData = null;
            //判断是否远程文件
            if (fileUri.ToLower().StartsWith("http://") || fileUri.ToLower().StartsWith("https://"))
            {
                using HttpClient client = new();
                byteData = await client.GetByteArrayAsync(fileUri);
            }
            else //本地源文件
            {
                string fullName = FileHelper.GetWebPath(fileUri);
                byteData = await FileHelper.GetBinaryFileAsync(fullName);
            }
            if (byteData == null)
            {
                throw new ResponseException($"无法获取原图片");
            }
            //裁剪后得到文件流
            byteData = ImageHelper.MakeThumbnail(byteData, fileExt, cropWidth, cropHeight, X, Y);

            //保存文件
            var result = await SaveImageAsync(byteData, fileUri);
            // 删除原图
            await DeleteAsync(fileUri);

            return result;
        }

        /// <summary>
        /// 保存远程图片到本地
        /// </summary>
        /// <param name="sourceUri">URI地址</param>
        /// <returns>上传后的文件信息</returns>
        public async Task<FileDto> RemoteAsync(string sourceUri)
        {
            if (!IsExternalIPAddress(sourceUri))
            {
                throw new ResponseException($"INVALID_URL");
            }
            using var response = await new HttpClient().GetAsync(sourceUri);
            if (response == null)
            {
                throw new ResponseException($"抓取文件错误");
            }
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ResponseException($"错误码：{response?.StatusCode}, {response?.RequestMessage}");
            }
            if (response.Content.Headers.ContentType?.MediaType?.IndexOf("image") == -1)
            {
                throw new ResponseException($"抓取的不是图片文件");
            }
            byte[] byteData = await response.Content.ReadAsByteArrayAsync();

            return await SaveImageAsync(byteData, sourceUri);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        public async Task<bool> DeleteAsync(string fileUri)
        {
            //取得站点配置信息
            var sysConfig = await _configService.QueryByTypeAsync(ConfigType.SysConfig);
            if (sysConfig == null)
            {
                throw new ResponseException("站点配置信息不存在");
            }
            var config = JsonHelper.ToJson<SysConfigDto>(sysConfig.JsonData);
            if (config == null)
            {
                throw new ResponseException("站点配置信息格式有误");
            }
            bool result = FileHelper.DeleteFile(FileHelper.GetWebPath(fileUri));
            return result;
        }
        #endregion

        #region 辅助的私有方法
        /// <summary>
        /// 保存文件流的图片
        /// </summary>
        private async Task<FileDto> SaveImageAsync(byte[] byteData, string fileName)
        {
            string fileExt = Path.GetExtension(fileName).Trim('.'); //文件扩展名，不含“.”
            var config = await GetConfigAsync(); // 取得站点配置信息
            var fileDto = new FileDto(); // 返回信息

            if (string.IsNullOrEmpty(fileExt) || !IsImage(fileExt))
            {
                throw new ResponseException($"该文件不是图片");
            }
            //检查文件扩展名是否合法
            if (!CheckFileExt(config, fileExt))
            {
                throw new ResponseException($"不允许上传{fileExt}类型的文件");
            }
            //检查文件大小是否合法
            if (!CheckFileSize(config, fileExt, byteData.Length))
            {
                throw new ResponseException($"文件超过限制的大小");
            }
            //如果超出最大尺寸，是则裁剪
            if (config.ImgMaxHeight > 0 || config.ImgMaxWidth > 0)
            {
                var thumbData = ImageHelper.MakeThumbnail(byteData, fileExt, config.ImgMaxWidth, config.ImgMaxHeight);
                if (thumbData != null)
                {
                    byteData = thumbData;
                }
            }
            string newFileName = Guid.NewGuid() + "." + fileExt; // 随机生成新的文件名
            string upLoadPath = GetUpLoadPath(config); // 本地上传目录相对路径
            string fullUpLoadPath = FileHelper.GetWebPath(upLoadPath); // 本地上传目录的物理路径
            string newFilePath = upLoadPath + newFileName; // 本地上传后的路径

            // 检查本地上传的物理路径是否存在，不存在则创建
            if (!Directory.Exists(fullUpLoadPath))
            {
                Directory.CreateDirectory(fullUpLoadPath);
            }
            // 保存主文件
            FileHelper.SaveFile(byteData, fullUpLoadPath + newFileName);
            // 赋值文件路径
            fileDto.FilePath = newFilePath;

            //返回成功信息
            fileDto.FileName = fileName;
            fileDto.FileSize = byteData.Length;
            fileDto.FileExt = fileExt;
            return fileDto;
        }

        /// <summary>
        /// 获取系统配置信息
        /// </summary>
        private async Task<SysConfigDto> GetConfigAsync()
        {
            // 取得站点配置信息
            var sysConfig = await _configService.QueryByTypeAsync(ConfigType.SysConfig);
            if (sysConfig == null)
            {
                throw new ResponseException("系统配置信息不存在");
            }
            var config = JsonHelper.ToJson<SysConfigDto>(sysConfig.JsonData);
            if (config == null)
            {
                throw new ResponseException("系统配置信息格式有误");
            }
            return config;
        }

        /// <summary>
        /// 获取上传目录相对路径
        /// </summary>
        private string GetUpLoadPath(SysConfigDto config)
        {
            string path = $"/{config.FilePath}/";
            switch (config.FileSave)
            {
                case 1: //按年月日每天一个文件夹
                    path += DateTime.Now.ToString("yyyyMMdd");
                    break;
                default: //按年月/日存入不同的文件夹
                    path += DateTime.Now.ToString("yyyyMM") + "/" + DateTime.Now.ToString("dd");
                    break;
            }
            return path + "/";
        }

        /// <summary>
        /// 是否为图片文件
        /// </summary>
        /// <param name="fileExt">文件扩展名，不含“.”</param>
        private bool IsImage(string? fileExt)
        {
            if (string.IsNullOrWhiteSpace(fileExt))
            {
                return false;
            }
            ArrayList al = new ArrayList();
            al.Add("bmp");
            al.Add("jpeg");
            al.Add("jpg");
            al.Add("gif");
            al.Add("png");
            al.Add("webp");
            if (al.Contains(fileExt.ToLower()))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查是否为合法的上传文件
        /// </summary>
        private bool CheckFileExt(SysConfigDto config, string fileExt)
        {
            //检查危险文件
            string[] excExt = { "asp", "aspx", "ashx", "asa", "asmx", "asax", "php", "jsp", "htm", "html" };
            for (int i = 0; i < excExt.Length; i++)
            {
                if (excExt[i].ToLower() == fileExt.ToLower())
                {
                    return false;
                }
            }
            //检查合法文件
            string[] allowExt = (config.FileExtension + "," + config.VideoExtension).Split(',');
            for (int i = 0; i < allowExt.Length; i++)
            {
                if (allowExt[i].ToLower() == fileExt.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查文件大小是否合法
        /// </summary>
        /// <param name="fileExt">文件扩展名，不含“.”</param>
        /// <param name="fileSize">文件大小(B)</param>
        private bool CheckFileSize(SysConfigDto config, string fileExt, long fileSize)
        {
            if (config.VideoExtension == null) return false;
            //将视频扩展名转换成ArrayList
            ArrayList lsVideoExt = new(config.VideoExtension.ToLower().Split(','));
            //判断是否为图片文件
            if (IsImage(fileExt))
            {
                if (config.ImgSize > 0 && fileSize > config.ImgSize * 1024)
                {
                    return false;
                }
            }
            else if (lsVideoExt.Contains(fileExt.ToLower()))
            {
                if (config.VideoSize > 0 && fileSize > config.VideoSize * 1024)
                {
                    return false;
                }
            }
            else
            {
                if (config.AttachSize > 0 && fileSize > config.AttachSize * 1024)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 检查文件地址是否文件服务器地址
        /// </summary>
        /// <param name="url">文件地址</param>
        private bool IsExternalIPAddress(string url)
        {
            var uri = new Uri(url);
            switch (uri.HostNameType)
            {
                case UriHostNameType.Dns:
                    var ipHostEntry = Dns.GetHostEntry(uri.DnsSafeHost);
                    foreach (IPAddress ipAddress in ipHostEntry.AddressList)
                    {
                        byte[] ipBytes = ipAddress.GetAddressBytes();
                        if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            if (!IsPrivateIP(ipAddress))
                            {
                                return true;
                            }
                        }
                    }
                    break;

                case UriHostNameType.IPv4:
                    return !IsPrivateIP(IPAddress.Parse(uri.DnsSafeHost));
            }
            return false;
        }

        /// <summary>
        /// 检查IP地址是否本地服务器地址
        /// </summary>
        /// <param name="myIPAddress">IP地址</param>
        private bool IsPrivateIP(IPAddress myIPAddress)
        {
            if (IPAddress.IsLoopback(myIPAddress)) return true;
            if (myIPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                byte[] ipBytes = myIPAddress.GetAddressBytes();
                // 10.0.0.0/24 
                if (ipBytes[0] == 10)
                {
                    return true;
                }
                // 172.16.0.0/16
                else if (ipBytes[0] == 172 && ipBytes[1] == 16)
                {
                    return true;
                }
                // 192.168.0.0/16
                else if (ipBytes[0] == 192 && ipBytes[1] == 168)
                {
                    return true;
                }
                // 169.254.0.0/16
                else if (ipBytes[0] == 169 && ipBytes[1] == 254)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
