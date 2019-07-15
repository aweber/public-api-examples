using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AWeber.Examples.Models;
using McMaster.Extensions.CommandLineUtils;

namespace AWeber.Examples.GetClicks
{
    public class GetClicksDemo : BaseDemo
    {
        public GetClicksDemo(HttpClient httpClient, IConsole console, AuthenticationHelper authHelper) : base(httpClient, console, authHelper)
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

            // Get a followup or broadcast to get links for
            var campaignsUrl = list.CampaignsCollectionLink;
            var campaigns = await GetCollectionAsync<Campaign>(accessToken, campaignsUrl);
            var campaign = campaigns.First(); // choose the first campaign
            var type = CampaignTypeName(campaign.Type);
            Console.WriteResponse(ConsoleColor.Green, "Clicks for {0} \"{1}\" by link:\n", type, campaign.Subject);

            // Cache subscriber email addresses to avoid looking them up repeatedly
            var subscriberEmails = new Dictionary<string, string>();

            // Get all the links included in the message
            var linksUrl = campaign.LinksCollectionLink;
            var links = await GetCollectionAsync<Link>(accessToken, linksUrl);

            foreach (var link in links)
            {
                Console.WriteResponse(ConsoleColor.Green, "{0}\n", link.Url);

                // Get all the clicks for each link
                var clicks = await GetCollectionAsync<Click>(accessToken, link.ClicksCollectionLink);
                foreach (var click in clicks)
                {
                    // Look up the email address of each subscriber who clicked
                    var subscriberUrl = click.SubscriberLink;
                    if (!subscriberEmails.ContainsKey(subscriberUrl))
                    {
                        // First time looking up a subscriber, save the email for next time
                        var subscriber = await GetAsync<Subscriber>(accessToken, subscriberUrl);
                        subscriberEmails[subscriberUrl] = subscriber.Email;
                    }

                    var email = subscriberEmails[subscriberUrl];
                    Console.WriteResponse(ConsoleColor.Green, "\t{0}: {1}", click.EventTime, email);
                }
            }
        }

        private string CampaignTypeName(string campaignType)
        {
            if (campaignType == "b") {
                return "broadcast";
            }

            if (campaignType == "f") {
                return "followup";
            }

            return string.Format("\"{0}\" type campaign", campaignType);
        }
    }
}