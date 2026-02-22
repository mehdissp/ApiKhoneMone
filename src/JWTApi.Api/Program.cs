////using JwtApi.Api.Middleware;
////using JWTApi.Api.Middleware;
////using JWTApi.Application.Services;
////using JWTApi.Domain.Entities;
////using JWTApi.Domain.Interfaces;
////using JWTApi.Infrastructure.Data;
////using JWTApi.Infrastructure.Middleware;
////using JWTApi.Infrastructure.Repositories;
////using JWTApi.Infrastructure.Services;
////using Microsoft.AspNetCore.Authentication.JwtBearer;
////using Microsoft.EntityFrameworkCore;
////using Microsoft.IdentityModel.Tokens;
////using Microsoft.OpenApi.Models;
////using System.Text;

////var builder = WebApplication.CreateBuilder(args);

////// DB
////builder.Services.AddDbContext<AppDbContext>(opt =>
////    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

////// DI
////builder.Services.AddMemoryCache();
////builder.Services.AddScoped<IUserRepository, UserRepository>();
////builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
////builder.Services.AddScoped<ITodoStatus, TodoStatusRepository>();
////builder.Services.AddScoped<IUnitOfWork, UnitOfWorkRepository>();
////builder.Services.AddScoped<JwtService>();
////builder.Services.AddScoped<AuthService>();
////builder.Services.AddScoped<ProjectService>();
////builder.Services.AddScoped<TodoStatusService>();
////builder.Services.AddScoped<Microsoft.AspNetCore.Identity.IPasswordHasher<User>, Microsoft.AspNetCore.Identity.PasswordHasher<User>>();
////builder.Services.Configure<SecurityOptions>(builder.Configuration.GetSection("Security"));

////// register token blacklist (replace with Redis implementation in production)
////builder.Services.AddSingleton<ITokenBlacklist, InMemoryTokenBlacklist>();
////// JWT
////var jwt = builder.Configuration.GetSection("Jwt");
////builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
////    .AddJwtBearer(opt =>
////    {
////        opt.TokenValidationParameters = new TokenValidationParameters
////        {
////            ValidateIssuer = true,
////            ValidateAudience = true,
////            ValidateIssuerSigningKey = true,
////            ValidIssuer = jwt["Issuer"],
////            ValidAudience = jwt["Audience"],
////            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
////            ClockSkew=TimeSpan.Zero
////        };
////    });
////builder.Services.AddCors(options =>
////{
////    options.AddPolicy("AllowReactApp", policy =>
////    {
////        policy.WithOrigins("https://localhost:3000", "http://localhost:3000")
////              .AllowAnyHeader()
////              .AllowAnyMethod()
////              .AllowCredentials();
////    });
////});

////// Controllers & Swagger
////builder.Services.AddControllers();
////builder.Services.AddEndpointsApiExplorer();
////builder.Services.AddSwaggerGen(c =>
////{
////    c.SwaggerDoc("v1", new OpenApiInfo
////    {
////        Title = "JWT Auth API",
////        Version = "v1",
////        Description = "API for JWT Authentication with Refresh Token"
////    });
////});

////var app = builder.Build();

////// Enable Swagger for all environments (or keep only Development)
////app.UseSwagger();
////app.UseSwaggerUI(c =>
////{
////    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JWT Auth API V1");
////});

////app.UseApiSecurity();
////app.UseCors("AllowReactApp");
//////app.UseRateLimiter();
////// ثبت IMemoryCache

////app.UseHttpsRedirection();
////app.UseAuthentication();
////app.UseStaticFiles();
////app.UseAuthorization();
//////app.UseCustomRateLimiter();
////app.UseCustomExceptionHandler();
//////app.UseMiddleware<RateLimitMiddleware>();
//////app.UseMiddleware<SecurityMiddleware>();
////app.UseMiddleware<MenuPermissionMiddleware>();
////app.UseMiddleware<ExceptionHandlingMiddleware>();
////app.MapControllers();
////app.Run();

