using AutoMapper;
using DTcms.Core.API.Filters;
using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Extensions;
using DTcms.Core.Common.Helpers;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using DTcms.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DTcms.Core.API.Controllers
{
    /// <summary>
    /// 会员信息
    /// </summary>
    [Route("admin/member")]
    [ApiController]
    public class MemberController(IMemberService memberService, IUserService userService, IConfigService configService, ISiteService siteService,
        IHttpContextAccessor httpContextAccessor, IMapper mapper) : ControllerBase
    {
        private readonly IMemberService _memberService = memberService;
        private readonly IUserService _userService = userService;
        private readonly IConfigService _configService = configService;
        private readonly ISiteService _siteService = siteService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 根据ID获取一条记录
        /// 示例：/admin/member/1
        /// </summary>
        [HttpGet("{userId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Member", ActionType.View)]
        public async Task<IActionResult> GetById([FromRoute] int userId, [FromQuery] MemberParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<MembersDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _memberService.QueryAsync<Members>(x => x.UserId == userId,
                query => query.Include(x => x.User).Include(x => x.Group), WriteRoRead.Write)
                ?? throw new ResponseException($"会员[{userId}]不存在或已删除");

            //根据字段进行塑形
            var result = _mapper.Map<MembersDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取总记录数量
        /// 示例：/admin/member/view/count
        /// </summary>
        [HttpGet("view/count")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Member", ActionType.View)]
        public async Task<IActionResult> GetCount([FromQuery] MemberParameter searchParam)
        {
            var result = await _memberService.QueryCountAsync(x => searchParam.Status < 0 || (x.User != null && x.User.Status == searchParam.Status));
            //返回成功200
            return Ok(result);
        }

        /// <summary>
        /// 获取会员注册统计
        /// 示例：/admin/member/view/report
        /// </summary>
        [HttpGet("view/report")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Member", ActionType.View)]
        public async Task<IActionResult> GetMemberRegister([FromQuery] MemberParameter searchParam)
        {
            if (searchParam.StartTime == null)
            {
                searchParam.StartTime = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
            }
            if (searchParam.EndTime == null)
            {
                searchParam.EndTime = DateTime.Now;
            }
            var result = await _memberService.QueryCountListAsync(0,
                x => x.RegTime >= searchParam.StartTime.Value
                && x.RegTime <= searchParam.EndTime.Value);
            return Ok(result);
        }

        // <summary>
        /// 获取分页列表
        /// 示例：/admin/member?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Member", ActionType.View)]
        public async Task<IActionResult> GetList([FromQuery] MemberParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy!=null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<MembersDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<MembersDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据列表
            var list = await _memberService.QueryPageAsync<Members>(
                pageParam.PageSize,
                pageParam.PageIndex,
                x => x.User != null
                && (searchParam.SiteId <= 0 || x.SiteId == searchParam.SiteId)
                && (searchParam.Status < 0 || x.User.Status == searchParam.Status)
                && (string.IsNullOrWhiteSpace(searchParam.Keyword)
                    || (x.RealName != null && x.RealName.Contains(searchParam.Keyword))
                    || (x.User.UserName != null && x.User.UserName.Contains(searchParam.Keyword))
                    || (x.User.Email != null && x.User.Email.Contains(searchParam.Keyword))
                    || (x.User.PhoneNumber != null && x.User.PhoneNumber.Contains(searchParam.Keyword))
                ),
                query => query.Include(x => x.User).Include(x => x.Group),
                searchParam.OrderBy ?? "-Id,-RegTime");

            //x-pagination
            var paginationMetadata = new
            {
                totalCount = list.TotalCount,
                pageSize = list.PageSize,
                pageIndex = list.PageIndex,
                totalPages = list.TotalPages
            };
            Response.Headers.Append("x-pagination", JsonHelper.ToJson(paginationMetadata));

            //映射成DTO
            var resultDto = _mapper.Map<IEnumerable<MembersDto>>(list.Items).ShapeData(searchParam.Fields);
            return Ok(resultDto);
        }

        /// <summary>
        /// 添加一条记录
        /// 示例：/admin/member
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Member", ActionType.Add)]
        public async Task<IActionResult> Add([FromBody] MembersEditDto modelDto)
        {
            var model = await _memberService.AddAsync(modelDto);
            var result = _mapper.Map<MembersDto>(model);
            return Ok(result);
        }

        /// <summary>
        /// 修改一条记录
        /// 示例：/admin/member/1
        /// </summary>
        [HttpPut("{userId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Member", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int userId, [FromBody] MembersEditDto modelDto)
        {
            var result = await _memberService.UpdateAsync(userId, modelDto);
            return NoContent();
        }

        /// <summary>
        /// 局部更新一条记录
        /// 示例：/admin/member/1
        /// Body：[{"op":"replace","path":"/title","value":"new title"}]
        /// </summary>
        [HttpPatch("{userId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Member", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int userId, [FromBody] JsonPatchDocument<MembersEditDto> patchDocument)
        {
            var model = await _memberService.QueryAsync<Members>(x => x.UserId == userId, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{userId}]不存在或已删除");

            var modelToPatch = _mapper.Map<MembersEditDto>(model);
            patchDocument.ApplyTo(modelToPatch, ModelState);
            //验证数据是否合法
            if (!TryValidateModel(modelToPatch))
            {
                return ValidationProblem(ModelState);
            }
            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelToPatch, model);
            await _memberService.SaveAsync();
            return NoContent();
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/admin/member/1
        /// </summary>
        [HttpDelete("{userId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Member", ActionType.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int userId)
        {
            //查找记录是否存在
            if (!await _memberService.ExistsAsync<Members>(x => x.UserId == userId))
            {
                throw new ResponseException($"数据{userId}不存在或已删除");
            }
            var result = await _memberService.DeleteAsync(userId);

            return NoContent();
        }

        /// <summary>
        /// 批量删除记录(级联数据)
        /// 示例：/admin/member?ids=1,2,3
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Member", ActionType.Delete)]
        public async Task<IActionResult> DeleteByIds([FromQuery] string Ids)
        {
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //将ID列表转换成IEnumerable
            var listIds = Ids.ToIEnumerable<int>() ?? throw new ResponseException("传输参数不符合规范");
            //将符合条件的会员ID一次查询出来
            var list = await _memberService.QueryListAsync<Members>(0, x => listIds.Contains(x.UserId));
            //执行批量删除操作
            foreach (var modelt in list)
            {
                await _memberService.DeleteAsync(modelt.UserId);
            }
            return NoContent();
        }

        /// <summary>
        /// 批量审核记录
        /// 示例：/admin/member
        /// </summary>
        [HttpPut("/admin/member")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Member", ActionType.Audit)]
        public async Task<IActionResult> Audit([FromQuery] string Ids)
        {
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //将ID列表转换成IEnumerable
            var listIds = Ids.ToIEnumerable<int>() ?? throw new ResponseException("传输参数不符合规范");
            //将符合条件的会员ID一次查询出来
            var list = await _memberService.QueryListAsync<Members>(0,
                x => x.User != null
                && (x.User.Status == 1 || x.User.Status == 2)
                && listIds.Contains(x.UserId),
                query => query.Include(x => x.User), "-RegTime,-Id",
                WriteRoRead.Write);
            //执行批量删除操作
            foreach (var modelt in list)
            {
                if (modelt.User != null)
                {
                    modelt.User.Status = 0;
                }
            }
            var result = await _memberService.SaveAsync();
            return NoContent();
        }
        #endregion

        #region 当前用户调用接口========================
        /// <summary>
        /// 会员注册
        /// 示例：/account/member/register?rid=1
        /// </summary>
        [HttpPost("/account/member/register")]
        public async Task<IActionResult> AccountAdd([FromQuery] int Rid, [FromBody] RegisterDto registerDto)
        {
            //检查验证码是否正确
            var cacheObj = MemoryHelper.Get(registerDto.CodeKey)
                ?? throw new ResponseException("验证码已过期，请重新获取");

            var cacheValue = cacheObj.ToString();
            var codeSecret = string.Empty;
            if (registerDto.Method == 1)
            {
                if (!registerDto.Phone.IsNotNullOrEmpty())
                {
                    throw new ResponseException("请填写手机号码");
                }
                codeSecret = MD5Helper.MD5Encrypt32(registerDto.Phone + registerDto.CodeValue);
            }
            else if (registerDto.Method == 2)
            {
                if (!registerDto.Email.IsNotNullOrEmpty())
                {
                    throw new ResponseException("请填写邮箱地址");
                }
                codeSecret = MD5Helper.MD5Encrypt32(registerDto.Email + registerDto.CodeValue);
            }
            else
            {
                if (!registerDto.UserName.IsNotNullOrEmpty())
                {
                    throw new ResponseException("请填写用户名");
                }
                codeSecret = registerDto.CodeValue;
            }
            if (cacheValue?.ToLower() != codeSecret?.ToLower())
            {
                throw new ResponseException("验证码有误，请重新获取");
            }
            //检查站点是否正确
            if (!await _siteService.ExistsAsync<Sites>(x => x.Id == registerDto.SiteId))
            {
                throw new ResponseException("所属站点不存在或已删除");
            }
            //取得会员参数设置
            var jsonData = await _configService.QueryByTypeAsync(ConfigType.MemberConfig)
                ?? throw new ResponseException("会员设置参数有误，请联系管理员");
            var memberConfig = JsonHelper.ToJson<MemberConfigDto>(jsonData.JsonData)
                ?? throw new ResponseException("会员设置参数格式有误，请联系管理员");

            var modelDto = _mapper.Map<MembersEditDto>(registerDto); //将注册DTO映射成修改DTO
            //检查是否有推荐人
            if (Rid > 0 && await _memberService.ExistsAsync<Members>(x => x.UserId == Rid))
            {
                modelDto.ReferrerId = Rid;
            }
            //检查系统是否开放注册
            if (memberConfig.RegStatus == 1)
            {
                throw new ResponseException("系统暂停开放注册，请稍候再试");
            }
            //检查保留用户名关健字
            if (memberConfig.RegKeywords != null && modelDto.UserName != null)
            {
                var keywords = memberConfig.RegKeywords.Split(',');
                if (keywords.Any(x => x.ToLower().Equals(modelDto.UserName.ToLower())))
                {
                    throw new ResponseException("用户名被系统保留，请更换");
                }
            }
            //检查同一IP注册的时间间隔
            if (memberConfig.RegCtrl > 0)
            {
                //获取客户IP地址
                var userIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
                //查找同一IP下最后的注册用户
                if (await _memberService.QueryAsync<Members>(x => x.RegIp != null && x.RegIp.Equals(userIp)
                    && DateTime.Compare(x.RegTime.AddHours(memberConfig.RegCtrl), DateTime.Now) > 0) != null)
                {
                    throw new ResponseException($"同IP注册过于频繁，请稍候再试");
                }
            }
            //新用户是否开启人工审核
            if (memberConfig.RegVerify == 1)
            {
                modelDto.Status = 2;
            }

            var model = await _memberService.AddAsync(modelDto);
            MemoryHelper.Remove(registerDto.CodeKey); //删除验证码缓存

            var result = _mapper.Map<MembersDto>(model);

            return Ok(result);
        }

        /// <summary>
        /// 获取当前会员信息
        /// 示例：/account/member/info
        /// </summary>
        [HttpGet("/account/member/info")]
        [Authorize]
        public async Task<IActionResult> AccountGetById([FromQuery] MemberParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<MembersDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //获取登录用户ID
            int userId = _userService.GetUserId();
            if (userId == 0)
            {
                throw new ResponseException("用户尚未登录");
            }
            //查询数据库获取实体
            var model = await _memberService.QueryAsync<Members>(x => x.UserId == userId, qurey => qurey.Include(x => x.User).Include(x => x.Group), WriteRoRead.Write)
                ?? throw new ResponseException($"会员{userId}不存在或已删除");

            //根据字段进行塑形
            var result = _mapper.Map<MembersDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取会员团队一级列表
        /// 示例：/account/member/team/1
        /// </summary>
        [HttpGet("/account/member/team/{userId}")]
        [Authorize]
        public async Task<IActionResult> AccountGetList([FromRoute] int userId, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<MembersClientDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<MembersClientDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //获取数据库列表
            var list = await _memberService.QueryListAsync<Members>(0,
                x => x.ReferrerId > 0 && x.ReferrerId == userId,
                query => query.Include(x => x.User),
                searchParam.OrderBy ?? "Id");

            //映射成DTO，根据字段进行塑形
            var result = _mapper.Map<IEnumerable<MembersClientDto>>(list).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(result);
        }

        /// <summary>
        /// 修改一条记录
        /// 示例：/account/member/info
        /// </summary>
        [HttpPut("/account/member/info")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] MembersModifyDto modelDto)
        {
            //获取登录用户ID
            int userId = _userService.GetUserId();
            if (userId == 0)
            {
                throw new ResponseException("用户尚未登录");
            }
            var model = await _memberService.QueryAsync<Members>(x => x.UserId == userId, null, WriteRoRead.Write);
            _mapper.Map(modelDto, model);
            await _memberService.SaveAsync();
            return NoContent();
        }

        /// <summary>
        /// 修改手机号码
        /// 示例：/account/member/phone
        /// </summary>
        [HttpPut("/account/member/phone")]
        [Authorize]
        public async Task<IActionResult> UpdatePhone([FromBody] MembersPhoneDto modelDto)
        {
            //获取登录用户ID
            int userId = _userService.GetUserId();
            if (userId == 0)
            {
                throw new ResponseException("用户尚未登录");
            }
            //检查验证码是否正确
            var cacheObj = MemoryHelper.Get(modelDto.CodeKey)
                ?? throw new ResponseException("验证码已过期，请重新获取");
            var cacheValue = cacheObj.ToString();
            var codeSecret = string.Empty;
            codeSecret = MD5Helper.MD5Encrypt32(modelDto.Phone + modelDto.CodeValue);
            if (cacheValue?.ToLower() != codeSecret?.ToLower())
            {
                throw new ResponseException("验证码有误，请重新获取");
            }
            MemoryHelper.Remove(modelDto.CodeKey); //删除验证码缓存
            //检查手机号码是否重复
            if (await _memberService.ExistsAsync<ApplicationUser>(x => x.PhoneNumber == modelDto.Phone))
            {
                throw new ResponseException("手机号码已存在，请更换后重试");
            }
            //保存到数据库
            var model = await _memberService.QueryAsync<ApplicationUser>(x => x.Id == userId, null, WriteRoRead.Write)
                ?? throw new ResponseException("账户不存在或删除，请核实后重试");
            model.PhoneNumber = modelDto.Phone;
            await _memberService.SaveAsync();
            return NoContent();
        }

        /// <summary>
        /// 修改邮箱账号
        /// 示例：/account/member/email
        /// </summary>
        [HttpPut("/account/member/email")]
        [Authorize]
        public async Task<IActionResult> UpdateEmail([FromBody] MembersEmailDto modelDto)
        {
            //获取登录用户ID
            int userId = _userService.GetUserId();
            if (userId == 0)
            {
                throw new ResponseException("用户尚未登录");
            }
            if (modelDto.Email == null)
            {
                throw new ResponseException("请填写邮箱账户");
            }
            //检查验证码是否正确
            var cacheObj = MemoryHelper.Get(modelDto.CodeKey)
                ?? throw new ResponseException("验证码已过期，请重新获取");
            var cacheValue = cacheObj.ToString();
            var codeSecret = string.Empty;
            codeSecret = MD5Helper.MD5Encrypt32(modelDto.Email + modelDto.CodeValue);
            if (cacheValue?.ToLower() != codeSecret?.ToLower())
            {
                throw new ResponseException("验证码有误，请重新获取");
            }
            MemoryHelper.Remove(modelDto.CodeKey); //删除验证码缓存
            //检查邮箱是否重复
            if (await _memberService.ExistsAsync<ApplicationUser>(x => x.Email!=null&&x.Email.ToLower() == modelDto.Email.ToLower()))
            {
                throw new ResponseException("邮箱已存在，请更换后重试");
            }
            //保存到数据库
            var model = await _memberService.QueryAsync<ApplicationUser>(x => x.Id == userId, null, WriteRoRead.Write)
                ?? throw new ResponseException("账户不存在或删除，请核实后重试");
            model.Email = modelDto.Email;
            await _memberService.SaveAsync();
            return NoContent();
        }

        /// <summary>
        /// 修改当前会员密码
        /// 示例：/account/member/password
        /// </summary>
        [HttpPut("/account/member/password")]
        [Authorize]
        public async Task<IActionResult> AccountPassword([FromBody] PasswordDto modelDto)
        {
            await _userService.UpdatePasswordAsync(modelDto);
            return NoContent();
        }
        #endregion

        #region 前台调用接口============================
        /// <summary>
        /// 检查获取当前会员信息(公共)
        /// 示例：/client/member
        /// </summary>
        [HttpGet("/client/member")]
        public async Task<IActionResult> ClientGetById([FromQuery] MemberParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<MembersDto>())
            {
                throw new ResponseException("请输入正确的属性参数", ErrorCode.NotFound, 404);
            }
            //获取登录用户ID
            int userId = _userService.GetUserId();
            if (userId == 0)
            {
                throw new ResponseException("用户尚未登录", ErrorCode.NotFound, 404);
            }

            //查询数据库获取实体
            var model = await _memberService.QueryAsync<Members>(x => x.UserId == userId, qurey => qurey.Include(x => x.User).Include(x => x.Group))
                ?? throw new ResponseException($"会员{userId}不存在或已删除", ErrorCode.NotFound, 404);
            //根据字段进行塑形
            var result = _mapper.Map<MembersDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }
        #endregion
    }
}
