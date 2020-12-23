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
    public class UserLibraryController
    {
        private readonly ILogger log;
        private readonly IStorageContext<UserLibrary> context;

        public UserLibraryController(ILogger log, IStorageContext<UserLibrary> context)
        {
            this.log = log;
            this.context = context;
        }

        [FunctionName("UserLibrary")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post","put","delete", Route = "users/{userId}/library")] HttpRequest req,
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
            var bar = JsonConvert.DeserializeObject<UserLibrary>(requestBody);
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
