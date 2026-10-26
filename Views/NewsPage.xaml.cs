using TicTacToe.Services;

namespace TicTacToe.Views;

public partial class NewsPage : ContentPage
{
    public NewsPage()
    {
        InitializeComponent();
        ThemeService.OnThemeChanged += (t) => MainThread.BeginInvokeOnMainThread(ApplyCurrentTheme);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ApplyCurrentTheme();
    }

    private void ApplyCurrentTheme()
    {
        ThemeService.ApplyToPage(this, HdrSurface, HdrShadow, BtnBackBorder, IconNotebookBorder);

        var theme = ThemeService.CurrentTheme;
        bool isDark = theme.IsDark;
        if (HedgehogBannerCard != null)
        {
            HedgehogBannerCard.BackgroundColor = isDark ? Color.FromArgb("#1B272C") : Colors.White;
            HedgehogBannerCard.Stroke = isDark ? Color.FromArgb("#2B383E") : Color.FromArgb("#E5E5E5");
        }
        if (HedgehogAvatarBorder != null)
        {
            HedgehogAvatarBorder.BackgroundColor = isDark ? Color.FromArgb("#2A373E") : Color.FromArgb("#F0FDF4");
            HedgehogAvatarBorder.Stroke = isDark ? Color.FromArgb("#374850") : Color.FromArgb("#DCFCE7");
        }
        if (LblHedgehogSpeech != null)
        {
            LblHedgehogSpeech.TextColor = isDark ? Colors.White : Color.FromArgb("#4B4B4B");
        }
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
        else
        {
            await (Shell.Current?.Navigation ?? Navigation).PopToRootAsync();
        }
    }
}

