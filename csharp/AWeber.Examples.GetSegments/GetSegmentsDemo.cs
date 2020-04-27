using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AWeber.Examples.Models;
using McMaster.Extensions.CommandLineUtils;

namespace AWeber.Examples.GetSegments
{
    public class GetSegmentsDemo : BaseDemo
    {
        public GetSegmentsDemo(HttpClient httpClient, IConsole console, AuthenticationHelper authHelper) : base(httpClient, console, authHelper)
        {
        }

        public override async Task ExecuteAsync()
        {
            var accessToken = await GetAccessTokenAsync();

            // Get an account to search on
            const string accountUrl = "https://api.aweber.com/1.0/accounts";
            var accounts = await GetCollectionAsync<Account>(accessToken, accountUrl);
            var account = accounts.First(); // choose the first account

            // Get a list to find messages on
            var lists = await GetCollectionAsync<List>(accessToken, account.ListsCollectionLink);
            var list = lists.First(); // choose the first list

            var segments = await GetCollectionAsync<Segment>(accessToken, list.SegmentsCollectionLink);
            Console.WriteResponse(ConsoleColor.Green, "Segments:");
            foreach (var segment in segments)
            {
                Console.WriteResponse(ConsoleColor.Green, string.Format("Service Name: {0}, Self Link: {1}", segment.Name, segment.SelfLink));
            }

        }
    }
}