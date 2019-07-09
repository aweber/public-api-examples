using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AWeber.Examples.Models;
using McMaster.Extensions.CommandLineUtils;

namespace AWeber.Examples.GetBroadcastDetails
{
    public class GetBroadcastDetailsDemo : BaseDemo
    {
        public GetBroadcastDetailsDemo(HttpClient httpClient, IConsole console, AuthenticationHelper authHelper) : base(httpClient, console, authHelper)
        {
        }

        public override async Task ExecuteAsync()
        {
            var accessToken = await GetAccessTokenAsync();

            // Get an account to search on
            const string accountsUrl = "https://api.aweber.com/1.0/accounts";
            var accounts = await GetCollectionAsync<Account>(accessToken, accountsUrl);
            var account = accounts.First(); // choose the first account

            // Get a list to find broadcasts on
            var lists = await GetCollectionAsync<List>(accessToken, account.ListsCollectionLink);
            var listUrl = lists.First().SelfLink; // choose the first list

            // Get the broadcast totals
            foreach (var status in new List<string>{"draft", "scheduled", "sent"})
            {
                var totalUrl = string.Format("{0}/broadcasts/total?status={1}", listUrl, status);
                var total = await RetryAsync(GetAsync<BroadcastTotal>(accessToken, totalUrl));
                Console.WriteResponse(ConsoleColor.Green, "Total {0} broadcasts: {1}", status, total.TotalSize);
            }

            var list = lists.First();
            // Get the first broadcast in each category
            foreach (var status in new List<string> {"draft", "scheduled", "sent"})
            {
                var broadcastStatusLink = DetermineBroadcastStatusLink(list, status);
                var broadcasts = await GetCollectionAsync<Broadcast>(accessToken, broadcastStatusLink);
                var subject = broadcasts.Any() ? broadcasts.First().Subject : "N/A";
                Console.WriteResponse(ConsoleColor.Green, "First {0} broadcast subject: {1}", status, subject);
            }
        }

        private static string DetermineBroadcastStatusLink(List list, string status)
        {
            switch (status)
            {
                case "draft":
                    return list.DraftBroadcastsLink;
                case "scheduled":
                    return list.ScheduledBroadcastsLink;
                case "sent":
                    return list.SentBroadcastsLink;
                default:
                    throw new ArgumentException(string.Format("Status '{0}' is not valid.", status));
            }
        }
    }
}