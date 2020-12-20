using System.Threading.Tasks;
using BarLib;
using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace BarLib.ServiceHost
{
    public abstract class StorageContextBase
    {
        public async Task<List<T>> GetAsync<T>(Container container){
            var query = container.GetItemQueryIterator<T>("SELECT * FROM c");
            var items = new List<T>();

            while(query.HasMoreResults){
                var sub = await query.ReadNextAsync();
                items.AddRange(sub.Resource);
            }

            return items;
        }

        public T Get<T>(Container container, System.Linq.Expressions.Expression<System.Func<T, bool>> predicate)
        {
            var item = container.GetItemLinqQueryable<T>()
            .Where(predicate)
            .AsEnumerable()
            .FirstOrDefault();

            return item;
        }

        public async Task<T> UpsertAsync<T>(Container container, string id, T item)
        {
            var pk = new PartitionKey(PartitionKey(id));
            var response = await container.UpsertItemAsync<T>(item, pk);

            return response.Resource;
        }
        public async Task DeleteAsync<T>(Container container, string id)
        {
            await container.DeleteItemAsync<T>(id, new PartitionKey(PartitionKey(id)));
        }

        public int PartitionKey(string Id) => System.Math.Abs(Id.GetHashCode() % 1000);
    }
}