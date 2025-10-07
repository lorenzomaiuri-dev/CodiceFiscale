using System.Globalization;
using CodiceFiscaleLib.Config;

namespace CodiceFiscaleLib.Helpers;

public static class CodeExtractorsHelper
{
    // Method to find the consonants
    public static List<char> ExtractConsonants(string s)
    {
        return s.Where(c => Constants._CONSONANTS.Contains(c)).ToList();
    }

    // Method to find the vowels
    public static List<char> ExtractVowels(string s)
    {
        return s.Where(c => Constants._VOWELS.Contains(c)).ToList();
    }

    // Method to find the first three consonants and vowels
    public static string ExtractConsonantsAndVowels(List<char> consonants, List<char> vowels)
    {
        return new string(consonants.Take(3).Concat(vowels.Take(3)).Concat(Enumerable.Repeat('X', 3)).Take(3).ToArray()).ToUpper();
    }

    // Method to get a date from string or datetime
    public static DateTime? ExtractDate(object? date, string separator = "-")
    {
        if (date == null)
        {
            return null;
        }

        if (date is DateTime)
        {
            return ((DateTime)date).ToUniversalTime();
        }

        string dateString = date.ToString();
        string dateSlug = StringsHelper.Slugify(dateString);

        // Split the date
        var dateParts = dateSlug.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        if (dateParts.Length < 3)
        {
            return null; // It's not valid
        }

        // Check if the year is first
        bool yearFirst = dateParts[0].Length == 4 || dateParts[2].Length == 4;

        // Try various formats
        string[] formats = yearFirst
            ? new[] { "yyyy-MM-dd", "yyyy/M/d" }
            : new[] { "dd-MM-yyyy", "M/d/yyyy" };

        try
        {
            // Parse the date
            DateTime parsedDate = DateTime.ParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            return parsedDate.ToUniversalTime();
        }
        catch (FormatException)
        {
            return null;
        }
    }

    // Method to find the birthplace
    public static List<Dictionary<string, object>>? ExtractBirthplace(string birthplace, object? birthdate = null)
    {
        // slugify the birthplace
        var tmpBirthplaceSlug = StringsHelper.Slugify(birthplace);

        // if it's a alphanumeric code it must be uppercase, if it's a place name it must be lowercase
        var birthplaceSlug = (StringsHelper.IsAlphaNumeric(tmpBirthplaceSlug)) ? tmpBirthplaceSlug.ToUpper() : tmpBirthplaceSlug.ToLower();
        List<Dictionary<string, object>>? birthplacesOptions = null;

        // Check if the slug of the birthplace is in the data
        if (DataHelper.INDEXED_DATA["municipalities"].ContainsKey(birthplaceSlug))
        {
            birthplacesOptions = DataHelper.INDEXED_DATA["municipalities"][birthplaceSlug];
        }
        else if (DataHelper.INDEXED_DATA["countries"].ContainsKey(birthplaceSlug))
        {
            birthplacesOptions = DataHelper.INDEXED_DATA["countries"][birthplaceSlug];
        }
        else if (DataHelper.INDEXED_DATA["codes"].ContainsKey(birthplaceSlug))
        {
            birthplacesOptions = DataHelper.INDEXED_DATA["codes"][birthplaceSlug];
        }

        if (birthplacesOptions == null)
        {
            return null;
        }

        var birthdateDate = ExtractDate(birthdate);
        if (birthdateDate == null)
        {
            // If there is no birth date, it returns the first element of the list
            return new List<Dictionary<string, object>> { birthplacesOptions.First() };
        }

        foreach (var birthplaceOption in birthplacesOptions)
        {
            // Use the keys to get the values
            var dateCreated = ExtractDate(birthplaceOption.ContainsKey("date_created") ? birthplaceOption["date_created"] : null) ?? DateTime.MinValue;
            var dateDeleted = ExtractDate(birthplaceOption.ContainsKey("date_deleted") ? birthplaceOption["date_deleted"] : null) ?? DateTime.MaxValue;

            if (birthdateDate >= dateCreated && birthdateDate <= dateDeleted)
            {
                return new List<Dictionary<string, object>> { birthplaceOption };
            }
        }

        // Fallback if there is no match
        return ExtractBirthplaceFallback(birthplacesOptions, (DateTime)birthdateDate);
    }

    // Fallback method to get the birth place
    public static List<Dictionary<string, object>> ExtractBirthplaceFallback(
    List<Dictionary<string, object>> birthplacesOptions,
    DateTime birthdateDate)
    {
        // Create the list for the result
        List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

        if (birthplacesOptions.Count > 1)
        {
            for (int i = 0; i < birthplacesOptions.Count - 1; i++)
            {
                var birthplaceOption = birthplacesOptions[i];
                var birthplaceOptionNext = birthplacesOptions[i + 1];

                // Check if the keys exist and manage null values
                var dateDeleted = ExtractDate(birthplaceOption.ContainsKey("date_deleted") ? birthplaceOption["date_deleted"] : null);
                var dateCreated = ExtractDate(birthplaceOptionNext.ContainsKey("date_created") ? birthplaceOptionNext["date_created"] : null);

                if (dateDeleted.HasValue && dateCreated.HasValue)
                {
                    if (birthdateDate >= dateDeleted.Value && birthdateDate <= dateCreated.Value)
                    {
                        // Check if the keys exist and manage null values
                        var dateCreatedOption = ExtractDate(birthplaceOption.ContainsKey("date_created") ? birthplaceOption["date_created"] : null);
                        var dateDeletedOption = ExtractDate(birthplaceOption.ContainsKey("date_deleted") ? birthplaceOption["date_deleted"] : null);

                        if (dateCreatedOption.HasValue && dateDeletedOption.HasValue)
                        {
                            var delta = dateDeletedOption.Value - dateCreatedOption.Value;
                            if (delta.TotalDays <= 1)
                            {
                                result.Add(birthplaceOption);
                                return result;
                            }
                        }

                        result.Add(birthplaceOptionNext);

                        return result;
                    }
                }
            }
        }

        // Fallback if there is no match
        // Return an empty list
        return result;
    }

    // Method to get a single omocode
    public static string ExtractOmocode(string code, List<int> subs, Dictionary<int, int> trans)
    {
        // Get first 15 characters
        var codeChars = code.Substring(0, 15).ToCharArray();

        // Apply the substitution on the indexes
        foreach (var i in subs)
        {
            char currentChar = codeChars[i];

            // If the character exists in the transition map, do the substitution
            if (trans.ContainsKey((int)currentChar))
            {
                codeChars[i] = (char)trans[(int)currentChar];
            }
        }

        // Create the code with substitutions and add the CIN
        string newCode = new string(codeChars);
        string codeCin = EncodingHelper.EncodeCin(newCode).ToString();
        newCode += codeCin;

        return newCode;
    }

    // Method to get the omocodes
    public static List<string> ExtractOmocodes(string code)
    {
        // Get the root code with the first transformation
        string codeRoot = ExtractOmocode(code, Constants._OMOCODIA_SUBS_INDEXES, Constants._OMOCODIA_DECODE_TRANS);

        // Generate a list of omocodes with all the substitution combinations
        var codes = Constants._OMOCODIA_SUBS_INDEXES_COMBINATIONS.Select(subs =>
               ExtractOmocode(codeRoot, subs, Constants._OMOCODIA_ENCODE_TRANS)
           ).ToList();

        return codes;
    }
}