using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace BarLib
{
    public interface IStorageContext<T> where T : StorageObjectBase
    {
        Task<IList<T>> GetAsync();
        Task<T?> GetAsync(string id);
        Task<IList<T>> GetAsync(QueryDefinition queryDef);
        Task<T> UpsertAsync(T item);
        Task DeleteAsync(string id);
    }

    
}