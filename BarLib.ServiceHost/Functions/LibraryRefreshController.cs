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
    public class LibraryRefreshController
    {
        private readonly ILogger log;
        private readonly IUserStorageContext<UserBar> barContext;
        private readonly ILibraryStorageContext libraryContext;
        private readonly ILibraryGenerator libraryGenerator;

        public LibraryRefreshController(ILoggerFactory factory, IUserStorageContext<UserBar> barContext, ILibraryStorageContext libraryContext, ILibraryGenerator libraryGenerator)
        {
            this.log = factory.CreateLogger<LibraryRefreshController>();
            this.barContext = barContext;
            this.libraryContext = libraryContext;
            this.libraryGenerator = libraryGenerator;
        }

        [FunctionName("UserLibrary_Refresh")]
        public async Task<IActionResult> RefreshAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users/{userId}/bars/{barId}/library/refresh")] HttpRequest req,
            string userId, string barId)
        {
            var bar = await barContext.GetAsync(userId, barId);
            if (bar == null)
            {
                return new NotFoundResult();
            }

            var library = await libraryContext.GetAsync(userId, barId);

            library = await libraryGenerator.BuildLibraryAsync(bar, library);

            await libraryContext.UpsertAsync(library);

            return new OkObjectResult(library);
        }
    }
}