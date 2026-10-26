using TicTacToe.Resources.Localization;

namespace TicTacToe.ViewModels;

public class RecordsViewModel : BaseViewModel
{
    public string Title => AppResources.HallOfFameTitle;
    public string StatusText => "Turniirid on tulekul!";
}
