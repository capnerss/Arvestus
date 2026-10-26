using TicTacToe.Models;
using TicTacToe.Services;
using TicTacToe.ViewModels;

namespace TicTacToe.Views;

public partial class TasksPage : ContentPage
{
    private readonly TasksViewModel _viewModel;
    private readonly Action<GameTheme> _themeChangedHandler;

    public TasksPage()
    {
        InitializeComponent();
        _viewModel = new TasksViewModel();
        BindingContext = _viewModel;

        _themeChangedHandler = (t) => MainThread.BeginInvokeOnMainThread(ApplyTheme);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ThemeService.OnThemeChanged += _themeChangedHandler;
        ApplyTheme();
        _viewModel.LoadTasksCommand.Execute(null);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ThemeService.OnThemeChanged -= _themeChangedHandler;
    }

    private void ApplyTheme()
    {
        var theme = ThemeService.CurrentTheme;
        ThemeService.ApplyToPage(this, HdrSurface, HdrShadow, BtnBack);
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await Navigation.PopAsync();
        }
    }
}
