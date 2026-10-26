using System.Windows.Input;
using TicTacToe.Resources.Localization;

namespace TicTacToe.ViewModels;

public class GameResultViewModel : BaseViewModel
{
    private string _resultTitle = AppResources.ResultVictory;
    private string _hedgehogMessage = "Suurepärane töö! Sa võitsid selle mängu!";
    private int _coinsEarned = 15;
    private int _livesLeft = 10;

    public string ResultTitle { get => _resultTitle; set => SetProperty(ref _resultTitle, value); }
    public string HedgehogMessage { get => _hedgehogMessage; set => SetProperty(ref _hedgehogMessage, value); }
    public int CoinsEarned { get => _coinsEarned; set => SetProperty(ref _coinsEarned, value); }
    public int LivesLeft { get => _livesLeft; set => SetProperty(ref _livesLeft, value); }

    public ICommand PlayAgainCommand { get; }

    public GameResultViewModel()
    {
        PlayAgainCommand = new RelayCommand(() =>
        {
            // Play again signal
        });
    }
}
