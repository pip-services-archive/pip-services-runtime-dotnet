using System.Runtime.Serialization;

namespace PipServices.Runtime.Data
{
    [DataContract]
    public class PartialUpdates : IExtensibleDataObject
    {
        public ExtensionDataObject ExtensionData { get; set; }
    }
}