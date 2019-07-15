using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AWeber.Examples.Models;
using Newtonsoft.Json;

namespace AWeber.Examples
{
    public class AuthenticationHelper
    {
        private const string GrantType = "grant_type";
        private const string AuthorizationCode = "authorization_code";
        private const string RefreshToken = "refresh_token";
        private const string RedirectUri = "redirect_uri";
        private const string Code = "code";
        private const string State = "state";

        private readonly HttpClient _httpClient;
        private readonly string _oauthUri;
        private readonly string _redirectUri;

        public AuthenticationHelper(HttpClient httpClient, string oauthUri, string redirectUri)
        {
            _httpClient = httpClient;
            _oauthUri = oauthUri;
            _redirectUri = redirectUri;
        }

        public async Task<AccessTokenResponse> MakeAccessTokenRequestAsync(string code, string clientId, string clientSecret)
        {
            var formValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(GrantType, AuthorizationCode),
                new KeyValuePair<string, string>(RedirectUri, _redirectUri),
                new KeyValuePair<string, string>(Code, code)
            };
            var request = new HttpRequestMessage(HttpMethod.Post, string.Format("{0}/token", _oauthUri));
            request.Headers.Authorization = CreateBasicAuthHeader(clientId, clientSecret);
            request.Content = new FormUrlEncodedContent(formValues);
            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("API request failed. Error: {0}", JsonConvert.DeserializeObject<SimpleApiError>(responseContent)));
            }
            return JsonConvert.DeserializeObject<AccessTokenResponse>(responseContent);
        }

        public async Task<AccessTokenResponse> MakeRefreshTokenRequestAsync(string refreshToken, string clientId, string clientSecret)
        {
            var formValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(GrantType, RefreshToken),
                new KeyValuePair<string, string>(RefreshToken, refreshToken)
            };
            var request = new HttpRequestMessage(HttpMethod.Post, string.Format("{0}/token", _oauthUri));
            request.Headers.Authorization = CreateBasicAuthHeader(clientId, clientSecret);
            request.Content = new FormUrlEncodedContent(formValues);
            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("API request failed. Error: {0}", JsonConvert.DeserializeObject<SimpleApiError>(responseContent)));
            }
            return JsonConvert.DeserializeObject<AccessTokenResponse>(responseContent);
        }

        public AuthorizationUrl CreateAuthorizationUrl(IList<string> scopes, string clientId)
        {
            var state = CreateStateTokenForOAuth();
            var url = string.Format(
                "{0}/authorize?response_type={1}&client_id={2}&redirect_uri={3}&scope={4}&state={5}",
                _oauthUri,
                Code,
                clientId,
                WebUtility.UrlEncode(_redirectUri),
                WebUtility.UrlEncode(string.Join(" ", scopes)),
                state);
            return new AuthorizationUrl {Url = url, State = state};
        }

        public string ParseAuthorizationCode(string authorizationCodeResponse, string state)
        {
            var match = Regex.Match(authorizationCodeResponse, string.Format("{0}=(?<{0}>\\w*)&{1}=(?<{1}>\\w*)", Code, State));
            if (!match.Groups[State].Value.Equals(state))
            {
                throw new Exception("Authorization Error: State does not match");
            }
            return match.Groups[Code].Value;
        }

        public void SaveAccessToken(Credentials credentials, string filePath)
        {
            if (!Directory.Exists(filePath))
            {
                var directoryName = Path.GetDirectoryName(filePath);
                Directory.CreateDirectory(directoryName);
            }
            File.WriteAllText(filePath, JsonConvert.SerializeObject(credentials));
        }

        public async Task<string> RetrieveAccessTokenAsync(string credentialsFilePath)
        {
            var credentials = JsonConvert.DeserializeObject<Credentials>(File.ReadAllText(credentialsFilePath));

            if (credentials.CreatedOn.AddSeconds(credentials.ExpiresIn) >= DateTime.Now)
            {
                return credentials.AccessToken;
            }

            var tokenResponse = await MakeRefreshTokenRequestAsync(credentials.RefreshToken, credentials.ClientId, credentials.ClientSecret);
            var newCredentials = new Credentials(tokenResponse, credentials.ClientId, credentials.ClientSecret);
            SaveAccessToken(newCredentials, credentialsFilePath);
            return newCredentials.AccessToken;
        }

        private static string CreateStateTokenForOAuth()
        {
            return Guid.NewGuid().ToString("N");
        }

        private static AuthenticationHeaderValue CreateBasicAuthHeader(string clientId, string clientSecret)
        {
            var byteArray = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", clientId, clientSecret));
            var base64String = Convert.ToBase64String(byteArray);
            return new AuthenticationHeaderValue("Basic", base64String);
        }
    }
}