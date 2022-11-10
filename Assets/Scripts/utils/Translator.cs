using System;
using System.Collections.Generic;
using JetBrains.Annotations;

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
            "argentyna", new()
            {
                { Language.English, "Argentina"},
                { Language.Polish, "Argentyna"}
            }
        },
        {
            "peru", new()
            {
                { Language.English, "Peru"},
                { Language.Polish, "Peru"}
            }
        },
        {
            "brazylia", new()
            {
                { Language.English, "Brazil"},
                { Language.Polish, "Brazylia"}
            }
        },
        {
            "wenezuela", new()
            {
                { Language.English, "Venezuela"},
                { Language.Polish, "Wenezuela"}
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
