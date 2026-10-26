using TicTacToe.Models;
using TicTacToe.Services;

namespace TicTacToe.Views;

public partial class GameModePage : ContentPage
{
    private readonly Action<GameTheme> _themeChangedHandler;

    public GameModePage()
    {
        InitializeComponent();
        _themeChangedHandler = (t) => MainThread.BeginInvokeOnMainThread(ApplyTheme);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ThemeService.OnThemeChanged += _themeChangedHandler;
        ApplyTheme();
        UpdateSelectionUI();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ThemeService.OnThemeChanged -= _themeChangedHandler;
    }

    private void ApplyTheme()
    {
        var theme = ThemeService.CurrentTheme;
        ThemeService.ApplyToPage(this, ModeHeaderBorder, HdrShadow);

        if (BtnStartGameBorder != null)
        {
            BtnStartGameBorder.BackgroundColor = theme.ButtonBackground;
        }
        if (BtnStartGameShadow != null)
        {
            BtnStartGameShadow.BackgroundColor = theme.ButtonShadowColor;
        }
        if (LblStartGame != null)
        {
            LblStartGame.TextColor = theme.ButtonTextColor;
        }

        UpdateSelectionUI();
    }

    private void UpdateSelectionUI()
    {
        bool isBot = AppSettingsService.IsBotMode;
        string diff = AppSettingsService.BotDifficulty;

        int selectedIndex = 0; // 2 Mängijat
        if (isBot)
        {
            if (diff.StartsWith("Lihtne", StringComparison.OrdinalIgnoreCase))
                selectedIndex = 1;
            else if (diff.StartsWith("Meister", StringComparison.OrdinalIgnoreCase))
                selectedIndex = 3;
            else
                selectedIndex = 2; // Nutikas
        }

        var cards = new[] { CardModeTwoPlayers, CardModeBotEasy, CardModeBotSmart, CardModeBotMaster };
        var checks = new[] { CheckTwoPlayers, CheckBotEasy, CheckBotSmart, CheckBotMaster };
        var titles = new[] { LblTitleTwoPlayers, LblTitleBotEasy, LblTitleBotSmart, LblTitleBotMaster };
        var theme = ThemeService.CurrentTheme;

        for (int i = 0; i < cards.Length; i++)
        {
            bool isCurrent = i == selectedIndex;
            if (cards[i] != null)
            {
                cards[i].Stroke = isCurrent ? theme.HeaderSurfaceColor : (theme.IsDark ? Color.FromArgb("#2B383E") : Color.FromArgb("#E5E5E5"));
                cards[i].StrokeThickness = isCurrent ? 3 : 2;
                cards[i].BackgroundColor = isCurrent 
                    ? (theme.IsDark ? Color.FromArgb("#1B272C") : Color.FromArgb("#F0F9FF")) 
                    : (theme.IsDark ? Color.FromArgb("#172227") : Color.FromArgb("#F7F7F7"));
            }

            if (checks[i] != null)
            {
                checks[i].Text = isCurrent ? "✓" : "";
                checks[i].TextColor = theme.HeaderSurfaceColor;
            }

            if (titles[i] != null)
            {
                titles[i].TextColor = theme.IsDark ? Colors.White : Color.FromArgb("#3C3C3C");
            }
        }
    }

    private void OnSelectTwoPlayersClicked(object? sender, EventArgs e)
    {
        AppSettingsService.IsBotMode = false;
        UpdateSelectionUI();
    }

    private void OnSelectBotEasyClicked(object? sender, EventArgs e)
    {
        AppSettingsService.IsBotMode = true;
        AppSettingsService.BotDifficulty = "Lihtne (Juhuslikud käigud)";
        UpdateSelectionUI();
    }

    private void OnSelectBotSmartClicked(object? sender, EventArgs e)
    {
        AppSettingsService.IsBotMode = true;
        AppSettingsService.BotDifficulty = "Nutikas (Tavaline - Blokeerib)";
        UpdateSelectionUI();
    }

    private void OnSelectBotMasterClicked(object? sender, EventArgs e)
    {
        AppSettingsService.IsBotMode = true;
        AppSettingsService.BotDifficulty = "Meister (Võitmatu taktika)";
        UpdateSelectionUI();
    }

    private async void OnStartGameClicked(object? sender, EventArgs e)
    {
        if (BtnStartGameBorder != null)
        {
            await BtnStartGameBorder.TranslateToAsync(0, 4, 60, Easing.CubicIn);
            await BtnStartGameBorder.TranslateToAsync(0, 0, 60, Easing.CubicOut);
        }

        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
        else
        {
            await Navigation.PopToRootAsync();
        }
    }
}

