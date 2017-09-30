using System.Runtime.Serialization;

namespace Farseer.Net.Data.Log.Default.Entity
{
    [DataContract]
    public class SqlParam
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }
    }
}