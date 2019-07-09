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

            // A broadcast is the receipt of sending email messages to a list.
            // Messages are each email sent to a subscriber; they
            // can be opened and clicked by each subscriber multiple times.
            var messagesUrl = broadcast.MessagesCollectionLink;
            var messages = await GetCollectionAsync<Message>(accessToken, messagesUrl);

            // mapping of subscriber url to email address
            var subscriberCache = new Dictionary<string, string>();

            Console.WriteResponse(ConsoleColor.Green, "Opens for broadcast:");
            foreach (var message in messages)
            {
                if (message.TotalOpens > 0)
                {
                    // You could also paginate the opens collection of each message,
                    // but the open count and the last_opened timestamp are in the message entry
                    // so unless we need exact times of each non-unique open, we use the message entry.
                    var openSubLink = message.SubscriberLink;
                    var openSub = await RetryAsync(GetAsync<Subscriber>(accessToken, openSubLink));
                    Console.WriteResponse(ConsoleColor.Green, "\t{0}: {1}", message.LastOpened, openSub.Email);
                    // First time looking up a subscriber; save them for next time
                    subscriberCache[openSubLink] = openSub.Email;
                }
            }

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