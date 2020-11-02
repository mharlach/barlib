using System;
using System.Collections.Generic;

namespace BarLib
{
    public class Ingredient
    {
        public Guid Id {get;set;}

        public string Name {get;set;}

        public string Description {get;set;}

        public IngredientType IngredientType {get;set;}

    }

    public enum IngredientType
    {
        Spirit,
        Adjunct,
        Garnish
    }

    public class DrinkStep
    {
        public float Quantity {get;set;}
        
        public string Units {get;set;}

        public Guid IngredientId {get;set;}
    }

    public class Drink
    {
        public Guid Id {get;set;}
        public string Name {get;set;}
        public string Description {get;set;}
        public List<DrinkStep> Steps {get;set;} = new List<DrinkStep>();
    }

    public class UserBar
    {
        public Guid UserId {get;set;}
        public List<Guid> AvailableIngredients {get;set;} = new List<Guid>();
    }
}
