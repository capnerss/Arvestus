using System.Windows.Input;
using TicTacToe.Resources.Localization;
using TicTacToe.Services;

namespace TicTacToe.ViewModels;

public class StatsViewModel : BaseViewModel
{
    private int _player1Wins;
    private int _player2Wins;
    private int _draws;
    private int _totalGames;

    public int Player1Wins { get => _player1Wins; private set => SetProperty(ref _player1Wins, value); }
    public int Player2Wins { get => _player2Wins; private set => SetProperty(ref _player2Wins, value); }
    public int Draws { get => _draws; private set => SetProperty(ref _draws, value); }
    public int TotalGames { get => _totalGames; private set => SetProperty(ref _totalGames, value); }

    public double WinRate => TotalGames > 0 ? ((double)Player1Wins / TotalGames) * 100.0 : 0.0;
    public string WinRateFormatted => $"{WinRate:F0}%";
    public string Title => AppResources.StatsTitle;

    public ICommand RefreshCommand { get; }
    public ICommand ResetStatsCommand { get; }

    public StatsViewModel()
    {
        RefreshCommand = new RelayCommand(LoadStats);
        ResetStatsCommand = new RelayCommand(() =>
        {
            GameStatsService.ResetStats();
            LoadStats();
        });
        LoadStats();
    }

    public void LoadStats()
    {
        Player1Wins = GameStatsService.Player1Wins;
        Player2Wins = GameStatsService.Player2Wins;
        Draws = GameStatsService.Draws;
        TotalGames = GameStatsService.TotalGames;
        OnPropertyChanged(nameof(WinRate));
        OnPropertyChanged(nameof(WinRateFormatted));
    }
}
