namespace TicTacToe.Models;

/// <summary>
/// Rakenduse seadete andmestruktuur (In-Memory andmemudel).
/// Hoiab kasutaja eelistusi (keel, teema, helid, mänguväli, bot) operatiivmälus.
/// </summary>
public class AppSettings
{
    public string Language { get; set; } = "et";
    public string ThemeName { get; set; } = "Blue";
    public bool IsMusicEnabled { get; set; } = true;
    public bool IsSoundEnabled { get; set; } = true;
    public bool IsVibrationEnabled { get; set; } = true;
    public int BoardSize { get; set; } = 3;
    public bool IsBotMode { get; set; } = false;
    public string BotDifficulty { get; set; } = "Nutikas (Tavaline)";
    public int BotSpeedMs { get; set; } = 400;

    public AppSettings Clone()
    {
        return (AppSettings)MemberwiseClone();
    }
}
