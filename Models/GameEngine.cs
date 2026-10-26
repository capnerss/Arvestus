namespace TicTacToe.Models;

public class GameEngine
{
    public int BoardSize { get; private set; } = 3;
    public int WinningStreak { get; private set; } = 3;
    public PlayerSymbol[,] Board { get; private set; } = new PlayerSymbol[3, 3];

    public Player Player1 { get; set; }
    public Player Player2 { get; set; }
    public PlayerSymbol CurrentTurn { get; private set; } = PlayerSymbol.X;
    public GameMode Mode { get; set; } = GameMode.TwoPlayers;

    public bool IsGameOver { get; private set; }
    public Player? Winner { get; private set; }
    public bool IsDraw { get; private set; }
    public List<(int Row, int Col)> WinningCells { get; private set; } = new();

    private readonly Random _random = new();

    public GameEngine(int boardSize = 3)
    {
        Player1 = new Player("Mängija 1", PlayerSymbol.X, Color.FromArgb("#2563EB"), isBot: false);
        Player2 = new Player("Mängija 2", PlayerSymbol.O, Color.FromArgb("#DC2626"), isBot: false);
        SetBoardSize(boardSize);
    }

    public void SetBoardSize(int size)
    {
        BoardSize = Math.Clamp(size, 3, 5);
        WinningStreak = BoardSize == 3 ? 3 : (BoardSize == 4 ? 4 : 4);
        Board = new PlayerSymbol[BoardSize, BoardSize];
        ResetGame(PlayerSymbol.X);
    }

    public void ResetGame(PlayerSymbol startingPlayer)
    {
        for (int r = 0; r < BoardSize; r++)
        {
            for (int c = 0; c < BoardSize; c++)
            {
                Board[r, c] = PlayerSymbol.None;
            }
        }

        CurrentTurn = startingPlayer;
        IsGameOver = false;
        Winner = null;
        IsDraw = false;
        WinningCells.Clear();
    }

    public Player CurrentPlayer => CurrentTurn == PlayerSymbol.X ? Player1 : Player2;

    public bool MakeMove(int row, int col)
    {
        if (IsGameOver) return false;
        if (row < 0 || row >= BoardSize || col < 0 || col >= BoardSize) return false;
        if (Board[row, col] != PlayerSymbol.None) return false;

        Board[row, col] = CurrentTurn;

        if (CheckWin(row, col, CurrentTurn, out var winningLine))
        {
            IsGameOver = true;
            Winner = CurrentPlayer;
            WinningCells = winningLine;
            return true;
        }

        if (IsBoardFull())
        {
            IsGameOver = true;
            IsDraw = true;
            return true;
        }

        CurrentTurn = (CurrentTurn == PlayerSymbol.X) ? PlayerSymbol.O : PlayerSymbol.X;
        return true;
    }

    public (int Row, int Col)? GetBotMove(string difficulty = "")
    {
        if (IsGameOver) return null;

        if (difficulty.StartsWith("Lihtne", StringComparison.OrdinalIgnoreCase))
        {
            return GetRandomMove();
        }

        if (difficulty.StartsWith("Meister", StringComparison.OrdinalIgnoreCase))
        {
            if (BoardSize == 3)
            {
                var minimaxMove = GetMinimaxMove();
                if (minimaxMove.HasValue) return minimaxMove;
            }
        }

        return GetSmartMove();
    }

    private (int Row, int Col)? GetSmartMove()
    {
        PlayerSymbol botSymbol = Player2.Symbol;
        PlayerSymbol opponentSymbol = Player1.Symbol;

        // 1. Kas bot saab võita ühe käiguga?
        var winMove = FindWinningMove(botSymbol);
        if (winMove.HasValue) return winMove;

        // 2. Kas vastane saab võita järgmise käiguga? Kui jah, blokeeri see!
        var blockMove = FindWinningMove(opponentSymbol);
        if (blockMove.HasValue) return blockMove;

        // 3. Eelistatud ruut: Keskkoht
        int center = BoardSize / 2;
        if (Board[center, center] == PlayerSymbol.None)
        {
            return (center, center);
        }

        // 4. Eelistatud ruudud: Nurgad
        var corners = new List<(int, int)>
        {
            (0, 0),
            (0, BoardSize - 1),
            (BoardSize - 1, 0),
            (BoardSize - 1, BoardSize - 1)
        };
        var availableCorners = corners.Where(c => Board[c.Item1, c.Item2] == PlayerSymbol.None).ToList();
        if (availableCorners.Count > 0)
        {
            return availableCorners[_random.Next(availableCorners.Count)];
        }

        return GetRandomMove();
    }

