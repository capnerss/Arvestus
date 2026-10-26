using Microsoft.Extensions.DependencyInjection;

namespace TicTacToe;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		TicTacToe.Services.SettingsService.Instance.Initialize();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		const int targetWidth = 430;
		const int targetHeight = 880;

		var window = new Window(new AppShell())
		{
			Title = "Trips-Traps-Trull",
			Width = targetWidth,
			Height = targetHeight,
			MinimumWidth = 360,
			MinimumHeight = 640
		};

#if WINDOWS
		window.Created += (s, e) =>
		{
			var platformWindow = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
			if (platformWindow?.AppWindow is Microsoft.UI.Windowing.AppWindow appWindow)
			{
				var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(appWindow.Id, Microsoft.UI.Windowing.DisplayAreaFallback.Primary);
				int width = 450;
				int height = 900;
				int x = Math.Max(0, (displayArea.WorkArea.Width - width) / 2);
				int y = Math.Max(20, (displayArea.WorkArea.Height - height) / 2);
				appWindow.MoveAndResize(new Windows.Graphics.RectInt32(x, y, width, height));

				var iconPath = Path.Combine(AppContext.BaseDirectory, "appicon.ico");
				if (!File.Exists(iconPath))
				{
					iconPath = Path.Combine(AppContext.BaseDirectory, "Resources", "AppIcon", "appicon.ico");
				}

				if (File.Exists(iconPath))
				{
					appWindow.SetIcon(iconPath);
				}
			}
		};
#endif

		return window;
	}
}