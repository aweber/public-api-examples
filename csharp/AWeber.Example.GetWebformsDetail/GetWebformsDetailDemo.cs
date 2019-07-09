using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AWeber.Examples;
using AWeber.Examples.Models;
using McMaster.Extensions.CommandLineUtils;

namespace AWeber.Example.GetWebformsDetail
{
    public class GetWebformsDetailDemo : BaseDemo
    {
        public GetWebformsDetailDemo(HttpClient httpClient, IConsole console, AuthenticationHelper authHelper) : base(httpClient, console, authHelper)
        {
        }

        public override async Task ExecuteAsync()
        {
            var accessToken = await GetAccessTokenAsync();

            // Get an account to get webforms for
            const string accountsUrl = "https://api.aweber.com/1.0/accounts";
            var accounts = await GetCollectionAsync<Account>(accessToken, accountsUrl);
            var account = accounts.First(); // choose the first account

            // Get a list to get webforms for
            var lists = await GetCollectionAsync<List>(accessToken, account.ListsCollectionLink);
            var list = lists.First(); // choose the first list

            // Get all the webforms for a list
            var webforms = await GetCollectionAsync<Webform>(accessToken, list.WebFormsCollectionLink);
            if (!webforms.Any())
            {
                Console.WriteResponse(ConsoleColor.Green, "No webforms for {0}:", list.Name);
                return;
            }

            Console.WriteResponse(ConsoleColor.Green, "Webforms for {0}", list.Name);
            foreach (var webform in webforms)
            {
                Console.WriteResponse(ConsoleColor.Green, "\t{0}", webform.Name);
                PrintWebformInfo(webform);
            }
            Console.WriteResponse(ConsoleColor.Green, string.Empty);

            // Get all the webform split tests for a list
            var splitTests = await GetCollectionAsync<SplitTest>(accessToken, list.WebFormSplitTestsCollectionLink);
            if (!splitTests.Any())
            {
                Console.WriteResponse(ConsoleColor.Green, "No webform split tests for {0}", list.Name);
                return;
            }

            Console.WriteResponse(ConsoleColor.Green, "Webform split tests for {0}:", list.Name);
            foreach (var splitTest in splitTests)
            {
                Console.WriteResponse(ConsoleColor.Green, "\t{0}: {1}", splitTest.Name, splitTest.JavascriptSourceLink);
                var components = await GetCollectionAsync<SplitTestComponent>(accessToken, splitTest.ComponentsCollectionLink);
                foreach (var component in components)
                {
                    Console.WriteResponse(ConsoleColor.Green, "\t\t{0} {1}%", component.Name, component.Weight);
                    PrintWebformInfo(component, 12);
                }
            }
        }

        private void PrintWebformInfo(Webform webform, int indent = 8)
        {
            var prefix = string.Empty.PadLeft(indent, ' ');
            Console.WriteResponse(ConsoleColor.Green, "{0}Type: {1}", prefix, webform.Type);
            Console.WriteResponse(ConsoleColor.Green, "{0}HTML source: {1}", prefix, webform.HtmlSourceLink);
            Console.WriteResponse(ConsoleColor.Green, "{0}JS source: {1}", prefix, webform.JavascriptSourceLink);
            Console.WriteResponse(ConsoleColor.Green, "{0}Displays: {1} ({2} unique)", prefix, webform.TotalDisplays, webform.TotalUniqueDisplays);
            Console.WriteResponse(ConsoleColor.Green, "{0}Submissions: {1}", prefix, webform.TotalSubmissions);
            Console.WriteResponse(ConsoleColor.Green, "{0}Conversion: {1}%", prefix, webform.ConversionPercentage);
            Console.WriteResponse(ConsoleColor.Green, "({0}% unique)", webform.UniqueConversionPercentage);
        }
    }
}