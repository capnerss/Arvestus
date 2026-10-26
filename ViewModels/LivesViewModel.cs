using System.Windows.Input;
using TicTacToe.Resources.Localization;
using TicTacToe.Services;

namespace TicTacToe.ViewModels;

public class LivesViewModel : BaseViewModel
{
    private int _currentLives;
    private int _maxLives;
    private string _timerText = string.Empty;
    private double _progress;

    public int CurrentLives { get => _currentLives; set => SetProperty(ref _currentLives, value); }
    public int MaxLives { get => _maxLives; set => SetProperty(ref _maxLives, value); }
    public string TimerText { get => _timerText; set => SetProperty(ref _timerText, value); }
    public double Progress { get => _progress; set => SetProperty(ref _progress, value); }

    public bool IsFull => CurrentLives >= MaxLives;
    public string Title => AppResources.LivesTitle;

    public ICommand RefreshCommand { get; }

    public LivesViewModel()
    {
        RefreshCommand = new RelayCommand(RefreshState);
        RefreshState();
    }

    public void RefreshState()
    {
        CurrentLives = LivesService.CurrentLives;
        MaxLives = LivesService.MaxLives;
        int remainingSec = LivesService.GetSecondsUntilNextLife();
        int minutes = remainingSec / 60;
        int seconds = remainingSec % 60;
        TimerText = $"{minutes:D2}:{seconds:D2}";
        Progress = LivesService.GetRegenProgress();
        OnPropertyChanged(nameof(IsFull));
    }
}
