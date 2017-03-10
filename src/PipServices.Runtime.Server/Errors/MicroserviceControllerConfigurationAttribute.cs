using System;
using System.Web.Http.Controllers;
using System.Web.Http.ExceptionHandling;

namespace PipServices.Runtime.Errors
{
    public sealed class MicroserviceControllerConfigurationAttribute : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Services.Replace(typeof(IExceptionHandler), new MicroserviceExceptionHandler());
        }
    }
}
