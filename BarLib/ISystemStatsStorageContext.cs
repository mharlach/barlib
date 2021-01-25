using System.Threading.Tasks;

namespace BarLib
{
    public interface ISystemStatsStorageContext 
    {
        Task<SystemStats> GetAsync();
        Task<SystemStats> UpsertAsync(SystemStats item);

    }

    public interface ISystemStatService
    {
        Task UpdateDrinkVersionAsync();
    }
    
}