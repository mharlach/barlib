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

        public UserBarsController(ILogger log, IStorageContext<UserBar> context)
        {
            this.log = log;
            this.context = context;
        }

        [FunctionName("UserBars")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post","put","delete", Route = "users/{userId}/bar")] HttpRequest req,
            string userId)
        {
            IActionResult response = req.Method.ToUpper() switch
            {
                "GET" => await GetAsync(userId),
                "POST" => await UpsertAsync(req),
                "PUT" => await UpsertAsync(req),
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

        private async Task<IActionResult> UpsertAsync(HttpRequest req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var bar = JsonConvert.DeserializeObject<UserBar>(requestBody);
            bar.HashCode = bar.GetHashCode();

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
