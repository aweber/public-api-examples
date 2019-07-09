using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AWeber.Examples.Models;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace AWeber.Examples.GetTags
{
    public class GetTagsDemo : BaseDemo
    {
        public GetTagsDemo(HttpClient httpClient, IConsole console, AuthenticationHelper authHelper) : base(httpClient, console, authHelper)
        {
        }

        public override async Task ExecuteAsync()
        {
            var accessToken = await GetAccessTokenAsync();

            const string accountUrl = "https://api.aweber.com/1.0/accounts";
            var accounts = await GetCollectionAsync<Account>(accessToken, accountUrl);

            var listsUrl = accounts.First().ListsCollectionLink;
            const string listName = "sample subscribers";
            var findListUrl = string.Format("{0}?ws.op=find&name={1}", listsUrl, listName);
            var lists = await GetCollectionAsync<List>(accessToken, findListUrl);

            if (!string.IsNullOrWhiteSpace(lists.FirstOrDefault()?.SelfLink))
            {
                // choose the first list
                var tagUrl = string.Format("{0}/tags", lists.FirstOrDefault()?.SelfLink);
                var request = new HttpRequestMessage(HttpMethod.Get, tagUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await Client.SendAsync(request);
                var contentString = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(string.Format("API request failed. Error: {0}", JsonConvert.DeserializeObject<SimpleApiError>(contentString)));
                }
                var tags = JsonConvert.DeserializeObject<IList<string>>(contentString);
                foreach (var tag in tags)
                {
                    Console.WriteResponse(ConsoleColor.Green, string.Format("{0}", tag));
                }
            }
            else
            {
                Console.WriteResponse(ConsoleColor.Yellow, string.Format("Could not find a list with name: {0}", listName));
            }
        }
    }
}