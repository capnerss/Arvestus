using SQLite;
using TicTacToe.Models;

namespace TicTacToe.Services;

/// <summary>
/// SQLite andmebaasi konkreetne teostus kasutades sqlite-net-pcl asünkroonset ühendust.
/// </summary>
public class SqliteTaskRepository : ITaskRepository
{
    private static SQLiteAsyncConnection? _database;
    private static readonly SemaphoreSlim _initLock = new(1, 1);

    private static async Task<SQLiteAsyncConnection> GetDatabaseAsync()
    {
        if (_database != null)
            return _database;

        await _initLock.WaitAsync();
        try
        {
            if (_database == null)
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "tictactoe.db3");
                var db = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);
                await db.CreateTableAsync<GameTask>();

                // Esialgne andmete laadimine (seeding), kui tabel on tühi
                int count = await db.Table<GameTask>().CountAsync();
                if (count == 0)
                {
                    await SeedInitialTasksAsync(db);
                }

                _database = db;
            }
            return _database;
        }
        finally
        {
            _initLock.Release();
        }
    }

    private static async Task SeedInitialTasksAsync(SQLiteAsyncConnection db)
    {
        var initial = new List<GameTask>
        {
            new()
            {
                Title = "Võida esimene mäng",
                Description = "Saavuta võit 3x3 laual sõbra või roboti vastu.",
                Category = "Päevaülesanne",
                RewardCoins = 25,
                IsCompleted = false,
                IsSynced = false
            },
            new()
            {
                Title = "Nutika roboti väljakutse",
                Description = "Mängi vähemalt üks matš Nutika roboti vastu.",
                Category = "Treening",
                RewardCoins = 35,
                IsCompleted = false,
                IsSynced = false
            },
            new()
            {
                Title = "Meistri taseme viik",
                Description = "Hoia Meister-roboti rünnak ära ja vii mäng viiki.",
                Category = "Meistriklubi",
                RewardCoins = 50,
                IsCompleted = false,
                IsSynced = false
            },
            new()
            {
                Title = "Mündikoguja",
                Description = "Kogu poest või võitudest vähemalt 100 münti.",
                Category = "Päevaülesanne",
                RewardCoins = 30,
                IsCompleted = true,
                IsSynced = true
            }
        };

        await db.InsertAllAsync(initial);
    }

    public async Task<List<GameTask>> GetAllTasksAsync()
    {
        var db = await GetDatabaseAsync();
        return await db.Table<GameTask>().OrderByDescending(t => t.Id).ToListAsync();
    }

    public async Task<List<GameTask>> GetTasksByStatusAsync(bool isCompleted)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<GameTask>()
            .Where(t => t.IsCompleted == isCompleted)
            .OrderByDescending(t => t.Id)
            .ToListAsync();
    }

    public async Task<GameTask?> GetTaskByIdAsync(int id)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<GameTask>().FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<int> SaveTaskAsync(GameTask task)
    {
        var db = await GetDatabaseAsync();
        task.UpdatedAt = DateTime.UtcNow;
        task.IsSynced = false; // Vajab uuesti kaugbaasi saatmist

        if (task.Id != 0)
        {
            return await db.UpdateAsync(task);
        }
        else
        {
            task.CreatedAt = DateTime.UtcNow;
            return await db.InsertAsync(task);
        }
    }

    public async Task<int> DeleteTaskAsync(int id)
    {
        var db = await GetDatabaseAsync();
        return await db.DeleteAsync<GameTask>(id);
    }

    public async Task<bool> ToggleCompleteAsync(int id)
    {
        var db = await GetDatabaseAsync();
        var task = await db.Table<GameTask>().FirstOrDefaultAsync(t => t.Id == id);
        if (task == null) return false;

        task.IsCompleted = !task.IsCompleted;
        task.UpdatedAt = DateTime.UtcNow;
        task.IsSynced = false;
        await db.UpdateAsync(task);

        return task.IsCompleted;
    }

    /// <summary>
    /// Kaugandmebaasi (Cloud / REST API) sünkroonimise valmidus.
    /// Tulevikus saadetakse siit HTTP päringud kaugserverisse.
    /// </summary>
    public async Task<int> SyncWithRemoteAsync()
    {
        var db = await GetDatabaseAsync();
        var unsynced = await db.Table<GameTask>().Where(t => !t.IsSynced).ToListAsync();

        // Siia lisatakse tulevikus kaugserveri REST/GraphQL päring
        // Näiteks: await httpClient.PostAsJsonAsync("https://api.tictactoe.com/tasks/sync", unsynced);
        foreach (var task in unsynced)
        {
            task.IsSynced = true;
            await db.UpdateAsync(task);
        }

        return unsynced.Count;
    }
}
