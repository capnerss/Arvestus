namespace TicTacToe.Services;

public class UserProfileService
{
    private const string KeyPlayerName = "UserProfile_PlayerName";
    private const string KeyPlayerAvatar = "UserProfile_PlayerAvatar";

    public static event Action? OnProfileChanged;

    public static readonly List<string> AvailableAvatars = new()
    {
        "janes2.png",
        "rebane2.png",
        "karu2.png",
        "kass2.png",
        "robot2.png",
        "star2.png"
    };

    public static string PlayerName
    {
        get
        {
            string saved = Preferences.Default.Get(KeyPlayerName, "Mängija");
            if (saved == "Duo Mängija")
            {
                saved = "Mängija";
                Preferences.Default.Set(KeyPlayerName, "Mängija");
            }
            return saved;
        }
        set
        {
            Preferences.Default.Set(KeyPlayerName, string.IsNullOrWhiteSpace(value) ? "Mängija" : value.Trim());
            OnProfileChanged?.Invoke();
        }
    }

    public static string PlayerAvatar
    {
        get
        {
            string saved = Preferences.Default.Get(KeyPlayerAvatar, "janes2.png");
            if (saved == "avatar_duo.png" || saved == "janes.png")
            {
                saved = "janes2.png";
                Preferences.Default.Set(KeyPlayerAvatar, "janes2.png");
            }
            else if (saved == "avatar_fox.png" || saved == "avatar_fox.svg")
            {
                saved = "rebane2.png";
                Preferences.Default.Set(KeyPlayerAvatar, "rebane2.png");
            }
            else if (saved == "avatar_bear.png" || saved == "avatar_bear.svg")
            {
                saved = "karu2.png";
                Preferences.Default.Set(KeyPlayerAvatar, "karu2.png");
            }
            else if (saved == "avatar_cat.png" || saved == "avatar_cat.svg")
            {
                saved = "kass2.png";
                Preferences.Default.Set(KeyPlayerAvatar, "kass2.png");
            }
            else if (saved == "avatar_robot.png" || saved == "avatar_robot.svg")
            {
                saved = "robot2.png";
                Preferences.Default.Set(KeyPlayerAvatar, "robot2.png");
            }
            else if (saved == "avatar_star.png" || saved == "avatar_star.svg")
            {
                saved = "star2.png";
                Preferences.Default.Set(KeyPlayerAvatar, "star2.png");
            }
            return saved;
        }
        set
        {
            Preferences.Default.Set(KeyPlayerAvatar, value);
            OnProfileChanged?.Invoke();
        }
    }

    public static string GetPlayerLeague(int wins)
    {
        return wins switch
        {
            >= 15 => "💎 Teemantliiga (Meister)",
            >= 10 => "👑 Kuldliiga (Ekspert)",
            >= 6 => "🥈 Hõbeliiga (Kogenud)",
            >= 3 => "🥉 Pronksliiga (Edasijõudnu)",
            _ => "🌱 Algaja (Avastaja)"
        };
    }

    private const string KeyFavoriteBadgePrefix = "Profile_FavoriteBadge_";

    public static string GetFavoriteBadge(int slotIndex)
    {
        string defaultBadge = slotIndex switch
        {
            0 => "🦔",
            1 => "🏆",
            _ => "⭐"
        };
        return Preferences.Default.Get(KeyFavoriteBadgePrefix + slotIndex, defaultBadge);
    }

    public static void SetFavoriteBadge(int slotIndex, string badge)
    {
        Preferences.Default.Set(KeyFavoriteBadgePrefix + slotIndex, badge);
        OnProfileChanged?.Invoke();
    }
}
