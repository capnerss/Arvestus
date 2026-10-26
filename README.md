# Trips-Traps-Trull (.NET MAUI)

Kaasaegne, taktiilse 3D stiilis kujundusega ja funktsioonirikas Trips-Traps-Trull (Tic-Tac-Toe) mängurakendus, mis on välja töötatud .NET 10 MAUI platvormil.

Rakendus toetab mitut mängurežiimi (kahekesi seadmes, nutikas bot, seiklusmõistatused), kohalikku SQLite andmebaasi, mitmekeelset kasutajaliidest (ET, EN, RU), heliefekte ja taustamuusikat, dünaamilisi teemasid (hele/tume režiim) ning interaktiivset lohistamise (Drag & Drop) funktsionaalsust.

---

## Projekti struktuur

Projekt järgib modulaarset MVVM (Model-View-ViewModel) arhitektuurimustrit ning kood on jaotatud loogilistesse kihtidesse:

```text
TicTacToe/
├── App.xaml / App.xaml.cs          # Rakenduse tuum, elutsükkel, globaalsed ressursid ja akna initsialiseerimine
├── AppShell.xaml / AppShell.xaml.cs # Rakenduse navigatsioonistruktuur (MAUI Shell)
├── MauiProgram.cs                  # MAUI rakenduse käivituspunkt, DI ja fontide registreerimine
├── TicTacToe.csproj                # Projekti konfiguratsioonifail, paketid ja sihtplatvormid
│
├── Controls/                       # Korduskasutatavad kasutajaliidese komponendid
│   └── DuolingoFooter.xaml (.cs)   # Universaalne 3D Duolingo-stiilis alumine navigatsiooniriba
│
├── Models/                         # Rakenduse andmemudelid ja mänguloogika
│   ├── AdventurePuzzle.cs          # Seiklusrežiimi mõistatuste ja tasemete mudel
│   ├── AppSettings.cs              # Tsentraalne konfiguratsioonimudel (keel, teema, heli, laua suurus)
│   ├── GameEngine.cs               # Mängu tuumikmootor: reeglid, käigud, võidukontroll ja AI bot
│   ├── GameMode.cs                 # Mängurežiimide loend (PvP, Bot)
│   ├── GameTask.cs                 # SQLite ülesannete ja missioonide mudel (ORM atribuutidega)
│   ├── GameTheme.cs                # Kujundusteemade mudel (värvipaletid, pinnad, varjud)
│   ├── Player.cs                   # Mängija mudel (nimi, sümbol, värv, boti olek)
│   └── PlayerSymbol.cs             # Sümbolite loend (X, O, tühi)
│
├── ViewModels/                     # MVVM vaatemudelid (äriloogika ja andmesidumine vaatega)
│   ├── BaseViewModel.cs            # Vaatemudelite baasklass (INotifyPropertyChanged realisatsioon)
│   ├── MainViewModel.cs            # Peamängulaua loogika, sümbolite ja käikude haldus
│   ├── AdventureViewModel.cs       # Seikluskaardi tasemete valik ja edenemine
│   ├── TasksViewModel.cs           # SQLite missioonide laadimine, edenemine ja preemiate lunastamine
│   ├── SettingsViewModel.cs        # Seadete dünaamiline haldamine reaalajas
│   ├── ShopViewModel.cs            # Mängusisese poe ostuloogika (avatarid, teemad, elud)
│   ├── ProfileViewModel.cs         # Mängija profiili, märkide ja saavutuste juhtimine
│   ├── LivesViewModel.cs           # Mänguelude süsteem ja taastumise taimer
│   ├── RecordsViewModel.cs         # Parimate tulemuste ja rekordite vaade
│   ├── StatsViewModel.cs           # Üldise mängustatistika vaade
│   ├── RulesViewModel.cs           # Mängureeglite ja taktikate vaade
│   ├── GameModeViewModel.cs        # Mängurežiimi valiku vaatemudel
│   ├── GameResultViewModel.cs      # Mängu lõpptulemuse ja auhindade vaade
│   └── NewsViewModel.cs            # Uudiste ja värskenduste teadete vaade
│
├── Views/                          # Kasutajaliidese lehed (XAML ja kooditagune loogika)
│   ├── MainPage.xaml (.cs)         # Peamine mängulaud dünaamilise Grid-võrgustikuga
│   ├── AdventurePage.xaml (.cs)    # Seiklusrežiimi teekaart tasemetega
│   ├── AdventurePuzzlePage.xaml    # Taktikaline puslelaud (nt "Võida 1 käiguga")
│   ├── TasksPage.xaml (.cs)        # Igapäevased missioonid (kasutab DataTemplate'i)
│   ├── ShopPage.xaml (.cs)         # Pood avataride ja teemade ostmiseks müntide eest
│   ├── ProfilePage.xaml (.cs)      # Profiil ja saavutuste vitriin (Drag & Drop / Tap-to-Place)
│   ├── ProfileSavedPage.xaml (.cs) # Profiili salvestamise kinnitusleht
│   ├── SettingsPage.xaml (.cs)     # Seadete leht (MAUI Styles ja AppThemeBinding)
│   ├── RulesPage.xaml (.cs)        # Interaktiivne reeglileht siili-maskoti nõuannetega
│   ├── LivesPage.xaml (.cs)        # Elude ja taastumise haldamise leht
│   ├── RecordsPage.xaml (.cs)      # Rekordite ja edetabeli leht
│   ├── StatsPage.xaml (.cs)        # Mängude ja võitude statistika leht
│   ├── GameModePage.xaml (.cs)     # Mängurežiimi ja laua suuruse valik
│   ├── GameResultPage.xaml (.cs)   # Võidu/kaotuse/viigi dialoog ja animatsioonid
│   ├── MoreMenuPage.xaml (.cs)     # Lisamenüü kiirviidetega
│   └── NewsPage.xaml (.cs)         # Uudiste ja versiooniuuenduste leht
│
├── Services/                       # Tsentraalsed teenused ja andmehaldus
│   ├── SettingsService.cs          # Mälupõhine dünaamiline seadete teenus (In-Memory + Preferences)
│   ├── AppSettingsService.cs       # Tagasiühilduvuse fassaad SettingsService teenusele
│   ├── ITaskRepository.cs          # SQLite ülesannete repositooriumi liides
│   ├── SqliteTaskRepository.cs     # Asünkroonne SQLite andmebaasiteenus (CRUD operatsioonid)
│   ├── LocalizationService.cs      # Mitmekeelsuse haldusteenus (ET, EN, RU) reaalajas
│   ├── ThemeService.cs             # Kujundusteemade ja värvide rakendamise teenus
│   ├── AudioService.cs             # Muusika ja heliefektide esitamine (Plugin.Maui.Audio)
│   ├── UserProfileService.cs       # Kasutajaprofiili, avatari, müntide ja märkide teenus
│   ├── ShopService.cs              # Poe toodete, hindade ja ostude valideerimise teenus
│   ├── LivesService.cs             # Elude kahanemise ja automaatse taastumise loogika
│   ├── GameStatsService.cs         # Mängustatistika kogumine ja püsiv talletamine
│   └── AdventurePuzzleService.cs   # Seiklusmõistatuste generaator ja lahenduskontroll
│
├── Resources/                      # Rakenduse staatilised ressursid
│   ├── AppIcon/                    # Rakenduse ikoonid eri platvormidele (.svg, .ico)
│   ├── Fonts/                      # Kohandatud kirjatüübid (Nunito perekond ja OpenSans)
│   ├── Images/                     # SVG ja PNG vektorgraafika (ikoonid, avatarid, maskott)
│   ├── Localization/               # Keelefailid (.resx) ja XAML laiendus
│   │   ├── AppResources.resx       # Vaikimisi ressurss (eesti keel)
│   │   ├── AppResources.en.resx    # Inglise keel
│   │   ├── AppResources.ru.resx    # Vene keel
│   │   ├── AppResources.cs         # ResX genereeritud kood
│   │   └── TranslateExtension.cs   # Dünaamiline tõlkelaiendus XAML-lehtedele
│   ├── Raw/                        # Toored meediafailid (taustamuusika ja heliefektid)
│   │   ├── background_music.wav    # Mängu taustamuusika
│   │   ├── tap.wav                 # Nupuvajutuse ja käigu heliefekt
│   │   └── win.wav                 # Võidu heliefekt
│   ├── Splash/                     # Rakenduse laadimisekraani (Splash Screen) pildid
│   └── Styles/                     # Kujundusstiilid ja värvid
│       ├── Colors.xaml             # Globaalsed värvipaletid
│       └── Styles.xaml             # MAUI Styles ja AppThemeBinding definitsioonid
│
└── Platforms/                      # Platvormispetsiifilised initsialiseerimisfailid
    ├── Android/                    # Androidi spetsiifiline konfiguratsioon ja MainActivity
    ├── iOS/                        # iOS-i elutsükkel ja AppDelegate
    ├── MacCatalyst/                # macOS (Mac Catalyst) tugi
    └── Windows/                    # Windows App SDK (WinUI 3) spetsiifiline käivitus ja aknahaldus
```

