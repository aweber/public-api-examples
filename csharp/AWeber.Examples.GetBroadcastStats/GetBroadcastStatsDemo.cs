using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AWeber.Examples.Models;
using McMaster.Extensions.CommandLineUtils;

namespace AWeber.Examples.GetBroadcastStats
{
    public class GetBroadcastStatsDemo : BaseDemo
    {
        public GetBroadcastStatsDemo(HttpClient httpClient, IConsole console, AuthenticationHelper authHelper) : base(httpClient, console, authHelper)
        {
        }

        public override async Task ExecuteAsync()
        {
            var accessToken = await GetAccessTokenAsync();

            // Get an account to manage custom fields on
            const string accountsUrl = "https://api.aweber.com/1.0/accounts";
            var accounts = await GetCollectionAsync<Account>(accessToken, accountsUrl);
            var accountUrl = accounts.First().SelfLink;

            // get all the list entries for the first account
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
            var broadcast = sentBroadcasts.First(); // get the first broadcast to retrieve stats

            Console.WriteResponse(ConsoleColor.Green, "Broadcast id: {0} Subject: {1} Sent At: {2}\n", broadcast.Id, broadcast.Subject, broadcast.SentAt);

            // get the stats for the first broadcast
            var stats = await GetCollectionAsync<BroadcastStatistic>(accessToken, broadcast.StatsCollectionLink);

            // look at every stat for the first broadcast
            foreach (var stat in stats)
            {
                // stats entries can be in many formats
                // daily stats are shown over 14 days
                // hourly stats are shown over 24 hours
                // some stats show the top 10 (e.g. webhits_by_link)
                // some stats show total counts (e.g. total_clicks)
                if (stat is ListStatistic)
                {
                    Console.WriteResponse(ConsoleColor.Green, "\n{0}:\n", stat.Description);
                    foreach (var values in (stat as ListStatistic).Value)
                    {
                        Console.WriteResponse(ConsoleColor.Green, "\n");  // add an extra newline between each item
                        foreach (var value in values)
                        {
                            Console.WriteResponse(ConsoleColor.Green, "\t{0}: {1}\n", value.Key, value.Value);
                        }
                    }
                }
                else if (stat is IntegerStatistic)
                {
                    Console.WriteResponse(ConsoleColor.Green, "\n{0}: {1}\n", stat.Description, (stat as IntegerStatistic).Value);
                }
                else if (stat is DecimalStatistic)
                {
                    Console.WriteResponse(ConsoleColor.Green, "\n{0}: {1}\n", stat.Description, (stat as DecimalStatistic).Value);
                }
                else
                {
                    Console.WriteResponse(ConsoleColor.Green, "\n{$0}: N/A\n", stat.Description);
                }
            }
        }
    }
}