namespace TicTacToe.Services;

public class GameStatsService
{
    private const string KeyPlayer1Wins = "Stats_P1_Wins";
    private const string KeyPlayer2Wins = "Stats_P2_Wins";
    private const string KeyDraws = "Stats_Draws";
    private const string KeyTotalGames = "Stats_Total_Games";

    public static event Action? OnStatsChanged;

    public static int Player1Wins
    {
        get => Preferences.Default.Get(KeyPlayer1Wins, 0);
        private set => Preferences.Default.Set(KeyPlayer1Wins, value);
    }

    public static int Player2Wins
    {
        get => Preferences.Default.Get(KeyPlayer2Wins, 0);
        private set => Preferences.Default.Set(KeyPlayer2Wins, value);
    }

    public static int Draws
    {
        get => Preferences.Default.Get(KeyDraws, 0);
        private set => Preferences.Default.Set(KeyDraws, value);
    }

    public static int TotalGames
    {
        get => Preferences.Default.Get(KeyTotalGames, 0);
        private set => Preferences.Default.Set(KeyTotalGames, value);
    }

    public static void RecordPlayer1Win()
    {
        Player1Wins++;
        TotalGames++;
        OnStatsChanged?.Invoke();
    }

    public static void RecordPlayer2Win()
    {
        Player2Wins++;
        TotalGames++;
        OnStatsChanged?.Invoke();
    }

    public static void RecordDraw()
    {
        Draws++;
        TotalGames++;
        OnStatsChanged?.Invoke();
    }

    public static void ResetStats()
    {
        Preferences.Default.Remove(KeyPlayer1Wins);
        Preferences.Default.Remove(KeyPlayer2Wins);
        Preferences.Default.Remove(KeyDraws);
        Preferences.Default.Remove(KeyTotalGames);
        OnStatsChanged?.Invoke();
    }
}