---

## Tehnoloogiline pinu ja lahendused

1. .NET 10 MAUI (Multi-platform App UI)
   - Ristplatvormne raamistik Windowsi, Androidi, iOS-i ja macOS-i jaoks.
   - Windowsi sihtmärk: `net10.0-windows10.0.19041.0` (Unpackaged WinUI 3 rakendus).

2. MVVM arhitektuur ja andmesidumine
   - Kõik lehed kasutavad vaatemudeleid (`ViewModels`), mis pärinevad `BaseViewModel` baasklassist.
   - Kahesuunaline andmesidumine (`Two-Way Binding`), käsud (`ICommand` / `RelayCommand`) ja atribuutide muutuse teavitused (`INotifyPropertyChanged`).
   - Lehel `TasksPage.xaml` kasutatakse `CollectionView.ItemTemplate` sees `DataTemplate` tehnikat.

3. Andmebaas (SQLite)
   - Kasutusel on `sqlite-net-pcl` ja `SQLitePCLRaw.bundle_green`.
   - Repositooriumi muster `ITaskRepository` ja `SqliteTaskRepository` tagab puhta andmebaasikihi asünkroonsete CRUD päringutega (`tictactoe.db3`).

4. Tsentraalsed dünaamilised seaded (SettingsService)
   - Seadeid hoitakse operatiivmälus (`Current` eksemplar), mis välistab tarbetud I/O päringud animatsioonide ajal.
   - Reaalajas sündmus `OnSettingsChanged` teavitab koheselt teisi komponente.
   - Püsivus tagatakse `Preferences` salvestusega.

