using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BarLib
{
    public class Drink : ModelBase
    {
        public Drink() { }

        public Drink(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("ingredients")]
        public List<DrinkIngredient> Ingredients { get; set; } = new List<DrinkIngredient>();

        [JsonProperty("steps")]
        public List<string> Steps { get; set; } = new List<string>();

        public override string ObjectType { get; set; } = "drink";


        public void AddStep(string IngredientId, float quantity, string units) => Ingredients.Add(new DrinkIngredient(IngredientId, quantity, units));

        public IEnumerable<string> GetIngredients() => Ingredients.Select(x => x.IngredientId);
    }

    public class ItemPair
    {

        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }
}
