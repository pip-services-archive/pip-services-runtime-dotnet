using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace PipServices.Runtime.Cache
{
    internal sealed class ActorCacheAdapter : IServiceFabricCacheAdapter
    {
        private readonly IActorStateManager _actorStateManger;

        public ActorCacheAdapter(IActorStateManager actorStateManger)
        {
            if (actorStateManger == null)
                throw new ArgumentNullException(nameof(actorStateManger));

            _actorStateManger = actorStateManger;
        }
        
        public Task<T> RetrieveAsync<T>(string key, CancellationToken token)
        {
            return _actorStateManger.GetStateAsync<T>(key, token);
        }

        public async Task<T> StoreAsync<T>(string key, T value, CancellationToken token)
        {
            await _actorStateManger.SetStateAsync(key, value, token);
            return value;
        }

        public Task RemoveAsync(string key, CancellationToken token)
        {
            return _actorStateManger.RemoveStateAsync(key, token);
        }
    }
}