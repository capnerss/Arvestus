namespace TicTacToe.Models;

public class PuzzleStep
{
    public int ExpectedRow { get; set; }
    public int ExpectedCol { get; set; }
    public int BotResponseRow { get; set; } = -1;
    public int BotResponseCol { get; set; } = -1;
}

public class PuzzleTask
{
    public int TaskNumber { get; set; } = 1;
    public string Title { get; set; } = string.Empty;
    public string Instruction { get; set; } = string.Empty;
    public PlayerSymbol PlayerSymbol { get; set; } = PlayerSymbol.X;
    public PlayerSymbol[,] InitialBoard { get; set; } = new PlayerSymbol[3, 3];
    public List<PuzzleStep> SolutionSteps { get; set; } = new();
}

public class AdventurePuzzle
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Hint { get; set; } = string.Empty;
    public string BadgeText { get; set; } = string.Empty;
    public PlayerSymbol PlayerSymbol { get; set; } = PlayerSymbol.X;
    public string ThemeColor { get; set; } = "#58CC02";
    public string ThemeShadow { get; set; } = "#46A302";
    public string Icon { get; set; } = "★";
    public int RewardCoins { get; set; } = 50;
    public int RewardHearts { get; set; } = 0;
    public int DifficultyStars { get; set; } = 1;

    // 5 типовых задач для каждого уровня
    public List<PuzzleTask> Tasks { get; set; } = new();

    // Свойства совместимости с первой задачей
    public PlayerSymbol[,] InitialBoard => Tasks.Count > 0 ? Tasks[0].InitialBoard : new PlayerSymbol[3, 3];
    public List<PuzzleStep> SolutionSteps => Tasks.Count > 0 ? Tasks[0].SolutionSteps : new();

    public bool IsBoss => Id == 5;
}
