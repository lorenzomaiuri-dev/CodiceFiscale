using System.Text.RegularExpressions;

namespace CodiceFiscaleLib.Helpers;

public static class StringsHelper
{
    public static bool IsAlphaNumeric(string s)
    {
        bool isAlpha = s.Any(char.IsLetter);
        bool isNumeric = s.Any(char.IsDigit);

        return isAlpha && isNumeric;
    }

    // Method to slugify a string
    public static string Slugify(string input)
    {
        if (input == null)
        {
            return "";
        }
        return Regex.Replace(input.ToLower(), @"\s+", "-");
    }
}