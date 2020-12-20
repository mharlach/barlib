using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BarLib
{
    public enum IngredientType
    {
        Spirit,
        Adjunct,
        Garnish
    }

    public class Ingredient
    {
        public Ingredient() { }

        public Ingredient(string id, string name, IngredientType type)
        {
            this.Id = id;
            this.Name = name;
            this.IngredientType = type;
        }

        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [JsonConverter(typeof(StringEnumConverter))]
        public IngredientType IngredientType { get; set; }

        public int PartitionKey() => System.Math.Abs(Id.GetHashCode() % 1000);

    }

}
