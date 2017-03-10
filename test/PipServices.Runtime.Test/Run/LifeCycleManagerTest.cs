using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipServices.Runtime.Config;
using PipServices.Runtime.Counters;
using PipServices.Runtime.Logs;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Run
{
    [TestClass]
    public class LifeCycleManagerTest
    {
        private DynamicMap context;
        private ComponentSet components;

        [TestInitialize]
        public void TestInitialize()
        {
            ILogger log = new NullLogger();
            log.Configure(new ComponentConfig());

            ICounters counters = new NullCounters();
            counters.Configure(new ComponentConfig());

            context = new DynamicMap();
            components = ComponentSet.FromComponents(log, counters);
        }

        [TestMethod]
        public void TestLink()
        {
            LifeCycleManager.Link(context, components);
        }

        [TestMethod]
        public void TestLinkAndOpen()
        {
            LifeCycleManager.LinkAndOpen(context, components);
        }

        [TestMethod]
        public void TestOpen()
        {
            LifeCycleManager.Link(context, components);
            LifeCycleManager.Open(components);
        }

        [TestMethod]
        public void TestClose()
        {
            LifeCycleManager.LinkAndOpen(context, components);
            LifeCycleManager.Close(components);
        }

        [TestMethod]
        public void TestForceClose()
        {
            LifeCycleManager.LinkAndOpen(context, components);
            LifeCycleManager.ForceClose(components);
        }
    }
}