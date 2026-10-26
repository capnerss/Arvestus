using System.Windows.Input;
using TicTacToe.Resources.Localization;
using TicTacToe.Services;

namespace TicTacToe.ViewModels;

public class GameModeViewModel : BaseViewModel
{
    private int _selectedIndex;

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (SetProperty(ref _selectedIndex, value))
            {
                ApplyModeSelection(value);
                OnPropertyChanged(nameof(IsTwoPlayers));
                OnPropertyChanged(nameof(IsEasy));
                OnPropertyChanged(nameof(IsSmart));
                OnPropertyChanged(nameof(IsMaster));
            }
        }
    }

    public bool IsTwoPlayers => SelectedIndex == 0;
    public bool IsEasy => SelectedIndex == 1;
    public bool IsSmart => SelectedIndex == 2;
    public bool IsMaster => SelectedIndex == 3;

    public string Title => AppResources.GameModesTitle;

    public ICommand SelectIndexCommand { get; }

    public GameModeViewModel()
    {
        _selectedIndex = DetermineCurrentIndex();
        SelectIndexCommand = new RelayCommand(p =>
        {
            if (p is int idx)
            {
                SelectedIndex = idx;
            }
            else if (p is string s && int.TryParse(s, out int parsed))
            {
                SelectedIndex = parsed;
            }
        });
    }

    private static int DetermineCurrentIndex()
    {
        if (!AppSettingsService.IsBotMode) return 0;
        string diff = AppSettingsService.BotDifficulty;
        if (diff.StartsWith("Lihtne", StringComparison.OrdinalIgnoreCase)) return 1;
        if (diff.StartsWith("Meister", StringComparison.OrdinalIgnoreCase)) return 3;
        return 2;
    }

    private static void ApplyModeSelection(int index)
    {
        switch (index)
        {
            case 0:
                AppSettingsService.IsBotMode = false;
                break;
            case 1:
                AppSettingsService.IsBotMode = true;
                AppSettingsService.BotDifficulty = "Lihtne (Juhuslikud käigud)";
                break;
            case 2:
                AppSettingsService.IsBotMode = true;
                AppSettingsService.BotDifficulty = "Nutikas (Tavaline - Blokeerib)";
                break;
            case 3:
                AppSettingsService.IsBotMode = true;
                AppSettingsService.BotDifficulty = "Meister (Võitmatu algoritm)";
                break;
        }
    }
}
