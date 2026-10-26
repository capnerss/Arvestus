namespace TicTacToe.Services;

public static class LivesService
{
    private const string KeyCurrentLives = "Lives_Current";
    private const string KeyLastRegenUtc = "Lives_LastRegenUtc";

    public const int MaxLives = 10;
    public const int RegenIntervalMinutes = 10;

    public static event Action? OnLivesChanged;

    public static int CurrentLives
    {
        get
        {
            CheckRegeneration();
            return Preferences.Default.Get(KeyCurrentLives, MaxLives);
        }
        private set
        {
            Preferences.Default.Set(KeyCurrentLives, Math.Clamp(value, 0, MaxLives));
            OnLivesChanged?.Invoke();
        }
    }

    public static DateTime LastRegenUtc
    {
        get
        {
            string saved = Preferences.Default.Get(KeyLastRegenUtc, string.Empty);
            if (DateTime.TryParse(saved, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dt))
            {
                return dt;
            }
            return DateTime.UtcNow;
        }
        private set
        {
            Preferences.Default.Set(KeyLastRegenUtc, value.ToString("o"));
        }
    }

    public static void CheckRegeneration()
    {
        int lives = Preferences.Default.Get(KeyCurrentLives, MaxLives);
        if (lives >= MaxLives)
        {
            LastRegenUtc = DateTime.UtcNow;
            return;
        }

        var now = DateTime.UtcNow;
        if (now < LastRegenUtc)
        {
            LastRegenUtc = now;
            return;
        }

        var elapsed = now - LastRegenUtc;
        if (elapsed.TotalMinutes >= RegenIntervalMinutes)
        {
            int recovered = (int)(elapsed.TotalMinutes / RegenIntervalMinutes);
            int newLives = Math.Min(MaxLives, lives + recovered);
            Preferences.Default.Set(KeyCurrentLives, newLives);

            if (newLives >= MaxLives)
            {
                LastRegenUtc = now;
            }
            else
            {
                LastRegenUtc = LastRegenUtc.AddMinutes(recovered * RegenIntervalMinutes);
            }

            OnLivesChanged?.Invoke();
        }
    }

    public static int GetSecondsUntilNextLife()
    {
        CheckRegeneration();
        int lives = Preferences.Default.Get(KeyCurrentLives, MaxLives);
        if (lives >= MaxLives)
            return 0;

        var elapsed = DateTime.UtcNow - LastRegenUtc;
        if (elapsed.TotalSeconds < 0)
        {
            LastRegenUtc = DateTime.UtcNow;
            return RegenIntervalMinutes * 60;
        }

        double remainingSec = (RegenIntervalMinutes * 60) - (elapsed.TotalSeconds % (RegenIntervalMinutes * 60));
        return Math.Clamp((int)remainingSec, 0, RegenIntervalMinutes * 60);
    }

    public static double GetRegenProgress()
    {
        int lives = Preferences.Default.Get(KeyCurrentLives, MaxLives);
        if (lives >= MaxLives)
            return 1.0;

        int remaining = GetSecondsUntilNextLife();
        double progress = 1.0 - (remaining / (double)(RegenIntervalMinutes * 60));
        return Math.Clamp(progress, 0.0, 1.0);
    }

    public static bool SpendLife()
    {
        CheckRegeneration();
        int lives = Preferences.Default.Get(KeyCurrentLives, MaxLives);
        if (lives <= 0)
            return false;

        if (lives >= MaxLives)
        {
            LastRegenUtc = DateTime.UtcNow;
        }

        CurrentLives = lives - 1;
        return true;
    }

    public static void AddLives(int count)
    {
        CheckRegeneration();
        int lives = Preferences.Default.Get(KeyCurrentLives, MaxLives);
        int newLives = Math.Min(MaxLives, lives + count);
        CurrentLives = newLives;

        if (newLives >= MaxLives)
        {
            LastRegenUtc = DateTime.UtcNow;
        }
    }

    public static void FillLives()
    {
        CurrentLives = MaxLives;
        LastRegenUtc = DateTime.UtcNow;
    }
}
