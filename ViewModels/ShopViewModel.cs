using System.Windows.Input;
using TicTacToe.Resources.Localization;
using TicTacToe.Services;

namespace TicTacToe.ViewModels;

public record ShopAvatarItem(string File, string Name, int Price, bool IsUnlocked);
public record ShopThemeItem(string Id, string DisplayName, int Price, string AccentColor, bool IsUnlocked);

public class ShopViewModel : BaseViewModel
{
    private int _coins;
    private List<ShopAvatarItem> _avatars = new();
    private List<ShopThemeItem> _themes = new();

    public int Coins { get => _coins; set => SetProperty(ref _coins, value); }
    public List<ShopAvatarItem> Avatars { get => _avatars; set => SetProperty(ref _avatars, value); }
    public List<ShopThemeItem> Themes { get => _themes; set => SetProperty(ref _themes, value); }

    public string Title => AppResources.ShopTitle;

    public ICommand RefreshCommand { get; }
    public ICommand BuyLifeCommand { get; }

    public ShopViewModel()
    {
        RefreshCommand = new RelayCommand(Refresh);
        BuyLifeCommand = new RelayCommand(() =>
        {
            if (LivesService.CurrentLives < LivesService.MaxLives && ShopService.Coins >= 100)
            {
                if (ShopService.BuyLives(1, 100))
                {
                    Refresh();
                }
            }
        });
        Refresh();
    }

    public void Refresh()
    {
        Coins = ShopService.Coins;
    }
}
