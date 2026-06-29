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

    private bool _isFirstLoad = true;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_isFirstLoad && BindingContext is StarRecordViewModel vm)
        {
            _isFirstLoad = false;
            await vm.LoadData();
        }
    }
}
