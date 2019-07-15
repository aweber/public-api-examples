using System;
using System.Net.Http;
using McMaster.Extensions.CommandLineUtils;

namespace AWeber.Examples.MoveSubscriber
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
            var authenticationHelper = new AuthenticationHelper(httpClient, OAuthUri, RedirectUri);
            var demo = new MoveSubscriberDemo(httpClient, console, authenticationHelper);

            try
            {
                demo.ExecuteAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                console.WriteError("Error during move subscriber example. {0}\n{1}", e.Message, e.StackTrace);
            }
            finally
            {
                httpClient.Dispose();
                Prompt.GetString("Press any key to exit:", promptColor: ConsoleColor.White,
                    promptBgColor: ConsoleColor.Black);
            }
        }
    }
}
