using System;
using System.Collections.Generic;
using System.Linq;

namespace BarLib
{

    public class UserBar : ModelBase
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> AvailableIngredients { get; set; } = new List<string>();

        public int HashCode { get; set; }

        public override int GetHashCode()
        {
            int hashCode = 0;
            AvailableIngredients.ForEach(i => hashCode ^= i.GetHashCode());

            return hashCode;

        }
    }
}
