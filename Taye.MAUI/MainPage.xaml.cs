using Taye.MAUI.ViewModels;

namespace Taye.MAUI;

public partial class MainPage : ContentPage
{
    public MainPage(StarRecordViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void OnFilterChanged(object sender, EventArgs e)
    {
        if (BindingContext is StarRecordViewModel vm)
        {
            vm.FilterRecordsCommand.Execute(null);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is StarRecordViewModel vm)
        {
            await vm.LoadData();
        }
    }
}
