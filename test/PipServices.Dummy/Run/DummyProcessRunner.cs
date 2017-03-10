using PipServices.Runtime.Run;

namespace PipServices.Dummy.Run
{
    public class DummyProcessRunner : ProcessRunner
    {
        public DummyProcessRunner()
            : base(new DummyMicroservice())
        {
        }
    }
}