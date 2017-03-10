using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipServices.Runtime.Run
{
    public enum ServiceFabricMicroserviceType
    {
        None,
        StatelessService,
        StatefulService,
        Actor
    }
}
