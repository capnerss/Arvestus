using TicTacToe.Services;

namespace TicTacToe.Views;

public partial class ProfilePage : ContentPage
{
    private string _selectedAvatar;
    private readonly Border[] _avatarBorders;
    private readonly Label[] _lockLabels;

    public ProfilePage()
    {
        InitializeComponent();

        _selectedAvatar = UserProfileService.PlayerAvatar;
        _avatarBorders = new[]
        {
            BorderAvatar0,
            BorderAvatar1,
            BorderAvatar2,
            BorderAvatar3,
            BorderAvatar4,
            BorderAvatar5
        };
        _lockLabels = new[]
        {
            LockAvatar0,
            LockAvatar1,
            LockAvatar2,
            LockAvatar3,
            LockAvatar4,
            LockAvatar5
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ApplyCurrentTheme();
        LoadProfileData();
    }

    private void ApplyCurrentTheme()
    {
        var theme = ThemeService.CurrentTheme;
        ThemeService.ApplyToPage(this, ProfileHeaderBorder, null, ProfileCloseBorder, ProfileGearBorder);

        if (CurrentAvatarBorder != null)
        {
            CurrentAvatarBorder.Stroke = theme.HeaderSurfaceColor;
        }
        if (BtnSaveBorder != null)
        {
            BtnSaveBorder.BackgroundColor = theme.ButtonBackground;
        }
        if (SheetBuyButtonBorder != null)
        {
            SheetBuyButtonBorder.BackgroundColor = theme.ButtonBackground;
        }

        bool isDark = theme.IsDark;
        if (BottomSheetCard != null)
        {
            BottomSheetCard.BackgroundColor = isDark ? Color.FromArgb("#1B272C") : Colors.White;
            BottomSheetCard.Stroke = isDark ? Color.FromArgb("#2B383E") : Color.FromArgb("#E5E5E5");
        }
        if (SheetTitle != null)
        {
            SheetTitle.TextColor = isDark ? Colors.White : Color.FromArgb("#2B2B2B");
        }
        if (SheetMessage != null)
        {
            SheetMessage.TextColor = isDark ? Color.FromArgb("#98A2B3") : Color.FromArgb("#6B7280");
        }
    }

    private void LoadProfileData()
    {
        _selectedAvatar = UserProfileService.PlayerAvatar;
        TxtPlayerName.Text = UserProfileService.PlayerName;
        ImgCurrentAvatar.Source = _selectedAvatar;

        UpdateAvatarSelectionUI();
        LoadFavoriteBadges();
    }

    private Action? _pendingPurchaseAction;

    private static (string Name, int Price) GetAvatarInfo(string avatarFile)
    {
        return avatarFile switch
        {
            "karu2.png" => ("Karu", 150),
            "avatar_bear.png" => ("Karu", 150),
            "kass2.png" => ("Kass", 150),
            "robot2.png" => ("Robot", 250),
            "star2.png" => ("Täht", 300),
            "avatar_cat.png" => ("Kass", 150),
            "avatar_robot.png" => ("Robot", 250),
            "avatar_star.png" => ("Täht", 300),
            "rebane2.png" => ("Rebane", 0),
            "janes2.png" => ("Jänes", 0),
            _ => ("Jänes", 0)
        };
    }

    private async void OnAvatarTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is string avatar)
        {
            if (!ShopService.IsAvatarUnlocked(avatar))
            {
                var info = GetAvatarInfo(avatar);
                if (ShopService.Coins < info.Price)
                {
                    await ShowAlertBottomSheetAsync(
                        "Münte pole piisavalt",
                        $"Sul on {ShopService.Coins} münti, kuid vaja on {info.Price} münti! Mängi ja võida mänge müntide teenimiseks.",
                        "duo_coin.png",
                        Color.FromArgb("#FEF2F2"),
                        Color.FromArgb("#FECACA"));
                    return;
                }

                var theme = ThemeService.CurrentTheme;
                await ShowConfirmBottomSheetAsync(
                    $"Osta avatar «{info.Name}»?",
                    $"Hind: {info.Price} münti (sul jääb alles {ShopService.Coins - info.Price} münti)",
                    avatar,
                    theme.FooterActiveTabBg,
                    theme.HeaderSurfaceColor,
                    $"OSTA ({info.Price} 🪙)",
                    () =>
                    {
                        if (ShopService.BuyAvatar(avatar, info.Price))
                        {
                            _selectedAvatar = avatar;
                            ImgCurrentAvatar.Source = avatar;
                            UpdateAvatarSelectionUI();
                        }
                    });
                return;
            }

            _selectedAvatar = avatar;
            ImgCurrentAvatar.Source = avatar;
            UpdateAvatarSelectionUI();
        }
    }

    private async Task ShowConfirmBottomSheetAsync(
        string title,
        string message,
        string iconSource,
        Color iconBg,
        Color iconStroke,
        string buyButtonText,
        Action onConfirm)
    {
        _pendingPurchaseAction = onConfirm;

        SheetTitle.Text = title;
        SheetMessage.Text = message;
        SheetIconImage.Source = iconSource;
        SheetIconBorder.BackgroundColor = iconBg;
        SheetIconBorder.Stroke = iconStroke;
        LblSheetBuyButton.Text = buyButtonText;

        ApplyCurrentTheme();

        SheetConfirmActions.IsVisible = true;
        SheetAlertActions.IsVisible = false;

        await AnimateOpenBottomSheetAsync();
    }

    private async Task ShowAlertBottomSheetAsync(
        string title,
        string message,
        string iconSource,
        Color iconBg,
        Color iconStroke)
    {
        _pendingPurchaseAction = null;

        SheetTitle.Text = title;
        SheetMessage.Text = message;
        SheetIconImage.Source = iconSource;
        SheetIconBorder.BackgroundColor = iconBg;
        SheetIconBorder.Stroke = iconStroke;

        ApplyCurrentTheme();

        SheetConfirmActions.IsVisible = false;
        SheetAlertActions.IsVisible = true;

        await AnimateOpenBottomSheetAsync();
    }

    private async Task AnimateOpenBottomSheetAsync()
    {
        try
        {
            BottomSheetOverlay.Opacity = 0;
            BottomSheetOverlay.IsVisible = true;
            BottomSheetCard.TranslationY = 320;

            await Task.WhenAll(
                BottomSheetOverlay.FadeToAsync(1, 180, Easing.CubicOut),
                BottomSheetCard.TranslateToAsync(0, 0, 240, Easing.CubicOut)
            );
        }
        catch
        {
            // Ignore animation cancellation
        }
    }

    private async Task AnimateCloseBottomSheetAsync()
    {
        try
        {
            await Task.WhenAll(
                BottomSheetOverlay.FadeToAsync(0, 160, Easing.CubicIn),
                BottomSheetCard.TranslateToAsync(0, 320, 180, Easing.CubicIn)
            );
            BottomSheetOverlay.IsVisible = false;
            _pendingPurchaseAction = null;
        }
        catch
        {
            BottomSheetOverlay.IsVisible = false;
            _pendingPurchaseAction = null;
        }
    }

    private async void OnBottomSheetScrimTapped(object? sender, EventArgs e)
    {
        await AnimateCloseBottomSheetAsync();
    }

    private async void OnCancelPurchaseClicked(object? sender, EventArgs e)
    {
        await AnimateCloseBottomSheetAsync();
    }

    private async void OnCloseAlertSheetClicked(object? sender, EventArgs e)
    {
        await AnimateCloseBottomSheetAsync();
    }

    private async void OnConfirmPurchaseClicked(object? sender, EventArgs e)
    {
        var action = _pendingPurchaseAction;
        await AnimateCloseBottomSheetAsync();
        action?.Invoke();
    }

    private void UpdateAvatarSelectionUI()
    {
        var theme = ThemeService.CurrentTheme;
        bool isDark = theme.IsDark;

        for (int i = 0; i < UserProfileService.AvailableAvatars.Count && i < _avatarBorders.Length; i++)
        {
            string avatar = UserProfileService.AvailableAvatars[i];
            bool isUnlocked = ShopService.IsAvatarUnlocked(avatar);
            bool isSelected = avatar == _selectedAvatar;

            _avatarBorders[i].Stroke = isSelected ? theme.HeaderSurfaceColor : (isDark ? Color.FromArgb("#2B383E") : Color.FromArgb("#E5E5E5"));
            _avatarBorders[i].StrokeThickness = isSelected ? 3 : 2;
            _avatarBorders[i].BackgroundColor = isSelected ? theme.FooterActiveTabBg : (isDark ? Color.FromArgb("#1B272C") : Color.FromArgb("#F7F7F7"));
            _avatarBorders[i].Opacity = isUnlocked ? 1.0 : 0.45;

            if (i < _lockLabels.Length)
            {
                _lockLabels[i].IsVisible = !isUnlocked;
            }
        }
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        string newName = TxtPlayerName.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(newName))
        {
            await DisplayAlertAsync("Tähelepanu", "Mängija nimi ei saa olla tühi!", "Selge");
            return;
        }

        UserProfileService.PlayerName = newName;
        UserProfileService.PlayerAvatar = _selectedAvatar;

        await Navigation.PushAsync(new ProfileSavedPage(newName, _selectedAvatar));
    }

    private async void OnBackButtonClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnCloseClicked(object? sender, EventArgs e)
    {
        if (Navigation.NavigationStack.Count > 1)
        {
            await Navigation.PopToRootAsync();
        }
        else if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage());
    }

    // ==========================================
    // DRAG & DROP JA TAP-TO-PLACE VITRIIN
    // ==========================================

    private string? _selectedBadgeForPlacement;
    private static readonly string[] Badges = { "🦔", "🏆", "⭐", "👑", "🎯", "⚡" };

    private void LoadFavoriteBadges()
    {
        LblSlot0.Text = UserProfileService.GetFavoriteBadge(0);
        LblSlot1.Text = UserProfileService.GetFavoriteBadge(1);
        LblSlot2.Text = UserProfileService.GetFavoriteBadge(2);
    }

    private void OnBadgeDragOver(object? sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Copy;
    }

    private void HandleBadgeDragStarting(string badge, DragStartingEventArgs e)
    {
        _selectedBadgeForPlacement = badge;
        e.Data.Text = badge;
        e.Data.Properties["Badge"] = badge;
    }

    private void OnBadge0DragStarting(object? sender, DragStartingEventArgs e) => HandleBadgeDragStarting(Badges[0], e);
    private void OnBadge1DragStarting(object? sender, DragStartingEventArgs e) => HandleBadgeDragStarting(Badges[1], e);
    private void OnBadge2DragStarting(object? sender, DragStartingEventArgs e) => HandleBadgeDragStarting(Badges[2], e);
    private void OnBadge3DragStarting(object? sender, DragStartingEventArgs e) => HandleBadgeDragStarting(Badges[3], e);
    private void OnBadge4DragStarting(object? sender, DragStartingEventArgs e) => HandleBadgeDragStarting(Badges[4], e);
    private void OnBadge5DragStarting(object? sender, DragStartingEventArgs e) => HandleBadgeDragStarting(Badges[5], e);

    private async void OnBadgeDropToSlot0(object? sender, DropEventArgs e) => await PlaceBadgeIntoSlotAsync(0, SlotBorder0, LblSlot0, e);
    private async void OnBadgeDropToSlot1(object? sender, DropEventArgs e) => await PlaceBadgeIntoSlotAsync(1, SlotBorder1, LblSlot1, e);
    private async void OnBadgeDropToSlot2(object? sender, DropEventArgs e) => await PlaceBadgeIntoSlotAsync(2, SlotBorder2, LblSlot2, e);

    private async Task PlaceBadgeIntoSlotAsync(int slotIndex, Border slotBorder, Label slotLabel, DropEventArgs? dropArgs = null)
    {
        string badge = _selectedBadgeForPlacement ?? string.Empty;
        if (dropArgs != null && dropArgs.Data.Properties.TryGetValue("Badge", out var obj) && obj is string b && !string.IsNullOrEmpty(b))
        {
            badge = b;
        }

        if (string.IsNullOrEmpty(badge))
            return;

        slotLabel.Text = badge;
        UserProfileService.SetFavoriteBadge(slotIndex, badge);
        AudioService.PlayTap();

        // 3D анимация приземления значка на пьедестал
        await slotBorder.ScaleToAsync(1.2, 80, Easing.CubicOut);
        await slotBorder.ScaleToAsync(1.0, 70, Easing.CubicIn);

        HighlightSelectedBadge(null);
        _selectedBadgeForPlacement = null;
    }

    private void OnBadge0Tapped(object? sender, EventArgs e) => SelectBadgeForPlacement(Badges[0], BadgeItem0);
    private void OnBadge1Tapped(object? sender, EventArgs e) => SelectBadgeForPlacement(Badges[1], BadgeItem1);
    private void OnBadge2Tapped(object? sender, EventArgs e) => SelectBadgeForPlacement(Badges[2], BadgeItem2);
    private void OnBadge3Tapped(object? sender, EventArgs e) => SelectBadgeForPlacement(Badges[3], BadgeItem3);
    private void OnBadge4Tapped(object? sender, EventArgs e) => SelectBadgeForPlacement(Badges[4], BadgeItem4);
    private void OnBadge5Tapped(object? sender, EventArgs e) => SelectBadgeForPlacement(Badges[5], BadgeItem5);

    private void SelectBadgeForPlacement(string badge, Border badgeBorder)
    {
        _selectedBadgeForPlacement = badge;
        AudioService.PlayTap();
        HighlightSelectedBadge(badgeBorder);
    }

    private void HighlightSelectedBadge(Border? selectedBorder)
    {
        var theme = ThemeService.CurrentTheme;
        Border[] allBadges = { BadgeItem0, BadgeItem1, BadgeItem2, BadgeItem3, BadgeItem4, BadgeItem5 };
        foreach (var b in allBadges)
        {
            if (b == selectedBorder)
            {
                b.Stroke = theme.HeaderSurfaceColor;
                b.StrokeThickness = 2.5;
                b.BackgroundColor = theme.FooterActiveTabBg;
            }
            else
            {
                b.Stroke = Color.FromArgb("#E5E5E5");
                b.StrokeThickness = 1.5;
                b.BackgroundColor = Color.FromArgb("#F7F7F7");
            }
        }
    }

    private async void OnSlot0Tapped(object? sender, EventArgs e)
    {
        if (_selectedBadgeForPlacement != null)
        {
            await PlaceBadgeIntoSlotAsync(0, SlotBorder0, LblSlot0);
        }
        else
        {
            await SlotBorder0.ScaleToAsync(1.1, 70);
            await SlotBorder0.ScaleToAsync(1.0, 70);
        }
    }

    private async void OnSlot1Tapped(object? sender, EventArgs e)
    {
        if (_selectedBadgeForPlacement != null)
        {
            await PlaceBadgeIntoSlotAsync(1, SlotBorder1, LblSlot1);
        }
        else
        {
            await SlotBorder1.ScaleToAsync(1.1, 70);
            await SlotBorder1.ScaleToAsync(1.0, 70);
        }
    }

    private async void OnSlot2Tapped(object? sender, EventArgs e)
    {
        if (_selectedBadgeForPlacement != null)
        {
            await PlaceBadgeIntoSlotAsync(2, SlotBorder2, LblSlot2);
        }
        else
        {
            await SlotBorder2.ScaleToAsync(1.1, 70);
            await SlotBorder2.ScaleToAsync(1.0, 70);
        }
    }
}

