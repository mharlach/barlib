using System.Threading.Tasks;
using System;

namespace BarLib.ServiceHost
{
    public class SystemStatsService : ISystemStatService
    {
        private readonly ISystemStatsStorageContext context;

        public SystemStatsService(ISystemStatsStorageContext context)
        {
            this.context = context;
        }

        public async Task UpdateDrinkVersionAsync()
        {
            var s = await context.GetAsync();
            s = s ?? new SystemStats();
            s.DrinksVersion = Guid.NewGuid().ToString();

            await context.UpsertAsync(s);
        }
    }

}