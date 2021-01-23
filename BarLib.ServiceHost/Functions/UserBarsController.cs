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
    public class UserBarsController
    {
        public readonly ILogger log;
        public readonly IStorageContext<UserBar> context;

        public UserBarsController(ILogger<UserBarsController> log, IStorageContext<UserBar> context)
        {
            this.log = log;
            this.context = context;
        }

        [FunctionName("UserBars")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "users/{userId}/bars")] HttpRequest req,
            string userId)
        {
            IActionResult response = req.Method.ToUpper() switch
            {
                "GET" => await GetAsync(userId),
                "POST" => await UpsertAsync(req, userId, null),
                // "PUT" => await UpsertAsync(req, userId),
                "DELETE" => await DeleteAsync(userId),
                _ => new BadRequestResult(),
            };

            return response;
        }

        [FunctionName("UserBars_GetPutDelete")]
        public async Task<IActionResult> RunAsync_GetPutDelete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "put", "delete", Route = "users/{userId}/bars/{barId}")] HttpRequest req,
            string userId, string barId)
        {
            IActionResult response = req.Method.ToUpper() switch
            {
                "GET" => await GetAsync(userId),
                // "POST" => await UpsertAsync(req, userId, null),
                "PUT" => await UpsertAsync(req, userId, barId),
                "DELETE" => await DeleteAsync(userId),
                _ => new BadRequestResult(),
            };

            return response;
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

        private async Task<IActionResult> UpsertAsync(HttpRequest req, string userId, string? barId)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var bar = JsonConvert.DeserializeObject<UserBar>(requestBody);
            bar.HashCode = bar.GetHashCode();
            bar.UserId = userId;
            bar.Id = barId ?? bar.Id;

            bar = await context.UpsertAsync(bar);

            return new OkObjectResult(bar);
        }

        private async Task<IActionResult> DeleteAsync(string id)
        {
            await context.DeleteAsync(id);
            return new NoContentResult();
        }

    }
}
