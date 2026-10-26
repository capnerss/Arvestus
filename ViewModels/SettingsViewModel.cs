using System.Windows.Input;
using TicTacToe.Models;
using TicTacToe.Resources.Localization;
using TicTacToe.Services;

namespace TicTacToe.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private bool _soundEnabled;
    private bool _musicEnabled;
    private bool _vibrationEnabled;
    private int _boardSize;
    private string _language;
    private string _themeName;

    public bool SoundEnabled
    {
        get => _soundEnabled;
        set
        {
            if (SetProperty(ref _soundEnabled, value))
                SettingsService.Instance.Update(s => s.IsSoundEnabled = value);
        }
    }

    public bool MusicEnabled
    {
        get => _musicEnabled;
        set
        {
            if (SetProperty(ref _musicEnabled, value))
                SettingsService.Instance.Update(s => s.IsMusicEnabled = value);
        }
    }

    public bool VibrationEnabled
    {
        get => _vibrationEnabled;
        set
        {
            if (SetProperty(ref _vibrationEnabled, value))
                SettingsService.Instance.Update(s => s.IsVibrationEnabled = value);
        }
    }

    public int BoardSize
    {
        get => _boardSize;
        set
        {
            if (SetProperty(ref _boardSize, value))
                SettingsService.Instance.Update(s => s.BoardSize = value);
        }
    }

    public string Language
    {
        get => _language;
        set
        {
            if (SetProperty(ref _language, value))
                SettingsService.Instance.Update(s => s.Language = value);
        }
    }

    public string ThemeName
    {
        get => _themeName;
        set
        {
            if (SetProperty(ref _themeName, value))
                SettingsService.Instance.Update(s => s.ThemeName = value);
        }
    }

    public string Title => AppResources.SettingsTitle;

    public ICommand ResetSettingsCommand { get; }

    public SettingsViewModel()
    {
        var current = SettingsService.Instance.Current;
        _soundEnabled = current.IsSoundEnabled;
        _musicEnabled = current.IsMusicEnabled;
        _vibrationEnabled = current.IsVibrationEnabled;
        _boardSize = current.BoardSize;
        _language = current.Language;
        _themeName = current.ThemeName;

        SettingsService.Instance.OnSettingsChanged += OnSettingsChangedHandler;

        ResetSettingsCommand = new RelayCommand(() =>
        {
            SettingsService.Instance.ResetToDefaults();
        });
    }

    private void OnSettingsChangedHandler(AppSettings settings)
    {
        SetProperty(ref _soundEnabled, settings.IsSoundEnabled, nameof(SoundEnabled));
        SetProperty(ref _musicEnabled, settings.IsMusicEnabled, nameof(MusicEnabled));
        SetProperty(ref _vibrationEnabled, settings.IsVibrationEnabled, nameof(VibrationEnabled));
        SetProperty(ref _boardSize, settings.BoardSize, nameof(BoardSize));
        SetProperty(ref _language, settings.Language, nameof(Language));
        SetProperty(ref _themeName, settings.ThemeName, nameof(ThemeName));
    }
}
