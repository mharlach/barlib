using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Collections.Generic;
using System;

namespace BarLib.ServiceHost
{
    public class UserModelBaseStorageContext<T> : StorageContextBase<T>, IStorageContext<T> where T : UserModelBase
    {
        private ILogger log;

        public UserModelBaseStorageContext(ILoggerFactory factory, IConfiguration config)
        : base(factory, config, "models")
        {
            this.log = factory.CreateLogger<UserModelBaseStorageContext<T>>();
        }

        public async Task<T?> GetAsync(string id)
        {
            var queryDef = new QueryDefinition("SELECT * FROM c WHERE c.userId=@userId").WithParameter("@userId", id);
            var response = await GetAsync(queryDef);
            return response.FirstOrDefault();
        }
    }

    public class UserModelBaseStorageContext2<T> : IUserStorageContext<T> where T : UserModelBase
    {
        private readonly Container container;
        private ILogger log;

        public UserModelBaseStorageContext2(ILoggerFactory factory, IConfiguration configuration)
        {
            this.log = factory.CreateLogger<UserModelBaseStorageContext2<T>>();
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
            var def = new QueryDefinition("SELECT * FROM c WHERE c.userId=@UserId AND c.Id=@Id AND c.type=@Type")
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

        public async Task<T> UpserAsync(T item)
        {
            item.PartitionKey = PartitionKey(item.Id).ToString();
            var response = await container.UpsertItemAsync<T>(item, new Microsoft.Azure.Cosmos.PartitionKey(item.PartitionKey));

            return response.Resource;
        }

        public Task DeleteAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public int PartitionKey(string Id) => System.Math.Abs(Id.GetHashCode() % 1000);

    }


}