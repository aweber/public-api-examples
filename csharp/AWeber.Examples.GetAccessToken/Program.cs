using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;

namespace AWeber.Examples.GetAccessToken
{
    internal class Program
    {
        private const string OAuthUri = "https://auth.aweber.com/oauth2";
        // Redirect Uri must be https and must be one of the redirects specified
        // for your integration
        private const string RedirectUri = "https://localhost";

        private static void Main(string[] args)
        {
            var httpClient = new HttpClient();
            var console = PhysicalConsole.Singleton;
            var configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            try
            {
                ExecuteAsync(httpClient, console, configurationRoot).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                console.WriteError("Error during retrieval of access token. {0}\n{1}", e.Message, e.StackTrace);
            }
            finally
            {
                httpClient.Dispose();
                Prompt.GetString("Press any key to exit:", promptColor: ConsoleColor.White,
                    promptBgColor: ConsoleColor.Black);
            }
        }

        private static async Task ExecuteAsync(HttpClient httpClient, IConsole console, IConfiguration config)
        {
            var authenticationHelper = new AuthenticationHelper(httpClient, OAuthUri, RedirectUri);
            var clientId = config["AWeberClientId"];
            if (string.IsNullOrWhiteSpace(clientId))
            {
                clientId = Prompt.GetString("Enter your client id:", promptColor: ConsoleColor.Yellow,
                    promptBgColor: ConsoleColor.Black);
            }
            else
            {
                console.WriteResponse(ConsoleColor.Green, "Client id retrieved from config.");
            }

            var clientSecret = config["AWeberClientSecret"];
            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                clientSecret = Prompt.GetPassword("Enter your client secret:", promptColor: ConsoleColor.Yellow,
                    promptBgColor: ConsoleColor.Black);
            }
            else
            {
                console.WriteResponse(ConsoleColor.Green, "Client secret retrieved from config.");
            }

            // Use your client id and secret to get an access token.
            //
            // See the Getting Started guide to redirect to a callback URL:
            // https://api.aweber.com/#tag/Getting-Started

            var scopes = new List<string>
            {
                Scopes.AccountRead,
                Scopes.ListRead,
                Scopes.ListWrite,
                Scopes.SubscriberRead,
                Scopes.SubscriberWrite,
                Scopes.EmailRead,
                Scopes.EmailWrite,
                Scopes.SubscriberReadExtended
            };

            // Log in to receive a redirect url
            var authorizationUrl = authenticationHelper.CreateAuthorizationUrl(scopes, clientId);
            console.WriteResponse(ConsoleColor.White, "Go to this URL:");
            console.WriteResponse(ConsoleColor.Blue, authorizationUrl.Url);
            var authorizationResponse = Prompt.GetString("Log in and paste the returned URL here: ",
                promptColor: ConsoleColor.Yellow,
                promptBgColor: ConsoleColor.Black);

            // Use authorization response to get a token
            var authorizationCode = authenticationHelper.ParseAuthorizationCode(authorizationResponse, authorizationUrl.State);
            var accessTokenResponse = await authenticationHelper.MakeAccessTokenRequestAsync(authorizationCode, clientId, clientSecret);

            // The path to store the credentials
            var credentialsFilePath = Path.Combine(Path.GetTempPath(), "AWeber", "credentials.json");
            authenticationHelper.SaveAccessToken(new Credentials(accessTokenResponse, clientId, clientSecret), credentialsFilePath);
            console.WriteResponse(ConsoleColor.Green, "Updated credentials.json with your new credentials");
        }
    }
}
