using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace BarLib.ServiceHost
{
    public class UserBarStorageContext : StorageContextBase, IStorageContext<UserBar>
    {
        private readonly Container container;

        public UserBarStorageContext(IConfiguration config)
        {
            var connectionString = config.GetValue<string>("Cosmos:ConnectionString");

            var client = new CosmosClient(connectionString);
            container = client.GetContainer("barlib", "users");
        }

        public async Task DeleteAsync(string id) => await DeleteAsync<UserBar>(container, id);

        public async Task<IList<UserBar>> GetAsync() => await GetAsync<UserBar>(container);

        public async Task<UserBar> GetAsync(string id) => Get<UserBar>(container,z=>z.UserId== new System.Guid(id));

        public async Task<UserBar> UpsertAsync(UserBar item) => await UpsertAsync<UserBar>(container, item.UserId.ToString(), item);
    }

    

}