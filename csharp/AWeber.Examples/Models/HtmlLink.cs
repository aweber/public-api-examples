using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class HtmlLink
    {
        [JsonProperty("href")] public string Href { get; set; }
        [JsonProperty("rel")] public string Relationship { get; set; }
    }
}