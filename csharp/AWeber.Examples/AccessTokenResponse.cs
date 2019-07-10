using Newtonsoft.Json;

namespace AWeber.Examples
{
    public class AccessTokenResponse
    {
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; } // Time in seconds when access token will expire
    }
}