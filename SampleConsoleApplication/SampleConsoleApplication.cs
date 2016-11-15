using ConstructorIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleConsoleApplication
{
    class SampleConsoleApplication
    {
        static string API_TOKEN = "XXXXXXXXXXXXXXXXXXXX";
        static string KEY       = "XXXXXXXXXXXXXXXXXXXX";

        static void Main(string[] args)
        {
            
            try
            {
                // We'll need an API TOKEN and KEY. You can get these from your dashboard on Custructor.io
                // Store them as an Environment Variable or hardcoded here.
                API_TOKEN = Environment.GetEnvironmentVariable("CONSTRUCTORIO_API_TOKEN", EnvironmentVariableTarget.User);
                KEY = Environment.GetEnvironmentVariable("CONSTRUCTORIO_KEY", EnvironmentVariableTarget.User);

                // Create a Constructor.IO client
                ConstructorIOAPI constructorClient = new ConstructorIOAPI(API_TOKEN, KEY);

                // Verify the API and KEY. While this is not strictly nesseary, it is nice to be methodical.
                if (!constructorClient.Verify())
                    throw new Exception("Authentication failed. Please check that you have the correct API token and KEY");

                Console.WriteLine("Authentication veridied");

                // Lets load a CSV file
                IEnumerable<ListItem> listItems = Util.LoadCSV("sample.csv");

                Console.WriteLine("CSV file loaded and has "+listItems.Count() +" entries");

                // And upload it to Construtor.IO, under the section "Products"
                // We will use AddOrUpdate function, which will add entries that don't exist, and overwrite entries that do.
                if (!constructorClient.AddOrUpdateBatch(listItems, ListItemAutocompleteType.Products))
                    throw new Exception("AddOrUpdateBatch failed.");

                Console.WriteLine("Entries sent to Constructor.IO successfully");

                // We can also create a ListItem manualy, this time under "Search Suggestions" section
                ListItem newListItem = new ListItem("Testing 123",ListItemAutocompleteType.SearchSuggestions);
                newListItem.SuggestedScore = 10;

                // Lets upload it to the server
                if (!constructorClient.Add(newListItem)) throw new Exception("Adding new ListItem failed.");

                // We can modify it and update 
                newListItem.SuggestedScore = 42;
                if (!constructorClient.AddOrUpdate(newListItem)) throw new Exception("Updaing ListItem failed.");

                // And finally delete it
                if (!constructorClient.Remove(newListItem)) throw new Exception("Removing new ListItem failed.");

                Console.WriteLine("ListItem created, updated and delete successfully. All done!");

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }




        }
    }
}
