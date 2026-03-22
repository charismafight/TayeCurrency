using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Taye.MAUI.Services;
using Taye.Shared.DTOs;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Media;

namespace Taye.MAUI.ViewModels;

public partial class StarRecordViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private List<StarRecordDto> records = new();

    [ObservableProperty]
    private StarStatisticsDto? statistics;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? selectedDate;

    [ObservableProperty]
    private int starCount;

    [ObservableProperty]
    private string reason = string.Empty;

    [ObservableProperty]
    private string type = "Gain"; // Gain 或 Spend

    [ObservableProperty]
    private string? notes;

    [ObservableProperty]
    private FileResult? selectedImage;

    [ObservableProperty]
    private string? imagePreview;  // 添加这个属性

    public StarRecordViewModel(IApiService apiService)
    {
        _apiService = apiService;
        SelectedDate = DateTime.Today.ToString("yyyy-MM-dd");
    }

    [RelayCommand]
    public async Task LoadData()
    {
        IsLoading = true;
        try
        {
            var startDate = DateTime.Today.AddDays(-30);
            var endDate = DateTime.Today;

            Records = await _apiService.GetRecordsAsync(startDate, endDate, FilterType);
            Statistics = await _apiService.GetStatisticsAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task SaveRecord()
    {
        if (string.IsNullOrWhiteSpace(Reason))
        {
            await Shell.Current.DisplayAlert("提示", "请输入原因", "确定");
            return;
        }

        if (StarCount <= 0)
        {
            await Shell.Current.DisplayAlert("提示", "请输入星星数量", "确定");
            return;
        }

        IsLoading = true;
        try
        {
            Stream? imageStream = null;
            string? imageFileName = null;

            if (SelectedImage != null)
            {
                imageStream = await SelectedImage.OpenReadAsync();
                imageFileName = SelectedImage.FileName;
            }

            var success = await _apiService.CreateRecordAsync(
                DateTime.Parse(SelectedDate!),
                StarCount,
                Reason,
                Type,
                Notes,
                imageStream,
                imageFileName);

            if (success)
            {
                await Shell.Current.DisplayAlert("成功", "记录已保存", "确定");

                // 清空表单
                StarCount = 0;
                Reason = string.Empty;
                Notes = string.Empty;
                SelectedImage = null;
                ImagePreview = null;  // 清空预览

                // 重新加载数据
                await LoadData();
            }
            else
            {
                await Shell.Current.DisplayAlert("失败", "保存失败，请重试", "确定");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("错误", $"保存失败: {ex.Message}", "确定");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task DeleteRecord(StarRecordDto record)
    {
        var confirm = await Shell.Current.DisplayAlert(
            "确认删除",
            $"确定要删除 {record.DisplayDate} 的 {record.DisplayStar} 吗？",
            "删除",
            "取消");

        if (confirm)
        {
            IsLoading = true;
            try
            {
                var success = await _apiService.DeleteRecordAsync(record.Id);
                if (success)
                {
                    await LoadData();
                    await Shell.Current.DisplayAlert("成功", "已删除", "确定");
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    [RelayCommand]
    public async Task PickImage()
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "选择图片"
            });

            if (result != null)
            {
                SelectedImage = result;
                ImagePreview = result.FullPath;  // 设置预览路径
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("错误", $"选择图片失败: {ex.Message}", "确定");
        }
    }

    [RelayCommand]
    public async Task TakePhoto()
    {
        try
        {
            // 请求相机权限
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                await Shell.Current.DisplayAlert("权限不足", "需要相机权限才能拍照", "确定");
                return;
            }

            var result = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = "拍照"
            });

            if (result != null)
            {
                SelectedImage = result;
                ImagePreview = result.FullPath;  // 设置预览路径
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("错误", $"拍照失败: {ex.Message}", "确定");
        }
    }

    [RelayCommand]
    public void ClearImage()
    {
        SelectedImage = null;
        ImagePreview = null;  // 清空预览
    }

    [ObservableProperty]
    private string? filterType;  // 筛选类型：Gain、Spend 或 null

    // 添加编辑命令
    [RelayCommand]
    public async Task EditRecord(StarRecordDto record)
    {
        // 跳转到编辑页面
        await Shell.Current.GoToAsync("edit", new Dictionary<string, object>
        {
            ["record"] = record
        });
    }

    // 添加记录命令
    [RelayCommand]
    public async Task AddRecord()
    {
        // 跳转到添加页面
        await Shell.Current.GoToAsync("add");
    }
}
