using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.Azure.Cosmos.Linq;

namespace BarLib.ServiceHost
{

    public class SystemStatsStorageContext : ISystemStatsStorageContext
    {
        private readonly Container container;
        private ILogger log;

        public SystemStatsStorageContext(ILoggerFactory factory, IConfiguration configuration)
        {
            this.log = factory.CreateLogger<SystemStatsStorageContext>();
            var connectionString = configuration.GetValue<string>("Cosmos:ConnectionString");

            var client = new CosmosClient(connectionString);
            this.container = client.GetContainer("barlib", "models");
        }

        public async Task<SystemStats> GetAsync()
        {
            var feed = container.GetItemLinqQueryable<SystemStats>().ToFeedIterator();
            var items = await feed.ReadNextAsync();

            var s = items.FirstOrDefault();
            if (s == null)
            {
                s = new SystemStats { DrinksVersion = Guid.NewGuid().ToString() };
                s = await UpsertAsync(s);
            }

            return s;

        }

        public async Task<SystemStats> UpsertAsync(SystemStats item)
        {
            item.Updated = DateTime.UtcNow;
            await container.UpsertItemAsync<SystemStats>(item, new PartitionKey("STATS"));
            return item;
        }
    }

}