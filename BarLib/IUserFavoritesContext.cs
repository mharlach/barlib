using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarLib
{
    public interface IUserFavoritesContext
    {
        Task<IEnumerable<UserFavorites>> GetAsync();

        Task<UserFavorites?> GetAsync(string userId);

        Task<UserFavorites> UpsertAsync(UserFavorites item);

        Task Delete(string id);
    }
}