using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipServices.Runtime.Errors
{
    [TestClass]
    public class MicroserviceErrorTest
    {
        [TestMethod]
        public void TestMicroserviceError()
        {
            var error = new MicroserviceError(ErrorCategory.UnknownError, 500, null, "TestError", "Test error")
                .ForComponent("TestComponent");

            Assert.AreEqual("TestComponent", error.Component);
            Assert.AreEqual("TestError", error.Code);
            Assert.AreEqual("Test error", error.Message);

            error = new MicroserviceError(ErrorCategory.UnknownError, 500, null, null, null)
                .ForComponent("TestComponent");

            Assert.AreEqual("TestComponent", error.Component);
            Assert.AreEqual("Undefined", error.Code);
            Assert.AreEqual("Unknown error", error.Message);
        }
    }
}