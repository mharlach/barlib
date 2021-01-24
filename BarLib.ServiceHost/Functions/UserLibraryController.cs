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
        private readonly ILibraryStorageContext context;

        public UserLibraryController(ILoggerFactory factory, ILibraryStorageContext context)
        {
            this.log = factory.CreateLogger<UserLibraryController>();
            this.context = context;
        }

        [FunctionName("UserLibrary")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post","put","delete", Route = "users/{userId}/bars/{barId}/library")] HttpRequest req,
            string userId, string barId)
        {
            IActionResult response = req.Method.ToUpper() switch
            {
                "GET" => await GetAsync(userId, barId),
                "POST" => await UpsertAsync(userId, barId, req),
                "PUT" => await UpsertAsync(userId, barId, req),
                "DELETE" => await DeleteAsync(userId),
                _ => new BadRequestResult(),
            };

            return response;
        }
        

        private async Task<IActionResult> GetAsync(string userId, string barId)
        {
            var item = await context.GetAsync(userId, barId);
            if (item == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(item);
            }
        }

        private async Task<IActionResult> UpsertAsync(string userId, string barId, HttpRequest req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var bar = JsonConvert.DeserializeObject<UserLibrary>(requestBody);
            bar.UserId = userId;
            bar.BarId = barId;
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
