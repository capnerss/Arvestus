namespace TicTacToe.Models;

public class GameTheme
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public Color PageBackground { get; set; } = Colors.White;
    public Color HeaderBackground { get; set; } = Colors.White;
    public Color HeaderBorderColor { get; set; } = Color.FromArgb("#E5E5E5");
    public Color HeaderSurfaceColor { get; set; } = Color.FromArgb("#1CB0F6");
    public Color HeaderShadowColor { get; set; } = Color.FromArgb("#0E88C7");
    public Color HeaderButtonColor { get; set; } = Color.FromArgb("#0E88C7");
    public Color BannerBackground { get; set; } = Color.FromArgb("#1CB0F6");
    public Color BannerBorderColor { get; set; } = Color.FromArgb("#0E88C7");
    public Color BoardBorderColor { get; set; } = Color.FromArgb("#BEE3F8");
    public Color CellBackground { get; set; } = Colors.White;
    public Color CellBorderColor { get; set; } = Color.FromArgb("#BEE3F8");
    public Color ButtonBackground { get; set; } = Color.FromArgb("#1CB0F6");
    public Color ButtonTextColor { get; set; } = Colors.White;
    public Color TitleTextColor { get; set; } = Color.FromArgb("#0B4A6F");
    public Color SubtitleTextColor { get; set; } = Color.FromArgb("#3182CE");
    public Color PrimaryTextColor { get; set; } = Color.FromArgb("#2B2B2B");
    public Color SecondaryTextColor { get; set; } = Color.FromArgb("#777777");
    public Color CardBackground { get; set; } = Colors.White;
    public Color CardBorderColor { get; set; } = Color.FromArgb("#E5E5E5");
    public Color FooterBackground { get; set; } = Colors.White;
    public Color FooterBorderColor { get; set; } = Color.FromArgb("#E5E5E5");
    public Color FooterActiveTabBg { get; set; } = Color.FromArgb("#E0F2FE");
    public Color FooterActiveTabBorder { get; set; } = Color.FromArgb("#38BDF8");
    public Color XColor { get; set; } = Color.FromArgb("#1CB0F6");
    public Color OColor { get; set; } = Color.FromArgb("#FF4B4B");
    public Color WinHighlightColor { get; set; } = Color.FromArgb("#D0EFFF");
    public Color CellShadowColor { get; set; } = Color.FromArgb("#A3D5EF");
    public Color BoardShadowColor { get; set; } = Color.FromArgb("#B4DEF5");
    public Color BannerShadowColor { get; set; } = Color.FromArgb("#0E88C7");
    public Color ButtonShadowColor { get; set; } = Color.FromArgb("#0E88C7");
    public Color RulesButtonBackground { get; set; } = Color.FromArgb("#0E88C7");
    public Color RulesButtonShadowColor { get; set; } = Color.FromArgb("#096596");
    public bool IsDark { get; set; } = false;

    public static List<GameTheme> AvailableThemes => new()
    {
        // 1. Sinine (Põhiteema / Vaikimisi tasuta aktiivne)
        new GameTheme
        {
            Name = "Blue",
            DisplayName = "Sinine",
            PageBackground = Color.FromArgb("#F4FAFF"),
            HeaderBackground = Colors.White,
            HeaderBorderColor = Color.FromArgb("#BEE3F8"),
            HeaderSurfaceColor = Color.FromArgb("#1CB0F6"),
            HeaderShadowColor = Color.FromArgb("#0E88C7"),
            HeaderButtonColor = Color.FromArgb("#0E88C7"),
            BannerBackground = Color.FromArgb("#1CB0F6"),
            BannerBorderColor = Color.FromArgb("#0E88C7"),
            BoardBorderColor = Color.FromArgb("#BEE3F8"),
            CellBackground = Colors.White,
            CellBorderColor = Color.FromArgb("#BEE3F8"),
            ButtonBackground = Color.FromArgb("#1CB0F6"),
            ButtonTextColor = Colors.White,
            TitleTextColor = Color.FromArgb("#0B4A6F"),
            SubtitleTextColor = Color.FromArgb("#3182CE"),
            PrimaryTextColor = Color.FromArgb("#0B4A6F"),
            SecondaryTextColor = Color.FromArgb("#4A7B9D"),
            CardBackground = Colors.White,
            CardBorderColor = Color.FromArgb("#D4ECFC"),
            FooterBackground = Colors.White,
            FooterBorderColor = Color.FromArgb("#E5E5E5"),
            FooterActiveTabBg = Color.FromArgb("#E0F2FE"),
            FooterActiveTabBorder = Color.FromArgb("#38BDF8"),
            XColor = Color.FromArgb("#1CB0F6"),
            OColor = Color.FromArgb("#FF4B4B"),
            WinHighlightColor = Color.FromArgb("#D0EFFF"),
            CellShadowColor = Color.FromArgb("#A3D5EF"),
            BoardShadowColor = Color.FromArgb("#B4DEF5"),
            BannerShadowColor = Color.FromArgb("#0E88C7"),
            ButtonShadowColor = Color.FromArgb("#0E88C7"),
            RulesButtonBackground = Color.FromArgb("#0E88C7"),
            RulesButtonShadowColor = Color.FromArgb("#096596"),
            IsDark = false
        },
        // 2. Must (Tasuta)
        new GameTheme
        {
            Name = "Dark",
            DisplayName = "Must",
            PageBackground = Color.FromArgb("#131F24"),
            HeaderBackground = Color.FromArgb("#131F24"),
            HeaderBorderColor = Color.FromArgb("#2B383E"),
            HeaderSurfaceColor = Color.FromArgb("#2B383E"),
            HeaderShadowColor = Color.FromArgb("#182024"),
            HeaderButtonColor = Color.FromArgb("#182024"),
            BannerBackground = Color.FromArgb("#2B383E"),
            BannerBorderColor = Color.FromArgb("#1B2529"),
            BoardBorderColor = Color.FromArgb("#2B383E"),
            CellBackground = Color.FromArgb("#1B272C"),
            CellBorderColor = Color.FromArgb("#2B383E"),
            ButtonBackground = Color.FromArgb("#1CB0F6"),
            ButtonTextColor = Colors.White,
            TitleTextColor = Colors.White,
            SubtitleTextColor = Color.FromArgb("#98A2B3"),
            PrimaryTextColor = Colors.White,
            SecondaryTextColor = Color.FromArgb("#98A2B3"),
            CardBackground = Color.FromArgb("#1B272C"),
            CardBorderColor = Color.FromArgb("#2B383E"),
            FooterBackground = Color.FromArgb("#1B272C"),
            FooterBorderColor = Color.FromArgb("#2B383E"),
            FooterActiveTabBg = Color.FromArgb("#2B383E"),
            FooterActiveTabBorder = Color.FromArgb("#1CB0F6"),
            XColor = Color.FromArgb("#1CB0F6"),
            OColor = Color.FromArgb("#FF4B4B"),
            WinHighlightColor = Color.FromArgb("#244228"),
            CellShadowColor = Color.FromArgb("#121A1E"),
            BoardShadowColor = Color.FromArgb("#1A2328"),
            BannerShadowColor = Color.FromArgb("#182024"),
            ButtonShadowColor = Color.FromArgb("#0E88C7"),
            RulesButtonBackground = Color.FromArgb("#182024"),
            RulesButtonShadowColor = Color.FromArgb("#101618"),
            IsDark = true
        },
        // 3. Kuld (Tasuline / 200 münti)
        new GameTheme
        {
            Name = "Gold",
            DisplayName = "Kuld",
            PageBackground = Color.FromArgb("#FFFDF5"),
            HeaderBackground = Colors.White,
            HeaderBorderColor = Color.FromArgb("#F4E5B8"),
            HeaderSurfaceColor = Color.FromArgb("#FFC800"),
            HeaderShadowColor = Color.FromArgb("#E5A700"),
            HeaderButtonColor = Color.FromArgb("#E5A700"),
            BannerBackground = Color.FromArgb("#FFC800"),
            BannerBorderColor = Color.FromArgb("#E5A700"),
            BoardBorderColor = Color.FromArgb("#F4E5B8"),
            CellBackground = Colors.White,
            CellBorderColor = Color.FromArgb("#F4E5B8"),
            ButtonBackground = Color.FromArgb("#FFC800"),
            ButtonTextColor = Color.FromArgb("#4A3700"),
            TitleTextColor = Color.FromArgb("#4A3700"),
            SubtitleTextColor = Color.FromArgb("#8A6F1D"),
            PrimaryTextColor = Color.FromArgb("#4A3700"),
            SecondaryTextColor = Color.FromArgb("#8A6F1D"),
            CardBackground = Colors.White,
            CardBorderColor = Color.FromArgb("#F4E5B8"),
            FooterBackground = Colors.White,
            FooterBorderColor = Color.FromArgb("#E5E5E5"),
            FooterActiveTabBg = Color.FromArgb("#FEF3C7"),
            FooterActiveTabBorder = Color.FromArgb("#FCD34D"),
            XColor = Color.FromArgb("#1CB0F6"),
            OColor = Color.FromArgb("#FF4B4B"),
            WinHighlightColor = Color.FromArgb("#FFF2B2"),
            CellShadowColor = Color.FromArgb("#E6D39E"),
            BoardShadowColor = Color.FromArgb("#ECDDB2"),
            BannerShadowColor = Color.FromArgb("#E5A700"),
            ButtonShadowColor = Color.FromArgb("#E5A700"),
            RulesButtonBackground = Color.FromArgb("#E5A700"),
            RulesButtonShadowColor = Color.FromArgb("#BF8B00"),
            IsDark = false
        },
        // 4. Roheline (Tasuline / 250 münti)
        new GameTheme
        {
            Name = "Green",
            DisplayName = "Roheline",
            PageBackground = Colors.White,
            HeaderBackground = Colors.White,
            HeaderBorderColor = Color.FromArgb("#E5E5E5"),
            HeaderSurfaceColor = Color.FromArgb("#58CC02"),
            HeaderShadowColor = Color.FromArgb("#46A302"),
            HeaderButtonColor = Color.FromArgb("#46A302"),
            BannerBackground = Color.FromArgb("#58CC02"),
            BannerBorderColor = Color.FromArgb("#46A302"),
            BoardBorderColor = Color.FromArgb("#E5E5E5"),
            CellBackground = Colors.White,
            CellBorderColor = Color.FromArgb("#E5E5E5"),
            ButtonBackground = Color.FromArgb("#58CC02"),
            ButtonTextColor = Colors.White,
            TitleTextColor = Color.FromArgb("#3C3C3C"),
            SubtitleTextColor = Color.FromArgb("#777777"),
            PrimaryTextColor = Color.FromArgb("#3C3C3C"),
            SecondaryTextColor = Color.FromArgb("#777777"),
            CardBackground = Colors.White,
            CardBorderColor = Color.FromArgb("#E5E5E5"),
            FooterBackground = Colors.White,
            FooterBorderColor = Color.FromArgb("#E5E5E5"),
            FooterActiveTabBg = Color.FromArgb("#DCFCE7"),
            FooterActiveTabBorder = Color.FromArgb("#58CC02"),
            XColor = Color.FromArgb("#1CB0F6"),
            OColor = Color.FromArgb("#FF4B4B"),
            WinHighlightColor = Color.FromArgb("#D7FFB8"),
            CellShadowColor = Color.FromArgb("#D0D7DE"),
            BoardShadowColor = Color.FromArgb("#DCE6EC"),
            BannerShadowColor = Color.FromArgb("#46A302"),
            ButtonShadowColor = Color.FromArgb("#46A302"),
            RulesButtonBackground = Color.FromArgb("#46A302"),
            RulesButtonShadowColor = Color.FromArgb("#398501"),
            IsDark = false
        }
    };

    public static GameTheme GetByName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return AvailableThemes[0]; // Blue

        return AvailableThemes.FirstOrDefault(t => 
                   t.Name.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                   t.DisplayName.Equals(name, StringComparison.OrdinalIgnoreCase))
               ?? AvailableThemes[0];
    }
}
