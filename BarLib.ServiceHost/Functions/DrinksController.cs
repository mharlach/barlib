using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;

namespace BarLib.ServiceHost.Functions
{
    public class DrinksController
    {
        private readonly ILogger log;
        private readonly IStorageContext<Drink> drinkContext;
        private readonly IStorageContext<Ingredient> ingredientContext;
        private readonly ISystemStatService systemStatService;

        public DrinksController(
            ILogger<DrinksController> log, 
            IStorageContext<Drink> drinkContext, 
            IStorageContext<Ingredient> ingredientContext,
            ISystemStatService systemStatService)
        {
            this.log = log;
            this.drinkContext = drinkContext;
            this.ingredientContext = ingredientContext;
            this.systemStatService = systemStatService;
        }

        [FunctionName("Drinks_GetPost")]
        public async Task<IActionResult> RunAsync_GetPost(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "drinks")] HttpRequest req)
        {
            IActionResult response = req.Method.ToUpper() switch
            {
                "GET" => await GetAsync(),
                "POST" => await UpsertAsync(req, null),
                _ => new BadRequestResult(),
            };

            return response;
        }

        [FunctionName("Drinks_GetDelete")]
        public async Task<IActionResult> RunAsync_GetDelete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "delete", "put", Route = "drinks/{id}")] HttpRequest req,
            string id)
        {
            IActionResult response = req.Method.ToUpper() switch
            {
                "GET" => await GetAsync(id),
                "DELETE" => await DeleteAsync(id),
                "PUT" => await UpsertAsync(req, id),
                _ => new BadRequestResult(),
            };

            return response;
        }

        private async Task<IActionResult> GetAsync()
        {
            var items = await drinkContext.GetAsync();
            return new OkObjectResult(items);
        }

        private async Task<IActionResult> GetAsync(string id)
        {
            var item = await drinkContext.GetAsync(id);
            if (item == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(item);
            }
        }

        private async Task<IActionResult> UpsertAsync(HttpRequest req, string? id)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var drink = JsonConvert.DeserializeObject<Drink>(requestBody);

            id = id ?? Guid.NewGuid().ToString();
            drink.Id = id;

            drink = await drinkContext.UpsertAsync(drink);
            await systemStatService.UpdateDrinkVersionAsync();

            return new OkObjectResult(drink);
        }

        private async Task<IActionResult> DeleteAsync(string id)
        {
            await drinkContext.DeleteAsync(id);
            return new NoContentResult();
        }

    }
}
