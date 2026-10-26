using System.Windows.Input;
using TicTacToe.Models;
using TicTacToe.Services;

namespace TicTacToe.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly GameEngine _engine = new(3);
    private int _playerScore;
    private int _botScore;
    private int _drawScore;
    private int _lives;
    private int _coins;
    private string _statusMessage = string.Empty;
    private GameTheme _currentTheme;

    public int PlayerScore { get => _playerScore; set => SetProperty(ref _playerScore, value); }
    public int BotScore { get => _botScore; set => SetProperty(ref _botScore, value); }
    public int DrawScore { get => _drawScore; set => SetProperty(ref _drawScore, value); }
    public int Lives { get => _lives; set => SetProperty(ref _lives, value); }
    public int Coins { get => _coins; set => SetProperty(ref _coins, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public GameTheme CurrentTheme { get => _currentTheme; set => SetProperty(ref _currentTheme, value); }

    public PlayerSymbol CurrentPlayer => _engine.CurrentTurn;
    public bool IsGameOver => _engine.IsGameOver;
    public bool IsDraw => _engine.IsDraw;
    public Player? Winner => _engine.Winner;

    public ICommand NewGameCommand { get; }
    public ICommand RefreshCommand { get; }

    public MainViewModel()
    {
        _currentTheme = ThemeService.CurrentTheme;

        NewGameCommand = new RelayCommand(StartNewGame);
        RefreshCommand = new RelayCommand(RefreshState);

        RefreshState();
        StartNewGame();
    }

    public void RefreshState()
    {
        Lives = LivesService.CurrentLives;
        Coins = ShopService.Coins;
        CurrentTheme = ThemeService.CurrentTheme;
    }

    public void StartNewGame()
    {
        _engine.ResetGame(PlayerSymbol.X);
        StatusMessage = "Sinu käik! Tee esimene samm.";
        OnPropertyChanged(nameof(CurrentPlayer));
        OnPropertyChanged(nameof(IsGameOver));
        OnPropertyChanged(nameof(IsDraw));
        OnPropertyChanged(nameof(Winner));
    }

    public bool MakeMove(int row, int col, out PlayerSymbol placed)
    {
        placed = _engine.CurrentTurn;
        if (_engine.MakeMove(row, col))
        {
            OnPropertyChanged(nameof(CurrentPlayer));
            OnPropertyChanged(nameof(IsGameOver));
            OnPropertyChanged(nameof(IsDraw));
            OnPropertyChanged(nameof(Winner));
            return true;
        }
        return false;
    }
}
