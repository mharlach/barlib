using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace BarLib.ServiceHost
{
    public class UserModelBaseStorageContext<T> : StorageContextBase<T>, IStorageContext<T> where T : UserModelBase
    {
        private ILogger log;

        public UserModelBaseStorageContext(ILoggerFactory factory, IConfiguration config)
            : base(factory, config, "user")
        {
            this.log = factory.CreateLogger<UserModelBaseStorageContext<T>>();
        }

        public async Task<T?> GetAsync(string id)
        {
            var queryDef = new QueryDefinition("SELECT * FROM c WHERE c.userId=@userId").WithParameter("@userId", id);
            return await GetAsync(queryDef);
        }
    }


}