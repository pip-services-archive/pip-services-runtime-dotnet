using PipServices.Runtime.Config;

namespace PipServices.Runtime.Boot
{
    public abstract class AbstractBootConfig : AbstractComponent, IBootConfig
    {
        protected AbstractBootConfig(ComponentDescriptor descriptor)
            : base(descriptor)
        {
        }

        public abstract MicroserviceConfig ReadConfig();
    }
}