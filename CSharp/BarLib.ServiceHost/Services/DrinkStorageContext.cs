// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Microsoft.Azure.Cosmos;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;
// using System.Linq;
// using System;

// namespace BarLib.ServiceHost
// {
//     public class DrinkStorageContext : StorageContext, IStorageContext<Drink>
//     {
//         private readonly Container container;

//         public DrinkStorageContext(IConfiguration config)
//         {
//             var connectionString = config.GetValue<string>("Cosmos:ConnectionString");

//             var client = new CosmosClient(connectionString);
//             container = client.GetContainer("barlib", "barlib");
//         }

//         public async Task DeleteAsync(string id) => await DeleteAsync<Drink>(container, id);

//         public async Task<IList<Drink>> GetAsync() => await GetAsync<Drink>(container);

//         public async Task<Drink> GetAsync(string id) => Get<Drink>(container,z=>z.Id==id.ToLower());

//         public async Task<Drink> UpsertAsync(Drink item) => await UpsertAsync<Drink>(container, item.Id, item);
//     }

    

// }