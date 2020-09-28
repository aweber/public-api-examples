using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AWeber.Examples.Models;
using McMaster.Extensions.CommandLineUtils;

namespace AWeber.Examples.GetBroadcastOpensClicks
{
    public class GetBroadcastOpensClicksDemo : BaseDemo
    {
        public GetBroadcastOpensClicksDemo(HttpClient httpClient, IConsole console, AuthenticationHelper authHelper) : base(httpClient, console, authHelper)
        {
        }

        public override async Task ExecuteAsync()
        {
            var accessToken = await GetAccessTokenAsync();

            // Get all the accounts entries
            const string accountsUrl = "https://api.aweber.com/1.0/accounts";
            var accounts = await GetCollectionAsync<Account>(accessToken, accountsUrl);

            // Get all the list entries for the first account
            var listsUrl = accounts.First().ListsCollectionLink;
            var lists = await GetCollectionAsync<List>(accessToken, listsUrl);

            // get a sent broadcast
            var broadcastsUrl = lists.First().SentBroadcastsLink;
            var sentBroadcasts = await GetCollectionAsync<Broadcast>(accessToken, broadcastsUrl);
            var broadcastUrl = sentBroadcasts.First().SelfLink;
            var broadcast = await RetryAsync(GetAsync<Broadcast>(accessToken, broadcastUrl));

            Console.WriteJson("Broadcast", broadcast);

            Console.WriteResponse(ConsoleColor.Green, "Opens for broadcast:");
            var opens = await GetCollectionAsync<BroadcastOpen>(accessToken, broadcast.OpensCollectionLink);
            foreach (var open in opens)
            {
                Console.WriteResponse(ConsoleColor.Green, "\t{0}: {1}", open.EventTime, open.Email);
            }

            Console.WriteResponse(ConsoleColor.Green, "Clicks for broadcast:");
            var clicks = await GetCollectionAsync<BroadcastClick>(accessToken, broadcast.ClicksCollectionLink);
            foreach (var click in clicks)
            {
                Console.WriteResponse(ConsoleColor.Green, "\t{0}: {1}", click.EventTime, click.Email);
            }
        }
    }
}
