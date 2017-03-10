using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipServices.Runtime.Logs
{
    public class LoggerFixture
    {
        private readonly ILogger _logger;

        public LoggerFixture(ILogger logger)
        {
            _logger = logger;
        }

        public void TestLogLevel()
        {
            Assert.IsTrue(_logger.Level >= LogLevel.None);
            Assert.IsTrue(_logger.Level <= LogLevel.Trace);
        }

        public void TestTextOutput()
        {
            _logger.Log(LogLevel.Fatal, "ABC", "123", new object[] {"Fatal error..."});
            _logger.Log(LogLevel.Error, "ABC", "123", new object[] {"Recoverable error..."});
            _logger.Log(LogLevel.Warn, "ABC", "123", new object[] {"Warning..."});
            _logger.Log(LogLevel.Info, "ABC", "123", new object[] {"Information message..."});
            _logger.Log(LogLevel.Debug, "ABC", "123", new object[] {"Debug message..."});
            _logger.Log(LogLevel.Trace, "ABC", "123", new object[] {"Trace message..."});
        }

        public void TestMixedOutput()
        {
            object obj = new {abc = "ABC"};

            _logger.Log(LogLevel.Fatal, "ABC", "123", new[] {123, "ABC", obj});
            _logger.Log(LogLevel.Error, "ABC", "123", new[] {123, "ABC", obj});
            _logger.Log(LogLevel.Warn, "ABC", "123", new[] {123, "ABC", obj});
            _logger.Log(LogLevel.Info, "ABC", "123", new[] {123, "ABC", obj});
            _logger.Log(LogLevel.Debug, "ABC", "123", new[] {123, "ABC", obj});
            _logger.Log(LogLevel.Trace, "ABC", "123", new[] {123, "ABC", obj});
        }
    }
}