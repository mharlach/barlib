using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarLib
{
    public class JsonDrinkStorageContext : IStorageContext<Drink>
    {
        public Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Drink>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Drink?> GetAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Drink> UpsertAsync(Drink ingredient)
        {
            throw new NotImplementedException();
        }
    }

    
}