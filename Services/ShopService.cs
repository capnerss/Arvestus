namespace TicTacToe.Services;

public static class ShopService
{
    private const string KeyCoins = "Shop_Coins";
    private const string KeyUnlockedAvatars = "Shop_UnlockedAvatars";
    private const string KeyUnlockedThemes = "Shop_UnlockedThemes";

    public static event Action? OnShopUpdated;

    // Default: 2 unlocked avatars
    public static readonly List<string> DefaultAvatars = new()
    {
        "janes2.png",
        "rebane2.png"
    };

    // Default: 2 unlocked themes (Sinine teemant on tasuta põhiteema, Öine on tasuta režiim)
    public static readonly List<string> DefaultThemes = new()
    {
        "Blue",
        "Dark"
    };

    public static int Coins
    {
        get => Preferences.Default.Get(KeyCoins, 500);
        set
        {
            Preferences.Default.Set(KeyCoins, Math.Max(0, value));
            OnShopUpdated?.Invoke();
        }
    }

    public static string NormalizeThemeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "Blue";
        if (name.Equals("Blue", StringComparison.OrdinalIgnoreCase) || name.Contains("Teemant", StringComparison.OrdinalIgnoreCase) || name.Contains("Diamond", StringComparison.OrdinalIgnoreCase) || name.Equals("Sinine", StringComparison.OrdinalIgnoreCase)) return "Blue";
        if (name.Equals("Green", StringComparison.OrdinalIgnoreCase) || name.Contains("Klassikaline", StringComparison.OrdinalIgnoreCase) || name.Equals("Roheline", StringComparison.OrdinalIgnoreCase)) return "Green";
        if (name.Equals("Dark", StringComparison.OrdinalIgnoreCase) || name.Contains("Öine", StringComparison.OrdinalIgnoreCase) || name.Equals("Must", StringComparison.OrdinalIgnoreCase)) return "Dark";
        if (name.Equals("Gold", StringComparison.OrdinalIgnoreCase) || name.Contains("Kuldne", StringComparison.OrdinalIgnoreCase) || name.Equals("Kuld", StringComparison.OrdinalIgnoreCase)) return "Gold";
        return name;
    }

    public static HashSet<string> GetUnlockedAvatars()
    {
        string raw = Preferences.Default.Get(KeyUnlockedAvatars, string.Empty);
        var set = new HashSet<string>(DefaultAvatars, StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(raw))
        {
            foreach (var item in raw.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                string trimmed = item.Trim();
                if (trimmed.Equals("avatar_bear.png", StringComparison.OrdinalIgnoreCase) || trimmed.Equals("avatar_bear.svg", StringComparison.OrdinalIgnoreCase))
                {
                    set.Add("karu2.png");
                }
                else if (trimmed.Equals("avatar_cat.png", StringComparison.OrdinalIgnoreCase) || trimmed.Equals("avatar_cat.svg", StringComparison.OrdinalIgnoreCase))
                {
                    set.Add("kass2.png");
                }
                else if (trimmed.Equals("avatar_robot.png", StringComparison.OrdinalIgnoreCase) || trimmed.Equals("avatar_robot.svg", StringComparison.OrdinalIgnoreCase))
                {
                    set.Add("robot2.png");
                }
                else if (trimmed.Equals("avatar_star.png", StringComparison.OrdinalIgnoreCase) || trimmed.Equals("avatar_star.svg", StringComparison.OrdinalIgnoreCase))
                {
                    set.Add("star2.png");
                }
                else
                {
                    set.Add(trimmed);
                }
            }
        }
        return set;
    }

    private const string KeyThemeMigration = "Shop_ThemeMigration_v3";

    public static HashSet<string> GetUnlockedThemes()
    {
        if (!Preferences.Default.Get(KeyThemeMigration, false))
        {
            Preferences.Default.Set(KeyThemeMigration, true);
            // Lähtestame vanad vaikeväärtused, et Sinine oleks vaikimisi ja Roheline poest ostetav
            Preferences.Default.Remove(KeyUnlockedThemes);
        }

        string raw = Preferences.Default.Get(KeyUnlockedThemes, string.Empty);
        var set = new HashSet<string>(DefaultThemes, StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(raw))
        {
            foreach (var item in raw.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                set.Add(NormalizeThemeName(item.Trim()));
            }
        }
        return set;
    }

    public static bool IsAvatarUnlocked(string avatar)
    {
        return GetUnlockedAvatars().Contains(avatar);
    }

    public static bool IsThemeUnlocked(string themeName)
    {
        return GetUnlockedThemes().Contains(NormalizeThemeName(themeName));
    }

    public static bool BuyAvatar(string avatar, int price)
    {
        if (Coins < price || IsAvatarUnlocked(avatar))
            return false;

        Coins -= price;
        var set = GetUnlockedAvatars();
        set.Add(avatar);
        Preferences.Default.Set(KeyUnlockedAvatars, string.Join(';', set));
        OnShopUpdated?.Invoke();
        return true;
    }

    public static bool BuyTheme(string themeName, int price)
    {
        string normalized = NormalizeThemeName(themeName);
        if (Coins < price || IsThemeUnlocked(normalized))
            return false;

        Coins -= price;
        var set = GetUnlockedThemes();
        set.Add(normalized);
        Preferences.Default.Set(KeyUnlockedThemes, string.Join(';', set));
        OnShopUpdated?.Invoke();
        return true;
    }

    public static bool BuyLives(int count, int price)
    {
        if (Coins < price)
            return false;

        if (LivesService.CurrentLives >= LivesService.MaxLives)
            return false;

        Coins -= price;
        LivesService.AddLives(count);
        OnShopUpdated?.Invoke();
        return true;
    }

    public static bool BuyFullLives(int price)
    {
        if (Coins < price)
            return false;

        if (LivesService.CurrentLives >= LivesService.MaxLives)
            return false;

        Coins -= price;
        LivesService.FillLives();
        OnShopUpdated?.Invoke();
        return true;
    }

    public static void AddCoins(int amount)
    {
        if (amount <= 0) return;
        Coins += amount;
    }
}
