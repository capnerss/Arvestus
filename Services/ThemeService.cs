using TicTacToe.Models;

namespace TicTacToe.Services;

public static class ThemeService
{
    private const string KeyCurrentTheme = "App_CurrentTheme";

    public static event Action<GameTheme>? OnThemeChanged;

    public static GameTheme CurrentTheme
    {
        get
        {
            string name = Preferences.Default.Get(KeyCurrentTheme, "Blue");
            return GameTheme.GetByName(name);
        }
        set
        {
            if (value != null)
            {
                Preferences.Default.Set(KeyCurrentTheme, value.Name);
                OnThemeChanged?.Invoke(value);
            }
        }
    }

    public static void SetTheme(string name)
    {
        CurrentTheme = GameTheme.GetByName(name);
    }

    public static void ApplyToPage(
        ContentPage page,
        Border? headerSurface = null,
        Border? headerShadow = null,
        Border? backButton = null,
        Border? extraPill = null)
    {
        var theme = CurrentTheme;

        // Lehe taust
        page.BackgroundColor = theme.PageBackground;

        // 3D-päise pinnad
        if (headerSurface != null)
        {
            headerSurface.BackgroundColor = theme.HeaderSurfaceColor;
        }

        if (headerShadow != null)
        {
            headerShadow.BackgroundColor = theme.HeaderShadowColor;
        }

        if (backButton != null)
        {
            backButton.BackgroundColor = theme.HeaderButtonColor;
        }

        if (extraPill != null)
        {
            extraPill.BackgroundColor = theme.HeaderButtonColor;
        }
    }
}
