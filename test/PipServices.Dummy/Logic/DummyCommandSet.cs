using System;
using System.Threading;
using PipServices.Dummy.Data;
using PipServices.Runtime;
using PipServices.Runtime.Commands;
using PipServices.Runtime.Data;
using PipServices.Runtime.Portability;
using PipServices.Runtime.Validation;

namespace PipServices.Dummy.Logic
{
    public class DummyCommandSet : CommandSet, IDummyBusinessLogicListener
    {
        private readonly IDummyBusinessLogic _logic;

        public DummyCommandSet(IDummyBusinessLogic logic)
        {
            if (logic == null)
                throw new ArgumentNullException(nameof(logic));

            _logic = logic;

            // Register commands
            AddCommand(MakeGetDummiesCommand());
            AddCommand(MakeGetDummyByIdCommand());
            AddCommand(MakeCreateDummyCommand());
            AddCommand(MakeUpdateDummyCommand());
            AddCommand(MakeDeleteDummyCommand());

            // Register events
            AddEvent(new Event(logic, "dummy_created"));
            AddEvent(new Event(logic, "dummy_create_failed"));
            AddEvent(new Event(logic, "dummy_updated"));
            AddEvent(new Event(logic, "dummy_update_failed"));
            AddEvent(new Event(logic, "dummy_deleted"));
            AddEvent(new Event(logic, "dummy_delete_failed"));

            // Adds this command set as a listener
            _logic.AddListener(this);
        }

        public void OnDummyCreated(string correlationId, string dummyId, DummyObject dummy)
        {
            var value = DynamicMap.FromTuples(
                "dummy_id", dummyId,
                "dummy", dummy
                );
            Notify("dummy_created", correlationId, value);
        }

        public void OnDummyCreateFailed(string correlationId, DummyObject dummy, Exception error)
        {
            var value = DynamicMap.FromTuples(
                "dummy", dummy,
                "error", error
                );
            Notify("dummy_create_failed", correlationId, value);
        }

        public void OnDummyUpdated(string correlationId, string dummyId, DummyObject dummy)
        {
            var value = DynamicMap.FromTuples(
                "dummy_id", dummyId,
                "dummy", dummy
                );
            Notify("dummy_updated", correlationId, value);
        }

        public void OnDummyUpdateFailed(string correlationId, string dummyId, object dummy, Exception error)
        {
            var value = DynamicMap.FromTuples(
                "dummy_id", dummyId,
                "dummy", dummy,
                "error", error
                );
            Notify("dummy_update_failed", correlationId, value);
        }

        public void OnDummyDeleted(string correlationId, string dummyId, DummyObject dummy)
        {
            var value = DynamicMap.FromTuples(
                "dummy_id", dummyId,
                "dummy", dummy
                );
            Notify("dummy_created", correlationId, value);
        }

        public void OnDummyDeleteFailed(string correlationId, string dummyId, Exception error)
        {
            var value = DynamicMap.FromTuples(
                "dummy_id", dummyId,
                "error", error
                );
            Notify("dummy_delete_failed", correlationId, value);
        }

        private ICommand MakeGetDummiesCommand()
        {
            return new Command(
                _logic,
                "get_dummies",
                new Schema()
                    .WithOptionalProperty("filter", "FilterParams")
                    .WithOptionalProperty("paging", "PagingParams"),
                async (correlationId, args, cancellationToken) =>
                {
                    var filter = FilterParams.FromValue(args.Get("filter"));
                    var paging = PagingParams.FromValue(args.Get("paging"));
                    return await _logic.GetDummiesAsync(correlationId, filter, paging, CancellationToken.None);
                }
                );
        }

        private ICommand MakeGetDummyByIdCommand()
        {
            return new Command(
                _logic,
                "get_dummy_by_id",
                new Schema()
                    .WithProperty("dummy_id", "string"),
                async (correlationId, args, cancellationToken) =>
                {
                    var dummyId = args.GetNullableString("dummy_id");
                    return await _logic.GetDummyByIdAsync(correlationId, dummyId, CancellationToken.None);
                }
                );
        }

        private ICommand MakeCreateDummyCommand()
        {
            return new Command(
                _logic,
                "create_dummy",
                new Schema()
                    .WithProperty("dummy", "Dummy"),
                async (correlationId, args, cancellationToken) =>
                {
                    var dummy = (DummyObject) args.Get("dummy");
                    return await _logic.CreateDummyAsync(correlationId, dummy, CancellationToken.None);
                }
                );
        }

        private ICommand MakeUpdateDummyCommand()
        {
            return new Command(
                _logic,
                "update_dummy",
                new Schema()
                    .WithProperty("dummy_id", "string")
                    .WithProperty("dummy", "any"),
                async (correlationId, args, cancellationToken) =>
                {
                    var dummyId = args.GetNullableString("dummy_id");
                    var dummy = args.Get("dummy");
                    return await _logic.UpdateDummyAsync(correlationId, dummyId, dummy, CancellationToken.None);
                }
                );
        }

        private ICommand MakeDeleteDummyCommand()
        {
            return new Command(
                _logic,
                "delete_dummy",
                new Schema()
                    .WithProperty("dummy_id", "string"),
                async (correlationId, args, cancellationToken) =>
                {
                    var dummyId = args.GetNullableString("dummy_id");
                    await _logic.DeleteDummyAsync(correlationId, dummyId, CancellationToken.None);
                    return null;
                }
                );
        }
    }
}