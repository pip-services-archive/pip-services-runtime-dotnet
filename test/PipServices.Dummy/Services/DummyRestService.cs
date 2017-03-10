using PipServices.Dummy.Logic;
using PipServices.Runtime;
using PipServices.Runtime.Config;
using PipServices.Runtime.Portability;
using PipServices.Runtime.Services;

namespace PipServices.Dummy.Services
{
    public class DummyRestService : RestService<DummyWebApiController, IDummyBusinessLogic>
    {
        public static readonly ComponentDescriptor ClassDescriptor = new ComponentDescriptor(
            Category.Services, "pip-services-dummies", "rest", "1.0"
            );

        public DummyRestService()
            : base(ClassDescriptor)
        {
        }

        public override void Link(DynamicMap context, ComponentSet components)
        {
            base.Link(context, components);

            Logic = (IDummyBusinessLogic) components.GetOnePrior(
                this, new ComponentDescriptor(Category.BusinessLogic, "pip-services-dummies", "*", "*")
            );
        }
    }
}