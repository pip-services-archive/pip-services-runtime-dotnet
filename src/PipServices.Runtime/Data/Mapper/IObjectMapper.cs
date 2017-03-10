namespace PipServices.Runtime.Data.Mapper
{
    public interface IObjectMapper
    {
        TD Transfer<TS, TD>(TS source)
            where TS : class
            where TD : class;
    }
}