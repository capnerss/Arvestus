using System.Diagnostics;
using TicTacToe.Models;
using TicTacToe.Services;

namespace TicTacToe.Views;

public enum GameResultType
{
    Win,
    Defeat,
    Draw
}

public partial class GameResultPage : ContentPage
{
    private readonly GameResultType _resultType;
    private readonly string _winnerName;
    private readonly string _winnerSymbol;
    private readonly bool _isVsBot;
    private readonly Action? _onPlayAgain;
    private readonly Action<GameTheme> _themeChangedHandler;
    private CancellationTokenSource? _animationCts;
    private bool _isNavigating;

    public GameResultPage(
        GameResultType resultType,
        string winnerName = "",
        string winnerSymbol = "",
        bool isVsBot = false,
        int coinsEarned = 0,
        int heartsSpent = 0,
        Action? onPlayAgain = null)
    {
        InitializeComponent();
        _resultType = resultType;
        _winnerName = winnerName;
        _winnerSymbol = winnerSymbol;
        _isVsBot = isVsBot;
        _onPlayAgain = onPlayAgain;

        _themeChangedHandler = (t) => MainThread.BeginInvokeOnMainThread(() => ApplyThemeStyles(t));

        SetupUI(coinsEarned, heartsSpent);
        ApplyThemeStyles(ThemeService.CurrentTheme);
        PrepareInitialAnimationState();
    }

    private void SetupUI(int coinsEarned, int heartsSpent)
    {
        LblCoinsEarned.Text = coinsEarned > 0 ? $"+{coinsEarned}" : $"{coinsEarned}";
        var theme = ThemeService.CurrentTheme;
        bool isDark = theme.IsDark;

        if (heartsSpent > 0)
        {
            LblHeartsSpent.Text = $"-{heartsSpent}";
            LblHeartsSpent.TextColor = isDark ? Color.FromArgb("#F87171") : Color.FromArgb("#DC2626");
            CardHeartsSpent.BackgroundColor = isDark ? Color.FromArgb("#3B1A1E") : Color.FromArgb("#FEF2F2");
            CardHeartsSpent.Stroke = isDark ? Color.FromArgb("#7F1D1D") : Color.FromArgb("#FECACA");
        }
        else
        {
            LblHeartsSpent.Text = "0";
            LblHeartsSpent.TextColor = isDark ? Color.FromArgb("#4ADE80") : Color.FromArgb("#16A34A");
            CardHeartsSpent.BackgroundColor = isDark ? Color.FromArgb("#162E20") : Color.FromArgb("#F0FDF4");
            CardHeartsSpent.Stroke = isDark ? Color.FromArgb("#166534") : Color.FromArgb("#DCFCE7");
        }
    }

