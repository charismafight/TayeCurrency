using Taye.MAUI.ViewModels;

namespace Taye.MAUI;

public partial class MainPage : ContentPage
{
    public MainPage(StarRecordViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        // 绑定筛选器事件
        FilterPicker.SelectedIndexChanged += OnFilterChanged;
    }

    private void OnFilterChanged(object sender, EventArgs e)
    {
        if (BindingContext is StarRecordViewModel vm)
        {
            var selected = FilterPicker.SelectedItem as string;
            vm.FilterType = selected switch
            {
                "获得" => "Gain",
                "消费" => "Spend",
                _ => null
            };
            _ = vm.LoadData();
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is StarRecordViewModel vm)
        {
            //await vm.LoadData();
        }
    }
}
