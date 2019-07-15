using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AWeber.Examples.Models;
using McMaster.Extensions.CommandLineUtils;

namespace AWeber.Examples.MoveSubscriber
{
    public class MoveSubscriberDemo : BaseDemo
    {
        public MoveSubscriberDemo(HttpClient httpClient, IConsole console, AuthenticationHelper authHelper) : base(httpClient, console, authHelper)
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

            // pick the list to move the subscriber from and to
            var originList = lists.FirstOrDefault();
            var destinationList = lists.Skip(1).FirstOrDefault();

            if (originList == null || destinationList == null)
            {
                Console.WriteError("You must have 2 lists to move a subscriber!\n");
                return;
            }

            // get all the subscribers from the first list
            var subscribers = await GetCollectionAsync<Subscriber>(accessToken, originList.SubscribersCollectionLink);

            // pick the subscriber we want to move
            var subscriber = subscribers.FirstOrDefault();

            if (subscriber == null)
            {
                Console.WriteError("You must have a subscriber on list: {0}!\n", originList.Name);
                return;
            }

            var moveParams = new Dictionary<string, string>
            {
                {"ws.op", "move"},
                {"list_link", destinationList.SelfLink}
            };
            var subscriberUrl = await CreateAsync(moveParams, accessToken, subscriber.SelfLink);
            Console.WriteResponse(ConsoleColor.Green, "Moved subscriber {0} from list: {1}  to list: {2}", subscriber.Email, originList.Name, destinationList.Name);
        }
    }
}