using TicTacToe.Views;

namespace TicTacToe;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(RulesPage), typeof(RulesPage));
		Routing.RegisterRoute(nameof(StatsPage), typeof(StatsPage));
		Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
		Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
		Routing.RegisterRoute(nameof(GameModePage), typeof(GameModePage));
		Routing.RegisterRoute(nameof(GameResultPage), typeof(GameResultPage));
		Routing.RegisterRoute(nameof(ShopPage), typeof(ShopPage));
		Routing.RegisterRoute(nameof(LivesPage), typeof(LivesPage));
		Routing.RegisterRoute(nameof(ProfileSavedPage), typeof(ProfileSavedPage));
		Routing.RegisterRoute(nameof(TasksPage), typeof(TasksPage));
	}
}
