using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DTcms.Core.API.Controllers
{
    /// <summary>
    /// 用户登录接口
    /// </summary>
    [Route("auth")]
    [ApiController]
    public class AuthorizeController(
        ITokenService tokenService,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<ApplicationUser> signInManager) : ControllerBase
    {
        private readonly ITokenService _tokenService = tokenService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

        /// <summary>
        /// 刷新Token
        /// </summary>
        [AllowAnonymous]
        [HttpPost("retoken")]
        public async Task<IActionResult> RefreshToken([FromBody] Tokens token)
        {
            var resultDto = await _tokenService.GetRefreshTokenAsync(token.RefreshToken);
            return Ok(resultDto);
        }

        /// <summary>
        /// 用户名密码登录
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var resultDto = await _tokenService.LoginAsync(loginDto);
            return Ok(resultDto);
        }

        /// <summary>
        /// 手机验证码登录
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login/phone")]
        public async Task<IActionResult> LoginPhone([FromBody] LoginPhoneDto loginDto)
        {
            var resultDto = await _tokenService.PhoneAsync(loginDto);
            return Ok(resultDto);
        }

        /// <summary>
        /// 重设登录密码
        /// </summary>
        [AllowAnonymous]
        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto modelDto)
        {
            await _tokenService.ResetAsync(modelDto);
            return NoContent();
        }

        /// <summary>
        /// 注销登录
        /// </summary>
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }
    }
}
