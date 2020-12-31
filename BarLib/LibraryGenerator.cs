using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarLib
{
    public interface ILibraryGenerator
    {
        Task<List<Drink>> BuildAsync(UserBar bar);
    }

    public class BruteForceLibraryGenerator : ILibraryGenerator
    {
        private readonly IStorageContext<Drink> drinkContext;

        public BruteForceLibraryGenerator(IStorageContext<Drink> drinkContext)
        {
            this.drinkContext = drinkContext;
        }

        public async Task<List<Drink>> BuildAsync(UserBar bar)
        {
            var barContentsKeys = bar.AvailableIngredients.Select(x=>x.Id).ToList();
            var allDrinks = await drinkContext.GetAsync();
            var allowed = new List<Drink>();
            foreach (var d in allDrinks)
            {
                bool isAllowed = true;
                foreach (var s in d.Ingredients)
                {
                    if (barContentsKeys.Contains(s.IngredientId) == false)
                    {
                        isAllowed = false;
                        break;
                    }
                }

                if (isAllowed)
                {
                    allowed.Add(d);
                }
            }

            return allowed;
        }
    }

    // public class LibraryGenerator : ILibraryGenerator
    // {
    //     private readonly IStorageContext<Drink> drinkContext;
    //     private readonly IStorageContext<Ingredient> ingredientContext;

    //     public LibraryGenerator(
    //         IStorageContext<Drink> drinkContext,
    //         IStorageContext<Ingredient> ingredientContext)
    //     {
    //         this.drinkContext = drinkContext;
    //         this.ingredientContext = ingredientContext;
    //     }

    //     public async Task<List<Drink>> BuildAsync(UserBar bar)
    //     {
    //         var allowed = (await drinkContext.GetAsync())
    //             .Where(d=>d.GetIngredients().Except(bar.AvailableIngredients).Any());
            
    //         return allowed.ToList();
    //     }
    // }
}