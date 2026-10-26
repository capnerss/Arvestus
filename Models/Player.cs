namespace TicTacToe.Models;

public class Player
{
    public string Name { get; set; }
    public PlayerSymbol Symbol { get; set; }
    public Color SymbolColor { get; set; }
    public bool IsBot { get; set; }

    public Player(string name, PlayerSymbol symbol, Color symbolColor, bool isBot = false)
    {
        Name = name;
        Symbol = symbol;
        SymbolColor = symbolColor;
        IsBot = isBot;
    }
}
