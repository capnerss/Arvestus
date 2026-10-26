using TicTacToe.Controls;
using TicTacToe.Services;

namespace TicTacToe.Views;

public partial class MoreMenuPage : ContentPage
{
    private bool _isClosing = false;

    public MoreMenuPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var theme = ThemeService.CurrentTheme;
        SheetContainer.BackgroundColor = theme.CardBackground;
        SheetContainer.Stroke = theme.CardBorderColor;
        if (LblMenuTitle != null)
        {
            LblMenuTitle.TextColor = theme.IsDark ? Colors.White : Color.FromArgb("#4B4B4B");
        }
        SheetContainer.TranslationY = 500;
        await SheetContainer.TranslateToAsync(0, 0, 250, Easing.CubicOut);
    }

    private async Task CloseModalAsync()
    {
        if (_isClosing) return;
        _isClosing = true;
        await SheetContainer.TranslateToAsync(0, 500, 180, Easing.CubicIn);
        await Navigation.PopModalAsync(false);
    }

    private async void OnBackdropTapped(object? sender, EventArgs e)
    {
        await CloseModalAsync();
    }

    private async void OnCloseTapped(object? sender, EventArgs e)
    {
        await CloseModalAsync();
    }

    private async Task AnimateAndNavigateAsync(Border button, Page targetPage)
    {
        if (_isClosing) return;
        _isClosing = true;

        // Тактильная анимация нажатия 3D кнопки
        await button.TranslateToAsync(0, 4, 60, Easing.CubicIn);
        await button.TranslateToAsync(0, 0, 60, Easing.CubicOut);

        // Закрываем модальное окно шторки
        await SheetContainer.TranslateToAsync(0, 500, 150, Easing.CubicIn);
        await Navigation.PopModalAsync(false);

        // Переходим на выбранную страницу
        var nav = Shell.Current?.Navigation ?? Application.Current?.Windows?.FirstOrDefault()?.Page?.Navigation;
        if (nav != null)
        {
            var stack = nav.NavigationStack.ToList();
            await nav.PushAsync(targetPage);

            // Если предыдущая страница была второстепенной (не главной), убираем её из стека
            for (int i = stack.Count - 1; i >= 1; i--)
            {
                try
                {
                    nav.RemovePage(stack[i]);
                }
                catch
                {
                    // Игнорируем ограничения платформы
                }
            }
        }
    }

    private async void OnRulesTapped(object? sender, EventArgs e)
    {
        await AnimateAndNavigateAsync(BtnRules, new RulesPage());
    }

    private async void OnThemesTapped(object? sender, EventArgs e)
    {
        if (_isClosing) return;
        _isClosing = true;

        await BtnThemes.TranslateToAsync(0, 4, 60, Easing.CubicIn);
        await BtnThemes.TranslateToAsync(0, 0, 60, Easing.CubicOut);

        // Закрываем шторку
        await SheetContainer.TranslateToAsync(0, 500, 150, Easing.CubicIn);
        await Navigation.PopModalAsync(false);

        // Возвращаемся на главную страницу, если находимся на другой
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
        else
        {
            var nav = Application.Current?.Windows?.FirstOrDefault()?.Page?.Navigation;
            if (nav != null)
            {
                await nav.PopToRootAsync();
            }
        }

        await Task.Delay(140);
        DuolingoFooter.TriggerThemePicker();
    }

    private async void OnProfileTapped(object? sender, EventArgs e)
    {
        await AnimateAndNavigateAsync(BtnProfile, new ProfilePage());
    }

    private async void OnStatsTapped(object? sender, EventArgs e)
    {
        await AnimateAndNavigateAsync(BtnStats, new StatsPage());
    }

    private async void OnSettingsTapped(object? sender, EventArgs e)
    {
        await AnimateAndNavigateAsync(BtnSettings, new SettingsPage());
    }

    private async void OnTasksTapped(object? sender, EventArgs e)
    {
        await AnimateAndNavigateAsync(BtnTasks, new TasksPage());
    }
}

