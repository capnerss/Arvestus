using System.Windows.Input;
using TicTacToe.Services;

namespace TicTacToe.ViewModels;

public class ProfileViewModel : BaseViewModel
{
    private string _playerName = string.Empty;
    private string _currentAvatar = "janes2.png";
    private int _coins;

    public string PlayerName
    {
        get => _playerName;
        set => SetProperty(ref _playerName, value);
    }

    public string CurrentAvatar
    {
        get => _currentAvatar;
        set => SetProperty(ref _currentAvatar, value);
    }

    public int Coins
    {
        get => _coins;
        set => SetProperty(ref _coins, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand SelectAvatarCommand { get; }

    public ProfileViewModel()
    {
        LoadProfile();

        SaveCommand = new RelayCommand(() =>
        {
            UserProfileService.PlayerName = PlayerName;
            UserProfileService.PlayerAvatar = CurrentAvatar;
        });

        SelectAvatarCommand = new RelayCommand(p =>
        {
            if (p is string avatar)
            {
                CurrentAvatar = avatar;
            }
        });
    }

    public void LoadProfile()
    {
        PlayerName = UserProfileService.PlayerName;
        CurrentAvatar = UserProfileService.PlayerAvatar;
        Coins = ShopService.Coins;
    }
}