    private void ApplyThemeStyles(GameTheme theme)
    {
        // 1. Page Background
        this.BackgroundColor = theme.PageBackground;

        bool isDark = theme.IsDark;

        // 2. Speech Bubble
        if (SpeechBubble != null)
        {
            SpeechBubble.BackgroundColor = theme.CardBackground;
            SpeechBubble.Stroke = theme.CardBorderColor;
        }
        if (SpeechMessage != null)
        {
            SpeechMessage.TextColor = theme.PrimaryTextColor;
        }

        // 3. Play Again Button (tactile 3D button in active theme)
        if (BtnPlayAgain != null)
        {
            BtnPlayAgain.BackgroundColor = theme.ButtonBackground;
        }
        if (BtnPlayAgainShadow != null)
        {
            BtnPlayAgainShadow.BackgroundColor = theme.ButtonShadowColor;
        }
        if (LblPlayAgain != null)
        {
            LblPlayAgain.TextColor = theme.ButtonTextColor;
        }

        // 4. Rewards Cards
        if (CardCoinsEarned != null)
        {
            CardCoinsEarned.BackgroundColor = isDark ? Color.FromArgb("#2E2210") : Color.FromArgb("#FFFBEB");
            CardCoinsEarned.Stroke = isDark ? Color.FromArgb("#784D12") : Color.FromArgb("#FCD34D");
        }
        if (LblCoinsEarned != null)
        {
            LblCoinsEarned.TextColor = isDark ? Color.FromArgb("#FBBF24") : Color.FromArgb("#B45309");
        }

        // 5. Result-specific styling (Banner & Mascot Aura)
        switch (_resultType)
        {
            case GameResultType.Win:
                if (HedgehogImage != null) HedgehogImage.Source = "happy.png";
                if (BannerTitle != null) BannerTitle.Text = "PALJU ÕNNE!";
                if (BannerSubtitle != null) BannerSubtitle.Text = _isVsBot ? "Alistasid roboti nutikate käikudega!" : $"Võitja: {_winnerName} ({_winnerSymbol})!";

                if (theme.Name == "Blue")
                {
                    if (BannerBorder != null) BannerBorder.BackgroundColor = Color.FromArgb("#1CB0F6");
                    if (BannerShadow != null) BannerShadow.BackgroundColor = Color.FromArgb("#0E88C7");
                    if (BannerTitle != null) BannerTitle.TextColor = Colors.White;
                    if (BannerSubtitle != null) BannerSubtitle.TextColor = Color.FromArgb("#E0F2FE");
                    if (AuraCircle != null) AuraCircle.BackgroundColor = Color.FromArgb("#E0F2FE");
                }
                else if (theme.Name == "Gold")
                {
                    if (BannerBorder != null) BannerBorder.BackgroundColor = Color.FromArgb("#FFC800");
                    if (BannerShadow != null) BannerShadow.BackgroundColor = Color.FromArgb("#E5A700");
                    if (BannerTitle != null) BannerTitle.TextColor = Color.FromArgb("#4A3700");
                    if (BannerSubtitle != null) BannerSubtitle.TextColor = Color.FromArgb("#785900");
                    if (AuraCircle != null) AuraCircle.BackgroundColor = Color.FromArgb("#FEF3C7");
                }
                else if (theme.Name == "Dark")
                {
                    if (BannerBorder != null) BannerBorder.BackgroundColor = Color.FromArgb("#1CB0F6");
                    if (BannerShadow != null) BannerShadow.BackgroundColor = Color.FromArgb("#0E88C7");
                    if (BannerTitle != null) BannerTitle.TextColor = Colors.White;
                    if (BannerSubtitle != null) BannerSubtitle.TextColor = Color.FromArgb("#BAE6FD");
                    if (AuraCircle != null) AuraCircle.BackgroundColor = Color.FromArgb("#1E2D34");
                }
                else // Green (Duolingo klassika)
                {
                    if (BannerBorder != null) BannerBorder.BackgroundColor = Color.FromArgb("#58CC02");
                    if (BannerShadow != null) BannerShadow.BackgroundColor = Color.FromArgb("#46A302");
                    if (BannerTitle != null) BannerTitle.TextColor = Colors.White;
                    if (BannerSubtitle != null) BannerSubtitle.TextColor = Color.FromArgb("#DCFCE7");
                    if (AuraCircle != null) AuraCircle.BackgroundColor = Color.FromArgb("#DCFCE7");
                }

                if (SpeechMessage != null)
                {
                    SpeechMessage.Text = _isVsBot
                        ? "«Vau, sa tegid seda! Suurepärane võit roboti üle! Sinu taktika oli laitmatu!»"
                        : $"«Suurepärane partii! Palju õnne, {_winnerName}! Ilus ja meisterlik võit!»";
                }
                break;

            case GameResultType.Defeat:
                if (HedgehogImage != null) HedgehogImage.Source = "sad.png";
                if (BannerTitle != null) BannerTitle.Text = "ÄRA HEIDA MEELT!";
                if (BannerSubtitle != null) BannerSubtitle.Text = "Seekord oli bot kiirem!";

                if (theme.Name == "Blue")
                {
                    if (BannerBorder != null) BannerBorder.BackgroundColor = Color.FromArgb("#0E88C7");
                    if (BannerShadow != null) BannerShadow.BackgroundColor = Color.FromArgb("#096596");
                    if (BannerTitle != null) BannerTitle.TextColor = Colors.White;
                    if (BannerSubtitle != null) BannerSubtitle.TextColor = Color.FromArgb("#E0F2FE");
                    if (AuraCircle != null) AuraCircle.BackgroundColor = Color.FromArgb("#E0F2FE");
                }
                else if (theme.Name == "Gold")
                {
                    if (BannerBorder != null) BannerBorder.BackgroundColor = Color.FromArgb("#D97706");
                    if (BannerShadow != null) BannerShadow.BackgroundColor = Color.FromArgb("#B45309");
                    if (BannerTitle != null) BannerTitle.TextColor = Colors.White;
                    if (BannerSubtitle != null) BannerSubtitle.TextColor = Color.FromArgb("#FEF3C7");
                    if (AuraCircle != null) AuraCircle.BackgroundColor = Color.FromArgb("#FEF3C7");
                }
                else if (theme.Name == "Dark")
                {
                    if (BannerBorder != null) BannerBorder.BackgroundColor = Color.FromArgb("#2B383E");
                    if (BannerShadow != null) BannerShadow.BackgroundColor = Color.FromArgb("#182024");
                    if (BannerTitle != null) BannerTitle.TextColor = Colors.White;
                    if (BannerSubtitle != null) BannerSubtitle.TextColor = Color.FromArgb("#94A3B8");
                    if (AuraCircle != null) AuraCircle.BackgroundColor = Color.FromArgb("#1E2529");
                }
                else // Green
                {
                    if (BannerBorder != null) BannerBorder.BackgroundColor = Color.FromArgb("#0E88C7");
                    if (BannerShadow != null) BannerShadow.BackgroundColor = Color.FromArgb("#096596");
                    if (BannerTitle != null) BannerTitle.TextColor = Colors.White;
                    if (BannerSubtitle != null) BannerSubtitle.TextColor = Color.FromArgb("#E0F2FE");
                    if (AuraCircle != null) AuraCircle.BackgroundColor = Color.FromArgb("#E0F2FE");
                }

                if (SpeechMessage != null)
                {
                    SpeechMessage.Text = "«Pole hullu! Harjutamine teeb meistriks. Järgmine kord läheb sul kindlasti paremini!»";
                }
                break;

            case GameResultType.Draw:
            default:
                if (HedgehogImage != null) HedgehogImage.Source = "neutral.png";
                if (BannerTitle != null) BannerTitle.Text = "SUUREPÄRANE LAHING!";
                if (BannerSubtitle != null) BannerSubtitle.Text = "Mäng lõppes võitlusliku viigiga!";

                if (theme.Name == "Gold")
                {
                    if (BannerBorder != null) BannerBorder.BackgroundColor = Color.FromArgb("#FFC800");
                    if (BannerShadow != null) BannerShadow.BackgroundColor = Color.FromArgb("#E5A700");
                    if (BannerTitle != null) BannerTitle.TextColor = Color.FromArgb("#4A3700");
                    if (BannerSubtitle != null) BannerSubtitle.TextColor = Color.FromArgb("#785900");
                    if (AuraCircle != null) AuraCircle.BackgroundColor = Color.FromArgb("#FEF3C7");
                }
                else if (theme.Name == "Dark")
                {
                    if (BannerBorder != null) BannerBorder.BackgroundColor = Color.FromArgb("#D97706");
                    if (BannerShadow != null) BannerShadow.BackgroundColor = Color.FromArgb("#92400E");
                    if (BannerTitle != null) BannerTitle.TextColor = Colors.White;
                    if (BannerSubtitle != null) BannerSubtitle.TextColor = Color.FromArgb("#FED7AA");
                    if (AuraCircle != null) AuraCircle.BackgroundColor = Color.FromArgb("#292115");
                }
                else // Blue / Green
                {
                    if (BannerBorder != null) BannerBorder.BackgroundColor = Color.FromArgb("#FF9600");
                    if (BannerShadow != null) BannerShadow.BackgroundColor = Color.FromArgb("#D97706");
                    if (BannerTitle != null) BannerTitle.TextColor = Colors.White;
                    if (BannerSubtitle != null) BannerSubtitle.TextColor = Color.FromArgb("#FFFBEB");
                    if (AuraCircle != null) AuraCircle.BackgroundColor = Color.FromArgb("#FEF3C7");
                }

                if (SpeechMessage != null)
                {
                    SpeechMessage.Text = "«Väga tubli! Tasavägine ja ülimalt põnev heitlus! Tõeliste taktikute mäng!»";
                }
                break;
        }
    }

