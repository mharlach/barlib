using System;
using System.Collections.Generic;
using System.Linq;

namespace BarLib
{

    public class UserBar
    {
        public Guid UserId { get; set; }
        public List<string> AvailableIngredients { get; set; } = new List<string>();
    }
}
