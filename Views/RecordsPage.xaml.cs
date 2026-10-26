using TicTacToe.Services;

namespace TicTacToe.Views;

public partial class RecordsPage : ContentPage
{
    public RecordsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ThemeService.ApplyToPage(this, HdrSurface, HdrShadow, BtnBackBorder);
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
        else if (Navigation.NavigationStack.Count > 1)
        {
            await Navigation.PopAsync();
        }
        else
        {
            await Navigation.PopToRootAsync();
        }
    }
}

