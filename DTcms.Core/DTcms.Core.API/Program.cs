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

//�ϴ��ļ�����
builder.WebHost.ConfigureKestrel(options =>
{
    //HTTP �����е���������С�� Ĭ��Ϊ 8kb
    options.Limits.MaxRequestLineSize = int.MaxValue;
    //���󻺳���������С�� Ĭ��Ϊ 1M
    options.Limits.MaxRequestBufferSize = int.MaxValue;
    //�κ��������ĵ���������С�����ֽ�Ϊ��λ��,Ĭ�� 30,000,000 �ֽڣ���ԼΪ 28.6MB
    options.Limits.MaxRequestBodySize = int.MaxValue;//�������󳤶�

});
//���ý����ļ����ȵ����ֵ��
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

//ע��NLog��¼��־
builder.Logging.AddNLog("nlog.config");

//Autofacע��
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    //�Զ�ע���������AOP����
    Assembly service = Assembly.Load("DTcms.Core.Services");
    Assembly iservice = Assembly.Load("DTcms.Core.IServices");
    //������service��β�������Ͳ����ǳ����
    builder.RegisterAssemblyTypes(service, iservice)
        .Where(t => (t.FullName != null && t.FullName.EndsWith("Service")) && !t.IsAbstract)
        .AsImplementedInterfaces()
        .InstancePerDependency(); //��������
});

//���û��������
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

//����������ע��
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddSingleton(new Appsettings(builder.Environment.ContentRootPath));//�����ļ���д������ע��
builder.Services.AddSingleton(new FileHelper(builder.Environment.ContentRootPath));//�ļ�������ע��
builder.Services.AddTransient<IDbContextFactory, DbContextFactory>();//ע�����ݿ����ӷ���

//IdentityUser����
builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.AllowedUserNameCharacters = null; //��������ע��
    options.Password.RequiredLength = 6;//������С����
    options.Password.RequireNonAlphanumeric = false;//������������ĸ�ر�
    options.Password.RequireUppercase = false;//������������д�ر�
    options.Password.RequireLowercase = false;//����������Сд�ر�
});
builder.Services.AddDbContext<AppDbContext>(); //IdentityUser����������
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders(); //ʹ��IdentityUserģ��

//ע����Դ��Ȩ����Handler��ʵ��IAuthorizationRequirement�ӿڵ���
builder.Services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
//ע��Swagger����
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v8", new OpenApiInfo { Version = "v8.0", Title = $"DTcms.Core API�ĵ�" });
    //��ȡӦ�ó�������Ŀ¼(����,���ܹ���Ŀ¼Ӱ�죬������ô˷�����ȡ·��)
    string basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location) ?? String.Empty;
    //��Ŀ�����������õ�xml�ļ���
    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //���ÿ�����ע��
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
});

//��������Ӧ����
builder.Services.AddControllers(setupAction =>
{
    //Ĭ����ӦJSON��ʽ
    setupAction.ReturnHttpNotAcceptable = true;
})
    .AddNewtonsoftJson(opt =>
    {
        opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        opt.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    })
    .AddXmlDataContractSerializerFormatters()//����XML�������֧��
    .ConfigureApiBehaviorOptions(setupAction =>
    {
        //������֤ʧ����Ӧ
        setupAction.InvalidModelStateResponseFactory = context =>
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status422UnprocessableEntity,
            };
            //ȡ��һ��������Ϣ����
            var resultMessage = new ResponseMessage(ErrorCode.ParamError, problemDetails.Errors.Values, context.HttpContext);
            //����422״̬��
            return new UnprocessableEntityObjectResult(resultMessage)
            {
                ContentTypes = { "application/json" }
            };

        };
    });

//JWT��֤��������
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
            ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
            ClockSkew = TimeSpan.FromSeconds(30),

            ValidateAudience = true,//�Ƿ���֤Audience
                                    //��̬��֤�ķ�ʽ��ˢ��token����tokenǿ��ʧЧ
            AudienceValidator = (m, n, z) =>
            {
                return m != null && string.Equals(m.FirstOrDefault(), builder.Configuration["Authentication:Audience"]);
            },
            ValidateIssuer = true,//�Ƿ���֤Issuer
            ValidIssuer = builder.Configuration["Authentication:Issuer"],

            ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey
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

//��ֹ��ת����¼ҳ��ֱ�ӷ���401
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
});

//ɨ��Profile�ļ�
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//ȫ���쳣����
builder.Services.AddMvc(setupAction => {
    //ȫ���쳣����
    setupAction.Filters.Add(typeof(GlobalExceptionFilter));
    //ȫ���������
    setupAction.Filters.Add(typeof(GlobalRequestFilter));
});
//���ÿ��������������ļ�AllowedHosts�ڵ��ֵ
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", options =>
        options.WithOrigins(builder.Configuration["AllowedHosts"] ?? string.Empty)
        .AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
        .WithExposedHeaders("content-disposition", "token-expired", "x-pagination"));
});
//����Swagger�д�JWT����ͷ
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
    //ʹ��Swagger�м��
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint($"/swagger/v8/swagger.json", $"DTcms.Core v8");
        //����·��Ϊ ������/swagger/index.html
        c.RoutePrefix = "swagger";
    });
}
//ʹ�þ�̬�ļ�
app.UseStaticFiles();
//ʹ��·��
app.UseRouting();
//ʹ�ÿ���
app.UseCors("cors");
//������֤
app.UseAuthentication();
//��Ȩ����
app.UseAuthorization();
//ӳ��·��
app.MapControllers();

app.Run();
