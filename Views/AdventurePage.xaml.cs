using TicTacToe.Models;
using TicTacToe.Services;

namespace TicTacToe.Views;

public partial class AdventurePage : ContentPage
{
    public AdventurePage()
    {
        InitializeComponent();
        AdventurePuzzleService.OnAdventureProgressChanged += () => MainThread.BeginInvokeOnMainThread(RefreshProgressUI);
        ShopService.OnShopUpdated += () => MainThread.BeginInvokeOnMainThread(RefreshProgressUI);
        ThemeService.OnThemeChanged += (t) => MainThread.BeginInvokeOnMainThread(ApplyCurrentTheme);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ApplyCurrentTheme();
        RefreshProgressUI();
        InitialScrollToActiveLevel();
    }

    private void ApplyCurrentTheme()
    {
        var theme = ThemeService.CurrentTheme;

        // Применяем стандартную шапку
        ThemeService.ApplyToPage(this, HdrSurface, HdrShadow, BtnBackBorder, CoinsPillBorder);
        ThisPage.BackgroundColor = theme.PageBackground;

        bool isDark = theme.IsDark;

        // Баннер биома
        BannerCard.BackgroundColor = isDark ? Color.FromArgb("#1B272C") : Colors.White;
        BannerCard.Stroke = isDark ? Color.FromArgb("#2B383E") : theme.CardBorderColor;
        LblBiomeTitle.TextColor = isDark ? Colors.White : theme.PrimaryTextColor;
        ProgressChapter.ProgressColor = theme.ButtonBackground;
        LblChapterProgressText.TextColor = theme.ButtonBackground;

        // Соединяющая тропа
        Color trailColor;
        if (isDark)
        {
            trailColor = Color.FromArgb("#2B383E");
        }
        else if (theme.Name.Equals("Gold", StringComparison.OrdinalIgnoreCase))
        {
            trailColor = Color.FromArgb("#F4E5B8");
        }
        else if (theme.Name.Equals("Green", StringComparison.OrdinalIgnoreCase))
        {
            trailColor = Color.FromArgb("#DCFCE7");
        }
        else
        {
            trailColor = Color.FromArgb("#BEE3F8");
        }
        TrailPath.Stroke = trailColor;

        // Плашка уведомления о закрытом уровне / главе
        if (BottomSheetCard != null)
        {
            BottomSheetCard.BackgroundColor = isDark ? Color.FromArgb("#1B272C") : Colors.White;
            BottomSheetCard.Stroke = isDark ? Color.FromArgb("#2B383E") : Color.FromArgb("#E5E5E5");
        }
        if (SheetTitle != null)
        {
            SheetTitle.TextColor = isDark ? Colors.White : Color.FromArgb("#2B2B2B");
        }
        if (SheetMessage != null)
        {
            SheetMessage.TextColor = isDark ? Color.FromArgb("#98A2B3") : Color.FromArgb("#6B7280");
        }
        if (SheetIconBorder != null)
        {
            SheetIconBorder.BackgroundColor = isDark ? Color.FromArgb("#2A373E") : Color.FromArgb("#FEF3C7");
            SheetIconBorder.Stroke = isDark ? Color.FromArgb("#374850") : Color.FromArgb("#FCD34D");
        }
        if (SheetButtonSurface != null)
        {
            SheetButtonSurface.BackgroundColor = theme.ButtonBackground;
        }
        if (SheetButtonBase != null)
        {
            SheetButtonBase.BackgroundColor = theme.ButtonShadowColor;
        }
        if (Chapter2Card != null)
        {
            Chapter2Card.BackgroundColor = isDark ? Color.FromArgb("#1B272C") : Colors.White;
            Chapter2Card.Stroke = isDark ? Color.FromArgb("#2B383E") : theme.CardBorderColor;
            LblChapter2Title.TextColor = isDark ? Color.FromArgb("#CBD5E1") : Color.FromArgb("#475569");
        }
    }

    private void RefreshProgressUI()
    {
        // Баланс монет
        LblCoinBalance.Text = ShopService.Coins.ToString();

        // Прогресс главы
        int completed = AdventurePuzzleService.CompletedLevelsCount;
        ProgressChapter.Progress = completed / 5.0;
        LblChapterProgressText.Text = $"{completed} / 5";

        // Обновляем узлы 1..5 (чистые 3D-кнопки без иконок)
        UpdateNodeUI(1, BaseNode1, ShadowNode1);
        UpdateNodeUI(2, BaseNode2, ShadowNode2);
        UpdateNodeUI(3, BaseNode3, ShadowNode3);
        UpdateNodeUI(4, BaseNode4, ShadowNode4);
        UpdateNodeUI(5, BaseNode5, ShadowNode5);
    }

