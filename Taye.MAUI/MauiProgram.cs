using Microsoft.Extensions.Logging;
using Refit;
using Taye.MAUI.Converters;
using Taye.MAUI.Services;
using Taye.MAUI.ViewModels;
using UraniumUI;

namespace Taye.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // 修复：取消注释这一行，这是必须的
        builder
            .UseMauiApp<App>()
            .UseUraniumUI()
            .UseUraniumUIMaterial()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        //// 配置 API 地址
        //var apiBaseUrl = DeviceInfo.Platform == DevicePlatform.Android
        //    ? "http://10.0.2.2:5000"
        //    : "https://localhost:5001";

        //builder.Services.AddSingleton(new ApiConfig { BaseUrl = apiBaseUrl });

        // 注册 API 服务
        builder.Services.AddSingleton<IApiService, ApiService>();

        // 注册转换器
        builder.Services.AddSingleton<TypeColorConverter>();
        builder.Services.AddSingleton<StarColorConverter>();
        builder.Services.AddSingleton<InverseBoolConverter>();
        builder.Services.AddSingleton<IsNotNullConverter>();

        // 注册 ViewModel
        builder.Services.AddTransient<StarRecordViewModel>();

        // 注册 MainPage
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}

public class ApiConfig
{
    public string BaseUrl { get; set; } = string.Empty;
}
