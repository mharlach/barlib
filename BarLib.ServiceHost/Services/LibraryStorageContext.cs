using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BarLib
{
    public interface ILibraryStorageContext
    {
        Task<List<UserLibrary>> GetAll();

        Task<List<UserLibrary>> GetAll(string userId);

        Task<UserLibrary?> GetAsync(string userId, string barId);

        Task<UserLibrary?> GetAsync(string id);

        Task<UserLibrary> UpsertAsync(UserLibrary item);

        Task DeleteAsync(string id);
    }

    public class LibraryStorageContext : ILibraryStorageContext
    {
        private readonly Container container;
        private ILogger log;


        public LibraryStorageContext(ILoggerFactory factory, IConfiguration configuration)
        {
            this.log = factory.CreateLogger<LibraryStorageContext>();
            var connectionString = configuration.GetValue<string>("Cosmos:ConnectionString");

            var client = new CosmosClient(connectionString);
            this.container = client.GetContainer("barlib", "models");
        }

        public Task DeleteAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<UserLibrary>> GetAll()
        {
            var def = new QueryDefinition("SELECT * FROM c WHERE c.type=@Type")
                .WithParameter("@Type", "userLibrary");

            var items = await QueryAsync(def);
            return items;
        }

        public async Task<List<UserLibrary>> GetAll(string userId)
        {
            var def = new QueryDefinition("SELECT * FROM c WHERE c.userId=@UserId AND c.type=@Type")
                .WithParameter("@UserId", userId)
               .WithParameter("@Type", "userLibrary");

            var items = await QueryAsync(def);
            return items;
        }

        public async Task<UserLibrary?> GetAsync(string userId, string barId)
        {
            var def = new QueryDefinition("SELECT * FROM c WHERE c.userId=@UserId AND c.barId=@BarId AND c.type=@Type")
                .WithParameter("@UserId", userId)
                .WithParameter("@BarID", barId)
               .WithParameter("@Type", "userLibrary");

            var items = await QueryAsync(def);
            return items.FirstOrDefault();
        }

        public async Task<UserLibrary?> GetAsync(string id)
        {
            var def = new QueryDefinition("SELECT * FROM c WHERE c.Id=@Id AND c.type=@Type")
                .WithParameter("@Id", id)
               .WithParameter("@Type", "userLibrary");

            var items = await QueryAsync(def);
            return items.FirstOrDefault();
        }

        public async Task<UserLibrary> UpsertAsync(UserLibrary item)
        {
            item.PartitionKey = System.Math.Abs(item.Id.GetHashCode() % 1000).ToString();
            var response = await container.UpsertItemAsync<UserLibrary>(item, new PartitionKey(item.PartitionKey));

            return response.Resource;
        }

        private async Task<List<UserLibrary>> QueryAsync(QueryDefinition def)
        {
            var query = container.GetItemQueryIterator<UserLibrary>(def);
            var items = new List<UserLibrary>();

            while (query.HasMoreResults)
            {
                var sub = await query.ReadNextAsync();
                items.AddRange(sub.Resource);
            }

            return items;
        }
    }
}