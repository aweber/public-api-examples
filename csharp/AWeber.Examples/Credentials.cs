using System;
using Newtonsoft.Json;

namespace AWeber.Examples
{
    public class Credentials : AccessTokenResponse
    {
        public Credentials()
        {
        }

        public Credentials(AccessTokenResponse accessTokenResponse, string clientId, string clientSecret)
        {
            RefreshToken = accessTokenResponse.RefreshToken;
            AccessToken = accessTokenResponse.AccessToken;
            TokenType = accessTokenResponse.TokenType;
            ExpiresIn = accessTokenResponse.ExpiresIn;
            ClientId = clientId;
            ClientSecret = clientSecret;
            CreatedOn = DateTime.Now;
        }

        [JsonProperty("created_on")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }
    }
}