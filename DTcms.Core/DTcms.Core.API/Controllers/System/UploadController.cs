using DTcms.Core.Common.Helpers;
using DTcms.Core.IServices;
using DTcms.Core.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DTcms.Core.API.Controllers
{
    /// <summary>
    /// 文件上传
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class UploadController(IWebHostEnvironment hostEnvironment, IFileService fileService) : ControllerBase
    {
        private readonly IWebHostEnvironment _hostEnvironment = hostEnvironment;
        private readonly IFileService _fileService = fileService;

        /// <summary>
        /// 文件上传
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpLoadFile([FromForm] IFormCollection formCollection, [FromQuery] UploadParameter param)
        {
            //检查是否有文件上传
            if (formCollection.Files.Count == 0)
            {
                throw new ResponseException("请选择要上传文件");
            }
            List<FileDto> listFileDto = [];
            //创建MIME类型映射的提供程序
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            //循环遍历要上传的文件
            foreach (IFormFile file in formCollection.Files)
            {
                string? fileExt = null;
                if (file.FileName.IndexOf('.') != -1)
                {
                    fileExt = Path.GetExtension(file.FileName);
                }
                else
                {
                    var dic = provider.Mappings.FirstOrDefault(x => x.Value.ToLower() == file.ContentType.ToLower());
                    fileExt = dic.Key;
                }

                listFileDto.Add(await _fileService.SaveAsync(file, fileExt, param.Thumb > 0, param.Water > 0, param.TWidth, param.THeight));
            }
            //返回文件上传地址
            return Ok(listFileDto);
        }

        /// <summary>
        /// 加载远程图片返回Base64
        /// </summary>
        [HttpGet("remote")]
        [Authorize]
        public async Task<IActionResult> RemoteFile([FromQuery] string uri)
        {
            if (string.IsNullOrWhiteSpace(uri) || !uri.StartsWith("http"))
            {
                throw new ResponseException("请填写正确的网址");
            }
            using var response = await new HttpClient().GetAsync(uri);
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
            string result = Convert.ToBase64String(byteData);

            return Ok($"data:{response.Content.Headers.ContentType?.MediaType};base64,{result}");
        }
    }
}
