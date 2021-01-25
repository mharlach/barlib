using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Primitives;
using System.Linq;

namespace BarLib.ServiceHost.Functions
{
    public class SystemStatsController
    {
        private readonly ILogger log;
        private readonly ISystemStatsStorageContext context;
        private readonly IStorageContext<Drink> drinksContext;

        public SystemStatsController(

            ILoggerFactory factory,
            ISystemStatsStorageContext context,
            IStorageContext<Drink> drinksContext)
        {
            this.log = factory.CreateLogger<SystemStatsController>();
            this.context = context;
            this.drinksContext = drinksContext;
        }

        [FunctionName("SystemStats")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "stats")] HttpRequest req)
        {
            var stats = await context.GetAsync();
            if (stats == null)
            {
                stats = new SystemStats { DrinksVersion = Guid.NewGuid().ToString() };
                await context.UpsertAsync(stats);

            }

            return new OkObjectResult(stats);
        }

        // private async Task<SystemStats> RefreshAsync(SystemStats? stats)
        // {
        //     var allDrinks = await drinksContext.GetAsync();
        //     stats = stats ?? new SystemStats { DrinkIds = allDrinks.Select(x => x.Id).ToList() };
        //     stats.CalculateDrinkHashCode();

        //     return stats;
        // }
    }
}