//using JwtApi.Api.Middleware;
//using JWTApi.Api.Middleware;
//using JWTApi.Application.Services;
//using JWTApi.Application.Services.Comments;
//using JWTApi.Application.Services.Menus;
//using JWTApi.Application.Services.Roles;
//using JWTApi.Domain.Entities;
//using JWTApi.Domain.Interfaces;
//using JWTApi.Domain.Interfaces.Comments;
//using JWTApi.Domain.Interfaces.Menus;
//using JWTApi.Domain.Interfaces.Roles;
//using JWTApi.Infrastructure.Data;
//using JWTApi.Infrastructure.Repositories;
//using JWTApi.Infrastructure.Repositories.Comments;
//using JWTApi.Infrastructure.Repositories.Menus;
//using JWTApi.Infrastructure.Repositories.Roles;
//using JWTApi.Infrastructure.Services;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using System.Text;
//using System.Text.Encodings.Web;
//using System.Text.Json;

//var builder = WebApplication.CreateBuilder(args);

//// DB
//builder.Services.AddDbContext<AppDbContext>(opt =>
//    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// CORS Configuration - اضافه کردن این قسمت در ابتدا
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowReactApp", policy =>
//    {
//        policy.WithOrigins("https://localhost:3000", "http://localhost:3000")
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials()
//              .WithExposedHeaders("X-Pagination", "Content-Disposition") // در صورت نیاز
//              .SetPreflightMaxAge(TimeSpan.FromMinutes(10)); // کش کردن preflight
//    });

//    // برای محیط production
//    options.AddPolicy("AllowProduction", policy =>
//    {
//        policy.WithOrigins("https://yourdomain.com", "https://www.yourdomain.com")
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials();
//    });
//});

//// DI
//builder.Services.AddMemoryCache();
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
//builder.Services.AddScoped<ITodoStatus, TodoStatusRepository>();
//builder.Services.AddScoped<IUnitOfWork, UnitOfWorkRepository>();
//builder.Services.AddScoped<IBaleRepository, BaleRepository>();
//builder.Services.AddScoped<ITodo, TodoRepository>();
//builder.Services.AddScoped<IComment, CommentRepository>();
//builder.Services.AddScoped<IRoleRespository, RoleRepository>();

//builder.Services.AddScoped<IMenuRepository, MenuRepository>();
//builder.Services.AddScoped<TodoService>();
//builder.Services.AddScoped<RoleService>();
//builder.Services.AddScoped<MenuService>();
//builder.Services.AddScoped<CommentService>();
//builder.Services.AddScoped<JwtService>();
//builder.Services.AddScoped<BaleService>();
//builder.Services.AddScoped<AuthService>();
//builder.Services.AddScoped<UserService>();
//builder.Services.AddScoped<ProjectService>();
//builder.Services.AddScoped<TodoStatusService>();
//builder.Services.AddScoped<Microsoft.AspNetCore.Identity.IPasswordHasher<User>, Microsoft.AspNetCore.Identity.PasswordHasher<User>>();
//builder.Services.Configure<SecurityOptions>(builder.Configuration.GetSection("Security"));

//// register token blacklist
//builder.Services.AddSingleton<ITokenBlacklist, InMemoryTokenBlacklist>();

//// JWT
//var jwt = builder.Configuration.GetSection("Jwt");
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(opt =>
//    {
//        opt.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = jwt["Issuer"],
//            ValidAudience = jwt["Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
//            ClockSkew = TimeSpan.Zero
//        };
//    });

//// Controllers & Swagger
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "JWT Auth API",
//        Version = "v1",
//        Description = "API for JWT Authentication with Refresh Token"
//    });
//});
//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
//        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping; // برای کاراکترهای خاص
//    });
//builder.Services.AddHttpClient("BaleClient", client =>
//{
//    client.BaseAddress = new Uri("https://tapi.bale.ai/");
//    client.DefaultRequestHeaders.Add("Accept", "application/json");
//    client.Timeout = TimeSpan.FromSeconds(30);
//});

//var app = builder.Build();

//// Middleware pipeline - ترتیب بسیار مهم است
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "JWT Auth API V1");
//    });
//}

