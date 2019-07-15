using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AWeber.Examples.Models;
using McMaster.Extensions.CommandLineUtils;

namespace AWeber.Examples.CreateScheduleBroadcast
{
    public class CreateScheduleBroadcastDemo : BaseDemo
    {
        public CreateScheduleBroadcastDemo(HttpClient httpClient, IConsole console, AuthenticationHelper authHelper) : base(httpClient, console, authHelper)
        {
        }

        public override async Task ExecuteAsync()
        {
            var accessToken = await GetAccessTokenAsync();

            // get an account to search on
            const string accountUrl = "https://api.aweber.com/1.0/accounts";
            var accounts = await GetCollectionAsync<Account>(accessToken, accountUrl);

            // get a list to find broadcasts on
            var lists = await GetCollectionAsync<List>(accessToken, accounts.First().ListsCollectionLink);

            var excludedLists = new List<string>();
            var includedLists = new List<string>();
            var createBroadcastValues = new Dictionary<string, string>
            {
                // the broadcast html, this can be provided without body_text
                {"body_html", "<html><body>An html version of my email</body></html>"},
                // if provided, this will be the plain text version of the email
                // if this is not provided, it will be auto-generated based on the body_html
                {"body_text", "A plain text version of my email"},
                // this is the broadcast subject line
                {"subject", "This is an email created by the api!"},
                // if there are links in this broadcast, track them
                {"click_tracking_enabled", "true"},
                // include or exclude subscribers on other lists in this broadcast
                // these are lists of URI's or list links
                {"exclude_lists", excludedLists.Stringify()},
                {"include_lists", includedLists.Stringify()},
                // this means the broadcast will be available via a url after it's sent
                {"is_archived", "true"},
                // if notifications are enabled and notify_on_send is True, send and email
                // to the AWeber account holder when this broadcast stats' are available
                {"notify_on_send", "true"},
            };

            // if enabled, get the facebook url to share the broadcast to facebook
            var integrations = await GetCollectionAsync<Integration>(accessToken, accounts.First().IntegrationsCollectionLink);
            foreach (var integration in integrations)
            {
                if (integration.ServiceName.ToLower() == "facebook")
                {
                    // there could be multiple, so pick the first one we find and break
                    createBroadcastValues.Add("facebook_integration", integration.SelfLink);
                    break;
                }
            }

            // make the broadcast on the first list
            var broadcast = await CreateFormAsync<Broadcast>(createBroadcastValues, accessToken, string.Format("{0}/broadcasts", lists.First().SelfLink));

            // schedule the broadcast we made
            var scheduleFormValues = new Dictionary<string, string> {{"scheduled_for", DateTime.UtcNow.ToString("O")}};
            var scheduleResponse = await CreateFormAsync<CreateResponse>(scheduleFormValues, accessToken, string.Format("{0}/schedule", broadcast.SelfLink));

            // retrieve the scheduled broadcast to see the updated scheduled_for
            var scheduledBroadcast = await GetAsync<Broadcast>(accessToken, broadcast.SelfLink);

            Console.WriteResponse(ConsoleColor.Green,
                "Scheduled broadcast subject: {0} on list: {1} to be sent at: {2}",
                scheduledBroadcast.Subject,
                lists.First().Name,
                scheduledBroadcast.ScheduledFor);
        }
    }
}