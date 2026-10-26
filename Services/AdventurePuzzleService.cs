using TicTacToe.Models;

namespace TicTacToe.Services;

public static class AdventurePuzzleService
{
    private const string KeyHighestUnlockedLevel = "Adventure_HighestUnlockedLevel";
    private const string KeyLevelCompletedPrefix = "Adventure_Completed_";
    private const string KeyLevelRewardClaimedPrefix = "Adventure_RewardClaimed_";
    private const string KeyChestClaimed = "Adventure_ChestClaimed_Chapter1";

    public static event Action? OnAdventureProgressChanged;

    public static bool IsChestClaimed
    {
        get => Preferences.Default.Get(KeyChestClaimed, false);
        set
        {
            Preferences.Default.Set(KeyChestClaimed, value);
            OnAdventureProgressChanged?.Invoke();
        }
    }

    public static bool ClaimChestReward(int coins = 100)
    {
        if (IsChestClaimed) return false;
        IsChestClaimed = true;
        ShopService.Coins += coins;
        return true;
    }

    public static int HighestUnlockedLevel
    {
        get => Math.Clamp(Preferences.Default.Get(KeyHighestUnlockedLevel, 1), 1, 5);
        private set
        {
            Preferences.Default.Set(KeyHighestUnlockedLevel, value);
            OnAdventureProgressChanged?.Invoke();
        }
    }

    public static bool IsLevelCompleted(int levelId)
    {
        return Preferences.Default.Get($"{KeyLevelCompletedPrefix}{levelId}", false);
    }

    public static bool IsRewardClaimed(int levelId)
    {
        return Preferences.Default.Get($"{KeyLevelRewardClaimedPrefix}{levelId}", false);
    }

    public static bool IsLevelUnlocked(int levelId)
    {
        if (levelId <= 1) return true;
        return levelId <= HighestUnlockedLevel;
    }

    public static int CompletedLevelsCount
    {
        get
        {
            int count = 0;
            for (int i = 1; i <= 5; i++)
            {
                if (IsLevelCompleted(i)) count++;
            }
            return count;
        }
    }