5. Mitmekeelsus (Localization)
   - Toetatud on eesti (`et`), inglise (`en`) ja vene (`ru`) keeled.
   - Tõlked pärinevad `.resx` ressurssidest ning `TranslateExtension` ja `LocalizationService` võimaldavad keelt vahetada reaalajas ilma rakendust taaskäivitamata.

6. Heli ja muusika (Plugin.Maui.Audio)
   - Taustamuusika (`background_music.wav`) tsükliline esitamine.
   - Käigu ja nupu vajutuse heli (`tap.wav`) ning võiduheli (`win.wav`).
   - Helisid saab seadetest igal ajal sisse ja välja lülitada või peatada/jätkata.

7. Kujundus ja teemad (MAUI Styles & AppThemeBinding)
   - Duolingo visuaalne stiil (taktiilsed 3D-nupud, kaardid, ümarad servad, spetsiaalsed fondid `Nunito`).
   - Heleda ja tumeda režiimi tugi `AppThemeBinding` abil (`Styles.xaml`).
   - Täiendavad mänguteemad (Sinine, Lilla, Roheline, Sunset).

8. Drag & Drop ja Tap-to-Place
   - Profiililehel (`ProfilePage.xaml`) saavutuste märkide ümberpaigutamine lohistades (`DragGestureRecognizer` ja `DropGestureRecognizer`) või puudutades.

---

## Käivitamine

### Nõuded
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- .NET MAUI tööriistad (`maui-windows`, `maui-android` jne)
- Windows 10 (versioon 19041 või uuem) või Windows 11

### Käivitamine käsurealt (Windows)

```powershell
# Projekti taastamine ja ehitamine
dotnet build -f net10.0-windows10.0.19041.0

# Rakenduse käivitamine
dotnet run -f net10.0-windows10.0.19041.0
```

### Käivitamine Visual Studio kaudu
1. Ava `TicTacToe.csproj` Visual Studio 2022+ abil.
2. Vali sihtmärgiks Windows Machine (või Android-emulaator).
3. Vajuta F5 või nuppu Start Debugging.