namespace BarLib
{
    public class DrinkStep
    {
        public DrinkStep() { }

        public DrinkStep(string ingredientId, float quantity, string units)
        {
            this.IngredientId = ingredientId;
            this.Quantity = quantity;
            this.Units = units;
        }

        public float Quantity { get; set; }

        public string Units { get; set; } = string.Empty;

        public string IngredientId { get; set; } = string.Empty;
    }
}
