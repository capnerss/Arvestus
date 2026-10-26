using TicTacToe.Resources.Localization;

namespace TicTacToe.ViewModels;

public class NewsViewModel : BaseViewModel
{
    private string _mascotMessage = "Tere tulemast Trips-Traps-Trull seiklusse! Siin kajastuvad kõik viimased uuendused ja uudised.";

    public string MascotMessage
    {
        get => _mascotMessage;
        set => SetProperty(ref _mascotMessage, value);
    }

    public string Title => AppResources.NewsTitle;
}
