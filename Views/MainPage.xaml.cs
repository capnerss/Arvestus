using TicTacToe.Controls;
using TicTacToe.Models;
using TicTacToe.Services;
using TicTacToe.ViewModels;

namespace TicTacToe.Views;

public partial class MainPage : ContentPage
{
    private readonly GameEngine _engine;
    private Border[,] _cellBorders = new Border[0, 0];
    private Border[,] _cellBases = new Border[0, 0];
    private Label[,] _cellLabels = new Label[0, 0];
    private GameTheme _currentTheme;
    private PlayerSymbol _startingPlayer = PlayerSymbol.X;
    private bool _isBotThinking = false;
    private CancellationTokenSource? _peekCts;
    private bool _isHedgehogPeeking = false;

    public MainPage()
    {
        InitializeComponent();

        int initialSize = AppSettingsService.BoardSize;
        _engine = new GameEngine(initialSize);
        _engine.Player1.Name = UserProfileService.PlayerName;
        _currentTheme = ThemeService.CurrentTheme;

        bool isBot = AppSettingsService.IsBotMode;
        _engine.Mode = isBot ? GameMode.VsBot : GameMode.TwoPlayers;
        _engine.Player2.IsBot = isBot;
        _engine.Player2.Name = isBot ? $"Bot ({AppSettingsService.BotDifficulty.Split(' ')[0]})" : "Mängija 2";

        BuildGridUI();
        ApplyTheme(_currentTheme);
        UpdateProfileDisplay();
        UpdateScoreDisplay();
        UpdateTurnIndicator();
        _ = AudioService.PlayMusicAsync();
        ResetHedgehogHidden();

        GameStatsService.OnStatsChanged += OnStatsUpdated;
        UserProfileService.OnProfileChanged += OnProfileUpdated;
        LivesService.OnLivesChanged += OnLivesUpdated;
        ShopService.OnShopUpdated += OnShopUpdated;
        ThemeService.OnThemeChanged += OnThemeChangedFromService;
        DuolingoFooter.OnThemePickerRequested += () => MainThread.BeginInvokeOnMainThread(() => OnThemeClicked(null, EventArgs.Empty));
    }

    private void OnThemeChangedFromService(GameTheme theme)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (_currentTheme.Name != theme.Name)
            {
                ApplyTheme(theme);
                UpdateThemePopupUI();
            }
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LivesService.CheckRegeneration();
        ResetHedgehogHidden();
        _ = PeekHedgehogAsync("Mängime!", 1800);
        UpdateProfileDisplay();

        if (_currentTheme.Name != ThemeService.CurrentTheme.Name)
        {
            ApplyTheme(ThemeService.CurrentTheme);
        }

        bool isBot = AppSettingsService.IsBotMode;
        bool modeChanged = (_engine.Mode == GameMode.VsBot) != isBot;
        if (modeChanged)
        {
            _engine.Mode = isBot ? GameMode.VsBot : GameMode.TwoPlayers;
            _engine.Player2.IsBot = isBot;
            _engine.Player2.Name = isBot ? $"Bot ({AppSettingsService.BotDifficulty.Split(' ')[0]})" : "Mängija 2";
            StartNewGame();
        }

        UpdateScoreDisplay();
        UpdateTurnIndicator();
        _ = AudioService.PlayMusicAsync();

