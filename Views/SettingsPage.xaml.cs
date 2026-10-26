using TicTacToe.Models;
using TicTacToe.Resources.Localization;
using TicTacToe.Services;

namespace TicTacToe.Views;

public partial class SettingsPage : ContentPage
{
    private readonly Action _languageChangedHandler;
    private readonly Action _musicStateChangedHandler;
    private readonly Action _soundStateChangedHandler;

    public SettingsPage()
    {
        InitializeComponent();
        _languageChangedHandler = () => MainThread.BeginInvokeOnMainThread(OnLanguageRefreshed);
        _musicStateChangedHandler = () => MainThread.BeginInvokeOnMainThread(UpdateAudioUI);
        _soundStateChangedHandler = () => MainThread.BeginInvokeOnMainThread(UpdateAudioUI);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ThemeService.ApplyToPage(this, SettingsHeaderBorder, null, SettingsCloseBorder, SettingsGearBorder);
        LocalizationService.Instance.OnLanguageChanged += _languageChangedHandler;
        AudioService.OnMusicStateChanged += _musicStateChangedHandler;
        AudioService.OnSoundStateChanged += _soundStateChangedHandler;
        LoadSettingsUI();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        LocalizationService.Instance.OnLanguageChanged -= _languageChangedHandler;
        AudioService.OnMusicStateChanged -= _musicStateChangedHandler;
        AudioService.OnSoundStateChanged -= _soundStateChangedHandler;
    }

    private void OnLanguageRefreshed()
    {
        UpdateLanguageUI(LocalizationService.Instance.CurrentLanguage);
        UpdateBoardSizeUI(AppSettingsService.BoardSize);
        UpdateAudioUI();
    }

    private void LoadSettingsUI()
    {
        UpdateBoardSizeUI(AppSettingsService.BoardSize);
        UpdateLanguageUI(LocalizationService.Instance.CurrentLanguage);
        UpdateAudioUI();
    }

    private void OnLangEtClicked(object? sender, EventArgs e) => SelectLanguage("et");
    private void OnLangEnClicked(object? sender, EventArgs e) => SelectLanguage("en");
    private void OnLangRuClicked(object? sender, EventArgs e) => SelectLanguage("ru");

    private void SelectLanguage(string lang)
    {
        LocalizationService.Instance.SetLanguage(lang);
        UpdateLanguageUI(lang);
    }

    private void UpdateLanguageUI(string lang)
    {
        ApplyLangButtonStyle(BorderLangEt, LblLangEt, lang == "et");
        ApplyLangButtonStyle(BorderLangEn, LblLangEn, lang == "en");
        ApplyLangButtonStyle(BorderLangRu, LblLangRu, lang == "ru");
    }

    private static void ApplyLangButtonStyle(Border border, Label label, bool isSelected)
    {
        var theme = ThemeService.CurrentTheme;
        if (isSelected)
        {
            border.BackgroundColor = theme.HeaderSurfaceColor;
            border.Stroke = theme.HeaderShadowColor;
            label.TextColor = Colors.White;
        }
        else
        {
            border.BackgroundColor = Color.FromArgb("#F7F7F7");
            border.Stroke = Color.FromArgb("#E5E5E5");
            label.TextColor = Color.FromArgb("#4B4B4B");
        }
    }

    private void OnBoardSize3Clicked(object? sender, EventArgs e) => SelectBoardSize(3);
    private void OnBoardSize4Clicked(object? sender, EventArgs e) => SelectBoardSize(4);
    private void OnBoardSize5Clicked(object? sender, EventArgs e) => SelectBoardSize(5);

    private void SelectBoardSize(int size)
    {
        AppSettingsService.BoardSize = size;
        UpdateBoardSizeUI(size);
    }

    private void UpdateBoardSizeUI(int size)
    {
        ApplyBoardButtonStyle(BorderBoard3, LblBoard3Title, size == 3);
        ApplyBoardButtonStyle(BorderBoard4, LblBoard4Title, size == 4);
        ApplyBoardButtonStyle(BorderBoard5, LblBoard5Title, size == 5);
    }

