using Microsoft.Extensions.Logging;

namespace TicTacToe;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("Nunito-Regular.ttf", "NunitoRegular");
				fonts.AddFont("Nunito-SemiBold.ttf", "NunitoSemiBold");
				fonts.AddFont("Nunito-Bold.ttf", "NunitoBold");
				fonts.AddFont("Nunito-ExtraBold.ttf", "NunitoExtraBold");
				fonts.AddFont("Nunito-Black.ttf", "NunitoBlack");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.Services.AddSingleton(Plugin.Maui.Audio.AudioManager.Current);

		return builder.Build();
	}
}
