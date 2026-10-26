using TicTacToe.Models;
using TicTacToe.Services;

namespace TicTacToe.Views;

public partial class ShopPage : ContentPage
{
    private record AvatarShopItem(string File, string Name, int Price);
    private record ThemeShopItem(string Id, string DisplayName, int Price, string AccentColor);

    private readonly List<AvatarShopItem> _avatarItems = new()
    {
        new("janes2.png", "Jänes", 0),
        new("rebane2.png", "Rebane", 0),
        new("karu2.png", "Karu", 150),
        new("kass2.png", "Kass", 150),
        new("robot2.png", "Robot", 250),
        new("star2.png", "Täht", 300),
    };

    private readonly List<ThemeShopItem> _themeItems = new()
    {
        new("Blue", "Sinine", 0, "#1CB0F6"),
        new("Dark", "Must", 0, "#2B383E"),
        new("Gold", "Kuld", 200, "#FFC800"),
        new("Green", "Roheline", 250, "#58CC02"),
    };

    public ShopPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ThemeService.ApplyToPage(this, ShopHeaderBanner, null, ShopBackBtn, ShopCoinsPill);
        RefreshUI();
    }

    private void RefreshUI()
    {
        LblShopCoins.Text = ShopService.Coins.ToString();
        LblLivesStatus.Text = $"Praegu: {LivesService.CurrentLives} / {LivesService.MaxLives} elu";
        BuildAvatarsUI();
        BuildThemesUI();
    }

    private void BuildAvatarsUI()
    {
        AvatarsGrid.Children.Clear();
        AvatarsGrid.RowDefinitions.Clear();

        int rowCount = (int)Math.Ceiling(_avatarItems.Count / 2.0);
        for (int r = 0; r < rowCount; r++)
        {
            AvatarsGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        }

        for (int i = 0; i < _avatarItems.Count; i++)
        {
            var item = _avatarItems[i];
            int row = i / 2;
            int col = i % 2;

            bool isUnlocked = ShopService.IsAvatarUnlocked(item.File);

            var card = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 18 },
                BackgroundColor = Color.FromArgb(isUnlocked ? "#FFFFFF" : "#FAFAFA"),
                Stroke = Color.FromArgb(isUnlocked ? "#86EFAC" : "#E5E5E5"),
                StrokeThickness = isUnlocked ? 2 : 1.5,
                Padding = new Thickness(14, 16),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            var stack = new VerticalStackLayout
            {
                Spacing = 10,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center
            };

            var imgContainer = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 18 },
                BackgroundColor = Color.FromArgb(isUnlocked ? "#DCFCE7" : "#F3F4F6"),
                StrokeThickness = 0,
                WidthRequest = 64,
                HeightRequest = 64,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Content = new Image
                {
                    Source = item.File,
                    WidthRequest = 48,
                    HeightRequest = 48,
                    Aspect = Aspect.AspectFit,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };
            stack.Children.Add(imgContainer);

            var nameLabel = new Label
            {
                Text = item.Name,
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#2B2B2B"),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };
            stack.Children.Add(nameLabel);

            if (isUnlocked)
            {
                var ownedPill = new Border
                {
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
                    BackgroundColor = Color.FromArgb("#DCFCE7"),
                    StrokeThickness = 0,
                    Padding = new Thickness(14, 5),
                    HeightRequest = 34,
                    WidthRequest = 110,
                    HorizontalOptions = LayoutOptions.Center,
                    Content = new Label
                    {
                        Text = "✓ OLEMAS",
                        FontSize = 12,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#15803D"),
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                };
                stack.Children.Add(ownedPill);
            }
            else
            {
                var buyBtnGrid = new Grid
                {
                    HeightRequest = 36,
                    WidthRequest = 110,
                    HorizontalOptions = LayoutOptions.Center
                };
                var shadow = new Border
                {
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
                    BackgroundColor = Color.FromArgb("#D97706"),
                    StrokeThickness = 0,
                    Margin = new Thickness(0, 3, 0, 0)
                };
                var front = new Border
                {
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
                    BackgroundColor = Color.FromArgb("#FF9600"),
                    StrokeThickness = 0,
                    Margin = new Thickness(0, 0, 0, 3),
                    Content = new Label
                    {
                        Text = $"{item.Price} 🪙",
                        FontSize = 13,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Colors.White,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                };
                var tap = new TapGestureRecognizer();
                tap.Tapped += async (s, e) =>
                {
                    await front.TranslateToAsync(0, 3, 60, Easing.CubicIn);
                    await front.TranslateToAsync(0, 0, 60, Easing.CubicOut);
                    await OnBuyAvatarClicked(item);
                };
                front.GestureRecognizers.Add(tap);
                buyBtnGrid.Children.Add(shadow);
                buyBtnGrid.Children.Add(front);
                stack.Children.Add(buyBtnGrid);
            }

            card.Content = stack;
            Grid.SetRow(card, row);
            Grid.SetColumn(card, col);
            AvatarsGrid.Children.Add(card);
        }
    }

    private void BuildThemesUI()
    {
        ThemesGrid.Children.Clear();
        ThemesGrid.RowDefinitions.Clear();

        int rowCount = (int)Math.Ceiling(_themeItems.Count / 2.0);
        for (int r = 0; r < rowCount; r++)
        {
            ThemesGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        }

        for (int i = 0; i < _themeItems.Count; i++)
        {
            var item = _themeItems[i];
            int row = i / 2;
            int col = i % 2;

            bool isUnlocked = ShopService.IsThemeUnlocked(item.Id);

            var card = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 18 },
                BackgroundColor = Color.FromArgb(isUnlocked ? "#FFFFFF" : "#FAFAFA"),
                Stroke = Color.FromArgb(isUnlocked ? item.AccentColor : "#E5E5E5"),
                StrokeThickness = isUnlocked ? 2 : 1.5,
                Padding = new Thickness(14, 16),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            var stack = new VerticalStackLayout
            {
                Spacing = 10,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center
            };

            var swatchContainer = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 14 },
                BackgroundColor = Color.FromArgb(item.AccentColor),
                Stroke = Color.FromArgb("#E5E5E5"),
                StrokeThickness = 1,
                WidthRequest = 64,
                HeightRequest = 44,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            stack.Children.Add(swatchContainer);

            var nameLabel = new Label
            {
                Text = item.DisplayName,
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#2B2B2B"),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };
            stack.Children.Add(nameLabel);

            if (isUnlocked)
            {
                var ownedPill = new Border
                {
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
                    BackgroundColor = Color.FromArgb("#DCFCE7"),
                    StrokeThickness = 0,
                    Padding = new Thickness(14, 5),
                    HeightRequest = 34,
                    WidthRequest = 110,
                    HorizontalOptions = LayoutOptions.Center,
                    Content = new Label
                    {
                        Text = "✓ AVATUD",
                        FontSize = 12,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#15803D"),
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                };
                stack.Children.Add(ownedPill);
            }
            else
            {
                var buyBtnGrid = new Grid
                {
                    HeightRequest = 36,
                    WidthRequest = 110,
                    HorizontalOptions = LayoutOptions.Center
                };
                var shadow = new Border
                {
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
                    BackgroundColor = Color.FromArgb("#D97706"),
                    StrokeThickness = 0,
                    Margin = new Thickness(0, 3, 0, 0)
                };
                var front = new Border
                {
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
                    BackgroundColor = Color.FromArgb("#FF9600"),
                    StrokeThickness = 0,
                    Margin = new Thickness(0, 0, 0, 3),
                    Content = new Label
                    {
                        Text = $"{item.Price} 🪙",
                        FontSize = 13,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Colors.White,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                };
                var tap = new TapGestureRecognizer();
                tap.Tapped += async (s, e) =>
                {
                    await front.TranslateToAsync(0, 3, 60, Easing.CubicIn);
                    await front.TranslateToAsync(0, 0, 60, Easing.CubicOut);
                    await OnBuyThemeClicked(item);
                };
                front.GestureRecognizers.Add(tap);
                buyBtnGrid.Children.Add(shadow);
                buyBtnGrid.Children.Add(front);
                stack.Children.Add(buyBtnGrid);
            }

            card.Content = stack;
            Grid.SetRow(card, row);
            Grid.SetColumn(card, col);
            ThemesGrid.Children.Add(card);
        }
    }

    private Action? _pendingPurchaseAction;

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

        var theme = ThemeService.CurrentTheme;
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

        var theme = ThemeService.CurrentTheme;
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
        if (SheetAlertButtonBorder != null)
        {
            SheetAlertButtonBorder.BackgroundColor = theme.ButtonBackground;
        }

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

    private async Task OnBuyAvatarClicked(AvatarShopItem item)
    {
        if (ShopService.Coins < item.Price)
        {
            await ShowAlertBottomSheetAsync(
                "Münte pole piisavalt",
                $"Sul on {ShopService.Coins} münti, kuid vaja on {item.Price} münti! Mängi ja võida mänge müntide teenimiseks.",
                "duo_coin.png",
                Color.FromArgb("#FEF2F2"),
                Color.FromArgb("#FECACA"));
            return;
        }

        var theme = ThemeService.CurrentTheme;
        await ShowConfirmBottomSheetAsync(
            $"Osta avatar «{item.Name}»?",
            $"Hind: {item.Price} münti (sul jääb alles {ShopService.Coins - item.Price} münti)",
            item.File,
            theme.FooterActiveTabBg,
            theme.HeaderSurfaceColor,
            $"OSTA ({item.Price} 🪙)",
            () =>
            {
                if (ShopService.BuyAvatar(item.File, item.Price))
                {
                    RefreshUI();
                }
            });
    }

    private async Task OnBuyThemeClicked(ThemeShopItem item)
    {
        if (ShopService.Coins < item.Price)
        {
            await ShowAlertBottomSheetAsync(
                "Münte pole piisavalt",
                $"Sul on {ShopService.Coins} münti, kuid vaja on {item.Price} münti! Mängi ja võida mänge müntide teenimiseks.",
                "duo_coin.png",
                Color.FromArgb("#FEF2F2"),
                Color.FromArgb("#FECACA"));
            return;
        }

        await ShowConfirmBottomSheetAsync(
            $"Osta teema «{item.DisplayName}»?",
            $"Hind: {item.Price} münti (sul jääb alles {ShopService.Coins - item.Price} münti)",
            "duo_palette.png",
            Color.FromArgb(item.AccentColor),
            Color.FromArgb("#E5E5E5"),
            $"OSTA ({item.Price} 🪙)",
            () =>
            {
                if (ShopService.BuyTheme(item.Id, item.Price))
                {
                    RefreshUI();
                }
            });
    }

    private async void OnBuyOneLifeClicked(object? sender, EventArgs e)
    {
        if (LivesService.CurrentLives >= LivesService.MaxLives)
        {
            await ShowAlertBottomSheetAsync(
                "Elud on täis",
                "Sul on juba maksimaalselt 10/10 elu!",
                "duo_heart.png",
                Color.FromArgb("#FEF2F2"),
                Color.FromArgb("#FECACA"));
            return;
        }

        const int price = 50;
        if (ShopService.Coins < price)
        {
            await ShowAlertBottomSheetAsync(
                "Münte pole piisavalt",
                $"Ühe elu taastamiseks on vaja {price} münti! Sul on praegu {ShopService.Coins} münti.",
                "duo_coin.png",
                Color.FromArgb("#FEF2F2"),
                Color.FromArgb("#FECACA"));
            return;
        }

        await ShowConfirmBottomSheetAsync(
            "Taasta +1 elu?",
            $"Hind: {price} münti (sul jääb alles {ShopService.Coins - price} münti). Pärast ostu on sul {LivesService.CurrentLives + 1}/10 elu.",
            "duo_heart.png",
            Color.FromArgb("#FEF2F2"),
            Color.FromArgb("#FECACA"),
            $"OSTA ({price} 🪙)",
            () =>
            {
                if (ShopService.BuyLives(1, price))
                {
                    RefreshUI();
                }
            });
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
        else if (Navigation.NavigationStack.Count > 1)
        {
            await Navigation.PopToRootAsync();
        }
    }
}

