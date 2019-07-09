using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AWeber.Examples.Models;
using McMaster.Extensions.CommandLineUtils;

namespace AWeber.Examples.FindSubscribersAcrossLists
{
    public class FindSubscribersAcrossListsDemo : BaseDemo
    {
        public FindSubscribersAcrossListsDemo(HttpClient httpClient, IConsole console, AuthenticationHelper authHelper) : base(httpClient, console, authHelper)
        {
        }

        public override async Task ExecuteAsync()
        {
            var accessToken = await GetAccessTokenAsync();

            // Get an account to search on
            const string accountsUrl = "https://api.aweber.com/1.0/accounts";
            var accounts = await GetCollectionAsync<Account>(accessToken, accountsUrl);
            var accountUrl = accounts.First().SelfLink; // choose the first account

            // Find all subscribers across lists that match an email
            var subscriberParams = new Dictionary<string, string>
            {
                {"email", "example@example.com"},
                {"ws.op", "findSubscribers"}
            };
            var findUrl = string.Format("{0}?{1}", accountUrl, subscriberParams.ToUrlParams());
            var foundSubscribers = await GetCollectionAsync<Subscriber>(accessToken, findUrl);
            Console.WriteResponse(ConsoleColor.Green, "{0} Subscribers found.", foundSubscribers.Count);
            foreach (var foundSubscriber in foundSubscribers)
            {
                Console.WriteResponse(ConsoleColor.Green, "\tSubscriber[{0}] - Name: {1}, Email: {2}, City, State: {3}, {4}", foundSubscriber.Id, foundSubscriber.Name, foundSubscriber.Email, foundSubscriber.City, foundSubscriber.Country);
            }
        }
    }
}