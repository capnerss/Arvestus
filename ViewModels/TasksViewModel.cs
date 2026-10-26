using System.Collections.ObjectModel;
using System.Windows.Input;
using TicTacToe.Models;
using TicTacToe.Services;

namespace TicTacToe.ViewModels;

public class TasksViewModel : BaseViewModel
{
    private readonly ITaskRepository _repository;
    private bool _isBusy;
    private int _filterIndex = 0; // 0 = Kõik, 1 = Pooleli, 2 = Tehtud
    private int _totalCount;
    private int _completedCount;
    private int _coins;
    private string _statusMessage = string.Empty;

    // Redigeerimise / Lisamise väljad
    private int _editingTaskId;
    private string _inputTitle = string.Empty;
    private string _inputDescription = string.Empty;
    private int _inputReward = 25;
    private string _inputCategory = "Päevaülesanne";
    private bool _isEditorOpen;
    private string _editorHeader = "UUS ÜLESANNE";

    public ObservableCollection<GameTask> Tasks { get; } = new();

    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }
    public int FilterIndex
    {
        get => _filterIndex;
        set
        {
            if (SetProperty(ref _filterIndex, value))
            {
                _ = LoadTasksAsync();
            }
        }
    }

    public int TotalCount { get => _totalCount; set => SetProperty(ref _totalCount, value); }
    public int CompletedCount { get => _completedCount; set => SetProperty(ref _completedCount, value); }
    public int PendingCount => TotalCount - CompletedCount;
    public int Coins { get => _coins; set => SetProperty(ref _coins, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

    public int EditingTaskId { get => _editingTaskId; set => SetProperty(ref _editingTaskId, value); }
    public string InputTitle { get => _inputTitle; set => SetProperty(ref _inputTitle, value); }
    public string InputDescription { get => _inputDescription; set => SetProperty(ref _inputDescription, value); }
    public int InputReward { get => _inputReward; set => SetProperty(ref _inputReward, value); }
    public string InputCategory { get => _inputCategory; set => SetProperty(ref _inputCategory, value); }
    public bool IsEditorOpen { get => _isEditorOpen; set => SetProperty(ref _isEditorOpen, value); }
    public string EditorHeader { get => _editorHeader; set => SetProperty(ref _editorHeader, value); }

    public ICommand LoadTasksCommand { get; }
    public ICommand SetFilterCommand { get; }
    public ICommand OpenAddCommand { get; }
    public ICommand OpenEditCommand { get; }
    public ICommand CloseEditorCommand { get; }
    public ICommand SaveTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }
    public ICommand ToggleCompleteCommand { get; }
    public ICommand SyncRemoteCommand { get; }

    public TasksViewModel() : this(new SqliteTaskRepository()) { }

    public TasksViewModel(ITaskRepository repository)
    {
        _repository = repository;

        LoadTasksCommand = new AsyncRelayCommand(LoadTasksAsync);
        SetFilterCommand = new RelayCommand(p =>
        {
            if (p is int idx) FilterIndex = idx;
            else if (p is string s && int.TryParse(s, out int parsed)) FilterIndex = parsed;
        });

        OpenAddCommand = new RelayCommand(OpenAdd);
        OpenEditCommand = new RelayCommand(p =>
        {
            if (p is GameTask task) OpenEdit(task);
        });

        CloseEditorCommand = new RelayCommand(() => IsEditorOpen = false);
        SaveTaskCommand = new AsyncRelayCommand(SaveTaskAsync);
        DeleteTaskCommand = new AsyncRelayCommand(p =>
        {
            if (p is int id) return DeleteTaskAsync(id);
            if (p is GameTask t) return DeleteTaskAsync(t.Id);
            return Task.CompletedTask;
        });

        ToggleCompleteCommand = new AsyncRelayCommand(p =>
        {
            if (p is int id) return ToggleCompleteAsync(id);
            if (p is GameTask t) return ToggleCompleteAsync(t.Id);
            return Task.CompletedTask;
        });

        SyncRemoteCommand = new AsyncRelayCommand(SyncRemoteAsync);

        RefreshCoins();
    }

    public void RefreshCoins()
    {
        Coins = ShopService.Coins;
    }

    public async Task LoadTasksAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            RefreshCoins();

            List<GameTask> all = await _repository.GetAllTasksAsync();
            TotalCount = all.Count;
            CompletedCount = all.Count(t => t.IsCompleted);
            OnPropertyChanged(nameof(PendingCount));

            List<GameTask> filtered = FilterIndex switch
            {
                1 => all.Where(t => !t.IsCompleted).ToList(),
                2 => all.Where(t => t.IsCompleted).ToList(),
                _ => all
            };

            Tasks.Clear();
            foreach (var task in filtered)
            {
                Tasks.Add(task);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OpenAdd()
    {
        EditingTaskId = 0;
        InputTitle = string.Empty;
        InputDescription = string.Empty;
        InputReward = 25;
        InputCategory = "Päevaülesanne";
        EditorHeader = "UUS ÜLESANNE";
        IsEditorOpen = true;
    }

    private void OpenEdit(GameTask task)
    {
        EditingTaskId = task.Id;
        InputTitle = task.Title;
        InputDescription = task.Description;
        InputReward = task.RewardCoins;
        InputCategory = task.Category;
        EditorHeader = "MUUDA ÜLESANNET";
        IsEditorOpen = true;
    }

    public async Task SaveTaskAsync()
    {
        if (string.IsNullOrWhiteSpace(InputTitle))
        {
            StatusMessage = "Palun sisesta ülesande pealkiri!";
            return;
        }

        var task = new GameTask
        {
            Id = EditingTaskId,
            Title = InputTitle.Trim(),
            Description = InputDescription.Trim(),
            RewardCoins = Math.Max(5, InputReward),
            Category = string.IsNullOrWhiteSpace(InputCategory) ? "Päevaülesanne" : InputCategory,
            IsCompleted = false
        };

        if (EditingTaskId != 0)
        {
            var existing = await _repository.GetTaskByIdAsync(EditingTaskId);
            if (existing != null)
            {
                task.IsCompleted = existing.IsCompleted;
                task.RemoteId = existing.RemoteId;
                task.CreatedAt = existing.CreatedAt;
            }
        }

        await _repository.SaveTaskAsync(task);
        IsEditorOpen = false;
        await LoadTasksAsync();
    }

    public async Task DeleteTaskAsync(int id)
    {
        await _repository.DeleteTaskAsync(id);
        await LoadTasksAsync();
    }

    public async Task ToggleCompleteAsync(int id)
    {
        var task = await _repository.GetTaskByIdAsync(id);
        if (task == null) return;

        bool nowCompleted = await _repository.ToggleCompleteAsync(id);
        if (nowCompleted)
        {
            ShopService.AddCoins(task.RewardCoins);
            RefreshCoins();
        }

        await LoadTasksAsync();
    }

    public async Task SyncRemoteAsync()
    {
        int synced = await _repository.SyncWithRemoteAsync();
        StatusMessage = synced > 0 
            ? $"Sünkroonitud {synced} kirjet kaugbaasiga!" 
            : "Kõik kirjed on juba ajakohased.";
        await LoadTasksAsync();
    }
}