    public static List<AdventurePuzzle> GetPuzzles()
    {
        return new List<AdventurePuzzle>
        {
            // ==========================================
            // УРОВЕНЬ 1: Võidukäik (Мат в 1 ход)
            // ==========================================
            new AdventurePuzzle
            {
                Id = 1,
                Title = "TASE 1 • VÕIDUKÄIK",
                Subtitle = "Leia võidukäik ja lõpeta mäng ühe liigutusega!",
                Description = "Leia puuduv kolmas sümbol real, veerul või diagonaalil ja saavuta kohene võit.",
                BadgeText = "Lihtne • 1 käik",
                ThemeColor = "#58CC02",
                ThemeShadow = "#46A302",
                Icon = "★",
                RewardCoins = 50,
                RewardHearts = 0,
                DifficultyStars = 1,
                Tasks = new List<PuzzleTask>
                {
                    // 1.1: Ülemine rida
                    new PuzzleTask
                    {
                        TaskNumber = 1,
                        Title = "Ülemine võidurida",
                        Instruction = "Lõpeta ülemine rida ja saavuta võit!",
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.X, PlayerSymbol.X, PlayerSymbol.None },
                            { PlayerSymbol.O, PlayerSymbol.O, PlayerSymbol.None },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.None }
                        },
                        SolutionSteps = new List<PuzzleStep> { new PuzzleStep { ExpectedRow = 0, ExpectedCol = 2 } }
                    },
                    // 1.2: Parem veerg
                    new PuzzleTask
                    {
                        TaskNumber = 2,
                        Title = "Parem võiduveerg",
                        Instruction = "Lõpeta parempoolne püstine rida!",
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.O, PlayerSymbol.None, PlayerSymbol.X },
                            { PlayerSymbol.O, PlayerSymbol.None, PlayerSymbol.X },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.None }
                        },
                        SolutionSteps = new List<PuzzleStep> { new PuzzleStep { ExpectedRow = 2, ExpectedCol = 2 } }
                    },
                    // 1.3: Peadiagonaal
                    new PuzzleTask
                    {
                        TaskNumber = 3,
                        Title = "Peadiagonaali võit",
                        Instruction = "Ühenda vasakust ülanurgast paremasse alanurka!",
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.X, PlayerSymbol.O, PlayerSymbol.None },
                            { PlayerSymbol.O, PlayerSymbol.X, PlayerSymbol.None },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.None }
                        },
                        SolutionSteps = new List<PuzzleStep> { new PuzzleStep { ExpectedRow = 2, ExpectedCol = 2 } }
                    },
                    // 1.4: Kõrvaldiagonaal
                    new PuzzleTask
                    {
                        TaskNumber = 4,
                        Title = "Kõrvaldiagonaali tabamus",
                        Instruction = "Lõpeta diagonaal paremalt ülevalt vasakule alla!",
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.O, PlayerSymbol.None, PlayerSymbol.X },
                            { PlayerSymbol.None, PlayerSymbol.X, PlayerSymbol.O },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.None }
                        },
                        SolutionSteps = new List<PuzzleStep> { new PuzzleStep { ExpectedRow = 2, ExpectedCol = 0 } }
                    },
                    // 1.5: Keskmine rida läbi keskme
                    new PuzzleTask
                    {
                        TaskNumber = 5,
                        Title = "Keskruudu otsustav löök",
                        Instruction = "Aseta rist keskele ja vormista taseme viimane võit!",
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.O, PlayerSymbol.O, PlayerSymbol.None },
                            { PlayerSymbol.X, PlayerSymbol.None, PlayerSymbol.X },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.None }
                        },
                        SolutionSteps = new List<PuzzleStep> { new PuzzleStep { ExpectedRow = 1, ExpectedCol = 1 } }
                    }
                }
            },

            // ==========================================
            // УРОВЕНЬ 2: Ohu blokeerimine (Парирование)
            // ==========================================
            new AdventurePuzzle
            {
                Id = 2,
                Title = "TASE 2 • OHU BLOKEERIMINE",
                Subtitle = "Vastane (X) ründab võidule. Blokeeri oht ringiga (O)!",
                Description = "Vastasel on kaks sümbolit reas. Leia ainus elupäästev ruut!",
                BadgeText = "Kaitse (O) • 1 käik",
                PlayerSymbol = PlayerSymbol.O,
                ThemeColor = "#1CB0F6",
                ThemeShadow = "#1899D6",
                Icon = "🛡",
                RewardCoins = 75,
                RewardHearts = 0,
                DifficultyStars = 2,
                Tasks = new List<PuzzleTask>
                {
                    // 2.1: Ülemise rea kaitse
                    new PuzzleTask
                    {
                        TaskNumber = 1,
                        Title = "Ülemise rea tõrje",
                        Instruction = "Aseta ring (O) ja blokeeri vastase võidukäik ülemisel real!",
                        PlayerSymbol = PlayerSymbol.O,
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.X, PlayerSymbol.X, PlayerSymbol.None },
                            { PlayerSymbol.None, PlayerSymbol.O, PlayerSymbol.None },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.None }
                        },
                        SolutionSteps = new List<PuzzleStep> { new PuzzleStep { ExpectedRow = 0, ExpectedCol = 2 } }
                    },
                    // 2.2: Vasaku veeru kaitse
                    new PuzzleTask
                    {
                        TaskNumber = 2,
                        Title = "Vasaku serva kaitsmine",
                        Instruction = "Vastane (X) ründab vasakus veerus. Sule tema tee ringiga (O)!",
                        PlayerSymbol = PlayerSymbol.O,
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.X, PlayerSymbol.None, PlayerSymbol.None },
                            { PlayerSymbol.X, PlayerSymbol.O, PlayerSymbol.None },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.None }
                        },
                        SolutionSteps = new List<PuzzleStep> { new PuzzleStep { ExpectedRow = 2, ExpectedCol = 0 } }
                    },
                    // 2.3: Diagonaali kaitse
                    new PuzzleTask
                    {
                        TaskNumber = 3,
                        Title = "Diagonaali blokeering",
                        Instruction = "Vastane (X) sihib peadiagonaali. Hõiva keskruut ringiga (O)!",
                        PlayerSymbol = PlayerSymbol.O,
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.X, PlayerSymbol.None, PlayerSymbol.None },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.O },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.X }
                        },
                        SolutionSteps = new List<PuzzleStep> { new PuzzleStep { ExpectedRow = 1, ExpectedCol = 1 } }
                    },
                    // 2.4: Alumise rea poolituse kaitse
                    new PuzzleTask
                    {
                        TaskNumber = 4,
                        Title = "Alumise liini lukk",
                        Instruction = "Vastasel (X) on nurgad käes. Astu ringiga (O) nende vahele!",
                        PlayerSymbol = PlayerSymbol.O,
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.O, PlayerSymbol.None, PlayerSymbol.None },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.None },
                            { PlayerSymbol.X, PlayerSymbol.None, PlayerSymbol.X }
                        },
                        SolutionSteps = new List<PuzzleStep> { new PuzzleStep { ExpectedRow = 2, ExpectedCol = 1 } }
                    },
                    // 2.5: Keskveeru kaitse
                    new PuzzleTask
                    {
                        TaskNumber = 5,
                        Title = "Keskveeru päästmine",
                        Instruction = "Peata vastase (X) rünnak ringiga (O) läbi laua keskosa!",
                        PlayerSymbol = PlayerSymbol.O,
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.None, PlayerSymbol.X, PlayerSymbol.None },
                            { PlayerSymbol.O, PlayerSymbol.X, PlayerSymbol.None },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.None }
                        },
                        SolutionSteps = new List<PuzzleStep> { new PuzzleStep { ExpectedRow = 2, ExpectedCol = 1 } }
                    }
                }
            },

            // ==========================================
            // УРОВЕНЬ 3: Kahvel ja topeltoht
            // ==========================================
            new AdventurePuzzle
            {
                Id = 3,
                Title = "TASE 3 • KAHVEL JA TOPELTOHT",
                Subtitle = "Loo topeltoht, mida vastane ei suuda kaitsta!",
                Description = "Aseta sümbol nii, et tekib kaks võiduvõimalust korraga.",
                BadgeText = "Rünnak • 2 käiku",
                ThemeColor = "#A855F7",
                ThemeShadow = "#9333EA",
                Icon = "⚔",
                RewardCoins = 100,
                RewardHearts = 1,
                DifficultyStars = 3,
                Tasks = new List<PuzzleTask>
                {
                    // 3.1: Nurgakahvel
                    new PuzzleTask
                    {
                        TaskNumber = 1,
                        Title = "Nurgakahvel",
                        Instruction = "Loo vasakusse ülanurka kahe liini oht!",
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.None, PlayerSymbol.X, PlayerSymbol.None },
                            { PlayerSymbol.X, PlayerSymbol.O, PlayerSymbol.None },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.O }
                        },
                        SolutionSteps = new List<PuzzleStep>
                        {
                            new PuzzleStep { ExpectedRow = 0, ExpectedCol = 0, BotResponseRow = 0, BotResponseCol = 2 },
                            new PuzzleStep { ExpectedRow = 2, ExpectedCol = 0 }
                        }
                    },
                    // 3.2: Keskuse kahvel
                    new PuzzleTask
                    {
                        TaskNumber = 2,
                        Title = "Keskuse kahvel",
                        Instruction = "Hõiva keskruut ja loo topeltrünnak!",
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.X, PlayerSymbol.O, PlayerSymbol.None },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.X },
                            { PlayerSymbol.None, PlayerSymbol.O, PlayerSymbol.None }
                        },
                        SolutionSteps = new List<PuzzleStep>
                        {
                            new PuzzleStep { ExpectedRow = 1, ExpectedCol = 1, BotResponseRow = 1, BotResponseCol = 0 },
                            new PuzzleStep { ExpectedRow = 2, ExpectedCol = 2 }
                        }
                    },
                    // 3.3: Parema nurga kahvel
                    new PuzzleTask
                    {
                        TaskNumber = 3,
                        Title = "Parema alanurga haarang",
                        Instruction = "Loo kahvel paremas alanurgas!",
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.O, PlayerSymbol.None, PlayerSymbol.X },
                            { PlayerSymbol.None, PlayerSymbol.O, PlayerSymbol.None },
                            { PlayerSymbol.None, PlayerSymbol.X, PlayerSymbol.None }
                        },
                        SolutionSteps = new List<PuzzleStep>
                        {
                            new PuzzleStep { ExpectedRow = 2, ExpectedCol = 2, BotResponseRow = 2, BotResponseCol = 0 },
                            new PuzzleStep { ExpectedRow = 1, ExpectedCol = 2 }
                        }
                    },
                    // 3.4: Diagonaalne rünnak
                    new PuzzleTask
                    {
                        TaskNumber = 4,
                        Title = "Ristuv surve",
                        Instruction = "Aseta sümbol paremasse ülanurka ja murra vastane!",
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.None, PlayerSymbol.X, PlayerSymbol.None },
                            { PlayerSymbol.O, PlayerSymbol.O, PlayerSymbol.X },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.None }
                        },
                        SolutionSteps = new List<PuzzleStep>
                        {
                            new PuzzleStep { ExpectedRow = 0, ExpectedCol = 2, BotResponseRow = 0, BotResponseCol = 0 },
                            new PuzzleStep { ExpectedRow = 2, ExpectedCol = 2 }
                        }
                    },
                    // 3.5: Kolmnurkne kahvel
                    new PuzzleTask
                    {
                        TaskNumber = 5,
                        Title = "Kolmnurga lukk",
                        Instruction = "Loo vasakus alanurgas otsustav topeltoht!",
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.None, PlayerSymbol.O, PlayerSymbol.None },
                            { PlayerSymbol.X, PlayerSymbol.O, PlayerSymbol.None },
                            { PlayerSymbol.None, PlayerSymbol.X, PlayerSymbol.None }
                        },
                        SolutionSteps = new List<PuzzleStep>
                        {
                            new PuzzleStep { ExpectedRow = 2, ExpectedCol = 0, BotResponseRow = 0, BotResponseCol = 0 },
                            new PuzzleStep { ExpectedRow = 2, ExpectedCol = 2 }
                        }
                    }
                }
            },

            // ==========================================
            // УРОВЕНЬ 4: Taktikaline lõks (Kaheetapiline)
            // ==========================================
            new AdventurePuzzle
            {
                Id = 4,
                Title = "TASE 4 • TAKTIKALINE LÕKS",
                Subtitle = "Vii vastane vältimatusse kaitseseisu ja karista!",
                Description = "Iga liigutus peab olema arvestatud — sunni vastane eksima.",
                BadgeText = "Taktika • 2 käiku",
                ThemeColor = "#FF9600",
                ThemeShadow = "#E08500",
                Icon = "🎯",
                RewardCoins = 150,
                RewardHearts = 1,
                DifficultyStars = 4,
                Tasks = new List<PuzzleTask>
                {
                    // 4.1: Kahe liini surve
                    new PuzzleTask
                    {
                        TaskNumber = 1,
                        Title = "Kahe liini surve",
                        Instruction = "Blokeeri vastase oht ja loo topeltrünnak diagonaalil!",
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.X, PlayerSymbol.None, PlayerSymbol.None },
                            { PlayerSymbol.None, PlayerSymbol.X, PlayerSymbol.O },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.O }
                        },
                        SolutionSteps = new List<PuzzleStep>
                        {
                            new PuzzleStep { ExpectedRow = 0, ExpectedCol = 2, BotResponseRow = 0, BotResponseCol = 1 },
                            new PuzzleStep { ExpectedRow = 2, ExpectedCol = 0 }
                        }
                    },
                    // 4.2: Ohu tõrje ja vasturünnak
                    new PuzzleTask
                    {
                        TaskNumber = 2,
                        Title = "Ohu tõrje ja vasturünnak",
                        Instruction = "Blokeeri vastase diagonaal ja püstita veeru lõks!",
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.X, PlayerSymbol.None, PlayerSymbol.O },
                            { PlayerSymbol.None, PlayerSymbol.O, PlayerSymbol.None },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.X }
                        },
                        SolutionSteps = new List<PuzzleStep>
                        {
                            new PuzzleStep { ExpectedRow = 2, ExpectedCol = 0, BotResponseRow = 2, BotResponseCol = 1 },
                            new PuzzleStep { ExpectedRow = 1, ExpectedCol = 0 }
                        }
                    },
                    // 4.3: Peadiagonaali lukk
                    new PuzzleTask
                    {
                        TaskNumber = 3,
                        Title = "Peadiagonaali lukk",
                        Instruction = "Peata vastase peadiagonaal ja ava võidutee paremal!",
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.O, PlayerSymbol.None, PlayerSymbol.X },
                            { PlayerSymbol.None, PlayerSymbol.O, PlayerSymbol.None },
                            { PlayerSymbol.None, PlayerSymbol.X, PlayerSymbol.None }
                        },
                        SolutionSteps = new List<PuzzleStep>
                        {
                            new PuzzleStep { ExpectedRow = 2, ExpectedCol = 2, BotResponseRow = 2, BotResponseCol = 0 },
                            new PuzzleStep { ExpectedRow = 1, ExpectedCol = 2 }
                        }
                    },
                    // 4.4: Nurgapääste ja topeltrünnak
                    new PuzzleTask
                    {
                        TaskNumber = 4,
                        Title = "Nurgapääste ja topeltrünnak",
                        Instruction = "Kaitse vasakut ülanurka ja karista vastast vasakus veerus!",
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.None, PlayerSymbol.X, PlayerSymbol.None },
                            { PlayerSymbol.X, PlayerSymbol.O, PlayerSymbol.None },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.O }
                        },
                        SolutionSteps = new List<PuzzleStep>
                        {
                            new PuzzleStep { ExpectedRow = 0, ExpectedCol = 0, BotResponseRow = 0, BotResponseCol = 2 },
                            new PuzzleStep { ExpectedRow = 2, ExpectedCol = 0 }
                        }
                    },
                    // 4.5: Otsustav taktikaline lõks
                    new PuzzleTask
                    {
                        TaskNumber = 5,
                        Title = "Otsustav taktikaline lõks",
                        Instruction = "Blokeeri vastase diagonaal ja vormista 4. taseme lõppvõit!",
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.X, PlayerSymbol.None, PlayerSymbol.O },
                            { PlayerSymbol.None, PlayerSymbol.O, PlayerSymbol.None },
                            { PlayerSymbol.None, PlayerSymbol.X, PlayerSymbol.None }
                        },
                        SolutionSteps = new List<PuzzleStep>
                        {
                            new PuzzleStep { ExpectedRow = 2, ExpectedCol = 0, BotResponseRow = 2, BotResponseCol = 2 },
                            new PuzzleStep { ExpectedRow = 1, ExpectedCol = 0 }
                        }
                    }
                }
            },

            // ==========================================
            // УРОВЕНЬ 5: Bossi mõistatus (Kaval Rebane)
            // ==========================================
            new AdventurePuzzle
            {
                Id = 5,
                Title = "TASE 5 • BOSSI MÕISTATUS",
                Subtitle = "Alista Kavala Rebase meisterlikud lõksud!",
                Description = "Metsa Boss Rebane on seadnud kavalaid lõkse. Murra tema strateegiad järjest läbi!",
                BadgeText = "Bossi Lahing 👑",
                ThemeColor = "#FFC800",
                ThemeShadow = "#E5A000",
                Icon = "👑",
                RewardCoins = 250,
                RewardHearts = 2,
                DifficultyStars = 5,
                Tasks = new List<PuzzleTask>
                {
                    // 5.1: Rebase tsentrirünnak
                    new PuzzleTask
                    {
                        TaskNumber = 1,
                        Title = "Rebase tsentrirünnak",
                        Instruction = "Hõiva keskruut ja murra Rebase esimene kaitseliin!",
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.O, PlayerSymbol.None, PlayerSymbol.X },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.O },
                            { PlayerSymbol.None, PlayerSymbol.X, PlayerSymbol.None }
                        },
                        SolutionSteps = new List<PuzzleStep>
                        {
                            new PuzzleStep { ExpectedRow = 1, ExpectedCol = 1, BotResponseRow = 2, BotResponseCol = 0 },
                            new PuzzleStep { ExpectedRow = 0, ExpectedCol = 1 }
                        }
                    },
                    // 5.2: Rebase rünnaku tõrje
                    new PuzzleTask
                    {
                        TaskNumber = 2,
                        Title = "Rebase rünnaku tõrje",
                        Instruction = "Peata Rebase vasak rünnak ja pööra seis enda kasuks!",
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.O, PlayerSymbol.None, PlayerSymbol.None },
                            { PlayerSymbol.O, PlayerSymbol.X, PlayerSymbol.None },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.X }
                        },
                        SolutionSteps = new List<PuzzleStep>
                        {
                            new PuzzleStep { ExpectedRow = 2, ExpectedCol = 0, BotResponseRow = 2, BotResponseCol = 1 },
                            new PuzzleStep { ExpectedRow = 0, ExpectedCol = 2 }
                        }
                    },
                    // 5.3: Rebase kilbi purustamine
                    new PuzzleTask
                    {
                        TaskNumber = 3,
                        Title = "Rebase kilbi purustamine",
                        Instruction = "Loo topeltrünnak paremale ja murra Rebase vastupanu!",
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.None, PlayerSymbol.X, PlayerSymbol.None },
                            { PlayerSymbol.O, PlayerSymbol.O, PlayerSymbol.X },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.None }
                        },
                        SolutionSteps = new List<PuzzleStep>
                        {
                            new PuzzleStep { ExpectedRow = 0, ExpectedCol = 2, BotResponseRow = 0, BotResponseCol = 0 },
                            new PuzzleStep { ExpectedRow = 2, ExpectedCol = 2 }
                        }
                    },
                    // 5.4: Rebase nurgapüünis
                    new PuzzleTask
                    {
                        TaskNumber = 4,
                        Title = "Rebase nurgapüünis",
                        Instruction = "Aseta sümbol nurka ja sunni Rebane taanduma!",
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.X, PlayerSymbol.None, PlayerSymbol.O },
                            { PlayerSymbol.None, PlayerSymbol.O, PlayerSymbol.None },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.X }
                        },
                        SolutionSteps = new List<PuzzleStep>
                        {
                            new PuzzleStep { ExpectedRow = 2, ExpectedCol = 0, BotResponseRow = 1, BotResponseCol = 0 },
                            new PuzzleStep { ExpectedRow = 2, ExpectedCol = 1 }
                        }
                    },
                    // 5.5: Kavala Rebase matt!
                    new PuzzleTask
                    {
                        TaskNumber = 5,
                        Title = "Kavala Rebase matt!",
                        Instruction = "Peata Rebase rünnak ja vii ellu peatüki lõplik võidukombinatsioon!",
                        InitialBoard = new PlayerSymbol[3, 3]
                        {
                            { PlayerSymbol.O, PlayerSymbol.None, PlayerSymbol.None },
                            { PlayerSymbol.O, PlayerSymbol.X, PlayerSymbol.None },
                            { PlayerSymbol.None, PlayerSymbol.None, PlayerSymbol.X }
                        },
                        SolutionSteps = new List<PuzzleStep>
                        {
                            new PuzzleStep { ExpectedRow = 2, ExpectedCol = 0, BotResponseRow = 2, BotResponseCol = 1 },
                            new PuzzleStep { ExpectedRow = 0, ExpectedCol = 2 }
                        }
                    }
                }
            }
        };
    }

    public static AdventurePuzzle? GetPuzzleById(int id)
    {
        return GetPuzzles().FirstOrDefault(p => p.Id == id);
    }

    public static bool CompleteLevel(int levelId)
    {
        var puzzle = GetPuzzleById(levelId);
        if (puzzle == null) return false;

        Preferences.Default.Set($"{KeyLevelCompletedPrefix}{levelId}", true);

        // Открываем следующий уровень, если он еще не открыт
        if (levelId >= HighestUnlockedLevel && levelId < 5)
        {
            HighestUnlockedLevel = levelId + 1;
        }

        // Выдаем награды, если еще не забирали
        if (!IsRewardClaimed(levelId))
        {
            Preferences.Default.Set($"{KeyLevelRewardClaimedPrefix}{levelId}", true);
            ShopService.AddCoins(puzzle.RewardCoins);
            if (puzzle.RewardHearts > 0)
            {
                LivesService.AddLives(puzzle.RewardHearts);
            }
        }

        OnAdventureProgressChanged?.Invoke();
        return true;
    }

    public static void ResetProgress()
    {
        Preferences.Default.Remove(KeyHighestUnlockedLevel);
        for (int i = 1; i <= 5; i++)
        {
            Preferences.Default.Remove($"{KeyLevelCompletedPrefix}{i}");
            Preferences.Default.Remove($"{KeyLevelRewardClaimedPrefix}{i}");
        }
        OnAdventureProgressChanged?.Invoke();
    }
}
