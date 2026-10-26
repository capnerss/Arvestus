using TicTacToe.Models;
using TicTacToe.Services;

namespace TicTacToe.Views;

public partial class AdventurePuzzlePage : ContentPage
{
    private AdventurePuzzle _puzzle;
    private int _currentTaskIndex = 0;
    private int _currentStepIndex = 0;
    private bool _isBusy = false;
    private bool _isSolved = false;

    private PlayerSymbol[,] _board = new PlayerSymbol[3, 3];
    private Border[,] _cellBorders = new Border[3, 3];
    private Border[,] _cellBases = new Border[3, 3];
    private Label[,] _cellLabels = new Label[3, 3];

    public AdventurePuzzlePage(int levelId = 1)
    {
        InitializeComponent();

        LivesService.OnLivesChanged += () => MainThread.BeginInvokeOnMainThread(UpdateLivesUI);
        ThemeService.OnThemeChanged += (theme) => MainThread.BeginInvokeOnMainThread(() =>
        {
            ApplyTheme(theme);
            RefreshBoardVisuals();
        });

        _puzzle = AdventurePuzzleService.GetPuzzleById(levelId) ?? AdventurePuzzleService.GetPuzzles()[0];
        _currentTaskIndex = 0;
        LoadTask(_currentTaskIndex);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ApplyTheme(ThemeService.CurrentTheme);
        RefreshBoardVisuals();
        UpdateLivesUI();
    }

