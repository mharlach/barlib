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


    public interface IUserStorageContext<T> where T : UserModelBase{
        
        Task<IList<T>> GetAsync(string userId);
        Task<T?> GetAsync(string userId, string id);
        Task<T> UpsertAsync(T item);
        Task DeleteAsync(string id);

    }
    
}