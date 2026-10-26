using TicTacToe.Models;

namespace TicTacToe.Services;

/// <summary>
/// Tsentraalne seadete halduse teenus (SettingsService).
/// Hoiab rakenduse seadeid operatiivmälus, reageerib muutustele reaalajas
/// ning tagab püsiva salvestamise ja taastamise Preferences abil.
/// </summary>
public class SettingsService
{
    private const string KeyLanguage = "app_language";
    private const string KeyTheme = "App_CurrentTheme";
    private const string KeyMusicEnabled = "Settings_MusicEnabled";
    private const string KeySoundEnabled = "Settings_SoundEnabled";
    private const string KeyVibrationEnabled = "Settings_VibrationEnabled";
    private const string KeyBoardSize = "Settings_BoardSize";
    private const string KeyIsBotMode = "Settings_IsBotMode";
    private const string KeyBotDifficulty = "Settings_BotDifficulty";
    private const string KeyBotSpeed = "Settings_BotSpeed";

    private static readonly Lazy<SettingsService> _lazyInstance = new(() => new SettingsService());
    public static SettingsService Instance => _lazyInstance.Value;

    /// <summary>
    /// Aktiivne seadete andmestruktuur operatiivmälus (In-Memory).
    /// </summary>
    public AppSettings Current { get; private set; } = new();

    /// <summary>
    /// Sündmus reaalajas muutustele reageerimiseks (Reaalajas dünaamiline sündmus).
    /// </summary>
    public event Action<AppSettings>? OnSettingsChanged;

    private bool _isInitialized = false;
    private bool _isApplying = false;

    private SettingsService()
    {
        Load();
    }

    /// <summary>
    /// Initsialiseerib ja taastab seaded mäluobjekti ning rakendab need süsteemile.
    /// </summary>
    public void Initialize()
    {
        if (_isInitialized) return;
        _isInitialized = true;

        Load();
        ApplyToSubsystems();
    }

    /// <summary>
    /// Taastab seaded Preferences salvestusest operatiivmällu.
    /// </summary>
    public void Load()
    {
        Current = new AppSettings
        {
            Language = Preferences.Default.Get(KeyLanguage, "et"),
            ThemeName = Preferences.Default.Get(KeyTheme, "Blue"),
            IsMusicEnabled = Preferences.Default.Get(KeyMusicEnabled, true),
            IsSoundEnabled = Preferences.Default.Get(KeySoundEnabled, true),
            IsVibrationEnabled = Preferences.Default.Get(KeyVibrationEnabled, true),
            BoardSize = Preferences.Default.Get(KeyBoardSize, 3),
            IsBotMode = Preferences.Default.Get(KeyIsBotMode, false),
            BotDifficulty = Preferences.Default.Get(KeyBotDifficulty, "Nutikas (Tavaline)"),
            BotSpeedMs = Preferences.Default.Get(KeyBotSpeed, 400)
        };
    }

    /// <summary>
    /// Salvestab praegused mäluseaded Preferences püsimällu.
    /// </summary>
    public void Save()
    {
        Preferences.Default.Set(KeyLanguage, Current.Language);
        Preferences.Default.Set(KeyTheme, Current.ThemeName);
        Preferences.Default.Set(KeyMusicEnabled, Current.IsMusicEnabled);
        Preferences.Default.Set(KeySoundEnabled, Current.IsSoundEnabled);
        Preferences.Default.Set(KeyVibrationEnabled, Current.IsVibrationEnabled);
        Preferences.Default.Set(KeyBoardSize, Current.BoardSize);
        Preferences.Default.Set(KeyIsBotMode, Current.IsBotMode);
        Preferences.Default.Set(KeyBotDifficulty, Current.BotDifficulty);
        Preferences.Default.Set(KeyBotSpeed, Current.BotSpeedMs);
    }

    /// <summary>
    /// Uuendab seadeid mälus, salvestab need ja teavitab kõiki komponente reaalajas.
    /// </summary>
    public void Update(Action<AppSettings> updateAction)
    {
        updateAction(Current);
        Save();
        ApplyToSubsystems();
        OnSettingsChanged?.Invoke(Current.Clone());
    }

    /// <summary>
    /// Lähtestab seaded vaikeväärtustele.
    /// </summary>
    public void ResetToDefaults()
    {
        Update(settings =>
        {
            settings.Language = "et";
            settings.ThemeName = "Blue";
            settings.IsMusicEnabled = true;
            settings.IsSoundEnabled = true;
            settings.IsVibrationEnabled = true;
            settings.BoardSize = 3;
            settings.IsBotMode = false;
            settings.BotDifficulty = "Nutikas (Tavaline)";
            settings.BotSpeedMs = 400;
        });
    }

    private void ApplyToSubsystems()
    {
        if (_isApplying) return;
        _isApplying = true;
        try
        {
            // Sünkroonime keele
            if (LocalizationService.Instance.CurrentLanguage != Current.Language)
            {
                LocalizationService.Instance.SetLanguage(Current.Language);
            }

            // Sünkroonime teema
            if (ThemeService.CurrentTheme.Name != Current.ThemeName)
            {
                ThemeService.SetTheme(Current.ThemeName);
            }

            // Sünkroonime heli
            AudioService.IsSoundEnabled = Current.IsSoundEnabled;
            AudioService.IsMusicEnabled = Current.IsMusicEnabled;
        }
        catch
        {
            // Vaikne erandite käsitlus käivitamisel
        }
        finally
        {
            _isApplying = false;
        }
    }
}
