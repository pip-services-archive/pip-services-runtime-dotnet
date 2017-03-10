using PipServices.Dummy.Build;
using PipServices.Runtime.Run;

namespace PipServices.Dummy.Run
{
    /// <summary>
    ///     Dummy microservice class.
    /// </summary>
    public class DummyMicroservice : Microservice
    {
        public DummyMicroservice()
            : base("pip-services-dummies", DummyFactory.Instance)
        {
        }
    }
}