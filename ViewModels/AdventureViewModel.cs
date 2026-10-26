using System.Windows.Input;
using TicTacToe.Resources.Localization;

namespace TicTacToe.ViewModels;

public class AdventureViewModel : BaseViewModel
{
    private int _unlockedLevels = 1;
    private int _currentChapter = 1;

    public int UnlockedLevels { get => _unlockedLevels; set => SetProperty(ref _unlockedLevels, value); }
    public int CurrentChapter { get => _currentChapter; set => SetProperty(ref _currentChapter, value); }

    public string Title => AppResources.AdventureTitle;

    public ICommand SelectLevelCommand { get; }

    public AdventureViewModel()
    {
        SelectLevelCommand = new RelayCommand(p =>
        {
            if (p is int level)
            {
                // Level selection logic
            }
        });
    }
}
