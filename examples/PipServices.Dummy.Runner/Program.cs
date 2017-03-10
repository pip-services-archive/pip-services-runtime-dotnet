using PipServices.Dummy.Run;

namespace PipServices.Dummy.Runner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var runner = new DummyProcessRunner();
            runner.RunWithDefaultConfigFile(args, "..\\..\\..\\..\\config\\config.yaml");
        }
    }
}