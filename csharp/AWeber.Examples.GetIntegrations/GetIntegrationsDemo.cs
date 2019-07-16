using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AWeber.Examples.Models;
using McMaster.Extensions.CommandLineUtils;

namespace AWeber.Examples.GetIntegrations
{
    public class GetIntegrationsDemo : BaseDemo
    {
        private static readonly IList<string> TwitterAndFacebookIntegrations = new List<string> { "twitter", "facebook" };

        public GetIntegrationsDemo(HttpClient httpClient, IConsole console, AuthenticationHelper authHelper) : base(httpClient, console, authHelper)
        {
        }

        public override async Task ExecuteAsync()
        {
            var accessToken = await GetAccessTokenAsync();

            // get all of the accounts
            const string accountUrl = "https://api.aweber.com/1.0/accounts";
            var accounts = await GetCollectionAsync<Account>(accessToken, accountUrl);

            // get all sharing integration uri's for twitter and facebook
            // these are used to create a broadcast that will post to twitter or facebook
            // see broadcast example here: https://github.com/aweber/public-api-examples/tree/master/csharp/AWeber.Examples.CreateScheduleBroadcast
            var integrations = await GetCollectionAsync<Integration>(accessToken, accounts.First().IntegrationsCollectionLink);
            Console.WriteResponse(ConsoleColor.Green, "Integrations:");
            foreach (var integration in integrations)
            {
                if (TwitterAndFacebookIntegrations.Contains(integration.ServiceName.ToLower()))
                {
                    Console.WriteResponse(ConsoleColor.Green, string.Format("Service Name: {0}, Login: {1}, Self Link: {2}", integration.ServiceName, integration.Login, integration.SelfLink));
                }
            }
        }
    }
}
