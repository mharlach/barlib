using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarLib
{
    public interface IStorageContext<T> where T : StorageObjectBase
    {
        Task<IList<T>> GetAsync();
        Task<T?> GetAsync(string id);
        Task<T> UpsertAsync(T item);
        Task DeleteAsync(string id);
    }

    
}