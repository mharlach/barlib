using System;
using System.Collections.Generic;

public class SystemStats
{
    public string DrinksVersion {get;set;} = string.Empty;

    // public List<string> DrinkIds {get;set;} = new List<string>();

    public DateTime Updated {get;set;} = DateTime.UtcNow;

    // public int CalculateDrinkHashCode()
    // {
    //     DrinksHashCode = 0;
    //     foreach(var id in DrinkIds){
    //         DrinksHashCode = DrinksHashCode ^ id.GetHashCode();
    //     }

    //     return DrinksHashCode;
    // }


}