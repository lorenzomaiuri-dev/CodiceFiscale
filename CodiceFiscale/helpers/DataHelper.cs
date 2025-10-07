using System.Text.Json;
using CodiceFiscaleLib.Models;

namespace CodiceFiscaleLib.Helpers;

public static class DataHelper
{
	// Function to load data
    public static Dictionary<string, Dictionary<string, List<Dictionary<string, object>>>> INDEXED_DATA = GetIndexedData();

   	// Returns the path to the data directory
  	public static string GetDataBaseDir()
	{
		// Returns the output directory
		return AppContext.BaseDirectory;
	}

   	// Read a JSON file and get the content
	public static T GetData<T>(string filename)
   	{
		string filePath = Path.Combine(GetDataBaseDir(), "data", filename);
		string jsonContent = File.ReadAllText(filePath);
		T? serializedData = JsonSerializer.Deserialize<T>(jsonContent);
		if (serializedData == null)
		{
			throw new InvalidOperationException($"Failed to deserialize JSON file: {filename}");
		}
		return serializedData;
	}

   	// Returns municipalities data (municipalities.json)
   	public static List<Municipality> GetMunicipalitiesData()
	{
   		return GetData<List<Municipality>>("municipalities.json");
   	}

	// Returns countries data (countries.json + deleted-countries.json)
	public static List<Country> GetCountriesData()
	{
		var deletedCountries = GetData<List<Country>>("deleted-countries.json");
		var countries = GetData<List<Country>>("countries.json");
		return deletedCountries.Concat(countries).ToList();
	}

	// Returns an indexed dict with municipalities, countries and related codes
	private static Dictionary<string, Dictionary<string, List<Dictionary<string, object>>>> GetIndexedData()
	{
		var municipalities = GetMunicipalitiesData();
		var countries = GetCountriesData();

		var data = new Dictionary<string, Dictionary<string, List<Dictionary<string, object>>>>()
		{
			{ "municipalities", new Dictionary<string, List<Dictionary<string, object>>>() },
			{ "countries", new Dictionary<string, List<Dictionary<string, object>>>() },
			{ "codes", new Dictionary<string, List<Dictionary<string, object>>>() }
		};

		// Indexes for municipalities
		foreach (var municipality in municipalities)
		{
			string code = municipality.Code;
			string province = municipality.Province.ToLower();
			var names = municipality.NameSlugs;

			foreach (var name in names)
			{
				string nameAndProvince = $"{name}-{province}";
				if (!data["municipalities"].ContainsKey(name)) data["municipalities"][name] = new List<Dictionary<string, object>>();
				if (!data["municipalities"].ContainsKey(nameAndProvince)) data["municipalities"][nameAndProvince] = new List<Dictionary<string, object>>();
				
				data["municipalities"][name].Add(municipality.ToDictionary());
				data["municipalities"][nameAndProvince].Add(municipality.ToDictionary());
			}
			if (!data["codes"].ContainsKey(code)) data["codes"][code] = new List<Dictionary<string, object>>();
			data["codes"][code].Add(municipality.ToDictionary());
		}

		// Indexes for countries
		foreach (var country in countries)
		{
			string code = country.Code;
			var names = country.NameSlugs;

			foreach (var name in names)
			{
				if (!data["countries"].ContainsKey(name)) data["countries"][name] = new List<Dictionary<string, object>>();
				data["countries"][name].Add(country.ToDictionary());
			}
			if (!data["codes"].ContainsKey(code)) data["codes"][code] = new List<Dictionary<string, object>>();
			data["codes"][code].Add(country.ToDictionary());
		}

		return data;

	}
}
