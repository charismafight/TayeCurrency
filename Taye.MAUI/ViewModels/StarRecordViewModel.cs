using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using Taye.MAUI.Services;
using Taye.Shared.DTOs;

namespace Taye.MAUI.ViewModels;

public partial class StarRecordViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private int _currentPage = 1;
    private const int PageSize = 20;
    private bool _hasMoreData = true;
    private int _totalCount = 0;

    [ObservableProperty]
    private ObservableCollection<StarRecordDto> records = new();

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

    [ObservableProperty]
    private Dictionary<string, int> currentTemplates = new();

    [ObservableProperty]
    private ObservableCollection<string> quickReasons = new();

    [ObservableProperty]
    private Dictionary<string, int> reasonStarMap = new();

    [ObservableProperty]
    private StarRecordDto? selectedRecord;

    [ObservableProperty]
    private string? selectedRecordImage;

    public StarRecordViewModel(IApiService apiService)
    {
        _apiService = apiService;
        SelectedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // 先设置 Type，然后手动调用加载
        Type = "奖励";
        // 直接调用异步方法（不使用 await，避免阻塞）
        Task.Run(async () => await LoadReasonsByType(Type));
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
        var result = await _apiService.GetRecordsPagedAsync(StartDate, EndDate.AddDays(1), typeValue, _currentPage, PageSize);

        if (result?.Items == null) return;

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            if (_currentPage == 1)
            {
                // 第一页，清空重新加载
                Records.Clear();
            }

            // 逐条添加，这样 ObservableCollection 会通知 UI 每一行的插入
            foreach (var item in result.Items.OrderByDescending(p => p.Date))
            {
                Records.Add(item);
            }
        });

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
            await Application.Current.MainPage.DisplayAlert("提示", "请输入原因", "确定");
            return;
        }

        if (StarCount < -1000 || StarCount > 1000)
        {
            await Application.Current.MainPage.DisplayAlert("提示", "星数范围必须在 -1000 到 +1000 之间", "确定");
            return;
        }

        // 添加照片校验
        if (SelectedImage == null)
        {
            await Application.Current.MainPage.DisplayAlert("提示", "请上传照片", "确定");
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
                await Application.Current.MainPage.DisplayAlert("成功", "记录已保存", "确定");

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
                await Application.Current.MainPage.DisplayAlert("失败", "保存失败，请重试", "确定");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("错误", $"保存失败: {ex.Message}", "确定");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task DeleteRecord(StarRecordDto record)
    {
        var confirm = await Application.Current.MainPage.DisplayAlert(
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
                    await Application.Current.MainPage.DisplayAlert("成功", "已删除", "确定");
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
            await Application.Current.MainPage.DisplayAlert("错误", $"选择图片失败: {ex.Message}", "确定");
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
                await Application.Current.MainPage.DisplayAlert("权限不足", "需要相机权限才能拍照", "确定");
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
            await Application.Current.MainPage.DisplayAlert("错误", $"拍照失败: {ex.Message}", "确定");
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
    }

    [RelayCommand]
    public async Task AddRecord()
    {

    }

    partial void OnTypeChanged(string value)
    {
        // 当类型改变时，重新加载对应的模板
        LoadReasonsByType(value);
        StarCount = 0;
    }

    private async Task LoadReasonsByType(string type)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"开始加载 {type} 类型的原因列表");

            // 先清空
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                QuickReasons.Clear();
                ReasonStarMap.Clear();
            });

            var response = await _apiService.GetReasonTemplatesAsync(type);
            if (response.Success && response.Data != null)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    ReasonStarMap = response.Data;
                    // 逐个添加，确保 ObservableCollection 触发 CollectionChanged 事件
                    foreach (var key in ReasonStarMap.Keys)
                    {
                        QuickReasons.Add(key);
                    }
                    System.Diagnostics.Debug.WriteLine($"加载完成，共 {QuickReasons.Count} 个原因");
                });
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"API 返回失败: {response.Message}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"加载原因列表失败: {ex.Message}");
        }
    }

    // 当选择原因时，自动填充星数
    partial void OnReasonChanged(string value)
    {
        if (!string.IsNullOrEmpty(value) && ReasonStarMap.ContainsKey(value))
        {
            StarCount = ReasonStarMap[value];
        }
    }

    [RelayCommand]
    public void SelectQuickReason(string reason)
    {
        if (!string.IsNullOrEmpty(reason) && CurrentTemplates.ContainsKey(reason))
        {
            Reason = reason;
            StarCount = CurrentTemplates[reason];
        }
    }

    partial void OnSelectedRecordChanged(StarRecordDto? value)
    {
        if (value != null && !string.IsNullOrEmpty(value.ImagePath))
        {
            System.Diagnostics.Debug.WriteLine($"选中记录: {value?.Id}, 原因: {value?.Reason}, ImagePath: {value?.ImagePath}");
            // 构建图片完整URL
            SelectedRecordImage = $"{App.ApiBaseUrl}/{value.ImagePath}";
        }
        else
        {
            SelectedRecordImage = null;
        }
    }

    [RelayCommand]
    private async Task ViewImage()
    {
        if (!string.IsNullOrEmpty(SelectedRecordImage))
        {
            // 创建图片预览页面
            var imagePage = new ContentPage
            {
                Title = "图片预览",
                Content = new Grid
                {
                    Children =
                {
                    new Image
                    {
                        Source = SelectedRecordImage,
                        Aspect = Aspect.AspectFit,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    }
                }
                }
            };

            // 添加关闭按钮
            var closeButton = new Button
            {
                Text = "关闭",
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start,
                Margin = new Thickness(10),
                BackgroundColor = Colors.Black,
                TextColor = Colors.White,
                CornerRadius = 20,
                WidthRequest = 60,
                HeightRequest = 40
            };
            closeButton.Clicked += async (s, e) => await Application.Current.MainPage.Navigation.PopModalAsync();

            (imagePage.Content as Grid).Children.Add(closeButton);

            // 使用模态窗口显示
            await Application.Current.MainPage.Navigation.PushModalAsync(imagePage);
        }
    }
}
