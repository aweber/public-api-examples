using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AWeber.Examples.Models;
using McMaster.Extensions.CommandLineUtils;

namespace AWeber.Examples.ManageCustomFields
{
    public class ManageCustomFieldsDemo : BaseDemo
    {
        public ManageCustomFieldsDemo(HttpClient httpClient, IConsole console, AuthenticationHelper authHelper) : base(httpClient, console, authHelper)
        {
        }

        public override async Task ExecuteAsync()
        {
            var accessToken = await GetAccessTokenAsync();

            // Get an account to manage custom fields on
            const string accountsUrl = "https://api.aweber.com/1.0/accounts";
            var accounts = await GetCollectionAsync<Account>(accessToken, accountsUrl);
            var accountUrl = accounts.First().SelfLink; // choose the first account

            // Get a list to manage custom fields on
            var listsUrl = accounts.First().ListsCollectionLink;
            var lists = await GetCollectionAsync<List>(accessToken, listsUrl);
            var list = lists.First(); // choose the first list

            var customFieldsUrl = list.CustomFieldsCollectionLink;
            var customFields = await GetCollectionAsync<CustomField>(accessToken, customFieldsUrl);
            foreach (var entry in customFields)
            {
                if (new List<string>{"Test", "Renamed"}.Contains(entry.Name))
                {
                    Console.WriteResponse(ConsoleColor.Green, "A custom field called {0} already exists on {1}", entry.Name, list.Name);
                    return;
                }
            }

            // Create a custom field called Test
            var createFormValues = new Dictionary<string, string>
            {
                {"ws.op", "create"},
                {"name", "Test"}
            };
            var fieldUrl = (await CreateFormAsync(createFormValues, accessToken, customFieldsUrl)).AbsoluteUri;
            Console.WriteResponse(ConsoleColor.Green, "Create new custom field at {0}", fieldUrl);

            // Update the custom field
            var updateCustomField = new UpdateCustomField{Name = "Renamed", IsSubscriberUpdateable = true};
            var updatedCustomField = await UpdateAsync<UpdateCustomField, CustomField>(updateCustomField, accessToken, fieldUrl);
            Console.WriteJson("Updated the custom field", updatedCustomField);

            // Delete the custom field
            await DeleteAsync(accessToken, fieldUrl);
            Console.WriteResponse(ConsoleColor.Green, "Deleted the custom field");
        }
    }
}