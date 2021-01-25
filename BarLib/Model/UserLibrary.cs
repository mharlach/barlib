using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;

namespace BarLib
{
    public class UserLibrary : UserModelBase
    {
        [JsonProperty("drinks")]
        public List<ItemPair> Drinks { get; set; } = new List<ItemPair>();

        [JsonProperty("barId")]
        public string BarId { get; set; } = string.Empty;

        [JsonProperty("drinksVersion")]
        public string DrinksVersion { get; set; } = string.Empty;

        [JsonProperty("barHashCode")]
        public int BarHashCode {get;set;} 

        public override string ObjectType { get; set; } = "userLibrary";

        public override int GetHashCode()
        {
            HashCode = base.GetHashCode();
            foreach (var d in Drinks)
            {
                HashCode = HashCode ^ d.GetHashCode();
            }

            return HashCode;
        }
    }
}
