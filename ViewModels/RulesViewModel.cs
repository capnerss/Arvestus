using System.Windows.Input;
using TicTacToe.Resources.Localization;

namespace TicTacToe.ViewModels;

public class RulesViewModel : BaseViewModel
{
    private string _hedgehogAdvice = "Saa 3 sümbolit ühte ritta horisontaalselt ja võit on sinu!";
    private string _activeScenario = "Horizontal";

    public string HedgehogAdvice
    {
        get => _hedgehogAdvice;
        set => SetProperty(ref _hedgehogAdvice, value);
    }

    public string ActiveScenario
    {
        get => _activeScenario;
        set => SetProperty(ref _activeScenario, value);
    }

    public string Title => AppResources.RulesTitle;

    public ICommand SelectScenarioCommand { get; }

    public RulesViewModel()
    {
        SelectScenarioCommand = new RelayCommand(p =>
        {
            if (p is string scenario)
            {
                ActiveScenario = scenario;
                HedgehogAdvice = scenario switch
                {
                    "Horizontal" => "Saa 3 sümbolit ühte ritta horisontaalselt ja võit on sinu!",
                    "Vertical" => "Ründa vertikaalselt! Kui vastane ei pane tähele, võta terve tulp!",
                    "Diagonal" => "Diagonaal läbi keskvälja on kogenud mängijate lemmikvõte!",
                    "Draw" => "Kui laud saab täis ja rida pole, on tulemuseks sõbralik viik!",
                    _ => HedgehogAdvice
                };
            }
        });
    }
}
