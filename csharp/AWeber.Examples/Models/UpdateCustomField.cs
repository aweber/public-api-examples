using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class UpdateCustomField
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("is_subscriber_updateable")] public bool IsSubscriberUpdateable { get; set; }
    }
}