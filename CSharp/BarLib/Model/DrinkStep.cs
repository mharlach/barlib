using Newtonsoft.Json;

namespace BarLib
{
    public class DrinkIngredient
    {
        public DrinkIngredient() { }

        public DrinkIngredient(string ingredientId, float quantity, string units)
        {
            this.IngredientId = ingredientId;
            this.Quantity = quantity;
            this.Units = units;
        }

        [JsonProperty("quantity")]
        public float Quantity { get; set; }

        [JsonProperty("units")]
        public string Units { get; set; } = string.Empty;

        [JsonProperty("ingredientId")]
        public string IngredientId { get; set; } = string.Empty;

        [JsonProperty("ingredientName")]
        public string IngredientName { get; set; } = string.Empty;

    }
}
