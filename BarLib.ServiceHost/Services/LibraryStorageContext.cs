using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
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
            var feed = container.GetItemLinqQueryable<UserLibrary>()
                            .ToFeedIterator();

            var results = await QueryAsync(feed);
            return results.ToList();
        }

        public async Task<List<UserLibrary>> GetAll(string userId)
        {
            var feed = container.GetItemLinqQueryable<UserLibrary>()
                .Where(z => z.UserId == userId)
                .ToFeedIterator();

            var results = await QueryAsync(feed);
            return results.ToList();
        }

        // public async Task<UserLibrary?> GetAsync()

        public async Task<UserLibrary?> GetAsync(string userId, string barId)
        {
            var feed = container.GetItemLinqQueryable<UserLibrary>(true)
                .Where(z => z.UserId == userId && z.BarId == barId)
                .ToFeedIterator();

            var results = await QueryAsync(feed);
            return results.FirstOrDefault();
        }

        public async Task<UserLibrary?> GetAsync(string id)
        {
            var feed = container.GetItemLinqQueryable<UserLibrary>()
                .Where(z => z.Id == id)
                .ToFeedIterator();

            var results = await QueryAsync(feed);
            return results.FirstOrDefault();
        }

        public async Task<UserLibrary> UpsertAsync(UserLibrary item)
        {
            item.PartitionKey = System.Math.Abs(item.Id.GetHashCode() % 1000).ToString();
            var response = await container.UpsertItemAsync<UserLibrary>(item, new PartitionKey(item.PartitionKey));

            return response.Resource;
        }

        public async Task<IEnumerable<UserLibrary>> QueryAsync(FeedIterator<UserLibrary> feed)
        {
            var items = new List<UserLibrary>();
            while (feed.HasMoreResults)
            {
                items.AddRange(await feed.ReadNextAsync());
            }

            return items;
        }
    }
}