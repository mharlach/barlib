using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarLib
{
    public interface ILibraryGenerator
    {
        Task<UserLibrary> BuildLibraryAsync(UserBar bar, UserLibrary? library);
        // Task<List<Drink>> BuildAsync(UserBar bar);
    }

    public class BruteForceLibraryGenerator : ILibraryGenerator
    {
        private readonly IStorageContext<Drink> drinkContext;
        private readonly IStorageContext<Ingredient> ingredientContext;
        private readonly ISystemStatsStorageContext statsStorageContext;

        public BruteForceLibraryGenerator(
            IStorageContext<Drink> drinkContext,
            IStorageContext<Ingredient> ingredientContext,
            ISystemStatsStorageContext statsStorageContext)
        {
            this.drinkContext = drinkContext;
            this.ingredientContext = ingredientContext;
            this.statsStorageContext = statsStorageContext;
        }

        public async Task<List<Drink>> BuildAsync(UserBar bar)
        {
            var barContentsKeys = bar.AvailableIngredients.Select(x => x.Id).ToList();
            var ingredientsById = (await ingredientContext.GetAsync()).ToDictionary(x => x.Id);

            // select * from d where d.ingredients.id
            var allDrinks = await drinkContext.GetAsync();
            var allowed = new List<Drink>();
            foreach (var d in allDrinks)
            {
                var missing = false;
                foreach (var i in d.Ingredients)
                {
                    if (ingredientsById.TryGetValue(i.IngredientId, out var ingredient) &&
                    ingredient.IngredientType != IngredientType.Garnish &&
                    barContentsKeys.Contains(i.IngredientId) == false)
                    {
                        missing = true;
                        break;
                    }
                    // if (barContentsKeys.Contains(i.IngredientId) == false)
                    // {
                    //     missing = true;
                    //     break;
                    // }
                }

                if (missing == false)
                {
                    allowed.Add(d);
                }
            }

            return allowed;
        }

        public async Task<UserLibrary> BuildLibraryAsync(UserBar bar, UserLibrary? library)
        {
            library = library ?? new UserLibrary
            {
                Id = Guid.NewGuid().ToString(),
                BarId = bar.Id,
                UserId = bar.UserId,
            };

            var stats = await statsStorageContext.GetAsync();
            if(library.DrinksVersion == stats.DrinksVersion && library.BarHashCode == bar.HashCode)
            {
                return library;
            }

            var drinks = await BuildAsync(bar);

            library.Drinks = drinks.Select(x => new ItemPair { Id = x.Id, Name = x.Name }).ToList();
            library.Updated = DateTime.UtcNow;
            library.HashCode = library.GetHashCode();
            library.BarHashCode = bar.HashCode;

            return library;
        }
    }
}