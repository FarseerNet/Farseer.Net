using System.Xml.Serialization;

namespace FS.Log.Entity
{
    public class SqlParam
    {
        [XmlAttribute]
        public string Name { get; set; }
        public string Value { get; set; }
    }
}