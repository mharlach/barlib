using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos.Linq;
using System.Linq;

namespace BarLib.ServiceHost
{
    public class UserFavoritesContext : IUserFavoritesContext
    {
        private readonly Container container;
        private ILogger<UserFavoritesContext> log;

        public UserFavoritesContext(ILogger<UserFavoritesContext> log, IConfiguration configuration)
        {
            this.log = log;
            var connectionString = configuration.GetValue<string>("Cosmos:ConnectionString");

            var client = new CosmosClient(connectionString);
            this.container = client.GetContainer("barlib", "models");
        }

        public async Task<IEnumerable<UserFavorites>> GetAsync()
        {
            var query = container.GetItemLinqQueryable<UserFavorites>().ToFeedIterator();

            var results = new List<UserFavorites>();
            while (query.HasMoreResults)
            {
                var items = await query.ReadNextAsync();
                results.AddRange(items);
            }

            return results;

        }

        public async Task<UserFavorites?> GetAsync(string userId)
        {
            var query = container.GetItemLinqQueryable<UserFavorites>()
                    .Where(x => x.UserId == userId.ToLower())
                    .ToFeedIterator();

            var results = await query.ReadNextAsync();
            return results.FirstOrDefault();
        }

        public async Task<UserFavorites> UpsertAsync(UserFavorites item)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
            {
                item.Id = Guid.NewGuid().ToString();
            }

            item.PartitionKey = System.Math.Abs(item.Id.GetHashCode() % 1000).ToString();

            var response = await container.UpsertItemAsync<UserFavorites>(item, new PartitionKey(item.PartitionKey));

            return response;
        }

        public async Task Delete(string userId)
        {
            var item = await GetAsync(userId);
            if (item == null)
            {
                return;
            }

            var response = await container.DeleteItemAsync<UserFavorite>(item.Id, new PartitionKey(item.PartitionKey));
            
        }
    }
}