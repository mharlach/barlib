using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BarLib
{

    public class UserBar : UserModelBase
    {
        [JsonProperty("availableIngredients")]
        public List<ItemPair> AvailableIngredients { get; set; } = new List<ItemPair>();

        public override string ObjectType { get; set; } = "userBar";

        public void AddIngredient(Ingredient i) => AvailableIngredients.Add(new ItemPair
        {
            Id = i.Id,
            Name = i.Name
        });

        public override int GetHashCode()
        {
            HashCode = base.GetHashCode();
            foreach (var i in AvailableIngredients)
            {
                HashCode = HashCode ^ i.Id.GetHashCode();
            }

            return HashCode;
        }

    }
}
