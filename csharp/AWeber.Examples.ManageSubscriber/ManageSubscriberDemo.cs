using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AWeber.Examples.Models;
using McMaster.Extensions.CommandLineUtils;

namespace AWeber.Examples.ManageSubscriber
{
    public class ManageSubscriberDemo : BaseDemo
    {
        public ManageSubscriberDemo(HttpClient httpClient, IConsole console, AuthenticationHelper authHelper) : base(httpClient, console, authHelper)
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

            // Find out if a subscriber exists on the first list
            var email = "example@example.com";
            var subscriberParams = new Dictionary<string, string>
            {
                {"email", email},
                {"ws.op", "find"}
            };
            var subscribersUrl = lists.First().SubscribersCollectionLink;
            var findUrl = string.Format("{0}?{1}", subscribersUrl, subscriberParams.ToUrlParams());
            var foundSubscribers = await GetCollectionAsync<Subscriber>(accessToken, findUrl);
            Console.WriteJson("Found subscribers", foundSubscribers);

            var subscriber = foundSubscribers.FirstOrDefault();
            string subscriberUrl;
            if (subscriber?.SelfLink != null)
            {
                // update subscriber if they are on the first list
                var updateSubscriber = new UpdateSubscriber
                {
                    CustomFields = new Dictionary<string, string> {{"awesomeness", "really awesome"}},
                    Tags = new Dictionary<string, IList<string>> {{"add", new List<string> {"prospect"}}},
                    Status = "subscribed"
                };
                subscriberUrl = subscriber.SelfLink;
                subscriber = await UpdateAsync<UpdateSubscriber, Subscriber>(updateSubscriber, accessToken, subscriberUrl);
                Console.WriteJson("Updated Subscriber", subscriber);
            }
            else
            {
                // add the subscriber if they are not already on the first list
                var addSubscriber = new AddSubscriber
                {
                    Email = email,
                    CustomFields = new Dictionary<string, string> {{"awesomeness", "somewhat"}},
                    Tags = new List<string> {"prospect"}
                };
                subscriberUrl = (await CreateAsync(addSubscriber, accessToken, subscribersUrl)).AbsoluteUri;
                subscriber = await GetAsync<Subscriber>(accessToken, subscriberUrl);
                Console.WriteJson("Created Subscriber", subscriber);
            }

            // get the activity for the subscriber
            var activityParams = new Dictionary<string, string>
            {
                {"ws.op", "getActivity"}
            };
            var activityUrl = string.Format("{0}?{1}", subscriberUrl, activityParams.ToUrlParams());
            var activity = await GetAsync<Activity>(accessToken, activityUrl);
            Console.WriteJson("Subscriber Activity", activity);

            // delete the subscriber; this can only be performed on confirmed subscribers
            // or a 405 Method Not Allowed will be returned
            if (subscriber.Status == "subscribed")
            {
                await DeleteAsync(accessToken, subscriberUrl);
                Console.WriteResponse(ConsoleColor.Green, "Deleted subscriber with email: {0}", email);
            }
        }
    }
}