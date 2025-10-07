using System.Text.Json.Serialization;

namespace CodiceFiscaleLib.Models;

public class Municipality
{
   [JsonPropertyName("code")]
   public string? Code { get; set; }
 
   [JsonPropertyName("province")]
   public string? Province { get; set; }
 
   [JsonPropertyName("name_slugs")]
   public List<string>? NameSlugs { get; set; }

   // Convert the class to a dictionary
   public Dictionary<string, object> ToDictionary()
   {
   	return new Dictionary<string, object>()
   	{
         { "code", Code ?? string.Empty },
         { "province", Province ?? string.Empty },
         { "name_slugs", NameSlugs ?? new List<string>() }
   	};
   }
}