using System.Collections.Generic;

public static class Translator
{
    public enum Language
    {
        Polish,
        English
    }

    public static Language ChosenLanguage = Language.English;

    private static readonly Dictionary<string, Dictionary<Language, string>> Fields = new()
    {
        {
            "argentyna", new Dictionary<Language, string>
            {
                { Language.English, "Argentina" },
                { Language.Polish, "Argentyna" }
            }
        },
        {
            "peru", new Dictionary<Language, string>
            {
                { Language.English, "Peru" },
                { Language.Polish, "Peru" }
            }
        },
        {
            "brazylia", new Dictionary<Language, string>
            {
                { Language.English, "Brazil" },
                { Language.Polish, "Brazylia" }
            }
        },
        {
            "wenezuela", new Dictionary<Language, string>
            {
                { Language.English, "Venezuela" },
                { Language.Polish, "Wenezuela" }
            }
        },
        {
            "ameryka centralna", new Dictionary<Language, string>
            {
                { Language.English, "Central America" },
                { Language.Polish, "Ameryka Centralna" }
            }
        },
        {
            "stany zjednoczone zachodnie", new Dictionary<Language, string>
            {
                { Language.English, "Western United States" },
                { Language.Polish, "Stany Zjednoczone Zachodnie" }
            }
        },
        {
            "stany zjednoczone wschodnie", new Dictionary<Language, string>
            {
                { Language.English, "Eastern United States" },
                { Language.Polish, "Stany Zjednoczone Wschodnie" }
            }
        },
        {
            "kanada wschodnia", new Dictionary<Language, string>
            {
                { Language.English, "Quebeck" },
                { Language.Polish, "Kanada Wschodnia" }
            }
        },
        {
            "ontario", new Dictionary<Language, string>
            {
                { Language.English, "Ontario" },
                { Language.Polish, "Ontario" }
            }
        },
        {
            "alberta", new Dictionary<Language, string>
            {
                { Language.English, "Alberta" },
                { Language.Polish, "Alberta" }
            }
        },
        {
            "grenlandia", new Dictionary<Language, string>
            {
                { Language.English, "Greenland" },
                { Language.Polish, "Grenlandia" }
            }
        },
        {
            "obszar północno zachodni", new Dictionary<Language, string>
            {
                { Language.English, "North West Territory" },
                { Language.Polish, "Obszar Północno-Zachodni" }
            }
        },
        {
            "alaska", new Dictionary<Language, string>
            {
                { Language.English, "Alaska" },
                { Language.Polish, "Alaska" }
            }
        },
        {
            "islandia", new Dictionary<Language, string>
            {
                { Language.English, "Iceland" },
                { Language.Polish, "Islandia" }
            }
        },
        {
            "skandynawia", new Dictionary<Language, string>
            {
                { Language.English, "Scandinavia" },
                { Language.Polish, "Skandynawia" }
            }
        },
        {
            "wielka brytania", new Dictionary<Language, string>
            {
                { Language.English, "Great Britain" },
                { Language.Polish, "Wielka Brytania" }
            }
        },
        {
            "ukraina", new Dictionary<Language, string>
            {
                { Language.English, "Ukraine" },
                { Language.Polish, "Ukraina" }
            }
        },
        {
            "europa północna", new Dictionary<Language, string>
            {
                { Language.English, "Northern Europe" },
                { Language.Polish, "Europa Północna" }
            }
        },
        {
            "europa zachodnia", new Dictionary<Language, string>
            {
                { Language.English, "Western Europe" },
                { Language.Polish, "Europa Zachodnia" }
            }
        },
        {
            "europa południowa", new Dictionary<Language, string>
            {
                { Language.English, "Southern Europe" },
                { Language.Polish, "Europa Południowa" }
            }
        },
        {
            "afryka północna", new Dictionary<Language, string>
            {
                { Language.English, "North Africa" },
                { Language.Polish, "Afryka Północna" }
            }
        },
        {
            "egipt", new Dictionary<Language, string>
            {
                { Language.English, "Egypt" },
                { Language.Polish, "Egipt" }
            }
        },
        {
            "afryka wschodnia", new Dictionary<Language, string>
            {
                { Language.English, "East Afica" },
                { Language.Polish, "Afryka Wschodnia" }
            }
        },
        {
            "kongo", new Dictionary<Language, string>
            {
                { Language.English, "Congo" },
                { Language.Polish, "Kongo" }
            }
        },
        {
            "afryka południowa", new Dictionary<Language, string>
            {
                { Language.English, "South Africa" },
                { Language.Polish, "Afryka Południowa" }
            }
        },
        {
            "madagaskar", new Dictionary<Language, string>
            {
                { Language.English, "Madagascar" },
                { Language.Polish, "Madagaskar" }
            }
        },
        {
            "środkowy wschód", new Dictionary<Language, string>
            {
                { Language.English, "Middle East" },
                { Language.Polish, "Środkowy Wschód" }
            }
        },
        {
            "afganistan", new Dictionary<Language, string>
            {
                { Language.English, "Afghanistan" },
                { Language.Polish, "Afganistan" }
            }
        },
        {
            "ural", new Dictionary<Language, string>
            {
                { Language.English, "Ural" },
                { Language.Polish, "Ural" }
            }
        },
        {
            "syberia", new Dictionary<Language, string>
            {
                { Language.English, "Siberia" },
                { Language.Polish, "Syberia" }
            }
        },
        {
            "jakuck", new Dictionary<Language, string>
            {
                { Language.English, "Yakutsk" },
                { Language.Polish, "Jakuck" }
            }
        },
        {
            "kamczatka", new Dictionary<Language, string>
            {
                { Language.English, "Kamchatka" },
                { Language.Polish, "Kamczatka" }
            }
        },
        {
            "irkuck", new Dictionary<Language, string>
            {
                { Language.English, "Irkutsk" },
                { Language.Polish, "Irkuck" }
            }
        },
        {
            "japonia", new Dictionary<Language, string>
            {
                { Language.English, "Japan" },
                { Language.Polish, "Japonia" }
            }
        },
        {
            "mongolia", new Dictionary<Language, string>
            {
                { Language.English, "Mongolia" },
                { Language.Polish, "Mongolia" }
            }
        },
        {
            "chiny", new Dictionary<Language, string>
            {
                { Language.English, "China" },
                { Language.Polish, "Chiny" }
            }
        },
        {
            "indie", new Dictionary<Language, string>
            {
                { Language.English, "India" },
                { Language.Polish, "Indie" }
            }
        },
        {
            "syjam", new Dictionary<Language, string>
            {
                { Language.English, "Siam" },
                { Language.Polish, "Syjam" }
            }
        },
        {
            "indonezja", new Dictionary<Language, string>
            {
                { Language.English, "Indonesia" },
                { Language.Polish, "Indonezja" }
            }
        },
        {
            "nowa gwinea", new Dictionary<Language, string>
            {
                { Language.English, "New Guinea" },
                { Language.Polish, "Nowa Gwinea" }
            }
        },
        {
            "australia wschodnia", new Dictionary<Language, string>
            {
                { Language.English, "Easter Australia" },
                { Language.Polish, "Australia Wschodnia" }
            }
        },
        {
            "australia zachodnia", new Dictionary<Language, string>
            {
                { Language.English, "Western Australia" },
                { Language.Polish, "Australia Zachodnia" }
            }
        }
    };

    public static string TranslateField(string fieldName)
    {
        return Fields[fieldName][ChosenLanguage];
    }

    public static string TranslateIncome(int income)
    {
        return ChosenLanguage switch
        {
            Language.English => "Income: " + income,
            Language.Polish => "Przychód: " + income,
            _ => "How did we get here?"
        };
    }

    public static string TranslateIncome()
    {
        return ChosenLanguage switch
        {
            Language.English => "Income: ???",
            Language.Polish => "Przychód: ???",
            _ => "How did we get here?"
        };
    }

    public static string TranslateOwner(string owner)
    {
        return ChosenLanguage switch
        {
            Language.English => "Owner: " + (owner ?? "unconquered land"),
            Language.Polish => "Władca: " + (owner ?? "ziemia niczyja"),
            _ => "How did we get here?"
        };
    }
}