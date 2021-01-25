using System.Collections.Generic;
using System.Threading.Tasks;

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
}