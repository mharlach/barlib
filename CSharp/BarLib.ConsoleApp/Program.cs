using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
// using System.Data

namespace BarLib.ConsoleApp
{
    class Program
    {
        private static readonly string ingredientInput = "ingredients_input.txt";
        private static readonly string drinksInput = "drinks_input.json";
        private static readonly string ingredientsData = "ingredients.json";
        private static readonly string drinksData = "drinks.json";

        static async Task Main(string[] args)
        {
            var userBar = LoadUserBar("mybar.txt");
            userBar.Id = Guid.NewGuid().ToString();
            userBar.UserId = Guid.NewGuid().ToString();

            var uploader = new Uploader("http://localhost:7071/api/");
            uploader.Put($"users/{userBar.UserId}/bar", userBar);
        }

        public static void Generate()
        {
            var generator = new BruteForceLibraryGenerator(new DrinkStorageContent(), new IngredientStorageContext());
            var drinksListTask = generator.BuildAsync(LoadUserBar("mybar.txt"));

            drinksListTask.Wait();

            var drinks = drinksListTask.Result;

            foreach (var d in drinks)
            {
                Console.WriteLine($"{d.Name}");
            }
        }

        private static void ImportIngredientList()
        {
            var existingContent = JsonConvert.DeserializeObject<Ingredient[]>(File.ReadAllText(ingredientsData));
            var ingredientsByName = existingContent.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

            var inputLines = File.ReadAllLines(ingredientInput);
            // var ingredients = new List<Ingredient>();
            foreach (var line in inputLines)
            {
                var split = line.Split(';');
                var name = split[0].Trim();
                var ingredientType = Enum.Parse<IngredientType>(split[1].Trim(), true);
                var i = new Ingredient
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = name,
                    IngredientType = ingredientType,
                };
                i.PartitionKey = Math.Abs(i.Id.GetHashCode() % 1000).ToString();
                if (ingredientsByName.ContainsKey(i.Name))
                {
                    ingredientsByName[i.Name] = i;
                }
                else
                {
                    ingredientsByName.Add(i.Name, i);
                }
            }

            var jsonString = JsonConvert.SerializeObject(ingredientsByName.Values);
            File.WriteAllText(ingredientsData, jsonString);

        }
        private static void ImportDrinksList()
        {
            var ingredients = JsonConvert.DeserializeObject<Ingredient[]>(File.ReadAllText(ingredientsData))
                .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

            // var allDrinks = JsonConvert.DeserializeObject<Drink[]>(File.ReadAllText(drinksData))
            //     .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

            var allDrinks = new Dictionary<string, Drink>(StringComparer.OrdinalIgnoreCase);

            var jInput = JArray.Parse(File.ReadAllText(drinksInput));
            foreach (JObject item in jInput)
            {
                var d = new Drink();
                d.Id = Guid.NewGuid().ToString();
                d.Name = item.Value<string>("Name");
                var drinkSteps = (JArray)item["Steps"];
                d.Steps = drinkSteps.ToObject<List<string>>();
                // {
                //     Id = Guid.NewGuid().ToString(),
                //     Name = item.Value<string>("Name"),
                //     Steps = item.Value<string[]>("Steps").ToList(),
                // };
                d.PartitionKey = Math.Abs(d.Id.GetHashCode() % 1000).ToString();
                foreach (var i in (JArray)item["Ingredients"])
                {
                    var ingredient = new DrinkIngredient
                    {
                        IngredientId = ingredients[i.Value<string>("Name")].Id,
                        IngredientName = i.Value<string>("Name"),
                        Quantity = i.Value<float>("Quantity"),
                        Units = i.Value<string>("Units"),
                    };
                    d.Ingredients.Add(ingredient);
                }

                if (allDrinks.ContainsKey(d.Name) == false)
                {
                    allDrinks.Add(d.Name, d);
                }
            }

            File.WriteAllText(drinksData, JsonConvert.SerializeObject(allDrinks.Values));
        }

        private static UserBar LoadUserBar(string barFile)
        {
            var inputs = File.ReadAllLines(barFile);
            var ingredients = JsonConvert.DeserializeObject<Ingredient[]>(File.ReadAllText(ingredientsData))
                .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

            var myBar = new UserBar
            {
                Id = Guid.NewGuid().ToString(),
            };

            foreach (var i in inputs)
            {
                if (ingredients.TryGetValue(i, out var ingredient))
                {
                    myBar.AddIngredient(ingredient);
                }
            }

            return myBar;
        }
    }

    public class DrinkStorageContent : IStorageContext<Drink>
    {
        private static string dataFile = "drinks.json";

        public Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Drink>> GetAsync()
        {
            var data = File.ReadAllText(dataFile);
            var drinks = JsonConvert.DeserializeObject<List<Drink>>(data);
            return drinks;
        }

        public Task<Drink> GetAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Drink>> GetAsync(QueryDefinition queryDef)
        {
            throw new NotImplementedException();
        }

        public Task<Drink> UpsertAsync(Drink item)
        {
            throw new NotImplementedException();
        }
    }

    public class IngredientStorageContext : IStorageContext<Ingredient>
    {

        public Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Ingredient>> GetAsync()
        {
            var data = File.ReadAllText("ingredients.json");
            var ingredients = JsonConvert.DeserializeObject<List<Ingredient>>(data);
            return ingredients;
        }

        public Task<Ingredient> GetAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Ingredient>> GetAsync(QueryDefinition queryDef)
        {
            throw new NotImplementedException();
        }

        public Task<Ingredient> UpsertAsync(Ingredient item)
        {
            throw new NotImplementedException();
        }
    }

}
