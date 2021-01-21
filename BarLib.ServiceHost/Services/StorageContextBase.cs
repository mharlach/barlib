using System.Threading.Tasks;
using BarLib;
using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace BarLib.ServiceHost
{
    public abstract class StorageContextBase<T> where T : StorageObjectBase
    {
        protected readonly Container container;
        private ILogger log;

        public StorageContextBase(ILoggerFactory factory, IConfiguration configuration, string container)
        {
            this.log = factory.CreateLogger<StorageContextBase<T>>();
            var connectionString = configuration.GetValue<string>("Cosmos:ConnectionString");

            var client = new CosmosClient(connectionString);
            this.container = client.GetContainer("barlib", container);
        }

        public int PartitionKey(string Id) => System.Math.Abs(Id.GetHashCode() % 1000);

        public async Task<IList<T>> GetAsync()
        {
            var query = container.GetItemQueryIterator<T>("SELECT * FROM c");
            var items = new List<T>();

            while (query.HasMoreResults)
            {
                var sub = await query.ReadNextAsync();
                items.AddRange(sub.Resource);
            }

            return items;
        }

        public async Task<IList<T>> GetAsync(QueryDefinition queryDef)
        {
            var query = container.GetItemQueryIterator<T>(queryDef);
            var results = new List<T>();

            while (query.HasMoreResults)
            {
                var items = await query.ReadNextAsync();
                results.AddRange(items.Resource);
            }

            return results;
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
    }


}