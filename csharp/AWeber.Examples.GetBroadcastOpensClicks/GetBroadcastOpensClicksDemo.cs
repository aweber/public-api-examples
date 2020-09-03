using System;
using System.Collections.Generic;
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
            var accountUrl = accounts.First().SelfLink;

            // Get all the list entries for the first account
            var listsUrl = accounts.First().ListsCollectionLink;
            var lists = await GetCollectionAsync<List>(accessToken, listsUrl);

            // get a sent broadcast
            var campaignParams = new Dictionary<string, string>
            {
                {"ws.op", "find"},
                {"campaign_type", "b"}
            };
            var campaignsUrl = lists.First().CampaignsCollectionLink;
            var broadcastsUrl = string.Format("{0}?{1}", campaignsUrl, campaignParams.ToUrlParams());
            var sentBroadcasts = await GetCollectionAsync<Campaign>(accessToken, broadcastsUrl);
            var broadcast = sentBroadcasts.First();
            Console.WriteJson("Broadcast", broadcast);


            // mapping of subscriber url to email address
            var subscriberCache = new Dictionary<string, string>();

            var links = await GetCollectionAsync<Link>(accessToken, broadcast.LinksCollectionLink);

            Console.WriteResponse(ConsoleColor.Green, "Clicks for broadcast:");
            foreach (var link in links)
            {
                Console.WriteResponse(ConsoleColor.Green, "{0}", link.Url);
                var clicksUrl = link.ClicksCollectionLink;
                var clicks = await GetCollectionAsync<Click>(accessToken, clicksUrl);
                foreach (var click in clicks)
                {
                    var clickSubLink = click.SubscriberLink;
                    var cachedSubscriber = subscriberCache[clickSubLink];
                    string email;
                    if (!string.IsNullOrEmpty(cachedSubscriber))
                    {
                        email = cachedSubscriber;
                    }
                    else
                    {
                        var clickSub = await RetryAsync(GetAsync<Subscriber>(accessToken, clickSubLink));
                        email = clickSub.Email;
                    }
                    Console.WriteResponse(ConsoleColor.Green, "\t{0}: {1}", click.EventTime, email);
                }
            }
        }
    }
}
