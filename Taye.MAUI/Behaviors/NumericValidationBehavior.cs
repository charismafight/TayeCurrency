using System.Globalization;

namespace Taye.MAUI.Behaviors;

public class NumericValidationBehavior : Behavior<Entry>
{
    public static readonly BindableProperty MaxValueProperty =
        BindableProperty.Create(nameof(MaxValue), typeof(int), typeof(NumericValidationBehavior), int.MaxValue);

    public static readonly BindableProperty MinValueProperty =
        BindableProperty.Create(nameof(MinValue), typeof(int), typeof(NumericValidationBehavior), int.MinValue);

    public int MaxValue
    {
        get => (int)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    public int MinValue
    {
        get => (int)GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    protected override void OnAttachedTo(Entry entry)
    {
        entry.TextChanged += OnEntryTextChanged;
        base.OnAttachedTo(entry);
    }

    protected override void OnDetachingFrom(Entry entry)
    {
        entry.TextChanged -= OnEntryTextChanged;
        base.OnDetachingFrom(entry);
    }

    private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.NewTextValue))
        {
            ((Entry)sender).Text = "0";
            return;
        }

        if (int.TryParse(e.NewTextValue, out int value))
        {
            if (value < MinValue)
            {
                ((Entry)sender).Text = MinValue.ToString();
                Application.Current.MainPage.DisplayAlert("提示", $"数值不能小于 {MinValue}", "确定");
            }
            else if (value > MaxValue)
            {
                ((Entry)sender).Text = MaxValue.ToString();
                Application.Current.MainPage.DisplayAlert("提示", $"数值不能大于 {MaxValue}", "确定");
            }
        }
        else
        {
            // 如果不是有效数字，恢复之前的值
            ((Entry)sender).Text = e.OldTextValue ?? "0";
            Application.Current.MainPage.DisplayAlert("提示", "请输入有效的数字", "确定");
        }
    }
}
