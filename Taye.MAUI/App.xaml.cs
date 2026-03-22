namespace Taye.MAUI;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;

        // 使用依赖注入获取 MainPage
        var mainPage = _services.GetRequiredService<MainPage>();
        MainPage = new NavigationPage(mainPage);
    }
}
