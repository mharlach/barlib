// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Microsoft.Azure.Cosmos;
// using Microsoft.Extensions.Configuration;
// using System;
// using System.Linq;

// namespace BarLib.ServiceHost
// {
//     public class UserLibraryStorageContext : StorageContext, IStorageContext<UserLibrary>
//     {
//         private readonly Container container;

//         public UserLibraryStorageContext(IConfiguration config)
//         {
//             var connectionString = config.GetValue<string>("Cosmos:ConnectionString");

//             var client = new CosmosClient(connectionString);
//             container = client.GetContainer("barlib", "barlib");
//         }

//         public async Task DeleteAsync(string id) => await DeleteAsync<UserLibrary>(container, id);

//         public async Task<IList<UserLibrary>> GetAsync() => await GetAsync<UserLibrary>(container);

//         public async Task<UserLibrary> GetAsync(string id) => Get<UserLibrary>(container, z => z.UserId == id);

//         public async Task<UserLibrary> UpsertAsync(UserLibrary item) => await UpsertAsync<UserLibrary>(container, item.UserId.ToString(), item);
//     }



// }