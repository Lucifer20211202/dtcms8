using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Helpers;
using DTcms.Core.Common.Extensions;
using DTcms.Core.Model.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DTcms.Core.DBFactory.Database
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int,
        ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>
    {
        private readonly DBType _dbType;
        private readonly string _connectionString = string.Empty;

        /// <summary>
        /// 默认连接写数据库
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            _dbType = Appsettings.GetValue(["ConnectionStrings", "DBType"]).ToEnum<DBType>();//数据库类型
            _connectionString = Appsettings.GetValue(["ConnectionStrings", "WriteConnection"]);//连接字符串
        }

        /// <summary>
        /// 指定读写分离模式连接
        /// </summary>
        public AppDbContext(DBType? dbType, string? connectionString)
        {
            _dbType = dbType ?? DBType.SqlServer; //数据库类型
            _connectionString = connectionString ?? String.Empty; //连接字符串
        }

        #region 实体到表的映射===========================
        //系统
        public DbSet<SysConfig>? dt_sysconfig { get; private set; }
        public DbSet<Sites>? dt_sites { get; private set; }
        public DbSet<SiteDomains>? dt_site_domains { get; private set; }
        public DbSet<SiteChannels>? dt_site_channels { get; private set; }
        public DbSet<SiteChannelFields>? dt_site_channel_fields { get; private set; }
        public DbSet<SitePayments>? dt_site_payments { get; private set; }
        public DbSet<SiteMenus>? dt_site_menus { get; private set; }
        public DbSet<SiteOAuths>? dt_site_oauths { get; private set; }
        public DbSet<SiteOAuthLogins>? dt_site_oauth_logins { get; private set; }
        public DbSet<Areas>? dt_areas { get; private set; }
        public DbSet<Payments>? dt_payments { get; private set; }
        public DbSet<NotifyTemplates>? dt_notify_templates { get; private set; }

        //管理员
        public DbSet<Managers>? dt_managers { get; private set; }
        public DbSet<ManagerLogs>? dt_manager_logs { get; private set; }
        public DbSet<ManagerMenus>? dt_manager_menus { get; private set; }
        public DbSet<ManagerMenuModels>? dt_manager_menu_models { get; private set; }
        
        //文章
        public DbSet<Articles>? dt_articles { get; private set; }
        public DbSet<ArticleGroups>? dt_article_groups { get; private set; }
        public DbSet<ArticleCategorys>? dt_article_categorys { get; private set; }
        public DbSet<ArticleCategoryRelations>? dt_article_category_relations { get; private set; }
        public DbSet<ArticleLabels>? dt_article_labels { get; private set; }
        public DbSet<ArticleLabelRelations>? dt_article_label_relations { get; private set; }
        public DbSet<ArticleAlbums>? dt_article_albums { get; private set; }
        public DbSet<ArticleAttachs>? dt_article_attachs { get; private set; }
        public DbSet<ArticleLikes>? dt_article_likes { get; private set; }
        public DbSet<ArticleContributes>? dt_article_contributes { get; private set; }
        public DbSet<ArticleFieldValues>? dt_article_field_values { get; private set; }
        public DbSet<ArticleComments>? dt_article_comments { get; private set; }
        public DbSet<ArticleCommentLikes>? dt_article_comment_likes { get; private set; }
        
        //会员
        public DbSet<Members>? dt_members { get; private set; }
        public DbSet<MemberGroups>? dt_member_groups { get; private set; }
        public DbSet<MemberRecharges>? dt_member_recharges { get; private set; }
        public DbSet<MemberAttachRecords>? dt_member_attach_records { get; private set; }
        public DbSet<MemberPointRecords>? dt_member_point_records { get; private set; }
        public DbSet<MemberBalanceRecords>? dt_member_balance_records { get; private set; }
        public DbSet<MemberMessages>? dt_member_messages { get; private set; }

        //订单
        public DbSet<OrderPayments>? dt_order_payments { get; private set; }

        //应用
        public DbSet<Adverts>? dt_app_adverts { get; private set; }
        public DbSet<AdvertBanners>? dt_app_advert_banners { get; private set; }
        public DbSet<Feedbacks>? dt_app_feedbacks { get; private set; }
        public DbSet<Links>? dt_app_links { get; private set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //默认只写了三种数据库，有需要自行加
            switch (_dbType)
            {
                case DBType.MySql:
                    optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
                    break;
                case DBType.Oracle:
                    optionsBuilder.UseOracle(_connectionString);
                    break;
                default:
                    //低于SQL Server 2016需要添加兼容配置，否则出错
                    optionsBuilder.UseSqlServer(_connectionString, o => o.UseCompatibilityLevel(120));
                    break;
            }
        }

        /// <summary>
        /// 模型构建器
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //用户导航属性与外链关系
            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.HasMany(e => e.Claims).WithOne(e => e.User).HasForeignKey(uc => uc.UserId).IsRequired();
                b.HasMany(e => e.Logins).WithOne(e => e.User).HasForeignKey(ul => ul.UserId).IsRequired();
                b.HasMany(e => e.Tokens).WithOne(e => e.User).HasForeignKey(ut => ut.UserId).IsRequired();
                b.HasMany(e => e.UserRoles).WithOne(e => e.User).HasForeignKey(ur => ur.UserId).IsRequired();
            });
            modelBuilder.Entity<ApplicationRole>(b =>
            {
                b.HasMany(e => e.UserRoles).WithOne(e => e.Role).HasForeignKey(ur => ur.RoleId).IsRequired();
                b.HasMany(e => e.RoleClaims).WithOne(e => e.Role).HasForeignKey(rc => rc.RoleId).IsRequired();
            });

            //配置实体类型映射到的表名
            modelBuilder.Entity<ApplicationUser>().ToTable("dt_users");
            modelBuilder.Entity<ApplicationRole>().ToTable("dt_roles");
            modelBuilder.Entity<ApplicationUserLogin>().ToTable("dt_user_logins");
            modelBuilder.Entity<ApplicationUserClaim>().ToTable("dt_user_claims");
            modelBuilder.Entity<ApplicationUserRole>().ToTable("dt_user_roles");
            modelBuilder.Entity<ApplicationRoleClaim>().ToTable("dt_role_claims");
            modelBuilder.Entity<ApplicationUserToken>().ToTable("dt_user_tokens");

            //添加种子数据
            IList<ApplicationRole>? roleList = JsonHelper.ToJson<IList<ApplicationRole>>(File.ReadAllText(FileHelper.GetCurrPath(@"/DataSeed/dt_roles.json")));
            if (roleList != null)
            {
                modelBuilder.Entity<ApplicationRole>().HasData(roleList);
            }
            
            IList<ApplicationUser>? userList = JsonHelper.ToJson<IList<ApplicationUser>>(File.ReadAllText(FileHelper.GetCurrPath(@"/DataSeed/dt_users.json")));
            if(userList != null)
            {
                modelBuilder.Entity<ApplicationUser>().HasData(userList);
            }
            
            IList<ApplicationUserRole>? userRoleList = JsonHelper.ToJson<IList<ApplicationUserRole>>(File.ReadAllText(FileHelper.GetCurrPath(@"/DataSeed/dt_user_roles.json")));
            if(userRoleList != null)
            {
                modelBuilder.Entity<ApplicationUserRole>().HasData(userRoleList);
            }
            
            IList<Managers>? managerList = JsonHelper.ToJson<IList<Managers>>(File.ReadAllText(FileHelper.GetCurrPath(@"/DataSeed/dt_managers.json")));
            if (managerList != null)
            {
                modelBuilder.Entity<Managers>().HasData(managerList);
            }
            
            IList<SysConfig>? configList = JsonHelper.ToJson<IList<SysConfig>>(File.ReadAllText(FileHelper.GetCurrPath(@"/DataSeed/dt_sysconfig.json")));
            if (configList != null)
            {
                modelBuilder.Entity<SysConfig>().HasData(configList);
            }
            
            IList<Sites>? siteList = JsonHelper.ToJson<IList<Sites>>(File.ReadAllText(FileHelper.GetCurrPath(@"/DataSeed/dt_sites.json")));
            if (siteList != null)
            {
                modelBuilder.Entity<Sites>().HasData(siteList);
            }

            IList<SiteChannels>? channelList = JsonHelper.ToJson<IList<SiteChannels>>(File.ReadAllText(FileHelper.GetCurrPath(@"/DataSeed/dt_site_channels.json")));
            if (channelList != null)
            {
                modelBuilder.Entity<SiteChannels>().HasData(channelList);
            }

            IList<SiteMenus>? menulList = JsonHelper.ToJson<IList<SiteMenus>>(File.ReadAllText(FileHelper.GetCurrPath(@"/DataSeed/dt_site_menus.json")));
            if (menulList != null)
            {
                modelBuilder.Entity<SiteMenus>().HasData(menulList);
            }

            IList<ManagerMenuModels>? modelList = JsonHelper.ToJson<IList<ManagerMenuModels>>(File.ReadAllText(FileHelper.GetCurrPath(@"/DataSeed/dt_manager_menu_models.json")));
            if (modelList != null)
            {
                modelBuilder.Entity<ManagerMenuModels>().HasData(modelList);
            }
            
            IList<ManagerMenus>? menuList = JsonHelper.ToJson<IList<ManagerMenus>>(File.ReadAllText(FileHelper.GetCurrPath(@"/DataSeed/dt_manager_menus.json")));
            if (menuList != null)
            {
                modelBuilder.Entity<ManagerMenus>().HasData(menuList);
            }
            
            IList<Payments>? payList = JsonHelper.ToJson<IList<Payments>>(File.ReadAllText(FileHelper.GetCurrPath(@"/DataSeed/dt_payments.json")));
            if (payList != null)
            {
                modelBuilder.Entity<Payments>().HasData(payList);
            }

            IList<NotifyTemplates>? notifyList = JsonHelper.ToJson<IList<NotifyTemplates>>(File.ReadAllText(FileHelper.GetCurrPath(@"/DataSeed/dt_notify_templates.json")));
            if (notifyList != null)
            {
                modelBuilder.Entity<NotifyTemplates>().HasData(notifyList);
            }

            IList<MemberGroups>? groupList = JsonHelper.ToJson<IList<MemberGroups>>(File.ReadAllText(FileHelper.GetCurrPath(@"/DataSeed/dt_member_groups.json")));
            if (groupList != null)
            {
                modelBuilder.Entity<MemberGroups>().HasData(groupList);
            }
        }
    }
}
