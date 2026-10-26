using TicTacToe.Models;
using TicTacToe.Services;

namespace TicTacToe.Views;

public partial class StatsPage : ContentPage
{
    private readonly Action<GameTheme> _themeChangedHandler;

    public StatsPage()
    {
        InitializeComponent();
        _themeChangedHandler = (t) => MainThread.BeginInvokeOnMainThread(ApplyCurrentTheme);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ThemeService.OnThemeChanged += _themeChangedHandler;
        ApplyCurrentTheme();
        LoadStats();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ThemeService.OnThemeChanged -= _themeChangedHandler;
    }

    private void ApplyCurrentTheme()
    {
        ThemeService.ApplyToPage(this, HdrSurface, HdrShadow, StatsCloseBorder, StatsIconBorder);

        var theme = ThemeService.CurrentTheme;
        bool isDark = theme.IsDark;

        if (StatCardSurface != null)
        {
            StatCardSurface.BackgroundColor = isDark ? Color.FromArgb("#1B272C") : Colors.White;
            StatCardSurface.Stroke = isDark ? Color.FromArgb("#2B383E") : Color.FromArgb("#E5E5E5");
        }

        if (StatCardShadow != null)
        {
            StatCardShadow.BackgroundColor = isDark ? Color.FromArgb("#131B1E") : Color.FromArgb("#E5E5E5");
        }

        if (Divider1 != null)
        {
            Divider1.Color = isDark ? Color.FromArgb("#2B383E") : Color.FromArgb("#EFEFEF");
        }

        if (Divider2 != null)
        {
            Divider2.Color = isDark ? Color.FromArgb("#2B383E") : Color.FromArgb("#EFEFEF");
        }

        if (LblTotalGamesTitle != null)
        {
            LblTotalGamesTitle.TextColor = isDark ? Color.FromArgb("#9CA3AF") : Color.FromArgb("#777777");
        }

        if (LblP1WinsTitle != null)
        {
            LblP1WinsTitle.TextColor = isDark ? Color.FromArgb("#9CA3AF") : Color.FromArgb("#777777");
        }

        if (LblDrawsTitle != null)
        {
            LblDrawsTitle.TextColor = isDark ? Color.FromArgb("#9CA3AF") : Color.FromArgb("#777777");
        }

        if (LblTotalGames != null)
        {
            LblTotalGames.TextColor = isDark ? Colors.White : Color.FromArgb("#3C3C3C");
        }

        if (BtnResetBorder != null)
        {
            BtnResetBorder.BackgroundColor = isDark ? Color.FromArgb("#2C1E20") : Color.FromArgb("#FFF0F0");
            BtnResetBorder.Stroke = isDark ? Color.FromArgb("#4A2528") : Color.FromArgb("#FFD1D1");
        }
    }

    private void LoadStats()
    {
        int total = GameStatsService.TotalGames;
        int p1 = GameStatsService.Player1Wins;
        int draws = GameStatsService.Draws;

        LblTotalGames.Text = total.ToString();
        LblP1Wins.Text = p1.ToString();
        LblDraws.Text = draws.ToString();
    }

    private async void OnResetStatsClicked(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync("Kinnita", "Kas oled kindel, et soovid kogu statistika kustutada?", "Jah, kustuta", "Ei");
        if (confirm)
        {
            GameStatsService.ResetStats();
            LoadStats();
        }
    }

    private async void OnCloseClicked(object? sender, EventArgs e)
    {
        if (Navigation.NavigationStack.Count > 1)
        {
            await Navigation.PopToRootAsync();
        }
        else if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}

