using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Taye.WebAPI.Data;
using Taye.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
            b =>
            {
                b.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
});

builder.Services.AddControllers().AddJsonOptions(options =>
    {
        // 配置 JSON 序列化
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddOpenApi();

// 注册数据库上下文
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    // 改为 PostgreSQL
    options.UseNpgsql(connectionString);

    // 开发环境显示 SQL 语句
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// 添加这个配置：全局设置 DateTime 为 UTC
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<ILevelConfigService, LevelConfigService>();
builder.Services.AddScoped<IReasonTemplateService, ReasonTemplateService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IAchievementService, AchievementService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//// 配置静态文件服务
app.UseStaticFiles(); // 这会让 wwwroot 目录下的文件可访问
app.MapScalarApiReference();
app.UseCors();
app.UseHttpsRedirection();
app.MapFallbackToFile("index.html");

app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();
