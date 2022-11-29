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