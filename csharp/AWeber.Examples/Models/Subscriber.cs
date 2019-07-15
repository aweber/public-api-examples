using System.Collections.Generic;
using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class Subscriber
    {
        [JsonProperty("ad_tracking")] public string AdTracking { get; set; }
        [JsonProperty("area_code")] public int? AreaCode { get; set; }
        [JsonProperty("city")] public string City { get; set; }
        [JsonProperty("country")] public string Country { get; set; }
        [JsonProperty("custom_fields")] public IDictionary<string, string> CustomFields { get; set; }
        [JsonProperty("dma_code")] public int? DmaCode { get; set; }
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("ip_address")] public string IpAddress { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("self_link")] public string SelfLink { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
    }
}