        if (_engine.BoardSize != AppSettingsService.BoardSize)
        {
            _engine.SetBoardSize(AppSettingsService.BoardSize);
            BuildGridUI();
            StartNewGame();
        }
    }

    private void OnStatsUpdated()
    {
        MainThread.BeginInvokeOnMainThread(UpdateScoreDisplay);
    }

    private void OnLivesUpdated()
    {
        MainThread.BeginInvokeOnMainThread(UpdateScoreDisplay);
    }

    private void OnShopUpdated()
    {
        MainThread.BeginInvokeOnMainThread(UpdateScoreDisplay);
    }

    private void OnProfileUpdated()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            UpdateProfileDisplay();
            UpdateTurnIndicator();
        });
    }

    private void UpdateProfileDisplay()
    {
        _engine.Player1.Name = UserProfileService.PlayerName;
        ImgTopAvatar.Source = UserProfileService.PlayerAvatar;
    }

    private void BuildGridUI()
    {
        int size = _engine.BoardSize;
        _cellBorders = new Border[size, size];
        _cellBases = new Border[size, size];
        _cellLabels = new Label[size, size];

        Grid grid = new Grid
        {
            RowSpacing = 7,
            ColumnSpacing = 7,
            BackgroundColor = Colors.Transparent
        };

        for (int i = 0; i < size; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }

        int fontSize = size switch
        {
            3 => 44,
            4 => 32,
            _ => 24
        };

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                var label = new Label
                {
                    Text = "",
                    FontSize = fontSize,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                };

                // Duolingo 3D taktiilne ruut: alumine varjukiht (base) + ülemine pealispind (surface)
                var tileGrid = new Grid
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill
                };

                var baseBorder = new Border
                {
                    StrokeThickness = 0,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
                    {
                        CornerRadius = new CornerRadius(size == 3 ? 16 : 12)
                    },
                    BackgroundColor = _currentTheme.CellShadowColor,
                    Margin = new Thickness(0, 4, 0, 0),
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill
                };

                var surfaceBorder = new Border
                {
                    StrokeThickness = 1.5,
                    Stroke = _currentTheme.CellBorderColor,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
                    {
                        CornerRadius = new CornerRadius(size == 3 ? 16 : 12)
                    },
                    BackgroundColor = _currentTheme.CellBackground,
                    Margin = new Thickness(0, 0, 0, 4),
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    Content = label
                };

                int row = r;
                int col = c;
                var tap = new TapGestureRecognizer();
                tap.Tapped += async (s, e) => await HandleCellClick(row, col);
                surfaceBorder.GestureRecognizers.Add(tap);

                tileGrid.Children.Add(baseBorder);
                tileGrid.Children.Add(surfaceBorder);

                _cellBorders[r, c] = surfaceBorder;
                _cellBases[r, c] = baseBorder;
                _cellLabels[r, c] = label;

                grid.Add(tileGrid, c, r);
            }
        }

        BoardContainer.Content = grid;
        RefreshBoardCells();
    }

    private async Task HandleCellClick(int row, int col)
    {
        if (_isBotThinking || _engine.IsGameOver)
            return;

        if (_engine.Mode == GameMode.VsBot && LivesService.CurrentLives <= 0)
        {
            bool goToShop = await DisplayAlertAsync("Elud otsas! ❤️",
                "Sul ei ole enam ühtegi elu. Oota taastumist (1 elu iga 10 min) või osta poest elusid juurde!",
                "Poodi 🛒", "Oota");
            if (goToShop)
            {
                await Navigation.PushAsync(new ShopPage());
            }
            return;
        }

        if (row < 0 || row >= _engine.BoardSize || col < 0 || col >= _engine.BoardSize)
            return;

        if (_engine.Board[row, col] != PlayerSymbol.None)
            return;

        var symbol = _engine.CurrentTurn;
        var player = _engine.CurrentPlayer;

        bool moved = _engine.MakeMove(row, col);
        if (!moved) return;

        AudioService.PlayTap();

        await AnimateCell(row, col, symbol, player.SymbolColor);

        if (await CheckGameEndAsync())
            return;

        UpdateTurnIndicator();

        // Kui tegemist on botiga ja mäng pole läbi
        if (_engine.Mode == GameMode.VsBot && _engine.CurrentTurn == PlayerSymbol.O)
        {
            await ExecuteBotMoveAsync();
        }
        else
        {
            string[] quotes = { "Kaval käik!", "Ohoo!", "Tubli käik!", "Põnev!", "Sinu kord!" };
            _ = PeekHedgehogAsync(quotes[Random.Shared.Next(quotes.Length)], 1400);
        }
    }

    private async Task ExecuteBotMoveAsync()
    {
        if (_isBotThinking || _engine.IsGameOver) return;

        _isBotThinking = true;
        LblTurnIndicator.Text = "Bot mõtleb käiku...";
        LblTurnIndicator.TextColor = Color.FromArgb("#FFF0B3");

        await Task.Delay(Math.Clamp(AppSettingsService.BotSpeedMs, 100, 1500));

        var botMove = _engine.GetBotMove(AppSettingsService.BotDifficulty);
        if (botMove.HasValue && !_engine.IsGameOver)
        {
            int r = botMove.Value.Row;
            int c = botMove.Value.Col;

            var botPlayer = _engine.Player2;
            _engine.MakeMove(r, c);
            AudioService.PlayTap();

            await AnimateCell(r, c, PlayerSymbol.O, botPlayer.SymbolColor);
            _ = PeekHedgehogAsync("Käidud! Sinu kord!", 1300);
            await CheckGameEndAsync();
        }

        _isBotThinking = false;
        UpdateTurnIndicator();
    }

    private async Task AnimateCell(int row, int col, PlayerSymbol symbol, Color color)
    {
        if (row < 0 || row >= _cellLabels.GetLength(0) || col < 0 || col >= _cellLabels.GetLength(1))
            return;

        var surface = _cellBorders[row, col];
        var label = _cellLabels[row, col];
        if (label == null || surface == null) return;

        // 1. Taktiilne 3D-vajutus: pinnaplaat vajub allapoole
        await surface.TranslateToAsync(0, 4, 60, Easing.CubicIn);

        label.Text = symbol.ToString();
        label.TextColor = color;
        label.Scale = 0.4;
        label.Opacity = 0.6;

        // 2. Pinnaplaat vetrub tagasi ja sümbol hüppab välja (Pop: 120% -> 100%)
        await Task.WhenAll(
            surface.TranslateToAsync(0, 0, 90, Easing.SpringOut),
            label.FadeToAsync(1.0, 80),
            label.ScaleToAsync(1.2, 80, Easing.CubicOut)
        );
        await label.ScaleToAsync(1.0, 70, Easing.CubicIn);
    }

    private async Task<bool> CheckGameEndAsync()
    {
        if (!_engine.IsGameOver)
            return false;

        _peekCts?.Cancel();

        if (_engine.Winner != null)
        {
            // Tõsta võiduruudud esile Duolingo rohelise/kuldse säraga
            foreach (var (r, c) in _engine.WinningCells)
            {
                if (r >= 0 && r < _cellBorders.GetLength(0) && c >= 0 && c < _cellBorders.GetLength(1))
                {
                    _cellBorders[r, c].BackgroundColor = _currentTheme.WinHighlightColor;
                    _cellBorders[r, c].Stroke = Color.FromArgb("#58CC02");
                    if (_cellBases != null && r < _cellBases.GetLength(0) && c < _cellBases.GetLength(1) && _cellBases[r, c] != null)
                    {
                        _cellBases[r, c].BackgroundColor = Color.FromArgb("#46A302");
                    }
                    _ = _cellBorders[r, c].ScaleToAsync(1.08, 140).ContinueWith(_ => _cellBorders[r, c].ScaleToAsync(1.0, 140));
                }
            }

            GameResultType resultType;
            int coinsEarned;
            int heartsSpent;

            if (_engine.Mode == GameMode.VsBot)
            {
                if (_engine.Winner.Symbol == PlayerSymbol.X)
                {
                    GameStatsService.RecordPlayer1Win();
                    resultType = GameResultType.Win;
                    coinsEarned = 50;
                    heartsSpent = 0;
                    ShopService.AddCoins(50);
                }
                else
                {
                    GameStatsService.RecordPlayer2Win();
                    resultType = GameResultType.Defeat;
                    coinsEarned = 5;
                    heartsSpent = 1;
                    ShopService.AddCoins(5);
                    LivesService.SpendLife();
                }
            }
            else
            {
                if (_engine.Winner.Symbol == PlayerSymbol.X)
                    GameStatsService.RecordPlayer1Win();
                else
                    GameStatsService.RecordPlayer2Win();

                resultType = GameResultType.Win;
                coinsEarned = 30;
                heartsSpent = 0;
                ShopService.AddCoins(30);
            }

            UpdateScoreDisplay();

            if (resultType == GameResultType.Win)
            {
                AudioService.PlayWin();
            }

            await Task.Delay(400);

            var winResultPage = new GameResultPage(
                resultType: resultType,
                winnerName: _engine.Winner.Name,
                winnerSymbol: _engine.Winner.Symbol.ToString(),
                isVsBot: _engine.Mode == GameMode.VsBot,
                coinsEarned: coinsEarned,
                heartsSpent: heartsSpent,
                onPlayAgain: () =>
                {
                    StartNewGame();
                });

            await Navigation.PushAsync(winResultPage);
            return true;
        }

        if (_engine.IsDraw)
        {
            GameStatsService.RecordDraw();
            ShopService.AddCoins(15);
            UpdateScoreDisplay();

            await Task.Delay(400);

            var drawResultPage = new GameResultPage(
                resultType: GameResultType.Draw,
                winnerName: string.Empty,
                winnerSymbol: string.Empty,
                isVsBot: _engine.Mode == GameMode.VsBot,
                coinsEarned: 15,
                heartsSpent: 0,
                onPlayAgain: () =>
                {
                    StartNewGame();
                });

            await Navigation.PushAsync(drawResultPage);
            return true;
        }

        return false;
    }

    private void StartNewGame()
    {
        _engine.ResetGame(_startingPlayer);
        RefreshBoardCells();
        UpdateTurnIndicator();
        _ = PeekHedgehogAsync("Uus mäng! Edu!", 1800);

        // Kui alustab Bot ja režiim on VsBot
        if (_engine.Mode == GameMode.VsBot && _engine.CurrentTurn == PlayerSymbol.O)
        {
            Dispatcher.Dispatch(async () => await ExecuteBotMoveAsync());
        }
    }

    private void RefreshBoardCells()
    {
        int size = _engine.BoardSize;

        if (_cellBorders == null || _cellLabels == null ||
            _cellBorders.GetLength(0) != size || _cellBorders.GetLength(1) != size ||
            _cellLabels.GetLength(0) != size || _cellLabels.GetLength(1) != size)
        {
            return;
        }

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                var border = _cellBorders[r, c];
                var label = _cellLabels[r, c];
                if (border == null || label == null) continue;

                border.BackgroundColor = _currentTheme.CellBackground;
                border.Stroke = _currentTheme.CellBorderColor;
                border.TranslationY = 0;

                if (_cellBases != null && r < _cellBases.GetLength(0) && c < _cellBases.GetLength(1) && _cellBases[r, c] != null)
                {
                    _cellBases[r, c].BackgroundColor = _currentTheme.CellShadowColor;
                }

                var sym = _engine.Board[r, c];
                label.Text = sym == PlayerSymbol.None ? "" : sym.ToString();
                label.TextColor = sym == PlayerSymbol.X ? _engine.Player1.SymbolColor : _engine.Player2.SymbolColor;
                label.Scale = 1.0;
                label.Opacity = 1.0;
            }
        }
    }

    private void UpdateTurnIndicator()
    {
        if (_engine.IsGameOver)
        {
            if (_engine.Winner != null)
            {
                LblTurnIndicator.Text = $"🏆 Võitja: {_engine.Winner.Name}!";
                LblTurnIndicator.TextColor = Colors.White;
            }
            else
            {
                LblTurnIndicator.Text = "🤝 Mäng jäi viiki!";
                LblTurnIndicator.TextColor = Colors.White;
            }
            return;
        }

        var player = _engine.CurrentPlayer;
        LblTurnIndicator.Text = $"Käik: {player.Name} ({player.Symbol})";
        LblTurnIndicator.TextColor = Colors.White;
    }

    private void UpdateScoreDisplay()
    {
        int p1 = GameStatsService.Player1Wins;

        // Duolingo päiseriba väärtused
        LblStreakCount.Text = p1.ToString();
        LblGemsCount.Text = ShopService.Coins.ToString();
        LblHeartsCount.Text = LivesService.CurrentLives.ToString();
    }

    private void ApplyTheme(GameTheme theme)
    {
        _currentTheme = theme;
        if (ThemeService.CurrentTheme.Name != theme.Name)
        {
            ThemeService.CurrentTheme = theme;
        }
        RootPage.BackgroundColor = theme.PageBackground;
        HeaderBorder.BackgroundColor = theme.HeaderBackground;
        HeaderBorder.Stroke = theme.HeaderBorderColor;
        UnitBanner.BackgroundColor = theme.BannerBackground;
        if (UnitBannerBase != null) UnitBannerBase.BackgroundColor = theme.BannerShadowColor;
        if (BtnRulesSurface != null) BtnRulesSurface.BackgroundColor = theme.RulesButtonBackground;
        if (BtnRulesBase != null) BtnRulesBase.BackgroundColor = theme.RulesButtonShadowColor;
        BoardBorder.Stroke = theme.BoardBorderColor;
        if (BoardBaseBorder != null) BoardBaseBorder.BackgroundColor = theme.BoardShadowColor;
        if (BtnNewGameSurface != null) BtnNewGameSurface.BackgroundColor = theme.ButtonBackground;
        if (BtnNewGameBase != null) BtnNewGameBase.BackgroundColor = theme.ButtonShadowColor;

        _engine.Player1.SymbolColor = theme.XColor;
        _engine.Player2.SymbolColor = theme.OColor;

        RefreshBoardCells();
        UpdateTurnIndicator();
    }

    // --- Sündmuste käitlejad ---

    private async void OnNewGameClicked(object? sender, EventArgs e)
    {
        if (BtnNewGameSurface != null)
        {
            await BtnNewGameSurface.TranslateToAsync(0, 4, 60, Easing.CubicIn);
            await BtnNewGameSurface.TranslateToAsync(0, 0, 80, Easing.SpringOut);
        }
        StartNewGame();
    }

    private async void OnWhoStartsClicked(object? sender, EventArgs e)
    {
        string p2Name = _engine.Mode == GameMode.VsBot ? "Bot (O)" : "Mängija 2 (O)";
        string? action = await DisplayActionSheetAsync(
            "Kes alustab mängu?",
            "Loobu",
            null,
            "Mängija 1 (X)",
            p2Name,
            "Juhuslik loos");

        if (string.IsNullOrEmpty(action) || action == "Loobu")
            return;

        if (action == "Mängija 1 (X)")
        {
            _startingPlayer = PlayerSymbol.X;
            StartNewGame();
        }
        else if (action == p2Name)
        {
            _startingPlayer = PlayerSymbol.O;
            StartNewGame();
        }
        else if (action == "Juhuslik loos")
        {
            _startingPlayer = Random.Shared.Next(2) == 0 ? PlayerSymbol.X : PlayerSymbol.O;
            string starter = _startingPlayer == PlayerSymbol.X ? "Mängija 1 (X)" : p2Name;
            await DisplayAlertAsync("🎲 Loosimine", $"Mängu alustab: {starter}", "Selge");
            StartNewGame();
        }
    }

    private async void OnChooseColorClicked(object? sender, EventArgs e)
    {
        string? action = await DisplayActionSheetAsync(
            "Vali sümbolite värvid",
            "Loobu",
            null,
            "Duolingo: Sinine (X) ja Punane (O)",
            "Neoon: Lilla (X) ja Roosa (O)",
            "Kuldne (X) ja Tumehall (O)",
            "Mündiroheline (X) ja Oranž (O)");

        if (string.IsNullOrEmpty(action) || action == "Loobu")
            return;

        if (action.StartsWith("Duolingo"))
        {
            _engine.Player1.SymbolColor = Color.FromArgb("#1CB0F6");
            _engine.Player2.SymbolColor = Color.FromArgb("#FF4B4B");
        }
        else if (action.StartsWith("Neoon"))
        {
            _engine.Player1.SymbolColor = Color.FromArgb("#7C3AED");
            _engine.Player2.SymbolColor = Color.FromArgb("#EC4899");
        }
        else if (action.StartsWith("Kuldne"))
        {
            _engine.Player1.SymbolColor = Color.FromArgb("#FFC800");
            _engine.Player2.SymbolColor = Color.FromArgb("#3C3C3C");
        }
        else if (action.StartsWith("Mündiroheline"))
        {
            _engine.Player1.SymbolColor = Color.FromArgb("#58CC02");
            _engine.Player2.SymbolColor = Color.FromArgb("#FF9600");
        }

        RefreshBoardCells();
        UpdateTurnIndicator();
    }

    private async void OnBoardSizeClicked(object? sender, EventArgs e)
    {
        string? action = await DisplayActionSheetAsync(
            "Vali mänguvälja suurus",
            "Loobu",
            null,
            "3 x 3 (Klassikaline - 3 ritta)",
            "4 x 4 (Keskmine - 4 ritta)",
            "5 x 5 (Suur - 4 ritta)");

        if (string.IsNullOrEmpty(action) || action == "Loobu")
            return;

        int newSize = _engine.BoardSize;
        if (action.StartsWith("3 x 3"))
            newSize = 3;
        else if (action.StartsWith("4 x 4"))
            newSize = 4;
        else if (action.StartsWith("5 x 5"))
            newSize = 5;

        if (newSize != _engine.BoardSize)
        {
            AppSettingsService.BoardSize = newSize;
            _engine.SetBoardSize(newSize);
            BuildGridUI();
            StartNewGame();
        }
    }

    private async void OnRulesClicked(object? sender, EventArgs e)
    {
        if (BtnRulesSurface != null)
        {
            await BtnRulesSurface.TranslateToAsync(0, 3, 50, Easing.CubicIn);
            await BtnRulesSurface.TranslateToAsync(0, 0, 70, Easing.SpringOut);
        }
        await Navigation.PushAsync(new RulesPage());
    }

    private async void OnStatsClicked(object? sender, EventArgs e)
    {
        if (HeaderTrophyPill != null)
        {
            await HeaderTrophyPill.TranslateToAsync(0, 3, 50, Easing.CubicIn);
            await HeaderTrophyPill.TranslateToAsync(0, 0, 70, Easing.SpringOut);
        }
        await Navigation.PushAsync(new StatsPage());
    }

    private async void OnShopClicked(object? sender, EventArgs e)
    {
        if (HeaderCoinsPill != null)
        {
            await HeaderCoinsPill.TranslateToAsync(0, 3, 50, Easing.CubicIn);
            await HeaderCoinsPill.TranslateToAsync(0, 0, 70, Easing.SpringOut);
        }
        await Navigation.PushAsync(new ShopPage());
    }

    private async void OnLivesClicked(object? sender, EventArgs e)
    {
        if (HeaderHeartsPill != null)
        {
            await HeaderHeartsPill.TranslateToAsync(0, 3, 50, Easing.CubicIn);
            await HeaderHeartsPill.TranslateToAsync(0, 0, 70, Easing.SpringOut);
        }
        await Navigation.PushAsync(new LivesPage());
    }

    private async void OnProfileClicked(object? sender, EventArgs e)
    {
        if (HeaderAvatarPill != null)
        {
            await HeaderAvatarPill.TranslateToAsync(0, 3, 50, Easing.CubicIn);
            await HeaderAvatarPill.TranslateToAsync(0, 0, 70, Easing.SpringOut);
        }
        await Navigation.PushAsync(new ProfilePage());
    }

    private void OnThemeClicked(object? sender, EventArgs e)
    {
        ShowThemePopup();
    }

    private async void ShowThemePopup()
    {
        UpdateThemePopupUI();
        ThemePopupOverlay.Opacity = 0;
        ThemePopupOverlay.IsVisible = true;
        ThemePopupCard.TranslationY = 320;

        await Task.WhenAll(
            ThemePopupOverlay.FadeToAsync(1, 180),
            ThemePopupCard.TranslateToAsync(0, 0, 220, Easing.SpringOut)
        );
    }

    private async Task CloseThemePopupAsync()
    {
        await Task.WhenAll(
            ThemePopupOverlay.FadeToAsync(0, 150),
            ThemePopupCard.TranslateToAsync(0, 320, 150, Easing.CubicIn)
        );
        ThemePopupOverlay.IsVisible = false;
    }

    private async void OnCloseThemePopupClicked(object? sender, EventArgs e)
    {
        await CloseThemePopupAsync();
    }

    private void OnPopupCardClicked(object? sender, EventArgs e)
    {
        // Tõkestab taustale klikkimise, kui klikitakse kaardi sees
    }

    private Action? _pendingThemePurchaseAction;

    private static int GetThemePrice(string themeName)
    {
        return themeName switch
        {
            "Green" => 250,
            "Gold" => 200,
            _ => 0
        };
    }

    private static (Color Bg, Color Stroke, string Icon) GetThemeStyle(string themeName)
    {
        return themeName switch
        {
            "Gold" => (Color.FromArgb("#FEF3C7"), Color.FromArgb("#FCD34D"), "duo_trophy.png"),
            "Green" => (Color.FromArgb("#E8F8D8"), Color.FromArgb("#58CC02"), "duo_palette.png"),
            "Dark" => (Color.FromArgb("#1B272C"), Color.FromArgb("#2B383E"), "duo_settings.png"),
            _ => (Color.FromArgb("#E0F2FE"), Color.FromArgb("#38BDF8"), "duo_gem.png")
        };
    }

    private async void OnSelectThemeOption(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is string indexStr && int.TryParse(indexStr, out int index) &&
            index >= 0 && index < GameTheme.AvailableThemes.Count)
        {
            var selectedTheme = GameTheme.AvailableThemes[index];
            if (!ShopService.IsThemeUnlocked(selectedTheme.Name))
            {
                int price = GetThemePrice(selectedTheme.Name);
                if (ShopService.Coins < price)
                {
                    await ShowAlertThemeSheetAsync(
                        "Münte pole piisavalt",
                        $"Teema «{selectedTheme.DisplayName}» maksab {price} münti! Sul on praegu {ShopService.Coins} münti. Teenimiseks võida mänge või külasta poodi.",
                        "duo_coin.png",
                        Color.FromArgb("#FEF2F2"),
                        Color.FromArgb("#FECACA"));
                    return;
                }

                var style = GetThemeStyle(selectedTheme.Name);
                await ShowConfirmThemeSheetAsync(
                    $"Osta teema «{selectedTheme.DisplayName}»?",
                    $"Hind: {price} münti (sul jääb alles {ShopService.Coins - price} münti)",
                    style.Icon,
                    style.Bg,
                    style.Stroke,
                    $"OSTA ({price} 🪙)",
                    () =>
                    {
                        if (ShopService.BuyTheme(selectedTheme.Name, price))
                        {
                            ThemeService.CurrentTheme = selectedTheme;
                            ApplyTheme(selectedTheme);
                            BuildGridUI();
                            UpdateThemePopupUI();
                            UpdateScoreDisplay();
                        }
                    });
                return;
            }

            ThemeService.CurrentTheme = selectedTheme;
            ApplyTheme(selectedTheme);
            BuildGridUI();
            UpdateThemePopupUI();
        }
    }

    private async Task ShowConfirmThemeSheetAsync(
        string title,
        string message,
        string iconSource,
        Color iconBg,
        Color iconStroke,
        string buyButtonText,
        Action onConfirm)
    {
        _pendingThemePurchaseAction = onConfirm;

        ThemeSheetTitle.Text = title;
        ThemeSheetMessage.Text = message;
        ThemeSheetIconImage.Source = iconSource;
        ThemeSheetIconBorder.BackgroundColor = iconBg;
        ThemeSheetIconBorder.Stroke = iconStroke;
        LblThemeSheetBuyButton.Text = buyButtonText;

        ThemeSheetConfirmActions.IsVisible = true;
        ThemeSheetAlertActions.IsVisible = false;

        await AnimateOpenThemeSheetAsync();
    }

    private async Task ShowAlertThemeSheetAsync(
        string title,
        string message,
        string iconSource,
        Color iconBg,
        Color iconStroke)
    {
        _pendingThemePurchaseAction = null;

        ThemeSheetTitle.Text = title;
        ThemeSheetMessage.Text = message;
        ThemeSheetIconImage.Source = iconSource;
        ThemeSheetIconBorder.BackgroundColor = iconBg;
        ThemeSheetIconBorder.Stroke = iconStroke;

        ThemeSheetConfirmActions.IsVisible = false;
        ThemeSheetAlertActions.IsVisible = true;

        await AnimateOpenThemeSheetAsync();
    }

    private async Task AnimateOpenThemeSheetAsync()
    {
        try
        {
            ThemePurchaseOverlay.Opacity = 0;
            ThemePurchaseOverlay.IsVisible = true;
            ThemePurchaseCard.TranslationY = 320;

            await Task.WhenAll(
                ThemePurchaseOverlay.FadeToAsync(1, 180, Easing.CubicOut),
                ThemePurchaseCard.TranslateToAsync(0, 0, 240, Easing.CubicOut)
            );
        }
        catch
        {
            // Ignore animation cancellation
        }
    }

    private async Task AnimateCloseThemeSheetAsync()
    {
        try
        {
            await Task.WhenAll(
                ThemePurchaseOverlay.FadeToAsync(0, 160, Easing.CubicIn),
                ThemePurchaseCard.TranslateToAsync(0, 320, 180, Easing.CubicIn)
            );
            ThemePurchaseOverlay.IsVisible = false;
            _pendingThemePurchaseAction = null;
        }
        catch
        {
            ThemePurchaseOverlay.IsVisible = false;
            _pendingThemePurchaseAction = null;
        }
    }

    private async void OnThemePurchaseScrimTapped(object? sender, EventArgs e)
    {
        await AnimateCloseThemeSheetAsync();
    }

    private async void OnCancelThemePurchaseClicked(object? sender, EventArgs e)
    {
        await AnimateCloseThemeSheetAsync();
    }

    private async void OnGoToShopFromThemeSheetClicked(object? sender, EventArgs e)
    {
        await AnimateCloseThemeSheetAsync();
        await CloseThemePopupAsync();
        await Navigation.PushAsync(new ShopPage());
    }

    private async void OnConfirmThemePurchaseClicked(object? sender, EventArgs e)
    {
        var action = _pendingThemePurchaseAction;
        await AnimateCloseThemeSheetAsync();
        action?.Invoke();
    }

    private void UpdateThemePopupUI()
    {
        var themeItems = new[] { ThemeItem0, ThemeItem1, ThemeItem2, ThemeItem3 };
        var checkLabels = new[] { CheckTheme0, CheckTheme1, CheckTheme2, CheckTheme3 };

        for (int i = 0; i < GameTheme.AvailableThemes.Count && i < themeItems.Length; i++)
        {
            var theme = GameTheme.AvailableThemes[i];
            bool isUnlocked = ShopService.IsThemeUnlocked(theme.Name);
            bool isCurrent = theme.Name == _currentTheme.Name;

            if (isCurrent)
            {
                themeItems[i].Stroke = theme.HeaderSurfaceColor;
                themeItems[i].StrokeThickness = 3;
                themeItems[i].BackgroundColor = theme.IsDark ? Color.FromArgb("#1B272C") : Color.FromArgb("#F0F9FF");
                checkLabels[i].Text = "✓";
                checkLabels[i].TextColor = theme.HeaderSurfaceColor;
                themeItems[i].Opacity = 1.0;
            }
            else if (!isUnlocked)
            {
                themeItems[i].Stroke = Color.FromArgb("#E5E5E5");
                themeItems[i].StrokeThickness = 1.5;
                themeItems[i].BackgroundColor = Color.FromArgb("#F7F7F7");
                checkLabels[i].Text = "🔒";
                checkLabels[i].TextColor = Color.FromArgb("#AFAFAF");
                themeItems[i].Opacity = 0.7;
            }
            else
            {
                themeItems[i].Stroke = Color.FromArgb("#E5E5E5");
                themeItems[i].StrokeThickness = 2;
                themeItems[i].BackgroundColor = Color.FromArgb("#F7F7F7");
                checkLabels[i].Text = "";
                themeItems[i].Opacity = 1.0;
            }
        }
    }

    #region Siilikese (Hedgehog Mascot) Piilumise Loogika

    private void ResetHedgehogHidden()
    {
        _peekCts?.Cancel();
        _isHedgehogPeeking = false;

        HedgehogPeekContainer.TranslationY = 150;
        HedgehogPeekContainer.Opacity = 1;
        HedgehogPeekBubble.Opacity = 0;
        HedgehogPeekBubble.Scale = 0.8;
        HedgehogPeekImage.Rotation = 0;
    }

    private async Task PeekHedgehogAsync(string text, int durationMs = 1800, bool wiggle = true)
    {
        _peekCts?.Cancel();
        _peekCts = new CancellationTokenSource();
        var token = _peekCts.Token;

        try
        {
            _isHedgehogPeeking = true;
            HedgehogPeekText.Text = text;
            HedgehogPeekContainer.Opacity = 1;

            // 1. Tõuseb laua tagant välja ruumi peale
            await Task.WhenAll(
                HedgehogPeekContainer.TranslateToAsync(0, 0, 360, Easing.SpringOut),
                HedgehogPeekBubble.FadeToAsync(1, 200, Easing.CubicOut),
                HedgehogPeekBubble.ScaleToAsync(1.0, 220, Easing.SpringOut)
            );

            if (token.IsCancellationRequested) return;

            // 2. Rõõmus peanoogutus või vibutus
            if (wiggle)
            {
                await HedgehogPeekImage.RotateToAsync(8, 120, Easing.SinInOut);
                if (token.IsCancellationRequested) return;
                await HedgehogPeekImage.RotateToAsync(-8, 130, Easing.SinInOut);
                if (token.IsCancellationRequested) return;
                await HedgehogPeekImage.RotateToAsync(0, 110, Easing.SinInOut);
            }

            // 3. Ootab määratud aja
            await Task.Delay(durationMs, token);
            if (token.IsCancellationRequested) return;

            // 4. Peidab end tagasi laua taha
            await Task.WhenAll(
                HedgehogPeekBubble.FadeToAsync(0, 160, Easing.CubicIn),
                HedgehogPeekBubble.ScaleToAsync(0.8, 160, Easing.CubicIn),
                HedgehogPeekContainer.TranslateToAsync(0, 150, 300, Easing.CubicIn)
            );
        }
        catch (TaskCanceledException)
        {
            // Oodatud katkestus uue käigu või mängu lõpu korral
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PeekHedgehog] Viga: {ex.Message}");
        }
        finally
        {
            _isHedgehogPeeking = false;
        }
    }

    private void OnHedgehogPeekTapped(object? sender, EventArgs e)
    {
        if (_isHedgehogPeeking) return;

        string[] tapQuotes =
        {
            "Ku-kuu! Olen siin!",
            "Mängime hoolega!",
            "Sul läheb suurepäraselt!",
            "Tee tark käik!",
            "Siilike Siim hoiab pöialt!"
        };
        _ = PeekHedgehogAsync(tapQuotes[Random.Shared.Next(tapQuotes.Length)], 2000, wiggle: true);
    }

    #endregion
}

