using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PeopleOData
{
    partial class Program
    {
        private static readonly HttpClient client = new();
        static readonly List<PeopleQueryProperty> possibleFilterOptions = new()
        {
            new PeopleQueryProperty{ Name="FirstName", Id="1", DisplayName="First Name"},
            new PeopleQueryProperty{ Name="LastName", Id="2", DisplayName="Last Name"},
            new PeopleQueryProperty{ Name="MiddleName", Id="3", DisplayName="Middle Name"},
            new PeopleQueryProperty{ Name="Gender", Id="4", DisplayName="Gender"},
            new PeopleQueryProperty{ Name="Age", Id="5", DisplayName="Age"},
            new PeopleQueryProperty{ Name="FavoriteFeature", Id="6", DisplayName="Favorite Feature"}
        };
        public class PeopleQueryProperty
        {
            public string Name { get; set; }
            public string Id { get; set; }
            public string DisplayName { get; set; }
        }
        static async Task Main(string[] args)
        {
            var spinner = new ConsoleSpiner(0, 0); 
            spinner.Start();
            string requestKey = await GetRequestKey();
            spinner.Stop();

            await HandleOperation(requestKey);
        }
        public static string Indent(int count)
        {
            return "".PadLeft(count);
        }
        static void DisplayOperations()
        {
            Console.WriteLine("What do you want to do:");

            Console.WriteLine($"{Indent(3)}1 to list people.");
            Console.WriteLine($"{Indent(3)}2 to Search/Filter people.");
            Console.WriteLine($"{Indent(3)}3 to select/view specific person's details");
            Console.WriteLine($"{Indent(3)}4 to delete a person");
            Console.WriteLine($"{Indent(3)}5 to update a person's detail");
            Console.WriteLine($"{Indent(3)}6 to quit the application.");
        }

        static async Task HandleOperation(string requestKey)
        {
            DisplayOperations();

            string operation = Console.ReadLine().ToLower();
            while (string.IsNullOrWhiteSpace(operation) || (operation != "1" && operation != "2" && operation != "3" && operation != "4" && operation != "5" && operation != "6"))
            {
                Console.WriteLine("invalid command.\n\n");
                DisplayOperations();
                operation = Console.ReadLine().ToLower();
            }

            if (operation == "6")
            {
                Console.WriteLine("shutting down...");
                Thread.Sleep(3000);
                Environment.Exit(0);
            }
            else if (operation == "1") await HandlePeopleList(requestKey);
            else if (operation == "2") await HandleGetPersonDetail(requestKey);
            else if (operation == "3") await HandleDataSearching(requestKey);
            else if (operation == "4") await HandleDeletePersonAsync(requestKey);
            Thread.Sleep(2000);
            await HandleOperation(requestKey);
        }

        static void DispalyData(object jsonData)
        {
            Console.WriteLine(JsonSerializer.Serialize(jsonData, new JsonSerializerOptions() { WriteIndented = true }));
        }

        static async Task HandlePeopleList(string requestKey)
        {
            using var httpResponse = await client.GetAsync($"https://services.odata.org/TripPinRESTierService/{requestKey}/People");
            var content = httpResponse.Content;
            var resultString = await content.ReadAsStringAsync();
            PeopleData peopleData = null;
            if (httpResponse.IsSuccessStatusCode)
            {
                peopleData = JsonSerializer.Deserialize<PeopleData>(resultString);
                if (peopleData!=null && peopleData.Peoples.Count > 0)
                {
                    foreach (var people in peopleData.Peoples)
                    {
                        DispalyData(people);
                    }
                    Console.WriteLine("\n--------------------END OF LIST--------------------\n");
                }
            }
        }

        static async Task HandleGetPersonDetail(string requestKey)
        {
            string username = null;

            while (string.IsNullOrWhiteSpace(username))
            {
                Console.Write("\nWhat is the person's ID?: ");
                username = Console.ReadLine();
            }

            var person = await GetPersonDetail(username, requestKey);
            if (person != null)
                DispalyData(person);
            else Console.WriteLine($"Person with ID = {username} not found\n\n");
        }

        static async Task<People> GetPersonDetail(string username, string requestKey)
        {
            using var httpResponse = await client.GetAsync($"https://services.odata.org/TripPinRESTierService/{requestKey}/People('{username}')");
            var content = httpResponse.Content;
            var resultString = await content.ReadAsStringAsync();
            People personData = null;
            if (httpResponse.IsSuccessStatusCode)
            {
                personData = JsonSerializer.Deserialize<People>(resultString);
            }
            return personData;
        }

        static async Task<string> GetRequestKey()
        {
            var streamTask = await client.GetStreamAsync("https://services.odata.org/TripPinRESTierService");
            var baseData = await JsonSerializer.DeserializeAsync<BaseData>(streamTask);
            return baseData.OdataContext.Split('/')[4];
        }

        static async Task HandleDataSearching(string requestKey)
        {
            DisplaySearchableProperties();
            var ans = Console.ReadLine();
            if (ans == "8")
            {
                Console.WriteLine("\ngoodbye...");
                Thread.Sleep(1000);
                Environment.Exit(0);
            }
            else if(ans!="7")
            {
                var selectedResponse = possibleFilterOptions.FirstOrDefault(d => d.Id == ans);
                while (selectedResponse == null)
                {
                    DisplaySearchableProperties();
                    ans = Console.ReadLine();
                }
                Console.WriteLine("Enter your search text: ");

                var searchText = Console.ReadLine();
                var result = await GetFilteredData(requestKey, selectedResponse.Name, searchText);
                if (result != null && result.Peoples.Count>0)
                {
                    Console.WriteLine($"\n\nFind your search result below... ");
                    foreach (var people in result.Peoples)
                    {
                        DispalyData(people);
                    }
                }
                else
                {
                    Console.WriteLine("no result found.");
                }
            }
        }
        static async Task<PeopleData> GetFilteredData(string requestKey, string searchProp, string searchVal)
        {
            using var httpResponse = await client.GetAsync($"https://services.odata.org/TripPinRESTierService/{requestKey}/People?$filter=contains({searchProp}, '{searchVal}')");
            var content = httpResponse.Content;
            var resultString = await content.ReadAsStringAsync();
            PeopleData peopleData = null;
            if (httpResponse.IsSuccessStatusCode)
            {
                peopleData = JsonSerializer.Deserialize<PeopleData>(resultString);
                return peopleData;
            }
            return null;
        }
        static void DisplaySearchableProperties()
        {
            Console.WriteLine(Indent(0) + "\n\nWhat do you want to filter on:");
            foreach(var option in possibleFilterOptions) Console.WriteLine(Indent(3) + $"{option.Id} for {option.DisplayName}");
            Console.WriteLine(Indent(3) + "7 to go back to the main menu");
            Console.WriteLine(Indent(3) + "8 to quit the application\n");
        }
        static async Task HandleDeletePersonAsync(string requestKey)
        {
            string username = null;

            while (string.IsNullOrWhiteSpace(username))
            {
                Console.Write("\nWhat is the person's ID?: ");
                username = Console.ReadLine();
            }

            if (await DeletePerson(requestKey, username))
            {
                Console.WriteLine("Person deleted successfully");
            }
            else Console.WriteLine("ID not valid");
        }
        static async Task<bool> DeletePerson(string requestKey, string username)
        {
            using var httpResponse = await client.DeleteAsync($"https://services.odata.org/TripPinRESTierService/{requestKey}/People('{username}')");
            var content = httpResponse.Content;
            var resultString = await content.ReadAsStringAsync();
            bool isSuccess = false;
            if (httpResponse.IsSuccessStatusCode)
            {
                isSuccess = true;
            }
            return isSuccess;
        }
    }
}