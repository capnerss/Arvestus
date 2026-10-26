using TicTacToe.Models;
using TicTacToe.Services;

namespace TicTacToe.Views;

public partial class LivesPage : ContentPage
{
    private IDispatcherTimer? _timer;
    private double _currentProgress = 1.0;
    private bool _isNavigating = false;
    private readonly Action<GameTheme> _themeChangedHandler;
    private readonly Action _livesChangedHandler;
    private readonly EventHandler _timerTickHandler;

    public LivesPage()
    {
        InitializeComponent();

        _themeChangedHandler = (t) => MainThread.BeginInvokeOnMainThread(ApplyCurrentTheme);
        _livesChangedHandler = () => MainThread.BeginInvokeOnMainThread(RefreshUI);
        _timerTickHandler = (s, e) =>
        {
            LivesService.CheckRegeneration();
            RefreshUI();
        };

        ApplyCurrentTheme();

        ProgressBarTrackGrid.SizeChanged += (s, e) =>
        {
            UpdateProgressWidth();
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        ThemeService.OnThemeChanged += _themeChangedHandler;
        LivesService.OnLivesChanged += _livesChangedHandler;

        ApplyCurrentTheme();
        LivesService.CheckRegeneration();
        RefreshUI();

        StopTimer();
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += _timerTickHandler;
        _timer.Start();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        ThemeService.OnThemeChanged -= _themeChangedHandler;
        LivesService.OnLivesChanged -= _livesChangedHandler;

        StopTimer();
    }

    private void StopTimer()
    {
        if (_timer != null)
        {
            _timer.Tick -= _timerTickHandler;
            _timer.Stop();
            _timer = null;
        }
    }

    private void ApplyCurrentTheme()
    {
        var theme = ThemeService.CurrentTheme;
        ThemeService.ApplyToPage(this, LivesHeaderBorder, HdrShadow, LivesCloseBorder, LivesIconBorder);

        if (BtnGoToShop != null)
        {
            BtnGoToShop.BackgroundColor = theme.ButtonBackground;
        }
        if (BtnGoToShopShadow != null)
        {
            BtnGoToShopShadow.BackgroundColor = theme.ButtonShadowColor;
        }

        bool isDark = theme.IsDark;
        if (CardTimerBorder != null)
        {
            CardTimerBorder.BackgroundColor = isDark ? Color.FromArgb("#1B272C") : Colors.White;
            CardTimerBorder.Stroke = isDark ? Color.FromArgb("#2B383E") : Color.FromArgb("#E5E5E5");
        }
        if (CardTimerShadow != null)
        {
            CardTimerShadow.BackgroundColor = isDark ? Color.FromArgb("#131B1E") : Color.FromArgb("#E5E5E5");
        }
        if (LblTimerTitle != null)
        {
            LblTimerTitle.TextColor = isDark ? Color.FromArgb("#9CA3AF") : Color.FromArgb("#4B4B4B");
        }
        if (ProgressBarTrack != null)
        {
            ProgressBarTrack.BackgroundColor = isDark ? Color.FromArgb("#3D1E24") : Color.FromArgb("#FEE2E2");
        }
    }

    private void RefreshUI()
    {
        int lives = LivesService.CurrentLives;
        var theme = ThemeService.CurrentTheme;
        bool isDark = theme.IsDark;

        if (lives >= LivesService.MaxLives)
        {
            SetProgress(1.0);
            if (LblTimerTitle != null)
            {
                LblTimerTitle.Text = "Elude seisund:";
            }
            if (LblTimerCountdown != null)
            {
                LblTimerCountdown.Text = "KÕIK ELUD ON TÄIS (10/10)";
                LblTimerCountdown.TextColor = Color.FromArgb("#16A34A");
            }
            if (RegenBarFill != null)
            {
                RegenBarFill.BackgroundColor = Color.FromArgb("#FF4B4B");
            }
            if (ProgressBarTrack != null)
            {
                ProgressBarTrack.BackgroundColor = isDark ? Color.FromArgb("#3D1E24") : Color.FromArgb("#FEE2E2");
            }
        }
        else
        {
            int remainingSec = LivesService.GetSecondsUntilNextLife();
            double progress = LivesService.GetRegenProgress();

            SetProgress(progress);
            if (LblTimerTitle != null)
            {
                LblTimerTitle.Text = "Järgmise eluni:";
            }
            if (LblTimerCountdown != null)
            {
                LblTimerCountdown.Text = $"{remainingSec / 60:D2}:{remainingSec % 60:D2}";
                LblTimerCountdown.TextColor = Color.FromArgb("#FF4B4B");
            }
            if (RegenBarFill != null)
            {
                RegenBarFill.BackgroundColor = Color.FromArgb("#FF4B4B");
            }
            if (ProgressBarTrack != null)
            {
                ProgressBarTrack.BackgroundColor = isDark ? Color.FromArgb("#3D1E24") : Color.FromArgb("#FEE2E2");
            }
        }
    }

    private void SetProgress(double progress)
    {
        _currentProgress = Math.Clamp(progress, 0.0, 1.0);
        UpdateProgressWidth();
    }

    private void UpdateProgressWidth()
    {
        if (ProgressBarTrackGrid == null || RegenBarFill == null) return;

        double trackWidth = ProgressBarTrackGrid.Width;
        if (double.IsNaN(trackWidth) || double.IsInfinity(trackWidth) || trackWidth <= 0)
        {
            return;
        }

        double targetWidth = Math.Clamp(trackWidth * _currentProgress, 0, trackWidth);
        if (targetWidth > 0 && targetWidth < 14)
        {
            targetWidth = 14;
        }

        if (Math.Abs(RegenBarFill.WidthRequest - targetWidth) > 0.5)
        {
            RegenBarFill.WidthRequest = targetWidth;
        }
    }

    private async void OnGoToShopClicked(object? sender, EventArgs e)
    {
        if (_isNavigating) return;
        _isNavigating = true;

        try
        {
            if (BtnGoToShop != null)
            {
                await BtnGoToShop.TranslateToAsync(0, 4, 60, Easing.CubicIn);
                await BtnGoToShop.TranslateToAsync(0, 0, 60, Easing.CubicOut);
            }
            await Navigation.PushAsync(new ShopPage());
        }
        finally
        {
            _isNavigating = false;
        }
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        if (_isNavigating) return;
        _isNavigating = true;

        try
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("//MainPage");
            }
            else if (Navigation.NavigationStack.Count > 1)
            {
                await Navigation.PopAsync();
            }
            else
            {
                await Navigation.PopToRootAsync();
            }
        }
        finally
        {
            _isNavigating = false;
        }
    }
}