//app.UseHttpsRedirection();

//// CORS باید قبل از Authentication و Authorization باشد
//app.UseCors("AllowReactApp"); // این خط باید دقیقاً اینجا باشد

//app.UseStaticFiles();
//app.UseAuthentication();
//app.UseAuthorization();

//// سایر middlewareهای سفارشی شما
//app.UseApiSecurity();
//app.UseCustomExceptionHandler();
//app.UseMiddleware<MenuPermissionMiddleware>();
//app.UseMiddleware<ExceptionHandlingMiddleware>();

//app.MapControllers();
//app.Run();

using JwtApi.Api.Middleware;
using JWTApi.Api.Middleware;
using JWTApi.Application.Services;
using JWTApi.Application.Services.Categories;
using JWTApi.Application.Services.Menus;
using JWTApi.Application.Services.RealEstates;
using JWTApi.Application.Services.Roles;

using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Categories;
using JWTApi.Domain.Interfaces.Menus;
using JWTApi.Domain.Interfaces.RealEstates;
using JWTApi.Domain.Interfaces.Roles;

using JWTApi.Domain.Interfaces.TokenBlacklist;
using JWTApi.Infrastructure.Data;
using JWTApi.Infrastructure.Repositories;
using JWTApi.Infrastructure.Repositories.Categories;
using JWTApi.Infrastructure.Repositories.Menus;
using JWTApi.Infrastructure.Repositories.RealEstates;
using JWTApi.Infrastructure.Repositories.Roles;

using JWTApi.Infrastructure.Repositories.TokenBlacklist;
using JWTApi.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configuration
ConfigureDatabase(builder);
ConfigureCors(builder);
ConfigureDependencies(builder);
ConfigureAuthentication(builder);
ConfigureControllers(builder);
ConfigureSwagger(builder);
ConfigureHttpClients(builder);
builder.Services.AddSwaggerGen(); // اختیاری - برای مستندسازی API

// تنظیم محدودیت حجم آپلود
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = 5 * 1024 * 1024; // 5MB
    options.MultipartBodyLengthLimit = 5 * 1024 * 1024; // 5MB
});
var app = builder.Build();

ConfigureMiddlewarePipeline(app);
ConfigureEndpoints(app);


// دیباگ مسیرها
var contentRoot = Directory.GetCurrentDirectory();
var wwwrootPath = Path.Combine(contentRoot, "wwwroot");
var uploadsPath = Path.Combine(wwwrootPath, "uploads");

Console.WriteLine("=== Path Debug Information ===");
Console.WriteLine($"Content Root: {contentRoot}");
Console.WriteLine($"WWWRoot Path: {wwwrootPath}");
Console.WriteLine($"Uploads Path: {uploadsPath}");
Console.WriteLine($"WWWRoot Exists: {Directory.Exists(wwwrootPath)}");
Console.WriteLine($"Uploads Exists: {Directory.Exists(uploadsPath)}");
// Configuration Methods
app.Run("https://localhost:7178");
static void ConfigureDatabase(WebApplicationBuilder builder)
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddScoped<IDbConnection>(sp =>
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        return new SqlConnection(connectionString);
    });
}

static void ConfigureCors(WebApplicationBuilder builder)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactApp", policy =>
        {
            policy.WithOrigins("https://localhost:3000", "http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()
                  .WithExposedHeaders("X-Pagination", "Content-Disposition")
                  .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
        });

        options.AddPolicy("AllowProduction", policy =>
        {
            policy.WithOrigins("https://yourdomain.com", "https://www.yourdomain.com")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
        options.AddPolicy("AllowAll", builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
    });
}

