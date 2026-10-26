namespace TicTacToe.Services;

/// <summary>
/// Fassaad ja tagasiühilduvuskiht tsentraalsele SettingsService teenusele.
/// Suunab kõik päringud operatiivmälus olevale SettingsService.Instance.Current mudelile.
/// </summary>
public static class AppSettingsService
{
    public static event Action<int>? OnBoardSizeChanged;

    public static bool IsBotMode
    {
        get => SettingsService.Instance.Current.IsBotMode;
        set => SettingsService.Instance.Update(s => s.IsBotMode = value);
    }

    public static bool SoundEnabled
    {
        get => SettingsService.Instance.Current.IsSoundEnabled;
        set => SettingsService.Instance.Update(s => s.IsSoundEnabled = value);
    }

    public static bool VibrationEnabled
    {
        get => SettingsService.Instance.Current.IsVibrationEnabled;
        set => SettingsService.Instance.Update(s => s.IsVibrationEnabled = value);
    }

    public static string BotDifficulty
    {
        get => SettingsService.Instance.Current.BotDifficulty;
        set => SettingsService.Instance.Update(s => s.BotDifficulty = value);
    }

    public static int BotSpeedMs
    {
        get => SettingsService.Instance.Current.BotSpeedMs;
        set => SettingsService.Instance.Update(s => s.BotSpeedMs = value);
    }

    public static int BoardSize
    {
        get => SettingsService.Instance.Current.BoardSize;
        set
        {
            int clamped = Math.Clamp(value, 3, 5);
            SettingsService.Instance.Update(s => s.BoardSize = clamped);
            OnBoardSizeChanged?.Invoke(clamped);
        }
    }
}
