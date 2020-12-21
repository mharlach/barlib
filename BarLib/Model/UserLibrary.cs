using System;
using System.Collections.Generic;
using System.Linq;

namespace BarLib
{
    public class UserLibrary : ModelBase
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> Drinks { get; set; } = new List<string>();

        public int HashCode { get; set; }

        public override int GetHashCode()
        {
            int hashCode = 0;
            Drinks.ForEach(i => hashCode ^= i.GetHashCode());

            return hashCode;

        }
    }
}
