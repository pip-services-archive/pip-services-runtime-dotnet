using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace PipServices.Runtime.Cache
{
    internal sealed class ServiceCacheAdapter : IServiceFabricCacheAdapter
    {
        private readonly IReliableStateManager _serviceStateManager;
        private readonly IReliableDictionary<string, object> _cacheDictionary;

        public ServiceCacheAdapter(IReliableStateManager serviceStateManager)
        {
            if (serviceStateManager == null)
                throw new ArgumentNullException(nameof(serviceStateManager));

            _serviceStateManager = serviceStateManager;
            var dictTask = _serviceStateManager.GetOrAddAsync<IReliableDictionary<string, object>>(nameof(_cacheDictionary));
            dictTask.Wait();
            _cacheDictionary = dictTask.Result;
        }

        public async Task<T> RetrieveAsync<T>(string key, CancellationToken token)
        {
            using (var tx = _serviceStateManager.CreateTransaction())
            {
                var result = await _cacheDictionary.TryGetValueAsync(tx, key);

                await tx.CommitAsync();

                return result.HasValue ? (T)result.Value : default(T);
            }
        }

        public async Task<T> StoreAsync<T>(string key, T value, CancellationToken token)
        {
            using (var tx = _serviceStateManager.CreateTransaction())
            {
                await _cacheDictionary.AddOrUpdateAsync(tx, key, value, (existedKey, existedValue) => value);

                await tx.CommitAsync();

                return value;
            }
        }

        public async Task RemoveAsync(string key, CancellationToken token)
        {
            using (var tx = _serviceStateManager.CreateTransaction())
            {
                await _cacheDictionary.TryRemoveAsync(tx, key);

                await tx.CommitAsync();
            }
        }
    }
}