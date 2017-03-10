using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipServices.Runtime.Config;

namespace PipServices.Runtime
{
    [TestClass]
    public class ComponentDescriptorTest
    {
        [TestMethod]
        public void TestMatch()
        {
            var descriptor = new ComponentDescriptor(Category.Controllers, "pip-services-dummies", "default", "1.0");

            // Check match by individual fields
            Assert.IsTrue(descriptor.Match(new ComponentDescriptor(Category.Controllers, null, null, null)));
            Assert.IsTrue(
                descriptor.Match(new ComponentDescriptor(Category.Controllers, "pip-services-dummies", null, null)));
            Assert.IsTrue(descriptor.Match(new ComponentDescriptor(Category.Controllers, null, "default", null)));
            Assert.IsTrue(descriptor.Match(new ComponentDescriptor(Category.Controllers, null, null, "1.0")));

            // Check match by individual "*" fields
            Assert.IsTrue(descriptor.Match(new ComponentDescriptor(Category.Controllers, "*", "*", "*")));
            Assert.IsTrue(
                descriptor.Match(new ComponentDescriptor(Category.Controllers, "pip-services-dummies", "*", "*")));
            Assert.IsTrue(descriptor.Match(new ComponentDescriptor(Category.Controllers, "*", "default", "*")));
            Assert.IsTrue(descriptor.Match(new ComponentDescriptor(Category.Controllers, "*", "*", "1.0")));

            // Check match by all values
            Assert.IsTrue(
                descriptor.Match(new ComponentDescriptor(Category.Controllers, "pip-services-dummies", "default", null)));
            Assert.IsTrue(descriptor.Match(new ComponentDescriptor(Category.Controllers, null, "default", "1.0")));
            Assert.IsTrue(
                descriptor.Match(new ComponentDescriptor(Category.Controllers, "pip-services-dummies", "default", "1.0")));

            // Check match by special BusinessLogic category
            Assert.IsTrue(descriptor.Match(new ComponentDescriptor(Category.BusinessLogic, null, null, null)));

            // Check mismatch by individual fields
            Assert.IsFalse(descriptor.Match(new ComponentDescriptor(Category.Cache, null, null, null)));
            Assert.IsFalse(
                descriptor.Match(new ComponentDescriptor(Category.Controllers, "pip-services-runtime", null, null)));
            Assert.IsFalse(descriptor.Match(new ComponentDescriptor(Category.Controllers, null, "special", null)));
            Assert.IsFalse(descriptor.Match(new ComponentDescriptor(Category.Controllers, null, null, "2.0")));
        }

        [TestMethod]
        public void TestToString()
        {
            var descriptor1 = new ComponentDescriptor(Category.Controllers, "pip-services-dummies", "default", "1.0");
            Assert.AreEqual("controllers/pip-services-dummies/default/1.0", descriptor1.ToString());

            var descriptor2 = new ComponentDescriptor(Category.Controllers, null, null, null);
            Assert.AreEqual("controllers/*/*/*", descriptor2.ToString());
        }
    }
}