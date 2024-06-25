using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 会员(显示)
    /// </summary>
    public class MembersDto : MembersEditDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 所属用户ID
        /// </summary>
        [Display(Name = "所属用户")]
        public int UserId { get; set; }

        /// <summary>
        /// 会员组名
        /// </summary>
        [Display(Name = "会员组名")]
        public string? GroupTitle { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        [Display(Name = "余额")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        [Display(Name = "积分")]
        public int Point { get; set; }

        /// <summary>
        /// 经验值
        /// </summary>
        [Display(Name = "经验值")]
        public int Exp { get; set; }

        /// <summary>
        /// 是否分销商
        /// </summary>
        [Display(Name = "是否分销商")]
        public byte IsReseller { get; set; } = 0;

        /// <summary>
        /// 消费总金额
        /// </summary>
        [Display(Name = "消费总金额")]
        public decimal OrderAmount { get; set; } = 0M;

        /// <summary>
        /// 佣金总金额
        /// </summary>
        [Display(Name = "佣金总金额")]
        public decimal CommAmount { get; set; } = 0M;

        /// <summary>
        /// 注册IP
        /// </summary>
        [Display(Name = "注册IP")]
        [MaxLength(128)]
        public string? RegIp { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        [Display(Name = "注册时间")]
        public DateTime RegTime { get; set; }

        /// <summary>
        /// 到期时间(付费会员到期时间)
        /// </summary>
        [Display(Name = "到期时间")]
        public DateTime? ExpiryTime { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        [Display(Name = "最后登录IP")]
        [MaxLength(128)]
        public string? LastIp { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        [Display(Name = "最后登录时间")]
        public DateTime? LastTime { get; set; }
    }

    /// <summary>
    /// 会员(编辑)
    /// </summary>
    public class MembersEditDto
    {
        /// <summary>
        /// 所属站点ID
        /// </summary>
        [Display(Name = "所属站点")]
        public int SiteId { get; set; }

        /// <summary>
        /// 推荐用户ID
        /// </summary>
        [Display(Name = "推荐用户")]
        public int ReferrerId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名")]
        [MinLength(3, ErrorMessage ="{0}至少{1}位字符")]
        [MaxLength(128, ErrorMessage ="{0}最多{2}位字符")]
        public string? UserName { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        [Display(Name = "邮箱地址")]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [Display(Name = "手机号码")]
        [RegularExpression(@"^(13|14|15|16|17|18|19)\d{9}$")]
        public string? Phone { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        [Display(Name = "登录密码")]
        [MinLength(6, ErrorMessage = "{0}至少{1}位字符")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        /// <summary>
        /// 状态(0正常1待验证2待审核3锁定)
        /// </summary>
        [Display(Name = "账户状态")]
        public byte Status { get; set; } = 0;

        /// <summary>
        /// 会员组
        /// </summary>
        [Display(Name = "会员组")]
        public int GroupId { get; set; }

        /// <summary>
        /// 会员头像
        /// </summary>
        [Display(Name = "会员头像")]
        [MaxLength(512)]
        public string? Avatar { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        [MaxLength(30)]
        public string? RealName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        [MaxLength(30)]
        public string? Sex { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [Display(Name = "生日")]
        public DateTime? Birthday { get; set; }
    }

    /// <summary>
    /// 会员(修改)
    /// </summary>
    public class MembersModifyDto
    {
        /// <summary>
        /// 会员头像
        /// </summary>
        [Display(Name = "会员头像")]
        [MaxLength(512)]
        public string? Avatar { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        [MaxLength(30)]
        public string? RealName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        [MaxLength(30)]
        public string? Sex { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [Display(Name = "生日")]
        public DateTime? Birthday { get; set; }
    }

    /// <summary>
    /// 会员(修改手机)
    /// </summary>
    public class MembersPhoneDto: VerifyCode
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        [Display(Name = "手机号码")]
        [RegularExpression(@"^(13|14|15|16|17|18|19)\d{9}$", ErrorMessage = "{0}填写有误")]
        public string? Phone { get; set; }
    }

    /// <summary>
    /// 会员(修改邮箱)
    /// </summary>
    public class MembersEmailDto : VerifyCode
    {
        /// <summary>
        /// 邮箱地址
        /// </summary>
        [Display(Name = "邮箱地址")]
        [EmailAddress(ErrorMessage = "{0}填写有误")]
        public string? Email { get; set; }
    }

    /// <summary>
    /// 会员(前端)
    /// </summary>
    public class MembersClientDto
    {
        /// <summary>
        /// 所属站点ID
        /// </summary>
        [Display(Name = "所属站点")]
        public int SiteId { get; set; }

        /// <summary>
        /// 所属用户ID
        /// </summary>
        [Display(Name = "所属用户")]
        public int UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名")]
        public string? UserName { get; set; }

        /// <summary>
        /// 会员头像
        /// </summary>
        [Display(Name = "会员头像")]
        public string? Avatar { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        public string? RealName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public string? Sex { get; set; }
    }

    /// <summary>
    /// 会员统计DTO
    /// </summary>
    public class MembersReportDto
    {
        /// <summary>
        /// 显示标题
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 显示数量
        /// </summary>
        public int Count { get; set; }
    }
}
