namespace Taye.MAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // 注册主页面路由
        Routing.RegisterRoute("MainPage", typeof(MainPage));
    }
}
