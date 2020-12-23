using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BarLib.ServiceHost.Functions
{
    public class DrinksController
    {
        public readonly ILogger log;
        public readonly IStorageContext<Drink> context;

        public DrinksController(ILogger log, IStorageContext<Drink> context)
        {
            this.log = log;
            this.context = context;
        }

        [FunctionName("Drinks_GetPost")]
        public async Task<IActionResult> RunAsync_GetPost(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "drinks")] HttpRequest req)
        {
            IActionResult response = req.Method.ToUpper() switch
            {
                "GET" => await GetAsync(),
                "POST" => await UpsertAsync(req),
                _ => new BadRequestResult(),
            };

            return response;
        }

        [FunctionName("Drinks_GetDelete")]
        public async Task<IActionResult> RunAsync_GetDelete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "delete", "put" Route = "drinks/{id}")] HttpRequest req,
            string id)
        {
            IActionResult response = req.Method.ToUpper() switch
            {
                "GET" => await GetAsync(id),
                "DELETE" => await DeleteAsync(id),
                _ => new BadRequestResult(),
            };

            return response;
        }

        private async Task<IActionResult> GetAsync()
        {
            var items = await context.GetAsync();
            return new OkObjectResult(items);
        }

        private async Task<IActionResult> GetAsync(string id)
        {
            var item = await context.GetAsync(id);
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
            var ingredient = JsonConvert.DeserializeObject<Drink>(requestBody);

            id = id ?? Guid.NewGuid().ToString();
            if (ingredient.Id.Equals(id, StringComparison.OrdinalIgnoreCase) == false)
            {
                return new BadRequestObjectResult("Id in model doesn't match route")
            }

            ingredient = await context.UpsertAsync(ingredient);

            return new OkObjectResult(ingredient);
        }

        private async Task<IActionResult> DeleteAsync(string id)
        {
            await context.DeleteAsync(id);
            return new NoContentResult();
        }

    }
}
