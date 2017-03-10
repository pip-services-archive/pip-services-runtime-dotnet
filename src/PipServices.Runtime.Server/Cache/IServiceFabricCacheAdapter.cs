using System.Threading;
using System.Threading.Tasks;

namespace PipServices.Runtime.Cache
{
    internal interface IServiceFabricCacheAdapter
    {
        Task<T> RetrieveAsync<T>(string key, CancellationToken token);
        Task<T> StoreAsync<T>(string key, T value, CancellationToken token);
        Task RemoveAsync(string key, CancellationToken token);
    }
}
