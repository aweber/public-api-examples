using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class SimpleApiError
    {
        public string Error { get; set; }
        [JsonProperty("error_description")]
        public string Description { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Error, Description);
        }
    }
}