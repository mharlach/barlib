using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace BarLib.ServiceHost
{
    public class ModelBaseStorageContext<T> : StorageContextBase<T>, IStorageContext<T> where T : ModelBase
    {
        private ILogger log;

        public ModelBaseStorageContext(ILoggerFactory factory, IConfiguration config)
            : base(factory, config, "models")
        {
            this.log = factory.CreateLogger<ModelBaseStorageContext<T>>();
        }

        public async Task<T?> GetAsync(string id)
        {
            var queryDef = new QueryDefinition("SELECT * FROM c WHERE c.id=@id").WithParameter("@id", id);
            var response = await GetAsync(queryDef);
            return response.FirstOrDefault();
        }
    }


}