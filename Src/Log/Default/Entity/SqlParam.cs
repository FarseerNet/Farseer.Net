using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FS.Log.Default.Entity
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