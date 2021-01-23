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
        public readonly IUserStorageContext<UserBar> context;

        public UserBarsController(ILogger<UserBarsController> log, IUserStorageContext<UserBar> context)
        {
            this.log = log;
            this.context = context;
        }

        [FunctionName("UserBars_GetPost")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "users/{userId}/bars")] HttpRequest req,
            string userId)
        {
            IActionResult response = req.Method.ToUpper() switch
            {
                "GET" => await GetAsync(userId, null),
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
                "GET" => await GetAsync(userId, barId),
                // "POST" => await UpsertAsync(req, userId, null),
                "PUT" => await UpsertAsync(req, userId, barId),
                "DELETE" => await DeleteAsync(userId),
                _ => new BadRequestResult(),
            };

            return response;
        }

        private async Task<IActionResult> GetAsync(string userId, string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                var bars = await context.GetAsync(userId);
                return new OkObjectResult(bars);
            }
            else
            {
                var bar = await context.GetAsync(userId, id);
                if (bar == null)
                {
                    return new NotFoundResult();
                }
                else
                {
                    return new OkObjectResult(bar);
                }
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
