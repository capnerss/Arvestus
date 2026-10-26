using TicTacToe.Models;
using TicTacToe.Views;
using TicTacToe.Services;

namespace TicTacToe.Controls;

public partial class DuolingoFooter : ContentView
{
    public static readonly BindableProperty ActiveTabProperty =
        BindableProperty.Create(
            nameof(ActiveTab),
            typeof(string),
            typeof(DuolingoFooter),
            "Home",
            propertyChanged: OnActiveTabChanged);

    public string ActiveTab
    {
        get => (string)GetValue(ActiveTabProperty);
        set => SetValue(ActiveTabProperty, value);
    }

    public static event Action? OnThemePickerRequested;

    public static void TriggerThemePicker()
    {
        OnThemePickerRequested?.Invoke();
    }

    public DuolingoFooter()
    {
        InitializeComponent();
        ApplyFooterTheme(ThemeService.CurrentTheme);
        UpdateActiveTabUI(ActiveTab);
        ThemeService.OnThemeChanged += OnThemeChangedHandler;
    }

    private void OnThemeChangedHandler(GameTheme theme)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ApplyFooterTheme(theme);
            UpdateActiveTabUI(ActiveTab);
        });
    }

    private void ApplyFooterTheme(GameTheme theme)
    {
        if (FooterBorder != null)
        {
            FooterBorder.BackgroundColor = theme.FooterBackground;
            FooterBorder.Stroke = theme.FooterBorderColor;
        }
    }

    private static void OnActiveTabChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is DuolingoFooter footer && newValue is string tab)
        {
            footer.UpdateActiveTabUI(tab);
        }
    }

    private void UpdateActiveTabUI(string tab)
    {
        ResetTab(TabHome);
        ResetTab(TabAdventure);
        ResetTab(TabRecords);
        ResetTab(TabBot);
        ResetTab(TabNews);
        ResetTab(TabMore);

        switch (tab?.ToLowerInvariant())
        {
            case "home":
                HighlightTab(TabHome);
                break;
            case "adventure":
                HighlightTab(TabAdventure);
                break;
            case "records":
                HighlightTab(TabRecords);
                break;
            case "bot":
                HighlightTab(TabBot);
                break;
            case "news":
                HighlightTab(TabNews);
                break;
            case "more":
            case "rules":
            case "stats":
            case "profile":
            case "settings":
            case "themes":
                HighlightTab(TabMore);
                break;
        }
    }

    private static void HighlightTab(Border tab)
    {
        var theme = ThemeService.CurrentTheme;
        tab.BackgroundColor = theme.FooterActiveTabBg;
        tab.Stroke = theme.FooterActiveTabBorder;
        tab.StrokeThickness = 2;
    }

    private static void ResetTab(Border tab)
    {
        tab.BackgroundColor = Colors.Transparent;
        tab.StrokeThickness = 0;
    }

    private bool _isNavigating = false;

    private async void OnHomeTapped(object? sender, EventArgs e)
    {
        await NavigateToTabAsync("Home");
    }

    private async void OnAdventureTapped(object? sender, EventArgs e)
    {
        await NavigateToTabAsync("Adventure");
    }

    private async void OnRecordsTapped(object? sender, EventArgs e)
    {
        await NavigateToTabAsync("Records");
    }

    private async void OnBotTapped(object? sender, EventArgs e)
    {
        await NavigateToTabAsync("Bot");
    }

    private async void OnNewsTapped(object? sender, EventArgs e)
    {
        await NavigateToTabAsync("News");
    }

    private async void OnThemesTapped(object? sender, EventArgs e)
    {
        if (!ActiveTab.Equals("Home", StringComparison.OrdinalIgnoreCase))
        {
            if (Shell.Current != null)
                await Shell.Current.GoToAsync("//MainPage");
            else
                await (Shell.Current?.Navigation ?? Navigation).PopToRootAsync();

            await Task.Delay(120);
        }

        OnThemePickerRequested?.Invoke();
    }

    private async void OnMoreTapped(object? sender, EventArgs e)
    {
        try
        {
            var nav = Shell.Current?.Navigation ?? Application.Current?.Windows?.FirstOrDefault()?.Page?.Navigation;
            if (nav != null)
            {
                await nav.PushModalAsync(new MoreMenuPage(), false);
            }
        }
        catch
        {
            // Игнорируем повторные клики
        }
    }

    private async Task NavigateToTabAsync(string targetTab)
    {
        if (_isNavigating || ActiveTab.Equals(targetTab, StringComparison.OrdinalIgnoreCase))
            return;

        _isNavigating = true;
        try
        {
            var nav = Shell.Current?.Navigation ?? Application.Current?.Windows?.FirstOrDefault()?.Page?.Navigation;
            if (nav == null)
                return;

            if (targetTab.Equals("Home", StringComparison.OrdinalIgnoreCase))
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.GoToAsync("//MainPage");
                }
                else
                {
                    await nav.PopToRootAsync();
                }
                return;
            }

            Page newPage = targetTab.ToLowerInvariant() switch
            {
                "adventure" => new AdventurePage(),
                "records" => new RecordsPage(),
                "rules" => new RulesPage(),
                "stats" => new StatsPage(),
                "profile" => new ProfilePage(),
                "bot" => new GameModePage(),
                "news" => new NewsPage(),
                _ => new AdventurePage()
            };

            // Если мы на главной странице (Home), делаем PushAsync
            if (ActiveTab.Equals("Home", StringComparison.OrdinalIgnoreCase))
            {
                await nav.PushAsync(newPage);
                return;
            }

            // Если переходим с одной вторичной страницы на другую, убираем предыдущую
            var stack = nav.NavigationStack.ToList();
            await nav.PushAsync(newPage);

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
        finally
        {
            _isNavigating = false;
        }
    }
}
