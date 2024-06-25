using DTcms.Core.Common.Emums;
using DTcms.Core.Model.ViewModels;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// Token服务接口
    /// </summary>
    public interface ITokenService : IBaseService
    {
        /// <summary>
        /// 用户名密码登录
        /// </summary>
        Task<Tokens> LoginAsync(LoginDto loginDto, WriteRoRead writeAndRead = WriteRoRead.Write);

        /// <summary>
        /// 手机验证码登录
        /// </summary>
        Task<Tokens> PhoneAsync(LoginPhoneDto loginDto, WriteRoRead writeAndRead = WriteRoRead.Write);

        /// <summary>
        /// 第三方授权登录
        /// </summary>
        Task<Tokens> OAuthAsync(int userId, WriteRoRead writeAndRead = WriteRoRead.Write);

        /// <summary>
        /// 重设密码
        /// </summary>
        Task<bool> ResetAsync(PasswordResetDto modelDto, WriteRoRead writeAndRead = WriteRoRead.Write);

        /// <summary>
        /// 刷新token
        /// </summary>
        Task<Tokens> GetRefreshTokenAsync(string? refreshToken, WriteRoRead writeAndRead = WriteRoRead.Write);
    }
}
