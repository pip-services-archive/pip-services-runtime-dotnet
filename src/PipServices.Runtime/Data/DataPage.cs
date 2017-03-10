using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PipServices.Runtime.Data
{
    [DataContract]
    public class DataPage<T>
    {
        public DataPage()
        {
            Data = new List<T>();
        }

        public DataPage(int? total, IEnumerable<T> items)
        {
            Total = total;
            Data = new List<T>(items);
        }

        [DataMember(Name = "total", IsRequired = false)]
        public int? Total { get; set; }

        [DataMember(Name = "data", IsRequired = true)]
        public IEnumerable<T> Data { get; set; }
    }
}