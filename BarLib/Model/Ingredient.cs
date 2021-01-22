using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BarLib
{
    public enum IngredientType
    {
        Spirit,
        Adjunct,
        Garnish,
        Liqueur,
        Beer,
        Wine
    }

    public class Ingredient : ModelBase
    {
        public Ingredient() { }

        public Ingredient(string id, string name, IngredientType type)
        {
            this.Id = id;
            this.Name = name;
            this.IngredientType = type;
        }

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("ingredientType")]
        public IngredientType IngredientType { get; set; }

        public override string ObjectType { get; set; } = "ingredient";
    }

}
