using System.Web.Http.Controllers;

namespace PipServices.Runtime.Services
{
    public interface IHttpLogicController<T> : IHttpController
        where T : IBusinessLogic
    {
        T Logic { get; set; }
    }
}