using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Collections.Generic;
using System;

namespace BarLib.ServiceHost
{

    public class UserModelBaseStorageContext<T> : IUserStorageContext<T> where T : UserModelBase
    {
        private readonly Container container;
        private ILogger log;

        public UserModelBaseStorageContext(ILoggerFactory factory, IConfiguration configuration)
        {
            this.log = factory.CreateLogger<UserModelBaseStorageContext<T>>();
            var connectionString = configuration.GetValue<string>("Cosmos:ConnectionString");

            var client = new CosmosClient(connectionString);
            this.container = client.GetContainer("barlib", "models");
        }

        public async Task<IList<T>> GetAsync(string userId)
        {
            var def = new QueryDefinition("SELECT * FROM c WHERE c.userId=@UserId AND c.type=@Type")
                .WithParameter("@UserId", userId)
                .WithParameter("@Type", Activator.CreateInstance<T>().ObjectType);

            var query = container.GetItemQueryIterator<T>(def);
            var items = new List<T>();

            while (query.HasMoreResults)
            {
                var sub = await query.ReadNextAsync();
                items.AddRange(sub.Resource);
            }

            return items;
        }

        public async Task<T?> GetAsync(string userId, string id)
        {
            var def = new QueryDefinition("SELECT * FROM c WHERE c.userId=@UserId AND c.id=@Id AND c.type=@Type")
                .WithParameter("@UserId", userId)
                .WithParameter("@Id", id)
                .WithParameter("@Type", Activator.CreateInstance<T>().ObjectType);

            var query = container.GetItemQueryIterator<T>(def);
            var items = new List<T>();

            while (query.HasMoreResults)
            {
                var sub = await query.ReadNextAsync();
                items.AddRange(sub.Resource);
            }

            return items.FirstOrDefault();
        }

        public async Task<T> UpsertAsync(T item)
        {
            item.PartitionKey = PartitionKey(item.Id).ToString();
            var response = await container.UpsertItemAsync<T>(item, new Microsoft.Azure.Cosmos.PartitionKey(item.PartitionKey));

            return response.Resource;
        }

        public async Task DeleteAsync(string id)
        {
            var pk = PartitionKey(id);
            await container.DeleteItemAsync<T>(id, new Microsoft.Azure.Cosmos.PartitionKey(pk));
        }

        public int PartitionKey(string Id) => System.Math.Abs(Id.GetHashCode() % 1000);

    }
}