static void ConfigureDependencies(WebApplicationBuilder builder)
{
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    // Infrastructure
    builder.Services.AddMemoryCache();
    builder.Services.AddHttpContextAccessor();
    // Repositories
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<ICategoryRepository, CategoriesRepository>();

    builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

    builder.Services.AddScoped<IUnitOfWork, UnitOfWorkRepository>();
    builder.Services.AddScoped<IBaleRepository, BaleRepository>();

    builder.Services.AddScoped<IRoleRespository, RoleRepository>();
    builder.Services.AddScoped<IMenuRepository, MenuRepository>();
    builder.Services.AddScoped<ITokenBlacklistRepository, TokenBlacklistRepository>();

    builder.Services.AddScoped<IRealEstatesRepository, RealEstatesRepository>();

    builder.Services.AddScoped<RoleService>();
    builder.Services.AddScoped<CategoriesService>();
    builder.Services.AddScoped<MenuService>();

    builder.Services.AddScoped<JwtService>();
    builder.Services.AddScoped<BaleService>();
    builder.Services.AddScoped<AuthService>();
    builder.Services.AddScoped<UserService>();
    builder.Services.AddScoped<ProjectService>();
    builder.Services.AddScoped<RealEstatesService>();


    // Identity
    builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

    // Configuration
    builder.Services.Configure<SecurityOptions>(builder.Configuration.GetSection("Security"));

    // Token Management
    builder.Services.AddSingleton<ITokenBlacklist, InMemoryTokenBlacklist>();
}

static void ConfigureAuthentication(WebApplicationBuilder builder)
{
    var jwtSection = builder.Configuration.GetSection("Jwt");
    var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSection["Issuer"],
                ValidAudience = jwtSection["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };
        });
}

static void ConfigureControllers(WebApplicationBuilder builder)
{
    // اضافه کردن این تنظیمات
    builder.Services.Configure<FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = 104857600; // 100MB
        options.MemoryBufferThreshold = 1024 * 1024 * 100; // 100MB
        options.ValueLengthLimit = int.MaxValue;
        options.MultipartBoundaryLengthLimit = int.MaxValue;
    });
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        });
}

static void ConfigureSwagger(WebApplicationBuilder builder)
{
    // تنظیمات فایل‌های multipart
    builder.Services.Configure<FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = 104857600; // 100MB
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "JWT Auth API",
            Version = "v1",
            Description = "API for JWT Authentication with Refresh Token"
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    });
}

static void ConfigureHttpClients(WebApplicationBuilder builder)
{
    builder.Services.AddHttpClient("BaleClient", client =>
    {
        client.BaseAddress = new Uri("https://tapi.bale.ai/");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.Timeout = TimeSpan.FromSeconds(30);
    });
}

static void ConfigureMiddlewarePipeline(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "JWT Auth API V1");
        });
    }

    app.UseHttpsRedirection();
    // مهم: UseStaticFiles باید قبل از UseRouting باشد
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
        RequestPath = "",
        OnPrepareResponse = ctx =>
        {
            var path = ctx.File.PhysicalPath;
            // کش برای فایل‌های استاتیک
            ctx.Context.Response.Headers["Cache-Control"] = "public, max-age=3600"; // 1 hour
            Console.WriteLine($"Serving static file: {path}");
        }
    });

    // Security Middleware
    app.UseCors("AllowReactApp");
    app.UseMiddleware<TokenValidationMiddleware>();
    app.UseAuthentication();
    app.UseAuthorization();

    // Custom Middleware
    app.UseApiSecurity();
    app.UseCustomExceptionHandler();
    app.UseMiddleware<MenuPermissionMiddleware>();
    app.UseMiddleware<ExceptionHandlingMiddleware>();
}

static void ConfigureEndpoints(WebApplication app)
{
    app.MapControllers();
}
