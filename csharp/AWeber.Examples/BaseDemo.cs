using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AWeber.Examples.Models;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace AWeber.Examples
{
    public abstract class BaseDemo
    {
        protected HttpClient Client { get; }
        protected IConsole Console { get; }
        private readonly AuthenticationHelper _authHelper;
        private readonly JsonSerializerSettings _serializerSettings;

        // Each retry after a rate limit response will exponentially back off. This max
        // retries should be 6 or greater which will ensure that the final attempt will
        // be in the next minute and escape the rate limit window. (2⁶ = 64 seconds)
        private const int MaxRetries = 6;

        protected BaseDemo(HttpClient httpClient, IConsole console, AuthenticationHelper authHelper)
        {
            Client = httpClient;
            Console = console;
            _authHelper = authHelper;
            _serializerSettings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            _serializerSettings.Converters.Add(new BroadcastStatisticConverter());
        }

        public abstract Task ExecuteAsync();

        protected async Task<string> GetAccessTokenAsync()
        {
            var credentialsFilePath = Path.Combine(Path.GetTempPath(), "AWeber", "credentials.json");
            return await _authHelper.RetrieveAccessTokenAsync(credentialsFilePath);
        }

        protected async Task<T> RetryAsync<T>(Task<T> getTask, int retryCount = 1)
        {
            Exception exception = null;
            foreach (var retry in Enumerable.Range(1, MaxRetries))
            {
                try
                {
                    return await getTask;
                }
                catch (ApiException ex)
                {
                    exception = ex;

                    // Only retry on a 403 (forbidden) status code with a rate limit error
                    if (ex.StatusCode != HttpStatusCode.Forbidden)
                    {
                        throw ex;
                    }

                    if (!ex.Error.Message.Contains("rate limit"))
                    {
                        throw ex;
                    }

                    Console.WriteResponse(ConsoleColor.Magenta, "Request was rate limited");
                    if (retry < MaxRetries)
                    {
                        // Wait longer between every attempt
                        await Task.Delay((int) Math.Pow(2, retry) * 1000);
                        Console.WriteResponse(ConsoleColor.Magenta, "Retry #{0}...", retry);
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }
            Console.WriteResponse(ConsoleColor.Magenta, "Giving up after {0} tries", MaxRetries);
            throw exception;
        }

        protected async Task<IList<T>> GetCollectionAsync<T>(string accessToken, string url) where T : class
        {
            var entries = new List<T>();
            var hasNextLink = true;
            while (hasNextLink)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await Client.SendAsync(request);
                var contentString = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    ThrowError(response.StatusCode, contentString);
                }
                var content = JsonConvert.DeserializeObject<CollectionResponse<T>>(contentString, _serializerSettings);
                entries.AddRange(content.Entries);
                hasNextLink = !string.IsNullOrWhiteSpace(content.NextCollectionLink);
            }

            return entries;
        }

        protected async Task<T> GetAsync<T>(string accessToken, string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await Client.SendAsync(request);
            var contentString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                ThrowError(response.StatusCode, contentString);
            }
            return JsonConvert.DeserializeObject<T>(contentString, _serializerSettings);
        }

        protected async Task<TResponse> UpdateAsync<TRequest, TResponse>(TRequest payload, string accessToken, string url)
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(JsonConvert.SerializeObject(payload, _serializerSettings), Encoding.UTF8);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await Client.SendAsync(request);
            var contentString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                ThrowError(response.StatusCode, contentString);
            }
            return JsonConvert.DeserializeObject<TResponse>(contentString);
        }

        protected async Task<Uri> CreateAsync<TRequest>(TRequest payload, string accessToken, string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(JsonConvert.SerializeObject(payload, _serializerSettings), Encoding.UTF8);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await Client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var contentString = await response.Content.ReadAsStringAsync();
                ThrowError(response.StatusCode, contentString);
            }
            return response.Headers.Location;
        }

        protected async Task<Uri> CreateFormAsync(IDictionary<string, string> payload, string accessToken, string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new FormUrlEncodedContent(payload);
            var response = await Client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var contentString = await response.Content.ReadAsStringAsync();
                ThrowError(response.StatusCode, contentString);
            }
            return response.Headers.Location;
        }

        protected async Task<T> CreateFormAsync<T>(IDictionary<string, string> payload, string accessToken, string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new FormUrlEncodedContent(payload);
            var response = await Client.SendAsync(request);
            var contentString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                ThrowError(response.StatusCode, contentString);
            }
            return JsonConvert.DeserializeObject<T>(contentString);
        }

        protected async Task DeleteAsync(string accessToken, string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await Client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var contentString = await response.Content.ReadAsStringAsync();
                ThrowError(response.StatusCode, contentString);
            }
        }

        private static void ThrowError(HttpStatusCode statusCode, string contentString)
        {
            throw new ApiException(statusCode, JsonConvert.DeserializeObject<ErrorWrapper>(contentString).Error);
        }
    }
}