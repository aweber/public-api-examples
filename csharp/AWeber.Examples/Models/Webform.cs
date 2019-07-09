using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class Webform
    {
        [JsonProperty("conversion_percentage")] public double ConversionPercentage { get; set; }
        [JsonProperty("html_source_link")] public string HtmlSourceLink { get; set; }
        [JsonProperty("http_etag")] public string HttpEtag { get; set; }
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("is_active")] public bool IsActive { get; set; }
        [JsonProperty("javascript_source_link")] public string JavascriptSourceLink { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("resource_type_link")] public string ResourceTypeLink { get; set; }
        [JsonProperty("self_link")] public string SelfLink { get; set; }
        [JsonProperty("total_displays")] public int TotalDisplays { get; set; }
        [JsonProperty("total_submissions")] public int TotalSubmissions { get; set; }
        [JsonProperty("total_unique_displays")] public int TotalUniqueDisplays { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("unique_conversion_percentage")] public double UniqueConversionPercentage { get; set; }
    }
}