    private void LoadTask(int taskIndex)
    {
        _currentTaskIndex = Math.Clamp(taskIndex, 0, _puzzle.Tasks.Count - 1);
        _currentStepIndex = 0;
        _isSolved = false;
        _isBusy = false;

        var task = _puzzle.Tasks[_currentTaskIndex];

        // Копируем доску текущего задания
        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                _board[r, c] = task.InitialBoard[r, c];
            }
        }

        // Применяем тему и обновляем все тексты под текущую задачу
        ApplyTheme(ThemeService.CurrentTheme);

        BuildBoardGrid();
    }

    private void ApplyTheme(GameTheme theme)
    {
        this.BackgroundColor = theme.PageBackground;

        // 1. Хедер в цветах текущей темы
        HdrBase.BackgroundColor = theme.HeaderSurfaceColor;
        HdrShadow.BackgroundColor = theme.HeaderShadowColor;
        BtnBackBorder.BackgroundColor = theme.HeaderButtonColor;
        LivesPillBorder.BackgroundColor = theme.HeaderButtonColor;
        UpdateLivesUI();

        LblLevelTitle.Text = _puzzle.Title;
        LblLevelBadge.Text = $"ÜLESANNE {_currentTaskIndex + 1}/5 • {new string('★', _puzzle.DifficultyStars)}{new string('☆', 5 - _puzzle.DifficultyStars)}";
        LblLevelBadge.TextColor = theme.IsDark ? Color.FromArgb("#98A2B3") : Color.FromArgb("#E0F2FE");

        // 1.1. Прогресс-бар уроков (Duolingo style: 1/5 .. 5/5)
        ProgressTaskBar.Progress = (_currentTaskIndex + 1) / 5.0;
        ProgressTaskBar.ProgressColor = theme.ButtonBackground;

        // 2. Карточка маскота
        CardMascot.BackgroundColor = theme.CardBackground;
        CardMascot.Stroke = theme.CardBorderColor;
        bool isDark = theme.IsDark;
        MascotAvatarBorder.BackgroundColor = isDark ? Color.FromArgb("#24333A") : (theme.Name == "Green" ? Color.FromArgb("#F0FDF4") : Color.FromArgb("#F0F9FF"));
        MascotAvatarBorder.Stroke = isDark ? Color.FromArgb("#32444E") : theme.CellBorderColor;
        ImgMascot.Source = "happy.png";

        var task = _puzzle.Tasks[_currentTaskIndex];
        LblMascotHeader.Text = $"ÜLESANNE {_currentTaskIndex + 1} / 5";
        LblMascotHeader.TextColor = theme.HeaderSurfaceColor;
        LblMascotText.Text = string.IsNullOrEmpty(task.Instruction) ? _puzzle.Subtitle : task.Instruction;
        LblMascotText.TextColor = theme.PrimaryTextColor;

        // 3. Индикатор хода
        LblTurnIndicator.Text = $"Sinu kord ({task.PlayerSymbol}) • Vali õige ruut";
        LblTurnIndicator.TextColor = theme.HeaderSurfaceColor;

        // 4. Игровое поле Duolingo 3D (как на главной странице)
        BoardBaseBorder.BackgroundColor = theme.BoardShadowColor;
        BoardBorder.BackgroundColor = theme.CardBackground;
        BoardBorder.Stroke = theme.BoardBorderColor;

        // 5. Кнопка действия на карточке результата в цветах темы
        BtnNextAction.BackgroundColor = theme.ButtonBackground;
        BtnNextActionBase.BackgroundColor = theme.ButtonShadowColor;

        CardResult.IsVisible = false;
    }

    private void BuildBoardGrid()
    {
        GridBoard.Children.Clear();
        GridBoard.RowDefinitions.Clear();
        GridBoard.ColumnDefinitions.Clear();

        for (int i = 0; i < 3; i++)
        {
            GridBoard.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            GridBoard.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }

        var theme = ThemeService.CurrentTheme;

        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                int row = r;
                int col = c;

                var label = new Label
                {
                    FontSize = 44,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                };

                // Duolingo 3D тактильный блок: базовый слой тени + лицевая грань (в точности как на MainPage)
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
                        CornerRadius = new CornerRadius(16)
                    },
                    BackgroundColor = theme.CellShadowColor,
                    Margin = new Thickness(0, 4, 0, 0),
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill
                };

                var surfaceBorder = new Border
                {
                    StrokeThickness = 1.5,
                    Stroke = theme.CellBorderColor,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
                    {
                        CornerRadius = new CornerRadius(16)
                    },
                    BackgroundColor = theme.CellBackground,
                    Margin = new Thickness(0, 0, 0, 4),
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    Content = label
                };

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => OnCellTapped(row, col);
                surfaceBorder.GestureRecognizers.Add(tapGesture);

                tileGrid.Add(baseBorder);
                tileGrid.Add(surfaceBorder);

                _cellBases[r, c] = baseBorder;
                _cellBorders[r, c] = surfaceBorder;
                _cellLabels[r, c] = label;

                GridBoard.Add(tileGrid, c, r);
            }
        }

        RefreshBoardVisuals();
    }

    private void RefreshBoardVisuals()
    {
        var theme = ThemeService.CurrentTheme;

        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                var sym = _board[r, c];
                var label = _cellLabels[r, c];
                var border = _cellBorders[r, c];
                var shadow = _cellBases[r, c];

                if (border == null || label == null || shadow == null) continue;

                border.BackgroundColor = theme.CellBackground;
                border.Stroke = theme.CellBorderColor;
                border.TranslationY = 0;
                shadow.BackgroundColor = theme.CellShadowColor;

                if (sym == PlayerSymbol.X)
                {
                    label.Text = "X";
                    label.TextColor = theme.XColor;
                }
                else if (sym == PlayerSymbol.O)
                {
                    label.Text = "O";
                    label.TextColor = theme.OColor;
                }
                else
                {
                    label.Text = "";
                }
            }
        }
    }

    private async void OnCellTapped(int row, int col)
    {
        if (_isBusy || _isSolved) return;

        // Если клетка уже занята
        if (_board[row, col] != PlayerSymbol.None) return;

        var task = _puzzle.Tasks[_currentTaskIndex];
        var step = task.SolutionSteps[_currentStepIndex];

        // Анимация нажатия
        var border = _cellBorders[row, col];
        await border.TranslateToAsync(0, 3, 40, Easing.CubicIn);
        await border.TranslateToAsync(0, 0, 40, Easing.CubicOut);

        // ПРОВЕРКА: Верный ли ход?
        if (row == step.ExpectedRow && col == step.ExpectedCol)
        {
            // ПРАВИЛЬНЫЙ ХОД!
            _board[row, col] = task.PlayerSymbol;
            RefreshBoardVisuals();

            var theme = ThemeService.CurrentTheme;

            // Анимация увеличения правильного хода
            _ = _cellLabels[row, col].ScaleToAsync(1.2, 100).ContinueWith(_ => _cellLabels[row, col].ScaleToAsync(1.0, 100));

            // Проверяем, есть ли следующий шаг или ответ бота в текущей задаче
            if (step.BotResponseRow >= 0 && step.BotResponseCol >= 0 && _currentStepIndex + 1 < task.SolutionSteps.Count)
            {
                _isBusy = true;
                LblTurnIndicator.Text = "Vastane mõtleb kaitsmisele...";
                LblTurnIndicator.TextColor = Color.FromArgb("#FF9600");
                LblMascotText.Text = "Väga hea käik! Vastane üritab end kaitsta...";

                await Task.Delay(450);

                // Бот делает ответный ход
                var botSymbol = task.PlayerSymbol == PlayerSymbol.X ? PlayerSymbol.O : PlayerSymbol.X;
                _board[step.BotResponseRow, step.BotResponseCol] = botSymbol;
                RefreshBoardVisuals();

                var botLabel = _cellLabels[step.BotResponseRow, step.BotResponseCol];
                _ = botLabel.ScaleToAsync(1.2, 100).ContinueWith(_ => botLabel.ScaleToAsync(1.0, 100));

                _currentStepIndex++;
                _isBusy = false;

                LblTurnIndicator.Text = $"Sinu kord ({task.PlayerSymbol}) • Lõpeta rünnak!";
                LblTurnIndicator.TextColor = theme.HeaderSurfaceColor;
                LblMascotText.Text = "Nüüd vii võidukombinatsioon lõpule!";
            }
            else
            {
                // ТЕКУЩАЯ ЗАДАЧА РЕШЕНА!
                HandleTaskSolved(row, col);
            }
        }
        else
        {
            // НЕВЕРНЫЙ ХОД!
            await HandleWrongMove(row, col);
        }
    }

    private List<(int r, int c)> FindWinningLine(PlayerSymbol symbol)
    {
        var lines = new (int, int)[][]
        {
            new[] { (0,0), (0,1), (0,2) },
            new[] { (1,0), (1,1), (1,2) },
            new[] { (2,0), (2,1), (2,2) },
            new[] { (0,0), (1,0), (2,0) },
            new[] { (0,1), (1,1), (2,1) },
            new[] { (0,2), (1,2), (2,2) },
            new[] { (0,0), (1,1), (2,2) },
            new[] { (0,2), (1,1), (2,0) }
        };

        foreach (var line in lines)
        {
            if (_board[line[0].Item1, line[0].Item2] == symbol &&
                _board[line[1].Item1, line[1].Item2] == symbol &&
                _board[line[2].Item1, line[2].Item2] == symbol)
            {
                return line.ToList();
            }
        }

        return new List<(int, int)>();
    }

    private List<(int r, int c)> FindBlockedLine(int placedRow, int placedCol, PlayerSymbol playerSymbol)
    {
        var opponentSymbol = playerSymbol == PlayerSymbol.X ? PlayerSymbol.O : PlayerSymbol.X;

        var lines = new (int, int)[][]
        {
            new[] { (0,0), (0,1), (0,2) },
            new[] { (1,0), (1,1), (1,2) },
            new[] { (2,0), (2,1), (2,2) },
            new[] { (0,0), (1,0), (2,0) },
            new[] { (0,1), (1,1), (2,1) },
            new[] { (0,2), (1,2), (2,2) },
            new[] { (0,0), (1,1), (2,2) },
            new[] { (0,2), (1,1), (2,0) }
        };

        foreach (var line in lines)
        {
            if (!line.Any(pt => pt.Item1 == placedRow && pt.Item2 == placedCol))
                continue;

            int opponentCount = line.Count(pt => _board[pt.Item1, pt.Item2] == opponentSymbol);
            int playerCount = line.Count(pt => _board[pt.Item1, pt.Item2] == playerSymbol);

            if (opponentCount == 2 && playerCount == 1)
            {
                return line.ToList();
            }
        }

        foreach (var line in lines)
        {
            int opponentCount = line.Count(pt => _board[pt.Item1, pt.Item2] == opponentSymbol);
            int playerCount = line.Count(pt => _board[pt.Item1, pt.Item2] == playerSymbol);

            if (opponentCount == 2 && playerCount == 1)
            {
                return line.ToList();
            }
        }

        return new List<(int, int)>();
    }

    private void HighlightCompletedLine(int lastRow, int lastCol)
    {
        var theme = ThemeService.CurrentTheme;
        var task = _puzzle.Tasks[_currentTaskIndex];

        List<(int r, int c)> cellsToHighlight;

        if (_puzzle.Id == 2)
        {
            // 2-й уровень: подсветка заблокированной линии противника КРАСНЫМ
            cellsToHighlight = FindBlockedLine(lastRow, lastCol, task.PlayerSymbol);
        }
        else
        {
            // Другие уровни (1, 3, 4, 5): подсветка победной линии (в стиле главной страницы)
            cellsToHighlight = FindWinningLine(task.PlayerSymbol);
        }

        if (cellsToHighlight.Count == 0) return;

        bool isRed = _puzzle.Id == 2;
        bool isDark = theme.IsDark;

        Color highlightBg = isRed
            ? (isDark ? Color.FromArgb("#3D1E24") : Color.FromArgb("#FEE2E2"))
            : (isDark ? Color.FromArgb("#1E3A2B") : (theme.WinHighlightColor ?? Color.FromArgb("#E8F8D8")));

        Color highlightStroke = isRed
            ? Color.FromArgb("#EF4444")
            : Color.FromArgb("#58CC02");

        Color highlightBase = isRed
            ? (isDark ? Color.FromArgb("#991B1B") : Color.FromArgb("#DC2626"))
            : Color.FromArgb("#46A302");

        foreach (var (r, c) in cellsToHighlight)
        {
            if (r >= 0 && r < 3 && c >= 0 && c < 3)
            {
                var border = _cellBorders[r, c];
                var baseBorder = _cellBases[r, c];

                if (border != null)
                {
                    border.BackgroundColor = highlightBg;
                    border.Stroke = highlightStroke;
                    _ = border.ScaleToAsync(1.08, 140).ContinueWith(_ => border.ScaleToAsync(1.0, 140));
                }

                if (baseBorder != null)
                {
                    baseBorder.BackgroundColor = highlightBase;
                }
            }
        }
    }

    private void HandleTaskSolved(int lastRow, int lastCol)
    {
        _isSolved = true;

        // Подсвечиваем победную линию (или заблокированную линию красным на 2 уровне)
        HighlightCompletedLine(lastRow, lastCol);

        var theme = ThemeService.CurrentTheme;

        ImgMascot.Source = "happy.png";
        LblMascotText.Text = _puzzle.Id == 2
            ? "Suurepärane kaitse! Vastase võiduliin on edukalt blokeeritud!"
            : "Suurepärane! Leidsid ainsa õige lahenduse!";

        // Показ победной плашки в едином стиле с выбранной темой
        CardResult.BackgroundColor = theme.IsDark ? Color.FromArgb("#1B272C") : Color.FromArgb("#F0F9FF");
        CardResult.Stroke = theme.HeaderSurfaceColor;
        LblResultSubtitle.TextColor = theme.IsDark ? Color.FromArgb("#CBD5E1") : Color.FromArgb("#4B4B4B");
        LblResultSubtitle.IsVisible = true;
        BadgeMinusHeart.IsVisible = false;

        BtnNextAction.BackgroundColor = theme.ButtonBackground;
        BtnNextActionBase.BackgroundColor = theme.ButtonShadowColor;

        if (_currentTaskIndex < 4)
        {
            // Еще есть задачи в этом уровне (1..4 из 5)
            LblResultTitle.Text = _puzzle.Id == 2 ? "OHT BLOKEERITUD! 🛡" : "SUUREPÄRANE TÖÖ!";
            LblResultTitle.TextColor = theme.HeaderSurfaceColor;
            LblResultSubtitle.Text = $"Ülesanne {_currentTaskIndex + 1}/5 edukalt lahendatud!";
            LblNextAction.Text = "JÄRGMINE ÜLESANNE ➔";
            LblTurnIndicator.Text = _puzzle.Id == 2
                ? $"🛡 Oht blokeeritud! ({_currentTaskIndex + 1}/5)"
                : $"Ülesanne {_currentTaskIndex + 1}/5 läbitud!";
            LblTurnIndicator.TextColor = theme.HeaderSurfaceColor;
        }
        else
        {
            // Все 5 задач решены! Уровень полностью пройден
            AdventurePuzzleService.CompleteLevel(_puzzle.Id);

            LblResultTitle.Text = "TASE LÄBITUD! 🏆";
            LblResultTitle.TextColor = theme.HeaderSurfaceColor;
            LblResultSubtitle.Text = "Kõik 5 ülesannet edukalt lahendatud!";
            LblNextAction.Text = "TAGASI SEIKLUSSE ➔";
            LblTurnIndicator.Text = "🏆 Tase edukalt läbitud!";
            LblTurnIndicator.TextColor = theme.HeaderSurfaceColor;
        }

        CardResult.IsVisible = true;
        CardResult.Opacity = 0;
        CardResult.FadeToAsync(1.0, 250);
    }

    private async Task HandleWrongMove(int row, int col)
    {
        _isBusy = true;

        // Отнимаем 1 жизнь при ошибке
        LivesService.SpendLife();
        UpdateLivesUI();

        var theme = ThemeService.CurrentTheme;

        ImgMascot.Source = "sad.png";
        LblMascotText.Text = "Ups! See käik ei vii võiduni ja vastane pääseb. Proovi uuesti!";
        LblTurnIndicator.Text = "Vale käik! Proovi uuesti";
        LblTurnIndicator.TextColor = Color.FromArgb("#EA2B2B");

        // Тряска неверной клетки
        var border = _cellBorders[row, col];
        border.BackgroundColor = Color.FromArgb("#FFEAEA");
        border.Stroke = Color.FromArgb("#FF4B4B");

        await border.TranslateToAsync(-6, 0, 40);
        await border.TranslateToAsync(6, 0, 40);
        await border.TranslateToAsync(-4, 0, 40);
        await border.TranslateToAsync(4, 0, 40);
        await border.TranslateToAsync(0, 0, 40);

        // Показ плашки ошибки: заголовок "VALE KÄIK", иконка "-1 ❤️" и кнопка действия в цвете темы
        CardResult.BackgroundColor = theme.IsDark ? Color.FromArgb("#2D1B1F") : Color.FromArgb("#FFEAEA");
        CardResult.Stroke = Color.FromArgb("#FF4B4B");
        LblResultTitle.Text = "VALE KÄIK";
        LblResultTitle.TextColor = Color.FromArgb("#EA2B2B");
        LblResultSubtitle.IsVisible = false;
        BadgeMinusHeart.IsVisible = true;

        // Кнопка в цвете темы
        BtnNextAction.BackgroundColor = theme.ButtonBackground;
        BtnNextActionBase.BackgroundColor = theme.ButtonShadowColor;
        LblNextAction.Text = "PROOVI UUESTI";

        CardResult.IsVisible = true;
        CardResult.Opacity = 0;
        await CardResult.FadeToAsync(1.0, 200);

        _isBusy = false;
    }

    private void UpdateLivesUI()
    {
        LblLivesCount.Text = LivesService.CurrentLives.ToString();
    }

    private async void OnLivesClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new LivesPage());
    }

    private async void OnNextActionClicked(object? sender, EventArgs e)
    {
        if (BtnNextAction != null)
        {
            await BtnNextAction.TranslateToAsync(0, 3, 40, Easing.CubicIn);
            await BtnNextAction.TranslateToAsync(0, 0, 40, Easing.SpringOut);
        }

        if (!_isSolved)
        {
            // Если была ошибка и жизней не осталось
            if (LivesService.CurrentLives <= 0)
            {
                bool buyLives = await DisplayAlertAsync("Elud on otsas", "Uueks katseks on vaja vähemalt ühte elu. Kas soovid minna poodi elusid taastama?", "Mine poodi", "Tagasi seiklusse");
                if (buyLives)
                {
                    await Navigation.PushAsync(new LivesPage());
                }
                else
                {
                    await Navigation.PopAsync();
                }
                return;
            }

            // Повтор текущей задачи
            LoadTask(_currentTaskIndex);
            return;
        }

        // Если текущая задача была решена:
        if (_currentTaskIndex < 4)
        {
            // Переход к следующей типовой задаче этого уровня (2/5, 3/5, 4/5, 5/5)
            LoadTask(_currentTaskIndex + 1);
        }
        else
        {
            // Все 5 типовых задач решены! Возвращаем игрока обратно на страницу приключений,
            // где он видит золотой пройденный уровень и может выбрать следующий
            await Navigation.PopAsync();
        }
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        if (BtnBackBorder != null)
        {
            await BtnBackBorder.TranslateToAsync(0, 2, 40, Easing.CubicIn);
            await BtnBackBorder.TranslateToAsync(0, 0, 40, Easing.SpringOut);
        }
        await Navigation.PopAsync();
    }
}

