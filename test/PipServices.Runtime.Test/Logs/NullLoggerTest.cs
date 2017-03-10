using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipServices.Runtime.Logs
{
    [TestClass]
    public class NullLoggerTest
    {
        private ILogger Log { get; set; }
        private LoggerFixture Fixture { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Log = new NullLogger();
            Fixture = new LoggerFixture(Log);
        }

        [TestMethod]
        public void TestLogLevel()
        {
            Fixture.TestLogLevel();
        }

        [TestMethod]
        public void TestTextOutput()
        {
            Fixture.TestTextOutput();
        }

        [TestMethod]
        public void TestMixedOutput()
        {
            Fixture.TestMixedOutput();
        }
    }
}