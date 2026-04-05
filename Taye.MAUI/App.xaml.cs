namespace Taye.MAUI;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public const string ApiBaseUrl = "http://kind-knowledge.top:40000";


    public App(IServiceProvider services)
    {
        InitializeComponent();

        Preferences.Set("ApiBaseUrl", ApiBaseUrl);

        _services = services;

        // 使用依赖注入获取 MainPage
        var mainPage = _services.GetRequiredService<MainPage>();
        MainPage = new NavigationPage(mainPage);
    }
}
