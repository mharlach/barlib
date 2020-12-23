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
        private readonly IStorageContext<UserBar> barContext;
        private readonly IStorageContext<UserLibrary> libraryContext;
        private readonly ILibraryGenerator libraryGenerator;

        public LibraryRefresh(ILogger log, IStorageContext<UserBar> barContext, IStorageContext<UserLibrary> libraryContext, ILibraryGenerator libraryGenerator)
        {
            this.log = log;
            this.barContext = barContext;
            this.libraryContext = libraryContext;
            this.libraryGenerator = libraryGenerator;
        }

        [FunctionName("UserLibrary_Refresh")]
        public async Task<IActionResult> RefreshAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users/{userId}/library/refresh")] HttpRequest req,
            string userId)
        {
            var getLibraryTask = libraryContext.GetAsync(userId);
            var bar = await barContext.GetAsync(userId);
            if (bar == null)
            {
                return new NotFoundResult();
            }

            var updatedDrinks = await libraryGenerator.BuildAsync(bar);
            
            var library = await getLibraryTask;
            library.Updated = DateTime.UtcNow;
            library.Drinks = updatedDrinks.Select(x=>x.Id).ToList();

            return new NoContentResult();
        }
    }
}