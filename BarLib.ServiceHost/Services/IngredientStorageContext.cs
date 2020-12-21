using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace BarLib.ServiceHost
{
    public class IngredientStorageContext : StorageContextBase, IStorageContext<Ingredient>
    {
        private readonly Container container;

        public IngredientStorageContext(IConfiguration config)
        {
            var connectionString = config.GetValue<string>("Cosmos:ConnectionString");

            var client = new CosmosClient(connectionString);
            container = client.GetContainer("barlib", "ingredients");
        }

        public async Task DeleteAsync(string id) => await DeleteAsync<Ingredient>(container, id);

        public async Task<IList<Ingredient>> GetAsync() => await GetAsync<Ingredient>(container);

        public async Task<Ingredient> GetAsync(string id) => Get<Ingredient>(container, z => z.Id == id.ToLower());

        public async Task<Ingredient> UpsertAsync(Ingredient item) => await UpsertAsync<Ingredient>(container, item.Id, item);
    }


}