using TicTacToe.Services;

namespace TicTacToe.Views;

public partial class ProfileSavedPage : ContentPage
{
    public ProfileSavedPage() : this(UserProfileService.PlayerName, UserProfileService.PlayerAvatar)
    {
    }

    public ProfileSavedPage(string playerName, string avatarSource)
    {
        InitializeComponent();

        LblSavedName.Text = string.IsNullOrWhiteSpace(playerName) ? UserProfileService.PlayerName : playerName;
        ImgSavedAvatar.Source = string.IsNullOrWhiteSpace(avatarSource) ? UserProfileService.PlayerAvatar : avatarSource;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            ImgSavedAvatar.Scale = 0.6;
            ImgSavedAvatar.Opacity = 0.5;

            await Task.WhenAll(
                ImgSavedAvatar.ScaleToAsync(1.0, 360, Easing.SpringOut),
                ImgSavedAvatar.FadeToAsync(1.0, 240)
            );
        }
        catch
        {
            // Ignore animation cancellation
        }
    }

    private async void OnContinueClicked(object? sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }

    private async void OnEditAgainClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}

