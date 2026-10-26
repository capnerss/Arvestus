using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using TicTacToe.Resources.Localization;

namespace TicTacToe.Services;

/// <summary>
/// Reaktiivne lokaliseerimisteenus, mis toetab keele dünaamilist vahetamist
/// reaalajas ilma rakendust taaskäivitamata.
/// </summary>
public class LocalizationService : INotifyPropertyChanged
{
    private const string KeyLanguage = "App_Language";

    private static LocalizationService? _instance;
    public static LocalizationService Instance => _instance ??= new LocalizationService();

    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action? OnLanguageChanged;

    private string _currentLanguage = "et";

    public string CurrentLanguage
    {
        get => _currentLanguage;
        private set
        {
            if (_currentLanguage != value)
            {
                _currentLanguage = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Indekseerija XAML andmesidumise jaoks: {Binding Source={x:Static services:LocalizationService.Instance}, Path=[Key]}
    /// </summary>
    public string this[string key] => AppResources.GetString(key);

    private LocalizationService()
    {
        string saved = Preferences.Default.Get(KeyLanguage, "et");
        ApplyCulture(saved, fireEvents: false);
    }

    public void SetLanguage(string lang)
    {
        if (string.IsNullOrWhiteSpace(lang)) lang = "et";
        lang = lang.ToLowerInvariant();

        if (lang != "et" && lang != "en" && lang != "ru")
            lang = "et";

        Preferences.Default.Set(KeyLanguage, lang);
        ApplyCulture(lang, fireEvents: true);
    }

    private void ApplyCulture(string lang, bool fireEvents)
    {
        _currentLanguage = lang;
        var culture = lang switch
        {
            "en" => new CultureInfo("en-US"),
            "ru" => new CultureInfo("ru-RU"),
            _ => new CultureInfo("et-EE")
        };

        AppResources.Culture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        if (fireEvents)
        {
            // Null või tühi string parameetris teatab WPF/MAUI-le, et KÕIK indekseeritud omadused on muutunud
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLanguage)));
            OnLanguageChanged?.Invoke();
        }
    }

    public static string Get(string key) => AppResources.GetString(key);

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
