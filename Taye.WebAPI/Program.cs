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
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddOpenApi();

// 注册数据库上下文（使用 SQLite）
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlite(connectionString);

    // 开发环境显示 SQL 语句
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

builder.Services.AddScoped<IFileUploadService, FileUploadService>();

builder.Services.AddSingleton<IReasonTemplateService, ReasonTemplateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
