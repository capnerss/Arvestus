using TicTacToe.Services;

namespace TicTacToe.Views;

public partial class RulesPage : ContentPage
{
    private enum ScenarioType
    {
        Horizontal,
        Vertical,
        Diagonal,
        Draw
    }

    private readonly Border[,] _cellSurfaces = new Border[3, 3];
    private readonly Border[,] _cellBases = new Border[3, 3];
    private readonly Label[,] _cellLabels = new Label[3, 3];

    private CancellationTokenSource? _playbackCts;
    private ScenarioType _currentScenario = ScenarioType.Horizontal;

    public RulesPage()
    {
        InitializeComponent();
        BuildDemoBoard();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ThemeService.ApplyToPage(this, HdrSurface, HdrShadow, BtnCloseBorder);
        await Task.Delay(200);
        await PlayScenarioAsync(ScenarioType.Horizontal);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _playbackCts?.Cancel();
    }

    private void BuildDemoBoard()
    {
        var grid = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Star)
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            RowSpacing = 6,
            ColumnSpacing = 6,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                var tileGrid = new Grid
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill
                };

                var baseBorder = new Border
                {
                    StrokeThickness = 0,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
                    BackgroundColor = Color.FromArgb("#DCE6EC"),
                    Margin = new Thickness(0, 3, 0, 0),
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill
                };

                var label = new Label
                {
                    Text = "",
                    FontSize = 24,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                var surfaceBorder = new Border
                {
                    StrokeThickness = 1.5,
                    Stroke = Color.FromArgb("#E5E5E5"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
                    BackgroundColor = Colors.White,
                    Margin = new Thickness(0, 0, 0, 3),
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    Content = label
                };

                tileGrid.Children.Add(baseBorder);
                tileGrid.Children.Add(surfaceBorder);

                _cellBases[r, c] = baseBorder;
                _cellSurfaces[r, c] = surfaceBorder;
                _cellLabels[r, c] = label;

                Grid.SetRow(tileGrid, r);
                Grid.SetColumn(tileGrid, c);
                grid.Children.Add(tileGrid);
            }
        }

        DemoBoardContainer.Content = grid;
    }

    private void ResetDemoBoard()
    {
        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                _cellLabels[r, c].Text = "";
                _cellLabels[r, c].TextColor = Color.FromArgb("#2B2B2B");
                _cellSurfaces[r, c].BackgroundColor = Colors.White;
                _cellSurfaces[r, c].Stroke = Color.FromArgb("#E5E5E5");
                _cellSurfaces[r, c].Scale = 1.0;
                _cellBases[r, c].BackgroundColor = Color.FromArgb("#DCE6EC");
            }
        }
    }

    private async Task PlayScenarioAsync(ScenarioType scenario)
    {
        _playbackCts?.Cancel();
        _playbackCts = new CancellationTokenSource();
        var token = _playbackCts.Token;

        _currentScenario = scenario;
        UpdateButtonStyles(scenario);

        try
        {
            ResetDemoBoard();

            List<(int row, int col, string symbol, bool isWin)> moves = new();
            List<(int row, int col)> winningCells = new();

            switch (scenario)
            {
                case ScenarioType.Horizontal:
                    LblHedgehogAdvice.Text = "Saa 3 sümbolit ühte ritta horisontaalselt ja võit on sinu!";
                    moves = new()
                    {
                        (1, 1, "X", true),
                        (0, 0, "O", false),
                        (1, 0, "X", true),
                        (2, 2, "O", false),
                        (1, 2, "X", true)
                    };
                    winningCells = new() { (1, 0), (1, 1), (1, 2) };
                    break;

                case ScenarioType.Vertical:
                    LblHedgehogAdvice.Text = "Ründa vertikaalselt! Kui vastane ei pane tähele, võta terve tulp!";
                    moves = new()
                    {
                        (0, 0, "X", true),
                        (1, 1, "O", false),
                        (1, 0, "X", true),
                        (0, 2, "O", false),
                        (2, 0, "X", true)
                    };
                    winningCells = new() { (0, 0), (1, 0), (2, 0) };
                    break;

                case ScenarioType.Diagonal:
                    LblHedgehogAdvice.Text = "Diagonaal läbi keskvälja on kogenud mängijate lemmikvõte!";
                    moves = new()
                    {
                        (1, 1, "X", true),
                        (0, 1, "O", false),
                        (0, 0, "X", true),
                        (1, 2, "O", false),
                        (2, 2, "X", true)
                    };
                    winningCells = new() { (0, 0), (1, 1), (2, 2) };
                    break;

                case ScenarioType.Draw:
                    LblHedgehogAdvice.Text = "Kui laud saab täis ja rida pole, on tulemuseks sõbralik viik!";
                    moves = new()
                    {
                        (0, 0, "X", false),
                        (0, 1, "O", false),
                        (0, 2, "X", false),
                        (1, 0, "X", false),
                        (1, 1, "O", false),
                        (1, 2, "O", false),
                        (2, 0, "O", false),
                        (2, 1, "X", false),
                        (2, 2, "X", false)
                    };
                    break;
            }

            // Разыгрываем ходы по очереди с задержкой
            foreach (var m in moves)
            {
                if (token.IsCancellationRequested) return;

                await Task.Delay(130, token);

                var label = _cellLabels[m.row, m.col];
                var surface = _cellSurfaces[m.row, m.col];

                label.Text = m.symbol;
                label.TextColor = m.symbol == "X" ? Color.FromArgb("#1CB0F6") : Color.FromArgb("#FF9600");

                await surface.ScaleToAsync(1.12, 50, Easing.CubicOut);
                await surface.ScaleToAsync(1.0, 50, Easing.CubicIn);
            }

            // Если есть выигрышная линия — подсвечиваем сочным зеленым и праздничным bounce
            if (winningCells.Count > 0)
            {
                if (token.IsCancellationRequested) return;
                await Task.Delay(120, token);

                foreach (var (r, c) in winningCells)
                {
                    _cellSurfaces[r, c].BackgroundColor = Color.FromArgb("#58CC02");
                    _cellSurfaces[r, c].Stroke = Color.FromArgb("#46A302");
                    _cellBases[r, c].BackgroundColor = Color.FromArgb("#398501");
                    _cellLabels[r, c].TextColor = Colors.White;
                }

                var bounceTasks = winningCells.Select(cell =>
                    Task.Run(async () =>
                    {
                        await _cellSurfaces[cell.row, cell.col].ScaleToAsync(1.18, 90, Easing.CubicOut);
                        await _cellSurfaces[cell.row, cell.col].ScaleToAsync(1.0, 90, Easing.CubicIn);
                    })
                );
                await Task.WhenAll(bounceTasks);
            }
        }
        catch (TaskCanceledException)
        {
            // Переключено пользователем
        }
    }

    private void UpdateButtonStyles(ScenarioType active)
    {
        SetButtonStyle(BtnHorizBase, BtnHorizSurface, LblHoriz, active == ScenarioType.Horizontal);
        SetButtonStyle(BtnVertBase, BtnVertSurface, LblVert, active == ScenarioType.Vertical);
        SetButtonStyle(BtnDiagBase, BtnDiagSurface, LblDiag, active == ScenarioType.Diagonal);
        SetButtonStyle(BtnDrawBase, BtnDrawSurface, LblDraw, active == ScenarioType.Draw);
    }

    private static void SetButtonStyle(Border baseBorder, Border surfaceBorder, Label label, bool isActive)
    {
        if (isActive)
        {
            baseBorder.BackgroundColor = Color.FromArgb("#1899D6");
            surfaceBorder.BackgroundColor = Color.FromArgb("#1CB0F6");
            label.TextColor = Colors.White;
        }
        else
        {
            baseBorder.BackgroundColor = Color.FromArgb("#D1D5DB");
            surfaceBorder.BackgroundColor = Color.FromArgb("#F3F4F6");
            label.TextColor = Color.FromArgb("#4B4B4B");
        }
    }

    private async void OnHorizTapped(object? sender, EventArgs e)
    {
        await AnimateButtonPressAsync(BtnHorizSurface);
        await PlayScenarioAsync(ScenarioType.Horizontal);
    }

    private async void OnVertTapped(object? sender, EventArgs e)
    {
        await AnimateButtonPressAsync(BtnVertSurface);
        await PlayScenarioAsync(ScenarioType.Vertical);
    }

    private async void OnDiagTapped(object? sender, EventArgs e)
    {
        await AnimateButtonPressAsync(BtnDiagSurface);
        await PlayScenarioAsync(ScenarioType.Diagonal);
    }

    private async void OnDrawTapped(object? sender, EventArgs e)
    {
        await AnimateButtonPressAsync(BtnDrawSurface);
        await PlayScenarioAsync(ScenarioType.Draw);
    }

    private static async Task AnimateButtonPressAsync(Border surface)
    {
        await surface.TranslateToAsync(0, 3, 50, Easing.CubicIn);
        await surface.TranslateToAsync(0, 0, 50, Easing.CubicOut);
    }

    private async void OnCloseClicked(object? sender, EventArgs e)
    {
        _playbackCts?.Cancel();
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
        else
        {
            await (Shell.Current?.Navigation ?? Navigation).PopToRootAsync();
        }
    }
}

