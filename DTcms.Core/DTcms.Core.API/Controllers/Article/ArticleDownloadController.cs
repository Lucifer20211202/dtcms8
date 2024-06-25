using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Helpers;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using Microsoft.AspNetCore.Mvc;

namespace DTcms.Core.API.Controllers
{
    /// <summary>
    /// 用户下载数据
    /// </summary>
    [Route("admin/article/download")]
    [ApiController]
    public class ArticleDownloadController(IUserService userService,
        IArticleAttachService attachService, IHttpClientFactory clientFactory, IWebHostEnvironment hostEnvironment) : ControllerBase
    {
        private readonly IWebHostEnvironment _hostEnvironment = hostEnvironment;
        private readonly IArticleAttachService _attachService = attachService;
        private readonly IUserService _userService = userService;
        private readonly IHttpClientFactory _clientFactory = clientFactory;

        #region 前台调用接口============================
        /// <summary>
        /// 用户下载附件
        /// 示例：/client/article/download/1
        /// </summary>
        [HttpGet("/client/article/download/{id}")]
        public async Task<IActionResult> Download([FromRoute] long id)
        {
            //获得下载ID
            if (id == 0)
            {
                throw new ResponseException("出错了，文件参数传值不正确");
            }
            //获取登录用户
            if (_userService.GetUserId() == 0)
            {
                throw new ResponseException("下载失败，请登录后下载", ErrorCode.AuthFailed);
            }

            var model = await _attachService.QueryAsync<ArticleAttachs>(t => t.Id == id);
            if (model == null || model.FilePath == null)
            {
                throw new ResponseException("出错了，文件不存在或已删除");
            }
            model.DownCount++;
            //检查文件本地还是远程
            if (model.FilePath.ToLower().StartsWith("http://") || model.FilePath.ToLower().StartsWith("https://"))
            {
                var client = _clientFactory.CreateClient();
                var responseStream = await client.GetStreamAsync(model.FilePath);
                if (responseStream == null)
                {
                    throw new ResponseException("出错了，文件不存在或已删除");
                }
                byte[] byteData = FileHelper.ConvertStreamToByteBuffer(responseStream);
                //更新下载数量
                await _attachService.UpdateDownCount(id);
                return File(byteData, "application/octet-stream", model.FileName);
            }
            else
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                //获取物理路径
                string fullFileName = (webRootPath + model.FilePath).Replace("\\", @"/");
                //检测文件是否存在
                if (!System.IO.File.Exists(fullFileName))
                {
                    throw new ResponseException("出错了，文件不存在或已删除！");
                }
                //更新下载数量
                await _attachService.UpdateDownCount(id);
                return File(model.FilePath, "application/octet-stream", model.FileName);
            }
        }
        #endregion
    }
}
