﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 管理日志
    /// </summary>
    public class ManagerLogs
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名")]
        [StringLength(128)]
        public string? UserName { get; set; }

        /// <summary>
        /// 请求方法
        /// </summary>
        [Display(Name = "请求方法")]
        [StringLength(128)]
        public string? Method { get; set; }

        /// <summary>
        /// 请求路径
        /// </summary>
        [Display(Name = "请求路径")]
        [StringLength(512)]
        public string? Path { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        [Display(Name = "请求参数")]
        [StringLength(512)]
        public string? Query { get; set; }

        /// <summary>
        /// 响应状态码
        /// </summary>
        [Display(Name = "响应状态码")]
        [StringLength(128)]
        public string? StatusCode { get; set; }

        /// <summary>
        /// 记录时间
        /// </summary>
        [Display(Name = "记录时间")]
        public DateTime AddTime { get; set; } = DateTime.Now;
    }
}
