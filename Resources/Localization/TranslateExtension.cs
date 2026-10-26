using TicTacToe.Services;

namespace TicTacToe.Resources.Localization;

/// <summary>
/// XAML märgistuse laiendus, mis võimaldab deklaratiivset tõlget reaalajas:
/// Text="{loc:Translate SettingsTitle}"
/// Keele vahetumisel uueneb tekst automaatselt ilma rakendust taaskäivitamata.
/// </summary>
[ContentProperty(nameof(Key))]
public class TranslateExtension : IMarkupExtension<BindingBase>
{
    public string Key { get; set; } = string.Empty;
    public string? StringFormat { get; set; }

    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
        return new Binding
        {
            Mode = BindingMode.OneWay,
            Path = $"[{Key}]",
            Source = LocalizationService.Instance,
            StringFormat = StringFormat
        };
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
}
