using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Helpers;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.ViewModels;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 手机短信接口实现
    /// </summary>
    public class SmsService(IDbContextFactory contentFactory, IConfigService configService, ICacheService cacheService)
        : BaseService(contentFactory, cacheService), ISmsService
    {
        private readonly IConfigService _configService = configService;

        /// <summary>
        /// 发送短信
        /// </summary>
        public async Task<string> Send(SmsMessageDto modelDto, WriteRoRead writeAndRead = WriteRoRead.Read)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库

            if (modelDto.PhoneNumbers == null)
            {
                throw new ResponseException($"手机号码不能为空，请检查重试");
            }
            if (modelDto.TemplateId == null || modelDto.TemplateParam == null)
            {
                throw new ResponseException($"请设置短信模板标识，请检查重试");
            }
            //查询系统配置
            var configModel = await _configService.QueryByTypeAsync(ConfigType.SysConfig);
            if (configModel == null)
            {
                throw new ResponseException($"获取系统参数有误，请联系管理员");
            }
            var config = JsonHelper.ToJson<SysConfigDto>(configModel.JsonData);
            if (config == null)
            {
                throw new ResponseException($"系统参数格式有误，请联系管理员");
            }
            if (string.IsNullOrEmpty(config.SmsSecretId) || string.IsNullOrEmpty(config.SmsSecretKey) || string.IsNullOrEmpty(config.SmsSignTxt))
            {
                throw new ResponseException($"短信配置参数有误，请联系管理员");
            }

            //检查短信平台
            string? requestId;
            switch (config.SmsProvider)
            {
                //阿里云
                case 1:
                    requestId = AliyunSmsHelper.Send(config.SmsSecretId, config.SmsSecretKey, config.SmsSignTxt,
                        modelDto.PhoneNumbers, modelDto.TemplateId, modelDto.TemplateParam);
                    break;
                //腾讯云
                case 2:
                    //手机号转换成数组
                    string[] phoneNumbers = modelDto.PhoneNumbers.Split(',');
                    //模板参数转换成数组
                    string[] templateParam = modelDto.TemplateParam.Split(",");
                    requestId = TencentSmsHelper.Send(config.SmsSecretId, config.SmsSecretKey, config.SmsAppId, config.SmsSignTxt,
                        phoneNumbers, modelDto.TemplateId, templateParam);
                    break;
                default:
                    throw new ResponseException($"无法确认短信服务商，请联系管理员");
            }

            //返回状态
            if (requestId == null)
            {
                throw new ResponseException($"短信发送出现未知错误，请联系管理员");
            }
            return requestId;
        }
    }
}