    private (int Row, int Col)? GetRandomMove()
    {
        var emptyCells = new List<(int, int)>();
        for (int r = 0; r < BoardSize; r++)
        {
            for (int c = 0; c < BoardSize; c++)
            {
                if (Board[r, c] == PlayerSymbol.None)
                {
                    emptyCells.Add((r, c));
                }
            }
        }

        if (emptyCells.Count > 0)
        {
            return emptyCells[_random.Next(emptyCells.Count)];
        }

        return null;
    }

    private (int Row, int Col)? GetMinimaxMove()
    {
        int bestScore = int.MinValue;
        (int Row, int Col)? bestMove = null;

        for (int r = 0; r < BoardSize; r++)
        {
            for (int c = 0; c < BoardSize; c++)
            {
                if (Board[r, c] == PlayerSymbol.None)
                {
                    Board[r, c] = Player2.Symbol;
                    int score = Minimax(0, false, int.MinValue, int.MaxValue);
                    Board[r, c] = PlayerSymbol.None;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = (r, c);
                    }
                }
            }
        }

        return bestMove;
    }

    private int Minimax(int depth, bool isMaximizing, int alpha, int beta)
    {
        if (CheckWinWithoutLine(Player2.Symbol)) return 10 - depth;
        if (CheckWinWithoutLine(Player1.Symbol)) return depth - 10;
        if (IsBoardFull() || depth >= 6) return 0;

        if (isMaximizing)
        {
            int maxEval = int.MinValue;
            for (int r = 0; r < BoardSize; r++)
            {
                for (int c = 0; c < BoardSize; c++)
                {
                    if (Board[r, c] == PlayerSymbol.None)
                    {
                        Board[r, c] = Player2.Symbol;
                        int eval = Minimax(depth + 1, false, alpha, beta);
                        Board[r, c] = PlayerSymbol.None;
                        maxEval = Math.Max(maxEval, eval);
                        alpha = Math.Max(alpha, eval);
                        if (beta <= alpha) break;
                    }
                }
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            for (int r = 0; r < BoardSize; r++)
            {
                for (int c = 0; c < BoardSize; c++)
                {
                    if (Board[r, c] == PlayerSymbol.None)
                    {
                        Board[r, c] = Player1.Symbol;
                        int eval = Minimax(depth + 1, true, alpha, beta);
                        Board[r, c] = PlayerSymbol.None;
                        minEval = Math.Min(minEval, eval);
                        beta = Math.Min(beta, eval);
                        if (beta <= alpha) break;
                    }
                }
            }
            return minEval;
        }
    }

    private bool CheckWinWithoutLine(PlayerSymbol symbol)
    {
        for (int r = 0; r < BoardSize; r++)
        {
            for (int c = 0; c < BoardSize; c++)
            {
                if (Board[r, c] == symbol && CheckWin(r, c, symbol, out _))
                    return true;
            }
        }
        return false;
    }

    private (int Row, int Col)? FindWinningMove(PlayerSymbol symbol)
    {
        for (int r = 0; r < BoardSize; r++)
        {
            for (int c = 0; c < BoardSize; c++)
            {
                if (Board[r, c] == PlayerSymbol.None)
                {
                    Board[r, c] = symbol;
                    bool wins = CheckWin(r, c, symbol, out _);
                    Board[r, c] = PlayerSymbol.None;

                    if (wins)
                    {
                        return (r, c);
                    }
                }
            }
        }

        return null;
    }

    private bool CheckWin(int lastR, int lastC, PlayerSymbol symbol, out List<(int Row, int Col)> winningLine)
    {
        winningLine = new List<(int Row, int Col)>();

        // 4 suunda: horisontaalne, vertikaalne, diagonaal 1, diagonaal 2
        int[][] directions = new int[][]
        {
            new int[] { 0, 1 },  // Horisontaal
            new int[] { 1, 0 },  // Vertikaal
            new int[] { 1, 1 },  // Diagonaal \
            new int[] { 1, -1 }  // Diagonaal /
        };

        foreach (var dir in directions)
        {
            var line = new List<(int Row, int Col)> { (lastR, lastC) };

            // Positiivne suund
            int r = lastR + dir[0];
            int c = lastC + dir[1];
            while (r >= 0 && r < BoardSize && c >= 0 && c < BoardSize && Board[r, c] == symbol)
            {
                line.Add((r, c));
                r += dir[0];
                c += dir[1];
            }

            // Negatiivne suund
            r = lastR - dir[0];
            c = lastC - dir[1];
            while (r >= 0 && r < BoardSize && c >= 0 && c < BoardSize && Board[r, c] == symbol)
            {
                line.Add((r, c));
                r -= dir[0];
                c -= dir[1];
            }

            if (line.Count >= WinningStreak)
            {
                winningLine = line;
                return true;
            }
        }

        return false;
    }

    public bool IsBoardFull()
    {
        for (int r = 0; r < BoardSize; r++)
        {
            for (int c = 0; c < BoardSize; c++)
            {
                if (Board[r, c] == PlayerSymbol.None)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
