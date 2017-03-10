using System;
using System.Linq;
using System.Threading;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Data;
using PipServices.Runtime.Config;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Cache
{
    public sealed class ServiceFabricCache : AbstractCache
    {
        /// <summary>
        ///     Unique descriptor for the ServiceFabricLogger component
        /// </summary>
        public static readonly ComponentDescriptor ClassDescriptor = new ComponentDescriptor(
            Category.Cache, "pip-services-runtime-log", "sf-state", "*"
            );

        public ServiceFabricCache() 
            : base(ClassDescriptor)
        {
        }

        private bool _isService;

        private IServiceFabricCacheAdapter _adapter;
        private IServiceFabricCacheAdapter Adapter
        {
            get
            {
                if (_adapter == null || !_isService)
                    _adapter = GetAdapter();

                return _adapter;
            }
        }

        private IServiceFabricCacheAdapter GetAdapter()
        {
            var actorStateManagers = Context.Get<DynamicMap>("service_fabric.actor_state_managers");
            if (actorStateManagers != null && actorStateManagers.Count > 0)
            {
                var actorStateManger = actorStateManagers.Values.FirstOrDefault() as IActorStateManager;
                _isService = false;
                if (actorStateManger != null)
                    return new ActorCacheAdapter(actorStateManger);
            }
            else
            {
                var serviceStateManager = Context.Get<IReliableStateManager>("service_fabric.actor_state_managers");
                _isService = true;
                if (serviceStateManager != null)
                    return new ServiceCacheAdapter(serviceStateManager);
            }

            return null;
        }

        public override object Retrieve(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var task = Adapter?.RetrieveAsync<object>(key, CancellationToken.None);
            task?.Wait();
            return task?.Result;
        }

        public override object Store(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var task = Adapter?.StoreAsync(key, value, CancellationToken.None);
            task?.Wait();
            return task?.Result;
        }

        public override void Remove(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var task = Adapter?.RemoveAsync(key, CancellationToken.None);
            task?.Wait();
        }
    }
}
