using AutoMapper;
using DTcms.Core.API.Filters;
using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Helpers;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTcms.Core.API.Controllers
{
    /// <summary>
    /// 系统参数配置
    /// </summary>
    [Route("admin/setting")]
    [ApiController]
    public class SettingController(IConfigService configService, ISmsService smsService, IMapper mapper) : ControllerBase
    {
        private readonly IConfigService _configService = configService;
        private readonly ISmsService _smsService = smsService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 获取系统配置
        /// 示例：/admin/setting/sysconfig
        /// </summary>
        [HttpGet("{type}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Setting", ActionType.View, "type")]
        public async Task<IActionResult> Get([FromRoute] ConfigType type)
        {
            var model = await _configService.QueryAsync<SysConfig>(x => x.Type != null && x.Type.ToLower() == type.ToString(), null, WriteRoRead.Write)
                ?? throw new ResponseException($"系统参数{type}无法找到");
            if (model.Type == ConfigType.SysConfig.ToString())
            {
                return Ok(JsonHelper.ToJson<SysConfigDto>(model.JsonData));
            }
            else if (model.Type == ConfigType.MemberConfig.ToString())
            {
                return Ok(JsonHelper.ToJson<MemberConfigDto>(model.JsonData));
            }
            throw new ResponseException($"找不到{type}配置返回类型");
        }

        /// <summary>
        /// 修改系统配置
        /// 示例：/admin/setting/sysconfig
        /// </summary>
        [HttpPut("sysconfig")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Setting", ActionType.Edit, "SysConfig")]
        public async Task<IActionResult> Update([FromBody] SysConfigDto modelDto)
        {
            var model = await _configService.QueryByTypeAsync(ConfigType.SysConfig, WriteRoRead.Write)
                ?? throw new ResponseException($"系统配置不存在或已删除。");

            model.JsonData = JsonHelper.GetJson(modelDto);
            var result = await _configService.UpdateAsync<SysConfig>(model);
            if (!result)
            {
                //抛出自定义异常错误
                throw new ResponseException("保存过程中发生错误。");
            }
            return NoContent();
        }

        /// <summary>
        /// 修改会员配置
        /// 示例：/admin/setting/memberconfig
        /// </summary>
        [HttpPut("memberconfig")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Setting", ActionType.Edit, "MemberConfig")]
        public async Task<IActionResult> Update([FromBody] MemberConfigDto modelDto)
        {
            var model = await _configService.QueryByTypeAsync(ConfigType.MemberConfig, WriteRoRead.Write)
                ?? throw new ResponseException($"会员配置不存在或已删除");

            model.JsonData = JsonHelper.GetJson(modelDto);
            var result = await _configService.UpdateAsync<SysConfig>(model);
            if (!result)
            {
                //抛出自定义异常错误
                throw new ResponseException("保存过程中发生错误。");
            }
            return NoContent();
        }

        /// <summary>
        /// 获取系统管理配置(缓存)
        /// 示例：/admin/setting/center/config
        /// </summary>
        [HttpGet("center/config")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> AdminGetSysConfig()
        {
            var model = await _configService.QueryByTypeAsync($"Admin-{ConfigType.SysConfig}", ConfigType.SysConfig)
                ?? throw new ResponseException($"找不到系统配置信息");

            return Ok(JsonHelper.ToJson<SysConfigClientDto>(model.JsonData));
        }

        /// <summary>
        /// 测试发送短信
        /// 示例：/admin/setting/sms/account/test
        /// </summary>
        [HttpPost("sms/account/test")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Setting", ActionType.Edit, "SysConfig")]
        public async Task<IActionResult> SmsSend([FromBody] SmsMessageDto modelDto)
        {
            await _smsService.Send(modelDto);
            return NoContent();
        }
        #endregion

        #region 前台调用接口============================
        /// <summary>
        /// 获取系统基本配置(缓存)
        /// 示例：/client/setting/sysconfig
        /// </summary>
        [HttpGet("/client/setting/{type}")]
        public async Task<IActionResult> ClientGetSysConfig([FromRoute] ConfigType type)
        {
            var configType = type;
            if (type == ConfigType.UploadConfig)
            {
                configType = ConfigType.SysConfig;
            }

            var model = await _configService.QueryByTypeAsync(configType.ToString(), configType)
                ?? throw new ResponseException($"找不到{type}配置返回类型");

            switch (type)
            {
                case ConfigType.SysConfig:
                    return Ok(JsonHelper.ToJson<SysConfigClientDto>(model.JsonData));
                case ConfigType.UploadConfig:
                    var sysConfig = JsonHelper.ToJson<SysConfigDto>(model.JsonData);
                    var uploadConfig = _mapper.Map<UploadConfigDto>(sysConfig);
                    return Ok(uploadConfig);
                case ConfigType.MemberConfig:
                    return Ok(JsonHelper.ToJson<MemberConfigDto>(model.JsonData));
                default:
                    throw new ResponseException($"找不到{type}配置返回类型");
            }
        }
        #endregion
    }
}
