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
    private int _currentPage = 1;
    private const int PageSize = 20;
    private bool _hasMoreData = true;
    private int _totalCount = 0;

    [ObservableProperty]
    private List<StarRecordDto> records = new();

    [ObservableProperty]
    private StarStatisticsDto? statistics;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isLoadingMore;  // 是否正在加载更多

    [ObservableProperty]
    private string? selectedDate;

    [ObservableProperty]
    private int starCount;

    [ObservableProperty]
    private string reason = string.Empty;

    [ObservableProperty]
    private string type = "奖励"; // 奖励、花费、惩罚

    [ObservableProperty]
    private string? notes;

    [ObservableProperty]
    private FileResult? selectedImage;

    [ObservableProperty]
    private string? imagePreview;

    // 筛选相关属性
    [ObservableProperty]
    private DateTime startDate = DateTime.Today.AddDays(-7);  // 默认最近7天

    [ObservableProperty]
    private DateTime endDate = DateTime.Today;

    [ObservableProperty]
    private string? selectedFilterType = "全部";  // 筛选类型：全部、奖励、花费、惩罚

    // 筛选类型选项
    public List<string> FilterTypes { get; } = new List<string>
    {
        "全部",
        "奖励",
        "花费",
        "惩罚"
    };

    public StarRecordViewModel(IApiService apiService)
    {
        _apiService = apiService;
        SelectedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    [RelayCommand]
    public async Task LoadData()
    {
        IsLoading = true;
        _currentPage = 1;
        _hasMoreData = true;
        _totalCount = 0;

        try
        {
            await LoadRecords();
            await LoadStatistics();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadRecords()
    {
        // 转换类型：中文转英文
        string? typeValue = null;
        if (SelectedFilterType != "全部")
        {
            typeValue = SelectedFilterType switch
            {
                "奖励" => "Gain",
                "花费" => "Spend",
                "惩罚" => "Punish",
                _ => null
            };
        }

        // 调用分页 API
        var result = await _apiService.GetRecordsPagedAsync(StartDate, EndDate, typeValue, _currentPage, PageSize);

        if (_currentPage == 1)
        {
            Records = result.Items;
        }
        else
        {
            var newList = new List<StarRecordDto>(Records);
            newList.AddRange(result.Items);
            Records = newList;
        }

        _hasMoreData = result.HasNextPage;
        _totalCount = result.TotalCount;
    }

    [RelayCommand]
    public async Task LoadMore()
    {
        if (IsLoadingMore || !_hasMoreData) return;

        IsLoadingMore = true;
        try
        {
            _currentPage++;
            await LoadRecords();
        }
        finally
        {
            IsLoadingMore = false;
        }
    }

    [RelayCommand]
    public async Task FilterRecords()
    {
        IsLoading = true;
        _currentPage = 1;
        _hasMoreData = true;
        try
        {
            await LoadRecords();
            await LoadStatistics();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadStatistics()
    {
        Statistics = await _apiService.GetStatisticsAsync();
    }

    [RelayCommand]
    public async Task SaveRecord()
    {
        if (string.IsNullOrWhiteSpace(Reason))
        {
            await Shell.Current.DisplayAlert("提示", "请输入原因", "确定");
            return;
        }

        if (StarCount < -1000 || StarCount > 1000)
        {
            await Shell.Current.DisplayAlert("提示", "星数范围必须在 -100 到 +100 之间", "确定");
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

            // 转换类型为英文（匹配后端枚举）
            var typeValue = Type switch
            {
                "奖励" => "Gain",
                "花费" => "Spend",
                "惩罚" => "Punish",
                _ => "Gain"
            };

            var success = await _apiService.CreateRecordAsync(
                DateTime.Parse(SelectedDate!),
                StarCount,
                Reason,
                typeValue,
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
                ImagePreview = null;

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
                ImagePreview = result.FullPath;
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
                ImagePreview = result.FullPath;
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
        ImagePreview = null;
    }

    [RelayCommand]
    public async Task EditRecord(StarRecordDto record)
    {
        await Shell.Current.GoToAsync("edit", new Dictionary<string, object>
        {
            ["record"] = record
        });
    }

    [RelayCommand]
    public async Task AddRecord()
    {
        await Shell.Current.GoToAsync("add");
    }
}
