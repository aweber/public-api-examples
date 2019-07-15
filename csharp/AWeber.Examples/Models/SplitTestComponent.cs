using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class SplitTestComponent : Webform
    {
        [JsonProperty("web_form_link")] public string WebFormLink { get; set; }
        [JsonProperty("weight")] public int Weight { get; set; }
    }
}