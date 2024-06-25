using Autofac;
using Autofac.Extensions.DependencyInjection;
using DTcms.Core.API.Filters;
using DTcms.Core.Common.Helpers;
using DTcms.Core.Common.Emums;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.Model.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.Features;
using NLog.Web;
using System.Reflection;
using System.Text;
using DTcms.Core.Model.ViewModels;
using DTcms.Core.API.Handler;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

//上传文件配置
builder.WebHost.ConfigureKestrel(options =>
{
    //HTTP 请求行的最大允许大小。 默认为 8kb
    options.Limits.MaxRequestLineSize = int.MaxValue;
    //请求缓冲区的最大大小。 默认为 1M
    options.Limits.MaxRequestBufferSize = int.MaxValue;
    //任何请求正文的最大允许大小（以字节为单位）,默认 30,000,000 字节，大约为 28.6MB
    options.Limits.MaxRequestBodySize = int.MaxValue;//限制请求长度

});
//设置接收文件长度的最大值。
builder.Services.Configure<FormOptions>(opt =>
{
    opt.ValueLengthLimit = int.MaxValue;
    opt.MultipartBodyLengthLimit = int.MaxValue;
    opt.MultipartHeadersLengthLimit = int.MaxValue;
});
builder.Services.Configure<IISServerOptions>(opt =>
{
    opt.MaxRequestBodySize = int.MaxValue;
});

//注入NLog记录日志
builder.Logging.AddNLog("nlog.config");

//Autofac注入
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    //自动注册服务及允许AOP拦截
    Assembly service = Assembly.Load("DTcms.Core.Services");
    Assembly iservice = Assembly.Load("DTcms.Core.IServices");
    //类名以service结尾，且类型不能是抽象的
    builder.RegisterAssemblyTypes(service, iservice)
        .Where(t => (t.FullName != null && t.FullName.EndsWith("Service")) && !t.IsAbstract)
        .AsImplementedInterfaces()
        .InstancePerDependency(); //生命周期
});

//配置缓存服务器
builder.Services.Configure<CacheSettingsDto>(builder.Configuration.GetSection("CacheSettings"));
var cacheSetting = builder.Configuration.GetSection("CacheSettings").Get<CacheSettingsDto>();
if (cacheSetting != null && cacheSetting.Enabled)
{
    switch (cacheSetting.Provider)
    {
        case "InMemory":
            builder.Services.AddDistributedMemoryCache();
            break;
    }
}

//其它帮助类注入
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddSingleton(new Appsettings(builder.Environment.ContentRootPath));//配置文件读写帮助类注入
builder.Services.AddSingleton(new FileHelper(builder.Environment.ContentRootPath));//文件帮助类注入
builder.Services.AddTransient<IDbContextFactory, DbContextFactory>();//注册数据库连接服务

//IdentityUser配置
builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.AllowedUserNameCharacters = null; //允许中文注册
    options.Password.RequiredLength = 6;//密码最小长度
    options.Password.RequireNonAlphanumeric = false;//密码必须包含字母关闭
    options.Password.RequireUppercase = false;//密码必须包含大写关闭
    options.Password.RequireLowercase = false;//密码必须包含小写关闭
});
builder.Services.AddDbContext<AppDbContext>(); //IdentityUser数据上下文
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders(); //使用IdentityUser模型

//注入资源授权处理Handler，实现IAuthorizationRequirement接口的类
builder.Services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
//注入Swagger服务
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v8", new OpenApiInfo { Version = "v8.0", Title = $"DTcms.Core API文档" });
    //获取应用程序所在目录(绝对,不受工作目录影响，建议采用此方法获取路径)
    string basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location) ?? String.Empty;
    //项目属性生成配置的xml文件名
    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //启用控制器注释
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
});

//控制器响应配置
builder.Services.AddControllers(setupAction =>
{
    //默认响应JSON格式
    setupAction.ReturnHttpNotAcceptable = true;
})
    .AddNewtonsoftJson(opt =>
    {
        opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        opt.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    })
    .AddXmlDataContractSerializerFormatters()//开启XML输入输出支持
    .ConfigureApiBehaviorOptions(setupAction =>
    {
        //数据验证失败响应
        setupAction.InvalidModelStateResponseFactory = context =>
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status422UnprocessableEntity,
            };
            //取第一条错误信息返回
            var resultMessage = new ResponseMessage(ErrorCode.ParamError, problemDetails.Errors.Values, context.HttpContext);
            //返回422状态码
            return new UnprocessableEntityObjectResult(resultMessage)
            {
                ContentTypes = { "application/json" }
            };

        };
    });

//JWT认证服务配置
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateLifetime = true,//是否验证失效时间
            ClockSkew = TimeSpan.FromSeconds(30),

            ValidateAudience = true,//是否验证Audience
                                    //动态验证的方式，刷新token，旧token强制失效
            AudienceValidator = (m, n, z) =>
            {
                return m != null && string.Equals(m.FirstOrDefault(), builder.Configuration["Authentication:Audience"]);
            },
            ValidateIssuer = true,//是否验证Issuer
            ValidIssuer = builder.Configuration["Authentication:Issuer"],

            ValidateIssuerSigningKey = true,//是否验证SecurityKey
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretKey"] ?? string.Empty))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                //Token expired
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Append("token-expired", "true");
                }
                return Task.CompletedTask;
            }
        };
    });

//禁止跳转到登录页，直接返回401
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
});

//扫描Profile文件
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//全局异常配置
builder.Services.AddMvc(setupAction => {
    //全局异常过滤
    setupAction.Filters.Add(typeof(GlobalExceptionFilter));
    //全局请求过滤
    setupAction.Filters.Add(typeof(GlobalRequestFilter));
});
//配置跨域处理，允许配置文件AllowedHosts节点的值
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", options =>
        options.WithOrigins(builder.Configuration["AllowedHosts"] ?? string.Empty)
        .AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
        .WithExposedHeaders("content-disposition", "token-expired", "x-pagination"));
});
//配置Swagger中带JWT报文头
builder.Services.AddSwaggerGen(c =>
{
    var scheme = new OpenApiSecurityScheme()
    {
        Description = "Authorization header. \r\nExample: 'Bearer 12345abcdef'",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Authorization"
        },
        Scheme = "oauth2",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    };
    c.AddSecurityDefinition("Authorization", scheme);
    var requirement = new OpenApiSecurityRequirement
    {
        [scheme] = new List<string>()
    };
    c.AddSecurityRequirement(requirement);
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    //使用Swagger中间件
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint($"/swagger/v8/swagger.json", $"DTcms.Core v8");
        //访问路径为 根域名/swagger/index.html
        c.RoutePrefix = "swagger";
    });
}
//使用静态文件
app.UseStaticFiles();
//使用路由
app.UseRouting();
//使用跨域
app.UseCors("cors");
//启用认证
app.UseAuthentication();
//授权服务
app.UseAuthorization();
//映射路由
app.MapControllers();

app.Run();
