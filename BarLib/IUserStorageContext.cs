using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarLib
{
    public interface IUserStorageContext<T> where T : UserModelBase{
        
        Task<IList<T>> GetAsync(string userId);
        Task<T?> GetAsync(string userId, string id);
        Task<T> UpsertAsync(T item);
        Task DeleteAsync(string id);

    }
    
}