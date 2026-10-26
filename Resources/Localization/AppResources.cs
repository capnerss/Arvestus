using System.Globalization;
using System.Resources;

namespace TicTacToe.Resources.Localization;

/// <summary>
/// Tugevalt tüübitud juurdepääs RESX ressurssidele (AppResources.resx, AppResources.en.resx, AppResources.ru.resx).
/// </summary>
public class AppResources
{
    private static ResourceManager? _resourceManager;
    private static CultureInfo? _culture;

    public static ResourceManager ResourceManager
    {
        get
        {
            if (_resourceManager == null)
            {
                _resourceManager = new ResourceManager(
                    "TicTacToe.Resources.Localization.AppResources",
                    typeof(AppResources).Assembly);
            }
            return _resourceManager;
        }
    }

    public static CultureInfo? Culture
    {
        get => _culture;
        set => _culture = value;
    }

    public static string GetString(string name)
    {
        try
        {
            return ResourceManager.GetString(name, Culture) ?? name;
        }
        catch
        {
            return name;
        }
    }

    public static string AppTitle => GetString(nameof(AppTitle));
    public static string SettingsTitle => GetString(nameof(SettingsTitle));
    public static string LanguageSelectTitle => GetString(nameof(LanguageSelectTitle));
    public static string BoardSizeTitle => GetString(nameof(BoardSizeTitle));
    public static string DataManagementTitle => GetString(nameof(DataManagementTitle));
    public static string ResetGameBtn => GetString(nameof(ResetGameBtn));
    public static string ResetGameConfirmTitle => GetString(nameof(ResetGameConfirmTitle));
    public static string ResetGameConfirmMsg => GetString(nameof(ResetGameConfirmMsg));
    public static string ResetYes => GetString(nameof(ResetYes));
    public static string ResetDoneTitle => GetString(nameof(ResetDoneTitle));
    public static string ResetDoneMsg => GetString(nameof(ResetDoneMsg));
    public static string RulesTitle => GetString(nameof(RulesTitle));
    public static string ThemesTitle => GetString(nameof(ThemesTitle));
    public static string ProfileTitle => GetString(nameof(ProfileTitle));
    public static string StatsTitle => GetString(nameof(StatsTitle));
    public static string TasksTitle => GetString(nameof(TasksTitle));
    public static string ShopTitle => GetString(nameof(ShopTitle));
    public static string LivesTitle => GetString(nameof(LivesTitle));
    public static string GameModesTitle => GetString(nameof(GameModesTitle));
    public static string ModeTwoPlayers => GetString(nameof(ModeTwoPlayers));
    public static string ModeEasy => GetString(nameof(ModeEasy));
    public static string ModeSmart => GetString(nameof(ModeSmart));
    public static string ModeMaster => GetString(nameof(ModeMaster));
    public static string StartGame => GetString(nameof(StartGame));
    public static string PlayAgain => GetString(nameof(PlayAgain));
    public static string Done => GetString(nameof(Done));
    public static string Cancel => GetString(nameof(Cancel));
    public static string Save => GetString(nameof(Save));
    public static string All => GetString(nameof(All));
    public static string Pending => GetString(nameof(Pending));
    public static string Completed => GetString(nameof(Completed));
    public static string NewTask => GetString(nameof(NewTask));
    public static string SyncRemote => GetString(nameof(SyncRemote));
    public static string YourTurn => GetString(nameof(YourTurn));
    public static string BotTurn => GetString(nameof(BotTurn));
    public static string Victory => GetString(nameof(Victory));
    public static string Defeat => GetString(nameof(Defeat));
    public static string Draw => GetString(nameof(Draw));
    public static string MoreOptions => GetString(nameof(MoreOptions));
    public static string AdventureTitle => GetString(nameof(AdventureTitle));
    public static string HallOfFameTitle => GetString(nameof(HallOfFameTitle));
    public static string NewsTitle => GetString(nameof(NewsTitle));
    public static string ResultVictory => GetString(nameof(ResultVictory));
    public static string MusicTitle => GetString(nameof(MusicTitle));
    public static string SoundEffectsTitle => GetString(nameof(SoundEffectsTitle));
    public static string PlayMusicBtn => GetString(nameof(PlayMusicBtn));
    public static string PauseMusicBtn => GetString(nameof(PauseMusicBtn));
    public static string SoundOn => GetString(nameof(SoundOn));
    public static string SoundOff => GetString(nameof(SoundOff));
    public static string FavoriteBadgesTitle => GetString(nameof(FavoriteBadgesTitle));
    public static string DragDropHint => GetString(nameof(DragDropHint));
    public static string SlotFirstPlace => GetString(nameof(SlotFirstPlace));
    public static string SlotSecondPlace => GetString(nameof(SlotSecondPlace));
    public static string SlotThirdPlace => GetString(nameof(SlotThirdPlace));
}
