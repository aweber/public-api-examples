using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class CreateResponse
    {
        [JsonProperty("self_link")] public string SelfLink { get; set; }
    }
}