    private void PrepareInitialAnimationState()
    {
        HedgehogImage.Opacity = 0;
        HedgehogImage.Scale = 0.5;
        HedgehogImage.TranslationY = 40;

        AuraCircle.Opacity = 0;
        AuraCircle.Scale = 0.6;

        SpeechBubble.Opacity = 0;
        SpeechBubble.Scale = 0.8;
        SpeechBubble.TranslationY = 20;

        ButtonsContainer.Opacity = 0;
        ButtonsContainer.TranslationY = 30;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        ThemeService.OnThemeChanged += _themeChangedHandler;
        ApplyThemeStyles(ThemeService.CurrentTheme);

        _animationCts?.Cancel();
        _animationCts = new CancellationTokenSource();

        try
        {
            // 1. Entrance animation
            await Task.WhenAll(
                HedgehogImage.FadeToAsync(1, 350, Easing.CubicOut),
                HedgehogImage.ScaleToAsync(1.15, 380, Easing.SpringOut),
                HedgehogImage.TranslateToAsync(0, 0, 380, Easing.SpringOut),
                AuraCircle.FadeToAsync(0.85, 400),
                AuraCircle.ScaleToAsync(1.05, 400, Easing.SpringOut)
            );

            await HedgehogImage.ScaleToAsync(1.0, 180, Easing.CubicIn);

            // 2. Speech bubble pops in
            await Task.WhenAll(
                SpeechBubble.FadeToAsync(1, 300, Easing.CubicOut),
                SpeechBubble.ScaleToAsync(1.0, 320, Easing.SpringOut),
                SpeechBubble.TranslateToAsync(0, 0, 300, Easing.CubicOut)
            );

            // 3. Buttons slide in
            await Task.WhenAll(
                ButtonsContainer.FadeToAsync(1, 240, Easing.CubicOut),
                ButtonsContainer.TranslateToAsync(0, 0, 240, Easing.CubicOut)
            );

            // 4. Start continuous celebratory/character animation
            _ = StartContinuousMascotAnimationAsync(_animationCts.Token);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[GameResultPage] Animation error: {ex.Message}");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ThemeService.OnThemeChanged -= _themeChangedHandler;

        _animationCts?.Cancel();
        _animationCts = null;
    }

    private async Task StartContinuousMascotAnimationAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                if (_resultType == GameResultType.Win)
                {
                    // Victory jump & wobble
                    await Task.WhenAll(
                        HedgehogImage.TranslateToAsync(0, -18, 280, Easing.CubicOut),
                        HedgehogImage.RotateToAsync(8, 280, Easing.SinInOut),
                        AuraCircle.ScaleToAsync(1.2, 280, Easing.CubicOut)
                    );

                    if (token.IsCancellationRequested) break;

                    await Task.WhenAll(
                        HedgehogImage.TranslateToAsync(0, 0, 240, Easing.BounceOut),
                        HedgehogImage.RotateToAsync(-8, 240, Easing.SinInOut),
                        AuraCircle.ScaleToAsync(0.95, 240, Easing.SinInOut)
                    );

                    if (token.IsCancellationRequested) break;

                    await Task.WhenAll(
                        HedgehogImage.RotateToAsync(0, 200, Easing.CubicOut),
                        AuraCircle.ScaleToAsync(1.05, 200, Easing.CubicOut)
                    );

                    await Task.Delay(400, token);
                }
                else if (_resultType == GameResultType.Defeat)
                {
                    // Encouraging gentle pulse and comforting sway
                    await Task.WhenAll(
                        HedgehogImage.ScaleToAsync(1.05, 750, Easing.SinInOut),
                        HedgehogImage.RotateToAsync(4, 750, Easing.SinInOut),
                        AuraCircle.ScaleToAsync(1.1, 750, Easing.SinInOut)
                    );

                    if (token.IsCancellationRequested) break;

                    await Task.WhenAll(
                        HedgehogImage.ScaleToAsync(1.0, 750, Easing.SinInOut),
                        HedgehogImage.RotateToAsync(-4, 750, Easing.SinInOut),
                        AuraCircle.ScaleToAsync(0.95, 750, Easing.SinInOut)
                    );

                    if (token.IsCancellationRequested) break;

                    await HedgehogImage.RotateToAsync(0, 400, Easing.SinInOut);
                    await Task.Delay(300, token);
                }
                else // Draw
                {
                    // Friendly curious bobbing & nod
                    await Task.WhenAll(
                        HedgehogImage.TranslateToAsync(0, -10, 350, Easing.SinInOut),
                        HedgehogImage.RotateToAsync(5, 350, Easing.SinInOut)
                    );

                    if (token.IsCancellationRequested) break;

                    await Task.WhenAll(
                        HedgehogImage.TranslateToAsync(0, 0, 300, Easing.SinInOut),
                        HedgehogImage.RotateToAsync(-5, 300, Easing.SinInOut)
                    );

                    if (token.IsCancellationRequested) break;

                    await HedgehogImage.RotateToAsync(0, 250, Easing.SinInOut);
                    await Task.Delay(500, token);
                }
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GameResultPage] Continuous animation loop error: {ex.Message}");
                break;
            }
        }
    }

    private async void OnPlayAgainClicked(object? sender, EventArgs e)
    {
        if (_isNavigating) return;
        _isNavigating = true;

        try
        {
            if (BtnPlayAgain != null)
            {
                await BtnPlayAgain.TranslateToAsync(0, 4, 60, Easing.CubicIn);
                await BtnPlayAgain.TranslateToAsync(0, 0, 60, Easing.CubicOut);
            }

            _animationCts?.Cancel();
            _onPlayAgain?.Invoke();
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[GameResultPage] OnPlayAgain error: {ex.Message}");
        }
        finally
        {
            _isNavigating = false;
        }
    }
}

