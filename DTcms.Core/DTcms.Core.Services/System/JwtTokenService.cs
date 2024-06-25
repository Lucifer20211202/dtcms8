using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Extensions;
using DTcms.Core.Common.Helpers;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DTcms.Core.Services
{
    /// <summary>
    /// JWT服务实现
    /// </summary>
    public class JwtTokenService(IDbContextFactory contentFactory,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ICacheService cacheService) : BaseService(contentFactory, cacheService), ITokenService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

        /// <summary>
        /// 用户名密码登录
        /// </summary>
        public async Task<Tokens> LoginAsync(LoginDto loginDto, WriteRoRead writeAndRead = WriteRoRead.Write)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库

            //检查验证码
            var code = MemoryHelper.Get(loginDto.CodeKey) ?? throw new ResponseException("验证码已过期，请重试");
            if (code.ToString()?.ToLower() != loginDto.CodeValue?.ToLower())
            {
                throw new ResponseException("验证码有误，请重试");
            }
            //验证完毕，删除验证码
            MemoryHelper.Remove(loginDto.CodeKey);

            //查找用户，AsNoTracking非跟踪查询
            var user = await _context.Set<ApplicationUser>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserName == loginDto.UserName || x.Email == loginDto.UserName || x.PhoneNumber == loginDto.UserName)
                ?? throw new ResponseException("用户不存在或已删除");

            if (user.Status == 1)
            {
                throw new ResponseException("账户未验证，请验证通过后登录");
            }
            if (user.Status == 2)
            {
                throw new ResponseException("账户待审核，请等待审核后登录");
            }
            if (user.Status == 3)
            {
                throw new ResponseException("账户异常，请联系管理员");
            }
            //验证用户名密码
            var loginResult = await _signInManager.PasswordSignInAsync(user.UserName, loginDto.Password, false, false);
            if (!loginResult.Succeeded)
            {
                throw new ResponseException("用户名或密码不正确");
            }
            //生成Token
            return await CreateTokenAsync(user.Id);
        }

        /// <summary>
        /// 手机验证码登录
        /// </summary>
        public async Task<Tokens> PhoneAsync(LoginPhoneDto modelDto, WriteRoRead writeAndRead = WriteRoRead.Write)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库

            //检查验证码
            var cacheObj = MemoryHelper.Get(modelDto.CodeKey) ?? throw new ResponseException("验证码已过期，请重试");
            var cacheValue = cacheObj.ToString();
            var codeSecret = MD5Helper.MD5Encrypt32(modelDto.Phone + modelDto.CodeValue);
            if (codeSecret != cacheValue)
            {
                throw new ResponseException("验证码有误，请重新获取");
            }
            //验证完毕，删除验证码
            MemoryHelper.Remove(modelDto.CodeKey);
            //查找用户，AsNoTracking非跟踪查询
            var user = await _context.Set<ApplicationUser>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.PhoneNumber == modelDto.Phone)
                ?? throw new ResponseException("用户不存在或已删除");

            if (user.Status == 1)
            {
                throw new ResponseException("账户未验证，请验证通过后登录");
            }
            if (user.Status == 2)
            {
                throw new ResponseException("账户待审核，请等待审核后登录");
            }
            if (user.Status == 3)
            {
                throw new ResponseException("账户异常，请联系管理员");
            }
            //生成Token
            return await CreateTokenAsync(user.Id);
        }

        /// <summary>
        /// 第三方授权登录
        /// </summary>
        public async Task<Tokens> OAuthAsync(int userId, WriteRoRead writeAndRead = WriteRoRead.Write)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库

            //查找用户，AsNoTracking非跟踪查询
            var user = await _context.Set<ApplicationUser>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId)
                ?? throw new ResponseException("用户不存在或已删除");

            if (user.Status == 1)
            {
                throw new ResponseException("账户未验证，请验证通过后登录");
            }
            if (user.Status == 2)
            {
                throw new ResponseException("账户待审核，请等待审核后登录");
            }
            if (user.Status == 3)
            {
                throw new ResponseException("账户异常，请联系管理员");
            }
            //生成Token
            return await CreateTokenAsync(user.Id);
        }

        /// <summary>
        /// 重设密码
        /// </summary>
        public async Task<bool> ResetAsync(PasswordResetDto modelDto, WriteRoRead writeAndRead = WriteRoRead.Write)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库

            //检查验证码是否正确
            var cacheObj = MemoryHelper.Get(modelDto.CodeKey);
            if (cacheObj == null)
            {
                throw new ResponseException("验证码已过期，请重新获取");
            }
            var cacheValue = cacheObj.ToString();
            var codeSecret = string.Empty;
            if (modelDto.Method == 1)
            {
                if (!modelDto.Phone.IsNotNullOrEmpty())
                {
                    throw new ResponseException("请填写手机号码");
                }
                codeSecret = MD5Helper.MD5Encrypt32(modelDto.Phone + modelDto.CodeValue);
            }
            else if (modelDto.Method == 2)
            {
                if (!modelDto.Email.IsNotNullOrEmpty())
                {
                    throw new ResponseException("请填写邮箱地址");
                }
                codeSecret = MD5Helper.MD5Encrypt32(modelDto.Email + modelDto.CodeValue);
            }
            else
            {
                throw new ResponseException("请选择取回方式");
            }
            if (string.IsNullOrWhiteSpace(modelDto.NewPassword))
            {
                throw new ResponseException("新密码不能为空");
            }
            //验证完毕，删除验证码
            MemoryHelper.Remove(modelDto.CodeKey);

            //查找用户
            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => (modelDto.Method == 1 && x.PhoneNumber == modelDto.Phone) || (modelDto.Method == 2 && x.Email == modelDto.Email));
            if (user == null)
            {
                throw new ResponseException("用户不存在或已删除");
            }
            //生成token，用于重置密码
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            //重置密码
            var result = await _userManager.ResetPasswordAsync(user, token, modelDto.NewPassword);
            if (!result.Succeeded)
            {
                throw new ResponseException(result.Errors.FirstOrDefault()?.Description);
            }
            return true;
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        public async Task<Tokens> GetRefreshTokenAsync(string? refreshToken, WriteRoRead writeAndRead = WriteRoRead.Write)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库

            //查找用户
            var user = await _context.Set<ApplicationUser>()
                .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken)
                ?? throw new ResponseException("认证失效，请重新登录", ErrorCode.TokenExpired, StatusCodes.Status403Forbidden);

            if (DateTime.Compare(user.LastTime.GetValueOrDefault(), DateTime.Now) > new TimeSpan(30, 0, 0, 0).Ticks)
            {
                throw new ResponseException("认证过期，请重新登录", ErrorCode.TokenExpired, StatusCodes.Status403Forbidden);
            }

            Tokens tokens = await CreateTokenAsync(user.Id);

            return tokens;
        }

        /// <summary>
        /// 生成Token以及更新用户信息
        /// </summary>
        private async Task<Tokens> CreateTokenAsync(int userId)
        {
            if (_context == null)
            {
                throw new ResponseException("请先连接数据库");
            }

            //查找用户
            var user = await _context.Set<ApplicationUser>()
                .Include(x => x.Member)
                .Include(x => x.Manager)
                .FirstOrDefaultAsync(x => x.Id == userId)
                ?? throw new ResponseException("用户不存在或已删除");

            //判断管理员或会员，用于权限验证区分
            //设置Claim
            List<Claim> claims =
            [
                new Claim(JwtRegisteredClaimNames.Nbf,
                    $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                new Claim (JwtRegisteredClaimNames.Exp,
                    $"{new DateTimeOffset(DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Authentication:Expired"]))).ToUnixTimeSeconds()}"),
                new Claim (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim (ClaimTypes.Name, user.UserName?? "")
            ];
            var roleNames = await _userManager.GetRolesAsync(user);
            foreach (var roleName in roleNames)
            {
                var roleClaim = new Claim(ClaimTypes.Role, roleName);
                claims.Add(roleClaim);
            }
            //签名Signiture
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"] ?? string.Empty));
            var token = new JwtSecurityToken(
                    //颁发者
                    issuer: _configuration["Authentication:Issuer"],
                    //接收者
                    audience: _configuration["Authentication:Audience"],
                    //过期时间
                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Authentication:Expired"])),
                    //签名证书
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                    //自定义参数
                    claims: claims
                    );
            //生成Token
            string accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            string refreshToken = GenerateToken();

            //更新用户信息
            user.RefreshToken = refreshToken;
            user.LastIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress.ToString();
            user.LastTime = DateTime.Now;
            _context.Set<ApplicationUser>().Update(user);
            var result = await this.SaveAsync();
            //异常错误提示
            if (!result)
            {
                throw new ResponseException("获取Token时发生错误");
            }
            //返回Token
            return new Tokens(accessToken, refreshToken);
        }

        /// <summary>
        /// 生成RefreshToken
        /// </summary>
        private static string GenerateToken(int size = 32)
        {
            var randomNumber = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
