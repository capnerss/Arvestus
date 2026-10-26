using TicTacToe.Models;

namespace TicTacToe.Services;

/// <summary>
/// Ülesannete andmelao liides.
/// Eraldab andmebaasi teostuse (SQLite või tulevikus kaugbaas / REST API) ViewModelitest.
/// </summary>
public interface ITaskRepository
{
    Task<List<GameTask>> GetAllTasksAsync();
    Task<List<GameTask>> GetTasksByStatusAsync(bool isCompleted);
    Task<GameTask?> GetTaskByIdAsync(int id);
    Task<int> SaveTaskAsync(GameTask task);
    Task<int> DeleteTaskAsync(int id);
    Task<bool> ToggleCompleteAsync(int id);
    Task<int> SyncWithRemoteAsync(); // Valmidus kaugandmebaasi jaoks
}
