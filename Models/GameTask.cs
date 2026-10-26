using SQLite;

namespace TicTacToe.Models;

/// <summary>
/// Mängu ülesande/missiooni andmemudel SQLite andmebaasi jaoks.
/// Sisaldab globaalset RemoteId ja sünkroonimisvälju tulevase kaugandmebaasi toe jaoks.
/// </summary>
[Table("GameTasks")]
public class GameTask
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Globaalne unikaalne ID (UUID), mis võimaldab kirjet seostada kaugandmebaasiga (Cloud/Remote DB).
    /// </summary>
    [Indexed]
    public string RemoteId { get; set; } = Guid.NewGuid().ToString();

    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Category { get; set; } = "Päevaülesanne"; // Päevaülesanne, Treening, Meistriklubi

    public int RewardCoins { get; set; } = 25;

    public bool IsCompleted { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Märge, kas kirje muudatused on kaugserverisse sünkroniseeritud.
    /// </summary>
    public bool IsSynced { get; set; } = false;
}