    private void UpdateNodeUI(int levelId, Border baseNode, Border shadowNode)
    {
        var theme = ThemeService.CurrentTheme;
        bool isCompleted = AdventurePuzzleService.IsLevelCompleted(levelId);
        bool isUnlocked = AdventurePuzzleService.IsLevelUnlocked(levelId);

        if (isCompleted)
        {
            // Пройден -> Золотой узел Duolingo
            baseNode.BackgroundColor = Color.FromArgb("#FFC800");
            shadowNode.BackgroundColor = Color.FromArgb("#D97706");
            baseNode.Opacity = 1.0;
        }
        else if (isUnlocked)
        {
            // Текущий открытый уровень -> В цвете выбранной темы
            baseNode.BackgroundColor = theme.ButtonBackground;
            shadowNode.BackgroundColor = theme.ButtonShadowColor;
            baseNode.Opacity = 1.0;
        }
        else
        {
            // Заблокирован -> Приглушенный графит или серый камень
            bool isDark = theme.IsDark;
            baseNode.BackgroundColor = isDark ? Color.FromArgb("#2A373E") : Color.FromArgb("#E2E8F0");
            shadowNode.BackgroundColor = isDark ? Color.FromArgb("#1B2529") : Color.FromArgb("#CBD5E1");
            baseNode.Opacity = 0.8;
        }
    }

    private async void InitialScrollToActiveLevel()
    {
        await Task.Delay(250);
        await ScrollToActiveLevelAsync();
    }

    private async Task ScrollToActiveLevelAsync()
    {
        int highest = AdventurePuzzleService.HighestUnlockedLevel;

        // Вычисляем целевой Y в зависимости от активного уровня (сверху вниз: 1 сверху, 5 снизу)
        double targetY = highest switch
        {
            1 => 0,
            2 => 80,
            3 => 220,
            4 => 330,
            5 => 460,
            _ => 0
        };

        await AdventureScroll.ScrollToAsync(0, targetY, true);
    }

    private async Task OpenLevelAsync(int levelId, Grid container)
    {
        if (!AdventurePuzzleService.IsLevelUnlocked(levelId))
        {
            // Тряска заблокированного узла
            await container.TranslateToAsync(-7, 0, 40);
            await container.TranslateToAsync(7, 0, 40);
            await container.TranslateToAsync(-5, 0, 40);
            await container.TranslateToAsync(5, 0, 40);
            await container.TranslateToAsync(0, 0, 40);

            await ShowLockedBottomSheetAsync(
                "Tase on lukus",
                $"Lahenda esmalt eelmine mõistatus (tase {levelId - 1}), et see tase avada ja teekonda jätkata!");
            return;
        }

        // Тактильное вдавливание 3D-кнопки
        await container.TranslateToAsync(0, 4, 60, Easing.CubicIn);
        await container.TranslateToAsync(0, 0, 60, Easing.CubicOut);

        await Navigation.PushAsync(new AdventurePuzzlePage(levelId));
    }

    private async void OnChapter2Clicked(object? sender, EventArgs e)
    {
        if (Chapter2Card != null)
        {
            await Chapter2Card.TranslateToAsync(-6, 0, 40);
            await Chapter2Card.TranslateToAsync(6, 0, 40);
            await Chapter2Card.TranslateToAsync(-4, 0, 40);
            await Chapter2Card.TranslateToAsync(4, 0, 40);
            await Chapter2Card.TranslateToAsync(0, 0, 40);
        }

        await ShowLockedBottomSheetAsync(
            "Peatükk on lukus",
            "2. peatükk avaneb pärast 1. peatüki («Tarkuse Mets») kõigi 5 taseme edukat läbimist!");
    }

    private async Task ShowLockedBottomSheetAsync(string title, string message)
    {
        SheetTitle.Text = title;
        SheetMessage.Text = message;

        ApplyCurrentTheme();

        try
        {
            BottomSheetOverlay.Opacity = 0;
            BottomSheetOverlay.IsVisible = true;
            BottomSheetCard.TranslationY = 320;

            await Task.WhenAll(
                BottomSheetOverlay.FadeToAsync(1, 180, Easing.CubicOut),
                BottomSheetCard.TranslateToAsync(0, 0, 240, Easing.CubicOut)
            );
        }
        catch
        {
            // Ignore animation cancellation
        }
    }

    private async Task AnimateCloseBottomSheetAsync()
    {
        try
        {
            await Task.WhenAll(
                BottomSheetOverlay.FadeToAsync(0, 160, Easing.CubicIn),
                BottomSheetCard.TranslateToAsync(0, 320, 180, Easing.CubicIn)
            );
            BottomSheetOverlay.IsVisible = false;
        }
        catch
        {
            BottomSheetOverlay.IsVisible = false;
        }
    }

    private async void OnBottomSheetScrimTapped(object? sender, EventArgs e)
    {
        await AnimateCloseBottomSheetAsync();
    }

    private async void OnCloseSheetClicked(object? sender, EventArgs e)
    {
        if (SheetButtonSurface != null)
        {
            await SheetButtonSurface.TranslateToAsync(0, 4, 60, Easing.CubicIn);
            await SheetButtonSurface.TranslateToAsync(0, 0, 70, Easing.SpringOut);
        }
        await AnimateCloseBottomSheetAsync();
    }

    private async void OnLevel1Clicked(object? sender, EventArgs e) => await OpenLevelAsync(1, Node1Container);
    private async void OnLevel2Clicked(object? sender, EventArgs e) => await OpenLevelAsync(2, Node2Container);
    private async void OnLevel3Clicked(object? sender, EventArgs e) => await OpenLevelAsync(3, Node3Container);
    private async void OnLevel4Clicked(object? sender, EventArgs e) => await OpenLevelAsync(4, Node4Container);
    private async void OnLevel5Clicked(object? sender, EventArgs e) => await OpenLevelAsync(5, Node5Container);

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

