using System;
using PipServices.Dummy.Data;

namespace PipServices.Dummy.Logic
{
    public interface IDummyBusinessLogicListener
    {
        void OnDummyCreated(string correlationId, string dummyId, DummyObject dummy);
        void OnDummyCreateFailed(string correlationId, DummyObject dummy, Exception error);

        void OnDummyUpdated(string correlationId, string dummyId, DummyObject dummy);
        void OnDummyUpdateFailed(string correlationId, string dummyId, object dummy, Exception error);

        void OnDummyDeleted(string correlationId, string dummyId, DummyObject dummy);
        void OnDummyDeleteFailed(string correlationId, string dummyId, Exception error);
    }
}