    private static void ApplyBoardButtonStyle(Border border, Label title, bool isSelected)
    {
        var theme = ThemeService.CurrentTheme;
        if (isSelected)
        {
            border.BackgroundColor = theme.HeaderSurfaceColor;
            border.Stroke = theme.HeaderShadowColor;
            title.TextColor = Colors.White;
        }
        else
        {
            border.BackgroundColor = Color.FromArgb("#F7F7F7");
            border.Stroke = Color.FromArgb("#E5E5E5");
            title.TextColor = Color.FromArgb("#4B4B4B");
        }
    }

    private void OnMusicPlayClicked(object? sender, EventArgs e)
    {
        AudioService.PlayTap();
        AudioService.IsMusicEnabled = true;
        _ = AudioService.PlayMusicAsync();
        UpdateAudioUI();
    }

    private void OnMusicPauseClicked(object? sender, EventArgs e)
    {
        AudioService.PlayTap();
        AudioService.IsMusicEnabled = false;
        AudioService.PauseMusic();
        UpdateAudioUI();
    }

    private void OnSoundOnClicked(object? sender, EventArgs e)
    {
        AudioService.IsSoundEnabled = true;
        AudioService.PlayTap();
        UpdateAudioUI();
    }

    private void OnSoundOffClicked(object? sender, EventArgs e)
    {
        AudioService.IsSoundEnabled = false;
        UpdateAudioUI();
    }

    private void UpdateAudioUI()
    {
        var theme = ThemeService.CurrentTheme;

        // Muusika nupud
        ApplyAudioButtonStyle(BorderMusicPlay, LblMusicPlay, AudioService.IsMusicEnabled, theme);
        ApplyAudioButtonStyle(BorderMusicPause, LblMusicPause, !AudioService.IsMusicEnabled, theme);

        // Heliefektide nupud
        ApplyAudioButtonStyle(BorderSoundOn, LblSoundOn, AudioService.IsSoundEnabled, theme);
        ApplyAudioButtonStyle(BorderSoundOff, LblSoundOff, !AudioService.IsSoundEnabled, theme);
    }

    private static void ApplyAudioButtonStyle(Border border, Label label, bool isSelected, GameTheme theme)
    {
        if (isSelected)
        {
            border.BackgroundColor = theme.HeaderSurfaceColor;
            border.Stroke = theme.HeaderShadowColor;
            label.TextColor = Colors.White;
        }
        else
        {
            border.BackgroundColor = Color.FromArgb("#F7F7F7");
            border.Stroke = Color.FromArgb("#E5E5E5");
            label.TextColor = Color.FromArgb("#4B4B4B");
        }
    }

    private async void OnResetGameClicked(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync(
            AppResources.ResetGameConfirmTitle,
            AppResources.ResetGameConfirmMsg,
            AppResources.ResetYes,
            AppResources.Cancel);

        if (confirm)
        {
            Preferences.Default.Clear();

            // Taasta vaikesätted
            AppSettingsService.BoardSize = 3;
            AppSettingsService.IsBotMode = false;
            AppSettingsService.BotDifficulty = "Nutikas (Tavaline)";
            AppSettingsService.SoundEnabled = true;
            AppSettingsService.VibrationEnabled = true;

            // Taasta profiil
            UserProfileService.PlayerName = "Mängija";
            UserProfileService.PlayerAvatar = "janes2.png";

            // Taasta statistika
            GameStatsService.ResetStats();

            // Taasta pood ja elud
            ShopService.Coins = 500;
            LivesService.FillLives();

            UpdateBoardSizeUI(3);
            UpdateLanguageUI("et");

            await DisplayAlertAsync(AppResources.ResetDoneTitle, AppResources.ResetDoneMsg, AppResources.Done);
        }
    }

    private async void OnCloseClicked(object? sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
        else if (Navigation.NavigationStack.Count > 1)
        {
            await Navigation.PopToRootAsync();
        }
    }
}
