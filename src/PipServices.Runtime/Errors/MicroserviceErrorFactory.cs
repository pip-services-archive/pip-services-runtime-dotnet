using System;
using System.Runtime.Remoting;

namespace PipServices.Runtime.Errors
{
    public sealed class MicroserviceErrorFactory
    {
        public static Exception Create(FaultData data)
        {
            MicroserviceError error = null;
            if (data.Category.Contains("BadRequestError"))
                error = new BadRequestError(data.Code, data.Message);
            else if (data.Category.Contains("BuildError"))
                error = new BuildError(data.Code, data.Message);
            else if (data.Category.Contains("CallError"))
                error = new CallError(data.Code, data.Message);
            else if (data.Category.Contains("ConfigError"))
                error = new ConfigError(data.Code, data.Message);
            else if (data.Category.Contains("ConflictError"))
                error = new ConflictError(data.Code, data.Message);
            else if (data.Category.Contains("ConnectionError"))
                error = new ConnectionError(data.Code, data.Message);
            else if (data.Category.Contains("FileError"))
                error = new FileError(data.Code, data.Message);
            else if (data.Category.Contains("NotFoundError"))
                error = new NotFoundError(data.Code, data.Message);
            else if (data.Category.Contains("StateError"))
                error = new StateError(data.Code, data.Message);
            else if (data.Category.Contains("UnauthorizedError"))
                error = new UnauthorizedError(data.Code, data.Message);
            else if (data.Category.Contains("UnknownError"))
                error = new UnknownError(data.Code, data.Message);
            else if (data.Category.Contains("UnsupportedError"))
                error = new UnsupportedError(data.Code, data.Message);

            if (error == null)
                return new ServerException(data.Message, data.Cause);

            error.Component = data.Component;
            error.CorrelationId = data.CorrelationId;

            return error;
        }
    }
}
