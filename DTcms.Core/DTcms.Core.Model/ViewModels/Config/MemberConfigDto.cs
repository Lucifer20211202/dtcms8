﻿using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 会员设置
    /// </summary>
    public class MemberConfigDto
    {
        /// <summary>
        /// 新用户注册设置
        /// 0.开放注册 1.关闭注册
        /// </summary>
        [Display(Name = "注册设置")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int RegStatus { get; set; } = 0;

        /// <summary>
        /// 新用户注册审核
        /// 0不需要 1人工审核
        /// </summary>
        [Display(Name = "注册审核")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int RegVerify { get; set; } = 0;

        /// <summary>
        /// 注册欢迎短信息
        /// 0不发送 1站内短消息 2发送邮件 3手机短信
        /// </summary>
        [Display(Name = "欢迎消息")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int RegMsgStatus { get; set; } = 0;

        /// <summary>
        /// 欢迎短信息内容
        /// </summary>
        [Display(Name = "短消息内容")]
        [Required(ErrorMessage = "{0}不可为空")]
        public string? RegMsgTxt { get; set; }

        /// <summary>
        /// 用户名保留关健字
        /// </summary>
        [Display(Name = "用户名保留关健字")]
        [Required(ErrorMessage = "{0}不可为空")]
        public string? RegKeywords { get; set; } = "admin,administrator,test";

        /// <summary>
        /// IP注册间隔限制0不限制(小时)
        /// </summary>
        [Display(Name = "注册间隔限制")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int RegCtrl { get; set; } = 0;

        /// <summary>
        /// 验证码间隔限制(分钟)须大于0
        /// </summary>
        [Display(Name = "验证码间隔限制")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int RegCodeCtrl { get; set; } = 2;

        /// <summary>
        /// 验证码生成位数(位)
        /// </summary>
        [Display(Name = "验证码位数")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int RegCodeLength { get; set; } = 4;

        /// <summary>
        /// 手机验证码有效期(分钟)须大于0
        /// </summary>
        [Display(Name = "验证码有效期")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int RegSmsExpired { get; set; } = 10;

        /// <summary>
        /// 邮件链接有效期(天)须大于0
        /// </summary>
        [Display(Name = "邮件有效期")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int RegEmailExpired { get; set; } = 1;

        /// <summary>
        /// 注册许可协议(0否1是)
        /// </summary>
        [Display(Name = "许可协议开关")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int RegRules { get; set; } = 0;

        /// <summary>
        /// 许可协议内容
        /// </summary>
        public string? RegRulesTxt { get; set; }

        /// <summary>
        /// 现金/积分兑换比例0禁用
        /// </summary>
        [Display(Name = "现金积分兑换比例")]
        [Required(ErrorMessage = "{0}不可为空")]
        public decimal PointCashRate { get; set; } = 0;

        /// <summary>
        /// 每天登录获得积分
        /// </summary>
        [Display(Name = "每天登录积分")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int PointLoginNum { get; set; } = 0;
    }
}
