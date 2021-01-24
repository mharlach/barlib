using System;
using System.Linq;
using System.Collections.Generic;
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
    public class LibraryRefresh
    {
        private readonly ILogger log;
        private readonly IUserStorageContext<UserBar> barContext;
        private readonly ILibraryStorageContext libraryContext;
        private readonly ILibraryGenerator libraryGenerator;

        public LibraryRefresh(ILoggerFactory factory, IUserStorageContext<UserBar> barContext, ILibraryStorageContext libraryContext, ILibraryGenerator libraryGenerator)
        {
            this.log = factory.CreateLogger<LibraryRefresh>();
            this.barContext = barContext;
            this.libraryContext = libraryContext;
            this.libraryGenerator = libraryGenerator;
        }

        [FunctionName("UserLibrary_Refresh")]
        public async Task<IActionResult> RefreshAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users/{userId}/bars/{barId}/library/refresh")] HttpRequest req,
            string userId, string barId)
        {
            var getLibraryTask = libraryContext.GetAsync(userId, barId);
            var bar = await barContext.GetAsync(userId, barId);
            if (bar == null)
            {
                return new NotFoundResult();
            }

            var updatedDrinks = await libraryGenerator.BuildAsync(bar);

            var library = await getLibraryTask;
            if (library == null)
            {
                library = new UserLibrary
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    BarId = barId,
                };
            }
            library.Updated = DateTime.UtcNow;
            library.Drinks = updatedDrinks.Select(x => new ItemPair
            {
                Id = x.Id,
                Name = x.Name,
            }).ToList();

            await libraryContext.UpsertAsync(library);

            return new OkObjectResult(library);
        }
    }
}