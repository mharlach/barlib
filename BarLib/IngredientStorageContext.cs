using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarLib
{
    public class JsonIngredientStorageContext : IStorageContext<Ingredient>
    {
        public Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Ingredient>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Ingredient?> GetAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Ingredient> UpsertAsync(Ingredient ingredient)
        {
            throw new NotImplementedException();
        }
    }
}