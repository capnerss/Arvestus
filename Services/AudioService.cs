using Plugin.Maui.Audio;

namespace TicTacToe.Services;

/// <summary>
/// Teenus mängu taustamuusika ja heliefektide haldamiseks (Plugin.Maui.Audio).
/// Võimaldab heli esitada, peatada, jätkata ja seadeid Preferences abil salvestada.
/// </summary>
public static class AudioService
{
    private const string KeyMusicEnabled = "Settings_MusicEnabled";
    private const string KeySoundEnabled = "Settings_SoundEnabled";

    private static IAudioPlayer? _musicPlayer;
    private static bool _isInitializing = false;

    public static event Action? OnMusicStateChanged;
    public static event Action? OnSoundStateChanged;

    public static bool IsMusicEnabled
    {
        get => Preferences.Default.Get(KeyMusicEnabled, true);
        set
        {
            bool current = Preferences.Default.Get(KeyMusicEnabled, true);
            Preferences.Default.Set(KeyMusicEnabled, value);
            if (!value)
            {
                PauseMusic();
            }
            else if (!current || _musicPlayer == null || !_musicPlayer.IsPlaying)
            {
                _ = PlayMusicAsync();
            }
            OnMusicStateChanged?.Invoke();
        }
    }

    public static bool IsSoundEnabled
    {
        get => Preferences.Default.Get(KeySoundEnabled, true);
        set
        {
            Preferences.Default.Set(KeySoundEnabled, value);
            OnSoundStateChanged?.Invoke();
        }
    }

    public static bool IsMusicPlaying => _musicPlayer != null && _musicPlayer.IsPlaying;

    public static async Task InitializeAsync()
    {
        if (_musicPlayer != null || _isInitializing)
            return;

        _isInitializing = true;
        try
        {
            var musicStream = await FileSystem.OpenAppPackageFileAsync("background_music.wav");
            _musicPlayer = AudioManager.Current.CreatePlayer(musicStream);
            _musicPlayer.Loop = true;
            _musicPlayer.Volume = 0.4;

            if (IsMusicEnabled)
            {
                _musicPlayer.Play();
            }
        }
        catch
        {
            // Platvormi audio piirangute vaigistamine
        }
        finally
        {
            _isInitializing = false;
        }
    }

    public static async Task PlayMusicAsync()
    {
        if (_musicPlayer == null)
        {
            await InitializeAsync();
        }

        if (_musicPlayer != null && !_musicPlayer.IsPlaying && IsMusicEnabled)
        {
            try
            {
                _musicPlayer.Play();
                OnMusicStateChanged?.Invoke();
            }
            catch
            {
                // Ignore
            }
        }
    }

    public static void PauseMusic()
    {
        if (_musicPlayer != null && _musicPlayer.IsPlaying)
        {
            try
            {
                _musicPlayer.Pause();
                OnMusicStateChanged?.Invoke();
            }
            catch
            {
                // Ignore
            }
        }
    }

    public static void ResumeMusic()
    {
        if (_musicPlayer != null && !_musicPlayer.IsPlaying)
        {
            try
            {
                _musicPlayer.Play();
                OnMusicStateChanged?.Invoke();
            }
            catch
            {
                // Ignore
            }
        }
    }

    public static void ToggleMusic()
    {
        if (IsMusicPlaying)
        {
            PauseMusic();
            Preferences.Default.Set(KeyMusicEnabled, false);
        }
        else
        {
            Preferences.Default.Set(KeyMusicEnabled, true);
            _ = PlayMusicAsync();
        }
        OnMusicStateChanged?.Invoke();
    }

    public static async void PlayTap()
    {
        if (!IsSoundEnabled)
            return;

        try
        {
            var stream = await FileSystem.OpenAppPackageFileAsync("tap.wav");
            var player = AudioManager.Current.CreatePlayer(stream);
            player.Volume = 0.6;
            player.Play();
        }
        catch
        {
            // Ignore
        }
    }

    public static async void PlayWin()
    {
        if (!IsSoundEnabled)
            return;

        try
        {
            var stream = await FileSystem.OpenAppPackageFileAsync("win.wav");
            var player = AudioManager.Current.CreatePlayer(stream);
            player.Volume = 0.8;
            player.Play();
        }
        catch
        {
            // Ignore
        }
    }
}
