using System.Collections.Generic;
using System.Linq;

namespace BarLib
{
    public class Drink
    {
        public Drink() { }

        public Drink(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<DrinkStep> Steps { get; set; } = new List<DrinkStep>();

        public void AddStep(string IngredientId, float quantity, string units) => Steps.Add(new DrinkStep(IngredientId, quantity, units));

        public IEnumerable<string> GetIngredients() => Steps.Select(x => x.IngredientId);
    }
}
