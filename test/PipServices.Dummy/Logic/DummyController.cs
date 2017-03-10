using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PipServices.Dummy.Data;
using PipServices.Dummy.Persistence;
using PipServices.Runtime;
using PipServices.Runtime.Config;
using PipServices.Runtime.Data;
using PipServices.Runtime.Logic;
using PipServices.Runtime.Portability;

namespace PipServices.Dummy.Logic
{
    public class DummyController : AbstractController, IDummyBusinessLogic
    {
        /// <summary>
        ///     Unique descriptor for the DummyController component
        /// </summary>
        public static readonly ComponentDescriptor ClassDescriptor = new ComponentDescriptor(
            Category.Controllers, "pip-services-dummies", "*", "*"
            );

        private IDummyPersistence _db;
        private readonly List<IDummyBusinessLogicListener> _listeners = new List<IDummyBusinessLogicListener>();

        public DummyController()
            : base(ClassDescriptor)
        {
        }

        public override void Link(DynamicMap context, ComponentSet components)
        {
            base.Link(context, components);

            // Locate reference to dummy persistence component
            _db = (IDummyPersistence) components.GetOneRequired(
                new ComponentDescriptor(Category.Persistence, "pip-services-dummies", null, null)
                );

            // Add commands
            var commands = new DummyCommandSet(this);
            AddCommandSet(commands);
        }

        public void AddListener(IDummyBusinessLogicListener listener)
        {
            _listeners.Add(listener);
        }

        public void RemoveListener(IDummyBusinessLogicListener listener)
        {
            _listeners.Remove(listener);
        }

        public async Task<DataPage<DummyObject>> GetDummiesAsync(string correlationId, FilterParams filter,
            PagingParams paging, CancellationToken cancellationToken)
        {
            using (var timing = Instrument(correlationId, "dummy.get_dummies"))
            {
                return await _db.GetDummiesAsync(correlationId, filter, paging, cancellationToken);
            }
        }

        public async Task<DummyObject> GetDummyByIdAsync(string correlationId, string dummyId,
            CancellationToken cancellationToken)
        {
            using (var timing = Instrument(correlationId, "dummy.get_dummy_by_id"))
            {
                return await _db.GetDummyByIdAsync(correlationId, dummyId, cancellationToken);
            }
        }

        public async Task<DummyObject> CreateDummyAsync(string correlationId, DummyObject dummy,
            CancellationToken cancellationToken)
        {
            using (var timing = Instrument(correlationId, "dummy.create_dummy"))
            {
                try
                {
                    var result = await _db.CreateDummyAsync(correlationId, dummy, cancellationToken);

                    foreach (var listener in _listeners)
                    {
                        listener.OnDummyCreated(correlationId, result.Id, result);
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    foreach (var listener in _listeners)
                    {
                        listener.OnDummyCreateFailed(correlationId, dummy, ex);
                    }

                    throw ex;
                }
            }
        }

        public async Task<DummyObject> UpdateDummyAsync(string correlationId, string dummyId, object dummy,
            CancellationToken cancellationToken)
        {
            using (var timing = Instrument(correlationId, "dummy.update_dummy"))
            {
                try
                {
                    var result = await _db.UpdateDummyAsync(correlationId, dummyId, dummy, cancellationToken);

                    foreach (var listener in _listeners)
                    {
                        listener.OnDummyUpdated(correlationId, dummyId, result);
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    foreach (var listener in _listeners)
                    {
                        listener.OnDummyUpdateFailed(correlationId, dummyId, dummy, ex);
                    }

                    throw ex;
                }
            }
        }

        public async Task<DummyObject> DeleteDummyAsync(string correlationId, string dummyId,
            CancellationToken cancellationToken)
        {
            using (var timing = Instrument(correlationId, "dummy.delete_dummy"))
            {
                try
                {
                    var result = await _db.DeleteDummyAsync(correlationId, dummyId, cancellationToken);

                    foreach (var listener in _listeners)
                    {
                        listener.OnDummyDeleted(correlationId, dummyId, result);
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    foreach (var listener in _listeners)
                    {
                        listener.OnDummyDeleteFailed(correlationId, dummyId, ex);
                    }

                    throw ex;
                }
            }
        }
    }
}